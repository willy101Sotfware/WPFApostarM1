using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using WPFApostar.Domain;
using WPFApostar.Domain.UseFull;
using WPFApostar.Domain.ApiService.Models;

namespace WPFApostar.UserControls.Betplay
{
    /// <summary>
    /// Lógica de interacción para PreviewFacture.xaml
    /// </summary>
    public partial class PreviewFacture : UserControl
    {

        private Transaction Transaction;
        private TimerGeneric timer;

        public PreviewFacture(Transaction Ts)
        {
            InitializeComponent();
            Transaction = Ts;
            ActivateTimer();
            GC.Collect();
            loadData();



        }

        public void loadData()
        {
            //Pruebas Sin Notificar

            TxtCedula.Text = String.Concat("Cedula: " + Transaction.Document);
            TxtValor.Text = String.Concat("Valor Recarga: " + "$" + Transaction.Amount);
            TxtFecha.Text = String.Concat("Fecha: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            TxtDatos.Text = String.Concat("OF: " + Transaction.codigoPuntoVenta, " ", "AS: " + Utilities.GetConfiguration("Terminal"), "  ", "EQ: " + Utilities.GetConfiguration("Id"));


        }

        private void ActivateTimer()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    tbTimer.Text = "00:15";
                    timer = new TimerGeneric(tbTimer.Text);
                    timer.CallBackClose = response =>
                    {
                        SetCallBacksNull();
                        timer.CallBackStop?.Invoke(1);
                        Utilities.navigator.Navigate(UserControlView.Finish, Transaction);
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

        private void Btn_ContinuarTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Finish, Transaction);
        }
    }
}
