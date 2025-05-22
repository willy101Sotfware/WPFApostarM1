using System.Windows;
using WPFApostar.Domain;
using WPFApostar.Models;

namespace WPFApostar
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetUserControl();
        }

        private void SetUserControl()
        {
            try
            {
                if (Utilities.navigator == null)
                {
                    Utilities.navigator = new Navigation();
                }

                var arduinoPort = Utilities.GetConfiguration("Port");
                var dispenserDenominations = Utilities.GetConfiguration("dispenserDenominations");

                //para no suar perifericos

               //PeripheralController.Initialize(arduinoPort, dispenserDenominations);

                WPKeyboard.Keyboard.ConsttrucKeyyboard(WPKeyboard.Keyboard.EStyle.style_2);

                Utilities.navigator.Navigate(UserControlView.Config);

                DataContext = Utilities.navigator;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
