using LiteDB;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DMRatt.Tasks
{
    internal class LicenseFetch
    {
        // DMR Contacts: https://www.radioid.net/static/user.csv
        // 
        // US: https://data.fcc.gov/download/pub/uls/complete/l_amat.zip (Weekly)
        //     
        // Canada: http://apc-cap.ic.gc.ca/datafiles/amateur_delim.zip
        //
        // Japan: http://motobayashi.net/callsign/licensesearchapi.shtml?target=<CALLSIGN>
        //    Check if "<TABLE[ A-Z0-9="]+>(.*)<\/TABLE>" contains more than one set of TR tags
        // 

        public void FCC(DB Database, Action<int> setProgressAsync, int ThreadCount)
        {
            // Load Weekly Database
            string TempFileName = System.IO.Path.GetTempFileName();
            string TempFolderName = System.IO.Path.GetTempPath() + "dbexpand";
            if (!Directory.Exists(TempFolderName))
            {
                Directory.CreateDirectory(TempFolderName);
            }
            else
            {
            #if DEBUG
            #else
                Directory.Delete(TempFolderName);
            #endif
            }
            #if DEBUG
            #else
            var WC = new WebClient();
            FileInfo FI = new FileInfo(TempFileName);
            FI.Attributes = FileAttributes.Temporary;
            WC.DownloadFile("https://data.fcc.gov/download/pub/uls/complete/l_amat.zip", TempFileName);
            ZipFile.ExtractToDirectory(TempFileName, TempFolderName);
            #endif

            // Pull in Amateur license table with stream
            var FCCReader = new StreamReader(File.OpenRead(TempFolderName + "\\HD.dat"));
            List<string>[] ThreadInput = new List<string>[ThreadCount];
            Int32 LineCounter = 0;
            while (!FCCReader.EndOfStream)
            { 
                var line = FCCReader.ReadLine();
                if (ThreadInput[(LineCounter) % ThreadCount] == null)
                {
                    ThreadInput[(LineCounter) % ThreadCount] = new List<string>();
                }     
                ThreadInput[(LineCounter) % ThreadCount].Add(line);
                LineCounter++;
            }

            foreach (List<String> Input in ThreadInput)
            {
                Task.Run(() => ProcessFCCLicenseeWorkerThread(Input, Database));
            }

            // Delete Weekly DB Download
            //DeleteDirectory(TempFolderName);
            File.Delete(TempFileName);

            // TODO: Load Daily Databases
        }

        public void CA(DB Database, Action<int> setProgressAsync, int ThreadCount)
        {
            
        }

        public void DMR(DB Database, Action<int> setProgressAsync, int ThreadCount)
        {
            // Download Database CSV
            string TempFileName = System.IO.Path.GetTempFileName();
            var WC = new WebClient();
            FileInfo FI = new FileInfo(TempFileName);
            FI.Attributes = FileAttributes.Temporary;
            WC.DownloadFile("https://www.radioid.net/static/user.csv", TempFileName);

            // Set Progressbar max
            var CallsignCount = File.ReadAllLines(TempFileName).Length;
            setProgressAsync(CallsignCount);

            // Loop through Database CSV and process into local DB
            var CSVReader = new StreamReader(File.OpenRead(TempFileName));
            Int32 LineCounter = 1;
            List<string>[] ThreadInput = new List<string>[ThreadCount];
            while (!CSVReader.EndOfStream)
            {
                var line = CSVReader.ReadLine();
                if (line != null && line.Length >= 1 && LineCounter > 1)
                {
                    // For each data row in the CSV...
                    // TODO: Limit to 2X logical proc count
                    //Task.Run(() => ProcessDMRContactRow(line, LineCounter, Database));
                    if(ThreadInput[(LineCounter - 1) % ThreadCount] == null)
                    {
                        ThreadInput[(LineCounter - 1) % ThreadCount] = new List<string>();
                    }
                    //Console.WriteLine(line);
                    //Console.WriteLine(ThreadCount);
                    //Console.WriteLine(LineCounter);        
                    ThreadInput[(LineCounter - 1) % ThreadCount].Add(line);
                }

                LineCounter++;
                setProgressAsync(LineCounter);
            }
            // Run Split Threads
            foreach (List<string> InputLines in ThreadInput)
            {
                Task.Run(() => ProcessDMRContactWorkerThread(InputLines, Database));
            }

            // Remove Temp Database CSV File
            //SetStatusAsync("Remove temporary master list file");
            setProgressAsync(CallsignCount);
            CSVReader.Close();
            File.Delete(TempFileName);
        }

        private void ProcessFCCLicenseeWorkerThread(List<string> Lines, DB Database)
        {
            foreach (string line in Lines)
            {
                ProcessFCCLicensee(line, Database);
            }
        }

        private void ProcessFCCLicensee(string line, DB Database)
        {
            var data = line.Split('|');
            var nullTime = DateTime.Parse("01/01/0001");
            string QueryInput = data[4];
            var license = Database.Licensees.FindOne(x => x.Callsign.Equals(QueryInput));
            if (license == null)
            {
                license = new Licensee();
                license.Callsign = data[4];
                license.GrantDate = (data[7].Length > 0 ? DateTime.Parse(data[7]) : nullTime);
                license.ExpireDate = (data[8].Length > 0 ? DateTime.Parse(data[8]) : nullTime);
                license.CancelDate = (data[9].Length > 0 ? DateTime.Parse(data[9]) : nullTime);
                license.Status = data[5];
                license.EffectiveDate = (data[42].Length > 0 ? DateTime.Parse(data[42]) : nullTime);
                license.LastActionDate = (data[43].Length > 0 ? DateTime.Parse(data[43]) : nullTime);
                license.FirstName = data[29];
                license.LastName = data[31];
                license.MiddleInitial = data[30];
                license.Suffix = data[32];
                license.Title = data[33];
                Database.Licensees.Insert(license);
                if (license.Callsign == "AA6IT")
                {
                    Console.WriteLine("INSERT");
                }
            }
            else
            {
                // Update if different
                if (data[7].Length > 0 && license.GrantDate != DateTime.Parse(data[7]))
                {
                    license.GrantDate = DateTime.Parse(data[7]);
                }
                if (data[8].Length > 0 && license.ExpireDate != DateTime.Parse(data[8]))
                {
                    license.ExpireDate = DateTime.Parse(data[8]);
                }
                if (data[9].Length > 0 && license.CancelDate != DateTime.Parse(data[9]))
                {
                    license.CancelDate = DateTime.Parse(data[9]);
                }
                if (data[42].Length > 0 && license.EffectiveDate != DateTime.Parse(data[42]))
                {
                    license.EffectiveDate = DateTime.Parse(data[42]);
                }
                if (data[43].Length > 0 && license.LastActionDate != DateTime.Parse(data[43]))
                {
                    license.LastActionDate = DateTime.Parse(data[43]);
                }

                if (license.Status != data[5])
                {
                    license.Status = data[5];
                }
                if (license.FirstName != data[29])
                {
                    license.FirstName = data[29];
                }
                if (license.LastName != data[31])
                {
                    license.LastName = data[31];
                }
                if (license.MiddleInitial != data[30])
                {
                    license.MiddleInitial = data[30];
                }
                if (license.Suffix != data[32])
                {
                    license.Suffix = data[32];
                }
                if (license.Title != data[33])
                {
                    license.Title = data[33];
                }

                Database.Licensees.Update(license);
            }
        }

        private void ProcessDMRContactWorkerThread(List<string> Lines, DB Database)
        {
            foreach(string line in Lines)
            {
                ProcessDMRContactRow(line, Database);
            }
        }

        private void ProcessDMRContactRow(string line, DB Database)
        {
            // Valid Callsign Entry, add to database
            var data = line.Split(',');
            string CallsignLookup = data[1];
            // Run Lookup based on Callsign NOT RadioId
            var license = Database.Licensees.FindOne(x => x.Callsign.Equals(CallsignLookup));
            if (license == null)
            {
                // RadioId was not found, create new entry
                var LicenseeToAdd = new Licensee();
                LicenseeToAdd.RadioIds = new string[] { data[0] };
                LicenseeToAdd.Callsign = data[1];
                LicenseeToAdd.DMRFirstName = data[2];
                LicenseeToAdd.DMRLastName = data[3];
                LicenseeToAdd.DMRCity = data[4];
                LicenseeToAdd.DMRState = data[5];
                LicenseeToAdd.DMRCountry = data[6];
                Database.Licensees.Insert(LicenseeToAdd);
                if (LicenseeToAdd.Callsign == "AA6IT")
                {
                    Console.WriteLine("INSERT");
                }
            }
            else
            {
                // Update existing Licensee Entry
                Licensee UpdateLicensee = license;
                if (UpdateLicensee.RadioIds == null)
                {
                    UpdateLicensee.RadioIds = new string[] { };
                }
                UpdateLicensee.RadioIds.Append(data[0]);
                Database.Licensees.Update(UpdateLicensee);
            }
        }
        private static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
    }
}
