using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hugeCSVsplitter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //lbl_version.Content = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void TextBlock_Drop(object sender, DragEventArgs e)
        {
            dragLabel.Foreground = (Brush)new BrushConverter().ConvertFrom("#AABBCC");

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 1)
                {
                    MessageBox.Show(
                        string.Format("Only {0} will be processed.", files[0]),
                        "You dragged more than 1 file",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                        );
                }
                //foreach (string file in files)
                //{
                    //MessageBox.Show(files[0]);
                    sourceCSVpath.Text = files[0];
                //}
            }

        }

        private void TextBlock_DragEnter(object sender, DragEventArgs e)
        {
            dragLabel.Foreground = (Brush)new BrushConverter().ConvertFrom("#8895A3");
        }

        private void dropArea_DragLeave(object sender, DragEventArgs e)
        {
            dragLabel.Foreground = (Brush)new BrushConverter().ConvertFrom("#AABBCC");
        }

        private void sourceCSVpath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(sourceCSVpath.Text)) { btn_spitDat.IsEnabled = true; }
            else { btn_spitDat.IsEnabled = false; }
        }

        private void btn_spitDat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileName = sourceCSVpath.Text.Trim();
                if (!File.Exists(fileName))
                {
                }
                string fileExt = System.IO.Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(fileExt))
                {
                    MessageBox.Show(
                            "The file has no extesion. Actually, it's not that big of a deal, but... long story. Having some extension is mandatory.",
                            "File without extension",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning
                            );
                    return;
                }
                if (fileExt != ".csv")
                {
                    if (MessageBox.Show(
                            string.Format("Are you sure that you want to split [{0}] file? Based on extension, it's not a CSV file", fileExt),
                            "Wrong file extension",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question,
                            MessageBoxResult.No
                            ) == MessageBoxResult.No)
                    { return; }
                }

                btn_spitDat.Visibility = System.Windows.Visibility.Hidden;
                btn_stopDat.Visibility = System.Windows.Visibility.Visible;

                bdr_drop.Visibility = System.Windows.Visibility.Hidden;
                pb_splitting.Visibility = System.Windows.Visibility.Visible;
                txbx_log.Visibility = System.Windows.Visibility.Visible;
            }
            catch (Exception ex)
            {

            }
        }

        private void btn_stopDat_Click(object sender, RoutedEventArgs e)
        {
            btn_stopDat.Visibility = System.Windows.Visibility.Hidden;
            btn_spitDat.Visibility = System.Windows.Visibility.Visible;

            pb_splitting.Visibility = System.Windows.Visibility.Hidden;
            txbx_log.Visibility = System.Windows.Visibility.Hidden;
            bdr_drop.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
