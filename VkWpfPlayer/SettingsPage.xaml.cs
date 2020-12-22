using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VkWpfPlayer
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Window
    {
        DispatcherTimer dispatcherTimer;
        public SettingsPage()
        {
            InitializeComponent();
           
            RoundImageSlider.Value          = ToolsAndsettings.CurrentSettings.ImageCornerRadios;
            ButtonRoundRadiusSlider.Value   = ToolsAndsettings.CurrentSettings.ButtonAndTextBoxCornerRadius;
            BorderThicknessSlider.Value     = ToolsAndsettings.CurrentSettings.ImageBorderThickness;
            TextColoTextbox.Text            = ToolsAndsettings.CurrentSettings.TextColor;    
            ControlColorTextbox.Text        = ToolsAndsettings.CurrentSettings.ConrolColor;
            ButtonColorTextbox.Text         = ToolsAndsettings.CurrentSettings.ButtonColor;
            BackGroundTextBox.Text          = ToolsAndsettings.CurrentSettings.BackGroundColor;
            SliderColorsTextBox.Text        = ToolsAndsettings.CurrentSettings.SliderColor;
            MouseOverColorTextBox.Text      = ToolsAndsettings.CurrentSettings.MouseOverColor;
            ImageBorderColorTextBox.Text    = ToolsAndsettings.CurrentSettings.ImageBorderColor;
            
            



        }

        private void RoundImageSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            Application.Current.Resources["VKImageCornerRadius"] =new CornerRadius((int)e.NewValue);
            RadiusRoundValue.Content = "Значение: " + ((int)e.NewValue);
            
        }

        private void TextColoTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
              
                
                    try
                    {
                        BrushConverter brushConverter = new BrushConverter();
                        brushConverter.ConvertFromString(TextColoTextbox.Text.ToUpper().Trim());
                        Application.Current.Resources["VKTextColor"] = brushConverter.ConvertFromString(TextColoTextbox.Text.ToUpper().Trim());
                        
                       
                    }
                    catch(Exception ex) { Application.Current.Resources["VKTextColor"] = Brushes.Black; }
                    
                
               
           
            
        }

        private void BackGroundTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           
                try
                {
                    BrushConverter brushConverter = new BrushConverter();
                  
                    Application.Current.Resources["VKBackGroundColor"] = brushConverter.ConvertFromString(BackGroundTextBox.Text.ToUpper().Trim());


                }
                catch (Exception ex) {  Application.Current.Resources["VKBackGroundColor"] = Brushes.White; }
               
            
        }

        private void ButtonRoundRadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Application.Current.Resources["VKButtonAndTextBoxCornerRadius"] = new CornerRadius((int)e.NewValue);
            ButtonRoundRadiusTextBlock.Content = ((int)e.NewValue);
        }

        private void ImageBorderColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                BrushConverter brushConverter = new BrushConverter();
                Application.Current.Resources["VKImageBorderColor"] = brushConverter.ConvertFromString(ImageBorderColorTextBox.Text.ToUpper().Trim());
            }
            catch(Exception ex)
            {
                Application.Current.Resources["VKImageBorderColor"] = Brushes.Transparent;
            }
        }

        private void MouseOverColorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BrushConverter brushConverter = new BrushConverter();
            try
            {
               

                Application.Current.Resources["VkMouseOverColor"] = brushConverter.ConvertFromString(MouseOverColorTextBox.Text.ToUpper().Trim());


            }
            catch (Exception ex) { Debug.WriteLine(ex); Application.Current.Resources["VkMouseOverColor"] = brushConverter.ConvertFromString(ToolsAndsettings.DefaultSettings.MouseOverColor); }
        }
        BrushConverter brushConverter = new BrushConverter();
        private void ButtonColorTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
        

                Application.Current.Resources["VkButtonColor"] = brushConverter.ConvertFromString(ButtonColorTextbox.Text.ToUpper().Trim());

                    
            }
            catch (Exception ex) { Debug.WriteLine(ex); Application.Current.Resources["VkButtonColor"] = brushConverter.ConvertFromString(ToolsAndsettings.DefaultSettings.ButtonColor); }

        }

        private void ControlColorTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BrushConverter brushConverter = new BrushConverter();
            try
            {
                Application.Current.Resources["VkContolColor"] = brushConverter.ConvertFromString(ControlColorTextbox.Text.ToUpper().Trim());
            }
            catch(Exception ex) { Console.WriteLine(ex.Message); Application.Current.Resources["VkContolColor"] = brushConverter.ConvertFromString(ToolsAndsettings.DefaultSettings.ConrolColor); }
        }

        private void SliderColorsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BrushConverter brushConverter = new BrushConverter();
            try
            {
               
                Application.Current.Resources["VkSliderColor"] = brushConverter.ConvertFromString(SliderColorsTextBox.Text.ToUpper().Trim());

                
            }
            catch(Exception ex) { Application.Current.Resources["VkSliderColor"] = brushConverter.ConvertFromString(ToolsAndsettings.DefaultSettings.SliderColor); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BorderThicknessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Application.Current.Resources["VKImageBorderThickness"] = new Thickness((int)e.NewValue);
            BorderThicknessLabel.Content = "Толщина обводки картинки:"+(int)e.NewValue;
        }
    }
}
