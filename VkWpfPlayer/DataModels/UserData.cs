using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VkWpfPlayer.DataModels
{
    public class UserData : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public long Id { get; set; }
        public string Token { get; set; }
        private string avatarurl;
        public string AvatarUrl { get { return avatarurl; } set { avatarurl = value; OnPropertyChanged("AvatarUrl"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }
}
