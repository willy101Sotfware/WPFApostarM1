using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPFApostar.Classes;
using WPFApostar.Domain.Peripherals;
using WPFApostar.Models;
using WPFApostar.Resources;
using WPFApostar.Services.ObjectIntegration;
using WPFApostar.ViewModel;

namespace WPFApostar.UserControls.Betplay
{
    /// <summary>
    /// Lógica de interacción para PaymentUC.xaml
    /// </summary>
    public partial class PaymentUC : UserControl
    {


        private Transaction Transaction;
        private ETypeTramites typeTramites;
        public PaymentViewModel paymentViewModel;
        private PeripheralController _peripherals;
        private int Intentos = 1;
        private Task notify = null;
        private bool NotifyStatus = false;

        public PaymentUC(Transaction Ts)
        {
            AdminPayPlus.SaveLog("PaymentUserControl", "Entrando a la ejecucion", "OK", "", Transaction);
            InitializeComponent();
            Transaction = Ts;

            Transaction.DevueltaCorrecta = false;

#if NO_PERIPHERALS
            // En modo de prueba, agregamos botones de prueba
            var testButton1 = new Button { Content = "1000", Width = 100, Height = 50, Margin = new Thickness(5) };
            var testButton2 = new Button { Content = "2000", Width = 100, Height = 50, Margin = new Thickness(5) };
            var testButton3 = new Button { Content = "5000", Width = 100, Height = 50, Margin = new Thickness(5) };
            var testButton4 = new Button { Content = "10000", Width = 100, Height = 50, Margin = new Thickness(5) };
            var testButton5 = new Button { Content = "20000", Width = 100, Height = 50, Margin = new Thickness(5) };
            var testButton6 = new Button { Content = "50000", Width = 100, Height = 50, Margin = new Thickness(5) };

            testButton1.Click += (s, e) => OnCashIn(1000);
            testButton2.Click += (s, e) => OnCashIn(2000);
            testButton3.Click += (s, e) => OnCashIn(5000);
            testButton4.Click += (s, e) => OnCashIn(10000);
            testButton5.Click += (s, e) => OnCashIn(20000);
            testButton6.Click += (s, e) => OnCashIn(50000);

            var testPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
            testPanel.Children.Add(testButton1);
            testPanel.Children.Add(testButton2);
            testPanel.Children.Add(testButton3);
            testPanel.Children.Add(testButton4);
            testPanel.Children.Add(testButton5);
            testPanel.Children.Add(testButton6);

            MainGrid.Children.Add(testPanel);
#else
            _peripherals = PeripheralController.Instance;
            _peripherals.CashIn += OnCashIn;
#endif

            this.Unloaded += OnUnloaded;
            this.Loaded += OnLoaded;



          

        }

        #region NewMethodsPay

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            OrganizeValues();

#if NO_PERIPHERALS
            // En modo de prueba, no necesitamos iniciar los periféricos
            AdminPayPlus.SaveLog("PaymentUC", "OnLoaded", "Modo NO_PERIPHERALS: No se inician periféricos", "", null);
#else
            _peripherals.StartAcceptance(paymentViewModel.PayValue);
#endif
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
#if NO_PERIPHERALS
            // En modo de prueba, no necesitamos detener los periféricos
            AdminPayPlus.SaveLog("PaymentUC", "OnUnloaded", "Modo NO_PERIPHERALS: No se detienen periféricos", "", null);
#else
            _peripherals.CashIn -= OnCashIn;
#endif
        }

