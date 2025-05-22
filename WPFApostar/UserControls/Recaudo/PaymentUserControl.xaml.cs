using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPFApostar.Domain.UIServices.Integration;
using WPFApostar.Domain.UIServices.ObjectIntegration;
using WPFApostar.Resources;
using WPFApostar.ViewModel;

namespace WPFApostar.UserControls.Recaudo
{
    /// <summary>
    /// Lógica de interacción para PaymentUserControl.xaml
    /// </summary>
    public partial class PaymentUserControl : UserControl
    {

        private Transaction Transaction;
        private ETypeTramites typeTramites;
        public PaymentViewModel paymentViewModel;
        private PeripheralController _peripherals;
        private int Intentos = 1;
        private Task notify = null;
        private bool NotifyStatus = false;

        public PaymentUserControl(Transaction Ts)
        {
            AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "entrando a la ejecucion", "OK", "", Transaction);
            InitializeComponent();
            Transaction = Ts;

            Transaction.DevueltaCorrecta = false;

#if NO_PERIPHERALS
            // Modo Desarrollo: Usar botones de prueba
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
            // Modo Demo o Producción: Usar periféricos reales
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
            // Modo Desarrollo: No iniciar periféricos
            AdminPayPlus.SaveLog("PaymentUC", "OnLoaded", "Modo Desarrollo: No se inician periféricos", "", null);
#else
            // Modo Demo o Producción: Iniciar periféricos
            _peripherals.StartAcceptance(paymentViewModel.PayValue);
#endif
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
#if NO_PERIPHERALS
            // Modo Desarrollo: No detener periféricos
            AdminPayPlus.SaveLog("PaymentUC", "OnUnloaded", "Modo Desarrollo: No se detienen periféricos", "", null);
#else
            // Modo Demo o Producción: Detener periféricos
            _peripherals.CashIn -= OnCashIn;
#endif
        }

        private async void OnCashIn(decimal value)
        {
            paymentViewModel.ValorIngresado += value;
            paymentViewModel.RefreshListDenomination(Convert.ToInt32(value), 1);
            LoadView();

            // Solo enviamos al API si NO estamos en modo Demo
            if (!Utilities.GetConfiguration("noPeripherals").Equals("true"))
            {
                AdminPayPlus.SaveDetailsTransaction(Transaction.IdTransactionAPi, value, 2, 1, "AP", string.Empty);
            }

            if (paymentViewModel.ValorIngresado >= paymentViewModel.PayValue)
            {
                AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "Entrando a la ejecucion OnCashIn paymentViewModel.ValorIngresado >= paymentViewModel.PayValue", "OK", "", Transaction);

                _ = Dispatcher.BeginInvoke((Action)delegate { btnCancell.Visibility = Visibility.Collapsed; });

#if NO_PERIPHERALS
                // Modo Desarrollo: No detener periféricos
                AdminPayPlus.SaveLog("PaymentUC", "OnCashIn", "Modo Desarrollo: No se detienen periféricos", "", null);
#else
                // Modo Demo o Producción: Detener periféricos
                await _peripherals.StopAceptance();
#endif

                AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "Entrando a la ejecucion OnCashIn  StopAceptance entrando a NotifyRecaudo ", "OK", "", Transaction);
                NotifyRecaudo();
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
            // Solo enviamos al API si NO estamos en modo Demo
            if (Utilities.GetConfiguration("noPeripherals").Equals("true"))
            {
                return;
            }

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

        private void OnDispenserReject(Dictionary<int, int> rejectData)
        {
            SendDispenseDetails(rejectData, isReject: true);
        }

        #endregion

        private void OrganizeValues()
        {
            try
            {
                string mensaje = string.Concat("PaymentRecaudoUserControl", " ", "entrando a la ejecucion organizevalues", " ", "OK");
                AdminPayPlus.SaveLog(mensaje);


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

                if (Convert.ToInt32(Transaction.RealAmount) % 100 != 0)
                {
                    moreMs = $"¿ Desea asumir el ajuste de {String.Format("{0:C0}", Transaction.RealAmount)} a {String.Format("{0:C0}", Convert.ToDecimal(Transaction.Amount))} ?";

                    if (!Utilities.ShowModal(string.Concat("Este dispositivo no devuelve cantidades inferiores a $100. ", Environment.NewLine, moreMs, " este sera un saldo a favor para su proxima factura"), EModalType.Information, false))
                    {
                        Utilities.navigator.Navigate(UserControlView.Config);
                    }
                }

                mensaje = string.Concat("PaymentRecaudoUserControl", " ", "Saliendo de la ejecucion organizevalues", " ", "OK");
                AdminPayPlus.SaveLog(mensaje);
            }
            catch (Exception ex)
            {
                string mensaje = string.Concat("catch organizevalues", " ", "Error", " ", ex.Message, " ", ex.StackTrace);
                AdminPayPlus.SaveLog(mensaje);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.Error);
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

        private void BtnCancell_TouchDown(object sender, TouchEventArgs e)
        {
            AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "Entrando a la ejecucion BtnCancell_TouchDown", "OK", "", Transaction);

            CancellPay();
        }

