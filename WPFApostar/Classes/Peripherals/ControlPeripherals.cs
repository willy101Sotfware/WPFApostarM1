using WPFApostar.Classes.UseFull;
using WPFApostar.Services.Object;
using WPFApostar.Classes.Peripherals;
using System.IO.Ports;

namespace WPFApostar.Classes
{
    public class ControlPeripherals
    {
        #region References

        #region "Timer"
        private TimerGeneric timer;
        #endregion


        #region SerialPorts
        private SerialPort _serialPort;

        #endregion

        #region CommandsPorts
        private string _StartPeripherals = "OR:START";

        private string _AceptanceBillOn = "OR:ON:AP";

        private string _DispenserBillOn = "OR:ON:DP";

        private string _AceptanceBillOFF = "OR:OFF:AP";
        #endregion

        #region Callbacks
        public Action<Tuple<decimal, string>> callbackValueIn;//Calback para cuando ingresan un billete

        public Action<Tuple<decimal, string>> callbackValueOut;//Calback para cuando sale un billete

        public Action<decimal> callbackTotalIn;//Calback para cuando se ingresa la totalidad del dinero

        public Action<decimal> callbackTotalOut;//Calback para cuando sale la totalidad del dinero

        public Action<decimal> callbackIncompleteOut;//Calback para cuando sale la totalidad del dinero

        public Action<decimal> callbackOut;//Calback para cuando sale cieerta cantidad del dinero

        public Action<Tuple<string, string>> callbackError;//Calback de error

        private bool token = false;

        public Action<string> callbackLog;//Calback de error

        public Action<string> callbackMessage;//Calback de mensaje

        public Action<bool> callbackToken;//Calback de mensaje

        private MEIAcceptorControl AcceptorControl;
        #endregion

        #region EvaluationValues
        private int _dividerBills;
        private int _dividerCoins;
        #endregion

        #region Variables
        private decimal payValue;//Valor a pagar

        private List<Tuple<string, int>> denominationsDispenser;

        private decimal enterValue;//Valor ingresado

        private decimal deliveryValue;//Valor entregado

        public decimal deliveryVal;

        private decimal dispenserValue;//Valor a dispensar

        private bool stateError;

        private int typeDispend;

        private static string TOKEN;//Llabe que retorna el dispenser
        #endregion

        #endregion

        #region LoadMethods
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public ControlPeripherals(string portNameBills, string denominatios, int dividerBills = 1, int dividerCoins = 100)
        {
            try
            {
                if (_serialPort == null && !string.IsNullOrEmpty(portNameBills))
                {
                    _serialPort = new SerialPort();
                    InitPortBills(portNameBills);
                }

                if (AcceptorControl == null)
                {
                    AcceptorControl = new MEIAcceptorControl();
                }

                if (!string.IsNullOrEmpty(denominatios))
                {
                    this._dividerBills = dividerBills;
                    this._dividerCoins = dividerCoins;

                    ConfigurateDispenser(denominatios);
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(Tuple.Create("", string.Concat("Error (ControlPeripherals), Iniciando los perifericos", ex)));
            }
        }

        /// <summary>
        /// Método que inicializa los billeteros
        /// </summary>
        public void Start()
        {
            try
            {
                if (!SendMessageBills(_StartPeripherals))
                {
                    callbackError?.Invoke(Tuple.Create("", string.Concat("Error (Start), Iniciando los perifericos", "No se pudo iniciar")));
                }
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(Tuple.Create("", string.Concat("Error (Start), Iniciando aceptacion", ex)));
            }
        }

        /// <summary>
        /// Método para inciar el puerto de los billeteros
        /// </summary>
        private void InitPortBills(string portName)
        {
            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.PortName = portName;
                    _serialPort.ReadTimeout = 3000;
                    _serialPort.WriteTimeout = 500;
                    _serialPort.BaudRate = 57600;
                    _serialPort.DtrEnable = true;
                    _serialPort.RtsEnable = true;
                    _serialPort.Open();
                }

                _serialPort.DataReceived += new SerialDataReceivedEventHandler(_serialPortBillsDataReceived);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region SendMessage
        /// <summary>
        /// Método para enviar orden al puerto de los billeteros
        /// </summary>
        /// <param name="message">mensaje a enviar</param>
        private bool SendMessageBills(string message)
        {
            try
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion SendMessageBills", "OK", message, null);


                if (_serialPort.IsOpen)
                {
                    Thread.Sleep(2000);
                    _serialPort.Write(message);

                    AdminPayPlus.SaveLog(new RequestLog
                    {
                        Reference = "",
                        Description = "Mensaje al billetero " + message,
                        State = 1,
                        Date = DateTime.Now
                    }, ELogType.General);

                    AdminPayPlus.SaveLog("ControlPeripherals", "Orden a los perifericos ejecucion SendMessageBills", "OK", message, null);
                    return true;
                }

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "Error Catch la ejecucion SendMessageBills", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                callbackError?.Invoke(Tuple.Create("AP", "Error (SendMessageBills), ha ocurrido una exepcion " + ex));
            }
            return false;
        }
        #endregion

