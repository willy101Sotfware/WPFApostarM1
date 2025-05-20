using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFApostar.Classes;
using WPFApostar.Classes.UseFull;
using WPFApostar.Models;
using WPFApostar.Resources;
using Newtonsoft.Json;
using WPFApostar.Services.ObjectIntegration;

namespace WPFApostar.UserControls.Betplay
{
    /// <summary>
    /// Lógica de interacción para RechargeUC.xaml
    /// </summary>
    public partial class RechargeUC : UserControl
    {
        private List<SubProductoGeneral> subProductosKiosko;
        private Transaction Transaction;
        private TimerGeneric timer;
        public ValueModel value;

        public RechargeUC(Transaction Ts)
        {
            InitializeComponent();
            Transaction = Ts;
            this.DataContext = Transaction;
            ActivateTimer();

            value = new ValueModel
            {
                Val = 0
            };

            this.DataContext = value;

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

        private void Keyboard_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                Image image = (Image)sender;
                string Tag = image.Tag.ToString();
                TxtMonto.Text += Tag;
            }
            catch (Exception ex)
            {

            }
        }

        private void Btn_DeleteTouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                string val = TxtMonto.Text;

                if (val.Length > 0)
                {
                    TxtMonto.Text = val.Remove(val.Length - 1);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Btn_DeleteAllTouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                TxtMonto.Text = string.Empty;

            }
            catch (Exception ex)
            {

            }
        }

        private bool Validate()
        {


            if (!string.IsNullOrEmpty(TxtMonto.Text))
            {
                string value = TxtMonto.Text.Replace("$", "");
                value = value.Replace(",", "");

                if (Convert.ToDecimal(value) <= 500000)
                {
                    if (TxtMonto.Text != string.Empty && value != "0" && Convert.ToDecimal(value) % 100 == 0)
                    {
                        Transaction.Amount = value;
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
            else
            {
                return false;
            }


        }

        private async void SendData()
        {
            try
            {

                AdminPayPlus.SaveLog("RechargeUC", "entrando a la ejecucion SendData", "OK", "", Transaction);

                Task.Run(async () =>
                {
                    Transaction.payer = new DataModel.PAYER
                    {
                        IDENTIFICATION = "Betplay",
                        NAME = Transaction.Name,

                        STATE = Transaction.statePaySuccess,

                    };

                    Transaction.State = ETransactionState.Initial;
                    Transaction.Type = ETypeTramites.BetPlay;
                    Transaction.Tipo = ETransactionType.Payment;
                    Transaction.eTypeTramites = ETypeTramites.BetPlay;
                    Transaction.valor = Transaction.Amount;

                    await AdminPayPlus.SaveTransactionBetPlay(Transaction);

                    AdminPayPlus.SaveLog("RechargeUC", "SendData", "OK", string.Concat("ID Transaccion:", Transaction.IdTransactionAPi, "/n", "Estado Transaccion:", "inicial", "/n", "Monto:", Transaction.Amount.ToString()), Transaction);


                    Utilities.CloseModal();

                    if (this.Transaction.IdTransactionAPi == 0)
                    {
                        AdminPayPlus.SaveLog("RechargeUC", "No se puede guardar la transacción, intentelo más tarde.", "OK", "", Transaction);


                        Utilities.ShowModal("No se puede guardar la transacción, intentelo más tarde.", EModalType.Error);

                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }
                    else
                    {

                        AdminPayPlus.SaveLog("RechargeUC", "entrando a la ejecucion ConsultInforBetplay()", "OK", "", Transaction);

                        ConsultInforBetplay();
                    }
                });

                Utilities.ShowModal(MessageResource.LoadInformation, EModalType.Preload);


            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("RechargeUC", "entrando a la ejecucion SendData Catch", "OK", ex.Message.ToString(), Transaction);


                Utilities.CloseModal();

                Utilities.ShowModal("No se puede guardar la transacción, intentelo más tarde.", EModalType.Error);

                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }


        private async void ConsultInforBetplay()
        {

            try
            {


                Task.Run(async () =>
                {

                    try
                    {

                        RequesttokenBetplay requesttokenBetplay = new RequesttokenBetplay();

                        requesttokenBetplay.Ubicacion =  Utilities.GetConfiguration("UbicacionMaquina");
                        requesttokenBetplay.Transaccionclienteid = Transaction.IdTransactionAPi.ToString();
                        requesttokenBetplay.Transacciondistribuidorid = Transaction.IdTransactionAPi.ToString();


                        AdminPayPlus.SaveLog("RechargeUC", "entrando a la ejecucion ConsultInforBetplay() entrando a GetTokenBetplay", "OK", "", Transaction);


                        var RespuestaToken = await AdminPayPlus.ApiIntegration.GetTokenBetplay(requesttokenBetplay);


                        if (RespuestaToken != null)
                        {



                            RequestConsultSubproductBetplay request = new RequestConsultSubproductBetplay();
                            request.Ubicacion =  Utilities.GetConfiguration("UbicacionMaquina");
                            request.Transacciondistribuidorid = Transaction.IdTransactionAPi.ToString();
                            request.Token = RespuestaToken.Token;


                            var Respuesta = await AdminPayPlus.ApiIntegration.GetProductsBetPlay(request);

                            var sere = JsonConvert.SerializeObject(Respuesta);

                            AdminPayPlus.SaveLog("RechargeUC", " Respuesta al servicio GetConsultBetplay", "OK", sere.ToString(), null);

                            Utilities.CloseModal();

                            if (Respuesta != null)
                            {
                                if (Respuesta.Estado == true)
                                {
                                    Transaction.Type = ETypeTramites.BetPlay;
                                    subProductosKiosko = Respuesta.Listadosubproductos.Subproducto;

                                    Transaction.ProductSelected = subProductosKiosko.Where(x => x.Nombre == "RECAUDO BETPLAY").FirstOrDefault();
                                    Utilities.navigator.Navigate(UserControlView.Validate, Transaction);

                                }
                                else
                                {
                                    Utilities.ShowModal("Este servicio no se encuentra disponible en este momento, intenta nuevamente.", EModalType.Error, true);
                                    Utilities.navigator.Navigate(UserControlView.Menu);

                                }
                            }
                            else
                            {
                                Utilities.ShowModal("Este servicio no se encuentra disponible en este momento, intenta nuevamente.", EModalType.Error, true);
                                Utilities.navigator.Navigate(UserControlView.Menu);

                            }
                        }
                        else
                        {
                            Utilities.CloseModal();
                            Utilities.ShowModal("Este servicio no se encuentra disponible en este momento, intenta nuevamente.", EModalType.Error, true);
                            Utilities.navigator.Navigate(UserControlView.Menu);
                        }


                    }
                    catch (Exception ex)

                    {

                        AdminPayPlus.SaveLog("RechargeUC", "entrando a la ejecucion ConsultInforBetplay Catch", "Fail", ex.Message, null);
                        Utilities.CloseModal();

                        Utilities.ShowModal("Ocurrió un error generando la transacción, inténtelo nuevamente", EModalType.Error, true);

                        Utilities.navigator.Navigate(UserControlView.Menu);

                    }

                });

                Utilities.ShowModal("Estamos Validando los Servicios un momento por favor", EModalType.Preload, false);






            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("RechargeUC", "entrando a la ejecucion ConsultInforBetplay Catch", "Fail", ex.Message, null);

                Utilities.CloseModal();

                Utilities.ShowModal("Ocurrió un error generando la transacción, inténtelo nuevamente", EModalType.Error, true);

                Utilities.navigator.Navigate(UserControlView.Menu);
            }




        }



        private void Btn_ContinuarTouchDown(object sender, EventArgs e)
        {
            
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);

            if (Validate())
            {
                DisableView();
                SendData();
            }
            else
            {
                Utilities.ShowModal("Por favor Ingresa un valor valido", EModalType.Error);
                ActivateTimer();
            }
        }

        private void Btn_CancelarTouchDown(object sender, EventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void focusTxtCedula(object sender, RoutedEventArgs e)
        {

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

        private void txtVal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TxtMonto.Text.Length > 15)
            {
                TxtMonto.Text = TxtMonto.Text.Remove(15, 1);
                return;
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
