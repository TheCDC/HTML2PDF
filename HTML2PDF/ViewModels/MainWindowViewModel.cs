using HTML2PDF.Models;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
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

        public string SelectedSourcePath { get => _SelectedSourcePath; set { _SelectedSourcePath = value; NotifyPropertyChanged(); } }
        public string SelectedDestinationPath { get => _SelectedDestinationPath; set { _SelectedDestinationPath = value; NotifyPropertyChanged(); } }
        private string webBrowserPath;
        public ICommand DoConversionCommand => _DoConversionCommand ?? (_DoConversionCommand = new RelayCommand.RelayCommand(DoConversion, CanDoConversion));
        public ICommand DoSelectSourceCommand => _DoSelectSourceCommand ?? (_DoSelectSourceCommand = new RelayCommand.RelayCommand(DoSelectSource, CanDoSelectSource));
        public ICommand DoSelectDestinationCommand => _DoSelectDestinationCommand ?? (_DoSelectDestinationCommand = new RelayCommand.RelayCommand(DoSelectDestination, CanDoSelectDestination));

        public string WebBrowserPath { get => webBrowserPath; set { webBrowserPath = value; NotifyPropertyChanged(); } }

        public string LoadingStatusLabel { get => _LoadingStatusLabel; set { _LoadingStatusLabel = value; NotifyPropertyChanged(); } }

        private string _LoadingStatusLabel;

        private bool CanDoSelectDestination()
        {
            return true;
        }

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
        private bool CanDoSelectSource()
        {
            return true;
        }
        private ICommand _DoConversionCommand;
        private ICommand _DoSelectSourceCommand;
        private ICommand _DoSelectDestinationCommand;

        public bool CanDoConversion()
        {
            return SelectedSourcePath?.Length > 0 && SelectedDestinationPath?.Length > 0;
        }
        public async void DoConversion()
        {
            await Task.Run(() =>
            {
                var conversionModel = new HTMLtoPDFModel(SelectedSourcePath, SelectedDestinationPath);
                conversionModel.Convert();
                Process p = new Process();
                p.StartInfo.FileName = SelectedDestinationPath;
                p.StartInfo.Verb = "open";
                p.Start();


            });

        }
    }
}
