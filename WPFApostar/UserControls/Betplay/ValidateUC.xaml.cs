using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using WPFApostar.Classes;
using WPFApostar.Classes.UseFull;
using WPFApostar.Models;
using WPFApostar.Services.ObjectIntegration;

namespace WPFApostar.UserControls.Betplay
{
    /// <summary>
    /// Lógica de interacción para ValidateUC.xaml
    /// </summary>
    public partial class ValidateUC : UserControl
    {


        private Transaction Transaction;
        private TimerGeneric timer;

        public ValidateUC(Transaction Ts)
        {
            InitializeComponent();
            Transaction = Ts;
            this.DataContext = Transaction;

            AdminPayPlus.SaveLog("ValidateUC", "entrando a la ejecucion", "OK", "", Transaction);

            ActivateTimer();

            TxtCedula.Text = string.Concat(Transaction.Document.ToString());
            TxtMonto.Text = string.Concat(String.Format("{0:C0}", Convert.ToDecimal(Transaction.Amount)));

            AdminPayPlus.SaveLog("ValidateUC", "Saliendo de la ejecucion", "OK", "", Transaction);


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


        private void Btn_CancelarTouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void Btn_ContinuarTouchDown(object sender, EventArgs e)
        {
            DisableView();

            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);


            try
            {
                Task.Run(() =>
                {

                            var isValidateMoney = AdminPayPlus.ValidateMoney(Transaction).GetAwaiter().GetResult();

                            //   Utilities.CloseModal();

                            if (isValidateMoney != false)
                            {
                                Utilities.CloseModal();
                                ValidateToken();

                            }
                            else
                            {
                                Utilities.CloseModal();
                                Utilities.ShowModal("En estos momentos la maquina no cuenta con suficiente cargue para esta operación", EModalType.Error);
                                Utilities.navigator.Navigate(UserControlView.Menu);

                            }


                });

                Utilities.ShowModal("Estamos verificando la transacción un momento por favor", EModalType.Preload);

            }
            catch
            {
                Utilities.CloseModal();
                Utilities.ShowModal("En estos momentos los servicios de Betplay no estan disponibles", EModalType.Error);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }

        private async void ValidateToken()
        {

            try
            {

                string modifiedId = "1" + Transaction.IdTransactionAPi.ToString();
                Transaction.Transacciondistribuidorid = ulong.Parse(modifiedId);


                RequesttokenBetplay requesttokenBetplay = new RequesttokenBetplay();

                requesttokenBetplay.Ubicacion =  Utilities.GetConfiguration("UbicacionMaquina");
                requesttokenBetplay.Transaccionclienteid =  modifiedId;
                requesttokenBetplay.Transacciondistribuidorid = modifiedId;

                var RespuestaToken = await AdminPayPlus.ApiIntegration.GetTokenBetplay(requesttokenBetplay);

                if (RespuestaToken != null)
                {

                    Transaction.ResponseTokenBetplay = RespuestaToken;

                    Utilities.navigator.Navigate(UserControlView.Payment, Transaction);

                }
                else
                {
                    Utilities.ShowModal("Este servicio no se encuentra disponible en este momento, intenta nuevamente.", EModalType.Error, true);
                    Utilities.navigator.Navigate(UserControlView.Menu);
                }

            }
            catch (Exception ex)
            {
                Utilities.CloseModal();
                Utilities.ShowModal("En estos momentos los servicios de Betplay no estan disponibles, intenta nuevamente", EModalType.Error, true);
                Utilities.navigator.Navigate(UserControlView.Menu);


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
    }
}
