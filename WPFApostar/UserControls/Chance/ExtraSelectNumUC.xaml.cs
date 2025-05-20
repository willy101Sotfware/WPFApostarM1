using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFApostar.Classes;
using WPFApostar.Models;
using WPFApostar.Services.ObjectIntegration;

namespace WPFApostar.UserControls.Chance
{
    /// <summary>
    /// Lógica de interacción para ExtraSelectNumUC.xaml
    /// </summary>
    public partial class ExtraSelectNumUC : UserControl
    {

        Transaction Transaction;
        public string Pos1 = "";
        public string Pos2 = "";
        public string Pos3 = "";
        public string Pos4 = "";
        public bool txtNum1 = false;
        public bool txtNum2 = false;
        public bool txtNum3 = false;
        public bool txtNum4 = false;
        public bool txtDerecho = false;
        public bool txtCombinado = false;
        public bool txtCifra = false;
        public bool txtCuna = false;
        public int ValorGanarNumeroDerecho = 0;
        public int ValorGanarNumerocuña = 0;
        public int ValorGanarNumeroCombinado = 0;
        public int ValorGanarNumeroCifra = 0;
        public string Numero = "";
        public int cifra = 4;

        public ExtraSelectNumUC(Transaction ts)
        {
            InitializeComponent();
            Transaction = ts;
            this.DataContext = Transaction;
        }

        private void Btn_NumRandom(object sender, TouchEventArgs e)
        {

        }

