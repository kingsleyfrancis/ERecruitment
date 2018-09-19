using System;
using System.Collections.Generic;

namespace ERecruitment.Core.Data
{
    public class DataSettings
    {
        public DataSettings()
        {
            RawDataSettings = new Dictionary<string, string>();
        }

        public string DataProvider { get; set; }
        public string DataConnectionString { get; set; }
        public IDictionary<string, string> RawDataSettings { get; private set; }

        public bool IsValid()
        {
            return !String.IsNullOrEmpty(DataProvider) && !String.IsNullOrEmpty(DataConnectionString);
        }
    }
}