using System;
using System.Windows;

namespace GaReporter
{
    public partial class FolderWindow : Window
    {
        public FolderWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}