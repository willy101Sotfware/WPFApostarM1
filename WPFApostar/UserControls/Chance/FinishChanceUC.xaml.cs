using Newtonsoft.Json;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFApostar.Domain;
using WPFApostar.Domain.UseFull;
using WPFApostar.Domain.ApiService.Models;
using WPFApostar.Domain.UIServices.ObjectIntegration;

namespace WPFApostar.UserControls.Chance
{
    /// <summary>
    /// Lógica de interacción para FinishChanceUC.xaml
    /// </summary>
    public partial class FinishChanceUC : UserControl
    {

        private Transaction Transaction;
        private TimerGeneric timer;

        public FinishChanceUC(Transaction Ts)
        {
            AdminPayPlus.SaveLog("FinishChanceUC", "Entrando a FinishChanceUC ", "OK", "", null);


            InitializeComponent();
            Transaction = Ts;
            FinishTransaction();

        //   SendAlert();
           ActivateTimer();
            this.DataContext = Transaction;
            
        }

        public void SendAlert()
        {
            try
            {
                AdminPayPlus.SaveLog("FinishChanceUC", "Entrando a SendAlert ", "OK", "", null);

                //     Transaction.TransaccionChance = Transaction.ResponseNotifyC.Transaccionid;

                RequestSendAlert sendAlert = new RequestSendAlert();


                sendAlert.transaccion = Transaction.TransactionId;
                sendAlert.TransaccionChance = Transaction.ResponseNotifyC.Transaccionid.ToString();

                sendAlert.codigoApostar = Utilities.GetConfigData("CodData");

                sendAlert.idPagador = Transaction.IdUser.ToString();

                sendAlert.cedula = Transaction.payer.IDENTIFICATION;

                AdminPayPlus.SaveLog("Request SendAlert: " + JsonConvert.SerializeObject(sendAlert));

                Task.Run(() =>
                {
                    var Response = AdminPayPlus.ApiIntegration.SendAlert(sendAlert);

                    AdminPayPlus.SaveLog($"Response Send Alert:  {JsonConvert.SerializeObject(Response)}");

                });

               
            }
            catch(Exception ex)
            {

            }
        }

        private void Btn_Calificacion(object sender, TouchEventArgs e)
        {
            try
            {
                Transaction.calificacion = "";

                int Tag = Convert.ToInt32((sender as Image).Tag);

                switch (Tag)
                {
                    case 1:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Hidden;
                        StarS3.Visibility = Visibility.Hidden;
                        StarS4.Visibility = Visibility.Hidden;
                        StarS5.Visibility = Visibility.Hidden;
                        aceptar.Visibility = Visibility.Visible;
                        Transaction.calificacion = "1";

                        break;

                    case 2:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Hidden;
                        StarS4.Visibility = Visibility.Hidden;
                        StarS5.Visibility = Visibility.Hidden;
                        aceptar.Visibility = Visibility.Visible;
                        Transaction.calificacion = "2";
                        break;
                    case 3:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Visible;
                        StarS4.Visibility = Visibility.Hidden;
                        StarS5.Visibility = Visibility.Hidden;
                        aceptar.Visibility = Visibility.Visible;
                        Transaction.calificacion = "3";
                        break;
                    case 4:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Visible;
                        StarS4.Visibility = Visibility.Visible;
                        StarS5.Visibility = Visibility.Hidden;
                        aceptar.Visibility = Visibility.Visible;
                        Transaction.calificacion = "4";

                        break;

                    case 5:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Visible;
                        StarS4.Visibility = Visibility.Visible;
                        StarS5.Visibility = Visibility.Visible;
                        aceptar.Visibility = Visibility.Visible;
                        Transaction.calificacion = "5";
                        break;
                }
            }
            catch (Exception Ex)
            {

            }
        }

        public void FinishTransaction()
        {
            try
            {

                AdminPayPlus.SaveLog("FinishChance", "entrando a la ejecucion FinishTransaction", "OK", "", Transaction);



                Transaction.State = ETransactionState.Success;

             
                Transaction.StatePay = "Aprobado";
                AdminPayPlus.UpdateTransactionChance(Transaction);

                AdminPayPlus.SaveLog("SuccesUserControl", "FinishTransaction", "OK", string.Concat("ID Transaccion:", Transaction.IdTransactionAPi, "/n", "Estado Transaccion:", "Aprobada", "/n", "Monto:", Transaction.Amount.ToString(), "/n", "Valor Dispensado:", Transaction.Payment.ValorDispensado.ToString(), "/n", "Valor Ingresado:", Transaction.Payment.ValorIngresado.ToString()), Transaction);
             
                Utilities.PrintVoucherSuperChance(Transaction);
                

                AdminPayPlus.SaveLog("SussesUserControl", "Saliendo de la ejecucion FinishTransaction", "OK", "", Transaction);

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("SuccesUserControl", "Error Catch la ejecucion FinishTransaction", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }

        }


        private async Task<bool> CalificacionCliente()
        {

            try
            {
                AdminPayPlus.SaveLog("FinishPaquetesUC", "entrando a la ejecucion CalificacionCliente", "OK", "", Transaction);


                var respuesta = await AdminPayPlus.CalificacionCliente(Transaction);

                AdminPayPlus.SaveLog("FinishPaquetesUC", "entrando a la ejecucion CalificacionCliente respuesta", "OK ", respuesta.ToString(), Transaction);

                if (respuesta == true)
                {


                    return true;
                }
                else
                {
                    return false;
                }


            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("FinishPaquetesUC", "entrando a la ejecucion CalificacionCliente catch", "OK ", ex.Message, Transaction);
            }
            return false;
        }


        private void Btn_salirTouchDown(object sender, TouchEventArgs e)
        {
            aceptar.IsEnabled = false;

            if (Transaction.calificacion != null)
            {
                var response = CalificacionCliente();

            }

            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Transaction = new Transaction();
            Initial();
        }


        private void Initial()
        {
            try
            {

                Utilities.navigator.Navigate(UserControlView.Config);


            }
            catch (Exception ex)
            {
                Utilities.navigator.Navigate(UserControlView.Config);
            }
        }

        private void ActivateTimer()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    tbTimer.Text = "00:30";
                    timer = new TimerGeneric(tbTimer.Text);
                    timer.CallBackClose = response =>
                    {
                        SetCallBacksNull();
                        timer.CallBackStop?.Invoke(1);

                        if (Transaction.calificacion != null)
                        {
                            var responses = CalificacionCliente();

                        }
                        Transaction = new Transaction();
                        Initial();
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
