using System.Diagnostics;
using System.IO.Ports;

namespace WPFApostar.Domain.Peripherals.Datafono;



    public class TEFTransactionManager
    {
        private bool _continue;

        private IniFile confFile = new IniFile("C:\\TEFII_NET\\resource\\serialParam.ini");

        private string response = "No se envió la instrucción";

        public string getTEFAuthorization(string message)
        {
            string param = confFile.getParam("SerialPortLongTimeout");
            string portName = confFile.getParam("SerialPortName").ToUpper();
            SerialPort _serialPort = new SerialPort
            {
                PortName = portName,
                ReadTimeout = 500,
                WriteTimeout = 500
            };
            if (Array.FindAll(SerialPort.GetPortNames(), (string s) => s.Equals(_serialPort.PortName)).Count() == 0)
            {
                return "No se encuentra el dispositivo en el puerto " + _serialPort.PortName;
            }

            _serialPort.Open();
            _continue = true;
            _serialPort.Write(message);
            response = "";
            string text = "";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (_continue)
            {
                string text2 = _serialPort.ReadExisting();
                if (text2.Length > 0)
                {
                    if (text2.Equals("06"))
                    {
                        text = "06";
                    }
                    else
                    {
                        response = text + text2;
                        _serialPort.Write("06");
                        text = "";
                        _continue = false;
                    }
                }

                if (stopwatch.ElapsedMilliseconds >= long.Parse(param))
                {
                    response = "DATAFONO NO RESPONDE";
                    stopwatch.Stop();
                    break;
                }
            }

            _serialPort.Close();
            return response;
        }

        public string getTEFAuthorization()
        {
            string portName = confFile.getParam("SerialPortName").ToUpper();
            string s2 = confFile.getParam("SerialPortLongTimeout").ToUpper();
            SerialPort _serialPort = new SerialPort
            {
                PortName = portName,
                ReadTimeout = 500,
                WriteTimeout = 500
            };
            if (Array.FindAll(SerialPort.GetPortNames(), (string s) => s.Equals(_serialPort.PortName)).Count() == 0)
            {
                return "No se encuentra el dispositivo en el puerto " + _serialPort.PortName;
            }

            _serialPort.Open();
            _continue = true;
            response = "";
            string text = "";
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (_continue)
            {
                string text2 = _serialPort.ReadExisting();
                if (text2.Length > 0)
                {
                    if (text2.Equals("06"))
                    {
                        text = "06";
                    }
                    else
                    {
                        response = text + text2;
                        _serialPort.Write("06");
                        text = "";
                        _continue = false;
                    }
                }

                if (stopwatch.ElapsedMilliseconds >= long.Parse(s2))
                {
                    response = "DATAFONO NO RESPONDE";
                    stopwatch.Stop();
                    break;
                }
            }

            _serialPort.Close();
            return response;
        }
    }

