using HTML2PDF.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HTML2PDF.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string _SelectedSourcePath;
        private string _SelectedDestinationPath;
        private string _LoadingStatusLabel;
        private ICommand _DoConversionCommand;
        private ICommand _DoSelectSourceCommand;
        private ICommand _DoSelectDestinationCommand;
        /// <summary>
        /// User-selected document source URI
        /// </summary>
        public string SelectedSourcePath { get => _SelectedSourcePath; set { _SelectedSourcePath = value; NotifyPropertyChanged(); } }
        /// <summary>
        /// User-selected document destination URI
        /// </summary>
        public string SelectedDestinationPath { get => _SelectedDestinationPath; set { _SelectedDestinationPath = value; NotifyPropertyChanged(); } }
        /// <summary>
        /// User-facing status message
        /// </summary>
        public string LoadingStatusLabel { get => _LoadingStatusLabel; set { _LoadingStatusLabel = value; NotifyPropertyChanged(); } }
        /// <summary>
        /// User initiates the conversion from source to destination
        /// </summary>
        public ICommand DoConversionCommand => _DoConversionCommand ?? (_DoConversionCommand = new RelayCommand.RelayCommand(DoConversion, CanDoConversion));
        /// <summary>
        /// User initiates setting the source document URI
        /// </summary>
        public ICommand DoSelectSourceCommand => _DoSelectSourceCommand ?? (_DoSelectSourceCommand = new RelayCommand.RelayCommand(DoSelectSource, CanDoSelectSource));
        /// <summary>
        /// User initiates setting the destination document URI
        /// </summary>
        public ICommand DoSelectDestinationCommand => _DoSelectDestinationCommand ?? (_DoSelectDestinationCommand = new RelayCommand.RelayCommand(DoSelectDestination, CanDoSelectDestination));
        /// <summary>
        /// Is user currently allowed to set source URI?
        /// </summary>
        /// <returns></returns>
        private bool CanDoSelectSource()
        {
            return true;
        }
        /// <summary>
        /// Select source using File Open dialog
        /// </summary>
        private void DoSelectSource()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "HTML File (*.xhtml; *.html)|*.xhtml; *.html",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            };
            var userDidSelectAPath = openFileDialog.ShowDialog();

            if (userDidSelectAPath.HasValue && userDidSelectAPath.Value)
            {
                SelectedSourcePath = openFileDialog.FileName;
            }
        }
        /// <summary>
        /// Is user currently allowed to set destination URI?
        /// </summary>
        /// <returns></returns>
        private bool CanDoSelectDestination()
        {
            return true;
        }
        /// <summary>
        /// Select destination URI using File Save dialog
        /// </summary>
        private void DoSelectDestination()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "PDF File (*.pdf)|*.pdf",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = "output.pdf",
            };
            var userDidSelectAPath = saveFileDialog.ShowDialog();
            if (userDidSelectAPath.HasValue && userDidSelectAPath.Value)
            {
                SelectedDestinationPath = saveFileDialog.FileName;
            }
        }
        /// <summary>
        /// Is User currently allowed to initiate document conversion?
        /// </summary>
        /// <returns></returns>
        public bool CanDoConversion()
        {
            return SelectedSourcePath?.Length > 0 && SelectedDestinationPath?.Length > 0;
        }
        /// <summary>
        /// Do the work of document conversion
        /// Async so that CPU-heavy operations can happen in the background.
        /// </summary>
        public async void DoConversion()
        {
            LoadingStatusLabel = "Converting...";
            //Map error types to user-facing messages
            var errorMessages = new Dictionary<Type, string>() {
                { typeof(NotSupportedException),"Unsupported URI!" },
                { typeof(ArgumentException),"Bad Source URI. Illegal characters?" },
                { typeof(FileNotFoundException),"File could not be found! Make sure Source exists." },
                { typeof(AggregateException),  "Multiple errors occurred."}
            };
            LoadingStatusLabel = await Task.Run(async () =>
              {
                  try
                  {
                      var conversionModel = new HTMLtoPDFModel(SelectedSourcePath, SelectedDestinationPath);
                      await conversionModel.Convert();
                      Process p = new Process();
                      p.StartInfo.FileName = SelectedDestinationPath;
                      p.StartInfo.Verb = "open";
                      p.Start();
                  }
                  catch (Exception ex)
                  {
                      var t = ex.GetType();
                      if (errorMessages.ContainsKey(t))
                      {
                          //Error is an expected type of error
                          return "Error: " + errorMessages[ex.GetType()];
                      }
                      else
                      {
                          //Error is unexpected.
                          return "Error: " + ex.Message;
                      }
                  }
                  return "Done!";
              });
        }
    }
}
