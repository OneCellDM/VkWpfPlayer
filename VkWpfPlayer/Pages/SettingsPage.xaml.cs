using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

namespace VkWpfPlayer.Pages
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>

    public class SettingsModel : IDataErrorInfo
    {
        public double ImageCornerRadios { get; set; }
        public double ButtonAndTextBoxCornerRadius { get; set; }
        public double ImageBorderThickness { get; set; }
        public double ButtonAndTextBoxBorderThickness { get; set; }
        public string VKAudioDownloadPath { get; set; }

        public string BackGroundColor { get; set; }
        public string TextColor { get; set; }

        public string MouseOverColor { get; set; }
        public string ButtonColor { get; set; }
        public string TextBoxColor { get; set; } = "white";
        public string SliderColor { get; set; }
        public string ControlColor { get; set; }
        public string ImageBorderColor { get; set; }
        public string PlayerButtonTextColor { get; set; }
        public string CachePath { get; set; }
        private List<string> ColorProperties = new List<string>() { "TextColor", "TextBoxColor", "SliderColor", "ControlColor", "ImageBorderColor", "PlayerButtonTextColor" };

        private List<string> PathsProperties = new List<string>()
        {
            "VKAudioDownloadPath",
            "CachePath"
        };

        private string checkColor(string color)
        {
            if (!ValidationColor(color))
                if (!ValidationColor("#" + color))
                    return "Неверный формат цвета";

            return string.Empty;
        }

        private bool ValidationColor(string Name)
        {
            try
            {
                brushConverter.ConvertFromString(Name);

                return true;
            }
            catch (System.FormatException ex) { return false; }
        }

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;

                foreach (var s in PathsProperties)
                {
                    if (s == columnName)
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo((String)typeof(SettingsModel).GetProperty(s).GetValue(this));

                        if (!directoryInfo.Exists)
                            return "Путь не существует, будет использован путь по умолчанию";
                    }
                }
                foreach (var s in ColorProperties)
                {
                    if (s == columnName)
                    {
                        return checkColor((String)typeof(SettingsModel).GetProperty(s).GetValue(this));
                    }
                }
                return error;
            }
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public SettingsModel()
        {
            ImageCornerRadios = Tools.CurrentSettings.ImageCornerRadios;
            ButtonAndTextBoxCornerRadius = Tools.CurrentSettings.ButtonAndTextBoxCornerRadius;
            ImageBorderThickness = Tools.CurrentSettings.ImageBorderThickness;
            TextColor = Tools.CurrentSettings.TextColor;
            ControlColor = Tools.CurrentSettings.ControlColor;
            ButtonColor = Tools.CurrentSettings.ButtonColor;
            BackGroundColor = Tools.CurrentSettings.BackGroundColor;
            SliderColor = Tools.CurrentSettings.SliderColor;
            MouseOverColor = Tools.CurrentSettings.MouseOverColor;
            ImageBorderColor = Tools.CurrentSettings.ImageBorderColor;
            PlayerButtonTextColor = Tools.CurrentSettings.PlayerButtonTextColor;
            VKAudioDownloadPath = Tools.CurrentSettings.VKAudioDownloadPath;
        }

        private BrushConverter brushConverter = new BrushConverter();
    }

    public partial class SettingsPage : Window
    {
        private DispatcherTimer dispatcherTimer;

        public SettingsPage()
        {
            InitializeComponent();
            Tools.LoadSettings();

            RoundImageSlider.Value = Tools.CurrentSettings.ImageCornerRadios;
            ButtonRoundRadiusSlider.Value = Tools.CurrentSettings.ButtonAndTextBoxCornerRadius;
            BorderThicknessSlider.Value = Tools.CurrentSettings.ImageBorderThickness;
            TextColoTextbox.Text = Tools.CurrentSettings.TextColor;
            ControlColorTextbox.Text = Tools.CurrentSettings.ControlColor;
            TextBoxAndButtonColorTextbox.Text = Tools.CurrentSettings.ButtonColor;
            BackGroundTextBox.Text = Tools.CurrentSettings.BackGroundColor;
            SliderColorsTextBox.Text = Tools.CurrentSettings.SliderColor;
            MouseOverColorTextBox.Text = Tools.CurrentSettings.MouseOverColor;
            ImageBorderColorTextBox.Text = Tools.CurrentSettings.ImageBorderColor;
            PlayerButtonTextColorTextBox.Text = Tools.CurrentSettings.PlayerButtonTextColor;
            AudioDirectoryTextbox.Text = Tools.CurrentSettings.VKAudioDownloadPath;
            UseImageCheckBox.IsChecked = Tools.CurrentSettings.IsBackgroundImage;
            if (Tools.CurrentSettings.BackGroundImage != null)
                BackGroundImagePath.Text = Tools.CurrentSettings.BackGroundImage;
            BackGroundOpacitySlider.Value = Tools.CurrentSettings.BackGroundOpacity;
        }

        private void MoveCacheToNewLocation(String oldpath, String newpath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(oldpath);
            directoryInfo.MoveTo(newpath);
        }

        private void RoundImageSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Tools.CurrentSettings.SetCornerRadius("VKImageCornerRadius", (int)e.NewValue);
            RadiusRoundValue.Content = "Значение: " + ((int)e.NewValue);
        }

        private void TextColoTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Tools.CurrentSettings.SetColor("VKTextColor", Tools.CurrentSettings.GetTextFromTextbox(sender));
                Tools.CurrentSettings.TextColor = Tools.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                Tools.CurrentSettings.SetColor("VKTextColor", Tools.DefaultSettings.TextColor);
                Tools.CurrentSettings.TextColor = Tools.DefaultSettings.TextColor;
            }
        }

        private void BackGroundTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Tools.CurrentSettings.SetColor("VKBackGroundColor", Tools.CurrentSettings.GetTextFromTextbox(sender));
                Tools.CurrentSettings.BackGroundColor = Tools.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                Tools.CurrentSettings.SetColor("VKBackGroundColor", Tools.DefaultSettings.BackGroundColor);

                Tools.CurrentSettings.BackGroundColor = Tools.DefaultSettings.BackGroundColor;
            }
        }

        private void ButtonRoundRadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Tools.CurrentSettings.SetCornerRadius("VKButtonAndTextBoxCornerRadius", (int)e.NewValue);
            Tools.CurrentSettings.ButtonAndTextBoxCornerRadius = (int)e.NewValue;
            ButtonRoundRadiusTextBlock.Content = ((int)e.NewValue);
        }

        private void ImageBorderColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Tools.CurrentSettings.SetColor("VKImageBorderColor", Tools.CurrentSettings.GetTextFromTextbox(sender));
                Tools.CurrentSettings.ImageBorderColor = Tools.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                Tools.CurrentSettings.SetColor("VKImageBorderColor", Tools.DefaultSettings.ImageBorderColor);
                Tools.CurrentSettings.ImageBorderColor = Tools.DefaultSettings.ImageBorderColor;
            }
        }

        private void MouseOverColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Tools.CurrentSettings.SetColor("VkMouseOverColor", Tools.CurrentSettings.GetTextFromTextbox(sender));
                Tools.CurrentSettings.MouseOverColor = Tools.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                Tools.CurrentSettings.SetColor("VkMouseOverColor", Tools.DefaultSettings.MouseOverColor);

                Tools.CurrentSettings.MouseOverColor = Tools.DefaultSettings.MouseOverColor;
            }
        }

        private void TextboxColorAndButtonColorTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Tools.CurrentSettings.SetColor("VkButtonColor", Tools.CurrentSettings.GetTextFromTextbox(sender));
                Tools.CurrentSettings.SetColor("VkTextBoxColor", Tools.CurrentSettings.GetTextFromTextbox(sender));
                Tools.CurrentSettings.ButtonColor = Tools.CurrentSettings.GetTextFromTextbox(sender);
                Tools.CurrentSettings.TextBoxColor = Tools.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                Tools.CurrentSettings.SetColor("VkTextBoxColor", Tools.DefaultSettings.TextBoxColor);
                Tools.CurrentSettings.SetColor("VkButtonColor", Tools.DefaultSettings.ButtonColor);

                Tools.CurrentSettings.ButtonColor = Tools.DefaultSettings.ButtonColor;
                Tools.CurrentSettings.TextBoxColor = Tools.DefaultSettings.TextBoxColor;
            }
        }

        private void ControlColorTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Tools.CurrentSettings.SetColor("VkContolColor", Tools.CurrentSettings.GetTextFromTextbox(sender));
                Tools.CurrentSettings.ControlColor = Tools.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                Tools.CurrentSettings.SetColor("VkContolColor", Tools.DefaultSettings.ControlColor);
                Tools.CurrentSettings.ControlColor = Tools.DefaultSettings.ControlColor;
            }
        }

        private void SliderColorsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Tools.CurrentSettings.SetColor("VkSliderColor", Tools.CurrentSettings.GetTextFromTextbox(sender));
                Tools.CurrentSettings.SliderColor = Tools.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                Tools.CurrentSettings.SetColor("VkSliderColor", Tools.DefaultSettings.SliderColor);
                Tools.CurrentSettings.SliderColor = Tools.DefaultSettings.SliderColor;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BorderThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Tools.CurrentSettings.SetBorderThickness("VKImageBorderThickness", (int)e.NewValue);
            BorderThicknessLabel.Content = "Толщина обводки картинки:" + (int)e.NewValue;
        }

        private void PlayerButtonTextColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Tools.CurrentSettings.SetColor("VKPlayerButtonTextColor", Tools.CurrentSettings.GetTextFromTextbox(sender));
                Tools.CurrentSettings.PlayerButtonTextColor = Tools.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                Tools.CurrentSettings.SetColor("VKPlayerButtonTextColor", Tools.DefaultSettings.PlayerButtonTextColor);

                Tools.CurrentSettings.PlayerButtonTextColor = Tools.DefaultSettings.PlayerButtonTextColor;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
        }

        private void SelectTextColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
                if (color.IsNamedColor)
                   TextColoTextbox.Text = color.Name;
                else TextColoTextbox.Text = "#" + color.Name;
            }
        }

        private void SelectBackGroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
                if (color.IsNamedColor)
                    BackGroundTextBox.Text = color.Name;
                else BackGroundTextBox.Text = "#" + color.Name;
            }
        }

        private void SelectImageBorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
                if (color.IsNamedColor)
                    ImageBorderColorTextBox.Text = color.Name;
                else ImageBorderColorTextBox.Text = "#" + color.Name;
            }
        }

        private void SelectMouseOverColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
                if (color.IsNamedColor)
                    MouseOverColorTextBox.Text = color.Name;
                else MouseOverColorTextBox.Text = "#" + color.Name;
            }
        }

        private void SelectTextBoxAndButtonColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
                if (color.IsNamedColor)
                    TextBoxAndButtonColorTextbox.Text = color.Name;
                else TextBoxAndButtonColorTextbox.Text = "#" + color.Name;
            }
        }

        private void SelectPlayerButtonTextColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
                if (color.IsNamedColor)
                    PlayerButtonTextColorTextBox.Text = color.Name;
                else PlayerButtonTextColorTextBox.Text = "#" + color.Name;
            }
        }

        private void SelectControlColorTextboxButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
                if (color.IsNamedColor)
                    ControlColorTextbox.Text = color.Name;
                else ControlColorTextbox.Text = "#" + color.Name;
            }
        }

        private void SelectSliderColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
                if (color.IsNamedColor)
                    SliderColorsTextBox.Text = color.Name;
                else SliderColorsTextBox.Text = "#" + color.Name;
            }
        }

        private void AudioSelectPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                AudioDirectoryTextbox.Text = folderBrowserDialog.SelectedPath;
        }

        private void AudioDirectoryTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(AudioDirectoryTextbox.Text);

            if (!directoryInfo.Exists)
                Tools.CurrentSettings.VKAudioDownloadPath = AudioDirectoryTextbox.Text;
            else
            {
                Tools.CurrentSettings.VKAudioDownloadPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\VKM\\AUDIO";
            }
        }

        private void TextColoTextbox_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            Debug.WriteLine("sss");
        }

        private void UseImageCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Tools.CurrentSettings.IsBackgroundImage = true;
            SelectBackGroundImageButton.IsEnabled = true;
            BackGroundImagePath.IsEnabled = true;
            Tools.CurrentSettings.BackGroundInvoke();
        }

        private void UseImageCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Tools.CurrentSettings.IsBackgroundImage = false;
            SelectBackGroundImageButton.IsEnabled = false;
            BackGroundImagePath.IsEnabled = false;
            Tools.CurrentSettings.BackGroundInvoke();
        }

        private void BackGroundImagePath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (File.Exists(BackGroundImagePath.Text))
            {
                Tools.CurrentSettings.BackGroundImage = BackGroundImagePath.Text;
                Tools.CurrentSettings.BackGroundInvoke();
            }             
        }
        private void SelectBackGroundImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter= "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
           
            if( openFileDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
                BackGroundImagePath.Text = openFileDialog.FileName;
        }

        private void BackGroundOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Tools.CurrentSettings.BackGroundOpacity = e.NewValue*10;
            BackGroundOpacityLabel.Content = (Math.Round(e.NewValue*1000,0)).ToString();
            Tools.CurrentSettings.BackGroundInvoke();

        }
    }
}