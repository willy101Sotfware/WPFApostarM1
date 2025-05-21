using System.Windows;
using System.Windows.Controls;
using WPFApostar.Classes;
using WPFApostar.Domain.Peripherals;
using WPFApostar.Models;

namespace WPFApostar.UserControls.Betplay
{
    /// <summary>
    /// Lógica de interacción para ReturnMoneyUC.xaml
    /// </summary>
    public partial class ReturnMoneyUC : UserControl
    {

        Transaction transaction;
        private bool NotifyStatus = false;
        AdminPayPlus init;
        private PeripheralController _peripherals;

        public ReturnMoneyUC(Transaction transaction)
        {
            AdminPayPlus.SaveLog("ReturnMoneyBetplayUC", "entrando al user control", "OK", "", transaction);


            InitializeComponent();

            this.transaction = transaction;

            _peripherals = PeripheralController.Instance;
            _peripherals.CashDispensed += OnCashDispensed;
            _peripherals.DispenserReject += OnDispenserReject;
            _peripherals.PeripheralError += OnPeripheralError;


            this.Unloaded += OnUnloaded;



            if (transaction.StatePay == "Cancelada")
            {
                AdminPayPlus.SaveLog("ReturnMoneyBetplayUC", "entrando al user control estado de transaccion", "OK", transaction.StatePay.ToString(), transaction);

                Return(transaction.Payment.ValorIngresado);
            }
            else if (transaction.StatePay == "Aprobado")
            {
                AdminPayPlus.SaveLog("ReturnMoneyBetplayUC", "entrando al user control estado de transaccion", "OK", transaction.StatePay.ToString(), transaction);
                Return(transaction.Payment.ValorSobrante);
            }


        }

        #region Methods new

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {

            _peripherals.CashDispensed -= OnCashDispensed;
            _peripherals.DispenserReject -= OnDispenserReject;
            _peripherals.PeripheralError -= OnPeripheralError;

        }


        private void OnPeripheralError(Exception ex)
        {
            //TODO: Evaluar Si es necesario reportar errores de perifericos al Dashboard por que ya los errores de perifericos se reportan internamente
        }


        private async void OnCashDispensed(decimal totalDispensed, Dictionary<int, int> details)
        {


            AdminPayPlus.SaveLog("ReturnMoneyUserControl", "entrando a la ejecucion OnCashDispensed", "OK", totalDispensed.ToString(), transaction);

            transaction.Payment.ValorDispensado = totalDispensed;


            Utilities.CloseModal();

            SendDispenseDetails(details);

            // paymentViewModel.ValorDispensado = totalout;
            //transaction.StateReturnMoney = false;

            decimal valorOut = transaction.Payment.ValorSobrante - transaction.Payment.ValorDispensado;
            decimal valorOut2 = transaction.Payment.ValorIngresado - transaction.Payment.ValorDispensado;
            string montoFormateado = string.Format("{0:#,0.##}", valorOut);
            string montoFormateado2 = string.Format("{0:#,0.##}", valorOut2);

            if (transaction.StatePay == "Cancelada")
            {
                NotifyStatus = false;
                AdminPayPlus.SaveLog("ReturnMoneyBetplayUC", "OnCashDispensed, el estado de la transaccion es Cancelada", "OK", "", transaction);

                if (transaction.Payment.ValorDispensado < transaction.Payment.ValorIngresado)
                {
                    AdminPayPlus.SaveLog("ReturnMoneyBetplayUC", "OnCashDispensed, entrando a la condicion transaction.Payment.ValorDispensado < transaction.Payment.ValorIngresado", "OK", "", transaction);


                    Utilities.CloseModal();
                    transaction.StateReturnMoney = false;
                    var faltante = transaction.Payment.ValorIngresado - transaction.Payment.ValorDispensado;
                    Utilities.ShowModal(String.Concat("No se pudo entregar la totalidad del dinero hay un faltante de: $" + montoFormateado2 + " por favor comunícate a la linea de atención."), EModalType.Error, false);
                    transaction.StatePay = "Cancelada";
                    transaction.State = ETransactionState.Cancel;
                    savepay(NotifyStatus);
                }
                else if (transaction.Payment.ValorDispensado == transaction.Payment.ValorIngresado)
                {
                    AdminPayPlus.SaveLog("ReturnMoneyBetplayUC", "OnCashDispensed, entrando a la condiciontransaction.Payment.ValorDispensado == transaction.Payment.ValorIngresado", "OK", "", transaction);


                    Utilities.CloseModal();
                    transaction.StateReturnMoney = true;
                    transaction.StatePay = "Cancelada";
                    transaction.State = ETransactionState.Cancel;
                    savepay(NotifyStatus);

                }
            }
            else if (transaction.StatePay == "Aprobado")
            {
                AdminPayPlus.SaveLog("ReturnMoneyBetplayUC", "OnCashDispensed, el estado de la transaccion es Aprobado", "OK", "", transaction);
                NotifyStatus = true;
                if (transaction.Payment.ValorDispensado < transaction.Payment.ValorSobrante)
                {


                    AdminPayPlus.SaveLog("ReturnMoneyBetplayUC", "OnCashDispensed, entrando a la condicion transaction.Payment.ValorDispensado < transaction.Payment.ValorIngresado", "OK", "", transaction);

                    Utilities.CloseModal();
                    transaction.StateReturnMoney = false;
                    var faltante = transaction.Payment.ValorIngresado - transaction.Payment.ValorDispensado;
                    Utilities.ShowModal(String.Concat("No se pudo entregar la totalidad del dinero hay un faltante de: $" + montoFormateado + " por favor comunícate a la linea de atención."), EModalType.Error, false);
                    transaction.StatePay = "Aprobado";
                    transaction.State = ETransactionState.Success;
                    savepay(NotifyStatus);
                }
                else if (transaction.Payment.ValorDispensado == transaction.Payment.ValorSobrante)
                {

                    AdminPayPlus.SaveLog("ReturnMoneyBetplayUC", "OnCashDispensed, entrando a la condicion transaction.Payment.ValorDispensado == transaction.Payment.ValorSobrante", "OK", "", transaction);


                    Utilities.CloseModal();
                    transaction.StateReturnMoney = true;
                    transaction.StatePay = "Aprobado";
                    transaction.State = ETransactionState.Success;
                    savepay(NotifyStatus);

                }


            }
        }


