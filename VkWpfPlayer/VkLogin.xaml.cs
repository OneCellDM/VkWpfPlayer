using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using VkNet.Enums.Filters;

namespace VkWpfPlayer
{
    /// <summary>
    /// Логика взаимодействия для VkLogin.xaml
    /// </summary>

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
    public partial class VkLogin : Page
    {
        private bool twoAuthorize = false;
        private bool codeSended = false;

        private Border PreviewErrorBorder;
        public VkLogin()
        {
            Uri AuthUrl = new Uri(
                "https://oauth.vk.com/authorize?client_id=6121396&scope=1073737727&redirect_uri=https://oauth.vk.com/blank.html&display=page&response_type=token&revoke=1"
            );

            
            this.InitializeComponent();
            AuthErrorBorder.Visibility = Visibility.Collapsed;
            SaveAuthDataCheckBox.Visibility = Visibility.Collapsed;

            TwoFactorCodeTextBox.Visibility = Visibility.Hidden;
            showAccountList();
            ShowActiveAccounts();
        }
        private void showAccountList()
        {
            LoginPanel.Visibility = Visibility.Hidden;
            LoginBorder.Visibility = Visibility.Hidden;
            AccountListPanelBorder.Visibility = Visibility.Visible;

        }
        
        private void HideAccountList()
        {
            LoginBorder.Visibility = Visibility.Visible;
            LoginPanel.Visibility = Visibility.Visible;
            AccountListPanelBorder.Visibility = Visibility.Hidden;
        }
        private void HideLoginComponents()
        {
            
            TwoFactorCodeTextBox.Visibility = Visibility.Hidden;
            LoginLabel.Visibility = Visibility.Collapsed;
            LoginTextBox.Visibility = Visibility.Collapsed;
            PasswordTextbox.Visibility = Visibility.Collapsed;
        }
        private void HideTwoAuthComponents()
        {
            AuthButton.Content = "Авторизироваться ";
            TwoFactorCodeTextBox.Visibility = Visibility.Hidden;
            LoginLabel.Visibility = Visibility.Visible;
            PasswordLabel.Content = "пароль";
            LoginTextBox.Visibility = Visibility.Visible;
            PasswordTextbox.Visibility = Visibility.Visible;
        }

