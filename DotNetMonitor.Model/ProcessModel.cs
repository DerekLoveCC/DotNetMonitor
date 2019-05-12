using System;
using System.Collections.Generic;

namespace DotNetMonitor.Model
{
    public class ProcessModel
    {
        public string Name { get; set; }
        public IList<ClrModel> ClrModels { get; set; } = new List<ClrModel>();

        public void AddClrModel(ClrModel clrModel)
        {
            ClrModels.Add(clrModel);
        }
    }
}