        private async void OnCashIn(decimal value)
        {
            paymentViewModel.ValorIngresado += value;
            paymentViewModel.RefreshListDenomination(Convert.ToInt32(value), 1);
            LoadView();

            AdminPayPlus.SaveLog("PaymentUserControl", "Entrando a la ejecucion OnCashIn  value ", value.ToString(), "", Transaction);

            // Determina el tipo de operación basado en el valor
            string typeOperation = (value == 100 || value == 200 || value == 500 || value == 1000) ? "MA" : "AP";

            // Llama al método con el tipo de operación correspondiente
            AdminPayPlus.SaveDetailsTransaction(Transaction.IdTransactionAPi, value, 2, 1, typeOperation, string.Empty);

            if (paymentViewModel.ValorIngresado >= paymentViewModel.PayValue)
            {
                AdminPayPlus.SaveLog("PaymentUserControl", "Entrando a la ejecucion OnCashIn paymentViewModel.ValorIngresado >= paymentViewModel.PayValue", "OK", "", Transaction);

                _ = Dispatcher.BeginInvoke((Action)delegate { btnCancell.Visibility = Visibility.Collapsed; });

#if NO_PERIPHERALS
                // En modo de prueba, no necesitamos detener los periféricos
                AdminPayPlus.SaveLog("PaymentUC", "OnCashIn", "Modo NO_PERIPHERALS: No se detienen periféricos", "", null);
#else
                await _peripherals.StopAceptance();
#endif

                AdminPayPlus.SaveLog("PaymentUserControl", "Entrando a la ejecucion OnCashIn  StopAceptance entrando a Notify ", "OK", "", Transaction);
                Notify();
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


        private void Btn_CancelarTouchDown(object sender, EventArgs e)
        {
            AdminPayPlus.SaveLog("PaymentUserControl", "Entrando a la ejecucion Btn_CancelarTouchDown", "OK", "", Transaction);

            CancellPay();
        }

        private void Btn_CancelarPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            AdminPayPlus.SaveLog("PaymentUserControl", "Entrando a la ejecucion Btn_CancelarPreviewMouseDown", "OK", "", Transaction);

            CancellPay();
        }

        private void OrganizeValues()
        {
            try
            {
                AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion OrganizeValues", "OK", "", Transaction);

                Transaction.Amount = Utilities.RoundValue(Convert.ToDecimal(Transaction.Amount)).ToString();

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

                AdminPayPlus.SaveLog("PaymentUserControl", "saliendo de la ejecucion OrganizeValues", "OK", "", Transaction);

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentUserControl", "Error Catch la ejecucion OrganizeValues", " Error", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

      
        private void CancellPay()
        {
            try
            {
                AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion CancellPay", "OK", "", Transaction);

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
                            AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion CancellPay paymentViewModel.ValorIngresado > 0 navegando a ReturnMoneyPaquetes", "OK", "", Transaction);
                            Transaction.StatePay = "Cancelada";
                            Transaction.Payment = paymentViewModel;
                            Utilities.navigator.Navigate(UserControlView.ReturnMoney, Transaction);
                        }
                        else
                        {
                            AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion CancellPay Navegando al Config", "OK", "", Transaction);
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

                AdminPayPlus.SaveLog("PaymentUserControl", "saliendo de la ejecucion CancellPay", "OK", "", Transaction);
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentUserControl", "Error Catch la ejecucion CancellPay", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private async Task SavePay(ETransactionState statePay = ETransactionState.Initial)
        {
            try
            {


                AdminPayPlus.SaveLog("PaymentUserControl", "entrando a la ejecucion SavePay", "OK", "", Transaction);


                if (!this.paymentViewModel.StatePay)
                {
                    this.paymentViewModel.StatePay = true;
                    Transaction.Payment = paymentViewModel;
                    Transaction.State = statePay;

                    // AdminPayPlus.ControlPeripherals.ClearValues();

                    AdminPayPlus.SaveLog("PaymentUserControl", "Navegando a FinishBetplay", "OK", "", Transaction);

                    Utilities.navigator.Navigate(UserControlView.Finish, Transaction);

                      GC.Collect();
                }



            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentUserControl", "Error Catch la ejecucion SavePay", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
                //   CancelTransaction();
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
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.Error);
            }
        }

     
        public void Notify()
        {

            try
            {
                InhabilitarVista();

                Task.Run(() =>
                {

                    AdminPayPlus.SaveLog("PaymentUC", "entrando a la ejecucion Notify", "OK", "", null);


                    RequestNotifyBetplay Request = new RequestNotifyBetplay
                    {
                        Ubicacion =  Utilities.GetConfiguration("UbicacionMaquina"),
                        Token = Transaction.ResponseTokenBetplay.Token,
                        ClienteId = Convert.ToUInt64(Transaction.Document),
                        transaccionId = Transaction.Transacciondistribuidorid,
                        valor = Convert.ToInt32(Transaction.Amount),                        
                    };
               

                    Request.subproducto = new subproducto
                    {
                        Id = Transaction.ProductSelected.Id
                    };
               

                    var Respuesta = AdminPayPlus.ApiIntegration.NotifyPayment(Request);

                  

                    if (Respuesta != null)
                    {

                        if (Respuesta.estado == "true")
                        {

                            Transaction.ResponseNotifyBetPlay = Respuesta;
                            Transaction.State = ETransactionState.Success;


                            if (paymentViewModel.ValorSobrante != 0)
                            {
                                Utilities.CloseModal();
                                AdminPayPlus.SaveLog("PaymentUserControl", "Entrando a returnmoney valor sobrante es igual a ", "OK", paymentViewModel.ValorSobrante.ToString(), Transaction);
                                Transaction.StatePay = "Aprobado";
                                Transaction.Payment = paymentViewModel;
                                Utilities.navigator.Navigate(UserControlView.ReturnMoney, Transaction);
                            }
                            else
                            {
                                Utilities.CloseModal();
                                Transaction.Payment = paymentViewModel;
                                Transaction.StatePay = "Aprobado";
                                AdminPayPlus.SaveLog("PaymentUserControl", "Navegando al metodo Savepay valor sobrante es igual a ", "OK", paymentViewModel.ValorSobrante.ToString(), Transaction);
                                SavePay();

                            }


                        }
                        else
                        {
                            Utilities.CloseModal();
                            AdminPayPlus.SaveLog("PaymentUserControl", "Respuesta False, navegando a return money ", "OK" + "Dinero ingresado es ", paymentViewModel.ValorIngresado.ToString(), Transaction);
                            Utilities.ShowModal("No se pudo notificar la recarga, se le hara devolucion de su dinero", EModalType.Error);
                            Transaction.Payment = paymentViewModel;
                            Transaction.StatePay = "Cancelada";
                            Utilities.navigator.Navigate(UserControlView.ReturnMoney, Transaction);
                        }

                       
                                
                    }
                    else
                    {
                        Utilities.CloseModal();
                        AdminPayPlus.SaveLog("PaymentUserControl", "Respuesta null, navegando a return money ", "OK" + "Dinero ingresado es ", paymentViewModel.ValorIngresado.ToString(), Transaction);
                        Utilities.ShowModal("No se pudo notificar la recarga, se le hara devolucion de su dinero", EModalType.Error);
                        Transaction.Payment = paymentViewModel;
                        Transaction.StatePay = "Cancelada";
                        Utilities.navigator.Navigate(UserControlView.ReturnMoney, Transaction);
                        
                        //     ReturnMoney(paymentViewModel.ValorIngresado, false);
                    //    Utilities.navigator.Navigate(UserControlView.Menu);
                    }
                  
                });

                Utilities.ShowModal("Estamos validando información un momento por favor", EModalType.Preload);


            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentPaquetesUC", "Entrando a metodo Notify Catch", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                Utilities.CloseModal();
                Utilities.ShowModal("No se pudo notificar la recarga, se le hara devolucion de su dinero", EModalType.Error);              
                Transaction.Payment = paymentViewModel;
                Transaction.StatePay = "Cancelada";
                Utilities.navigator.Navigate(UserControlView.ReturnMoney, Transaction);
                //      return false;
            }

            //    return false;
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
