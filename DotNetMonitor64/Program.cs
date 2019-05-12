using DotNetMonitor.Common;
using DotNetMonitor.Model;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DotNetMonitor64
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var processId = args[0].ToNullableInt();
            if (processId == null)
            {
                processId = Process.GetProcessesByName(args[0]).FirstOrDefault()?.Id;
            }
            if (processId == null)
            {
                return;
            }
            //Test(processId.Value);
            var processInfoCollector = new ProcessInfoCollector(processId.Value);
            processInfoCollector.Collect();
        }

        private static void Test(int processId)
        {
            var processModel = new ProcessModel();

            using (DataTarget dataTarget = DataTarget.AttachToProcess(processId, 500, AttachFlag.NonInvasive))
            {
                foreach (var clrVersion in dataTarget.ClrVersions)
                {
                    var runtime = clrVersion.CreateRuntime();
                    //PrintAppDomains(runtime, sw);
                    //PrintModules(runtime, sw);
                    //PrintThreadS(runtime, sw);
                    //PrintSegments(runtime, sw);
                    //PrintGCHandles(runtime, sw);
                    //PrintHeapSegments(runtime, sw);
                    //PrintLogicHeapBalance(runtime, sw);
                    //PrintManagedObjectsBySegment(runtime, sw);
                    //PrintManagedObjects(runtime, sw);
                }
            }
        }

        private static void PrintManagedObjects(ClrRuntime runtime)
        {
            if (!runtime.Heap.CanWalkHeap)
            {
                Console.WriteLine("Cannot walk the heap!");
            }
            else
            {
                foreach (ulong obj in runtime.Heap.EnumerateObjectAddresses())
                {
                    ClrType type = runtime.Heap.GetObjectType(obj);

                    // If heap corruption, continue past this object.
                    if (type == null)
                    {
                        continue;
                    }

                    ulong size = type.GetSize(obj);
                    LogHelper.Logger.InfoFormat("{0,12:X} {1,8:n0} {2,1:n0} {3}", obj, size, runtime.Heap.GetGeneration(obj), type.Name);
                }
            }
        }

        private static void PrintManagedObjectsBySegment(ClrRuntime runtime)
        {
            if (!runtime.Heap.CanWalkHeap)
            {
                LogHelper.Logger.InfoFormat("Cannot walk the heap!");
            }
            else
            {
                foreach (ClrSegment seg in runtime.Heap.Segments)
                {
                    for (ulong obj = seg.FirstObject; obj != 0; obj = seg.NextObject(obj))
                    {
                        ClrType type = runtime.Heap.GetObjectType(obj);

                        // If heap corruption, continue past this object.
                        if (type == null)
                        {
                            continue;
                        }

                        ulong size = type.GetSize(obj);
                        LogHelper.Logger.InfoFormat("{0,12:X} {1,8:n0} {2,1:n0} {3}", obj, size, seg.GetGeneration(obj), type.Name);
                    }
                }
            }
        }

        private static void PrintLogicHeapBalance(ClrRuntime runtime, StreamWriter sw)
        {
            foreach (var item in (from seg in runtime.Heap.Segments
                                  group seg by seg.ProcessorAffinity into g
                                  orderby g.Key
                                  select new
                                  {
                                      Heap = g.Key,
                                      Size = g.Sum(p => (uint)p.Length)
                                  }))
            {
                sw.WriteLine("Heap {0,2}: {1:n0} bytes", item.Heap, item.Size);
            }
        }

        private static void PrintHeapSegments(ClrRuntime runtime, StreamWriter sw)
        {
            sw.WriteLine("{0,12} {1,12} {2,12} {3,12} {4,4} {5}", "Start", "End", "CommittedEnd", "ReservedEnd", "Heap", "Type");
            foreach (ClrSegment segment in runtime.Heap.Segments)
            {
                string type;
                if (segment.IsEphemeral)
                {
                    type = "Ephemeral";
                }
                else if (segment.IsLarge)
                {
                    type = "Large";
                }
                else
                {
                    type = "Gen2";
                }

                sw.WriteLine($"{segment.Start,12:X} {segment.End,12:X} {segment.CommittedEnd,12:X} {segment.ReservedEnd,12:X} {segment.ProcessorAffinity,4} {type}");
            }
        }

        private static void PrintGCHandles(ClrRuntime runtime, StreamWriter sw)
        {
            foreach (ClrHandle handle in runtime.EnumerateHandles())
            {
                string objectType = runtime.Heap.GetObjectType(handle.Object).Name;
                sw.WriteLine("{0,12:X} {1,12:X} {2,12} {3}", handle.Address, handle.Object, handle.HandleType, objectType);
            }
        }

        private static void PrintSegments(ClrRuntime runtime, StreamWriter sw)
        {
            var regions = (from r in runtime.EnumerateMemoryRegions()
                           where r.Type != ClrMemoryRegionType.ReservedGCSegment
                           group r by r.Type into g
                           let total = g.Sum(p => (uint)p.Size)
                           orderby total descending
                           select new
                           {
                               TotalSize = total,
                               Count = g.Count(),
                               Type = g.Key
                           });

            foreach (var region in regions)
            {
                sw.WriteLine("{0,6:n0} {1,12:n0} {2}", region.Count, region.TotalSize, region.Type);
            }
        }

        private static void PrintThreadS(ClrRuntime runtime, StreamWriter sw)
        {
            foreach (ClrThread thread in runtime.Threads)
            {
                if (!thread.IsAlive)
                {
                    continue;
                }

                sw.WriteLine("Thread {0:X}:", thread.OSThreadId);
                foreach (ClrStackFrame frame in thread.StackTrace)
                {
                    sw.WriteLine("{0,12:X} {1,12:X} {2}", frame.StackPointer, frame.InstructionPointer, frame);
                }

                sw.WriteLine();
            }
        }

        private static void PrintModules(ClrRuntime runtime, StreamWriter sw)
        {
            foreach (ClrAppDomain domain in runtime.AppDomains)
            {
                foreach (ClrModule module in domain.Modules)
                {
                    sw.WriteLine($"Domain: {domain.Id}-{domain.Name} Module: {module.Name}");
                }
            }
        }

        private static void PrintAppDomains(ClrRuntime runtime, StreamWriter sw)
        {
            foreach (ClrAppDomain domain in runtime.AppDomains)
            {
                sw.WriteLine("ID:      {0}", domain.Id);
                sw.WriteLine("Name:    {0}", domain.Name);
                sw.WriteLine("Address: {0}", domain.Address);
            }
        }

        private static void ObjSize(ClrHeap heap, ulong obj, out uint count, out ulong size)
        {
            // Evaluation stack
            Stack<ulong> eval = new Stack<ulong>();

            // To make sure we don't count the same object twice, we'll keep a set of all objects
            // we've seen before.  Note the ObjectSet here is basically just "HashSet<ulong>".
            // However, HashSet<ulong> is *extremely* memory inefficient.  So we use our own to
            // avoid OOMs.
            ObjectSet considered = new ObjectSet(heap);

            count = 0;
            size = 0;
            eval.Push(obj);

            while (eval.Count > 0)
            {
                // Pop an object, ignore it if we've seen it before.
                obj = eval.Pop();
                if (considered.Contains(obj))
                {
                    continue;
                }

                considered.Add(obj);

                // Grab the type. We will only get null here in the case of heap corruption.
                ClrType type = heap.GetObjectType(obj);
                if (type == null)
                {
                    continue;
                }

                count++;
                size += type.GetSize(obj);

                // Now enumerate all objects that this object points to, add them to the
                // evaluation stack if we haven't seen them before.
                type.EnumerateRefsOfObject(obj, delegate (ulong child, int offset)
                {
                    if (child != 0 && !considered.Contains(child))
                    {
                        eval.Push(child);
                    }
                });
            }
        }

        private static void PrintClassInterfaces(ClrRuntime runtime, ClrType type, StreamWriter sw)
        {
            sw.WriteLine("Type {0} implements interfaces:", type);

            foreach (ClrInterface inter in type.Interfaces)
            {
                sw.WriteLine("    {0}", inter.Name);
            }
        }

        public static void WriteFields(ulong obj, ClrType type)
        {
            WriteFieldsWorker(obj, type, null, 0, false);
        }

        private static void WriteFieldsWorker(ulong obj, ClrType type, string baseName, int offset, bool inner)
        {
            // Keep track of nested fields.
            if (baseName == null)
                baseName = "";
            else
                baseName += ".";

            foreach (var field in type.Fields)
            {
                ulong addr = field.GetAddress(obj, inner);

                string output;
                if (field.HasSimpleValue)
                    output = field.GetValue(obj, inner).ToString();
                else
                    output = addr.ToString("X");

                Console.WriteLine("  +{0,2:X2} {1} {2}{3} = {4}", field.Offset + offset, field.Type.Name, baseName, field.Name, output);

                // Recurse for structs.
                if (field.ElementType == ClrElementType.Struct)
                    WriteFieldsWorker(addr, field.Type, baseName + field.Name, offset + field.Offset, true);
            }
        }

        private static void PrintArray(ClrRuntime runtime)

        {
            var heap = runtime.Heap;

            var intArrays = from o in heap.EnumerateObjects()
                            let t = heap.GetObjectType(o)
                            where t.IsArray && t.Name == "System.Int32[]"
                            select new { Address = o, Type = t };

            foreach (var item in intArrays)
            {
                ClrType type = item.Type;
                ulong obj = item.Address;

                int len = type.GetArrayLength(obj);

                Console.WriteLine("Object: {0:X}", obj);
                Console.WriteLine("Length: {0}", len);
                Console.WriteLine("Elements:");

                for (int i = 0; i < len; i++)
                    Console.WriteLine("{0,3} - {1}", i, type.GetArrayElementValue(obj, i));
            }
        }

        private static void PrintArrayOfStruct(ClrRuntime runtime)
        {
            var heap = runtime.Heap;
            foreach (ulong obj in heap.EnumerateObjects())
            {
                var type = heap.GetObjectType(obj);

                // Only consider types which are arrays that do not have simple values (I.E., are structs).
                if (!type.IsArray || type.ComponentType.HasSimpleValue)
                    continue;

                int len = type.GetArrayLength(obj);

                Console.WriteLine("Object: {0:X}", obj);
                Console.WriteLine("Type:   {0}", type.Name);
                Console.WriteLine("Length: {0}", len);
                for (int i = 0; i < len; i++)
                {
                    ulong addr = type.GetArrayElementAddress(obj, i);
                    foreach (var field in type.ComponentType.Fields)
                    {
                        string output;
                        if (field.HasSimpleValue)
                            output = field.GetValue(addr, true).ToString();        // <- true here, as this is an embedded struct
                        else
                            output = field.GetAddress(addr, true).ToString("X");   // <- true here as well

                        Console.WriteLine("{0}  +{1,2:X2} {2} {3} = {4}", i, field.Offset, field.Type.Name, field.Name, output);
                    }
                }
            }
        }

        private static void PrintEnums(ClrRuntime runtime)
        {
            var heap = runtime.Heap;
            // This walks the heap looking for enums, but keep in mind this works for field
            // values which are enums too.
            foreach (ulong obj in heap.EnumerateObjects())
            {
                var type = heap.GetObjectType(obj);
                if (!type.IsEnum)
                    continue;

                // Enums do not have to be ints!  We will only handle the int case here.
                if (type.GetEnumElementType() != ClrElementType.Int32)
                    continue;

                int objValue = (int)type.GetValue(obj);

                bool found = false;
                foreach (var name in type.GetEnumNames())
                {
                    int value;
                    if (type.TryGetEnumValue(name, out value) && objValue == value)
                    {
                        Console.WriteLine("{0} - {1}", value, name);
                        found = true;
                        break;
                    }
                }

                if (!found)
                    Console.WriteLine("{0} - {1}", objValue, "Unknown");
            }
        }

        private static void PrintExceptions(ClrRuntime runtime)
        {
            var heap = runtime.Heap;
            foreach (ulong obj in heap.EnumerateObjects())
            {
                var type = heap.GetObjectType(obj);
                if (!type.IsException)
                    continue;

                var ex = heap.GetExceptionObject(obj);
                Console.WriteLine(ex.Type.Name);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.HResult.ToString("X"));

                foreach (var frame in ex.StackTrace)
                    Console.WriteLine(frame.ToString());
            }
        }

        private static void PrintAppDomainStaticObjects(ClrRuntime runtime)
        {
            var heap = runtime.Heap;
            foreach (var type in heap.EnumerateTypes())
            {
                foreach (var appDomain in runtime.AppDomains)
                {
                    foreach (var field in type.StaticFields)
                    {
                        if (field.HasSimpleValue)
                            Console.WriteLine("{0}.{1} ({2}) = {3}", type.Name, field.Name, appDomain.Id, field.GetValue(appDomain));
                    }
                }
            }
        }

        private static void PrintThreadStaticObjects(ClrRuntime runtime)
        {
            var heap = runtime.Heap;
            foreach (var type in heap.EnumerateTypes())
            {
                foreach (var appDomain in runtime.AppDomains)
                {
                    foreach (var thread in runtime.Threads)
                    {
                        foreach (var field in type.ThreadStaticFields)
                        {
                            if (field.HasSimpleValue)
                            {
                                Console.WriteLine("{0}.{1} ({2}, {3:X}) = {4}", type.Name, field.Name, appDomain.Id, thread.OSThreadId, field.GetValue(appDomain, thread));
                            }
                        }
                    }
                }
            }
        }
    }
}