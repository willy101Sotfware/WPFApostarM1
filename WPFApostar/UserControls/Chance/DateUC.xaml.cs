using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFApostar.Classes;
using WPFApostar.Models;
using WPFApostar.Services.ObjectIntegration;
using Path = System.IO.Path;
using LotteriesViewModel = WPFApostar.Models.LotteriesViewModel;
using WPFApostar.Classes.UseFull;

namespace WPFApostar.UserControls.Chance
{
    /// <summary>
    /// Lógica de interacción para DateUC.xaml
    /// </summary>
    public partial class DateUC : UserControl
    {
        public Transaction Transaction;
        private BitmapImage bgSelect = new BitmapImage(new Uri("/Images/SuperChance/Backgrounds/BgFechas.png", UriKind.RelativeOrAbsolute));
        private BitmapImage bgNoSelect = new BitmapImage(new Uri("/Images/SuperChance/Backgrounds/bgDia.png", UriKind.RelativeOrAbsolute));
        private DateTime selectedDateDT;
        private TimerGeneric timer;
        private Task consultarLoterias;
        public string año = "";
        private string _selectedDate;
        private string selectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;

                bgDia1.Source = bgNoSelect;
                bgDia2.Source = bgNoSelect;
                bgDia3.Source = bgNoSelect;
                Dispatcher.BeginInvoke((Action)delegate
                {
                    switch (_selectedDate)
                    {
                        case "fecha1":
                            selectedDateDT = DateTime.Now.Date;
                    //        bgDia1.Source = bgSelect;
                            break;
                        case "fecha2":
                            selectedDateDT = DateTime.Now.Date.AddDays(1);
                      //      bgDia2.Source = bgSelect;
                            break;
                        case "fecha3":
                            selectedDateDT = DateTime.Now.Date.AddDays(2);
                      //      bgDia3.Source = bgSelect;
                            break;
                    }
                    UpdateLayout();
                });

            }
        }

        private List<Loteria> resLoterias = new List<Loteria>();
        private ObservableCollection<LotteriesViewModel> lstLotteriesModel;
        private CollectionViewSource view = new CollectionViewSource();


        public DateUC(Transaction transaction)
        {
            InitializeComponent();
            Transaction = transaction;   
            txtValidaciones.Text = string.Empty;
            selectedDate = "fecha1";
            lstLotteriesModel = new ObservableCollection<LotteriesViewModel>();
            SetDays();
          ActivateTimer();

            //GetDays();
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


        private void ConsultLotteries()
        {
            try
            {

                Random rd = new Random();

                Thread.Sleep(50); // Es necesario esperar cualquier actualizacion de la fecha
                var requestObj = new RequestGetLotteries
                {
                    Ubicacion =  Utilities.GetConfiguration("UbicacionMaquina"),
                    Id = Transaction.productSelect.idField,
                    FechaS = Transaction.Fecha,
                    transaccion = rd.Next(2000,200000)
                };


                var respuesta = AdminPayPlus.ApiIntegration.GetLotteries(requestObj);

                if (respuesta == null)
                {
                    throw new Exception("Respuesta nula de la api de integracion");
                }

                if (respuesta.estadoField != true)
                {
                    throw new Exception("Estado false, no hubo respuesta satisfactoria");
                }

                resLoterias = respuesta.listadoloteriasField;


            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog($"Error en la consulta de loterias: {ex.Message} " + this.Name);
                Utilities.ShowModal("Ocurrio un error consultando los servicios de loterias", EModalType.Error);
                Utilities.navigator.Navigate(UserControlView.Dia, Transaction);
            }


        }

        private Task LoadLotteries()
        {
            this.consultarLoterias = Task.Run(() => ConsultLotteries());
            lstLotteriesModel = new ObservableCollection<LotteriesViewModel>();
            string loteriasPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "Loterias");
            string loteriasSPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "LoteriasS");

            InhabilitarVista();
            return Task.Run(() =>
            {
                Thread.Sleep(50);
                Dispatcher.BeginInvoke((Action)delegate
                {
                    try
                    {
                        // Primero esperar si se está haciendo alguna consulta de las loterias
                        if (consultarLoterias != null)
                        {
                            consultarLoterias.GetAwaiter().GetResult();
                            consultarLoterias.Dispose();
                            consultarLoterias = null;

                        }

                    

                            foreach (var loteria in resLoterias)
                            {

                                  bool Add = false;

                                 string PathL = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "Loterias", loteria.nombreField + ".png");

                                   
                                 if(loteria.nombreField == "DORADO NOCHE" && Utilities.ValidateFestivo(Transaction.Fecha))
                                 {
                                    Add = true;
                                 }
                                 if(loteria.nombreField == "DORADO NOCHE" && Convert.ToDateTime(Transaction.Fecha).ToString("dddd").ToUpper() == "SÁBADO")
                                 {
                                    Add = true;
                                 }      
                                 if(loteria.nombreField == "DORADO NOCHE" && Convert.ToDateTime(Transaction.Fecha).ToString("dddd").ToUpper() == "DOMINGO")
                                 {
                                    Add = true;
                                 }   
                                 if(loteria.nombreField == "PAISA 3" && Convert.ToDateTime(Transaction.Fecha).ToString("dddd").ToUpper() == "SÁBADO")
                                 {
                                    Add = true;
                                 }

                                 if (File.Exists(PathL))
                                 {


                                     if(loteria.nombreField != "DORADO NOCHE" && loteria.nombreField != "PAISA 3")
                                     {
                                          lstLotteriesModel.Add(new LotteriesViewModel
                                          {
                                              ImageData = Utilities.LoadImageFromFile(new Uri(Path.Combine(loteriasPath, loteria.nombreField + ".png"), UriKind.Relative)),
                                              CodigoCodesa = loteria.codigoField.ToString(),
                                              IdCodesa = loteria.idField.ToString(),
                                              Title = loteria.nombrecortoField,
                                              ImageS = Utilities.LoadImageFromFile(new Uri(Path.Combine(loteriasSPath, loteria.nombreField + ".png"), UriKind.Relative)),
                                              Image = Utilities.LoadImageFromFile(new Uri(Path.Combine(loteriasPath, loteria.nombreField + ".png"), UriKind.Relative)),
                                              Nombre = loteria.nombreField,
                                              NombreCorto = loteria.nombrecortoField,
                                              IsSelected = false
                                         });
                                     }
                                     else
                                     {
                                          if (loteria.nombreField == "DORADO NOCHE" && Add)
                                          {
                                              lstLotteriesModel.Add(new LotteriesViewModel
                                              {
                                                 ImageData = Utilities.LoadImageFromFile(new Uri(Path.Combine(loteriasPath, loteria.nombreField + ".png"), UriKind.Relative)),
                                                 CodigoCodesa = loteria.codigoField.ToString(),
                                                 IdCodesa = loteria.idField.ToString(),
                                                 Title = loteria.nombrecortoField,
                                                 ImageS = Utilities.LoadImageFromFile(new Uri(Path.Combine(loteriasSPath, loteria.nombreField + ".png"), UriKind.Relative)),
                                                 Image = Utilities.LoadImageFromFile(new Uri(Path.Combine(loteriasPath, loteria.nombreField + ".png"), UriKind.Relative)),
                                                 Nombre = loteria.nombreField,
                                                 NombreCorto = loteria.nombrecortoField,
                                                 IsSelected = false
                                              });
                                          }
                                          if (loteria.nombreField == "PAISA 3" && Add)
                                          {
                                              lstLotteriesModel.Add(new LotteriesViewModel
                                              {
                                                  ImageData = Utilities.LoadImageFromFile(new Uri(Path.Combine(loteriasPath, loteria.nombreField + ".png"), UriKind.Relative)),
                                                  CodigoCodesa = loteria.codigoField.ToString(),
                                                  IdCodesa = loteria.idField.ToString(),
                                                  Title = loteria.nombrecortoField,
                                                  ImageS = Utilities.LoadImageFromFile(new Uri(Path.Combine(loteriasSPath, loteria.nombreField + ".png"), UriKind.Relative)),
                                                  Image = Utilities.LoadImageFromFile(new Uri(Path.Combine(loteriasPath, loteria.nombreField + ".png"), UriKind.Relative)),
                                                  Nombre = loteria.nombreField,
                                                  NombreCorto = loteria.nombrecortoField,
                                                  IsSelected = false
                                              });
                                          }
                                  
                                     }
                                  
                      
                                 }
                            }

                        

                        

                        view.Source = lstLotteriesModel;
                        this.DataContext = view;
                        HabilitarVista();
                    }
                    catch (Exception ex)
                    {
                        AdminPayPlus.SaveErrorControl(JsonConvert.SerializeObject(ex), "Load lotteries", EError.Aplication, ELevelError.Medium);
                        HabilitarVista();
                    }

                });
            });
        }

        private void SetDays()
        {
            DateTime hoy = DateTime.Now.Date;

            dia_mes1.Content = "Hoy";
            mes_ano1.Content = hoy.ToString("MM/yyyy", new CultureInfo("es-CO")).ToUpper();
     //       mes_ano1.Content = hoy.ToString("ddddd", new CultureInfo("es-CO")).ToUpper();

            dia_mes2.Content = hoy.AddDays(1).ToString("dd");
            mes_ano2.Content = hoy.AddDays(1).ToString("MM/yyyy");

            dia_mes3.Content = hoy.AddDays(2).ToString("dd");
            mes_ano3.Content = hoy.AddDays(2).ToString("MM/yyyy");
        }



        private void InhabilitarVista()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 0.3;
                this.IsEnabled = false;
            });
        }

        private void HabilitarVista()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 1;
                this.IsEnabled = true;
            });
        }

        private void SelectDay_TouchDown(object sender, EventArgs e)
        {
            string Tag = "";

            if (sender is Label)
                 Tag = (sender as Label).Tag.ToString();
            if (sender is Image)
                Tag = (sender as Image).Tag.ToString();



            switch (Tag)
            {

                case "Fecha1":
                    Transaction.Fecha = DateTime.Now.ToString("dd/MM/yyyy");
                    Dia1S.Visibility = Visibility.Visible;
                    Dia2S.Visibility = Visibility.Hidden;
                    Dia3S.Visibility = Visibility.Hidden;

                    break;
                case "Fecha2":
                    Transaction.Fecha = string.Concat(dia_mes2.Content + "/" + mes_ano1.Content);
                    Dia1S.Visibility = Visibility.Hidden;
                    Dia2S.Visibility = Visibility.Visible;
                    Dia3S.Visibility = Visibility.Hidden;
                    break;
                case "Fecha3":
                    Transaction.Fecha = string.Concat(dia_mes3.Content + "/" + mes_ano3.Content);
                    Dia1S.Visibility = Visibility.Hidden;
                    Dia2S.Visibility = Visibility.Hidden;
                    Dia3S.Visibility = Visibility.Visible;
                    break;

            }


            LoadLotteries();
        }

        private void BtnSelectLottery(object sender, EventArgs e)
        {
            var data = sender as ListViewItem;
            LotteriesViewModel selectedLottery = (LotteriesViewModel)data.Content;

            selectedLottery.IsSelected = !selectedLottery.IsSelected;

            lvLotteries.Items.Refresh();
            txtValidaciones.Text = "";
        }

        private void Btn_CancelarTouchDown(object sender, EventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void Btn_ContinuarTouchDown(object sender, EventArgs e)
        {
            DisableView();

            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);

            if (lstLotteriesModel.Where(lottery => lottery.IsSelected).Count() <= 0)
            {
                txtValidaciones.Text = "*No se ha seleccionado ninguna loteria";
                EnableView();
                ActivateTimer();
                return;
            }

       //     SetCallBacksNull();
     //       timer.CallBackStop?.Invoke(1);
       //     Transaction.Fecha = selectedDateDT.ToString("dd/MM/yyyy"); // Todas las fechas se deben consultar asi
            Transaction.ListaLoteriasSeleccionadas = lstLotteriesModel.Where(lot => lot.IsSelected).ToList();
     
            Utilities.navigator.Navigate(UserControlView.Apuesta,Transaction);
            //   GetLotteries();

        }

        private void Btn_SelectDay(object sender, TouchEventArgs e)
        {

        }

        private void Btn_ContinueMouse(object sender, MouseButtonEventArgs e)
        {
     //       SetCallBacksNull();
     //       timer.CallBackStop?.Invoke(1);
            //GetLotteries();
        }

        private void Btn_SelectMoused(object sender, MouseButtonEventArgs e)
        {
       

        }

        private void BtnAtras_TouchDown(object sender, EventArgs e)
        {
   //         SetCallBacksNull();
    //        timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void ActivateTimer()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    tbTimer.Text = "00:59";
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