        private void Return(decimal returnValue)
        {
            if (!Utilities.GetConfiguration("noPeripherals").Equals("true"))
            {
                _peripherals.StartDispenser(returnValue);
            }
            else
            {
                // En modo prueba, simulamos que se dispensó todo el valor
                OnCashDispensed(returnValue, new Dictionary<int, int>());
            }
        }


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
            var mdString = GetDetailsString(details, new[] { 100, 500 }, "MD");

            // Enviar primero la información de DP
            if (!string.IsNullOrEmpty(dpString))
            {
                AdminPayPlus.SaveDetailsTransaction(transaction.IdTransactionAPi, 0, 0, 0, string.Empty, dpString);
            }

            // Luego enviar la información de MD
            if (!string.IsNullOrEmpty(mdString))
            {
                AdminPayPlus.SaveDetailsTransaction(transaction.IdTransactionAPi, 0, 0, 0, string.Empty, mdString);
            }
        }

        private void OnDispenserReject(Dictionary<int, int> rejectData)
        {

            SendDispenseDetails(rejectData, isReject: true);

        }


        public void savepay(bool notify)
        {
            Utilities.CloseModal();

            // init.CleanValues();
            AdminPayPlus.SaveLog("ReturnMoneyBetplayUC", "Entrando a la ejecucion savepay", "OK", notify.ToString(), transaction);
            if (notify)
            {
                AdminPayPlus.SaveLog("ReturnMoneyBetplayUC", "SavePay, transaccion aprobada, navegando al finish", "OK", "", transaction);


                Utilities.navigator.Navigate(UserControlView.Finish, transaction);
            }
            else
            {
                AdminPayPlus.SaveLog("ReturnMoneyBetplayUC", "SavePay, transaccion Cancelada, ", "OK", "", transaction);


                transaction.State = ETransactionState.Cancel;

                transaction.StatePay = "Cancelada";


                AdminPayPlus.SaveLog("ReturnMoneyBetplayUC", "FinishCancelNotPay", "OK", string.Concat("ID Transaccion:", transaction.IdTransactionAPi, "/n", "Estado Transaccion:", transaction.StatePay.ToString(), "/n", "Monto:", transaction.Amount.ToString(), "/n", "Valor Dispensado:", transaction.Payment.ValorDispensado.ToString(), "/n", "Valor Ingresado:", transaction.Payment.ValorIngresado.ToString()), transaction);


                AdminPayPlus.UpdateTransactionBetplay(transaction);

                AdminPayPlus.SaveLog("ReturnMoneyBetplayUC", "Saliendo de la ejecucion savepay false", "OK", "", transaction);

                Utilities.navigator.Navigate(UserControlView.Config);
            }

        }

        #endregion


    }
}
