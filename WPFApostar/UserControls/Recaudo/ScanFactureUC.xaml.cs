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
    /// Lógica de interacción para ScanFactureUC.xaml
    /// </summary>
    public partial class ScanFactureUC : UserControl
    {
        private Transaction Transaction;
        private TimerGeneric timer;

        public ScanFactureUC(Transaction Ts)
        {
            Transaction = Ts;
            this.DataContext = Transaction;
            InitializeComponent();
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

                            string referente = Reference.Substring(20, 7);
                            string codpredio = Reference.Substring(3, 13);
                            string valor = Reference.Substring(31, 7);
                            string fecha = Reference.Substring(41, 8);



                            Transaction.reference = referente;
                            Transaction.Amount = valor;
                            //Transaction.codpredial = codpredio;
                            //Transaction.fechalimite = fecha;

                            ConsultRefence();

               //             Utilities.navigator.Navigate(UserControlView.DataConsult, Transaction);

                            //        Utilities.navigator.Navigate(UserControlView.DataList, false, transaction);
                            //    Reference = String.Empty;
                         //   AdminPayPlus.controlScanner.ClosePortScanner();
                           // consultData();
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

        public void DeserealizeFacture()
        {
            try
            {

            }
            catch(Exception ex)
            {

            }
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

                        DataConsult1.valor = "FACTURA";

                        

                        LstdataConsult.Add(DataConsult1);

                    }
                    else
                    {
                        Camposm DataConsult1 = new Camposm();

                        DataConsult1.nombre = x.nombreField;
                        DataConsult1.id = x.idField;
                        DataConsult1.valor = Transaction.reference;

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

                Transaction.reference = Transaction.reference;

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

        private void BtnCancelar_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

    }
}
