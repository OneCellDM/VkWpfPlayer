using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;

namespace VkWpfPlayer.Pages
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>

    public partial class SettingsPage : Window
    {
        public delegate void UpdateBg();
        public event UpdateBg BackgroundUpdated;
        public SettingsPage()
        {
            InitializeComponent();



            Tools.settings.LoadSettings();
            ButtonAndTextboxRoundRadiusSlider.Value = Tools.settings.CurrentSettings.ButtonAndTextBoxCornerRadius;
            ImageBorderThicknessSlider.Value = Tools.settings.CurrentSettings.ImageBorderThickness;
            TextColoTextbox.Text = Tools.settings.CurrentSettings.TextColor;
            ControlColorTextbox.Text = Tools.settings.CurrentSettings.ControlColor;
            TextBoxAndButtonColorTextbox.Text = Tools.settings.CurrentSettings.ButtonColor;
            BackGroundTextBox.Text =  Tools.settings.CurrentSettings.BackGroundColor;
            SliderColorsTextBox.Text = Tools.settings.CurrentSettings.SliderColor;
            MouseOverColorTextBox.Text = Tools.settings.CurrentSettings.MouseOverColor;
            ImageBorderColorTextBox.Text = Tools.settings.CurrentSettings.ImageBorderColor;
            PlayerButtonTextColorTextBox.Text = Tools.settings.CurrentSettings.PlayerButtonTextColor;
            AudioDirectoryTextbox.Text = Tools.settings.CurrentSettings.VKAudioDownloadPath;
            UseImageCheckBox.IsChecked = Tools.settings.CurrentSettings.IsBackgroundImage;

            if (Tools.settings.CurrentSettings.BackGroundImage != null)
                BackGroundImagePath.Text = Tools.settings.CurrentSettings.BackGroundImage;

          
            RoundImageSlider.Value = Tools.settings.CurrentSettings.ImageCornerRadios;
        }

        private void TextColoTextbox_TextChanged(object sender, TextChangedEventArgs e) =>
            Tools.settings.CurrentSettings.TextColor = Tools.UI.GetTextFromTextboxToUpper(sender);
        private void BackGroundTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tools.settings.CurrentSettings.BackGroundColor = Tools.UI.GetTextFromTextboxToUpper(sender);
            BackgroundUpdated?.Invoke();
        }
        private void ImageBorderColorTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
            Tools.settings.CurrentSettings.ImageBorderColor = Tools.UI.GetTextFromTextboxToUpper(sender);
        private void MouseOverColorTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
            Tools.settings.CurrentSettings.MouseOverColor = Tools.UI.GetTextFromTextboxToUpper(sender);
        private void TextBoxAndButtonColorTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Tools.settings.CurrentSettings.TextBoxColor = Tools.UI.GetTextFromTextboxToUpper(sender);
            Tools.settings.CurrentSettings.ButtonColor = Tools.UI.GetTextFromTextboxToUpper(sender);
        }
        private void PlayerButtonTextColorTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
            Tools.settings.CurrentSettings.PlayerButtonTextColor = Tools.UI.GetTextFromTextboxToUpper(sender);
        private void ControlColorTextbox_TextChanged(object sender, TextChangedEventArgs e) =>
            Tools.settings.CurrentSettings.ControlColor = Tools.UI.GetTextFromTextboxToUpper(sender);
        private void SliderColorsTextBox_TextChanged(object sender, TextChangedEventArgs e) =>
            Tools.settings.CurrentSettings.SliderColor = Tools.UI.GetTextFromTextboxToUpper(sender);
        private void BackGroundImagePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            string Ptext = ((System.Windows.Controls.TextBox)sender).Text;
            if (File.Exists(Ptext))
                Tools.settings.CurrentSettings.BackGroundImage = Ptext;
            BackgroundUpdated?.Invoke();
        }
        private void AudioDirectoryTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string Ptext = ((System.Windows.Controls.TextBox)sender).Text;
            if (new DirectoryInfo(Ptext).Exists)
                Tools.settings.CurrentSettings.VKAudioDownloadPath = Ptext;
        }

        private void RoundImageSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            Tools.settings.CurrentSettings.ImageCornerRadios =(int)e.NewValue;
            RadiusRoundValue.Content = "Значение: " + ((int)e.NewValue);
        }

        private void ImageBorderThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Tools.settings.CurrentSettings.ImageBorderThickness =(int)e.NewValue;
            ImageBorderThicknessLabel.Content = "Толщина обводки картинки:" + (int)e.NewValue;
        }

        private void ButtonAndTextboxRoundRadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Tools.settings.CurrentSettings.ButtonAndTextBoxCornerRadius = e.NewValue;
            ButtonAndTextboxRoundRadiusTextBlock.Content = ((int)e.NewValue);
        }

        private void AudioSelectPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if(folderBrowserDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
                    AudioDirectoryTextbox.Text = folderBrowserDialog.SelectedPath;
            
        }
        private string OpenColorDialog()
        {
            ColorDialog colorDialog = new ColorDialog();
            if( colorDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
              return colorDialog.Color.IsNamedColor ? colorDialog.Color.Name : "#"+ colorDialog.Color.Name;

            return string.Empty;
        }

        private void SelectTextColorButton_Click(object sender, RoutedEventArgs e)
        {
            var ColorName = OpenColorDialog();
            if (ColorName != string.Empty)
                TextColoTextbox.Text = ColorName;
        }

        private void SelectBackGroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            var ColorName = OpenColorDialog();
            if (ColorName != string.Empty)
                BackGroundTextBox.Text = ColorName;
           

        }

        private void SelectBackGroundImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                BackGroundImagePath.Text = openFileDialog.FileName;

        }

        private void SelectImageBorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            var ColorName = OpenColorDialog();
            if (ColorName != string.Empty)
                ImageBorderColorTextBox.Text = ColorName;
        }

        private void SelectMouseOverColorButton_Click(object sender, RoutedEventArgs e)
        {
            var ColorName = OpenColorDialog();
            if (ColorName != string.Empty)
                MouseOverColorTextBox.Text = ColorName;
        }

        private void SelectTextBoxAndButtonColorButton_Click(object sender, RoutedEventArgs e)
        {
            var ColorName = OpenColorDialog();
            if (ColorName != string.Empty)
                TextBoxAndButtonColorTextbox.Text = ColorName;
        }

        private void SelectPlayerButtonTextColorButton_Click(object sender, RoutedEventArgs e)
        {
            var ColorName = OpenColorDialog();
            if (ColorName != string.Empty)
                PlayerButtonTextColorTextBox.Text = ColorName;
        }

        private void SelectControlColorTextboxButton_Click(object sender, RoutedEventArgs e)
        {
            var ColorName = OpenColorDialog();
            if (ColorName != string.Empty)
                ControlColorTextbox.Text = ColorName;
        }

        private void SelectSliderColorButton_Click(object sender, RoutedEventArgs e)
        {
            var ColorName = OpenColorDialog();
            if (ColorName != string.Empty)
                SliderColorsTextBox.Text = ColorName;
        }

        private void UseImageCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Tools.settings.CurrentSettings.IsBackgroundImage = true;
            BackgroundUpdated?.Invoke();
        }

        private void UseImageCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Tools.settings.CurrentSettings.IsBackgroundImage = false;
            BackgroundUpdated?.Invoke();
        }
    }
}