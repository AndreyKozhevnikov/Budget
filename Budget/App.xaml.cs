using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Budget {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private void Application_Startup_1(object sender, StartupEventArgs e) {
            AssemblyResolverDll.AsseblyResolver.Attach("Dll181");
        }
        public App() {
            CultureInfo newCI = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            newCI.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
            Thread.CurrentThread.CurrentCulture = newCI;
        }
    }
}
