using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Ookii.Dialogs.Wpf;

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

            // selected text encoding for reading/writing files
            try
            {
                cmbx_filesEncoding.SelectedItem = cmbx_filesEncoding.Items
                    .Cast<ComboBoxItem>()
                    .First(
                        i => i.Content.ToString().ToLower() == Properties.Settings.Default.filesEncoding.ToLower()
                    );
            }
            catch
            {
                //MessageBox.Show(
                //    new StringBuilder()
                //        .Append("The application config contains an unknown encoding, ")
                //        .Append("falling back to UTF-8.")
                //        .ToString(),
                //    "Unknown encoding",
                //    MessageBoxButton.OK,
                //    MessageBoxImage.Warning
                //);

                cmbx_filesEncoding.SelectedIndex = cmbx_filesEncoding.Items.Add(
                    new ComboBoxItem
                    {
                        //Tag = Properties.Settings.Default.filesEncoding.ToLower(),
                        Content = Properties.Settings.Default.filesEncoding
                    }
                );
            }

            // when F1 is pressed, show help window
            CommandBinding helpBinding = new CommandBinding(ApplicationCommands.Help);
            helpBinding.Executed += f1pressed;
            CommandBindings.Add(helpBinding);
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

        private void resetAll()
        {
            btn_stopDat.Visibility = Visibility.Hidden;
            btn_spitDat.Visibility = Visibility.Visible;

            pb_splitting.Visibility = Visibility.Hidden;
            txbx_log.Visibility = Visibility.Hidden;
            bdr_drop.Visibility = Visibility.Visible;

            pb_splitting.Value = 0;
            pb_splitting.Minimum = 0;
            pb_splitting.Maximum = 100;

            txbx_log.Text = string.Empty;

            sourceCSVpath.IsEnabled = true;
            outputDir.IsEnabled = true;
        }

        private CancellationTokenSource cts;

        private void btn_spitDat_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileName = sourceCSVpath.Text.Trim();
                if (!File.Exists(fileName))
                {
                    MessageBox.Show(
                        string.Format("The specified file [{0}] doesn't exist.", fileName),
                        "File doesn't exist",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                    return;
                }
                string fileExt = System.IO.Path.GetExtension(fileName);
                if (string.IsNullOrEmpty(fileExt))
                {
                    MessageBox.Show(
                        "The file has no extesion. Actually, it's not that big of a deal, but... long story. Having an extension is mandatory.",
                        "File without extension",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                    return;
                }
                if (fileExt != ".csv")
                {
                    if (
                        MessageBox.Show(
                            string.Format(
                                "Are you sure that you want to split [{0}] file? Based on file the extension, it is not a CSV file.",
                                fileExt
                            ),
                            "Wrong file extension",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question,
                            MessageBoxResult.No
                        ) == MessageBoxResult.No
                    )
                    {
                        return;
                    }
                }

                string outDir = outputDir.Text.Trim();
                bool outDirWasNotProvided = false;
                if (string.IsNullOrEmpty(outDir))
                {
                    outDir = Path.Combine(
                        Path.GetDirectoryName(fileName),
                        Path.GetFileNameWithoutExtension(fileName)
                    );
                    outDirWasNotProvided = true;
                }
                if (!Directory.Exists(outDir))
                {
                    try { Directory.CreateDirectory(outDir); }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            string.Format(
                                "Creating directory [{0}] failed.{1}Details:{2}",
                                outDir,
                                Environment.NewLine,
                                ex.Message
                            ),
                            "Couldn't create directory",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                        return;
                    }
                }
                else
                {
                    if (outDirWasNotProvided) // it is a fallback directory
                    {
                        MessageBox.Show(
                            new StringBuilder()
                                .Append("Since you haven't speficied the output directory, the application ")
                                .Append($"was going to use [{outDir}], but this directory already exists. ")
                                .Append("Won't proceed in order to avoid accidentially overriding important files.")
                                .ToString(),
                            "Output directory already exists",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error
                        );
                        return;
                    }
                }

                var filesEncodingName = ((ComboBoxItem)cmbx_filesEncoding.SelectedItem).Content.ToString();

                sourceCSVpath.IsEnabled = false;
                outputDir.IsEnabled = false;

                btn_spitDat.Visibility = Visibility.Hidden;
                btn_stopDat.Visibility = Visibility.Visible;

                bdr_drop.Visibility = Visibility.Hidden;
                pb_splitting.Visibility = Visibility.Visible;
                txbx_log.Visibility = Visibility.Visible;

                // --- все проверки прошли, начинаем
                
                txbx_log.Text += string.Format(
                    "[{0}] The splitting has started. Output directory: {2}{1}",
                    DateTime.Now.ToString(),
                    Environment.NewLine,
                    outDir
                );

                cts = new CancellationTokenSource();

                var task = Task.Run(
                    () => splitTheFile(
                        outDir,
                        fileName,
                        fileExt,
                        filesEncodingName,
                        cts.Token
                    ),
                    cts.Token
                );
                Task uiTask = task.ContinueWith(
                    (continuation) =>
                    {
                        resetAll();
                    }, TaskScheduler.FromCurrentSynchronizationContext()
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format("Some error has occured. Details:{0}{1}",
                        Environment.NewLine, ex.Message),
                    "Splitting failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                resetAll();
            }
        }

        private void splitTheFile(
            string outDir,
            string fileName,
            string fileExt,
            string filesEncodingName,
            CancellationToken cancellationToken
            )
        {
            int partsCounter = 1;
            bool progressWasSet = false;
            Stopwatch sw = Stopwatch.StartNew();

            int bufferCap = Properties.Settings.Default.linesPerFile;
            if (bufferCap > 999999) // если пользователь вмочил буфер на лимон и более - урезать
            {
                bufferCap = 300000;
                MessageBox.Show(
                    string.Format("You have set a pretty damn big buffer. It will be reduced to {0}.",
                        bufferCap.ToString()),
                    "Buffer is too damn big",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
            else
            {
                if (bufferCap < 1000) // если в буфер не пролезают даже кварки - расширить
                {
                    bufferCap = 50000;
                    MessageBox.Show(
                        string.Format(
                            "You have set a pretty damn small buffer. It will be increased to {0}.",
                            bufferCap.ToString()
                        ),
                        "Buffer is too damn small",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
            }
            try
            {
                FileInfo fi = new FileInfo(Path.Combine(outDir, fileName));

                var filesEncoding = Encoding.GetEncoding("utf-8"); // fallback encoding
                try
                {
                    filesEncoding = Encoding.GetEncoding(filesEncodingName);
                }
                catch // (ArgumentException ex)
                {
                    MessageBox.Show(
                        "Could not find the specified encoding, will default to UTF-8.",
                        "Unknown encoding",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }

                using (var reader = new StreamReader(fileName, filesEncoding))
                {
                    string header = string.Empty;
                    if (Properties.Settings.Default.addHeader) // читаем шапку CSV "таблицы", если это надо
                    {
                        header = reader.ReadLine();
                    }
                
                    int currentLine = 0;
                    string line;
                    List<string> buffer = new List<string>();
                    while ((line = reader.ReadLine()) != null)
                    {
                        // если буфер достиг максимума, скинуть его в файл и опустошить
                        if (currentLine >= bufferCap)
                        {
                            if (!string.IsNullOrEmpty(header)) { buffer.Insert(0, header); }
                            
                            File.WriteAllLines(
                                string.Format(
                                    "{0}_part{1}{2}",
                                    Path.Combine(
                                        outDir,
                                        Path.GetFileNameWithoutExtension(fileName)
                                    ),
                                    partsCounter.ToString(),
                                    ".csv" // fileExt
                                ),
                                buffer,
                                filesEncoding
                            );
                            buffer.Clear();
                            currentLine = 0;
                            this.Dispatcher.Invoke(
                                DispatcherPriority.Normal,
                                new Action(
                                    () =>
                                    {
                                        txbx_log.Text += string.Format("[{0}] Part {1} was splitted.{2}",
                                            DateTime.Now.ToString(),
                                            partsCounter.ToString(),
                                            Environment.NewLine
                                        );
                                        txbx_log.ScrollToEnd();
                                    }
                                )
                            );
                            
                            // оцениваем прогресс
                            if (!progressWasSet)
                            {
                                FileInfo fiPart = new FileInfo(
                                    string.Format("{0}_part{1}{2}",
                                        Path.Combine(
                                            outDir,
                                            Path.GetFileNameWithoutExtension(fileName)
                                        ),
                                        partsCounter.ToString(),
                                        ".csv"
                                    )
                                ); // fileExt
                                this.Dispatcher.Invoke(
                                    DispatcherPriority.Normal,
                                    new Action(
                                        () =>
                                        {
                                            // поправка на ветер
                                            pb_splitting.Maximum = fi.Length / fiPart.Length + 30;
                                            pb_splitting.Value = 0;
                                        }
                                    )
                                );
                                progressWasSet = true;
                            }

                            partsCounter++;
                            this.Dispatcher.Invoke(
                                DispatcherPriority.Normal,
                                new Action(
                                    () =>
                                    {
                                        pb_splitting.Value++;
                                    }
                                )
                            );
                            // проверяем, не был ли процесс отменён
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                        else
                        {
                            buffer.Add(line);
                            currentLine++;
                        }
                    }
                    // если в буфере что-то есть, то скинуть в файл
                    if (buffer.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(header)) { buffer.Insert(0, header); }
                        File.WriteAllLines(
                            string.Format("{0}_part{1}{2}",
                                Path.Combine(
                                    outDir,
                                    Path.GetFileNameWithoutExtension(fileName)
                                ),
                                partsCounter.ToString(),
                                ".csv" // fileExt
                            ),
                            buffer,
                            filesEncoding
                        );
                        buffer.Clear();
                    }
                }

                // останавливаем таймер
                sw.Stop();
                // прогресс-бар на 100%
                this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        pb_splitting.Value = pb_splitting.Maximum;
                    }
                ));
                MessageBox.Show(
                    string.Format("The file was successfully splitted into {0} parts. It took {1}.",
                        partsCounter.ToString(),
                        sw.Elapsed.ToString()),
                    "Splitting completed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    MessageBox.Show(
                        string.Format("User has canselled the operation. The file has been splitted into {0} parts so far.",
                            partsCounter.ToString()),
                        "Operation cancelled",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
                else
                {
                    MessageBox.Show(
                        string.Format("Some error has occured during the splitting.{0}Details:{1}",
                            Environment.NewLine, ex.Message),
                        "Process failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        }

        private void btn_stopDat_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
            resetAll();
        }

        private void f1pressed(object sender, ExecutedRoutedEventArgs e)
        {
            HelpWindow hlp = new HelpWindow();
            hlp.ShowDialog();
        }

        private void sourceCSVpath_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "CSV file (*.csv)|*.csv";
            if ((bool)dialog.ShowDialog(this)) { sourceCSVpath.Text = dialog.FileName; }
        }

        private void outputDir_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
            if ((bool)dialog.ShowDialog(this)) { outputDir.Text = dialog.SelectedPath + @"\"; }
        }
    }
}
