using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
using WPFApostar.Services.ObjectIntegration;

namespace WPFApostar.UserControls.Recaudo
{
    /// <summary>
    /// Lógica de interacción para ConsultForm3UC.xaml
    /// </summary>
    public partial class ConsultForm3UC : UserControl
    {

        private Transaction Transaction;
        private TimerGeneric timer;

        public ConsultForm3UC(Transaction Ts)
        {
            InitializeComponent();
            Transaction = Ts;
            Transaction.DataReference = new RecaudofieldData();

            ActivateTimer();
            ActivateScanner();
            DatoSolicitado.Text = Transaction.ParametersCompany[1].etiquetaField;
        }

        private void SelectTouchDown(object sender, TouchEventArgs e)
        {
            try
            {

                if(CheckContratoSi.Visibility == Visibility.Hidden)
                {
                    CheckContratoSi.Visibility = Visibility.Visible;
                    CheckFacturaSi.Visibility = Visibility.Hidden;
                }
                else
                {
                    CheckContratoSi.Visibility = Visibility.Hidden;
                    CheckFacturaSi.Visibility = Visibility.Visible;
                }

            }
            catch(Exception ex)
            {

            }
        }

        private void SelectFacturaTouchDown(object sender, TouchEventArgs e)
        {
            try
            {

                if (CheckFacturaSi.Visibility == Visibility.Hidden)
                {
                    CheckContratoSi.Visibility = Visibility.Hidden;
                    CheckFacturaSi.Visibility = Visibility.Visible;
                }
                else
                {
                    CheckContratoSi.Visibility = Visibility.Visible;
                    CheckFacturaSi.Visibility = Visibility.Hidden;
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void BtnAtras_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.TypeRecaudo, Transaction);
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

        public void ConsultRefence()
        {

            try
            {

                List<Camposm> LstdataConsult = new List<Camposm>();



                foreach (var x in Transaction.ParametersCompany)
                {
                    if (x.nombreField == "SELECTOR")
                    {
                        Camposm DataConsult1 = new Camposm();

                        DataConsult1.nombre = x.nombreField;
                        DataConsult1.id = x.idField;

                        if (CheckContratoSi.Visibility == Visibility.Visible)
                        {
                            DataConsult1.valor = "CONTRATO";
                        }
                        else
                        {
                            DataConsult1.valor = "FACTURA";

                        }

                        LstdataConsult.Add(DataConsult1);

                    }
                    else
                    {
                        Camposm DataConsult1 = new Camposm();

                        DataConsult1.nombre = x.nombreField;
                        DataConsult1.id = x.idField;
                        DataConsult1.valor = TxtReference.Text;

                        LstdataConsult.Add(DataConsult1);

                    }

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

                Request.ListadoCamposM.camposM = LstdataConsult;

                Task.Run(() =>
                {
                    try
                    {



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
                        Utilities.ShowModal("Hubo un error inesperado, por favor intenta mas tarde", EModalType.Error);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                        throw ex;

                    }

                });

                Utilities.ShowModal(MessageResource.ConsultingConinsidences, EModalType.Preload);
            }
            catch (Exception ex)
            {

            }


        }

        private void ActivateScanner()
        {

            try
            {
                AdminPayPlus.controlScanner.callbackScanner = Reference =>
                {
                    if (!string.IsNullOrEmpty(Reference))
                    {
                        Dispatcher.BeginInvoke((Action)delegate
                        {

                            SetCallBacksNull();
                            timer.CallBackStop?.Invoke(1);
                            SerealizerScan(Reference);

                        });



                    }
                };

                AdminPayPlus.controlScanner.callbackErrorScanner = Error =>
                {
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Utilities.ShowModal(Error, EModalType.Error);
                        ActivateScanner();
                    });
                };

                AdminPayPlus.controlScanner.flagScanner = 0;
                AdminPayPlus.controlScanner.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public void SerealizerScan(string Reference)
        {
            try
            {

                string referente = "";

                switch (Transaction.Company.Codigo)
                {

                    case 24:
                         referente = Reference.Substring(23, 9);
                        break;
                    case 2:
                        referente = Reference.Substring(28,6);
                    break;
                    case 8:
                        referente = Reference.Substring(21, 9);
                    break;
                    case 46:
                        referente = Reference.Substring(20, 10);
                    break;



                };

                Transaction.reference = referente;
                TxtReference.Text = referente;

            //    ConsultRefence();

                //             Utilities.navigator.Navigate(UserControlView.DataConsult, Transaction);
            }
            catch (Exception ex)
            {

            }
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
            catch (Exception ex)
            {


            }

            return false;

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
