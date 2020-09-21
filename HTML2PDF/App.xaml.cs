using Autofac;
using HTML2PDF.Models;
using HTML2PDF.ViewModels;
using System;
using System.Windows;

namespace HTML2PDF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {

        }

        private static Autofac.IContainer Container
        { get; set; }

        public static Autofac.IContainer GetContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<HTMLtoPDFService>().As<IPdfConverterService>();
            builder.RegisterType<PdfConverterViewModel>();
            builder.RegisterType<MainWindowViewModel>();
            return builder.Build();
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose;
                Container = GetContainer();
                using (var scope = Container.BeginLifetimeScope())
                {
                    var mainWindow = new MainWindow() { DataContext = scope.Resolve<MainWindowViewModel>() };
                    mainWindow.Show();
                }
                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}