using Microsoft.Win32;
using System;
using System.IO;

namespace DMRatt.Tasks
{
    internal class FileManager
    {
        private string SelectedOutputFile;
        private StreamWriter OutputWriter;

        public string GetOutputFile()
        {
            return this.SelectedOutputFile;
        }

        public void SetOutputFile(string filename)
        {
            this.SelectedOutputFile = filename;
        }

        public void SelectOutputFile()
        {
            SelectOutputFile(null);
        }

        public void SelectOutputFile(String RequiredFileTypes)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (RequiredFileTypes != null)
            {
                saveFileDialog.Filter = RequiredFileTypes;
            }
            if (saveFileDialog.ShowDialog() == true)
            {
                SelectedOutputFile = saveFileDialog.FileName;
                OutputWriter = File.CreateText(this.SelectedOutputFile);
                OutputWriter.Close();
            }
        }

        public bool Exists()
        {
            return File.Exists(this.SelectedOutputFile);
        }

        public void ResetFile()
        {
            OutputWriter = File.CreateText(this.SelectedOutputFile);
            OutputWriter.Close();
        }
        public void AppendLine(string Data)
        {
            // If file doesn not exist, create
            if (!this.Exists())
            {
                this.ResetFile();
            }
            // Write to file
            OutputWriter = File.AppendText(this.SelectedOutputFile);
            OutputWriter.WriteLine(Data);
            OutputWriter.Close();
        }

    }
}
