﻿using System.Reflection;
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

namespace WPFApostar.UserControls.Chance
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
            AdminPayPlus.SaveLog("PaymentChanceUC", "entrando a la ejecucion", "OK", "", Transaction);
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
                AdminPayPlus.SaveLog("PaymentChanceUC", "Entrando a la ejecucion OnCashIn paymentViewModel.ValorIngresado >= paymentViewModel.PayValue", "OK", "", Transaction);

                _ = Dispatcher.BeginInvoke((Action)delegate { btnCancell.Visibility = Visibility.Collapsed; });

                await _peripherals.StopAceptance();

                AdminPayPlus.SaveLog("PaymentChanceUC", "Entrando a la ejecucion OnCashIn  StopAceptance entrando a Notify ", "OK", "", Transaction);

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



        private void Btn_CancelarPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            AdminPayPlus.SaveLog("PaymentUC", "Entrando a la ejecucion Btn_CancelarPreviewMouseDown", "OK", "", Transaction);
            CancellPay();
        }

        private void OrganizeValues()
        {
            try
            {
                AdminPayPlus.SaveLog("PaymentChanceUC", "entrando a la ejecucion OrganizeValues", "OK", "", Transaction);

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

                AdminPayPlus.SaveLog("PaymentChanceUC", "saliendo de la ejecucion OrganizeValues", "OK", "", Transaction);

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentChanceUC", "Error Catch la ejecucion OrganizeValues", " Error", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }


        private void CancellPay()
        {
            try
            {
                AdminPayPlus.SaveLog("PaymentChanceUC", "entrando a la ejecucion CancellPay", "OK", "", Transaction);

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
                            AdminPayPlus.SaveLog("PaymentChanceUC", "entrando a la ejecucion CancellPay paymentViewModel.ValorIngresado > 0 navegando a ReturnMoneyChance", "OK", "", Transaction);
                            Transaction.Payment = paymentViewModel;
                            Utilities.navigator.Navigate(UserControlView.ReturnChance, Transaction);
                        }
                        else
                        {
                            AdminPayPlus.SaveLog("PaymentChanceUC", "entrando a la ejecucion CancellPay Navegando al Config", "OK", "", Transaction);
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

                AdminPayPlus.SaveLog("PaymentChanceUC", "saliendo de la ejecucion CancellPay", "OK", "", Transaction);
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentChanceUC", "Error Catch la ejecucion CancellPay", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private async Task SavePay(ETransactionState statePay = ETransactionState.Initial)
        {
            try
            {


                AdminPayPlus.SaveLog("PaymentChanceUC", "entrando a la ejecucion SavePay", "OK", "", Transaction);


                if (!this.paymentViewModel.StatePay)
                {
                    this.paymentViewModel.StatePay = true;
                    Transaction.Payment = paymentViewModel;
                    Transaction.State = statePay;
            
                    AdminPayPlus.SaveLog("PaymentChanceUC", "Navegando a FinishChance", "OK", "", Transaction);

                    Utilities.navigator.Navigate(UserControlView.FinishChance, Transaction);

                  
                }



            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PaymentChanceUC", "Error Catch la ejecucion SavePay", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
              
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
              

                AdminPayPlus.SaveLog("PaymentChanceUC", "entrando a la ejecucion Notify", "OK", "", null);

                InhabilitarVista();


                int LotId = 1;
                List<ApuestasNotify> LstApuestas = new List<ApuestasNotify>();

                List<LoteriaNotify> LstLoteria = new List<LoteriaNotify>();

                foreach (var x in Transaction.ListaChances)
                {

                    ApuestasNotify Apuestas = new ApuestasNotify();

                    Apuestas.id = LotId++;
                    Apuestas.NumeroApostado = x.Numero;
                    Apuestas.ValorDirecto = x.Directo;
                    Apuestas.ValorCombinado = x.Combinado;
                    Apuestas.ValorPata = x.Pata;
                    Apuestas.ValorUna = x.Una;
                    Apuestas.tipoChance = new TipoChanceNotifyM
                    {
                        Id = x.TipoChance
                    };


                    foreach (var y in x.Loterias)
                    {
                        LoteriaNotify Lot = new LoteriaNotify();

                        Lot.codigo = y.CodigoCodesa;

                        LstLoteria.Add(Lot);
                    }


                    Apuestas.ListLoteriasValidate = new ListLoteriasNotify
                    {
                        loteria = new List<LoteriaNotify>()
                    };

                    Apuestas.ListLoteriasValidate.loteria = LstLoteria;

                    LstApuestas.Add(Apuestas);
                }

                RequestNotifyChance Request = new RequestNotifyChance();

                Request.Ubicacion =  Utilities.GetConfiguration("UbicacionMaquina");

                Request.Subproducto = new SubproductoNotify()
                {

                    CodigoServicio = Convert.ToInt32(Utilities.GetConfigData("CodServicio")),
                    Id = Transaction.productSelect.idField

                };

                Request.LstApuestas = new ListApuestasNotify
                {
                    apuestas = new List<ApuestasNotify>()
                };

                Request.AsumeIva = false; 

                Request.FechaSorteo = Transaction.Fecha;

                Request.Transaccion = Transaction.IdTransactionAPi;

                Request.LstApuestas.apuestas = LstApuestas;

                Request.codigoApostar = Utilities.GetConfigData("CodData");

                Request.idPagador = Transaction.IdUser.ToString();

                Request.cedula = Transaction.payer.IDENTIFICATION;            

                Task.Run(() =>
                {

                    AdminPayPlus.SaveLog("PaymentChanceUC", "entrando a la ejecucion NotifyChance", "OK", "", null);
                 
                    var Respuesta = AdminPayPlus.ApiIntegration.NotifyChance(Request);

                    Utilities.CloseModal();

                    if (Respuesta != null)
                    {

                        if (Respuesta.Estado == true)
                        {
                            Transaction.State = ETransactionState.Success;                         
                            Transaction.ResponseNotifyC = Respuesta;

                            if (paymentViewModel.ValorSobrante != 0)
                            {
                                Utilities.CloseModal();
                                AdminPayPlus.SaveLog("PaymentChanceUC", "Entrando a returnmoney valor sobrante es igual a ", "OK", paymentViewModel.ValorSobrante.ToString(), Transaction);
                                Transaction.StatePay = "Aprobado";
                                Transaction.Payment = paymentViewModel;
                                Utilities.navigator.Navigate(UserControlView.ReturnChance, Transaction);
                            }
                            else
                            {
                                Utilities.CloseModal();
                                Transaction.Payment = paymentViewModel;
                                Transaction.StatePay = "Aprobado";
                                AdminPayPlus.SaveLog("PaymentChanceUC", "Navegando al metodo Savepay valor sobrante es igual a ", "OK", paymentViewModel.ValorSobrante.ToString(), Transaction);
                                SavePay();

                            }

                        }
                        else
                        {
                          
                            Utilities.CloseModal();
                            Transaction.StatePay = "Cancelada";
                            Transaction.Payment = paymentViewModel;
                            AdminPayPlus.SaveLog("PaymentChanceUC", "Respuesta False, navegando a return money ", "OK" + "Dinero ingresado es ", paymentViewModel.ValorIngresado.ToString(), Transaction);
                            Utilities.ShowModal("No se pudo notificar el chance, se le hara devolucion de su dinero", EModalType.Error);
                            Utilities.navigator.Navigate(UserControlView.ReturnChance,Transaction);
                        }
                       
                    }
                    else
                    {

                        Utilities.CloseModal();
                        Transaction.StatePay = "Cancelada";
                        Transaction.Payment = paymentViewModel;
                        AdminPayPlus.SaveLog("PaymentChanceUC", "Respuesta null, navegando a return money ", "OK" + "Dinero ingresado es ", paymentViewModel.ValorIngresado.ToString(), Transaction);
                        Utilities.ShowModal("No se pudo notificar la chance, se le hara devolucion de su dinero", EModalType.Error);
                        //   ReturnMoneyCancel(paymentViewModel.ValorIngresado, true);
                        Utilities.navigator.Navigate(UserControlView.ReturnChance, Transaction);
                    }

                });

                Utilities.ShowModal("Estamos validando información un momento por favor", EModalType.Preload);


            }
            catch (Exception ex)
            {
                Utilities.CloseModal();
                Transaction.StatePay = "Cancelada";
                Transaction.Payment = paymentViewModel;
                AdminPayPlus.SaveLog("PaymentChanceUC", "Entrando a metodo Notify Catch", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);             
                Utilities.ShowModal("Ocurrió un error al momento de realizar la notificación, se le hara devolucion de su dinero", EModalType.Error);            
                Utilities.navigator.Navigate(UserControlView.ReturnChance, Transaction);
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
