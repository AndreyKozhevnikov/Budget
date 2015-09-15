using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Budget {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private void Application_Startup_1(object sender, StartupEventArgs e) {
            if (e.Args.Count() > 0 && e.Args[0] == "-testBase") {
                OrderViewModel.IsTestMode = true;
            }
        }
    }
}
