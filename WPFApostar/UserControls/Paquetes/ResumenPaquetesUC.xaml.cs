using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WPFApostar.Classes;
using WPFApostar.Classes.UseFull;
using WPFApostar.Models;
using WPFApostar.Resources;
using System.IO;
using System.Windows.Input;

namespace WPFApostar.UserControls.Paquetes
{
    /// <summary>
    /// Lógica de interacción para ResumenPaquetesUC.xaml
    /// </summary>
    public partial class ResumenPaquetesUC : UserControl
    {
        Transaction Transaction;
        private TimerGeneric timer;

        public ResumenPaquetesUC(Transaction transaction)
        {
            InitializeComponent();
            ActivateTimer();
            Loaded += MainWindow_Loaded;
            Transaction = transaction;

            INFO.Text = Transaction.SelectOperator.nomPaquete;
            LblCelular.Text = Transaction.NumOperator;
            decimal valor = Convert.ToDecimal(transaction.SelectOperator.valorComercial);
            var convert = string.Format("{0:C0}", valor);
            Precio.Text = convert;
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



        private void BtnCancelar_TouchDown(object sender, EventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);

        }

        private void BtnContinuar_TouchDown(object sender, EventArgs e)
        {
            DisableView();

            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Transaction.Amount = Transaction.SelectOperator.valorComercial.ToString();
            Task.Run(() =>
            {

                var isValidateMoney = AdminPayPlus.ValidateMoney(Transaction).GetAwaiter().GetResult();

                Utilities.CloseModal();

                if (isValidateMoney != false)
                {
                    SendData();

                }
                else
                {
                    Utilities.ShowModal("En estos momentos la maquina no cuenta con suficiente cargue para esta operación", EModalType.Error);
                    Utilities.navigator.Navigate(UserControlView.Menu);

                }

            });

            Utilities.ShowModal("Estamos verificando la transacción un momento por favor", EModalType.Preload);
        }

        private void BtnCancelar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void BtnContinuar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DisableView();
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Transaction.Amount = Transaction.SelectOperator.valorComercial.ToString();
            Task.Run(() =>
            {
                var isValidateMoney = AdminPayPlus.ValidateMoney(Transaction).GetAwaiter().GetResult();
                Utilities.CloseModal();
                if (isValidateMoney != false)
                {
                    SendData();
                }
                else
                {
                    Utilities.ShowModal("En estos momentos la maquina no cuenta con suficiente cargue para esta operación", EModalType.Error);
                    Utilities.navigator.Navigate(UserControlView.Menu);
                }
            });
            Utilities.ShowModal("Estamos verificando la transacción un momento por favor", EModalType.Preload);
        }

        private void SendData()
        {
            try
            {



                AdminPayPlus.SaveLog("DataConsult", "entrando a la ejecucion SendData", "OK", "", Transaction);

                _= Task.Run(async () =>
                {

                    Transaction.State = ETransactionState.Initial;
                    Transaction.Type = ETypeTramites.PaquetesCel;
                    Transaction.Tipo = ETransactionType.Payment;
                    Transaction.eTypeTramites = ETypeTramites.PaquetesCel;
                    Transaction.valor = Transaction.Amount;
                    Transaction.reference = Transaction.NumOperator.ToString();
                    Transaction.Description =  Transaction.SelectOperator.nomPaquete.ToString();


                    await AdminPayPlus.SaveTransactionPaquetes(Transaction);

                    AdminPayPlus.SaveLog("DataConsult", "SendData", "OK", string.Concat("ID Transaccion:", Transaction.IdTransactionAPi, "/n", "Estado Transaccion:", "inicial", "/n", "Monto:", Transaction.Amount.ToString()), Transaction);

                    //     SetCallBacksNull();
                    //        timer.CallBackStop?.Invoke(1);

                    Utilities.CloseModal();

                    if (this.Transaction.IdTransactionAPi == 0)
                    {
                        Utilities.ShowModal("No se pudo guardar la transacción, intentelo nuevamente.", EModalType.Error);

                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }
                    else
                    {

                        Utilities.navigator.Navigate(UserControlView.PaymentPaquetes, Transaction);
                    }
                });

                Utilities.ShowModal(MessageResource.LoadInformation, EModalType.Preload);


            }
            catch (Exception ex)
            {
                Utilities.CloseModal();
                Utilities.ShowModal("No se pudo guardar la transacción, intentelo nuevamente.", EModalType.Error);

                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }



        private async Task DisableView()
        {
            await Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 0.3;
                this.IsEnabled = false;
            });
        }


        private async Task EnableView()
        {
            await Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 1;
                this.IsEnabled = true;
            });
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
    }
}
