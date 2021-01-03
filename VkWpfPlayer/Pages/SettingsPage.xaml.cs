using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Forms;
using System.IO;

namespace VkWpfPlayer.Pages
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Window
    {
        private DispatcherTimer dispatcherTimer;
        private BrushConverter brushConverter = new BrushConverter();
        public void SetColor(String resourceName,String colorName)=>
                System.Windows.Application.Current.Resources[resourceName] = brushConverter.ConvertFromString(colorName);
        public void SetCornerRadius(String resourceName, int value)=>
            System.Windows.Application.Current.Resources[resourceName] = new CornerRadius(value);

        public String GetTextFromTextbox(object textBox)=>
            ((System.Windows.Controls.TextBox) textBox).Text.ToUpper().Trim();
        
        public void SetBorderThickness(String resourceName, int value)=>
             System.Windows.Application.Current.Resources[resourceName] = new Thickness(value);
        

        public SettingsPage()
        {
            InitializeComponent();

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
            AudioDirectoryTextbox.Text = ToolsAndsettings.VKAudioDownloadPath;
        }


        private void RoundImageSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetCornerRadius("VKImageCornerRadius",(int)e.NewValue);
            RadiusRoundValue.Content = "Значение: " + ((int)e.NewValue);
        }

        private void TextColoTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
               SetColor("VKTextColor",GetTextFromTextbox(sender));
            }
            catch (Exception ex) { SetColor("VKTextColor", ToolsAndsettings.DefaultSettings.TextColor); }
        }

        private void BackGroundTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SetColor("VKBackGroundColor", GetTextFromTextbox(sender));
            }
            catch (Exception ex) { SetColor("VKBackGroundColor", ToolsAndsettings.DefaultSettings.BackGroundColor); }
        }

        private void ButtonRoundRadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetCornerRadius("VKButtonAndTextBoxCornerRadius",(int)e.NewValue);
            ButtonRoundRadiusTextBlock.Content = ((int)e.NewValue);
        }

        private void ImageBorderColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
               SetColor("VKImageBorderColor", GetTextFromTextbox(sender));
            }
            catch (Exception ex)
            {
                SetColor("VKImageBorderColor", ToolsAndsettings.DefaultSettings.ImageBorderColor);
            }
        }

        private void MouseOverColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SetColor("VkMouseOverColor", GetTextFromTextbox(sender));
            }
            catch (Exception ex) { SetColor("VkMouseOverColor", ToolsAndsettings.DefaultSettings.MouseOverColor); }
        }

        private void TextboxColorAndButtonColorTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                    SetColor("VkButtonColor", GetTextFromTextbox(sender));
                    SetColor("VkTextBoxColor", GetTextFromTextbox(sender));
                
                
            }
            
            catch (Exception ex) { 
                SetColor("VkTextBoxColor", ToolsAndsettings.DefaultSettings.TextBoxColor); 
                SetColor("VkButtonColor", ToolsAndsettings.DefaultSettings.ButtonColor); 
            }
        }

        private void ControlColorTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SetColor("VkContolColor", GetTextFromTextbox(sender));
            }
            catch (Exception ex) {SetColor("VkContolColor", ToolsAndsettings.DefaultSettings.ControlColor); }
        }

        private void SliderColorsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SetColor("VkSliderColor", GetTextFromTextbox(sender));
               
            }
            catch (Exception ex) { SetColor("VkSliderColor",ToolsAndsettings.DefaultSettings.SliderColor); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BorderThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetBorderThickness("VKImageBorderThickness", (int)e.NewValue);
            BorderThicknessLabel.Content = "Толщина обводки картинки:" + (int)e.NewValue;
        }

      

        private void PlayerButtonTextColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                SetColor("VKPlayerButtonTextColor", GetTextFromTextbox(sender));
            }            
            catch (Exception ex) {SetColor("VKPlayerButtonTextColor", ToolsAndsettings.DefaultSettings.PlayerButtonTextColor); }
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
               TextColoTextbox.Text=color.Name;
            }
        }

        private void SelectBackGroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
                BackGroundTextBox.Text = color.Name;
            }

        }

        private void SelectImageBorderColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
                ImageBorderColorTextBox.Text = color.Name;
            }
        }

        private void SelectMouseOverColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
                MouseOverColorTextBox.Text = color.Name;
            }
        }

        private void SelectTextBoxAndButtonColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
                TextBoxAndButtonColorTextbox.Text = color.Name;
            }
        }

        private void SelectPlayerButtonTextColor_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
                PlayerButtonTextColorTextBox.Text = color.Name;
            }
        }

        private void SelectPlayerButtonTextColorButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color color = colorDialog.Color;
               
            }
        }

        private void SelectControlColorTextboxButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SelectSliderColorButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AudioSelectPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if(folderBrowserDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
                AudioDirectoryTextbox.Text = folderBrowserDialog.SelectedPath;
            
        }

        private void AudioDirectoryTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(AudioDirectoryTextbox.Text);

            if (directoryInfo.Exists)
            {
                ToolsAndsettings.VKAudioDownloadPath = AudioDirectoryTextbox.Text;
                PathValideInfoLabel.Content = "Путь существует";
            }
            else
            {
                ToolsAndsettings.VKAudioDownloadPath =Environment.GetFolderPath( System.Environment.SpecialFolder.ApplicationData) + "\\VKM\\AUDIO"; 
                PathValideInfoLabel.Content = "Путь не существует, будет использован путь по умолчанию";
            }
            
        }
    }
}