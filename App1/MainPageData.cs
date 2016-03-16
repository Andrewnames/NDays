using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Appointments;
using Windows.ApplicationModel.Contacts;
using Windows.ApplicationModel.Email;
using Common;

namespace App1
{
    public class MainPageData : ObservableObject
    {
        private string _greeting;

        private List<Nameday> _allNamedays = new List<Nameday>();
        public AddRemidnerCommand AddReminderCommand { get; }

        public string Greeting
        {
            get { return _greeting; }
            set {
                Set(ref _greeting, value);          
                            }
        }

        private string _filter;

        public string Filter
        {
            get { return _filter; }
            set
            {
                Set(ref _filter, value);
                Filtering();
            }
        }

        public ObservableCollection<ContactEX> Contacts { get; } =
            new ObservableCollection<ContactEX>();
        public ObservableCollection<Nameday> NameDays { get; set; }
        public MainPageData()
        {
            AddReminderCommand = new AddRemidnerCommand(this);
            NameDays = new ObservableCollection<Nameday>();
            if (DesignMode.DesignModeEnabled)
            {

                Contacts = new ObservableCollection<ContactEX> {

                    new ContactEX ("Contact","1" ),
                    new ContactEX ("Contact","2" ),
                                        new ContactEX ("Contact","3" )
    };

                for (int i = 1; i <= 12; i++)
                {
                    _allNamedays.Add(new Nameday(i, 1, new string[] { "Adam" }));
                    _allNamedays.Add(new Nameday(i, 24, new string[] { "Eve", "Andrew" }));

                }
                Filtering();
            }
            else LoadData();
        }

        private async void LoadData()
        {
            _allNamedays = await NamedaysRepository.GetAllNamedaysAsync();
            Filtering();
        }

        private void Filtering()
        {

            if (_filter == null)
            {
                _filter = "";
            }

            var lowerCaseFilter = Filter.ToLowerInvariant().Trim();
            var result = _allNamedays.Where(d => d.NameAsString.ToLowerInvariant()
                                                 .Contains(lowerCaseFilter)).ToList();

            var toRemove = NameDays.Except(result).ToList();

            toRemove.ForEach(d =>
            {

                NameDays.Remove(d);

            });

            var resultCount = result.Count;
            for (int i = 0; i < resultCount; i++)
            {
                var resultItem = result[i];
                if (i + 1 > NameDays.Count || !NameDays[i].Equals(resultItem))
                {
                    NameDays.Insert(i, resultItem);
                }
            }



        }

        private Nameday _selectedNameday;
        public Settings Settings { get; } = new Settings();
        public Nameday SelectedNameday
        {
            get { return _selectedNameday; }
            set
            {
                _selectedNameday = value;
                if (value == null)
                {
                    Greeting = "Hello";
                }

                else
                {

                    Greeting = "Hello " + value.NameAsString;
                }
                UpdateContacts();
                AddReminderCommand.FireCanExecuteChanged();

            }
        }

        private async void UpdateContacts()
        {
            Contacts.Clear();
            if (SelectedNameday != null)
            {
                var contactStore = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AllContactsReadOnly);
                foreach (var name in SelectedNameday.Names)
                {
                    foreach (var contact in await contactStore.FindContactsAsync(name))
                    {
                        Contacts.Add(new ContactEX(contact));
                    }
                }
            }
        }

        public async Task SendEmailAsync(Contact contact)
        {
            if (contact == null || contact.Emails.Count == 0) return;
            var msg = new EmailMessage();
            msg.To.Add(new EmailRecipient(contact.Emails[0].Address));
            msg.Subject = "Happy nameday";
            await EmailManager.ShowComposeNewEmailAsync(msg);
        }


        public async void AddReminderToCalendarAsync()
        {
            var appointment = new Appointment();
            appointment.Subject = "Reminder for " + SelectedNameday.NameAsString;
            appointment.AllDay = true;
            appointment.BusyStatus = AppointmentBusyStatus.Free;
            var dateThisYear = new DateTime(DateTime.Now.Year, SelectedNameday.Month, SelectedNameday.Day);
            appointment.StartTime = dateThisYear < DateTime.Now ? dateThisYear.AddYears(1) : dateThisYear;
            await AppointmentManager.ShowEditNewAppointmentAsync(appointment);
        }
    }

    public class AddRemidnerCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MainPageData _mpd;
        public AddRemidnerCommand(MainPageData mpd)
        {
            _mpd = mpd;
        }
        public bool CanExecute(object parameter)
        {
            return _mpd.SelectedNameday != null;
        }

        public void Execute(object parameter)
        {
            _mpd.AddReminderToCalendarAsync();
        }

        public void FireCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
