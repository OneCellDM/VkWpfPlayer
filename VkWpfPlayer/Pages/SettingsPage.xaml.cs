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
            ImageCornerRadios = ToolsAndsettings.CurrentSettings.ImageCornerRadios;
            ButtonAndTextBoxCornerRadius = ToolsAndsettings.CurrentSettings.ButtonAndTextBoxCornerRadius;
            ImageBorderThickness = ToolsAndsettings.CurrentSettings.ImageBorderThickness;
            TextColor = ToolsAndsettings.CurrentSettings.TextColor;
            ControlColor = ToolsAndsettings.CurrentSettings.ControlColor;
            ButtonColor = ToolsAndsettings.CurrentSettings.ButtonColor;
            BackGroundColor = ToolsAndsettings.CurrentSettings.BackGroundColor;
            SliderColor = ToolsAndsettings.CurrentSettings.SliderColor;
            MouseOverColor = ToolsAndsettings.CurrentSettings.MouseOverColor;
            ImageBorderColor = ToolsAndsettings.CurrentSettings.ImageBorderColor;
            PlayerButtonTextColor = ToolsAndsettings.CurrentSettings.PlayerButtonTextColor;
            VKAudioDownloadPath = ToolsAndsettings.CurrentSettings.VKAudioDownloadPath;
        }

        private BrushConverter brushConverter = new BrushConverter();
    }

    public partial class SettingsPage : Window
    {
        private DispatcherTimer dispatcherTimer;

        public SettingsPage()
        {
            InitializeComponent();
            ToolsAndsettings.LoadSettings();

            RoundImageSlider.Value = ToolsAndsettings.CurrentSettings.ImageCornerRadios;
            ButtonRoundRadiusSlider.Value = ToolsAndsettings.CurrentSettings.ButtonAndTextBoxCornerRadius;
            BorderThicknessSlider.Value = ToolsAndsettings.CurrentSettings.ImageBorderThickness;
            TextColoTextbox.Text = ToolsAndsettings.CurrentSettings.TextColor;
            ControlColorTextbox.Text = ToolsAndsettings.CurrentSettings.ControlColor;
            TextBoxAndButtonColorTextbox.Text = ToolsAndsettings.CurrentSettings.ButtonColor;
            BackGroundTextBox.Text = ToolsAndsettings.CurrentSettings.BackGroundColor;
            SliderColorsTextBox.Text = ToolsAndsettings.CurrentSettings.SliderColor;
            MouseOverColorTextBox.Text = ToolsAndsettings.CurrentSettings.MouseOverColor;
            ImageBorderColorTextBox.Text = ToolsAndsettings.CurrentSettings.ImageBorderColor;
            PlayerButtonTextColorTextBox.Text = ToolsAndsettings.CurrentSettings.PlayerButtonTextColor;
            AudioDirectoryTextbox.Text = ToolsAndsettings.CurrentSettings.VKAudioDownloadPath;
        }

        private void MoveCacheToNewLocation(String oldpath, String newpath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(oldpath);
            directoryInfo.MoveTo(newpath);
        }

        private void RoundImageSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ToolsAndsettings.CurrentSettings.SetCornerRadius("VKImageCornerRadius", (int)e.NewValue);
            RadiusRoundValue.Content = "Значение: " + ((int)e.NewValue);
        }

        private void TextColoTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ToolsAndsettings.CurrentSettings.SetColor("VKTextColor", ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender));
                ToolsAndsettings.CurrentSettings.TextColor = ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                ToolsAndsettings.CurrentSettings.SetColor("VKTextColor", ToolsAndsettings.DefaultSettings.TextColor);
                ToolsAndsettings.CurrentSettings.TextColor = ToolsAndsettings.DefaultSettings.TextColor;
            }
        }

        private void BackGroundTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ToolsAndsettings.CurrentSettings.SetColor("VKBackGroundColor", ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender));
                ToolsAndsettings.CurrentSettings.BackGroundColor = ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                ToolsAndsettings.CurrentSettings.SetColor("VKBackGroundColor", ToolsAndsettings.DefaultSettings.BackGroundColor);

                ToolsAndsettings.CurrentSettings.BackGroundColor = ToolsAndsettings.DefaultSettings.BackGroundColor;
            }
        }

        private void ButtonRoundRadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ToolsAndsettings.CurrentSettings.SetCornerRadius("VKButtonAndTextBoxCornerRadius", (int)e.NewValue);
            ToolsAndsettings.CurrentSettings.ButtonAndTextBoxCornerRadius = (int)e.NewValue;
            ButtonRoundRadiusTextBlock.Content = ((int)e.NewValue);
        }

        private void ImageBorderColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ToolsAndsettings.CurrentSettings.SetColor("VKImageBorderColor", ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender));
                ToolsAndsettings.CurrentSettings.ImageBorderColor = ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                ToolsAndsettings.CurrentSettings.SetColor("VKImageBorderColor", ToolsAndsettings.DefaultSettings.ImageBorderColor);
                ToolsAndsettings.CurrentSettings.ImageBorderColor = ToolsAndsettings.DefaultSettings.ImageBorderColor;
            }
        }

        private void MouseOverColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ToolsAndsettings.CurrentSettings.SetColor("VkMouseOverColor", ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender));
                ToolsAndsettings.CurrentSettings.MouseOverColor = ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                ToolsAndsettings.CurrentSettings.SetColor("VkMouseOverColor", ToolsAndsettings.DefaultSettings.MouseOverColor);

                ToolsAndsettings.CurrentSettings.MouseOverColor = ToolsAndsettings.DefaultSettings.MouseOverColor;
            }
        }

        private void TextboxColorAndButtonColorTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ToolsAndsettings.CurrentSettings.SetColor("VkButtonColor", ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender));
                ToolsAndsettings.CurrentSettings.SetColor("VkTextBoxColor", ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender));
                ToolsAndsettings.CurrentSettings.ButtonColor = ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender);
                ToolsAndsettings.CurrentSettings.TextBoxColor = ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                ToolsAndsettings.CurrentSettings.SetColor("VkTextBoxColor", ToolsAndsettings.DefaultSettings.TextBoxColor);
                ToolsAndsettings.CurrentSettings.SetColor("VkButtonColor", ToolsAndsettings.DefaultSettings.ButtonColor);

                ToolsAndsettings.CurrentSettings.ButtonColor = ToolsAndsettings.DefaultSettings.ButtonColor;
                ToolsAndsettings.CurrentSettings.TextBoxColor = ToolsAndsettings.DefaultSettings.TextBoxColor;
            }
        }

        private void ControlColorTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ToolsAndsettings.CurrentSettings.SetColor("VkContolColor", ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender));
                ToolsAndsettings.CurrentSettings.ControlColor = ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                ToolsAndsettings.CurrentSettings.SetColor("VkContolColor", ToolsAndsettings.DefaultSettings.ControlColor);
                ToolsAndsettings.CurrentSettings.ControlColor = ToolsAndsettings.DefaultSettings.ControlColor;
            }
        }

        private void SliderColorsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ToolsAndsettings.CurrentSettings.SetColor("VkSliderColor", ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender));
                ToolsAndsettings.CurrentSettings.SliderColor = ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                ToolsAndsettings.CurrentSettings.SetColor("VkSliderColor", ToolsAndsettings.DefaultSettings.SliderColor);
                ToolsAndsettings.CurrentSettings.SliderColor = ToolsAndsettings.DefaultSettings.SliderColor;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BorderThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ToolsAndsettings.CurrentSettings.SetBorderThickness("VKImageBorderThickness", (int)e.NewValue);
            BorderThicknessLabel.Content = "Толщина обводки картинки:" + (int)e.NewValue;
        }

        private void PlayerButtonTextColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ToolsAndsettings.CurrentSettings.SetColor("VKPlayerButtonTextColor", ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender));
                ToolsAndsettings.CurrentSettings.PlayerButtonTextColor = ToolsAndsettings.CurrentSettings.GetTextFromTextbox(sender);
            }
            catch (Exception ex)
            {
                ToolsAndsettings.CurrentSettings.SetColor("VKPlayerButtonTextColor", ToolsAndsettings.DefaultSettings.PlayerButtonTextColor);

                ToolsAndsettings.CurrentSettings.PlayerButtonTextColor = ToolsAndsettings.DefaultSettings.PlayerButtonTextColor;
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
                TextColoTextbox.Text = "#" + color.Name;
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
                else SelectTextBoxAndButtonColorButton.Tag = "#" + color.Name;
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
                ToolsAndsettings.CurrentSettings.VKAudioDownloadPath = AudioDirectoryTextbox.Text;
            else
            {
                ToolsAndsettings.CurrentSettings.VKAudioDownloadPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\VKM\\AUDIO";
            }
        }

        private void TextColoTextbox_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            Debug.WriteLine("sss");
        }
    }
}