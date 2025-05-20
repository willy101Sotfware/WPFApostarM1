using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFApostar.Classes;
using WPFApostar.Domain.Peripherals;
using WPFApostar.Models;
using WPFApostar.Resources;
using WPFApostar.Services.ObjectIntegration;
using WPFApostar.ViewModel;

namespace WPFApostar.UserControls.Paquetes
{
    /// <summary>
    /// Lógica de interacción para PaymentPaquetesUC.xaml
    /// </summary>
    public partial class PaymentPaquetesUC : UserControl
    {
        private Transaction Transaction;
        private ETypeTramites typeTramites;
        public PaymentViewModel paymentViewModel;
        private PeripheralController _peripherals;
        private int Intentos = 1;
        private Task notify = null;
        private bool NotifyStatus = false;

        public PaymentPaquetesUC(Transaction Ts)
        {

            AdminPayPlus.SaveLog("PaymentPaquetesUC", "entrando a la ejecucion", "OK", "", Transaction);
            InitializeComponent();
            Transaction = Ts;

            Transaction.DevueltaCorrecta = false;

            _peripherals = PeripheralController.Instance;
            _peripherals.CashIn += OnCashIn;
         
            this.Unloaded += OnUnloaded;


            this.Loaded += OnLoaded;

        

        }

        #region NewMethodsPay

        private void OnLoaded(object sender, RoutedEventArgs e)
        {

            OrganizeValues();
            _peripherals.StartAcceptance(paymentViewModel.PayValue);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _peripherals.CashIn -= OnCashIn;
       

        }

        private async void OnCashIn(decimal value)
        {

          

            paymentViewModel.ValorIngresado += value;


            paymentViewModel.RefreshListDenomination(Convert.ToInt32(value), 1);
            LoadView();

            //SendTransactionDetail(TypeOperation.AP, value);
            AdminPayPlus.SaveDetailsTransaction(Transaction.IdTransactionAPi, value, 2, 1, "AP", string.Empty);

            

            if (paymentViewModel.ValorIngresado >= paymentViewModel.PayValue)
            {
                AdminPayPlus.SaveLog("PaymentPaquetesUC", "Entrando a la ejecucion OnCashIn paymentViewModel.ValorIngresado >= paymentViewModel.PayValue", "OK", "", Transaction);

                _ = Dispatcher.BeginInvoke((Action)delegate { btnCancell.Visibility = Visibility.Collapsed; });

                await _peripherals.StopAceptance();

                AdminPayPlus.SaveLog("PaymentPaquetesUC", "Entrando a la ejecucion OnCashIn  StopAceptance entrando a NotifyPaquetes ", "OK", "", Transaction);
                NotifyPaquetes();
            }

        }



        #endregion

        #region NewMethodsDispenser

     
        private string GetDetailsString(Dictionary<int, int> details, int[] denominations, string prefix)
        {
            var filteredDetails = details.Where(d => denominations.Contains(d.Key))
                                         .Select(d => $"{d.Key}-{d.Value}")
                                         .ToList();

            if (filteredDetails.Any())
            {
                return $"{prefix}:" + string.Join(";", filteredDetails) + "!";
            }

            return string.Empty;
        }
        private void SendDispenseDetails(Dictionary<int, int> details, bool isReject = false)
        {
            var dpString = GetDetailsString(details, new[] { 2000, 10000, 50000 }, isReject ? "RJ" : "DP");
            var mdString = GetDetailsString(details, new[] { 200, 500 }, "MD");

            // Enviar primero la información de DP
            if (!string.IsNullOrEmpty(dpString))
            {
                AdminPayPlus.SaveDetailsTransaction(Transaction.IdTransactionAPi, 0, 0, 0, string.Empty, dpString);
            }

            // Luego enviar la información de MD
            if (!string.IsNullOrEmpty(mdString))
            {
                AdminPayPlus.SaveDetailsTransaction(Transaction.IdTransactionAPi, 0, 0, 0, string.Empty, mdString);
            }
        }

 

        #endregion



