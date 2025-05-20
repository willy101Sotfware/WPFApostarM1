using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using WPFApostar.Classes;
using WPFApostar.Models;
using System.IO;
using LotteriesViewModel = WPFApostar.Models.LotteriesViewModel;
using WPFApostar.Resources;
using WPFApostar.Classes.UseFull;
using System.Windows.Input;

namespace WPFApostar.UserControls.Chance
{
    /// <summary>
    /// Lógica de interacción para ConfirmLotteryUC.xaml
    /// </summary>
    public partial class ConfirmLotteryUC : UserControl
    {

        Transaction Transaction;
        private ObservableCollection<LotteriesViewModel> lstLotteriesModel;
        private CollectionViewSource view = new CollectionViewSource();
        private TimerGeneric timer;


        public ConfirmLotteryUC(Transaction transaction)
        {
            InitializeComponent();
            Transaction = transaction;
            this.DataContext = Transaction;        
            view = new CollectionViewSource();
            lstLotteriesModel = new ObservableCollection<LotteriesViewModel>();
            GenerateTotal();
            CargarNumeros();
           ActivateTimer();
            LoadLotteries();
        //    InitView();
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


        public void GenerateTotal()
        {
            try
            {

                int Total = 0;
                double iva = 0;

                foreach(var x in Transaction.ListaChances)
                {
                    Total += x.GetTotalChance();
                }

                iva = Total * 0.19;

                Transaction.Amount = Total.ToString();

                int ValorTL = (int)(Convert.ToInt32(Transaction.Amount) - iva);

                Iva.Content = string.Format("{0:C0}",Convert.ToDecimal(iva));
                ValorT.Content = string.Format("{0:C0}", Convert.ToDecimal(Transaction.Amount));
                Valor.Content = string.Format("{0:C0}", Convert.ToDecimal(ValorTL));

            }
            catch (Exception ex)
            {

            }
        }

        public void LoadLotteries()
        {
            try
            {
                string loteriasPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "Loterias");

                foreach (var loteria in Transaction.ListaLoteriasSeleccionadas)
                {
                    lstLotteriesModel.Add(new LotteriesViewModel
                    {
                        ImageData = Utilities.LoadImageFromFile(new Uri(Path.Combine(loteriasPath, loteria.Nombre + ".png"), UriKind.Relative)) 
                    });
                }

                view.Source = lstLotteriesModel;
                this.DataContext = view;
            }
            catch (Exception ex)
            {

            }
        }

