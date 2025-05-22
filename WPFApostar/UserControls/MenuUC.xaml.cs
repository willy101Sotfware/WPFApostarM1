using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFApostar.Domain;
using WPFApostar.Domain.UseFull;
using WPFApostar.Domain.ApiService.Models;
using WPFApostar.Domain.UIServices.ObjectIntegration;

namespace WPFApostar.UserControls
{
    /// <summary>
    /// Lógica de interacción para MenuUC.xaml
    /// </summary>
    public partial class MenuUC : UserControl
    {
        private Transaction Transaction;
        private TimerGeneric timer;

        public MenuUC()
        {
            InitializeComponent();
            Transaction = new Transaction();
            ActivateTimer();
            Transaction.productSelect = new ProductSelect();
        }

        private void Btn_JugarPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as FrameworkElement;
            HandleJugarAction(control);
        }

        private void Btn_ChancePreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as FrameworkElement;
            HandleChanceAction(control);
        }

        private void Btn_RecaudoPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as FrameworkElement;
            HandleRecaudoAction(control);
        }

        private void Btn_RechargePreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as FrameworkElement;
            HandleRechargeAction(control);
        }

        private void HandleJugarAction(FrameworkElement control)
        {
            try
            {
                // Efecto visual al presionar (opcional)
                if (control != null)
                {
                    control.Opacity = 0.8;
                }
                
                AdminPayPlus.SaveLog("MenuUC", "entrando al metodo HandleJugarAction", "OK", "", null);

                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
                Utilities.navigator.Navigate(UserControlView.Login, Transaction);
            }
            catch(Exception ex)
            {
                Utilities.CloseModal();
                Utilities.ShowModal("los servicios de Apostar no estan disponibles, intenta nuevamente", EModalType.Error);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }

        private void HandleChanceAction(FrameworkElement control)
        {
            try
            {
                // Efecto visual al presionar (opcional)
                if (control != null)
                {
                    control.Opacity = 0.8;
                }
                
                DisableView();

                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);

                Task.Run(() =>
                {
                    RequestSubproducts Request = new RequestSubproducts();

                    Request.Ubicacion = Utilities.GetConfiguration("UbicacionMaquina");
                    var Respuesta = AdminPayPlus.ApiIntegration.GetProductsChance(Request);
                  

                    Utilities.CloseModal();

                    if (Respuesta != null)
                    {
                        if (Respuesta.Estado == true)
                        {

                            foreach (var item in Respuesta.Listadosubproductos.Subproducto)
                            {
                                if (item.Nombre == "CHANCE")
                                {
                                    Transaction.productSelect.idField = item.Id;
                                    Transaction.productSelect.nombreField = item.Nombre;
                                    Transaction.productSelect.codigoField = item.Codigo;
                                    Transaction.productSelect.CodigoServicio = item.Codigoservicio;
                                }
                            }

                            GetTypeChance();

                            //   Utilities.navigator.Navigate(UserControlView.Form, Transaction);

                        }
                        else
                        {
                            Utilities.ShowModal("los servicios de Apostar no estan disponibles, intenta nuevamente", EModalType.Error);
                            Utilities.navigator.Navigate(UserControlView.Menu);

                        }
                    }

                });

                Utilities.ShowModal("Estamos Validando los Servicios un momento por favor", EModalType.Preload);
            }
            catch (Exception ex)
            {
                Utilities.CloseModal();
                Utilities.ShowModal("los servicios de Apostar no estan disponibles, intenta nuevamente", EModalType.Error);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }

        // Este método ya no es necesario porque ahora usamos HandleChanceAction

        public void GetTypeChance()
        {
            try
            {
                IdProducto Request = new IdProducto();

                Request.Ubicacion =  Utilities.GetConfiguration("UbicacionMaquina");
                Request.transaccion = Transaction.IdTransactionAPi;
                Request.Id = Transaction.productSelect.idField;

                Task.Run(() =>
                {

                    var Response = AdminPayPlus.ApiIntegration.TypeChance(Request);

                    Utilities.CloseModal();



                    if (Response != null)
                    {
                        if (Response.estadoField)
                        {

                            Transaction.TypeChance = Response.listadotipochanceField;
                            Utilities.navigator.Navigate(UserControlView.Dia, Transaction);

                        }
                        else
                        {
                            Utilities.ShowModal("los servicios de Apostar no estan disponibles, intenta nuevamente", EModalType.Error);
                            Utilities.navigator.Navigate(UserControlView.Menu);
                        }
                    }
                    else
                    {
                        Utilities.ShowModal("los servicios de Apostar no estan disponibles, intenta nuevamente", EModalType.Error);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                    }
                });

                Utilities.ShowModal("Estamos validando información un momento por favor", EModalType.Preload);


            }
            catch (Exception ex)
            {
                Utilities.CloseModal();
                Utilities.ShowModal("los servicios de Apostar no estan disponibles, intenta nuevamente", EModalType.Error);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }

        private void HandleRecaudoAction(FrameworkElement control)
        {
            try
            {
                // Efecto visual al presionar (opcional)
                if (control != null)
                {
                    control.Opacity = 0.8;
                }
                
                DisableView();

                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);

                Task.Run(() =>
                {
                    RequestGetRecaudos Request = new RequestGetRecaudos();

                    Request.IdTrx = Transaction.IdTransactionAPi;
                    Request.codigo = Convert.ToInt32(Utilities.GetConfigData("IdDepartamento"));

                    var Respuesta = AdminPayPlus.ApiIntegration.GetRecaudos(Request);

                    Utilities.CloseModal();

                    if (Respuesta != null)
                    {
                        if (Respuesta.Estado == true)
                        {
                            Transaction.listadorecaudosField = Respuesta.Listadorecaudos.Recaudo;
                            Utilities.navigator.Navigate(UserControlView.TypeRecaudo, Transaction);
                        }
                        else
                        {
                            Utilities.ShowModal("los servicios de Apostar no estan disponibles, intenta nuevamente", EModalType.Error);
                            Utilities.navigator.Navigate(UserControlView.Menu);

                        }
                    }

                });

                Utilities.ShowModal("Estamos Validando los Servicios un momento por favor", EModalType.Preload);
            }
            catch (Exception ex)
            {
                Utilities.CloseModal();
                Utilities.ShowModal("los servicios de Apostar no estan disponibles, intenta nuevamente", EModalType.Error);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }


        private void HandleRechargeAction(FrameworkElement control)
        {
            try
            {
                // Efecto visual al presionar (opcional)
                if (control != null)
                {
                    control.Opacity = 0.8;
                }
                
                DisableView();

                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);

                AdminPayPlus.SaveLog("MenuUC", "Entrando a la opcion HandleRechargeAction", "OK", "", null);


                Task.Run(async () =>
                {
                    RequestConsultSubproductosPaquetes Request = new RequestConsultSubproductosPaquetes();

                    Request.Ubicacion =  Utilities.GetConfiguration("UbicacionMaquina");
                    Request.Transacciondistribuidorid = Transaction.IdTransactionAPi;


                    var Respuesta = await AdminPayPlus.ApiIntegration.ConsultSubproductosPaquetes(Request);

                    Utilities.CloseModal();

                    if (Respuesta != null)
                    {
                        if (Respuesta.Estado == true)
                        {
                            Transaction.responseConsultSubproductosPaquetes = Respuesta;

                            Utilities.navigator.Navigate(UserControlView.SelectOperador, Transaction);
                        }
                        else
                        {
                            Utilities.ShowModal("los servicios de Apostar no estan disponibles, intenta nuevamente", EModalType.Error);
                            Utilities.navigator.Navigate(UserControlView.Menu);

                        }
                    }
                    else
                    {
                        Utilities.ShowModal("los servicios de Apostar no estan disponibles, intenta nuevamente", EModalType.Error);
                        Utilities.navigator.Navigate(UserControlView.Menu);

                    }

                });

                Utilities.ShowModal("Estamos Validando los Servicios un momento por favor", EModalType.Preload);




            }
            catch (Exception ex)
            {
                Utilities.CloseModal();
                Utilities.ShowModal("los servicios de Apostar no estan disponibles, intenta nuevamente", EModalType.Error);
                Utilities.navigator.Navigate(UserControlView.Menu);
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
                    tbTimer.Text = "00:30";
                    timer = new TimerGeneric(tbTimer.Text);
                    timer.CallBackClose = response =>
                    {
                        SetCallBacksNull();
                        timer.CallBackStop?.Invoke(1);
                        Utilities.navigator.Navigate(UserControlView.Main);
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
