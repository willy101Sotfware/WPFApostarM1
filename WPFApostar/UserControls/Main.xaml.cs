using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Grabador.Transaccion;
using WPFApostar.Classes;

namespace WPFApostar.UserControls
{
    /// <summary>
    /// Lógica de interacción para Main.xaml
    /// </summary>
    public partial class Main : UserControl
    {

        private ImageSleader _imageSleader;

        public Main()
        {
            InitializeComponent();
            CLSGrabador.FinalizarGrabacion();
                    
 
        }

        private void Init()
        {
            try
            {
              
                //AdminPayPlus.NotificateInformation();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }


        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            var url = Utilities.GetConfiguration("VideoPublish");


            MediaElement mediaElement = (MediaElement)sender;
            mediaElement.Source = new Uri(url); // Establece la fuente del video
            mediaElement.Volume = 0;
            mediaElement.Position = TimeSpan.Zero;

        }
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GC.Collect();
            Utilities.navigator.Navigate(UserControlView.Menu);
        }
    }
}
