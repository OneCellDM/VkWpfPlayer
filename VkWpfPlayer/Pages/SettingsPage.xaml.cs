using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

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
                Application.Current.Resources[resourceName] = brushConverter.ConvertFromString(colorName);
        public void SetCornerRadius(String resourceName, int value)=>
            Application.Current.Resources[resourceName] = new CornerRadius(value);

        public String GetTextFromTextbox(object textBox)=>
            ((TextBox) textBox).Text.ToUpper().Trim();
        
        public void SetBorderThickness(String resourceName, int value)=>
            Application.Current.Resources[resourceName] = new Thickness(value);
        

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
    }
}