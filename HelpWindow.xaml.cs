using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace hugeCSVsplitter
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();

            vers.Content = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        // https://stackoverflow.com/a/66864661/1688203
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!e.Handled && e.Key == Key.Escape && Keyboard.Modifiers == ModifierKeys.None)
            {
                this.Close();
            }
        }

        private void openLink(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try { Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)); }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format("For some reasons openning this link has failed.{0}Details: {1}",
                        Environment.NewLine, ex.Message),
                    "Couldn't open the link",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
            
            e.Handled = true;
        }
    }
}
