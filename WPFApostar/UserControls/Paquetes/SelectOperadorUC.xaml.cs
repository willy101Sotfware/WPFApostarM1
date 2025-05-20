using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPFApostar.Classes.UseFull;
using WPFApostar.Classes;
using WPFApostar.Models;
using WPFApostar.Services.ObjectIntegration;
using WPFApostar.ViewModel;
using System.IO;
using System.Windows;

namespace WPFApostar.UserControls.Paquetes
{
    /// <summary>
    /// Lógica de interacción para SelectOperadorUC.xaml
    /// </summary>
    public partial class SelectOperadorUC : UserControl
    {
        private ObservableCollection<OperatorsViewModel> LstSelectOperators;
        Transaction Transaction;
        CollectionViewSource view = new CollectionViewSource();
        OperatorsViewModel SelectedOperator = new OperatorsViewModel();
        private TimerGeneric timer;

        public SelectOperadorUC(Transaction transaction)
        {
            InitializeComponent();
            ActivateTimer();
            Transaction = transaction;
            LstSelectOperators = new ObservableCollection<OperatorsViewModel>();
            LoadOperatorsAsync();
        }

        private void HandleButtonAction(object sender, EventArgs e, UserControlView view)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(view);
        }

        private void BtnAtras_TouchDown(object sender, EventArgs e)
        {
            HandleButtonAction(sender, e, UserControlView.Menu);
        }

        private void BtnAtras_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HandleButtonAction(sender, e, UserControlView.Menu);
        }

        private void BtnInicio_TouchDown(object sender, EventArgs e)
        {
            HandleButtonAction(sender, e, UserControlView.Main);
        }

        private void BtnInicio_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HandleButtonAction(sender, e, UserControlView.Main);
        }

        private void BtnCancelar_TouchDown(object sender, EventArgs e)
        {
            HandleButtonAction(sender, e, UserControlView.Menu);
        }

        private void BtnCancelar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HandleButtonAction(sender, e, UserControlView.Menu);
        }

        private void Btn_SelectOperator(object sender, EventArgs e)
        {
            try
            {
                var control = sender as FrameworkElement;
                HandleSelectOperatorAction(control);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
                Utilities.ShowModal("Error al seleccionar el operador", EModalType.Error);
            }
        }

        private void HandleSelectOperatorAction(FrameworkElement control)
        {
            try
            {
                DisableView();
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);

                var data = control as ListViewItem;
                if (data != null)
                {
                    SelectedOperator = (OperatorsViewModel)data.Content;

                    Transaction.SelectOperator = new OperatorSelected();
                    Transaction.Operador = SelectedOperator.DesOperator;
                    Transaction.IdProduct = SelectedOperator.IdOperator;

                    GetOperators(Transaction);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
                Utilities.ShowModal("Error al seleccionar el operador", EModalType.Error);
                EnableView();
            }
        }

        public void GetOperators(Transaction Transaction)
        {
            try
            {
                Task.Run(async () =>
                {
                    RequestConsultPaquetes Request = new RequestConsultPaquetes();
                    Request.Ubicacion = Utilities.GetConfiguration("UbicacionMaquina");
                    Request.Transacciondistribuidorid = Transaction.IdTransactionAPi;
                    Request.Codigosubproducto = Transaction.IdProduct;

                    var Respuesta = await AdminPayPlus.ApiIntegration.ConsultPaquetes(Request);

                    if (Respuesta != null)
                    {
                        if (Respuesta.Estado == true)
                        {
                            Transaction.responseConsultPaquetes = Respuesta;
                            var x = JsonConvert.SerializeObject(Transaction.responseConsultPaquetes);
                            Utilities.CloseModal();
                            AdminPayPlus.SaveLog("MenuUC", " GetOperators metodo ConsultPaquetes Response true", "OK", x, null);
                            Utilities.navigator.Navigate(UserControlView.SelectPaquete, Transaction, Respuesta);
                        }
                        else
                        {
                            Utilities.CloseModal();
                            Utilities.ShowModal("No se pudo obtener los operadores por favor intenta nuevamente", EModalType.Error);
                            AdminPayPlus.SaveLog("MenuUC", "Saliendo de la ejecucion GetOperators if not true", "OK", Respuesta.Estado.ToString(), null);
                            Utilities.navigator.Navigate(UserControlView.Menu);
                        }
                    }
                    else
                    {
                        Utilities.CloseModal();
                        Utilities.ShowModal("No se pudo obtener los operadores por favor intenta nuevamente", EModalType.Error);
                        AdminPayPlus.SaveLog("MenuUC", "Saliendo de la ejecucion GetOperators if not true", "OK", Respuesta.Estado.ToString(), null);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }
                });

                Utilities.ShowModal("Estamos Consultado la informacion un momento porfavor", EModalType.Preload);
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }

        private async Task LoadOperatorsAsync()
        {
            string PaqueteDisponible = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images", "Paquetes");
            var lista = Transaction.responseConsultSubproductosPaquetes;

            if (lista.Listadosubproductospaquetes.Paquete.Count > 0)
            {
                foreach (var Paquete in lista.Listadosubproductospaquetes.Paquete)
                {
                    string PathL = Path.Combine(PaqueteDisponible, Paquete.Nombre + ".png");

                    if (File.Exists(PathL))
                    {
                        LstSelectOperators.Add(new OperatorsViewModel
                        {
                            ImageData = Utilities.LoadImageFromFile(new Uri(Path.Combine(PaqueteDisponible, Paquete.Nombre + ".png"), UriKind.Relative)),
                            DesOperator = Paquete.Nombre,
                            IdOperator = Paquete.Codigo,
                        });
                    }
                }
            }

            Dispatcher.Invoke(() =>
            {
                view.Source = LstSelectOperators;
                this.DataContext = view;
            });
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
