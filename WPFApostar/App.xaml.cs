using System.Diagnostics;
using System.Windows;
using WPFApostar.Domain;

namespace WPFApostar
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            try
            {
                var procesos = Process.GetProcessesByName("WPFApostar");

                var process = Process.GetCurrentProcess();


                foreach (var item in procesos)
                {

                    if (process.Id != item.Id)
                    {
                        item.Kill();
                    }

                }
            }
            catch (Exception ex)
            {
                ShowModalError(ex.Message);
            }
        }

        /// <summary>
        /// Encargado de cerrar los otros procesos(si los hay) 
        /// y sólo dejar el principal
        /// </summary>
        private void FinishProcess()
        {
            var procesos = Process.GetProcessesByName("WPFApostar");
            foreach (var item in procesos)
            {
                item.CloseMainWindow();
            }
        }

        private void ShowModalError(string description, string message = "")
        {

            Utilities.RestartApp();
        }

    }
}
