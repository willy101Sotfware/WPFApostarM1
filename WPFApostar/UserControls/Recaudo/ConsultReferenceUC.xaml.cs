using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using WPFApostar.Classes;
using WPFApostar.Classes.UseFull;
using WPFApostar.Models;
using WPFApostar.Resources;
using WPFApostar.Services.ObjectIntegration;

namespace WPFApostar.UserControls.Recaudo
{
    /// <summary>
    /// Lógica de interacción para ConsultReferenceUC.xaml
    /// </summary>
    public partial class ConsultReferenceUC : UserControl
    {

        private Transaction Transaction;
        private TimerGeneric timer;

        public ConsultReferenceUC(Transaction Ts)
        {
            Transaction = Ts;
            Transaction.DataReference = new RecaudofieldData();

            ActivateTimer();

            InitializeComponent();

            DatoSolicitado.Text = Transaction.ParametersCompany[0].etiquetaField;
        }

        private void BtnAtras_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.TypeRecaudo,Transaction);
        }

        private void Keyboard_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                Image image = (Image)sender;
                string Tag = image.Tag.ToString();
                TxtReference.Text += Tag;
            }
            catch (Exception ex)
            {

            }
        }

        private void Btn_DeleteTouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                string val = TxtReference.Text;

                if (val.Length > 0)
                {
                    TxtReference.Text = val.Remove(val.Length - 1);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void Btn_DeleteAllTouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                TxtReference.Text = string.Empty;

            }
            catch (Exception ex)
            {

            }
        }

        private void BtnContinuar_TouchDown(object sender, TouchEventArgs e)
        {

            if (Validate())
            {
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
                ConsultRefence();
            }
            else
            {
                Utilities.ShowModal("Debes de ingresar un referente valido para continuar", EModalType.Error);
            }

        }

        private void BtnCancelar_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        public bool Validate()
        {
            try
            {

                if (!string.IsNullOrEmpty(TxtReference.Text))
                {
                    return true;
                }      

            }
            catch(Exception ex)
            {
            
            
            }

            return false;

        }

        public void ConsultRefence()
        {

            try
            {

                List<Camposm> LstdataConsult = new List<Camposm>();

                foreach (var x in Transaction.ParametersCompany)
                {
                    Camposm DataConsult = new Camposm();

                    DataConsult.id = x.idField;
                    DataConsult.nombre = x.nombreField;
                    DataConsult.valor = TxtReference.Text;

                    LstdataConsult.Add(DataConsult);
                }

                RequestConsultValue Request = new RequestConsultValue
                {

                    idDepartamento = new Iddepartamento
                    {
                        codigo = Convert.ToInt32(Utilities.GetConfiguration("IdDepartamento"))
                    },

                    Recaudo = new Services.ObjectIntegration.Recaudo
                    {
                        codigo = Transaction.Company.Codigo
                    },

                    IdTrx = Transaction.IdTransactionAPi,

                    Servicio = Transaction.IdServicioRecaudo,

                    ListadoCamposM = new Listadocamposm
                    {
                        camposM = new List<Camposm>()
                    }


                };

                Transaction.reference = TxtReference.Text;


                Task.Run(() =>
                {
                    try
                    {

                    
                        Request.ListadoCamposM.camposM = LstdataConsult;

                        var ResponseData = AdminPayPlus.ApiIntegration.ConsultValueRecaudo(Request);

                        Utilities.CloseModal();

                        if (ResponseData != null)
                        {
                            if (ResponseData.recaudoField != null)
                            {

                                if (ResponseData.estadoField)
                                {
                                //    Transaction.reference = TxtReference.Text;
                                    Transaction.DataReference = ResponseData.recaudoField;
                                    Utilities.navigator.Navigate(UserControlView.DataConsult, Transaction);
                                }
                                else
                                {
                                    Utilities.ShowModal("No es posible realizar el pago de esta factura por favor comunicate con un administrador", EModalType.Error);
                                }

                            }
                            else
                            {
                                Utilities.ShowModal("No se encontraron Registros para este referente por favor intente de nuevo", EModalType.Error);
                            }
                        }
                        else
                        {
                            Utilities.ShowModal("No se encontraron Registros para este referente por favor intente de nuevo", EModalType.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        Utilities.ShowModal("Hubo un error inesperado, por favor intenta mas tarde",EModalType.Error);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                        throw ex;

                    }

                });

                Utilities.ShowModal(MessageResource.ConsultingConinsidences,EModalType.Preload);
            }
            catch(Exception ex)
            {

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
