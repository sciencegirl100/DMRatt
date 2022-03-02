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

        private FileManager FM = new FileManager();
        public MainWindow()
        {
            InitializeComponent();
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
            }
            else
            {
                await Task.Run(() => Process());
            }
        }

        // Commented out until background task can be killed
        //private void StopButton_Click(object sender, RoutedEventArgs e)
        //{
        //    // Kill background task(s)
        //}
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
            // Download Latest DMR Contacts
            SetStatusAsync("Create temporary holding file");
            string TempFileName = System.IO.Path.GetTempFileName();
            FileInfo FI = new FileInfo(TempFileName);
            FI.Attributes = FileAttributes.Temporary;
            var WC = new WebClient();
            string FullContactList = "https://www.radioid.net/static/user.csv";
            string cplusList = "https://www.radioid.net/static/cplus.csv";
            SetStatusAsync("Download master DMR contacts list");
            WC.DownloadFile(FullContactList, TempFileName);
            Console.WriteLine(TempFileName);
            FM.ResetFile();
            FM.AppendLine("Radio-ID,Callsign,First-Name,City,State/Prov,Country");
            var LookupClient = new RestClient("https://callook.info/");
            // Load Contacts into memory
            var CallsignCount = File.ReadAllLines(TempFileName).Length;
            await StatusBar.Dispatcher.BeginInvoke(
                (Action)(() => {
                    StatusBar.Maximum = CallsignCount;
                }
                )
            );

            // Run Options
            var RemoveExpired = true;
            await OptionIncludeInvalidCallsigns.Dispatcher.BeginInvoke(
                (Action)(() =>
                {
                    RemoveExpired = !(bool)OptionIncludeInvalidCallsigns.IsChecked;
                }
                )
            );
            var SaveOmitted = false;
            await OptionSaveOmittedCallsigns.Dispatcher.BeginInvoke(
                (Action)(() =>
                {
                    SaveOmitted = (bool)OptionSaveOmittedCallsigns.IsChecked;
                }
                )
            );
            FileManager OmitManager = null;
            if (SaveOmitted)
            {
                OmitManager = new FileManager();
                OmitManager.SetOutputFile(FM.GetOutputFile().Substring(0,FM.GetOutputFile().Length-3) + "omitted.csv");
                OmitManager.ResetFile();
                OmitManager.AppendLine("Radio-ID,Callsign,First-Name,City,State/Prov,Country");
            }

            SetStatusAsync("Read list into memory");
            var CSVReader = new StreamReader(File.OpenRead(TempFileName));
            Int32 LineCounter = 1;
            bool ToAppend = false;
            while (!CSVReader.EndOfStream)
            {
                var line = CSVReader.ReadLine();
                if (line != null && line.Length >= 1)
                {
                    // For each data row in the CSV...
                    if (LineCounter > 1)
                    {
                        // Extract Callsign (Column 2)
                        ToAppend = false;
                        var elements = line.Split(',');
                        string callsign = elements[1];
                        SetStatusAsync("Query data about " + callsign);
                        // Lookup callsign on Callook
                        var LookupRequest = new RestRequest(callsign+"/json", Method.Get);
                        var APIResult = await LookupClient.ExecuteAsync(LookupRequest);
                        // TODO: Handle HTTP Errors
                        var CallsignStatus = APIResult.Content.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                        //Console.WriteLine(LineCounter + "> " + callsign + ":" + CallsignStatus[1]);
                        if (RemoveExpired)
                        {
                            if (!CallsignStatus[1].Contains("INVALID"))
                            {
                                // Append to final listing
                                ToAppend = true;
                            }
                            else if (SaveOmitted && OmitManager != null)
                            {
                                OmitManager.AppendLine(line);
                            }
                        }
                        else
                        {
                            ToAppend = true;
                        }
                    }
                    if (ToAppend)
                    {
                        FM.AppendLine(line);
                    }
                }

                LineCounter++;
                SetProgressAsync(LineCounter);
            }
            SetStatusAsync("Remove temporary master list file");
            CSVReader.Close();
            File.Delete(TempFileName);
            // TODO: Windows alert sound
            SetStatusAsync("Complete!");
        }

        private async void SetStatusAsync(string msg)
        {
            await CurrentTaskLabel.Dispatcher.BeginInvoke(
                (Action)(() => {
                    CurrentTaskLabel.Content = msg;
                }
                )
            );
        }

        private async void SetProgressAsync(int val)
        {
            await StatusBar.Dispatcher.BeginInvoke(
                (Action)(() => {
                    //CurrentTaskLabel.Content = msg;
                    StatusBar.Value = val;
                }
                )
            );
        }
        #endregion
    }
}