        public void CargarNumeros()
        {

            try
            {

                switch (Transaction.ListaChances.Count())
                {

                    case 1:

                        Num1.Text = Transaction.ListaChances[0].Numero;
                        Directo1.Text = Transaction.ListaChances[0].Directo.ToString();
                        Combinado1.Text = Transaction.ListaChances[0].Combinado.ToString();
                        Pata1.Text = Transaction.ListaChances[0].Pata.ToString();
                        Una1.Text = Transaction.ListaChances[0].Una.ToString();

                    break;
                    case 2:
                        Num1.Text = Transaction.ListaChances[0].Numero;
                        Directo1.Text = Transaction.ListaChances[0].Directo.ToString();
                        Combinado1.Text = Transaction.ListaChances[0].Combinado.ToString();
                        Pata1.Text = Transaction.ListaChances[0].Pata.ToString();
                        Una1.Text = Transaction.ListaChances[0].Una.ToString();

                        Num2.Text = Transaction.ListaChances[1].Numero;
                        Directo2.Text = Transaction.ListaChances[1].Directo.ToString();
                        Combinado2.Text = Transaction.ListaChances[1].Combinado.ToString();
                        Pata2.Text = Transaction.ListaChances[1].Pata.ToString();
                        Una2.Text = Transaction.ListaChances[1].Una.ToString();
                        break;
                    case 3:
                        Num1.Text = Transaction.ListaChances[0].Numero;
                        Directo1.Text = Transaction.ListaChances[0].Directo.ToString();
                        Combinado1.Text = Transaction.ListaChances[0].Combinado.ToString();
                        Pata1.Text = Transaction.ListaChances[0].Pata.ToString();
                        Una1.Text = Transaction.ListaChances[0].Una.ToString();

                        Num2.Text = Transaction.ListaChances[1].Numero;
                        Directo2.Text = Transaction.ListaChances[1].Directo.ToString();
                        Combinado2.Text = Transaction.ListaChances[1].Combinado.ToString();
                        Pata2.Text = Transaction.ListaChances[1].Pata.ToString();
                        Una2.Text = Transaction.ListaChances[1].Una.ToString();

                        Num3.Text = Transaction.ListaChances[2].Numero;
                        Directo3.Text = Transaction.ListaChances[2].Directo.ToString();
                        Combinado3.Text = Transaction.ListaChances[2].Combinado.ToString();
                        Pata3.Text = Transaction.ListaChances[2].Pata.ToString();
                        Una3.Text = Transaction.ListaChances[2].Una.ToString();

                        break;
                    case 4:
                        Num1.Text = Transaction.ListaChances[0].Numero;
                        Directo1.Text = Transaction.ListaChances[0].Directo.ToString();
                        Combinado1.Text = Transaction.ListaChances[0].Combinado.ToString();
                        Pata1.Text = Transaction.ListaChances[0].Pata.ToString();
                        Una1.Text = Transaction.ListaChances[0].Una.ToString();

                        Num2.Text = Transaction.ListaChances[1].Numero;
                        Directo2.Text = Transaction.ListaChances[1].Directo.ToString();
                        Combinado2.Text = Transaction.ListaChances[1].Combinado.ToString();
                        Pata2.Text = Transaction.ListaChances[1].Pata.ToString();
                        Una2.Text = Transaction.ListaChances[1].Una.ToString();

                        Num3.Text = Transaction.ListaChances[2].Numero;
                        Directo3.Text = Transaction.ListaChances[2].Directo.ToString();
                        Combinado3.Text = Transaction.ListaChances[2].Combinado.ToString();
                        Pata3.Text = Transaction.ListaChances[2].Pata.ToString();
                        Una3.Text = Transaction.ListaChances[2].Una.ToString();

                        Num4.Text = Transaction.ListaChances[3].Numero;
                        Directo4.Text = Transaction.ListaChances[3].Directo.ToString();
                        Combinado4.Text = Transaction.ListaChances[3].Combinado.ToString();
                        Pata4.Text = Transaction.ListaChances[3].Pata.ToString();
                        Una4.Text = Transaction.ListaChances[3].Una.ToString();
                        break;
                    case 5:
                        Num1.Text = Transaction.ListaChances[0].Numero;
                        Directo1.Text = Transaction.ListaChances[0].Directo.ToString();
                        Combinado1.Text = Transaction.ListaChances[0].Combinado.ToString();
                        Pata1.Text = Transaction.ListaChances[0].Pata.ToString();
                        Una1.Text = Transaction.ListaChances[0].Una.ToString();

                        Num2.Text = Transaction.ListaChances[1].Numero;
                        Directo2.Text = Transaction.ListaChances[1].Directo.ToString();
                        Combinado2.Text = Transaction.ListaChances[1].Combinado.ToString();
                        Pata2.Text = Transaction.ListaChances[1].Pata.ToString();
                        Una2.Text = Transaction.ListaChances[1].Una.ToString();

                        Num3.Text = Transaction.ListaChances[2].Numero;
                        Directo3.Text = Transaction.ListaChances[2].Directo.ToString();
                        Combinado3.Text = Transaction.ListaChances[2].Combinado.ToString();
                        Pata3.Text = Transaction.ListaChances[2].Pata.ToString();
                        Una3.Text = Transaction.ListaChances[2].Una.ToString();

                        Num4.Text = Transaction.ListaChances[3].Numero;
                        Directo4.Text = Transaction.ListaChances[3].Directo.ToString();
                        Combinado4.Text = Transaction.ListaChances[3].Combinado.ToString();
                        Pata4.Text = Transaction.ListaChances[3].Pata.ToString();
                        Una4.Text = Transaction.ListaChances[3].Una.ToString();

                        Num5.Text = Transaction.ListaChances[4].Numero;
                        Directo5.Text = Transaction.ListaChances[4].Directo.ToString();
                        Combinado5.Text = Transaction.ListaChances[4].Combinado.ToString();
                        Pata5.Text = Transaction.ListaChances[4].Pata.ToString();
                        Una5.Text = Transaction.ListaChances[4].Una.ToString();
                        break;
                    case 6:
                        Num1.Text = Transaction.ListaChances[0].Numero;
                        Directo1.Text = Transaction.ListaChances[0].Directo.ToString();
                        Combinado1.Text = Transaction.ListaChances[0].Combinado.ToString();
                        Pata1.Text = Transaction.ListaChances[0].Pata.ToString();
                        Una1.Text = Transaction.ListaChances[0].Una.ToString();

                        Num2.Text = Transaction.ListaChances[1].Numero;
                        Directo2.Text = Transaction.ListaChances[1].Directo.ToString();
                        Combinado2.Text = Transaction.ListaChances[1].Combinado.ToString();
                        Pata2.Text = Transaction.ListaChances[1].Pata.ToString();
                        Una2.Text = Transaction.ListaChances[1].Una.ToString();

                        Num3.Text = Transaction.ListaChances[2].Numero;
                        Directo3.Text = Transaction.ListaChances[2].Directo.ToString();
                        Combinado3.Text = Transaction.ListaChances[2].Combinado.ToString();
                        Pata3.Text = Transaction.ListaChances[2].Pata.ToString();
                        Una3.Text = Transaction.ListaChances[2].Una.ToString();

                        Num4.Text = Transaction.ListaChances[3].Numero;
                        Directo4.Text = Transaction.ListaChances[3].Directo.ToString();
                        Combinado4.Text = Transaction.ListaChances[3].Combinado.ToString();
                        Pata4.Text = Transaction.ListaChances[3].Pata.ToString();
                        Una4.Text = Transaction.ListaChances[3].Una.ToString();

                        Num5.Text = Transaction.ListaChances[4].Numero;
                        Directo5.Text = Transaction.ListaChances[4].Directo.ToString();
                        Combinado5.Text = Transaction.ListaChances[4].Combinado.ToString();
                        Pata5.Text = Transaction.ListaChances[4].Pata.ToString();
                        Una5.Text = Transaction.ListaChances[4].Una.ToString();

                        Num6.Text = Transaction.ListaChances[5].Numero;
                        Directo6.Text = Transaction.ListaChances[5].Directo.ToString();
                        Combinado6.Text = Transaction.ListaChances[5].Combinado.ToString();
                        Pata6.Text = Transaction.ListaChances[5].Pata.ToString();
                        Una6.Text = Transaction.ListaChances[5].Una.ToString();
                        break;
                    case 7:
                        Num1.Text = Transaction.ListaChances[0].Numero;
                        Directo1.Text = Transaction.ListaChances[0].Directo.ToString();
                        Combinado1.Text = Transaction.ListaChances[0].Combinado.ToString();
                        Pata1.Text = Transaction.ListaChances[0].Pata.ToString();
                        Una1.Text = Transaction.ListaChances[0].Una.ToString();

                        Num2.Text = Transaction.ListaChances[1].Numero;
                        Directo2.Text = Transaction.ListaChances[1].Directo.ToString();
                        Combinado2.Text = Transaction.ListaChances[1].Combinado.ToString();
                        Pata2.Text = Transaction.ListaChances[1].Pata.ToString();
                        Una2.Text = Transaction.ListaChances[1].Una.ToString();

                        Num3.Text = Transaction.ListaChances[2].Numero;
                        Directo3.Text = Transaction.ListaChances[2].Directo.ToString();
                        Combinado3.Text = Transaction.ListaChances[2].Combinado.ToString();
                        Pata3.Text = Transaction.ListaChances[2].Pata.ToString();
                        Una3.Text = Transaction.ListaChances[2].Una.ToString();

                        Num4.Text = Transaction.ListaChances[3].Numero;
                        Directo4.Text = Transaction.ListaChances[3].Directo.ToString();
                        Combinado4.Text = Transaction.ListaChances[3].Combinado.ToString();
                        Pata4.Text = Transaction.ListaChances[3].Pata.ToString();
                        Una4.Text = Transaction.ListaChances[3].Una.ToString();

                        Num5.Text = Transaction.ListaChances[4].Numero;
                        Directo5.Text = Transaction.ListaChances[4].Directo.ToString();
                        Combinado5.Text = Transaction.ListaChances[4].Combinado.ToString();
                        Pata5.Text = Transaction.ListaChances[4].Pata.ToString();
                        Una5.Text = Transaction.ListaChances[4].Una.ToString();

                        Num6.Text = Transaction.ListaChances[5].Numero;
                        Directo6.Text = Transaction.ListaChances[5].Directo.ToString();
                        Combinado6.Text = Transaction.ListaChances[5].Combinado.ToString();
                        Pata6.Text = Transaction.ListaChances[5].Pata.ToString();
                        Una6.Text = Transaction.ListaChances[5].Una.ToString();

                        Num7.Text = Transaction.ListaChances[6].Numero;
                        Directo7.Text = Transaction.ListaChances[6].Directo.ToString();
                        Combinado7.Text = Transaction.ListaChances[6].Combinado.ToString();
                        Pata7.Text = Transaction.ListaChances[6].Pata.ToString();
                        Una7.Text = Transaction.ListaChances[6].Una.ToString();
                        break;
                };
               
            }
            catch (Exception ex)
            {

            }
        }

