using DMRatt.Tasks;
using RestSharp;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace DMRatt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DB Database;
        private FileManager FM = new FileManager();
        //private FileManager OmitManager = null;
        //private string OmittedContacts = "Radio-ID,Callsign,First-Name,City,State/Prov,Country" + Environment.NewLine;
        //private bool SaveOmitted = false;
        //private bool RemoveExpired = true;
        //private string CompiledList = "Radio-ID,Callsign,First-Name,City,State/Prov,Country" + Environment.NewLine;
        public MainWindow()
        {
            InitializeComponent();
            Database = new DB();
            Database.InitDatabase();
            ThreadControl.Maximum = Environment.ProcessorCount;
            ThreadControl.Value = Math.Round(ThreadControl.Maximum / 3);

        }

        #region "Handlers"
        private async void OutputSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => SelectFile());
            if (OutputFileSelection.Text.Length > 0)
            {
                RunButton.IsEnabled = true;
            }
            else
            {
                RunButton.IsEnabled=false;
            }
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (OutputFileSelection.Text.Length < 1)
            {
                MessageBox.Show("You need to select an ouput destination first.");
                // TODO: verify destination is writable
            }
            else
            {
                await Task.Run(() => Process());
            }
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ThreadDisplay.Content = ThreadControl.Value;
        }

        #endregion

        #region "Threads"
        private void SelectFile()
        {
            // Open file picker
            FM.SelectOutputFile("Comma-Separated List (*.csv)|*.csv");
            // Place filepath into Path box
            OutputFileSelection.Dispatcher.BeginInvoke(
                (Action)(() =>
                {
                    OutputFileSelection.Text = FM.GetOutputFile();
                })
            );
        }

        private async void Process()
        {
            FM.ResetFile();

            // Run Options
            Boolean RemoveExpired = false;
            Boolean SaveOmitted   = false;
            Boolean FreshFCC      = false;
            Boolean FreshCA       = false;
            Boolean FreshDMR      = false;
            int     ThreadCount   = 2    ;
            Boolean PurgeDB       = false;
            await OptionIncludeInvalidCallsigns.Dispatcher.BeginInvoke(
                (Action)(() =>
                {
                    RemoveExpired = !(bool)OptionIncludeInvalidCallsigns.IsChecked;
                }
                )
            );
            await OptionSaveOmittedCallsigns.Dispatcher.BeginInvoke(
                (Action)(() =>
                {
                    SaveOmitted = (bool)OptionSaveOmittedCallsigns.IsChecked;
                }
                )
            );
            await OptionGetFreshDMR.Dispatcher.BeginInvoke(
                (Action)(() =>
                {
                    FreshDMR = (bool)OptionGetFreshDMR.IsChecked;
                }
                )
            );
            await OptionGetFreshFCC.Dispatcher.BeginInvoke(
                (Action)(() =>
                {
                    FreshFCC = (bool)OptionGetFreshFCC.IsChecked;
                }
                )
            );
            await OptionGetFreshCanada.Dispatcher.BeginInvoke(
                (Action)(() =>
                {
                    FreshCA = (bool)OptionGetFreshCanada.IsChecked;
                }
                )
            );
            await ThreadControl.Dispatcher.BeginInvoke(
                (Action)(() =>
                {
                    ThreadCount = (int)ThreadControl.Value;
                }
                )
            );
            await OptionPurgeDB.Dispatcher.BeginInvoke(
                (Action)(() =>
                {
                    PurgeDB = (bool)OptionPurgeDB.IsChecked;
                }
                )
            );
            //if (SaveOmitted)
            //{
            //    OmitManager = new FileManager();
            //    OmitManager.SetOutputFile(FM.GetOutputFile().Substring(0, FM.GetOutputFile().Length - 3) + "omitted.csv");
            //    OmitManager.ResetFile();
            //}

            if (PurgeDB)
            {
                Database.Licensees.DeleteAll();
            }

            Boolean OverrideSkipQueries = Database.Licensees.Count() <= 5;
            LicenseFetch Fetcher = new LicenseFetch();
            // Process Contacts
            if (FreshDMR || OverrideSkipQueries)
            {
                SetStatusAsync("Loading DMR Contacts");
                ResetProgressBar(2);
                Fetcher.DMR(Database, SetProgressAsync, ThreadCount);
            }

            // Process FCC Database
            if (FreshFCC || OverrideSkipQueries)
            {
                SetStatusAsync("Loading FCC Licenses");
                ResetProgressBar(2);
                
                Fetcher.FCC(Database, SetProgressAsync, ThreadCount);
            }


            //SetStatusAsync("Loading Canadian Licenses");


            // Save File(s)
            //SetStatusAsync("Saving Files");
            //FM.WriteFile(CompiledList);
            //OmitManager.WriteFile(OmittedContacts);


            // TODO: Windows alert sound
            SetStatusAsync("Complete!");
        }

        private async void SetStatusAsync(string msg)
        {
            if (msg != null)
            {
                await CurrentTaskLabel.Dispatcher.BeginInvoke(
                    (Action)(() =>
                    {
                        CurrentTaskLabel.Content = msg;
                    }
                    )
                );
            }
        }

        private async void SetProgressAsync(int val)
        {
            // TODO: Make cleaner interface
            await StatusBar.Dispatcher.BeginInvoke(
                (Action)(() => {
                    //CurrentTaskLabel.Content = msg;
                    if (val > StatusBar.Maximum)
                    {
                        StatusBar.Maximum = val;
                    }
                    else
                    {
                        CountLabel.Content = "Completed: " + val.ToString();
                    }
                    StatusBar.Value = val;
                }
                )
            );
        }
        private async void ResetProgressBar(int val)
        {
            await StatusBar.Dispatcher.BeginInvoke(
                (Action)(() => {
                    StatusBar.Maximum = val;
                    StatusBar.Value = val;
                }
                )
            );
        }
        private async void AppendLogAsync(string val)
        {
            await Log.Dispatcher.BeginInvoke(
                (Action)(() => {
                    //CurrentTaskLabel.Content = msg;
                    Log.Text += val + Environment.NewLine;
                    Log.ScrollToEnd();
                }
                )
            );
        }
        #endregion

    }
}
