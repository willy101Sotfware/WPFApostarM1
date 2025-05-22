using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.IO;
using WPFApostar.Domain;
using WPFApostar.Domain.UseFull;
using WPFApostar.ViewModel;
using Path = System.IO.Path;
using WPFApostar.Domain.ApiService.Models;
using WPFApostar.Domain.UIServices.ObjectIntegration;

namespace WPFApostar.UserControls.Recaudo
{
    /// <summary>
    /// Lógica de interacción para SelectCompanyUC.xaml
    /// </summary>
    public partial class SelectCompanyUC : UserControl
    {
        private Transaction Transaction;
        private TimerGeneric timer;
        CollectionViewSource view = new CollectionViewSource();
        RecaudosViewModel Recaudos = new RecaudosViewModel();
        private ObservableCollection<RecaudosViewModel> LstRecaudosModel;


        public SelectCompanyUC(Transaction Ts)
        {   
            InitializeComponent();
            Transaction = Ts;
            this.DataContext = Transaction;
            LstRecaudosModel = new ObservableCollection<RecaudosViewModel>();
            Transaction.ParametersCompany = new List<Listadocamposfield>();
       
      

            if(Transaction.tramite == "All")
            {
                OrderRecaudos();
            }
            else
            {
                RecaudosFilter();
            }

            ActivateTimer();

        }

        #region Methods

        public void RecaudosFilter()
        {
            try
            {

                foreach (var Recaudos in Transaction.listadorecaudosField)
                {

                    string ImagePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Recaudos",Transaction.tramite, Recaudos.Codigo + ".png");

                    if (File.Exists(ImagePath))
                    {
                        LstRecaudosModel.Add(new RecaudosViewModel
                        {

                            Codigo = Recaudos.Codigo,
                            CodigoSubProducto = Recaudos.Codigosubproducto,
                            Iva = Recaudos.Iva,
                            Nombre = Recaudos.Nombre,
                            ImageData = Utilities.LoadImageFromFile(new Uri(ImagePath))
                            

                        });

                    }


                }

                view.Source = LstRecaudosModel;
                 this.DataContext = view;
 
            }
            catch (Exception ex)
            {

            }
        }

        public void OrderRecaudos()
        {

            try
            {

                foreach (var Recaudos in Transaction.listadorecaudosField)
                {

                    string ImagePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Recaudos", Recaudos.Codigo + ".png");

                    if(File.Exists(ImagePath))
                    {
                        LstRecaudosModel.Add(new RecaudosViewModel
                        {

                            Codigo = Recaudos.Codigo,
                            CodigoSubProducto = Recaudos.Codigosubproducto,
                            Iva = Recaudos.Iva,
                            Nombre = Recaudos.Nombre,
                            ImageData = Utilities.LoadImageFromFile(new Uri(ImagePath))


                        });
                    }

                 


                }

                if (LstRecaudosModel.Count() > 0)
                {
                    view.Source = LstRecaudosModel;
                    this.DataContext = view;
                }
                else
                {
                    Utilities.ShowModal("No hay Recaudos disponibles para este servicio,intente nuevamente por favor.", EModalType.Error);
                    Utilities.navigator.Navigate(UserControlView.TypeRecaudo, Transaction);
                }
            }
            catch (Exception ex)
            {

            }

        }

        #endregion


        private void BtnAtras_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.TypeRecaudo,Transaction);
        }

        private void Btn_SelectRecaudo(object sender, TouchEventArgs e)
        {
            try
            {

                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);

                var data = sender as ListViewItem;

                Recaudos = (RecaudosViewModel)data.Content;

                RequestGetParameters request = new RequestGetParameters
                {

                    recaudo = new Services.ObjectIntegration.Recaudo
                    {
                        codigo = Recaudos.Codigo
                    },
                    idDepartamento = new Iddepartamento
                    {
                        codigo = Convert.ToInt32(Utilities.GetConfigData("IdDepartamento"))
                    },
                    IdTrx = Transaction.IdTransactionAPi


                };
         
                Task.Run(() =>
                {

                    var Response = AdminPayPlus.ApiIntegration.GetParameters(request);

                    Utilities.CloseModal();

                    if(Response != null)
                    {

                        if (Response.estadoField)
                        {

                            Transaction.Company = new DataCompany
                            {

                                CodigoSubProducto = Recaudos.CodigoSubProducto,
                                Codigo = Recaudos.Codigo,
                                Iva = Recaudos.Iva,
                                Nombre = Recaudos.Nombre,
                                ImageData = Recaudos.ImageData

                            };

                            List<Listadocamposfield> filter = new List<Listadocamposfield>();

                            foreach(var item in Response.recaudoField.listadoserviciosField.servicioField.listadocamposField)
                            {

                                Listadocamposfield filt = new Listadocamposfield();

                                if (item.visibleField)
                                {
                                    filt.etiquetaField = item.etiquetaField;
                                    filt.visibleField = item.visibleField;
                                    filt.editableField = item.editableField;
                                    filt.idField = item.idField;
                                    filt.tipodatoField = item.tipodatoField;
                                    filt.nombreField = item.nombreField;
                                    filt.formatosalidaField = item.formatosalidaField;
                                    filt.jsonvalidacionesField = item.jsonvalidacionesField;
                                    filt.tipoCampoField = item.tipoCampoField;

                                    filter.Add(filt);
                                }


                            }

                     //       Transaction.ParametersCompany = Response.recaudoField.listadoserviciosField.servicioField.listadocamposField;

                            Transaction.IdServicioRecaudo = Response.recaudoField.listadoserviciosField.servicioField.idField;

                            Transaction.ParametersCompany = filter;

                            if(Transaction.Company.Codigo == 8 || Transaction.Company.Codigo == 46 || Transaction.Company.Codigo == 24 || Transaction.Company.Codigo == 19)
                            {
                                Utilities.navigator.Navigate(UserControlView.ConsultForm3, Transaction);

                            }
                            else  if (Transaction.ParametersCompany.Count() > 5)
                            {
                                Utilities.navigator.Navigate(UserControlView.ConsultForm2, Transaction);
                            }
                            else if (Transaction.ParametersCompany.Count() > 1 && Transaction.ParametersCompany.Count() < 6)
                            {
                                Utilities.navigator.Navigate(UserControlView.ConsultForm, Transaction);
                            }
                            else
                            {
                                Utilities.navigator.Navigate(UserControlView.ConsultReference, Transaction);
                            }
                        }
                        else
                        {
                            Utilities.ShowModal("Hubo un error Consultando la información por favor intente nuevamente", EModalType.Error);
                        }

                    }
                    else
                    {

                        Utilities.ShowModal("Hubo un error Consultando la información por favor intente nuevamente",EModalType.Error);

                    }

                });

                Utilities.ShowModal("Estamos consultando la información un momento por favor",EModalType.Preload);


            }
            catch (Exception ex)
            {
                Utilities.ShowModal("Hubo un error Consultando la información por favor intente nuevamente", EModalType.Error);
            }
        }

        private void BtnSelectCompany_TouchDown(object sender, TouchEventArgs e)
        {
       //     Utilities.navigator.Navigate(UserControlView.Selectoption,Transaction);
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
