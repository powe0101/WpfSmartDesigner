using System.Windows;

namespace WpfSmartDesigner.Engine
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize services
            var pipeServer = new PipeServerService();
            pipeServer.Start();

            // Create and show main window
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Cleanup
            base.OnExit(e);
        }
    }
}
