using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFApostar.Classes;
using WPFApostar.Classes.UseFull;
using WPFApostar.Models;
using WPFApostar.ViewModel;
using System.IO;

namespace WPFApostar.UserControls.Paquetes
{
    /// <summary>
    /// Lógica de interacción para SelectPaqueteUC.xaml
    /// </summary>
    public partial class SelectPaqueteUC : UserControl
    {

        private ObservableCollection<OperatorsViewModel> LstSelectOperators;

        private ObservableCollection<OperatorsViewModel> LstPackets;

        Transaction Transaction;

        CollectionViewSource view = new CollectionViewSource();

        OperatorsViewModel SelectedOperator = new OperatorsViewModel();

        private TimerGeneric timer;
        public SelectPaqueteUC(Transaction transaction)
        {
            InitializeComponent();

            Transaction = transaction;

            ActivateTimer();
            LstPackets = new ObservableCollection<OperatorsViewModel>();

            Loaded += MainWindow_Loaded;
            LoadPacketsAsync();
            {


            }

        }


        private void Btn_SelectPacketPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DisableView();
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
                var data = sender as ListViewItem;
                SelectedOperator = (OperatorsViewModel)data.Content;
                Transaction.SelectOperator.idOperador = SelectedOperator.idPaqueteOperador;
                Transaction.SelectOperator.nomPaquete = SelectedOperator.nomPaquete;
                Transaction.SelectOperator.nomCorto = SelectedOperator.abreviatura;
                Transaction.SelectOperator.valorComercial = Convert.ToDouble(SelectedOperator.valorComercial.Replace("$", ""));
                Utilities.navigator.Navigate(UserControlView.DigitarNumero, Transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void BtnCancelar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadOperatorAsync();
        }


        private async Task LoadOperatorAsync()
        {
            string paqueteDisponible = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "PaquetesImages");
            string pathL = Path.Combine(paqueteDisponible, Transaction.Operador + ".png");

            if (File.Exists(pathL))
            {
                var imageUri = new Uri(pathL, UriKind.Absolute);
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = imageUri;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                ImageData.Source = bitmap;

            }

        }





        public async Task LoadPacketsAsync()
        {
            try
            {


                foreach (var Operator in Transaction.responseConsultPaquetes.Listadopaquetes.Paquetes)
                {

                    LstPackets.Add(new OperatorsViewModel
                    {
                        idPaqueteOperador = Operator.Id,
                        nomPaquete = Operator.Nombre,
                        abreviatura = Operator.Nombrecorto,
                        valorComercial = String.Format("{0:C0}", Operator.Valor),


                    });

                };

                //LstSelectOperators.Add(new OperatorsViewModel
                //{
                //    ImageData =
                //             Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                //            Assembly.GetEntryAssembly().Location),
                //            "Loterias", Transaction.LotteryList.model.list[0].desLoteria.ToString() + ".png"))),
                //    Tag = Transaction.LotteryList.model.list[0].sorteo.ToString(),
                //    IdLoteria = Transaction.LotteryList.model.list[0].idLoteria.ToString(),
                //    DesLoteria = Transaction.LotteryList.model.list[0].desLoteria.ToString(),
                //    abreviatura = Transaction.LotteryList.model.list[0].abreviatura.ToString(),
                //    ImageDataS = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                //            Assembly.GetEntryAssembly().Location),
                //            "LoteriasS", Transaction.LotteryList.model.list[0].desLoteria.ToString() + ".png"))),
                //    IsSelect = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                //            Assembly.GetEntryAssembly().Location),
                //            "Loterias", Transaction.LotteryList.model.list[0].desLoteria.ToString() + ".png"))),
                //});

                view.Source = LstPackets;
                this.DataContext = view;
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveErrorControl(JsonConvert.SerializeObject(ex), "Load lotteries", EError.Aplication, ELevelError.Medium);
            }
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



        private void ActivateTimer()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    tbTimer.Text = "00:60";
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
