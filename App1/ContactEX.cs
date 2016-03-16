﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Contacts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace App1
{
   public  class ContactEX
    {

        public Contact Contact { get; }

        public ContactEX(Contact contact)
        {
            Contact = contact;
        }

        public ContactEX(string firstName, string lastName)
        {
            Contact = new Contact
            {
                FirstName = firstName,
                LastName = lastName
            };
        }

        public string Initials => GetFirstCharacter (Contact.FirstName) + GetFirstCharacter (Contact.LastName);

        private string GetFirstCharacter(string s)=> string.IsNullOrEmpty(s) ? "" : s.Substring(0,1);
        public Visibility EmailVisibility =>
            DesignMode.DesignModeEnabled || Contact.Emails.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        public ImageBrush Picture {

            get {
                if (Contact.SmallDisplayPicture == null)
                {
                    return null;  
                }
                var image = new BitmapImage();
                image.SetSource(Contact.SmallDisplayPicture.OpenReadAsync().GetAwaiter().GetResult());
                return new ImageBrush { ImageSource = image };
            }


        }
    }
}
