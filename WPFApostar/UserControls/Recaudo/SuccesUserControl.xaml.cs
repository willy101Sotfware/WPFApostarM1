using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFApostar.Classes;
using WPFApostar.Classes.UseFull;
using WPFApostar.Models;

namespace WPFApostar.UserControls.Recaudo
{
    /// <summary>
    /// Lógica de interacción para SuccesUserControl.xaml
    /// </summary>
    public partial class SuccesUserControl : UserControl
    {


        private Transaction Transaction;
        private TimerGeneric timer;

        public SuccesUserControl(Transaction Ts)
        {
            InitializeComponent();
            Transaction = Ts;
            this.DataContext = Transaction;
            AdminPayPlus.SaveLog("SuccesUserControl", "entrando a la ejecucion", "OK", "", Transaction);

            FinishTransaction();

            AdminPayPlus.SaveLog("SuccesUserControl", "Saliendo de la ejecucion", "OK", "", Transaction);

            ActivateTimer();
            GC.Collect();
        }

        public void FinishTransaction()
        {
            try
            {

                AdminPayPlus.SaveLog("SuccesUserControl", "entrando a la ejecucion FinishTransaction", "OK", "", Transaction);

               
                Transaction.State = ETransactionState.Success;

                Transaction.StatePay = "Aprobado";

                AdminPayPlus.UpdateTransaction(Transaction);

                AdminPayPlus.SaveLog("SuccesUserControl", "FinishTransaction", "OK", string.Concat("ID Transaccion:", Transaction.IdTransactionAPi, "/n", "Estado Transaccion:", "Aprobada", "/n", "Monto:", Transaction.Amount.ToString(), "/n", "Valor Dispensado:", Transaction.Payment.ValorDispensado.ToString(), "/n", "Valor Ingresado:", Transaction.Payment.ValorIngresado.ToString()), Transaction);


                Utilities.PrintVoucherRecaudo(this.Transaction);

               

                AdminPayPlus.SaveLog("SuccesUserControl", "Saliendo de la ejecucion FinishTransaction", "OK", "", Transaction);

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("SuccesUserControl", "Error Catch la ejecucion FinishTransaction", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
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


        private async Task<bool> CalificacionCliente()
        {

            try
            {
                AdminPayPlus.SaveLog("SuccesUserControl", "entrando a la ejecucion CalificacionCliente", "OK", "", Transaction);


                var respuesta = await AdminPayPlus.CalificacionCliente(Transaction);

                AdminPayPlus.SaveLog("SuccesUserControl", "entrando a la ejecucion CalificacionCliente respuesta", "OK ", respuesta.ToString(), Transaction);

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
                AdminPayPlus.SaveLog("SuccesUserControl", "entrando a la ejecucion CalificacionCliente catch", "OK ", ex.Message, Transaction);
            }
            return false;
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

    }
}
