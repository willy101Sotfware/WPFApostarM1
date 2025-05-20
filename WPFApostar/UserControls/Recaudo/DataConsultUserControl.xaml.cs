using Grabador.Transaccion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFApostar.Classes;
using WPFApostar.Classes.UseFull;
using WPFApostar.Models;
using WPFApostar.Resources;
using WPFApostar.ViewModel;

namespace WPFApostar.UserControls.Recaudo
{
    /// <summary>
    /// Lógica de interacción para DataConsultUserControl.xaml
    /// </summary>
    public partial class DataConsultUserControl : UserControl
    {

        private Transaction Transaction;
        private CollectionViewSource view;
        private ObservableCollection<DetailRenovateList> lstPager;
        private DetailRenovateList ProductsSelected;
        private TimerGeneric timer;



        public DataConsultUserControl(Transaction Ts)
        {
            InitializeComponent();
            Transaction = Ts;
            this.DataContext = Transaction;
            Company.Content = Transaction.Company.Nombre;
            lstPager = new ObservableCollection<DetailRenovateList>();
            view = new CollectionViewSource();
            ProductsSelected = new DetailRenovateList();
            InitView();
            ActivateTimer();


        }

        private void Btn_back_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private string GetImage(bool flag)
        {
            try
            {
                if (!flag)
                {
                    return "/Images/Others/rbtn-off.png";
                }

                return "/Images/Others/ok.png";
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
            return string.Empty;
        }

        private void InitView()
        {
            try
            {

                //DateTime fb = DateTime.ParseExact(Transaction.referenceConsult.Fecha_limite_pago, "dd/MM/yyyy", null);

                ////   M = Fecha.ToString("MMM", new CultureInfo("es-CO")).ToUpper();


                //Identificacion.Content = Transaction.referenceConsult.Propiedades[0].Valor;
                //RazonS.Content = Transaction.referenceConsult.InformacionAdicional[0].Valor;

                foreach(var x in Transaction.DataReference.listadoregistrosField.registroField)
                {
                    if (x.etiquetaField.Contains("VALOR"))
                    {
                        Transaction.Amount = x.valorField;
                    }
                }

                ValueT.Content = string.Format("{0:C0}", Convert.ToDecimal(Transaction.Amount));


                lstPager.Add(new DetailRenovateList
                {
                    img = GetImage(false),
                    Value = Convert.ToDecimal(Transaction.Amount),
                    Referencia = Transaction.reference,
                    FechaLimite = DateTime.Now.ToString("ddd d MMM yyyy", CultureInfo.CreateSpecificCulture("es-CO")).ToUpper()
                });


                if (lstPager.Count > 0)
                {
                    view.Source = lstPager;
                    lv_Products.DataContext = view;
                }



            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void Btn_Continuar(object sender, TouchEventArgs e)
        {


         
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);



                Transaction.infoNotify = new List<infoNotifyOrder>();

                List<infoNotifyOrder> LstData = new List<infoNotifyOrder>();


                foreach (var x in Transaction.DataReference.listadoserviciosField.servicioField.listadocamposField)
                {

                    foreach (var y in Transaction.DataReference.listadoregistrosField.registroField)
                    {

                        if (x.etiquetaField == y.etiquetaField)
                        {

                            infoNotifyOrder data = new infoNotifyOrder();

                            data.id = x.idField;
                            data.Nombre = x.nombreField;
                            data.valor = y.valorField;

                            LstData.Add(data);

                        }

                    }

                };

                Transaction.infoNotify = LstData;

                validateMoney();


            


        
        }

        public void validateMoney()
        {

            try
            {


                Task.Run(async () =>
                {

                    var isValidateMoney = AdminPayPlus.ValidateMoney(Transaction).GetAwaiter().GetResult();

                    Utilities.CloseModal();

                    if (isValidateMoney != false)
                    {
                        Utilities.CloseModal();
                        SendData();
                    }
                    else
                    {
                        Utilities.CloseModal();
                        Utilities.ShowModal("En estos momentos la maquina no cuenta con suficiente cargue para esta operación", EModalType.Error);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }

                });

                Utilities.ShowModal("Estamos verificando la transacción un momento por favor", EModalType.Preload);

            }
            catch (Exception ex)
            {

            }


        }

        private async void SendData()
        {
            try
            {

                AdminPayPlus.SaveLog("DataConsult", "entrando a la ejecucion SendData", "OK", "", Transaction);

                Task.Run(async () =>
                {
                    //Transaction.payer = new DataModel.PAYER
                    //{
                    //    IDENTIFICATION = Transaction.Document,
                    //    NAME = Transaction.Name,

                    //    STATE = Transaction.statePaySuccess,

                    //};

                    Transaction.State = ETransactionState.Initial;
                    Transaction.Type = ETypeTramites.Recaudos;
                    Transaction.Tipo = ETransactionType.Payment;
                    Transaction.eTypeTramites = ETypeTramites.Recaudos;
                    Transaction.valor = Transaction.Amount;
                    Transaction.reference = Transaction.infoNotify[0].valor;
                    await AdminPayPlus.SaveTransaction(Transaction);

                    AdminPayPlus.SaveLog("DataConsult", "SendData", "OK", string.Concat("ID Transaccion:", Transaction.IdTransactionAPi, "/n", "Estado Transaccion:", "inicial", "/n", "Monto:", Transaction.Amount.ToString()), Transaction);


                    Utilities.CloseModal();

                    if (this.Transaction.IdTransactionAPi == 0)
                    {
                        Utilities.ShowModal("No se puede guardar la transacción, intentelo más tarde.", EModalType.Error);

                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }
                    else
                    {

                        CLSGrabador.IniciarGrabacion(new DataVidio
                        {
                            paypadID = 1212,
                            RecorderRoute = Utilities.GetConfiguration("RecorderRoute"),
                            selectedCamera = 0,
                            transactionID = Convert.ToInt32(Transaction.IdTransactionAPi),
                            videoPath = $"'{Utilities.GetConfiguration("RutaVideo")}'",
                            mailAlert = $"'{Utilities.GetConfigData("Email")}'"
                        });
                        Thread.Sleep(100);
                        CLSGrabador.IniciarGrabacion(new DataVidio
                        {
                            paypadID = 1212,
                            RecorderRoute = Utilities.GetConfiguration("RecorderRoute"),
                            selectedCamera = 1,
                            transactionID = Convert.ToInt32(Transaction.IdTransactionAPi),
                            videoPath = $"'{Utilities.GetConfiguration("RutaVideo")}'",
                            mailAlert = $"'{Utilities.GetConfigData("Email")}'"
                        });

                        Utilities.navigator.Navigate(UserControlView.PaymentRecaudo, Transaction);
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


        private void ListViewItem_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var service = (DetailRenovateList)(sender as ListViewItem).Content;
                if (service.img.Contains("ok.png"))
                {

                    service.img = GetImage(false);
                  //  BtnCompraCertificados.Visibility = Visibility.Hidden;


                }
                else
                {
                    service.img = GetImage(true);

                    //if (Transaction.referenceConsult.Propiedades[3].Valor == "true ")
                    //{
                    //    BtnCompraCertificados.Visibility = Visibility.Visible;
                    //}
                    //else
                    //{
                    //    BtnCompraCertificados.Visibility = Visibility.Hidden;

                    //}
                }

                //           ProductsSelected.img = GetImage(false);

                lv_Products.Items.Refresh();

                ProductsSelected = service;

                //Transaction.detailsPago = service;

                

                Transaction.Amount = Utilities.RoundValue(service.Value).ToString();

                Transaction.RealAmount = service.Value;
                //    

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
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
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
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
