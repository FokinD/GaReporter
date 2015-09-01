using System;
using System.Windows;

namespace GaReporter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Title = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + " " + version.Major + "." + version.Minor + "." + version.Build;

        }
    }
}