        private void Btn_DeleteAllTouchDown(object sender, TouchEventArgs e)
        {
            try
            {

                if (txtNum1 == true)
                {
                    Num1.Text = string.Empty;
                    txtNum1 = false;
                }

                if (txtNum2 == true)
                {
                    Num2.Text = string.Empty;
                    txtNum2 = false;
                }

                if (txtNum3 == true)
                {
                    Num3.Text = string.Empty;
                    txtNum3 = false;
                }

                if (txtNum4 == true)
                {
                    Num4.Text = string.Empty;
                    txtNum4 = false;
                }

                if (txtDerecho == true)
                {
                    TxtValorDerecho.Text = string.Empty;
                }

                if (txtCombinado == true)
                {
                    TxtValorCombinado.Text = string.Empty;
                }

                if (txtCuna == true)
                {
                    TxtValorCuna.Text = string.Empty;

                }

                if (txtCifra == true)
                {
                    TxtValorCifra.Text = string.Empty;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Btn_DeleteTouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                string val = Num1.Text;
                string val2 = Num2.Text;
                string val3 = Num3.Text;
                string val4 = Num4.Text;
                string val5 = TxtValorCombinado.Text;
                string val6 = TxtValorDerecho.Text;
                string val7 = TxtValorCuna.Text;
                string val8 = TxtValorCifra.Text;


                if (txtNum1 == true)
                {
                    if (val.Length > 0)
                    {
                        Num1.Text = val.Remove(val.Length - 1);
                    }
                    //   txtNum1 = false;
                }

                if (txtNum2 == true)
                {
                    if (val2.Length > 0)
                    {
                        Num2.Text = val2.Remove(val2.Length - 1);
                    }
                    //    txtNum2 = false;
                }

                if (txtNum3 == true)
                {
                    if (val3.Length > 0)
                    {
                        Num3.Text = val3.Remove(val3.Length - 1);
                    }
                    //     txtNum3 = false;
                }

                if (txtNum4 == true)
                {
                    if (val4.Length > 0)
                    {
                        Num4.Text = val4.Remove(val4.Length - 1);
                    }
                    //        txtNum4 = false;
                }

                if (txtDerecho == true)
                {
                    if (val6.Length > 0)
                    {
                        TxtValorDerecho.Text = val6.Remove(val6.Length - 1);
                    }
                }

                if (txtCombinado == true)
                {
                    if (val5.Length > 0)
                    {
                        TxtValorCombinado.Text = val5.Remove(val5.Length - 1);
                    }
                }

                if (txtCuna == true)
                {
                    if (val7.Length > 0)
                    {
                        TxtValorCuna.Text = val7.Remove(val7.Length - 1);
                    }
                }

                if (txtCifra == true)
                {
                    if (val8.Length > 0)
                    {
                        TxtValorCifra.Text = val8.Remove(val8.Length - 1);
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Keyboard_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                if (txtNum1 == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    Num1.Text += Tag;
                    //  txtNum1 = false;
                }



                if (txtNum2 == true)
                {


                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    Num2.Text += Tag;

                }

                //   txtNum2 = false;

                if (txtNum3 == true)
                {

                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    Num3.Text += Tag;

                }

                //    txtNum3 = false;

                if (txtNum4 == true)
                {

                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    Num4.Text += Tag;

                }

                if (txtDerecho == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtValorDerecho.Text += Tag;
                }

                if (txtCombinado == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtValorCombinado.Text += Tag;
                }

                if (txtCuna == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtValorCuna.Text += Tag;

                }

                if (txtCifra == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtValorCifra.Text += Tag;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Focus_Cifra(object sender, RoutedEventArgs e)
        {
            txtNum1 = false;
            txtNum2 = false;
            txtNum3 = false;
            txtNum4 = false;
            txtCifra = true;
            txtCombinado = false;
            txtCuna = false;
            txtDerecho = false;
        }

        private void Focus_Cuna(object sender, RoutedEventArgs e)
        {
            txtNum1 = false;
            txtNum2 = false;
            txtNum3 = false;
            txtNum4 = false;
            txtCifra = false;
            txtCombinado = false;
            txtCuna = true;
            txtDerecho = false;
        }

        private void Focus_Combinado(object sender, RoutedEventArgs e)
        {
            txtNum1 = false;
            txtNum2 = false;
            txtNum3 = false;
            txtNum4 = false;
            txtCifra = false;
            txtCombinado = true;
            txtCuna = false;
            txtDerecho = false;
        }

        private void Focus_Derecho(object sender, RoutedEventArgs e)
        {
            txtNum1 = false;
            txtNum2 = false;
            txtNum3 = false;
            txtNum4 = false;
            txtCifra = false;
            txtCombinado = false;
            txtCuna = false;
            txtDerecho = true;
        }

        private void Focus_Num4(object sender, RoutedEventArgs e)
        {
            txtNum1 = false;
            txtNum2 = false;
            txtNum3 = false;
            txtNum4 = true;
            txtCifra = false;
            txtCombinado = false;
            txtCuna = false;
            txtDerecho = false;
        }

        private void Focus_Num3(object sender, RoutedEventArgs e)
        {
            txtNum1 = false;
            txtNum2 = false;
            txtNum3 = true;
            txtNum4 = false;
            txtCifra = false;
            txtCombinado = false;
            txtCuna = false;
            txtDerecho = false;
        }

        private void Focus_Num2(object sender, RoutedEventArgs e)
        {
            txtNum1 = false;
            txtNum2 = true;
            txtNum3 = false;
            txtNum4 = false;
            txtCifra = false;
            txtCombinado = false;
            txtCuna = false;
            txtDerecho = false;
        }

        private void Focus_Num1(object sender, RoutedEventArgs e)
        {
            txtNum1 = true;
            txtNum2 = false;
            txtNum3 = false;
            txtNum4 = false;
            txtCifra = false;
            txtCombinado = false;
            txtCuna = false;
            txtDerecho = false;
        }

        private void Btn_Cancel(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);

        }

        private void Btn_Continue(object sender, TouchEventArgs e)
        {
            validarNumero();
        }

        private void Btn_NumAdd(object sender, TouchEventArgs e)
        {
            GuardarNumero();

        }

        public void validarNumero()
        {
            try
            {

                if (Num1.Text == "" && Num2.Text == "")
                {
                    Utilities.ShowModal("Debes ingresar un numero valido", EModalType.Error);
                }
                else
                {


                    AdminPayPlus.SaveLog("SelecTNumUCS", "entrando a la ejecucion GetNumber", "OK", "", null);

                    Numero = String.Concat(Num1.Text + Num2.Text + Num3.Text + Num4.Text);

                    Transaction.NumeroLoteria = Numero;

                    NumeroChance NumerosC = new NumeroChance();

                    NumerosC.numero = Transaction.NumeroLoteria;


                    if (TxtValorDerecho.Text != null && TxtValorDerecho.Text != "")
                    {
                        NumerosC.directo = Convert.ToInt32(TxtValorDerecho.Text);
                    }

                    if (TxtValorCombinado.Text != null && TxtValorCombinado.Text != "")
                    {
                        NumerosC.combinado = Convert.ToInt32(TxtValorCombinado.Text);
                    }

                    if (TxtValorCuna.Text != null && TxtValorCuna.Text != "")
                    {
                        NumerosC.pata = Convert.ToInt32(TxtValorCuna.Text);
                    }

                    if (TxtValorCifra.Text != null && TxtValorCifra.Text != "")
                    {
                        NumerosC.una = Convert.ToInt32(TxtValorCifra.Text);
                    }


                    Transaction.Chances.Add(NumerosC);

                    // AdminPayPlus.SaveLog("DateTxUC", "Respuesta del servicio Validar Numero", "OK", String.Concat(ResponseData), null);

                    Utilities.CloseModal();
                    Utilities.navigator.Navigate(UserControlView.Verificar, Transaction);


                }

            }
            catch (Exception ex)
            {

                Utilities.ShowModal("En estos Momentos los servicios de SuperChance no estan Disponibles", EModalType.Error);
                AdminPayPlus.SaveLog("LoginUC", "En estos Momentos los servicios de BetPlay no estan Disponibles", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                Utilities.navigator.Navigate(UserControlView.Menu);

            }



        }


        public void GuardarNumero()
        {
            try
            {

                AdminPayPlus.SaveLog("SelecTNumUCS", "entrando a la ejecucion GetNumber", "OK", "", null);

                //   Task.Run(() =>
                //   {
                //    if (Convert.ToInt32(TxtNumCombinado.Text) > 840 || Convert.ToInt32(TxtNumCifra.Text) > 840 || Convert.ToInt32(TxtNumDerecho.Text) > 840 || Convert.ToInt32(TxtNumCuña.Text) > 840 || Convert.ToInt32(TxtNumCombinado.Text) < 840000 || Convert.ToInt32(TxtNumCifra.Text) < 840000 || Convert.ToInt32(TxtNumDerecho.Text) < 840000 || Convert.ToInt32(TxtNumCuña.Text) < 840000)
                //        {
                try
                {

                    if (Num1.Text == "" && Num2.Text == "" && Num3.Text == "" && Num4.Text == "")
                    {
                        Utilities.ShowModal("Debes ingresar un numero valido", EModalType.Error);
                    }
                    else
                    {

                        Numero = String.Concat(Num1.Text + Num2.Text + Num3.Text + Num4.Text);

                        Transaction.NumeroLoteria = Numero;

                        if (Transaction.ExtraMum < 7)
                        {

                            NumeroChance NumerosC = new NumeroChance();

                            NumerosC.numero = Transaction.NumeroLoteria;

                            if (TxtValorDerecho.Text != null && TxtValorDerecho.Text != "")
                            {
                                NumerosC.directo = Convert.ToInt32(TxtValorDerecho.Text);
                            }

                            if (TxtValorCombinado.Text != null && TxtValorCombinado.Text != "")
                            {
                                NumerosC.combinado = Convert.ToInt32(TxtValorCombinado.Text);
                            }

                            if (TxtValorCuna.Text != null && TxtValorCuna.Text != "")
                            {
                                NumerosC.pata = Convert.ToInt32(TxtValorCuna.Text);
                            }

                            if (TxtValorCifra.Text != null && TxtValorCifra.Text != "")
                            {
                                NumerosC.una = Convert.ToInt32(TxtValorCifra.Text);
                            }


                            //    Transaction.NumeroLiquidar.Add(NumerosD);
                            Transaction.Chances.Add(NumerosC);


                            //foreach (var item in Transaction.NumeroLiquidar)
                            //{
                            //    NumeroValidar Numeros = new NumeroValidar
                            //    {
                            //        numero = item.derecho,

                            //    };

                            //    Data.numeros.Add(Numeros);
                            //}

                            //foreach (var item in Transaction.LoteriaLiquidar)
                            //{
                            //    LoteriaValidar Loterias = new LoteriaValidar
                            //    {

                            //        idLoteria = item.idLoteria,

                            //    };
                            //    Data.loterias.Add(Loterias);
                            //}

                            //var Respuesta = AdminPayPlus.ApiIntegration.ValidateNumber(Data);

                            //var ResponseData = JsonConvert.DeserializeObject<ResponseValidarNumero>(Respuesta.ResponseData.ToString());

                            //AdminPayPlus.SaveLog("DateTxUC", "Respuesta del servicio GetLotteries", "OK", ResponseData.ToString(), null);

                            //if (Respuesta != null)
                            //{
                            //    if (ResponseData.ok == true)
                            //    {
                            //        Transaction.NumberValidate = ResponseData;
                            //        Transaction.ExtraMum += 1;
                            //        if (Transaction.ExtraMum <= 7)
                            //        {

                            Utilities.CloseModal();
                            //     LiquidarChance();
                            Utilities.navigator.Navigate(UserControlView.Adicional, Transaction);
                            //        }
                            //        else
                            //        {
                            //            Utilities.CloseModal();
                            //            Utilities.ShowModal("Ya superaste la cantidad maxima de numeros a escoger", EModalType.Error);

                            //        }

                            //    }
                            //    else
                            //    {
                            //        Utilities.ShowModal("No se pudo Validar el Numero Aleatorio", EModalType.Error);
                            //    }
                            //}
                            //else
                            //{
                            //    Utilities.ShowModal("En estos Momentos los servicios de SuperChance no estan Disponibles", EModalType.Error);
                            //    Utilities.navigator.Navigate(UserControlView.Menu);
                            //}
                        }
                        else
                        {
                            Utilities.ShowModal("Ya superaste la cantidad maxima de numeros a escoger", EModalType.Error);
                        }

                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                // }
                //else
                //{
                //    Utilities.ShowModal("el monto minimo es de $840 pesos y el maximo son $840.000", EModalType.Error);
                //}

                //     });

                //     Utilities.ShowModal("Estamos Validando la informacion, Un momento Porfavor", EModalType.Preload);

            }
            catch (Exception ex)
            {

                Utilities.ShowModal("En estos Momentos los servicios de SuperChance no estan Disponibles", EModalType.Error);
                AdminPayPlus.SaveLog("LoginUC", "En estos Momentos los servicios de BetPlay no estan Disponibles", "ERROR", string.Concat(ex.Message, " ", ex.StackTrace), null);
                Utilities.navigator.Navigate(UserControlView.Menu);
            }
        }
    }
}
