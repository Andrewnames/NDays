using System;
using System.Globalization;
using Windows.Storage;

namespace Common
{
    public class Settings : ObservableObject
    {
        public Settings()
        {
            LoadSettings();
        }

        private bool _notificationsEnabled;

        public bool NotificationsEnabled
        {
            get { return _notificationsEnabled; }
            set
            {
                if (Set(ref _notificationsEnabled, value))
                    _localSettings.Values[nameof(NotificationsEnabled)] = value;
            }
        }


        private bool _updatingLiveTileEnabled;

        public bool UpdatingLiveTileEnabled
        {
            get { return _updatingLiveTileEnabled; }
            set {

                if (Set(ref _updatingLiveTileEnabled, value))
                    _localSettings.Values[nameof(UpdatingLiveTileEnabled)] = value;


            }
        }


        private DateTime _lastSuccessfulRun;

        public DateTime LastSuccessfulRun
        {
            get { return _lastSuccessfulRun; }
            set
            {
                if (Set(ref _lastSuccessfulRun, value))
                    _localSettings.Values[nameof(LastSuccessfulRun)] = value;                
            }
        }


        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;


        private void LoadSettings()
        {
            var notificationEnabled = _localSettings.Values[nameof(NotificationsEnabled)];
            if (notificationEnabled != null)
            {
                NotificationsEnabled = (bool)notificationEnabled;
            }
            var updatingLiveTileEnabled = _localSettings.Values[nameof(UpdatingLiveTileEnabled)];
            if (updatingLiveTileEnabled != null)
            {
                UpdatingLiveTileEnabled = (bool)updatingLiveTileEnabled;
            }
            var lastSuccessfulRun = _localSettings.Values[nameof(LastSuccessfulRun)];
            if (lastSuccessfulRun != null)
            {
                LastSuccessfulRun = DateTime.Parse(lastSuccessfulRun.ToString(), CultureInfo.InvariantCulture);
            }
        }
    }
}
