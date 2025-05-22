using Newtonsoft.Json;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFApostar.Domain;
using WPFApostar.Domain.UseFull;
using WPFApostar.Domain.ApiService.Models;
using WPFApostar.Domain.UIServices.ObjectIntegration;

namespace WPFApostar.UserControls.Chance
{
    /// <summary>
    /// Lógica de interacción para FormUC.xaml
    /// </summary>
    public partial class FormUC : UserControl
    {

        private Transaction Transaction;
        private TimerGeneric timer;
        private string numeroDocumento;
        public string Validar = "Debes Completar los Campos Requeridos: \n";


        public FormUC(Transaction Ts)
        {
            InitializeComponent();
            Transaction = Ts;
            this.DataContext = Transaction;
            //    ActivateTimer();
            TypeDocument.SelectedIndex = 0;

        }

        private void BtnCancelar_TouchDown(object sender, TouchEventArgs e)
        {
            //       SetCallBacksNull();
            //      timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void BtnContinuar_TouchDown(object sender, TouchEventArgs e)
        {
            ValidateData();
            SaveUser();
        }

        private void Btn_CedulaTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(false, (TextBox)sender, this, 250, 940);
        }

        private void Btn_DocumentoTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(true, (TextBox)sender, this, 630, 740);

        }

        private void Btn_CelTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(true, (TextBox)sender, this, 500, 1250);

        }

        private void Btn_EmailTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(false, (TextBox)sender, this, 300, 1450);

        }

        public async void ValidateData()
        {

            if (TxtCorreo.Text == "" || TxtNombre.Text == "" || ((ComboBoxItem)TypeDocument.SelectedItem) == null || ((ComboBoxItem)Dia.SelectedItem) == null || ((ComboBoxItem)Mes.SelectedItem) == null || ((ComboBoxItem)Año.SelectedItem) == null || TxtCedula.Text == "" || TxtCelular.Text == "")
            {
                if (TxtNombre.Text == "")
                {
                    Validar += string.Concat("Nombre \n");
                }
                if (((ComboBoxItem)TypeDocument.SelectedItem) == null)
                {
                    Validar += string.Concat("Tipo de Documento \n");
                };
                if (((ComboBoxItem)Dia.SelectedItem) == null)
                {
                    Validar += string.Concat("Dia de Nacimiento \n");
                };
                if (((ComboBoxItem)Mes.SelectedItem) == null)
                {
                    Validar += string.Concat("Mes de Nacimiento \n");
                };
                if (((ComboBoxItem)Año.SelectedItem) == null)
                {
                    Validar += string.Concat("Año de Nacimiento \n");
                };
                if (TxtCedula.Text == "")
                {
                    Validar += string.Concat("Cedula \n");
                }
                if (TxtCelular.Text == "")
                {
                    Validar += string.Concat("Celular \n");
                } 
                if (TxtCorreo.Text == "")
                {
                    Validar += string.Concat("Correo \n");
                }

                Utilities.ShowModal(Validar, EModalType.Error);
                Validar = "Debes Completar los Campos Requeridos: \n";
            }
            else
            {



                Transaction.payer = new DataModel.PAYER();

                int day = Convert.ToInt32(((ComboBoxItem)Dia.SelectedItem).Content);
                int month = Convert.ToInt32(((ComboBoxItem)Mes.SelectedItem).Tag);
                int year = Convert.ToInt32(((ComboBoxItem)Año.SelectedItem).Content);

                string birthdayString = $"{day:D2}/{month:D2}/{year}";

                Transaction.Name = TxtNombre.Text;
                Transaction.Document = TxtCedula.Text;
                Transaction.payer.EMAIL = TxtCorreo.Text;
                Transaction.payer.PHONE = Convert.ToDecimal(TxtCelular.Text);
                Transaction.payer.NAME= TxtNombre.Text;
                Transaction.payer.IDENTIFICATION = TxtCedula.Text;
                Transaction.payer.BIRTHDAY = birthdayString;
                Transaction.payer.IDENTIFICATION = TxtCedula.Text;
                Transaction.payer.IDENTIFICATION_TYPE = 1;
                Transaction.payer.STATE = Transaction.statePaySuccess;
                

                if (((ComboBoxItem)TypeDocument.SelectedItem).Content.ToString() != null)
                {
                    if (((ComboBoxItem)TypeDocument.SelectedItem).Content.ToString() == "Cedula de ciudadania")
                    {
                        Transaction.TypeDocument = "CC";
                    }
                }

                //           SetCallBacksNull();
                //       timer.CallBackStop?.Invoke(1);


                var respuesta = await AdminPayPlus.InsertRecord(Transaction);

                if (respuesta != null)
                {

                    GetTypeChance();

                }
                else
                {
                    Utilities.ShowModal("No es posible continuar, intente nuevamente", EModalType.Error);


                }

             

            }

        }

        private void BtnCOntinuar_MouseDown(object sender, MouseButtonEventArgs e)
        {
          //  ValidateData();
        }

        public void SaveUser()
        {

            try
            {

                RequestSavePayer Request = new RequestSavePayer();

                Request.identificacion = Convert.ToInt32(TxtCedula.Text);
                Request.NombreUser = TxtNombre.Text;
                Request.Correo = TxtCorreo.Text;
                Request.transaccion = 0;

                AdminPayPlus.SaveLog("Request SavePayer: " + JsonConvert.SerializeObject(Request));


                Task.Run(() =>
                {

                    var Response = AdminPayPlus.ApiIntegration.SavePayerChance(Request);

                    Transaction.IdUser = Response.Tercero.Id;

                    AdminPayPlus.SaveLog($"Response SavePayer:  {Response}");

                });
            }
            catch(Exception ex)
            {

            }

        }

        public void GetTypeChance()
        {
            try
            {
                IdProducto Request = new IdProducto();

                Request.transaccion = Transaction.IdTransactionAPi;
                Request.Id = Transaction.productSelect.idField;

                Task.Run(() =>
                {

                    var Response = AdminPayPlus.ApiIntegration.TypeChance(Request);

                    Utilities.CloseModal();

                 

                    if (Response != null)
                    {
                        if(Response.estadoField)
                        {

                            Transaction.TypeChance = Response.listadotipochanceField;
                            Utilities.navigator.Navigate(UserControlView.Dia, Transaction);

                        }
                        else
                        {
                            Utilities.ShowModal("En estos momentos los servicios de apostar no estan disponibles", EModalType.Error);
                        }
                    }
                    else
                    {
                        Utilities.ShowModal("En estos momentos los servicios de apostar no estan disponibles",EModalType.Error);
                    }


                });

                Utilities.ShowModal("Estamos validando información un momento por favor",EModalType.Preload);


            }
            catch(Exception ex)
            {
                Utilities.CloseModal();
            }
        }

        private void BtnFunction_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {


                if(si.Visibility == Visibility.Hidden)
                {
                    BtnContinue.Visibility = Visibility.Visible;
                    si.Visibility = Visibility.Visible;
                }
                else
                {
                    BtnContinue.Visibility = Visibility.Hidden;
                    si.Visibility = Visibility.Hidden;

                }

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

        private async void BtnConsultPayer_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {

                if (!string.IsNullOrEmpty(TxtCedula.Text))
                {
                    numeroDocumento = TxtCedula.Text;

                    var response = await ValidatePayer(numeroDocumento);

                    if (response != null)
                    {

                        TxtNombre.Text = response.data[0].name;

                        TxtCelular.Text = response.data[0].phone;

                        if (response.data[0].identification_Type == 1)
                        {
                            string cedula = "Cedula de ciudadania";
                            TypeDocument.Text = cedula;
                        }

                        if (response.data[0].email != null)
                        {
                            TxtCorreo.Text = response.data[0].email;
                        }

                        if (response.data[0].birthday != null)
                        {

                            string[] nacimiento = response.data[0].birthday.Split('/');
                            if (nacimiento.Length == 3)
                            {
                                // Si la fecha tiene tres partes (día, mes y año), intentamos convertir cada parte a un número entero
                                if (int.TryParse(nacimiento[0], out int dia) &&
                                    int.TryParse(nacimiento[1], out int mes) &&
                                    int.TryParse(nacimiento[2], out int año))
                                {
                                    // Si la conversión es exitosa, autocompletamos los campos correspondientes
                                    // con los valores obtenidos de la fecha de cumpleaños
                                    Dia.SelectedIndex = dia - 1;
                                    Mes.SelectedIndex = mes - 1; // Restamos 1 ya que los índices de los ComboBox comienzan en 
                                    Año.Text = año.ToString();
                                }


                            }


                        }
                    }
                    else
                    {
                        Utilities.ShowModal("En este momento no te encuentras registrado en la base de datos, es necesario ingresar los datos de forma manual.", EModalType.Error);
                    }
                }
                else
                {
                    Utilities.ShowModal("Señor Usuario por favor digite su numero de Documento", EModalType.Error);
                }


            }
            catch(Exception ex)
            {

            }
        }

        private async Task<ResponseValidatePayer> ValidatePayer(string numero)
        {
            try
            {

                Transaction.payer = new DataModel.PAYER();
                Transaction.payer.IDENTIFICATION = numero;

                var respuesta = await AdminPayPlus.ValidatePayer(Transaction);

                Utilities.CloseModal();

                if (respuesta != null)
                {
                    return respuesta;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "ValidatePayer", ex, ex.ToString());
            }
            return null;
        }

        private async void BtnConsultPayer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                if (!string.IsNullOrEmpty(TxtCedula.Text))
                {
                    numeroDocumento = TxtCedula.Text;

                    var response = await ValidatePayer(numeroDocumento);

                    if (response != null)
                    {

                        TxtNombre.Text = response.data[0].name;

                        TxtCelular.Text = response.data[0].phone;

                        if (response.data[0].identification_Type == 1)
                        {
                            string cedula = "Cedula de ciudadania";
                            TypeDocument.Text = cedula;
                        }

                        if (response.data[0].email != null)
                        {
                            TxtCorreo.Text = response.data[0].email;
                        }

                        if (response.data[0].birthday != null)
                        {

                            string[] nacimiento = response.data[0].birthday.Split('/');
                            if (nacimiento.Length == 3)
                            {
                                // Si la fecha tiene tres partes (día, mes y año), intentamos convertir cada parte a un número entero
                                if (int.TryParse(nacimiento[0], out int dia) &&
                                    int.TryParse(nacimiento[1], out int mes) &&
                                    int.TryParse(nacimiento[2], out int año))
                                {
                                    // Si la conversión es exitosa, autocompletamos los campos correspondientes
                                    // con los valores obtenidos de la fecha de cumpleaños
                                    Dia.SelectedIndex = dia - 1;
                                    Mes.SelectedIndex = mes - 1; // Restamos 1 ya que los índices de los ComboBox comienzan en 
                                    Año.Text = año.ToString();
                                }


                            }


                        }
                    }
                    else
                    {
                        Utilities.ShowModal("En este momento no te encuentras registrado en la base de datos, es necesario ingresar los datos de forma manual.", EModalType.Error);
                    }
                }
                else
                {
                    Utilities.ShowModal("Señor Usuario por favor digite su numero de Documento", EModalType.Error);
                }


            }
            catch (Exception ex)
            {

            }
        }
    }
}
