using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using WPFApostar.Domain;
using WPFApostar.Domain.UseFull;
using System.IO;
using WPFApostar.ViewModel;
using Path = System.IO.Path;
using System.Collections.ObjectModel;
using WPFApostar.Domain.ApiService.Models;

namespace WPFApostar.UserControls.Recaudo
{
    /// <summary>
    /// Lógica de interacción para TypeRecaudoUC.xaml
    /// </summary>
    public partial class TypeRecaudoUC : UserControl
    {

        private Transaction Transaction;
        private TimerGeneric timer;
        private ObservableCollection<RecaudosViewModel> LstRecaudosModel;

        public TypeRecaudoUC(Transaction Ts)
        {
            InitializeComponent();
            ActivateTimer();
            LstRecaudosModel = new ObservableCollection<RecaudosViewModel>();
            Transaction = Ts;
        }

        private void Btn_OptionTouchDown(object sender, TouchEventArgs e)
        {

            try
            {

                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);

                var Img = sender as Image;

                string tag = Img.Tag.ToString();

                Transaction.tramite = tag;

                if(Transaction.tramite == "All")
                {
                    foreach (var Recaudos in Transaction.listadorecaudosField)
                    {

                        string ImagePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Recaudos", Recaudos.Codigo + ".png");

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
                }
                else
                {
                    foreach (var Recaudos in Transaction.listadorecaudosField)
                    {

                        string ImagePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Recaudos", Transaction.tramite, Recaudos.Codigo + ".png");

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
                }

     

                if (LstRecaudosModel.Count() > 0)
                {
                    Utilities.navigator.Navigate(UserControlView.SelectCompany, Transaction);
                }
                else
                {
                    Utilities.ShowModal("No hay Recaudos disponibles para este servicio,intente nuevamente por favor.", EModalType.Error);
                    Utilities.navigator.Navigate(UserControlView.TypeRecaudo, Transaction);
                }

               
            }
            catch(Exception ex)
            {

            }

        }

        private void BtnAtras_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
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
