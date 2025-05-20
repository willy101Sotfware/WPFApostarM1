﻿using System.Windows;
using WPFApostar.Classes;


namespace HantleDispenserAPI
{

    public static class Dispenser
    {

        private static CDMS_Handler _handler { get; set; }
        private static int[] _quantities { get; set; }
        private static int _valueToDispense { get; set; } = 0;
        private static bool _isMaxRejectErr { get; set; } = false;

        public static int DispensedValue { get; set; } = 0;
        public static int CoinsValue { get; set; } = 0;
        public static Dictionary<int, int> RejectData { get; set; } = new Dictionary<int, int>();
        public static Dictionary<int, int> DispensedData { get; set; } = new Dictionary<int, int>();

        public static bool MustReinitialize { get; set; } = false;

        private const string DISP_OK = "OK";
        private const string DISP_CONTINUE = "CONTINUE";
        private const string DISP_EMPTY = "EMPTY";
        private const string DISP_JAMMED = "JAMMED";
        private const string DISP_MAXREJECT = "MAXREJECT";
        private const string DISP_UNKNOWN = "UNKNOWN";
        private static bool IsConnected()
        {
            try
            {
                _handler = new CDMS_Handler(Utilities.GetConfiguration("dispenserPort"), Utilities.GetConfiguration("dispenserDenominations"));
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("PeripheralController " + "IsConnected " + "El puerto o las denominaciones en App.config no están en el formato correcto " + ex);
                return false;
            }

            _handler.Disconnect();
            if (_handler.Connect())
            {
                return true;
            }
            return false;
        }

        public static async Task<bool> Start()
        {
            bool res = false;
            await Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {

                AdminPayPlus.SaveLog("PeripheralController " + "Start " + $"Inicializando dispensador...");
                if (!IsConnected())
                {
                    AdminPayPlus.SaveLog("PeripheralController " + "Start " + $"Error al connectarse al puerto del dispensador {Utilities.GetConfiguration("Port")}");
                    res = false;
                    return;
                }

                CleanVariable();

                //Verificar que el numero de denominaciones del App.config corresponda al número de baules conectados
                var sensor = _handler.GetSensor();
                if (sensor == null || sensor.ErrorCode == ErrorCDMS.RuntimeError)
                {
                    AdminPayPlus.SaveLog("PeripheralController " + "Start " + $"Error al consultar los sensores del dispensador" + sensor.ToString());

                    res = false;
                    return;
                }

                for (int i = 0; i<_handler.cassetesValues.Count; i++)
                {
                    if (!sensor.SensorInfo.CassetteConnected[i])
                    {
                        AdminPayPlus.SaveLog("PeripheralController " + "Start " + $"El número de baules conectados es menor a los especificados en el App.config" + sensor.ToString());

                        res = false;
                        return;
                    }
                }


                var handlerResponse = _handler.Initialize();

                if (!handlerResponse.isSuccess)
                {
                    AdminPayPlus.SaveLog("PeripheralController " + "Start " + $"Error, dispensador inicia incorrectamente " + handlerResponse);
                    res = false;
                    return;
                }
                AdminPayPlus.SaveLog("PeripheralController " + "Start " + $"Dispensador inicia correctamente " + handlerResponse);
                MustReinitialize = false;
                res = true;
            });
            return res;
        }

        public static void CleanVariable()
        {
            IsConnected();
            _valueToDispense = 0;
            _isMaxRejectErr = false;
            DispensedValue = 0;
            DispensedData = new Dictionary<int, int>();
            RejectData =   new Dictionary<int, int>();
            foreach (var denom in _handler.cassetesValues)
            {
                DispensedData.Add(denom, 0);
                RejectData.Add(denom, 0);
            }

        }