        private void OrganizeValues()
        {
            try
            {

                string mensaje = string.Concat("", " ", "entrando a la ejecucion organizevalues", " ", "OK");
                AdminPayPlus.SaveLog(mensaje);
                //InitTimer();

                this.paymentViewModel = new PaymentViewModel
                {
                    PayValue = Convert.ToDecimal(Transaction.Amount),
                    ValorFaltante = Convert.ToDecimal(Transaction.Amount),
                    ImgContinue = Visibility.Hidden,
                    ImgCancel = Visibility.Visible,
                    ImgCambio = Visibility.Hidden,
                    ValorSobrante = 0,
                    ValorIngresado = 0,
                    viewList = new CollectionViewSource(),
                    Denominations = new List<DenominationMoney>(),
                    ValorDispensado = 0
                };

                this.DataContext = this.paymentViewModel;
                string moreMs = string.Empty;



                mensaje = string.Concat("", " ", "Saliendo de la ejecucion organizevalues", " ", "OK");
                AdminPayPlus.SaveLog(mensaje);

            



            }
            catch (Exception ex)
            {

                string mensaje = string.Concat("catch organizevalues", " ", "Error", " ", ex.Message, " ", ex.StackTrace);
                AdminPayPlus.SaveLog(mensaje);
              
            }
        }



