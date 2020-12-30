using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace VkWpfPlayer
{
    public partial class EISDialog : UserControl
    { 

     

 
    
        public delegate void Accept();
        public event Accept Accepted;

        public delegate void Cancel();
        public event Accept Cancelled;

        private String _accetpButtonText = "Да";
        private String _cancelButtonText = "Нет";

        private object _content;
        private Visibility _acceptButtonVisible;
        private Visibility _cancelButtonVisible;

        public bool AcceptDialog { get; set; } = false;

        public Visibility AcceptButtonVisible
        {
            get => _acceptButtonVisible; 
            set
            {
                _acceptButtonVisible = value;
                AcceptButton.Visibility = _acceptButtonVisible;
            }
        }
        public Visibility CancelButtonVisible {
            get => _cancelButtonVisible;
            set
            {
                _cancelButtonVisible = value;
                CancelButton.Visibility = CancelButtonVisible;
            }
        }
        public string CancelButtonText
        {
            get => _cancelButtonText;
            set
            {
                _cancelButtonText = value;
                CancelButton.Content = _cancelButtonText;
                
            }
        }
        public string AcceptButtonText
        {
            get => _accetpButtonText;
            set
            {
                _accetpButtonText = value;
                AcceptButton.Content = _accetpButtonText;
            } 
        }

        public object ContentDialogData
        {
            get => _content;
            set
            {
                _content = value;
                ContentData.Content = _content;
            }
        }
        public EISDialog()=> InitializeComponent();   
        

        private void Accept_Click(object sender, RoutedEventArgs e)=> Accepted?.Invoke();
          
        private void Cancel_Click(object sender, RoutedEventArgs e)=> Cancelled?.Invoke();

    }
}