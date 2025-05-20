using Newtonsoft.Json;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using Color = System.Drawing.Color;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPFApostar.Classes.Printer;
using WPFApostar.Models;
using WPFApostar.Resources;
using WPFApostar.Windows.Alerts;
using BarcodeLib.Barcode;

namespace WPFApostar.Classes
{
    public class Utilities
    {
        #region "Referencias"
        public static Navigation navigator { get; set; }

      //  private static SpeechSynthesizer speechSynthesizer;

        public static UserControl UCSupport;

        private static ModalW modal { get; set; }

        //   private static ModalW modal { get; set; }
        #endregion
        private static PrintService _printService;

        public static PrintService PrintService
        {
            get { return _printService; }
        }

        public static string GetConfigData(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static string GetConfiguration(string key, bool decodeString = false)
        {
            try
            {
                string value = "";
                AppSettingsReader reader = new AppSettingsReader();
                value = reader.GetValue(key, typeof(String)).ToString();
                if (decodeString)
                {
                    value = Encryptor.Decrypt(value);
                }
                return value;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return string.Empty;
            }
        }

        public static string CodeBar(string Factura)
        {
            byte[] base64SingleBytes;
            string str2;
            try
            {
                string str = "";

                int num3 = 0;
                int num4 = 0x3b9ac9ff;
                for (int i = 0; i < 50; i++)
                {
                    Linear barcode = new Linear();
                    barcode.Type = BarcodeType.CODABAR;
                    barcode.Data = string.Concat(Factura);
                    // barcode.Data = string.Concat("(415)" + Code + "(8020)" + string.Format(Factura).PadLeft(10, '0') + "(3900)" + string.Format("{0:0}", ValorFactura).PadLeft(10, '0') + "(96)" + fecha);
                    barcode.UOM = UnitOfMeasure.PIXEL;
                    barcode.BarWidth = 2;
                    barcode.BarHeight = 30;
                    barcode.TextFont = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);
                    base64SingleBytes = barcode.drawBarcodeAsBytes();
                    int length = base64SingleBytes.GetLength(0);
                    if (length > num3) { num3 = length; }
                    if (length < num4) { num4 = length; }
                    if ((num3 != length) && (num4 < num3))
                    {
                        byte[] inArray = base64SingleBytes;
                        str = string.Format("data:image/png;base64," + Convert.ToBase64String(inArray), new object[0]);

                        File.WriteAllBytes(@"C:\Users\Administrator\Desktop\WPFGANA\WPFGANA\Barcode\code.png", inArray);

                        break;
                    }
                }
                str2 = str;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return str2;
        }

        public static bool ShowModal(string message, EModalType type, bool timer = false)
        {
            bool response = false;
            try
            {
                ModalModel model = new ModalModel
                {
                    Tittle = "Estimado Cliente: ",
                    Messaje = message,
                    Timer = timer,
                    TypeModal = type,
                    ImageModal = @"Images/Backgrounds/bg-modal-info.png",
                };

                if (type == EModalType.Error)
                {
                    model.ImageModal = @"Images/Backgrounds/bg-modal-danger.png";
                }
                else if (type == EModalType.Information)
                {
                    model.ImageModal = @"Images/Backgrounds/bg-modal-info.png";
                }
                else if (type == EModalType.NoPaper)
                {
                    model.ImageModal = @"Images/Backgrounds/bg-modal-danger.png";
                }
                else if (type == EModalType.Preload)
                {
                    model.ImageModal = @"Images/Backgrounds/bg-modal-info.png";
                }

                Application.Current.Dispatcher.Invoke(delegate
                {
                    modal = new ModalW(model);
                    modal.ShowDialog();

                    if (modal.DialogResult.HasValue && modal.DialogResult.Value)
                    {
                        response = true;
                    }
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
            GC.Collect();
            return response;
        }



        public static BitmapImage LoadImageFromFile(Uri path)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = path;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bitmap.DecodePixelWidth = 900;
            bitmap.EndInit();
            bitmap.Freeze(); //This is the magic line that releases/unlocks the file.
            return bitmap;
        }

        public static void CloseModal() => Application.Current.Dispatcher.Invoke((Action)delegate
        {
            try
            {
                if (modal != null)
                {
                    modal.Close();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
            }
        });

        public static void RestartApp()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Process pc = new Process();
                    Process pn = new Process();
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = Path.Combine(Directory.GetCurrentDirectory(), GetConfiguration("NAME_APLICATION"));
                    pn.StartInfo = si;
                    pn.Start();
                    pc = Process.GetCurrentProcess();
                    pc.Kill();
                }));
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }

        public static void UpdateApp()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Process pc = new Process();
                    Process pn = new Process();
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = GetConfiguration("APLICATION_UPDATE");
                    pn.StartInfo = si;
                    pn.Start();
                    pc = Process.GetCurrentProcess();
                    pc.Kill();
                }));
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }
         
        public static bool ValidateFestivo(string Fecha)
        {
            try
            {
                {

                    Process proceso = new Process();
                    proceso.StartInfo.FileName = $@"{Utilities.GetConfigData("RutaFestivo")}"; // Cambia esto a la ruta de tu .exe
                    proceso.StartInfo.Arguments = $"{Fecha}"; // Tus argumentos separados por espacio
                    proceso.StartInfo.UseShellExecute = false; // No usar el shell
                    proceso.StartInfo.RedirectStandardOutput = true; // Para poder leer la salida estándar
                    proceso.StartInfo.RedirectStandardError = true; // Opcional: Para poder leer la salida de error
                    proceso.StartInfo.CreateNoWindow = true;

                    // Inicia el proceso y lee la salida
                    proceso.Start();

                    // Leer la salida estándar del proceso
                    string output = proceso.StandardOutput.ReadToEnd();
                    // Opcional: Leer la salida de error del proceso
                    string errorOutput = proceso.StandardError.ReadToEnd();

                    // Espera a que el proceso termine
                    proceso.WaitForExit();

                    if (output.Trim().Equals("True"))
                    {
                        return true;         
                    }

                    // Cierra el proceso
                    proceso.Close();
                }
            }
            catch(Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }

            return false;
            
        }

        //public static void PrintVoucherBetplay(TransactionBetPlay ts)
        //{
        //    try
        //    {
        //        CLSPrint objPrint = new CLSPrint();

        //        objPrint.Cedula = ts.Document;
        //        objPrint.Monto = ts.Amount;

        //        objPrint.ImprimirComprobante();
        //    }
        //    catch (Exception ex)
        //    {
        //        Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
        //        PrintVoucherBetPlay(ts);
        //    }
        //}


        //public static void PrintRecaudoP(Transaction Ts)
        //{
        //    try
        //    {
        //        if (_printService == null)
        //        {
        //            _printService = new PrintService();
        //        }

        //        SolidBrush color = new SolidBrush(Color.Black);
        //        Font fontKey = new Font("Arial", 9, System.Drawing.FontStyle.Bold);
        //        Font fontValue = new Font("Arial", 9, System.Drawing.FontStyle.Regular);
        //        int y = 0;
        //        int sum = 20;
        //        //int sum = 10;
        //        int x = 200;
        //        //int x = 40;
        //        int xKey = 15;
        //        int xMax = 100;
        //        //int xMax = 50;
        //        int ymax = 24;

        //        string formaPago = "Efectivo";


        //        StringFormat sf = new StringFormat();
        //        sf.Alignment = StringAlignment.Center;
        //        sf.LineAlignment = StringAlignment.Center;

        //        Rectangle rect = new Rectangle(0, y += sum - 15, 100, 20);

        //        //int multiplier = (xMax / 45);


        //        rect = new Rectangle(0, y += ymax, 270, 15);
        //        //rect = new Rectangle(0, y = y, 270, 15);

        //        var data = new List<DataPrinter>()
        //            {

        //                new DataPrinter { brush = color, font = fontKey, value = "Apostar S.A.", x = 90, y = y },
        //                new DataPrinter { brush = color, font = fontKey, value = "NIT: 800001520", x = 90, y = y+=20 },
        //                new DataPrinter { brush = color, font = fontKey, value = "CL 17 NO.6 - 42 PEREIRA-RISARALDA", x = 40, y = y+=20 },


        //                new DataPrinter { brush = color, font = fontValue, value = "Fecha Recaudo: " + DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty, x = 40, y = y+=20},
        //                new DataPrinter { brush = color, font = fontValue, value = "EQ: 1386" + "OFI: 1" + "Vendedor: 1088025" , x = 40, y = y+=20},

        //                new DataPrinter { brush = color, font = fontKey, value = "Tr: " + Ts.IdTransactionAPi, x = 40, y = y += 20},
        //                new DataPrinter { brush = color, font = fontKey, value = "Cod Seg: " + Ts.CodigoSeguridad, x = 167 ,y = y = y},


        //        //    new DataPrinter { brush = color, font = fontValue, value = "-------------------------------------------------------------------", x = 2, y = y += 20},

        //                new DataPrinter { brush = color, font = fontKey, value = Ts.Company.Nombre, x = 90, y = y += 24 },

        //                new DataPrinter { brush = color, font = fontKey, value = "Referencia:", x = 40, y = y+=24},
        //                new DataPrinter { brush = color, font = fontValue, value = Ts.reference, x = 167, y = y = y},

        //            //Valor A Pagar


        //              ////Estado de la trasaccion
        //                new DataPrinter { brush = color, font = fontKey, value = "Estado de trasacción", x = 40, y = y += 24},
        //               new DataPrinter { brush = color, font = fontValue, value = "Aprobada",  x = 167, y = y  },


        //                new DataPrinter { brush = color, font = fontKey, value = "Valor Recaudo", x = 40, y = y += 24 },
        //                new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C2}",  String.Concat("$" + Ts.Amount)), x = 167, y = y },

        //               new DataPrinter { brush = color, font = fontKey, value = "Valor ingresado", x = 40, y = y += 24},

        //                new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C2}", String.Concat("$" + Ts.Payment.ValorIngresado)),  x = 167, y = y  },

        //            new DataPrinter { brush = color, font = fontKey, value = "Total Devuelto:", x = 40, y = y += 24},
        //            new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C2}", String.Concat("$" + Ts.Payment.ValorDispensado)), x = 167, y = y },


        //            //Total Imgresado


        //            //new DataPrinter { brush = color, font = fontValue, value = "-------------------------------------------------------------------", x = 2, y = y+=20 },

        //            //    new DataPrinter { brush = color, font = fontValue, value = "Toda Transaccion esta sujeta", x = 55, y = y += 15},
        //            //    new DataPrinter { brush = color, font = fontValue, value = "a verificacion y aprobacion", x = 55, y = y +=15 },




        //                };
        //        ymax = 0;

        //        Utilities.PrintService.Start(data);

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        public static void PrintBetplayP(Transaction Ts)
        {
            try
            {

                string PuntoVenta = Utilities.GetConfiguration("PuntoVenta");
                string Oficina = Utilities.GetConfiguration("PuntoOficina");

                if (_printService == null)
                {
                    _printService = new PrintService();
                }

                PrintService.PrintInLeft("Apostar S.A");
                PrintService.PrintInLeft("NIT: 800001520");
                PrintService.PrintInLeft("CLL 117 NO.6 - 42 PEREIRA");
                PrintService.PrintInLeft("TEL: 3340043"); 
                PrintService.PrintInLeft(" ");
                PrintService.PrintInLeft($"CLI: {Ts.ResponseNotifyBetPlay.clienteid} MUN: {Ts.ResponseNotifyBetPlay.empresa.ciudad.codigo}");
                PrintService.PrintInLeft($"FECHA: {DateTime.Now.ToString("dd-MM-yy")} HORA: {DateTime.Now.ToString("HH:mm:ss")} ");
                PrintService.PrintInLeft($"VEN: {Ts.IdTransactionAPi} PV: {PuntoVenta}");
                PrintService.PrintInLeft($"ID.TRA: {Ts.ResponseNotifyBetPlay.transaccionid} OFI: {Oficina}");    
                PrintService.PrintInLeft($"COD.SEG: {Ts.ResponseNotifyBetPlay.codigoseguridad}");
                PrintService.PrintInLeft(" ");
                PrintService.PrintInLeft("RECAUDO BETPLAY");

                PrintService.PrintInLeft(" ");
                PrintService.PrintInLeft($"Identificacion: {Ts.ResponseNotifyBetPlay.clienteid}");
                PrintService.PrintInLeft($"Monto recargado: ${Ts.ResponseNotifyBetPlay.ValorRecargado}");
                PrintService.PrintInLeft($"Iva: ${Ts.ResponseNotifyBetPlay.Iva}");
                PrintService.PrintInLeft($"Total: ${Ts.ResponseNotifyBetPlay.valorrecaudo}");
                PrintService.CutPrinting();

                AdminPayPlus.SaveLog("utilities", "Saliendo de PrintBetplayP", "OK", Ts.ResponseNotifyBetPlay.ToString(), Ts);
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("utilities", "Error Catch la ejecucion PrintBetplayP", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Ts);
            }
        }

        //public static void PrintVoucherRecaudo()
        //{
        //    try
        //    {

        //        if (_printService == null)
        //        {
        //            _printService = new PrintService();
        //        }




        //        SolidBrush color = new SolidBrush(Color.Black);
        //            Font fontKey = new Font("Arial", 9, System.Drawing.FontStyle.Bold);
        //            Font fontValue = new Font("Arial", 9, System.Drawing.FontStyle.Regular);
        //            int y = 0;
        //            int sum = 20;
        //            //int sum = 10;
        //            int x = 200;
        //            //int x = 40;
        //            int xKey = 15;
        //            int xMax = 100;
        //            //int xMax = 50;
        //            int ymax = 38;

        //            string formaPago = "Efectivo";


        //            StringFormat sf = new StringFormat();
        //            sf.Alignment = StringAlignment.Center;
        //            sf.LineAlignment = StringAlignment.Center;

        //            // Rectangle rect = new Rectangle(0, y += sum - 15, 280, 20);

        //            Rectangle rect = new Rectangle(0, y += sum - 15, 100, 20);

        //            //int multiplier = (xMax / 45);


        //            rect = new Rectangle(0, y += ymax, 270, 15);
        //            //rect = new Rectangle(0, y = y, 270, 15);


        //            var data = new List<DataPrinter>()
        //            {

        //                 new DataPrinter { brush = color, font = fontKey, value = "APOSTAR S.A.", x = 85, y = y  },
        //                 new DataPrinter { brush = color, font = fontKey, value = "NIT 800001520", x = 85, y = y += 20},
        //                new DataPrinter { brush = color, font = fontKey, value = "Fecha", x = 85, y = y+= 30  },
        //                new DataPrinter { brush = color, font = fontKey, value = "Hora", x = 145, y = y  },


        //                new DataPrinter { brush = color, font = fontValue, value = DateTime.Now.Date.ToString("yyyy-MM-dd") ?? string.Empty, x = 75, y = y+=20},
        //                new DataPrinter { brush = color, font = fontValue, value = DateTime.Now.ToString("HH:mm:ss") ?? string.Empty, x = 145, y = y},

        //                new DataPrinter { brush = color, font = fontKey, value = "Transaccion", x = 55, y = y += 20},
        //                new DataPrinter { brush = color, font = fontValue, value = "123548", x = 155, y = y },


        //            new DataPrinter { brush = color, font = fontValue, value = "-------------------------------------------------------------------", x = 2, y = y += 20},

        //            //Numero de Cedula

        //                new DataPrinter { brush = color, font = fontKey, value = "Referencia de Pago:", x = 30, y = y += 24 },
        //                new DataPrinter { brush = color, font = fontValue, value = "526518" ?? string.Empty, x = 155, y = y},
        //                new DataPrinter { brush = color, font = fontKey, value = "Nombre entidad:", x = 30, y = y += 24 },
        //                new DataPrinter { brush = color, font = fontValue, value = "Recaudos Raspa" ?? string.Empty, x = 155, y = y},

        //            //Valor A Pagar
        //                new DataPrinter { brush = color, font = fontKey, value = "Valor Recaudo:", x = 30, y = y += 24 },
        //                new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C2}",  String.Concat("$" + "10000")), x = 155, y = y },

        //              ////Estado de la trasaccion
        //                new DataPrinter { brush = color, font = fontKey, value = "Estado:", x = 30, y = y += 24},

        //                new DataPrinter { brush = color, font = fontValue, value ="Aprobado" ?? string.Empty,  x = 155, y = y  },


        //            //Total Imgresado
        //                new DataPrinter { brush = color, font = fontKey, value = "Valor ingresado", x = 30, y = y += 24},

        //                new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C2}", String.Concat("$" + "10000")),  x = 155, y = y  },

        //            new DataPrinter { brush = color, font = fontKey, value = "Total Devuelto:", x = 30, y = y += 24},
        //            new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C2}", String.Concat("$" + "0")), x = 155, y = y },

        //            new DataPrinter { brush = color, font = fontValue, value = "-------------------------------------------------------------------", x = 2, y = y+=20 },

        //                new DataPrinter { brush = color, font = fontValue, value = "Toda Transaccion esta sujeta", x = 55, y = y += 15},
        //                new DataPrinter { brush = color, font = fontValue, value = "a verificacion y aprobacion", x = 55, y = y +=15 },




        //                };
        //            ymax = 0;

        //            Utilities.PrintService.Start(data);


        //    }
        //    catch (Exception ex)
        //    {
        //        Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PrintVoucher", ex, ex.ToString());
        //    }

        //}

        public static void PrintVoucherRecaudo(Transaction Ts)
        {
            try
            {


                if (_printService == null)
                {
                    _printService = new PrintService();
                }

                PrintService.PrintInCenter("Apostar S.A");
                PrintService.PrintInCenter("NIT: 800001520");
                PrintService.PrintInCenter("CLL 117 NO.6 - 42 PEREIRA - RISARALDA");
                //PrintService.PrintInCenter("TEL: 3340043");
                PrintService.PrintInCenter(" ");
              //  PrintService.PrintInCenter($"CLI: {Ts.ResponseNotifyBetPlay.clienteid} MUN: {Ts.ResponseNotifyBetPlay.empresa.ciudad.codigo} OFI: 1");
                PrintService.PrintInCenter($"Fecha de Recaudo: {DateTime.Now.ToString("dd-MM-yy HH:mm:ss")}");
                PrintService.PrintInCenter($"Eq: {Ts.ResponseNotifyPayment.Jerarquia.Equipoid} Ofi: {Ts.ResponseNotifyPayment.Jerarquia.Oficinaid} Vendedor: 42144173");
                PrintService.PrintInCenter($"Tr: {Ts.ResponseNotifyPayment.Transaccionid} Cod Seg: {Ts.ResponseNotifyPayment.Codigoseguridad}");
                PrintService.PrintInCenter($"Cli: {Ts.reference}");
                PrintService.PrintInCenter(" ");
                PrintService.PrintInCenter($"{Ts.Company.Nombre}");

                PrintService.PrintInLeft(" ");
                PrintService.PrintInLeft($"Referencia: {Ts.reference}");
                PrintService.PrintInLeft($"Servicio: {Ts.Company.Nombre}");
                PrintService.PrintInLeft($"Valor: ${Ts.RealAmount}");
                PrintService.PrintInCenter(" ");
                PrintService.PrintInLeft($"Ajuste al Peso: ${Convert.ToDecimal(Ts.Amount) - Ts.RealAmount}");
                PrintService.PrintInLeft($"Valor Recaudado ${Ts.Amount}");
                PrintService.CutPrinting();





          
            }
            catch (Exception ex)
            {
                   Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PrintVoucher", ex, ex.ToString());
            }

        }

        public static void PrintVoucherBetPlay(Transaction Transaction)
        {
            try
            {

                if (_printService == null)
                {
                    _printService = new PrintService();
                }

                if (Transaction != null)
                {


                    String CodigoSitio = Utilities.GetConfigData("CodSitio");
                    String Usuario = Utilities.GetConfigData("UsuarioID");

                    SolidBrush color = new SolidBrush(Color.Black);
                    Font fontKey = new Font("Arial", 7, System.Drawing.FontStyle.Bold);
                    Font fontK = new Font("Arial", 6, System.Drawing.FontStyle.Bold);
                    Font fontValue = new Font("Arial", 9, System.Drawing.FontStyle.Regular);
                    int y = 0;
                    int sum = 20;
                    //int sum = 10;
                    int x = 200;
                    //int x = 40;
                    int xKey = 15;
                    int xMax = 100;
                    //int xMax = 50;
                    int ymax = 38;

                    string formaPago = "Efectivo";


                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    // Rectangle rect = new Rectangle(0, y += sum - 15, 280, 20);

                //    Rectangle rect = new Rectangle(0, y += sum - 15, 100, 20);

                    //int multiplier = (xMax / 45);


                  //  rect = new Rectangle(0, y += ymax, 270, 15);
                    //rect = new Rectangle(0, y = y, 270, 15);


                    var data = new List<DataPrinter>()
                    {
                        //new DataPrinter{ image = GetConfiguration("ImageBoucher"), x = 10, y = y += 10, direction = sf  },
                        //ok new DataPrinter { image = GetConfiguration("ImageBoucher"), x = 10, y +=15 , direction = sf},

                        new DataPrinter { brush = color, font = fontKey, value = "BetPlay-Apuestas Deportivas", x = 55, y = 0 },
                    //     new DataPrinter { brush = color, font = fontKey, value = "Autoriza COLJUEGOS Contrato C1444", x = 40, y = y+=10 },
                      //   new DataPrinter { brush = color, font = fontK, value = "AV CLL 26 #69091 OF.802A PBX(1)5190395", x = 40, y = y+=10 },

                        new DataPrinter { brush = color, font = fontValue, value = String.Concat("Fecha: ",DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss")) ?? string.Empty, x = 45, y = y+=30},
       //             new DataPrinter { brush = color, font = fontValue, value = "-------------------------------------------------------------------", x = 2, y = y += 20},

                    new DataPrinter { brush = color, font = fontKey, value = "DATOS RECARGA - BETPLAY", x = 45, y = y += 20},       
                    
                    //Numero de Cedula

                        new DataPrinter { brush = color, font = fontKey, value = "Número de Cedula", x = 40, y = y += 30 },
                        new DataPrinter { brush = color, font = fontValue, value = Transaction.Document ?? string.Empty, x = 155, y = y},

                        //new DataPrinter { brush = color, font = fontKey, value = "Número de Cedula", x = xKey, y = y += sum },
                        //new DataPrinter { brush = color, font = fontValue, value = Transaction.Document ?? string.Empty, x = (xMax - Transaction.Document.Length * multiplier), y = y },

                    //Valor A Pagar
                        new DataPrinter { brush = color, font = fontKey, value = "Valor recarga", x = 40, y = y += 30 },
                        new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C0}", Convert.ToDouble(Transaction.Amount)), x = 155, y = y },

                         new DataPrinter { brush = color, font = fontKey, value = "Valor ingresado", x = 40, y = y += 24},

                        new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C0}", Transaction.Payment.ValorIngresado),  x = 155, y = y  },

                    new DataPrinter { brush = color, font = fontKey, value = "Total Devuelto:", x = 40, y = y += 24},
                    new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C0}", Transaction.Payment._valorDispensado), x = 155, y = y },

                       new DataPrinter { brush = color, font = fontKey, value = "REALIZA TU APUESTA EN: BETPLAY.COM.CO", x = 30, y = y+=30 },





                        };
                    ymax = 0;

             //       Utilities.PrintService.Start(data);
                    //AdminPayPlus.PrintService.Start(data);




                }
            }
            catch (Exception ex)
            {
                //   Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PrintVoucher", ex, ex.ToString());
            }

        }



        public static void printvou()
        {
            if (_printService == null)
            {
                _printService = new PrintService();
            }


            string[] array1 = { "NUM", "DIR", "COM", "PAT", "UNA" };
            string[][] array2 = new string[][]
            {
                    new string[] { "4136", "420", "0", "0", "0" },
                    new string[] { "5432", "592", "0", "0", "0" },
                    new string[] { "8702", "674", "0", "0", "0" },
            };

            PrintService.PrintInCenter("Contrato de concesion No.035 de");
            PrintService.PrintInCenter("2018");
            PrintService.PrintInCenter("CLI: MUN: 66001 CHANCE");
            PrintService.PrintInCenter("OF:81 VEN:900113592 EQ:2476");
            PrintService.PrintInCenter("F.V: 15/05/2024 H.V:10:38:19");
            PrintService.PrintInCenter("F.Sorteo:10/05/2024");
            PrintService.PrintInCenter("ID.TRA:6027753431");
            PrintService.PrintInCenter("COD.SEG:567531761");
            PrintService.PrintInCenter("CONSEC:AQO000021945156");
            PrintService.PrintInLeft(" ");
            PrintService.PrintInLeft("LOT: MANI");
            PrintService.PrintInCenter(PrintService.PrepareRow(array1));
            foreach (var row in array2)
            {
                PrintService.PrintInCenter(PrintService.PrepareRow(row));
            }
            PrintService.PrintInLeft(" ");
            PrintService.PrintInRight("Subtotal: $420");
            PrintService.PrintInRight("IVA(19%): $80");
            PrintService.PrintInRight("Total: $500");
            PrintService.CutPrinting();

        }

        public static void PrintVoucherSuperChance(Transaction Transaction)
        {
            try
            {


                if (_printService == null)
                {
                    _printService = new PrintService();
                }

                string LotPay = "";

                foreach(var x in Transaction.ResponseNotifyC.Chance.Listadoapuestas.Apuesta)
                {
                    foreach(var y in x.Listadoloterias.Loteria)
                    {
                        LotPay += y.Nombrecorto + " ";
                    }
                }                    

                string[] array1 = { "NUM", "DIR", "COM", "PAT", "UNA" };

                string[][] array2 = new string[][]
                {

                };

                List<string[]> lista2 = new List<string[]>(array2);

                foreach (var Data in Transaction.ResponseNotifyC.Chance.Listadoapuestas.Apuesta)
                {

                    string[] array3 = { Data.Numeroapostado.ToString(), Data.Valorapostadodirecto.ToString(), Data.Valorapostadocombinado.ToString(), Data.Valorapostadopata.ToString(), Data.Valorapostadouna.ToString() };

                    lista2.Add(array3);
                }

                array2 = lista2.ToArray();




                PrintService.PrintInLeft("Contrato de concesion No.035 del");
                PrintService.PrintInLeft($"2018 Nit: {Transaction.ResponseNotifyC.Empresa.Nit}");
                PrintService.PrintInLeft($"tel:{Transaction.ResponseNotifyC.Empresa.Telefono} VEN:{Transaction.ResponseNotifyC.Codigoasesor}");
                PrintService.PrintInLeft($"CLI: MUN: {Transaction.ResponseNotifyC.Municipio} CHANCE");
                PrintService.PrintInLeft($"OF:{Transaction.ResponseNotifyC.Oficina} P.V:{Transaction.ResponseNotifyC.Jerarquia.Puntoventaid} EQ:{Transaction.ResponseNotifyC.Jerarquia.Equipoid}");
                PrintService.PrintInLeft($"F.V: {DateTime.Now.ToString("dd/MM/yyyy")} H.V:{Transaction.ResponseNotifyC.Hora}");
                PrintService.PrintInLeft($"F.S:{Transaction.ResponseNotifyC.Chance.Fechasorteo}");
                PrintService.PrintInLeft($"ID.TRA:{Transaction.ResponseNotifyC.Transaccionid}");
                PrintService.PrintInLeft($"COD.SEG:{Transaction.ResponseNotifyC.Codigoseguridad}");
                PrintService.PrintInLeft($"CONSEC:{Transaction.ResponseNotifyC.Consecutivoapuesta}");
                PrintService.PrintInLeft(" ");
                PrintService.PrintInLeft($"LOT: {LotPay}");
                PrintService.PrintInLeft(PrintService.PrepareRow(array1));
                foreach (var row in array2)
                {
                    PrintService.PrintInLeft(PrintService.PrepareRow(row));
                }
                PrintService.PrintInLeft(" ");
                PrintService.PrintInLeft($"Subtotal: ${Transaction.ResponseNotifyC.Chance.Listadoapuestas.Apuesta[0].Valorapostadototal}");
                PrintService.PrintInLeft($"IVA(19%): ${Transaction.ResponseNotifyC.Chance.Listadoapuestas.Apuesta[0].Valoriva}");
                PrintService.PrintInLeft($"Total: ${Transaction.ResponseNotifyC.Chance.Listadoapuestas.Apuesta[0].Valortotal}");           
                PrintService.CutPrinting();




            }
            catch (Exception ex)
            {
                //   Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PrintVoucher", ex, ex.ToString());
            }

        }



        public static void PrintVoucherPaquetes(Transaction Transaction)
        {
            try
            {
                if (_printService == null)
                {
                    _printService = new PrintService();
                }

                PrintService.PrintInLeft("Apostar S.A");
                PrintService.PrintInLeft("NIT: 800001520");
                PrintService.PrintInLeft("CLL 117 6-42 PEREIRA-RISARALDA");
                //PrintService.PrintInCenter("TEL: 3340043");
                PrintService.PrintInLeft(" ");
                //  PrintService.PrintInCenter($"CLI: {Ts.ResponseNotifyBetPlay.clienteid} MUN: {Ts.ResponseNotifyBetPlay.empresa.ciudad.codigo} OFI: 1");
                PrintService.PrintInLeft($"DATOS RECARGA - PAQUETES CELULAR");
                PrintService.PrintInLeft(" ");
                PrintService.PrintInLeft($"Fecha Recarga:{DateTime.Now.ToString("dd-MM-yy HH:mm:ss")}");              
                PrintService.PrintInLeft($"Transaccion ID: {Transaction.responseGuardarPaquetes.Transaccionid}");
                PrintService.PrintInLeft($"Operador: {Transaction.Operador}");
                PrintService.PrintInLeft($"No Celular: {Transaction.reference}");
                PrintService.PrintInLeft(" ");
                PrintService.PrintInLeft(" ");
                PrintService.PrintInLeft($"Servicio: Paquetes");
                PrintService.PrintInLeft($"Estado: {Transaction.StatePay}");               
                PrintService.PrintInLeft($"Valor Paquete: ${Transaction.Amount}");              
                PrintService.PrintInLeft($"Valor ingresado: ${Transaction.Payment.ValorIngresado}");
                PrintService.PrintInLeft($"Total Devuelto: ${Transaction.Payment.ValorDispensado}");
                PrintService.CutPrinting();

            
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PrintVoucher", ex, ex.ToString());
            }

        }




        public static decimal RoundValue(decimal Total)
        {
            try
            {

                decimal roundTotal = 0;
                roundTotal = Math.Ceiling(Total / 100) * 100;
                return roundTotal;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
                return Total;
            }
        }

        public static bool ValidateModule(decimal module, decimal amount)
        {
            try
            {
                var result = (amount % module);
                return result == 0 ? true : false;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
                return false;
            }
        }

        public static T ConverJson<T>(string path)
        {
            T response = default(T);
            try
            {
                using (StreamReader file = new StreamReader(path, Encoding.UTF8))
                {
                    try
                    {
                        var json = file.ReadToEnd().ToString();
                        if (!string.IsNullOrEmpty(json))
                        {
                            response = JsonConvert.DeserializeObject<T>(json);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
            return response;
        }

        public static bool IsValidEmailAddress(string email)
        {
            try
            {
                Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,8}$");
                return regex.IsMatch(email);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return false;
            }
        }

       

        public static string[] ReadFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return File.ReadAllLines(path);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
            return null;
        }

        public static string GetIpPublish()
        {
            try
            {
                using (var client = new WebClient())
                {
                    return client.DownloadString(GetConfiguration("UrlGetIp"));
                }
            }
            catch (Exception ex)
            {
                // Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
            return GetConfiguration("IpDefoult");
        }


        public static void OpenKeyboard(bool keyBoard_Numeric, object textBox, object thisView, int x = 0, int y = 0)
        {
            try
            {



                WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
                {
                    control = textBox,
                    userControl = thisView is UserControl ? thisView as UserControl : null,
                    eType = (keyBoard_Numeric == true) ? WPKeyboard.Keyboard.EType.Numeric : WPKeyboard.Keyboard.EType.Standar,
                    window = thisView is Window ? thisView as Window : null,
                    X = x,
                    Y = y
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
        }

        public static string[] ErrorDevice()
        {
            try
            {
                string[] keys = Utilities.ReadFile(@"" + ConstantsResource.PathDevice);

                return keys;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return null;
            }
        }

        public static bool IsMultiple(decimal value)
        {
            try
            {
                if (value % 100 != 0)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
            }
            return true;
        }

        public static string GetDescriptionEnum(Type enume, string name)
        {
            try
            {
                MemberInfo info = enume.GetMember(name).First();
                return info.GetCustomAttribute<DescriptionAttribute>().Description.ToString();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return string.Empty;
            }
        }
    }
}