        private void Btn_Cancel(object sender, EventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Apuesta, Transaction);
        }

        private void Btn_Continue(object sender, EventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            SendData();
        }

        private void Btn_Cancel_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Btn_Cancel(sender, e);
        }

        private void Btn_Continue_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Btn_Continue(sender, e);
        }

        private void ActivateTimer()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    tbTimer.Text = "01:30";
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



        private async void SendData()
        {
            try
            {

                AdminPayPlus.SaveLog("DataConsult", "entrando a la ejecucion SendData", "OK", "", Transaction);

                Task.Run(async () =>
                {

                    Transaction.State = ETransactionState.Initial;
                    Transaction.Type = ETypeTramites.Chance;
                    Transaction.Tipo = ETransactionType.Payment;
                    Transaction.eTypeTramites = ETypeTramites.Chance;
                    Transaction.valor = Transaction.Amount;

                    await AdminPayPlus.SaveTransactionChance(Transaction);

                    AdminPayPlus.SaveLog("DataConsult", "SendData", "OK", string.Concat("ID Transaccion:", Transaction.IdTransactionAPi, "/n", "Estado Transaccion:", "inicial", "/n", "Monto:", Transaction.Amount.ToString()), Transaction);

               //     SetCallBacksNull();
            //        timer.CallBackStop?.Invoke(1);

                    Utilities.CloseModal();

                    if (this.Transaction.IdTransactionAPi == 0)
                    {
                        Utilities.ShowModal("No se puede guardar la transacción, intentelo más tarde.", EModalType.Error);

                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }
                    else
                    {

                        //CLSGrabador.IniciarGrabacion(new DataVidio
                        //{
                        //    paypadID = 1212,
                        //    RecorderRoute = Utilities.GetConfiguration("RecorderRoute"),
                        //    selectedCamera = 0,
                        //    transactionID = Convert.ToInt32(Transaction.IdTransactionAPi),
                        //    videoPath = $"'{Utilities.GetConfiguration("RutaVideo")}'",
                        //    mailAlert = $"'{Utilities.GetConfigData("Email")}'"
                        //});
                        //Thread.Sleep(100);
                        //CLSGrabador.IniciarGrabacion(new DataVidio
                        //{
                        //    paypadID = 1212,
                        //    RecorderRoute = Utilities.GetConfiguration("RecorderRoute"),
                        //    selectedCamera = 1,
                        //    transactionID = Convert.ToInt32(Transaction.IdTransactionAPi),
                        //    videoPath = $"'{Utilities.GetConfiguration("RutaVideo")}'",
                        //    mailAlert = $"'{Utilities.GetConfigData("Email")}'"
                        //});

                        Utilities.navigator.Navigate(UserControlView.PagosChance, Transaction);
                    }
                });

                Utilities.ShowModal(MessageResource.LoadInformation, EModalType.Preload);


            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("RechargeUC", "Error Catch la ejecucion SendData", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), Transaction);
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }


    }



 



    internal class ChanceListView
    {
        public string Numero { get; set; }
        public int Directo { get; set; }
        public int Combinado { get; set; }
        public int Pata { get; set; }
        public int Una { get; set; }
        public int Total { get; set; }
    }
}
