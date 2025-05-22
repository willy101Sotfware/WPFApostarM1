using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFApostar.Domain;
using WPFApostar.Domain.UseFull;
using WPFApostar.Domain.ApiService.Models;
using WPFApostar.Resources;
using WPFApostar.Services.Object;

namespace WPFApostar.UserControls.Paquetes
{
    /// <summary>
    /// Lógica de interacción para FinishPaquetesUC.xaml
    /// </summary>
    public partial class FinishPaquetesUC : UserControl
    {

        private Transaction transaction;
        private TimerGeneric timer;
        public FinishPaquetesUC(Transaction Ts)
        {
            AdminPayPlus.SaveLog("FinishPaquetesUC", "Entrando a FinishPaquetesUC ", "OK", "", transaction);

            InitializeComponent();
            ActivateTimer();
            transaction = Ts;
            FinishTransaction();

        }


        private void Btn_Calificacion(object sender, TouchEventArgs e)
        {
            try
            {
                transaction.calificacion = "";

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
                        transaction.calificacion = "1";

                        break;

                    case 2:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Hidden;
                        StarS4.Visibility = Visibility.Hidden;
                        StarS5.Visibility = Visibility.Hidden;
                        aceptar.Visibility = Visibility.Visible;
                        transaction.calificacion = "2";
                        break;
                    case 3:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Visible;
                        StarS4.Visibility = Visibility.Hidden;
                        StarS5.Visibility = Visibility.Hidden;
                        aceptar.Visibility = Visibility.Visible;
                        transaction.calificacion = "3";
                        break;
                    case 4:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Visible;
                        StarS4.Visibility = Visibility.Visible;
                        StarS5.Visibility = Visibility.Hidden;
                        aceptar.Visibility = Visibility.Visible;
                        transaction.calificacion = "4";

                        break;

                    case 5:
                        StarS1.Visibility = Visibility.Visible;
                        StarS2.Visibility = Visibility.Visible;
                        StarS3.Visibility = Visibility.Visible;
                        StarS4.Visibility = Visibility.Visible;
                        StarS5.Visibility = Visibility.Visible;
                        aceptar.Visibility = Visibility.Visible;
                        transaction.calificacion = "5";
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

                AdminPayPlus.SaveLog("FinishPaquetesUC", "entrando a la ejecucion FinishTransaction", "OK", "", transaction);
         

                transaction.State = ETransactionState.Success;

             
                transaction.StatePay = "Aprobado";

                AdminPayPlus.UpdateTransactionPaquetes(transaction);

                AdminPayPlus.SaveLog("SuccesUserControl", "FinishTransaction", "OK", string.Concat("ID Transaccion:", transaction.IdTransactionAPi, "/n", "Estado Transaccion:", "Aprobada", "/n", "Monto:", transaction.Amount.ToString(), "/n", "Valor Dispensado:", transaction.Payment.ValorDispensado.ToString(), "/n", "Valor Ingresado:", transaction.Payment.ValorIngresado.ToString()), transaction);


                Utilities.PrintVoucherPaquetes(transaction);

            

                AdminPayPlus.SaveLog("SussesUserControl", "Saliendo de la ejecucion FinishTransaction", "OK", "", transaction);

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("SuccesUserControl", "Error Catch la ejecucion FinishTransaction", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }

        }

        private async Task<bool> CalificacionCliente()
        {

            try
            {
                AdminPayPlus.SaveLog("FinishPaquetesUC", "entrando a la ejecucion CalificacionCliente", "OK", "", transaction);


                var respuesta = await AdminPayPlus.CalificacionCliente(transaction);

                AdminPayPlus.SaveLog("FinishPaquetesUC", "entrando a la ejecucion CalificacionCliente respuesta", "OK ", respuesta.ToString(), transaction);

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
                AdminPayPlus.SaveLog("FinishPaquetesUC", "entrando a la ejecucion CalificacionCliente catch", "OK ", ex.Message, transaction);
            }
            return false;
        }



        private void Btn_salirTouchDown(object sender, EventArgs e)
        {
            aceptar.IsEnabled = false;

            if (transaction.calificacion != null)
            {
                var response = CalificacionCliente();

            }

            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            transaction = new Transaction();
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

                        if (transaction.calificacion != null)
                        {
                            var responses = CalificacionCliente();

                        }
                        transaction = new Transaction();
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