        public static async Task DispenseAmount(int dispendAmount)
        {
            if (dispendAmount < _handler.cassetesValues.Min())
            {
                CoinsValue = dispendAmount;
                return;
            }

            _valueToDispense = dispendAmount;
            int topDenomQuantity = dispendAmount / _handler.cassetesValues[0];
            int topDenmonPacketAmount = _handler.cassetesValues[0] * 20;
            await Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                if (topDenomQuantity <= 20)
                {
                    GoDispend(dispendAmount);
                    return;
                }


                //Algoritmo para enviar dispensacion por paquetes de máximo 20 billetes
                int numOfDispends = dispendAmount / topDenmonPacketAmount;
                AdminPayPlus.SaveLog("PeripheralController " + "DispenseAmount " + $"Valor a dispensar: {dispendAmount}. Se enviaran {numOfDispends} dispensaciones por valor de {topDenmonPacketAmount} c/u");

                for (int i = 0; i < numOfDispends; i++)
                {
                    var res = GoDispend(topDenmonPacketAmount);
                    AdminPayPlus.SaveLog("PeripheralController " + "DispenseAmount " +  $"Paquete {i+1}/{numOfDispends} terminado. Respuesta: {res}");

                    if (res == DISP_EMPTY || res == DISP_JAMMED || res == DISP_MAXREJECT) return;
                }


                if (CoinsValue < _handler.cassetesValues.Min())
                    return;

                AdminPayPlus.SaveLog("PeripheralController " + "DispenseAmount " +  $"Última dispensación con valor de {CoinsValue}");

                GoDispend(CoinsValue);

            });
        }

        /* Cuidado, cuando escribi esta función solo había dos personas que sabían como funcionaba, Dios y yo. Ahora solo sabe Dios
         * Esta función es recursiva
         */
        private static string GoDispend(int dispendValue, List<int> cassetteToIgnore = null)
        {
            if (cassetteToIgnore == null) cassetteToIgnore = new List<int>();

            _quantities = CalcReturn(dispendValue, cassetteToIgnore);


            AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " + $"Enviando dispensación... Cantidades" + _quantities);

            var response = _handler.Dispense(_quantities);

            AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " + $"Respuesta dispensación" + response);
            if (response.ErrorCode == ErrorCDMS.RuntimeError)
            {
                CoinsValue = _valueToDispense - DispensedValue;
                return DISP_CONTINUE;
            }

            List<CDMS_DenomInfo> denomsWithMissing = new List<CDMS_DenomInfo>();
            int MissingValue = 0;

            //variables jam info
            int rejectQuantityJam = 0;

            foreach (var denomInfo in response.DispenseData)
            {
                DispensedValue += denomInfo.Denomination * (denomInfo.DispensedQuantity);

                DispensedData[denomInfo.Denomination] += denomInfo.DispensedQuantity;
                RejectData[denomInfo.Denomination] += denomInfo.OutOfCassetteQuantity - denomInfo.DispensedQuantity;


                if ((denomInfo.RequestedQuantity - denomInfo.DispensedQuantity) != 0)
                {
                    denomsWithMissing.Add(denomInfo);

                    //info in case of Jam
                    if (denomInfo.OutOfCassetteQuantity - denomInfo.DispensedQuantity != 0)
                    {
                        rejectQuantityJam += denomInfo.OutOfCassetteQuantity - denomInfo.DispensedQuantity - denomInfo.RejectQuantity;
                    }

                }
                MissingValue += denomInfo.Denomination * (denomInfo.RequestedQuantity - denomInfo.DispensedQuantity);

            }

            if (response.isSuccess && MissingValue == 0)
            {
                CoinsValue = _valueToDispense - DispensedValue;
                AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " + $"Devuelta correcta, valor a devolver en monedas: {CoinsValue}");
                return DISP_OK;
            }


            //Si se confirma que hay un baúl vacio
            if (response.ErrorDescription.EndsWith("JamOrEmpty") && !IsJammed())
            {
                AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " + $"Un baúl se quedó sin billetes {response.ErrorDescription}");
                var indexCassetteEmpty = ((int)response.ErrorCode) - 61;
                if (indexCassetteEmpty >= (_handler.cassetesValues.Count() - 1))
                {
                    CoinsValue = _valueToDispense - DispensedValue;
                    AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " + $"El último baúl se quedó sin billetes, valor a devolver en monedas: {CoinsValue}");
                    return DISP_EMPTY;
                }
                cassetteToIgnore.Add(indexCassetteEmpty);


                var res = GoDispend(MissingValue, cassetteToIgnore);
                CoinsValue = _valueToDispense - DispensedValue;
                return res;

            }

            //Si se presenta un atasco
            if (response.ErrorDescription.EndsWith("Sensor") ||
                response.ErrorDescription.EndsWith("Jam") ||
                response.ErrorDescription.EndsWith("JamOrEmpty") ||
                response.ErrorDescription =="26") // Este error no está en la documentación, perdón
            {
                AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " + $"Se detectó un atasco {response.ErrorDescription}");

                //Si el atasco es en el sensor de salida se debe comprobar que el billete no haya salido del dispensador antes de ejectar
                if (response.ErrorCode == ErrorCDMS.ExitSensorJam || response.ErrorCode == ErrorCDMS.Exit1PathSensor)
                {
                    Thread.Sleep(200); // Se espera un tiempo debido al movimiento y la inercia del billete
                    var resSensor = _handler.GetSensor();
                    AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " +  $"Analizando atasco de sensor Exit..." + resSensor.ToString());


                    var quantityWentReject = 0;

                    var resEject = _handler.Eject();
                    AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " +  $"Se realizó eject" + resEject.ToString());

                    if (resEject == null || resEject.EjectInfo == null)
                    {
                        MissingValue -= denomsWithMissing[0].Denomination * 1;
                        DispensedValue += denomsWithMissing[0].Denomination * 1;
                        DispensedData[denomsWithMissing[0].Denomination] += 1;
                        RejectData[denomsWithMissing[0].Denomination] -= 1;

                        CoinsValue = _valueToDispense - DispensedValue;
                        AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " + $"No se pudo desatascar, llamar a Julio. Valor a devolver en monedas: {CoinsValue}. Estado de sensores del dispensador: ");
                        return DISP_JAMMED;
                    }

                    quantityWentReject += resEject.EjectInfo.RejectCount;

                    //Se va a asumir siempre que por lo menos un billete salió
                    try
                    {

                        var dif = (rejectQuantityJam - quantityWentReject);
                        dif = dif <= 0 ? 1 : dif; // Papi que está viendo acá? esta linea no la intente argumentar, tiene demasiado trasfondo, para mi tampoco tiene sentido. Siga leyendo
                        AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " +  $"Cantidad faltante: {MissingValue},rejectQuantityJam: {rejectQuantityJam}, quantityWentReject: {quantityWentReject}, dif: {dif}");
                        AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " +  $"Se detectó no hay billete atascado en el sensor de exit. Se asume que esté salió. se tomará que un billete de {denomsWithMissing[0].Denomination}  adicional salió");

                        MissingValue -= denomsWithMissing[0].Denomination * dif;
                        DispensedValue += denomsWithMissing[0].Denomination * dif;
                        AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " + $"Cantidad faltante recalculada {MissingValue}");
                        DispensedData[denomsWithMissing[0].Denomination] += dif;
                        RejectData[denomsWithMissing[0].Denomination] -= dif;
                    }
                    catch (Exception ex)
                    {
                        AdminPayPlus.SaveLog("PeripheralController " + "GoDispend catch" + $"Error fatal trantando de calcular la salida de billetes luego de un atasco en el sensor Exit" + ex.Message);

                    }

                    /*
                    //if (rejectQuantityJam != quantityWentReject) // Si la cantidad que fue a reject no es la misma faltante se recalcula
                    //{
                    //    try
                    //    {
                    //        var dif = rejectQuantityJam - quantityWentReject;
                    //        EventLogger.SaveLog(EventType.Warning, $"Se detectó no hay billete atascado en el sensor de exit. Se asume que esté salió. se tomará que un billete de {denomsWithMissing[0].Denomination}  adicional salió");
                    //        MissingValue -= denomsWithMissing[0].Denomination * dif;
                    //        DispensedValue += denomsWithMissing[0].Denomination * dif;
                    //        EventLogger.SaveLog(EventType.Warning, $"Cantidad faltante recalculada {MissingValue}");
                    //        DispensedData[denomsWithMissing[0].Denomination] += dif;
                    //        RejectData[denomsWithMissing[0].Denomination] -= dif; 
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        EventLogger.SaveLog(EventType.P_Dispenser, $"Error fatal trantando de calcular la salida de billetes luego de un atasco en el sensor Exit", ex);
                    //    } 
                    //}
                    */
                }


                if (!TryEject()) //Intenta Ejectar 3 veces
                {

                    var sensors = _handler.GetSensor();
                    CoinsValue = _valueToDispense - DispensedValue;
                    AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " +  $"No se pudo desatascar, llamar a Julio. Valor a devolver en monedas: {CoinsValue}. Estado de sensores del dispensador: " + sensors.ToString());
                    MustReinitialize = true;
                    return DISP_JAMMED;
                }



                var res = GoDispend(MissingValue, cassetteToIgnore);
                CoinsValue = _valueToDispense - DispensedValue;
                return res;
            };

            if (response.ErrorDescription.Contains("RejectMax"))
            {
                AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " +  $"Se detectó cantidad máxima de rechazo {response.ErrorDescription}");

                if (!_isMaxRejectErr)
                {
                    _isMaxRejectErr = true;
                    _handler.Eject();
                    var res = GoDispend(MissingValue, cassetteToIgnore);
                    CoinsValue = _valueToDispense - DispensedValue;
                    return res;
                }

                CoinsValue = _valueToDispense - DispensedValue;
                AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " +  $"Muchos billetes se han ido al reject no se intentará dispensar de nuevo, llamar a Julio. Valor a devolver en monedas: {CoinsValue}");
                MustReinitialize = true;
                return DISP_MAXREJECT;

            }


            CoinsValue = _valueToDispense - DispensedValue;
            AdminPayPlus.SaveLog("PeripheralController " + "GoDispend " +  $"No se logró determinar el estado del dispensador, ocurrió un error no determinado, valor a devolver en monedas: {CoinsValue}" + response.ToString());
            MustReinitialize = true;
            return DISP_UNKNOWN;
        }

        private static int[] CalcReturn(int valueToDispend, List<int> _cassetteToIgnore)
        {

            int[] _quantities = new int[_handler.cassetesValues.Count];
            for (int i = 0; i < _handler.cassetesValues.Count; i++)
            {
                var denominacion = _handler.cassetesValues[i];
                // Si la denominación es -1 es por que se debe ignorar
                if ((valueToDispend < denominacion) || (_cassetteToIgnore.Contains(i)))
                {
                    _quantities[i] = 0;
                    continue;
                }
                _quantities[i] = (int)(valueToDispend / denominacion);
                valueToDispend -= (_quantities[i] * denominacion);
            }

            return _quantities;
        }
        private static bool TryEject()
        {
            bool ejectIsSuccess = false;
            AdminPayPlus.SaveLog("PeripheralController " + "TryEject " + $"Se va a intentar ejectar");
            int tries = 3;
            do
            {
                Thread.Sleep(800);
                var ejectRes = _handler.Eject();
                AdminPayPlus.SaveLog("PeripheralController " + "TryEject " + $"Intento de Ejección {(3 - tries) + 1}");
                if (ejectRes == null || ejectRes.EjectInfo == null)
                {
                    ejectIsSuccess = false;
                    tries--;
                    continue;
                }
                ejectIsSuccess = ejectRes.isSuccess && !IsJammed();
                tries--;
            }
            while (!ejectIsSuccess && tries >= 0);

            return ejectIsSuccess;
        }
        private static bool IsJammed()
        {
            var sensors = _handler.GetSensor();

            if (sensors == null || sensors.ErrorCode == ErrorCDMS.RuntimeError) return false;
            if (sensors.SensorInfo == null) return false;

            bool skewJam = false;
            for (int i = 0; i<8; i++)
            {
                skewJam = skewJam || sensors.SensorInfo.CassetteSkew1[i];
                skewJam = skewJam || sensors.SensorInfo.CassetteSkew2[i];
            }

            if (skewJam) return true;

            return (
                sensors.SensorInfo.ScanStart ||
                sensors.SensorInfo.Gate1 || sensors.SensorInfo.Gate2 ||
                sensors.SensorInfo.Exit ||
                sensors.SensorInfo.RejectIn
            );


        }

        //    public static string GetLoadMessage()
        //    {
        //        if (!IsConnected())
        //        {
        //            return DispenserMessages.COM_NOT_AVAILABLE;
        //        }
        //        CDMS_Response sensorResponse = null;
        //        Application.Current.Dispatcher.Invoke(() =>
        //        {
        //            sensorResponse = _handler.GetSensor();
        //        });

        //        if (sensorResponse == null ||
        //            sensorResponse.ErrorCode == ErrorCDMS.RuntimeError ||
        //            sensorResponse.SensorInfo == null)
        //        {
        //            return DispenserMessages.PHYSICAL_CONN_LOST;
        //        }

        //        if (sensorResponse.SensorInfo.RejectBoxOpen) return DispenserMessages.REJECT_BOX_OPEN;

        //        for (int i = 0; i < _handler.cassetesValues.Count; i++)
        //        {
        //            if (!sensorResponse.SensorInfo.CassetteConnected[i]) return string.Format(DispenserMessages.CASSETTE_DISCONNECTED, i + 1);
        //            if (sensorResponse.SensorInfo.CassetteDismounted[i]) return string.Format(DispenserMessages.CASSETTE_DISMOUNTED, i + 1);
        //            if (sensorResponse.SensorInfo.CassetteSkew1[i] ||
        //                sensorResponse.SensorInfo.CassetteSkew2[i])
        //                return string.Format(DispenserMessages.CASSETTE_BAD_LOAD, i + 1);
        //        }

        //        if (sensorResponse.SensorInfo.CisOpen) return DispenserMessages.CIS_OPEN;

        //        if (sensorResponse.SensorInfo.ScanStart ||
        //            sensorResponse.SensorInfo.Gate1 || sensorResponse.SensorInfo.Gate2 ||
        //            sensorResponse.SensorInfo.Exit ||
        //            sensorResponse.SensorInfo.RejectIn)
        //            return DispenserMessages.JAM;

        //        return string.Empty;
        //    }

             }

   }

        public class ErrorPrint
        {
            public string Code { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }

    
