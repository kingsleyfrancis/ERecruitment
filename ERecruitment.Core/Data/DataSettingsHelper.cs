using System;

namespace ERecruitment.Core.Data
{
    public class DataSettingsHelper
    {
        private static bool? _databaseIsInstalled;

        public static bool DatabaseIsInstalled()
        {
            if (!_databaseIsInstalled.HasValue)
            {
                var manager = new DataSettingsManager();
                DataSettings settings = manager.LoadSettings();
                _databaseIsInstalled = settings != null && !String.IsNullOrEmpty(settings.DataConnectionString);
            }
            return _databaseIsInstalled.Value;
        }

        public static void ResetCache()
        {
            _databaseIsInstalled = null;
        }
    }
}