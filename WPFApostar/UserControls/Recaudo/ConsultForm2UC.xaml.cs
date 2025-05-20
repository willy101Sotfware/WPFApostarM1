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
    /// Lógica de interacción para ConsultForm2UC.xaml
    /// </summary>
    public partial class ConsultForm2UC : UserControl
    {

        public Transaction Transaction;

        private TimerGeneric timer;

        public ConsultForm2UC(Transaction Ts)
        {
            InitializeComponent();
            Transaction = Ts;
            ActivateTimer();
            OrganizeForm();
        }

        public void OrganizeForm()
        {

            try
            {

                switch (Transaction.ParametersCompany.Count())
                {
                  
                    case 6:
                        Data1.Text = Transaction.ParametersCompany[0].etiquetaField;
                        Data2.Text = Transaction.ParametersCompany[1].etiquetaField;
                        Data3.Text = Transaction.ParametersCompany[2].etiquetaField;
                        Data4.Text = Transaction.ParametersCompany[3].etiquetaField;
                        Data5.Text = Transaction.ParametersCompany[4].etiquetaField;
                        Data6.Text = Transaction.ParametersCompany[5].etiquetaField;
                        Data1.Tag = Transaction.ParametersCompany[0].tipodatoField;
                        Data2.Tag = Transaction.ParametersCompany[1].tipodatoField;
                        Data3.Tag = Transaction.ParametersCompany[2].tipodatoField;
                        Data4.Tag = Transaction.ParametersCompany[3].tipodatoField;
                        Data5.Tag = Transaction.ParametersCompany[4].tipodatoField;
                        Data6.Tag = Transaction.ParametersCompany[5].tipodatoField;
                        Campo1.Tag = Transaction.ParametersCompany[0].etiquetaField;
                        Campo2.Tag = Transaction.ParametersCompany[1].etiquetaField;
                        Campo3.Tag = Transaction.ParametersCompany[2].etiquetaField;
                        Campo4.Tag = Transaction.ParametersCompany[3].etiquetaField;
                        Campo5.Tag = Transaction.ParametersCompany[4].etiquetaField;
                        Campo6.Tag = Transaction.ParametersCompany[5].etiquetaField;
                        break;
                    default:
                        Utilities.ShowModal("Hubo un error al momento de consultar la información,por favor comunicate con un administrador", EModalType.Error);
                        Utilities.navigator.Navigate(UserControlView.Menu);
                        break;


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

        private void BtnCancelar_TouchDown(object sender, TouchEventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);
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
                Utilities.ShowModal("Por favor verifica que toda la información este correcta", EModalType.Error);
            }
        }

        public bool Validate()
        {
            try
            {

                switch (Transaction.ParametersCompany.Count())
                {


                    case 6:
                        if (!string.IsNullOrEmpty(Campo1.Text) && !string.IsNullOrEmpty(Campo2.Text) && !string.IsNullOrEmpty(Campo3.Text) && !string.IsNullOrEmpty(Campo4.Text) && !string.IsNullOrEmpty(Campo5.Text) && !string.IsNullOrEmpty(Campo6.Text))
                        {
                            return true;
                        }
                        break;
                }

            }
            catch (Exception ex)
            {

            }

            return false;
        }

        public void ConsultRefence()
        {

            try
            {

                List<DataReq> CamposR = new List<DataReq>();

                if (!string.IsNullOrEmpty(Campo1.Text))
                {
                    DataReq data1 = new DataReq()
                    {
                        Etiqueta = Campo1.Tag.ToString(),
                        Dato = Campo1.Text
                    };

                    CamposR.Add(data1);
                }
                if (!string.IsNullOrEmpty(Campo2.Text))
                {
                    DataReq data2 = new DataReq()
                    {
                        Etiqueta = Campo2.Tag.ToString(),
                        Dato = Campo2.Text
                    };

                    CamposR.Add(data2);
                }
                if (!string.IsNullOrEmpty(Campo3.Text))
                {
                    DataReq data3 = new DataReq()
                    {
                        Etiqueta = Campo3.Tag.ToString(),
                        Dato = Campo3.Text
                    };

                    CamposR.Add(data3);
                }
                if (!string.IsNullOrEmpty(Campo4.Text))
                {
                    DataReq data4 = new DataReq()
                    {
                        Etiqueta = Campo4.Tag.ToString(),
                        Dato = Campo4.Text
                    };

                    CamposR.Add(data4);
                }
                if (!string.IsNullOrEmpty(Campo5.Text))
                {
                    DataReq data5 = new DataReq()
                    {
                        Etiqueta = Campo5.Tag.ToString(),
                        Dato = Campo5.Text
                    };

                    CamposR.Add(data5);

                }

                if (!string.IsNullOrEmpty(Campo5.Text))
                {
                    DataReq data6 = new DataReq()
                    {
                        Etiqueta = Campo6.Tag.ToString(),
                        Dato = Campo6.Text
                    };

                    CamposR.Add(data6);

                }

                List<Camposm> LstdataConsult = new List<Camposm>();

                foreach (var x in Transaction.ParametersCompany)
                {
                    Camposm DataConsult = new Camposm();

                    DataConsult.id = x.idField;
                    DataConsult.nombre = x.nombreField;

                    foreach (var y in CamposR)
                    {
                        if (!string.IsNullOrEmpty(y.Dato) && x.etiquetaField == y.Etiqueta)
                        {
                            DataConsult.valor = y.Dato;
                        }
                    }

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

                //  Transaction.reference = TxtReference.Text;


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


        private void ActivateTimer()
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    tbTimer.Text = "01:59";
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

        private void KeyboardC6_TouchDown(object sender, TouchEventArgs e)
        {
            if (Data6.Tag.ToString() == "NUMERICO" || Data6.Tag.ToString() == "VALOR" || Data6.Tag.ToString() == "CEDULA")
            {
                Utilities.OpenKeyboard(true, (TextBox)sender, this, 460, 1500);
            }
            else
            {
                Utilities.OpenKeyboard(false, (TextBox)sender, this, 280, 1500);
            }
        }

        private void KeyboardC5_TouchDown(object sender, TouchEventArgs e)
        {
            if (Data5.Tag.ToString() == "NUMERICO" || Data5.Tag.ToString() == "VALOR")
            {
                Utilities.OpenKeyboard(true, (TextBox)sender, this, 460, 1500);
            }
            else
            {
                Utilities.OpenKeyboard(false, (TextBox)sender, this, 280, 1500);
            }
        }

        private void KeyboardC4_TouchDown(object sender, TouchEventArgs e)
        {
            if (Data4.Tag.ToString() == "NUMERICO" || Data4.Tag.ToString() == "VALOR")
            {
                Utilities.OpenKeyboard(true, (TextBox)sender, this, 460, 1270);

            }
            else
            {
                Utilities.OpenKeyboard(false, (TextBox)sender, this, 280, 1270);

            }
        }

        private void KeyboardC3_TouchDown(object sender, TouchEventArgs e)
        {
            if (Data3.Tag.ToString() == "NUMERICO" || Data3.Tag.ToString() == "VALOR")
            {
                Utilities.OpenKeyboard(true, (TextBox)sender, this, 460, 1020);

            }
            else
            {
                Utilities.OpenKeyboard(false, (TextBox)sender, this, 280, 1020);

            }
        }

        private void KeyboardC2_TouchDown(object sender, TouchEventArgs e)
        {
            if (Data2.Tag.ToString() == "NUMERICO" || Data2.Tag.ToString() == "VALOR")
            {
                Utilities.OpenKeyboard(true, (TextBox)sender, this, 460, 810);

            }
            else
            {
                Utilities.OpenKeyboard(false, (TextBox)sender, this, 280, 810);

            }
        }

        private void KeyboardC1_TouchDown(object sender, TouchEventArgs e)
        {
            if (Data1.Tag.ToString() == "NUMERICO" || Data1.Tag.ToString() == "VALOR")
            {
                Utilities.OpenKeyboard(true, (TextBox)sender, this, 460, 610);

            }
            else
            {
                Utilities.OpenKeyboard(false, (TextBox)sender, this, 280, 610);

            }

        }
    

    
    }

    //public class DataReq
    //{
    //    public string Etiqueta { get; set; }

    //    public string Dato { get; set; }

    //}
}