        private void CancellPay()
        {
            try
            {
                AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "entrando a la ejecucion CancellPay", "OK", "", Transaction);

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
                            AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "entrando a la ejecucion CancellPay paymentViewModel.ValorIngresado > 0 navegando a ReturnMoneyRe", "OK", "", Transaction);
                            Transaction.StatePay = "Cancelada";
                            Transaction.Payment = paymentViewModel;
                            Utilities.navigator.Navigate(UserControlView.ReturnMoneyRe, Transaction);
                        }
                        else
                        {
                            AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "entrando a la ejecucion CancellPay Navegando al Config", "OK", "", Transaction);
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

                AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "saliendo de la ejecucion CancellPay", "OK", "", Transaction);
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "Error Catch la ejecucion CancellPay", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
            }
        }

        public async Task SavePay(ETransactionState statePay = ETransactionState.Initial)
        {
            AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "entrando a la ejecucion SavePay", "OK", "", Transaction);

            if (!this.paymentViewModel.StatePay)
            {
                this.paymentViewModel.StatePay = true;
                Transaction.Payment = paymentViewModel;
                Transaction.State = statePay;
            
                AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "Navegando a SuccesRecaudo", "OK", "", Transaction);

                Utilities.navigator.Navigate(UserControlView.SuccesRecaudo, Transaction);
            }
        }

        public void NotifyRecaudo()
        {
            try
            {
                InhabilitarVista();

                AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "entrando a la ejecucion NotifyRecaudo", "OK", "", null);

                List<Camposm> LstdataConsult = new List<Camposm>();

                foreach (var x in Transaction.infoNotify)
                {
                    Camposm DataConsult = new Camposm();

                    DataConsult.id = x.id;
                    DataConsult.nombre = x.Nombre;
                    DataConsult.valor = x.valor;

                    LstdataConsult.Add(DataConsult);
                }

                RequestNotifyRecaudo Request = new RequestNotifyRecaudo
                {
                    IdDepartamento = new Iddepartamento
                    {
                        codigo = Convert.ToInt32(Utilities.GetConfiguration("IdDepartamento"))
                    },
                    Recaudo = new RecaudoNotify
                    {
                        codigo = Transaction.Company.Codigo,
                        valor = Convert.ToInt32(Transaction.Amount)
                    },
                    IdTrx = Transaction.IdTransactionAPi,
                    ServicioNotify = new Servicionotify
                    {
                        id = Transaction.IdServicioRecaudo,
                        listadoCampoNotify = new Listadocamponotify
                        {
                            camposM = new List<Camposm>()
                        }
                    }
                };

                Request.ServicioNotify.listadoCampoNotify.camposM = LstdataConsult;

                Task.Run(() =>
                {
                    var ResponseNotify = AdminPayPlus.ApiIntegration.NotifyPaymentRecaudo(Request);

                    if (ResponseNotify != null)
                    {
                        if (ResponseNotify.Estado == true)
                        {
                            Transaction.ResponseNotifyPayment = ResponseNotify;
                            Transaction.TransaccionRecaudo = ResponseNotify.Transaccionid;
                            Transaction.CodigoSeguridad = ResponseNotify.Codigoseguridad;
                            Transaction.State = ETransactionState.Success;

                            AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "saliendo de la ejecucion NotifyPaquetes estado", "OK", ResponseNotify.Estado.ToString(), Transaction);

                            if (paymentViewModel.ValorSobrante != 0)
                            {
                                Utilities.CloseModal();
                                AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "Entrando a returnmoney valor sobrante es igual a ", "OK", paymentViewModel.ValorSobrante.ToString(), Transaction);
                                Transaction.StatePay = "Aprobado";
                                Transaction.Payment = paymentViewModel;
                                Utilities.navigator.Navigate(UserControlView.ReturnMoneyRe, Transaction);
                            }
                            else
                            {
                                Utilities.CloseModal();
                                Transaction.Payment = paymentViewModel;
                                Transaction.StatePay = "Aprobado";
                                AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "Navegando al metodo Savepay valor sobrante es igual a ", "OK", paymentViewModel.ValorSobrante.ToString(), Transaction);
                                SavePay();
                            }
                        }
                        else
                        {
                            AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "Respuesta False, navegando a return money ", "OK" + "Dinero ingresado es ", paymentViewModel.ValorIngresado.ToString(), Transaction);
                            Utilities.CloseModal();
                            Transaction.StatePay = "Cancelada";
                            Transaction.Payment = paymentViewModel;
                            Utilities.ShowModal("Hubo un error al momento de notificar el pago se le hara devolucion de su dinero", EModalType.Error);
                            Utilities.navigator.Navigate(UserControlView.ReturnMoneyRe, Transaction);
                        }
                    }
                    else
                    {
                        AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "Respuesta null, navegando a return money ", "OK" + "Dinero ingresado es ", paymentViewModel.ValorIngresado.ToString(), Transaction);
                        Utilities.CloseModal();
                        Transaction.EstadoTransaccion = "Cancelada";
                        Transaction.Payment = paymentViewModel;
                        Utilities.ShowModal("Hubo un error al momento de notificar el pago se le hara devolucion de su dinero", EModalType.Error);
                        Utilities.navigator.Navigate(UserControlView.ReturnMoneyRe, Transaction);
                    }
                });
            }
            catch(Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentRecaudoUserControl", "Entrando a metodo NotifyPaymentRecaudo Catch", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                Utilities.CloseModal();
                Transaction.StatePay = "Cancelada";
                Transaction.Payment = paymentViewModel;
                Utilities.ShowModal("Hubo un error al momento de notificar el pago se le hara devolucion de su dinero", EModalType.Error);
                Utilities.navigator.Navigate(UserControlView.ReturnMoneyRe, Transaction);
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