        private void LoadView()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    paymentViewModel.viewList.Source = paymentViewModel.Denominations;
                    lv_denominations.DataContext = paymentViewModel.viewList;
                    lv_denominations.Items.Refresh();
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                string mensaje = string.Concat("catch LoadView", " ", "Error", " ", ex.Message, " ", ex.StackTrace);
                AdminPayPlus.SaveLog(mensaje);

            }
        }




        private void BtnCancell_TouchDown(object sender, EventArgs e)
        {
            AdminPayPlus.SaveLog("PaymentPaquetesUC", "Entrando a la ejecucion BtnCancell_TouchDown", "OK", "", Transaction);

            CancellPay();
        }

        private void CancellPay()
        {
            try
            {
                AdminPayPlus.SaveLog("PaymentPaquetesUC", "entrando a la ejecucion CancellPay", "OK", "", Transaction);

                this.paymentViewModel.ImgContinue = Visibility.Hidden;

                this.paymentViewModel.ImgCancel = Visibility.Hidden;

                if (Utilities.ShowModal(MessageResource.CancelTransaction, EModalType.Information))
                {
                    //     AdminPayPlus.ControlPeripherals.StopAceptance();
                    //       AdminPayPlus.ControlPeripherals.callbackLog = null;

                    if (!this.paymentViewModel.StatePay)
                    {
                        if (paymentViewModel.ValorIngresado > 0)
                        {
                            AdminPayPlus.SaveLog("PaymentPaquetesUC", "entrando a la ejecucion CancellPay paymentViewModel.ValorIngresado > 0 navegando a ReturnMoneyPaquetes", "OK", "", Transaction);
                            Transaction.StatePay = "Cancelada";
                            Transaction.Payment = paymentViewModel;
                            Utilities.navigator.Navigate(UserControlView.ReturnMoneyPaquetes, Transaction);
                        }
                        else
                        {
                            AdminPayPlus.SaveLog("PaymentPaquetesUC", "entrando a la ejecucion CancellPay Navegando al Config", "OK", "", Transaction);
                            Utilities.navigator.Navigate(UserControlView.Config);
                        }
                    }
                }
                else
                {
                    if (paymentViewModel.ValorIngresado > 0)
                    {
                        this.paymentViewModel.ImgContinue = Visibility.Visible;
                    }

                    this.paymentViewModel.ImgCancel = Visibility.Visible;
                }

                AdminPayPlus.SaveLog("PaymentPaquetesUC", "saliendo de la ejecucion CancellPay", "OK", "", Transaction);
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentPaquetesUC", "Error Catch la ejecucion CancellPay", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
               
            }
        }

        public async Task SavePay(ETransactionState statePay = ETransactionState.Initial)
        {

            AdminPayPlus.SaveLog("PaymentPaquetesUC", "entrando a la ejecucion SavePay", "OK", "", Transaction);


            if (!this.paymentViewModel.StatePay)
            {
                this.paymentViewModel.StatePay = true;
                Transaction.Payment = paymentViewModel;
                Transaction.State = statePay;

             //   AdminPayPlus.ControlPeripherals.ClearValues();

                AdminPayPlus.SaveLog("PaymentPaquetesUC", "Navegando a FinishPaquetes", "OK", "", Transaction);

                Utilities.navigator.Navigate(UserControlView.FinishPaquetes, Transaction);

              //  GC.Collect();
            }

        }

        public void NotifyPaquetes()
        {

            try
            {
                InhabilitarVista();

                Task.Run(async () =>
                {

                    AdminPayPlus.SaveLog("PaymentPaquetesUC", "entrando a la ejecucion NotifyPaquetes", "OK", "", null);


                    RequestGuardarPaquete Request = new RequestGuardarPaquete
                    {
                        Ubicacion =  Utilities.GetConfiguration("UbicacionMaquina"),
                       Codigosubproducto = Transaction.IdProduct,
                        Valor =  Convert.ToInt32(Transaction.Amount),
                        Numero = Transaction.NumOperator,
                        Id =  Transaction.SelectOperator.idOperador,
                        Transacciondistribuidorid =  Transaction.IdTransactionAPi
                    };

                    
                    var Respuesta = await AdminPayPlus.ApiIntegration.GuardarPaquetes(Request);


                    if (Respuesta != null)
                    {
                        if (Respuesta.Estado != false)

                        {
                            Transaction.responseGuardarPaquetes = Respuesta;
                            Transaction.State = ETransactionState.Success;

                            AdminPayPlus.SaveLog("PaymentPaquetesUC", "saliendo de la ejecucion NotifyPaquetes estado", "OK", Respuesta.Estado.ToString(), Transaction);

                            if (paymentViewModel.ValorSobrante != 0)
                            {
                                Utilities.CloseModal();
                                AdminPayPlus.SaveLog("PaymentPaquetesUC", "Entrando a returnmoney valor sobrante es igual a ", "OK", paymentViewModel.ValorSobrante.ToString(), Transaction);
                                Transaction.StatePay = "Aprobado";
                                Transaction.Payment = paymentViewModel;
                                Utilities.navigator.Navigate(UserControlView.ReturnMoneyPaquetes, Transaction);
                            }
                            else
                            {
                                Utilities.CloseModal();
                                Transaction.Payment = paymentViewModel;
                                Transaction.StatePay = "Aprobado";
                                AdminPayPlus.SaveLog("PaymentPaquetesUC", "Navegando al metodo Savepay valor sobrante es igual a ", "OK", paymentViewModel.ValorSobrante.ToString(), Transaction);
                                SavePay();

                            }
                        }
                        else
                        {

                            AdminPayPlus.SaveLog("PaymentPaquetesUC", "Respuesta False, navegando a return money ", "OK" + "Dinero ingresado es ", paymentViewModel.ValorIngresado.ToString(), Transaction);
                            Utilities.CloseModal();
                            Transaction.StatePay = "Cancelada";
                            Transaction.Payment = paymentViewModel;
                            Utilities.ShowModal("No se pudo notificar la recarga, se le hara devolucion de su dinero", EModalType.Error);

                            Utilities.navigator.Navigate(UserControlView.ReturnMoneyPaquetes, Transaction);

                        }


                    }
                    else
                    {
                        AdminPayPlus.SaveLog("PaymentPaquetesUC", "Respuesta null, navegando a return money ", "OK" + "Dinero ingresado es ", paymentViewModel.ValorIngresado.ToString(), Transaction);
                        Utilities.CloseModal();
                        Transaction.StatePay = "Cancelada";
                        Transaction.Payment = paymentViewModel;
                        Utilities.ShowModal("No se pudo notificar la recarga, se le hara devolucion de su dinero", EModalType.Error);
                        Utilities.navigator.Navigate(UserControlView.ReturnMoneyPaquetes, Transaction);

                    }

                });

                Utilities.ShowModal("Estamos validando la información, un momento por favor", EModalType.Preload);


            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentPaquetesUC", "Entrando a metodo Notify Catch", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                Utilities.CloseModal();
                Transaction.StatePay = "Cancelada";
                Transaction.Payment = paymentViewModel;
                Utilities.ShowModal("Ocurrió un error al momento de realizar la notificación, se le hara devolucion de su dinero", EModalType.Error);           
               Utilities.navigator.Navigate(UserControlView.ReturnMoneyPaquetes, Transaction);
                //      return false;
            }


        }


        private async Task InhabilitarVista()
        {
            await Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 0.3;
                this.IsEnabled = false;
            });
        }

        private async Task HabilitarVista()
        {
            await Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 1;
                this.IsEnabled = true;
            });
        }



    }

}