        #region Listeners
        /// <summary>
        /// Método que escucha la respuesta del puerto del billetero
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _serialPortBillsDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion _serialPortBillsDataReceived", "OK", "", null);


                string response = _serialPort.ReadLine();
                if (!string.IsNullOrEmpty(response))
                {
                    ProcessResponseBills(response.Replace("\r", string.Empty));
                }

                AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion _serialPortBillsDataReceived", "OK", "", null);

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "Error Catch la ejecucion _serialPortBillsDataReceived", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);

                callbackError?.Invoke(Tuple.Create("AP", "Error (_serialPortBillsDataReceived), ha ocurrido una exepcion " + ex));
            }
        }
        #endregion

        #region ProcessResponse
        /// <summary>
        /// Método que procesa la respuesta del puerto de los billeteros
        /// </summary>
        /// <param name="message">respuesta del puerto de los billeteros</param>
        private void ProcessResponseBills(string message)
        {
            string[] response = message.Split(':');
            switch (response[0])
            {
                case "RC":
                    ProcessRC(response);
                    break;
                case "ER":

                    var errorDiveces = Utilities.ErrorDevice();

                    if (errorDiveces != null)
                    {
                        foreach (var item in errorDiveces)
                        {
                            if (message.ToLower().Contains(item.ToLower()))
                            {
                                AdminPayPlus.SaveLog(new RequestLog
                                {
                                    Reference = "",
                                    Description = "Respuesta del billetero " + message,
                                    State = 1,
                                    Date = DateTime.Now
                                }, ELogType.General);
                            }
                        }
                    }

                    ProcessER(response);
                    break;
                case "UN":
                    ProcessUN(response);
                    break;
                case "TO":
                    ProcessTO(response);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region ProcessResponseCases
        /// <summary>
        /// Respuesta para el caso de Recepción de un mensaje enviado
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessRC(string[] response)
        {
            AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ProcessRC", "OK", "", null);


            if (response[1] == "OK")
            {
                switch (response[2])
                {
                    case "AP":

                        break;
                    case "DP":
                        if (response[3] == "HD" && !string.IsNullOrEmpty(response[4]))
                        {
                            TOKEN = response[4].Replace("\r", string.Empty);

                            token = true;
                            AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion CASE DP", "OK", String.Concat(response[1].ToString() + TOKEN), null);

                            callbackToken?.Invoke(true);

                            AcceptorControl.OpenAcceptor(Utilities.GetConfiguration("PortAP"));
                        }
                        break;
                    case "MD":
                        break;
                    default:
                        break;
                }
            }

            AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion ProcessRC", "OK", "", null);

        }

        /// <summary>
        /// Respuesta para el caso de error
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessER(string[] response)
        {
            AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ProcessER", "OK", String.Concat(response[1].ToString() + response[2].ToString()), null);


            if (response[1] == "DP" || response[1] == "MD")
            {
                if (response[2] == "Abnormal Near End sensor of the cassette")
                {
                    try
                    {
                        stateError = false;
                        callbackError?.Invoke(Tuple.Create(response[1], string.Concat("Error, ", response[2])));
                    }
                    catch { }
                }
                else
                {
                    stateError = true;
                    callbackError?.Invoke(Tuple.Create(response[1], string.Concat("Error, se alcanzó a entregar:", deliveryVal, " Error: ", response[2])));
                }
            }
            if (response[1] == "AP")
            {
                stateError = true;
                callbackError?.Invoke(Tuple.Create("AP", "Error, en el billetero Aceptador: " + response[2]));
            }
            else if (response[1] == "FATAL")
            {
                callbackError?.Invoke(Tuple.Create("FATAL", "Error, FATAL" + response[2]));
            }

            AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion ProcessER", "OK", String.Concat(response[1].ToString() + response[2].ToString()), null);

        }

        /// <summary>
        /// Respuesta para el caso de ingreso o salida de un billete/moneda
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessUN(string[] response)
        {
            AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ProcessUN", "OK", String.Concat(response[1].ToString() + response[2].ToString()), null);

            if (response[1] == "DP")
            {
                deliveryValue += decimal.Parse(response[2]) * _dividerBills;
                AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion ProcessUN", "OK", String.Concat(response[1].ToString() + response[2].ToString()), null);

                callbackValueOut?.Invoke(Tuple.Create(decimal.Parse(response[2]) * _dividerBills, response[1]));
            }
            else if (response[1] == "MD")
            {
                deliveryValue += decimal.Parse(response[2]) * _dividerCoins;
                AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion ProcessUN", "OK", String.Concat(response[1].ToString() + response[2].ToString()), null);

                callbackValueOut?.Invoke(Tuple.Create(decimal.Parse(response[2]) * _dividerCoins, response[1]));
            }
            else
            {
                if (response[1] == "AP")
                {
                    enterValue += decimal.Parse(response[2]) * _dividerBills;
                    AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion ProcessUN", "OK", String.Concat(response[1].ToString() + response[2].ToString()), null);

                    callbackValueIn?.Invoke(Tuple.Create(decimal.Parse(response[2]) * _dividerBills, response[1]));
                }
                else if (response[1] == "MA")
                {
                    enterValue += decimal.Parse(response[2]);
                    AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion ProcessUN", "OK", String.Concat(response[1].ToString() + response[2].ToString()), null);

                    callbackValueIn?.Invoke(Tuple.Create(decimal.Parse(response[2]), response[1]));
                }
                ValidateEnterValue();
            }


        }

        public void ClearValues()
        {
            deliveryValue = 0;
            enterValue = 0;
            deliveryVal = 0;

            this.callbackTotalIn = null;
            this.callbackTotalOut = null;
            this.callbackValueIn = null;
            this.callbackValueOut = null;
            this.callbackOut = null;
        }

        /// <summary>
        /// Respuesta para el caso de total cuando responde el billetero/monedero dispenser
        /// </summary>
        /// <param name="response">respuesta</param>
        private void ProcessTO(string[] response)
        {
            AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ProcessTO", "OK", response.ToString(), null);

            string responseFull;
            if (response[1] == "OK")
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ProcessTO if", "OK", String.Concat(response[1].ToString() + response[2].ToString()), null);


                responseFull = string.Concat(response[2], ":", response[3]);
                if (response[2] == "DP")
                {
                    ConfigDataDispenser(responseFull, 1);
                }

                if (response[2] == "MD")
                {
                    ConfigDataDispenser(responseFull);
                }

                AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion ProcessTO if", "OK", String.Concat(response[1].ToString() + response[2].ToString()), null);

            }
            else
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ProcessTO else", "OK", String.Concat(response[1].ToString() + response[2].ToString()), null);


                responseFull = string.Concat(response[2], ":", response[3]);
                if (response[2] == "DP")
                {
                    ConfigDataDispenser(responseFull, 2);
                }

                AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion ProcessTO else", "OK", response.ToString(), null);

            }

            AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion ProcessTO", "OK", String.Concat(response[1].ToString() + response[2].ToString()), null);

        }
        #endregion

        #region Dispenser
        /// <summary>
        /// Inicia el proceso paara el billetero dispenser
        /// </summary>
        /// <param name="valueDispenser">valor a dispensar</param>
        public void StartDispenser(decimal valueDispenser)
        {
            try
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion StartDispenser", "OK", valueDispenser.ToString(), null);


                stateError = false;
                dispenserValue = valueDispenser;
            //    ActivateTimer();
                ValidateValueDispenser();

                AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion StartDispenser", "OK", valueDispenser.ToString(), null);

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "Error Catch la ejecucion  StartDispenser", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);

                callbackError?.Invoke(Tuple.Create("DP", "Error (StartDispenser), ha ocurrido una exepcion " + ex));
            }
        }

        private void ValidateValueDispenser()
        {
            try
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ValidateValuesDispenser", "OK", "", null);


                if (dispenserValue > 0 && denominationsDispenser.Count > 0)
                {
                    AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ValidateValuesDispenser if", "OK", String.Concat(dispenserValue.ToString() + denominationsDispenser.Count), null);


                    decimal amountCoins = dispenserValue;

                    decimal amountBills = 0;

                    foreach (var denomination in denominationsDispenser)
                    {
                        if (denomination.Item1.Equals("DP"))
                        {
                            var amount = ((int)(amountCoins / denomination.Item2) * denomination.Item2);

                            amountBills += amount;
                            amountCoins -= amount;
                        }
                    }

                    if (amountBills > 0 && amountCoins > 0)
                    {
                        typeDispend = 2;
                    }
                    else
                    {
                        typeDispend = 1;
                    }

                    //if (amountBills > 0)
                    //{
                    //DispenserMoney(((int)(amountBills / _dividerBills)).ToString());
                    AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion ValidateValuesDispenser if", "OK", String.Concat(dispenserValue.ToString() + denominationsDispenser.Count), null);

                    DispenserMoney(dispenserValue.ToString());
                    //}

                    //if (amountCoins > 0)
                    //{
                    //    SendMessageCoins(_DispenserCoinOn + ((int)(amountCoins / _dividerCoins)).ToString());
                    //}
                }

                AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion ValidateValuesDispenser", "OK", "", null);

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "Error Catch la ejecucion  ValidateValueDispenser", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                callbackError?.Invoke(Tuple.Create("DP", "Error (ValidateValueDispenser), ha ocurrido una exepcion " + ex));
            }
        }

        /// <summary>
        /// Configura el valor a dispensar para distribuirlo entre monedero y billetero
        /// </summary>
        private void ConfigurateDispenser(string values)
        {
            try
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ConfigurateDispenser", "OK", values, null);
    

                if (!string.IsNullOrEmpty(values))
                {
                    denominationsDispenser = new List<Tuple<string, int>>();
                    var denominations = values.Split('-');

                    if (denominations.Length > 0)
                    {
                        foreach (var denomination in denominations)
                        {
                            var value = denomination.Split(':');
                            if (value.Length == 2)
                            {
                                denominationsDispenser.Add(Tuple.Create(value[0], int.Parse(value[1])));
                            }
                        }
                    }
                }

                AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion ConfigurateDispenser", "OK", values, null);

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "Error Catch la ejecucion  ConfigurateDispenser", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                callbackError?.Invoke(Tuple.Create("DP", "Error (ConfigurateDispenser), ha ocurrido una exepcion " + ex));
            }
        }

        /// <summary>
        /// Enviar la orden de dispensar al billetero
        /// </summary>
        /// <param name="valuePay"></param>
        private void DispenserMoney(string valuePay)
        {
            try
            {

                AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion DispenserMoney", "OK", "", null);

                if (!string.IsNullOrEmpty(TOKEN))
                {
                    string message = string.Format("{0}:{1}:{2}", _DispenserBillOn, TOKEN, valuePay);
                    SendMessageBills(message);
                }

                AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion DispenserMoney", "OK", "", null);

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "Error Catch la ejecucion DispenserMoney", "OK", String.Concat(ex.Message + ex.StackTrace), null);

                callbackError?.Invoke(Tuple.Create("DP", "Error (DispenserMoney), ha ocurrido una exepcion " + ex));
            }
        }
        #endregion

        #region Aceptance
        /// <summary>
        /// Inicia la operación de billetero aceptance
        /// </summary>
        /// <param name="payValue">valor a pagar</param>
        public void StartAceptance(decimal payValue)
        {
            try
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion StartAcceptance", "OK", "", null);


                ListenerAceptance();
                this.payValue = payValue;
                AcceptorControl.InitAcceptance();
                SendMessageBills(_AceptanceBillOn);

                AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion StartAcceptance", "OK", "", null);

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "Error Catch la ejecucion StartAceptance", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                callbackError?.Invoke(Tuple.Create("AP", "Error (StartAceptance), ha ocurrido una exepcion " + ex));
            }
        }

        /// <summary>
        /// Valida el valor que ingresa
        /// </summary>
        private void ValidateEnterValue()
        {

            AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ValidateEnterValue", "OK", "", null);

            decimal enterVal = enterValue;
            if (enterValue >= payValue)
            {
                //StopAceptance();
                enterValue = 0;
                AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion ValidateEnterValue", "OK", enterVal.ToString(), null);
                callbackTotalIn?.Invoke(enterVal);
            }
        }

        private void ListenerAceptance()
        {
            try
            {
                AcceptorControl.callbackBillAccepted = Value =>
                {
                    var responseAP = Convert.ToDecimal(Value);

                    AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ValidateEnterValue", "OK", Value.ToString(), null);


                    //   CallBackSaveRequestResponse?.Invoke("Respuesta del billetero AP", responseAP.ToString(), 1);

                    if (responseAP > 0)
                    {
                        enterValue += responseAP;//* this._dividerBills;

                        AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ValidateEnterValue", "OK", responseAP.ToString(), null);

                        //callbackValueIn?.Invoke(Tuple.Create(responseAP * _dividerBills, "AP"));
                        callbackValueIn?.Invoke(Tuple.Create(responseAP, "AP"));
                    }

                    ValidateEnterValue();
                };

                AcceptorControl.callbackMeiMessages = EMessage =>
                {
                    var responseAPMS = AcceptorControl.MeiMessagesHomologate[EMessage.ToString()] + Environment.NewLine;

                    AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ValidateEnterValue", "OK", responseAPMS.ToString(), null);


                    //    CallBackSaveRequestResponse?.Invoke("Respuesta del billetero AP-MS", responseAPMS.ToString(), 1);

                    if (responseAPMS.Contains("Aceptador Conectado"))
                    {
                        if (token)
                        {
                            token = false;
                            AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ValidateEnterValue", "OK", token.ToString(), null);

                            callbackToken?.Invoke(true);
                        }
                    }
                    else
                    {

                        callbackError?.Invoke(Tuple.Create("", responseAPMS));
                    }
                };
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "Error Catch la ejecucion ListenerAceptance", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                callbackError?.Invoke(Tuple.Create("", string.Concat("Error (ListenerAceptance) ", ex)));

            }
        }

        /// <summary>
        /// Para la aceptación de dinero
        /// </summary>
        public void StopAceptance()
        {
            AcceptorControl.StopAcceptance();
            SendMessageBills(_AceptanceBillOFF);
        }
        #endregion

        #region Responses
        /// <summary>
        /// Procesa la respuesta de los dispenser M y B
        /// </summary>
        /// <param name="data">respuesta</param>
        /// <param name="isRj">si se fue o no al reject</param>
        private void ConfigDataDispenser(string data, int isBX = 0)
        {
            try
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ConfigDataDispenser", "OK", String.Concat(data + isBX), null);


                string[] values = data.Split(':')[1].Split(';');
                if (isBX < 2)
                {
                    foreach (var value in values)
                    {
                        int denominacion = int.Parse(value.Split('-')[0]);
                        int cantidad = int.Parse(value.Split('-')[1]);
                        deliveryVal += denominacion * cantidad;
                    }

                    AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion ConfigDataDispense if(isBx<2)", "OK", values.ToString(), null);

                }

                if (isBX == 0 || isBX == 2)
                {
                    callbackLog?.Invoke(string.Concat(data.Replace("\r", string.Empty), "!"));
                    typeDispend--;
                }

                if (!stateError)
                {

                    AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ConfigDataDispenser if(!stateError)", "OK", stateError.ToString(), null);


                    if (dispenserValue == deliveryVal)
                    {
                        if (typeDispend == 0)
                        {
                    //        timer.CallBackClose = null;
                   //         timer.CallBackStop?.Invoke(1);
                            callbackTotalOut?.Invoke(deliveryVal);
                        }
                    }
                }
                else
                {
                    AdminPayPlus.SaveLog("ControlPeripherals", "entrando a la ejecucion ConfigDataDispenser e;se (!stateError)", "OK", stateError.ToString(), null);


                    if (typeDispend == 0)
                    {
                  //      timer.CallBackClose = null;
                   //     timer.CallBackStop?.Invoke(1);
                        callbackOut?.Invoke(deliveryVal);
                    }
                }

                AdminPayPlus.SaveLog("ControlPeripherals", "Saliendo de la ejecucion ConfigDataDispenser", "OK", String.Concat(data + isBX), null);

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ControlPeripherals", "Error Catch la ejecucion  ConfigDataDispenser", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);

                callbackError?.Invoke(Tuple.Create("AP", "Error (ConfigDataDispenser), ha ocurrido una exepcion " + ex));
            }
        }
        #endregion

        #region "TimerInactividad"
        private void ActivateTimer()
        {
            try
            {
                string timerInactividad = Utilities.GetConfiguration("TimerInactividad");
                timer = new TimerGeneric(timerInactividad);
                timer.CallBackClose = response =>
                {
                    try
                    {
                        timer.CallBackClose = null;
                        callbackOut?.Invoke(deliveryVal);
                    }
                    catch (Exception ex)
                    {
                        callbackError?.Invoke(Tuple.Create("ActivateTimer", ex.ToString()));
                    }
                };
            }
            catch (Exception ex)
            {
                callbackError?.Invoke(Tuple.Create("DP", "Error (ActivateTimer), ha ocurrido una exepcion en ActivateTimer " + ex));
            }
        }
        #endregion
    }
}
