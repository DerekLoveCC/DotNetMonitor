using DotNetMonitor.Model;
using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;

namespace DotNetMonitor64
{
    public class ProcessInfoCollector
    {
        private readonly int processId;

        public ProcessInfoCollector(int processId)
        {
            this.processId = processId;
        }

        internal ProcessModel Collect()
        {
            var processModel = new ProcessModel();

            using (DataTarget dataTarget = DataTarget.AttachToProcess(processId, 500, AttachFlag.NonInvasive))
            {
                foreach (ClrInfo clrInfo in dataTarget.ClrVersions)
                {
                    ClrModel clrModel = BuildClrModel(clrInfo);
                    processModel.AddClrModel(clrModel);
                }
            }

            return processModel;
        }

        private static ClrModel BuildClrModel(ClrInfo clrInfo)
        {
            var runtime = clrInfo.CreateRuntime();
            var clrModel = new ClrModel
            {
                Version = CreateVersion(clrInfo),
                PointerSize = runtime.PointerSize,
                ServerGC = runtime.ServerGC,
                HeapCount = runtime.HeapCount,
                DacLocation = runtime.ClrInfo.LocalMatchingDac,
                ClrObjects = BuildClrObjects(runtime),
            };

            //PrintAppDomains(runtime, sw);
            //PrintModules(runtime, sw);
            //PrintThreadS(runtime, sw);
            //PrintSegments(runtime, sw);
            //PrintGCHandles(runtime, sw);
            //PrintHeapSegments(runtime, sw);
            //PrintLogicHeapBalance(runtime, sw);
            //PrintManagedObjectsBySegment(runtime, sw);
            //PrintManagedObjects(runtime, sw);
            return clrModel;
        }

        private static IList<ClrObjectModel> BuildClrObjects(ClrRuntime runtime)
        {
            var clrObjects = new List<ClrObjectModel>();

            TraverseManagedObjectsBySegment(runtime, clrObjects);

            return clrObjects;
        }

        private static Version CreateVersion(ClrInfo clrInfo)
        {
            return new Version(clrInfo.Version.Major,
                               clrInfo.Version.Minor,
                               clrInfo.Version.Patch,
                               clrInfo.Version.Revision);
        }

        private static void TraverseManagedObjects(ClrRuntime runtime, IList<ClrObjectModel> clrObjects)
        {
            if (!runtime.Heap.CanWalkHeap)
            {
                Console.WriteLine("Cannot walk the heap!");
                return;
            }

            foreach (ulong obj in runtime.Heap.EnumerateObjectAddresses())
            {
                ClrType type = runtime.Heap.GetObjectType(obj);

                // If heap corruption, continue past this object.
                if (type == null)
                {
                    continue;
                }

                ulong size = type.GetSize(obj);
                clrObjects.Add(new ClrObjectModel
                {
                    Gen = runtime.Heap.GetGeneration(obj),
                    Size = size,
                    TypeName = type.Name,
                });
            }
        }

        private static void TraverseManagedObjectsBySegment(ClrRuntime runtime, IList<ClrObjectModel> clrObjects)
        {
            if (!runtime.Heap.CanWalkHeap)
            {
                Console.WriteLine("Cannot walk the heap!");
                return;
            }

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
                    clrObjects.Add(new ClrObjectModel
                    {
                        Gen = seg.GetGeneration(obj),
                        Size = size,
                        TypeName = type.Name,
                        InnerId = obj,
                    });
                }
            }
        }
    }
}