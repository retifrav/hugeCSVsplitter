using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace hugeCSVsplitter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    base.OnStartup(e);

        //    string[] cmdArgs = Environment.GetCommandLineArgs();
        //    Dictionary<string, string> args = new Dictionary<string, string>();
        //    if (cmdArgs.Length > 1)
        //    {
        //        try
        //        {
        //            for (int i = 1; i < cmdArgs.Length; i += 2)
        //            {
        //                args.Add(cmdArgs[i], cmdArgs[i + 1]);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(
        //                "You passed some wrong arguments. Start the application without any arguments and press F1 for help.",
        //                "Error parsing command line arguments",
        //                MessageBoxButton.OK,
        //                MessageBoxImage.Error
        //                );
        //        }
        //    }
        //    else
        //    {
        //        new MainWindow().ShowDialog();
        //    }
        //    this.Shutdown();
        //}

        private void Application_DispatcherUnhandledException(
            object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e
            )
        {
            MessageBox.Show(
                //string.Format("Some error has just hapenned.{1}Details: {0}{1}Even more details: {2}", e.Exception.Message, Environment.NewLine, e.Exception.StackTrace),
                string.Format("Some error has just hapenned.{1}Details: {0}",
                    e.Exception.Message, Environment.NewLine),
                "Unknown error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
                );
            e.Handled = true;
        }
    }
}
