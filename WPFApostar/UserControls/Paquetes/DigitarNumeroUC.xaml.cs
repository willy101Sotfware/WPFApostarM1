using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WPFApostar.Classes;
using WPFApostar.Classes.UseFull;
using WPFApostar.Models;
using System.IO;

namespace WPFApostar.UserControls.Paquetes
{
    /// <summary>
    /// Lógica de interacción para DigitarNumeroUC.xaml
    /// </summary>
    public partial class DigitarNumeroUC : UserControl
    {

        public bool txtcedula = false;
        public bool txtvalidar = false;
        Transaction Transaction;
        private TimerGeneric timer;
        public DigitarNumeroUC(Transaction transaction)
        {
            InitializeComponent();
            ActivateTimer();
            Loaded += MainWindow_Loaded;
            TxtNumCel.GotFocus += focusTxtCedula;
            TxtVal.GotFocus += focusTxtvalidar;

            TxtNumCel.LostFocus += TxtNumCel_LostFocus;
            TxtVal.LostFocus += TxtVal_LostFocus;

            TxtNumCel.Foreground = new SolidColorBrush(Colors.Gray);
            TxtVal.Foreground = new SolidColorBrush(Colors.Gray);

            Transaction = transaction;
        }


        private void Btn_ContinuarTouchDown(object sender, EventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            if (validateNum())
            {
                Transaction.NumOperator = TxtNumCel.Text;
                Utilities.navigator.Navigate(UserControlView.ResumenPaquete, Transaction);
            }
            else
            {
                Utilities.ShowModal("Por favor verifica la información ingresada", EModalType.Error);
                ActivateTimer();

            }
        }

        private void Btn_CancelarTouchDown(object sender, EventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);
        }


        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadOperatorAsync();
        }


        private async Task LoadOperatorAsync()
        {
            string paqueteDisponible = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "PaquetesImages");
            string pathL = Path.Combine(paqueteDisponible, Transaction.Operador + ".png");

            if (File.Exists(pathL))
            {
                var imageUri = new Uri(pathL, UriKind.Absolute);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = imageUri;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ImageData.Source = bitmap;

            }

        }


        private void TxtNumCel_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNumCel.Text))
            {
                TxtNumCel.Text = "Número de celular";
                TxtNumCel.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private void TxtVal_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtVal.Text))
            {
                TxtVal.Text = "Confirmar";
                TxtVal.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }




        private void focusTxtCedula(object sender, RoutedEventArgs e)
        {
            txtcedula = true;
            txtvalidar = false;
            if (TxtNumCel.Text == "Número de celular")
            {
                TxtNumCel.Text = "";
                TxtNumCel.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void focusTxtvalidar(object sender, RoutedEventArgs e)
        {
            txtcedula = false;
            txtvalidar = true;
            if (TxtVal.Text == "Confirmar")
            {
                TxtVal.Text = "";
                TxtVal.Foreground = new SolidColorBrush(Colors.Black);
            }
        }



        private void Btn_DeleteAllTouchDown(object sender, EventArgs e)
        {
            try
            {
                if (txtcedula)
                {
                    TxtNumCel.Text = string.Empty;
                }

                if (txtvalidar)
                {
                    TxtVal.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                // Manejo del error
                throw ex;
            }
        }

        private void Btn_DeleteTouchDown(object sender, EventArgs e)
        {
            try
            {
                if (txtcedula && TxtNumCel.Text.Length > 0)
                {
                    TxtNumCel.Text = TxtNumCel.Text.Remove(TxtNumCel.Text.Length - 1);
                }

                if (txtvalidar && TxtVal.Text.Length > 0)
                {
                    TxtVal.Text = TxtVal.Text.Remove(TxtVal.Text.Length - 1);
                }
            }
            catch (Exception ex)
            {
                // Manejo del error
                throw ex;
            }
        }


        private void Keyboard_TouchDown(object sender, EventArgs e)
        {
            try
            {
                Image image = (Image)sender;
                string tag = image.Tag.ToString();

                if (txtcedula)
                {
                    TxtNumCel.Text += tag;
                }

                if (txtvalidar)
                {
                    TxtVal.Text += tag;
                }
            }
            catch (Exception ex)
            {
                // Manejo del error
                throw ex;
            }
        }

        private void BtnAtras_TouchDown(object sender, EventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void BtnInicio_TouchDown(object sender, EventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Main);
        }


        private void BtnContinuar_TouchDown(object sender, EventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            if (validateNum())
            {
                Transaction.NumOperator = TxtNumCel.Text;
                Utilities.navigator.Navigate(UserControlView.ResumenPaquete, Transaction);
            }
            else
            {
                Utilities.ShowModal("Por favor verifica la información ingresada", EModalType.Error);
                ActivateTimer();

            }
        }

        private void BtnCancelar_TouchDown(object sender, EventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);
        }


        public bool validateNum()
        {

            if (TxtNumCel.Text.Length == 10 && TxtVal.Text.Length == 10)
            {
                if (Convert.ToInt64(TxtNumCel.Text) == Convert.ToInt64(TxtVal.Text))
                {
                    return true;
                }
                else
                {

                    return false;
                }
            }
            else
            {

                return false;
            }

        }




        private void ActivateTimer()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    tbTimer.Text = "00:60";
                    timer = new TimerGeneric(tbTimer.Text);
                    timer.CallBackClose = response =>
                    {
                        SetCallBacksNull();
                        timer.CallBackStop?.Invoke(1);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                    };
                    timer.CallBackTimer = response =>
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            tbTimer.Text = response;
                        });
                    };
                    GC.Collect();
                });

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void SetCallBacksNull()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    timer.CallBackClose = null;
                    timer.CallBackTimer = null;

                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }
        private void Keyboard_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {

                if (txtcedula == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtNumCel.Text += Tag;
                }

                if (txtvalidar == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtVal.Text += Tag;

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void TxtNumCel_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