        private void ShowTwoAuthComponents()
        {
            AuthButton.Content = "Отправить код авторизации";
            TwoFactorCodeTextBox.Visibility = Visibility.Visible;
            PasswordLabel.Content = "Введите код";
            LoginLabel.Visibility = Visibility.Hidden;
            LoginTextBox.Visibility = Visibility.Hidden;
            PasswordTextbox.Visibility = Visibility.Hidden;
        }
        private void ShowActiveAccounts()
        {
            AccountList.Items.Clear();
            List<long> Ids = new List<long>();

            foreach (var account in GetSavedAuthData())
            {
                long lgp = long.Parse(account["ID"].ToString());
                Ids.Add(lgp);
                Debug.WriteLine(lgp);
                AccountList.Items.Add(new UserData
                {
                    Id = lgp,
                    Token = account["Token"].ToString(),
                    Name = account["Name"].ToString(),

                });
            }
            var api = new VkNet.VkApi();
            api.Authorize(new VkNet.Model.ApiAuthParams() { AccessToken = "4b0168fd4b0168fd4b0168fd8f4b676c6744b014b0168fd1093d8fdf1e3c0017422a04c" });
            var data = api.Users.Get(Ids, ProfileFields.Photo50);

            for (int i = 0; i < AccountList.Items.Count; i++)
            {
                if (data[i].Photo50 != null)
                    ((UserData)AccountList.Items[i]).AvatarUrl = data[i].Photo50.AbsoluteUri;
            }

            Ids.Clear();


        }
        private List<JObject> GetSavedAuthData()
        {
            List<JObject> objects = new List<JObject>();
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey("VkPlayerByOneCellDM");
            foreach (var val in key.GetValueNames())
            {
                if (val != "Settings")
                    try
                    {
                        objects.Add(JObject.Parse(key.GetValue(val).ToString()));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
            }
            return objects;
        }

        private void SaveAuthData(VkNet.VkApi api)
        {
            var AccountInfo = api.Account.GetProfileInfo();


            var data = new JObject {

                        { "ID", api.UserId },
                        { "Name", AccountInfo.FirstName +" "+ AccountInfo.LastName },
                        { "Token", api.Token }
                    };

            var kay = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey("VkPlayerByOneCellDM");
            kay.SetValue(api.UserId.ToString(), data);
            kay.Close();

        }


        private void TwoFactorAuth()
        {
            var api = new VkNet.VkApi();
            if (twoAuthorize == false)
            {
                var Awaiter = api.AuthorizeAsync(new VkNet.Model.ApiAuthParams()
                {
                    Password = PasswordTextbox.Text,
                    Login = LoginTextBox.Text,
                    TwoFactorSupported = true,
                    ApplicationId = 6121396,
                    Settings = Settings.Audio | Settings.Friends | Settings.Offline,
                    TwoFactorAuthorization = () =>
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            ShowTwoAuthComponents();
                        });
                        string code = "";
                        try
                        {
                            twoAuthorize = true;
                            while (codeSended != true)
                            { Thread.Sleep(1000); }
                            this.Dispatcher.Invoke(() =>
                            {
                                codeSended = false;
                                code = TwoFactorCodeTextBox.Text;
                                TwoFactorCodeTextBox.Text = "";
                            });
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                        }
                        return code;
                    }
                }).GetAwaiter();
                Awaiter.OnCompleted(() =>
                {
                    try
                    {

                        Awaiter.GetResult();
                        if (api.IsAuthorized == true)
                        {
                            SaveAuthData(api);
                            ShowActiveAccounts();
                            showAccountList();

                        }

                    }
                    catch (VkNet.Exception.VkAuthorizationException ex) 
                    {

                        this.PreviewErrorBorder = LoginBorder;
                        this.PreviewErrorBorder.Visibility = Visibility.Collapsed;
                        this.AuthErrorBorder.Visibility = Visibility.Visible;

                        
                        this.AuthErrorTextRun.Text = ex.Message;


                        Debug.WriteLine(ex.StackTrace);
                    }
                    catch(VkNet.Exception.UserAuthorizationFailException ex)
                    {
                        this.PreviewErrorBorder = LoginBorder;

                        this.AuthErrorBorder.Visibility = Visibility.Visible;

                        this.AuthErrorCodeTextRun.Text = "Код:" + ex.ErrorCode;
                        this.AuthErrorTextRun.Text = ex.Message;


                        Debug.WriteLine(ex.StackTrace);
                    }

                });


            }
            else codeSended = true;
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            TwoFactorAuth();
        }

        private void AuthBrowser_Click(object sender, RoutedEventArgs e)
        {
        }

        private void GoToAuthButton_Click(object sender, RoutedEventArgs e)
        {
            PasswordTextbox.Text = "";
            LoginTextBox.Text = "";
            HideAccountList();
        }

        private void AccountList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VkNet.VkApi api = new VkNet.VkApi();
            var item = (UserData)AccountList.SelectedItem;
            api.Authorize(new VkNet.Model.ApiAuthParams
            {
                AccessToken = item.Token,
                UserId = (long)item.Id,
                
            });
            try
            {
                api.OnTokenExpires += Api_OnTokenExpires;
                    api.Account.GetInfo();
                    if (api.IsAuthorized)
                        ToolsAndsettings.VkApi = api;
                    
            }
            catch(VkNet.Exception.UserAuthorizationFailException ex)
            {
                PreviewErrorBorder= this.AccountListPanelBorder;
                PreviewErrorBorder.Visibility = Visibility.Collapsed;
                this.AuthErrorBorder.Visibility = Visibility.Visible;
             
                this.AuthErrorCodeTextRun.Text = "Код:" + ex.ErrorCode;
                this.AuthErrorTextRun.Text = ex.Message;
               
            }
        }

        private void Api_OnTokenExpires(VkNet.VkApi sender)
        {
            Debug.WriteLine("TOKENEXP");
        }

        private void CancelLoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.AuthErrorBorder.Visibility = Visibility.Collapsed;
            PreviewErrorBorder.Visibility = Visibility.Visible;

        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {

            var button= sender as Button;
            var data = button.DataContext as UserData;
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).OpenSubKey("VkPlayerByOneCellDM",true);
            foreach (var val in key.GetValueNames())
            {
                if (val != "Settings")
                    if (val == data.Id.ToString())
                    {
                        key.DeleteValue(val);
                        ShowActiveAccounts();
                        break;
                    }
                        
             }
            
        }



        /*

AuthWebView.Navigate(AuthUrl);
Debug.WriteLine("Authoid");
}

[DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);

public static void SetAnotherUserAgent(string ua)
{
const int urlmonOptionUseragent = 0x10000001;
const int urlmonOptionUseragentRefresh = 0x10000002;
var UserAgent = ua;
UrlMkSetSessionOption(urlmonOptionUseragentRefresh, null, 0, 0);
UrlMkSetSessionOption(urlmonOptionUseragent, UserAgent, UserAgent.Length, 0);
}

private void AuthWebView_LoadCompleted(object sender, NavigationEventArgs e)
{
if (e.Uri.AbsoluteUri.Contains("#access_token"))
{
String token = e.Uri.AbsoluteUri.Split('=')[1].Split('&')[0];
String ID = e.Uri.AbsoluteUri.Split('=')[3];

var api = new VkNet.VkApi();
var awaiter = api.AuthorizeAsync(new VkNet.Model.ApiAuthParams()
{
AccessToken = token,
UserId = long.Parse(ID)
}).GetAwaiter();

awaiter.OnCompleted(() =>
{
ToolsAndsettings.VkApi = api;
});
}
}
*/
    }
}