using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
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
    /// Lógica de interacción para SelectNum.xaml
    /// </summary>
    public partial class SelectNum : UserControl
    {
        Transaction transaction;
        private List<TextBox> textBoxes = new List<TextBox>();
        private SelectNumViewModel viewModel = new SelectNumViewModel();
        private TimerGeneric timer;

        public SelectNum(Transaction ts)
        {
            InitializeComponent();
            transaction = ts;
      

            foreach (UIElement element in mainContainer.Children)
            {
                if (element is TextBox textBox)
                {
                    textBoxes.Add(textBox);
                }
            }
            if (transaction.ListaChances == null)
            {
                transaction.ListaChances = new List<Domain.ApiService.Models.Chance>();
            }
            if (transaction.Editar)
            {
                LoadEditChance();
            }

           ActivateTimer();

            this.DataContext = viewModel;

        }

        private void Btn_NumRandom(object sender, EventArgs e)
        {

            try
            {
                Random randObj = new Random();

                if (C4CheckSi.Visibility == Visibility.Visible)
                {
                    string numTxt = randObj.Next(0, 9999).ToString("D4");

                    Num1.Text = numTxt.Substring(0, 1);
                    Num2.Text = numTxt.Substring(1, 1);
                    Num3.Text = numTxt.Substring(2, 1);
                    Num4.Text = numTxt.Substring(3, 1);
                }
                if (C3CheckSi.Visibility == Visibility.Visible)
                {
                    string numTxt = randObj.Next(0, 999).ToString("D3");

                    Num2.Text = numTxt.Substring(0, 1);
                    Num3.Text = numTxt.Substring(1, 1);
                    Num4.Text = numTxt.Substring(2, 1);
                }
                if (C2CheckSi.Visibility == Visibility.Visible)
                {
                    string numTxt = randObj.Next(0, 99).ToString("D2");

                    Num3.Text = numTxt.Substring(0, 1);
                    Num4.Text = numTxt.Substring(1, 1);
                }
                if (C1CheckSi.Visibility == Visibility.Visible)
                {
                    string numTxt = randObj.Next(0, 9).ToString("D1");
                    
                    Num4.Text = numTxt.Substring(0, 1);
                }



            }
            catch(Exception ex)
            {

            }

       
        }

        private void LoadEditChance()
        {

            string numChance = transaction.ListaChances[transaction.IndexChanceToEdit].Numero;

            if (numChance.Length == 4)
            {
                Num1.Text = numChance.Substring(0, 1);
                Num2.Text = numChance.Substring(1, 1);
                Num3.Text = numChance.Substring(2, 1);
                Num4.Text = numChance.Substring(3, 1);
            }
            if (numChance.Length == 3)
            {
                Num1.Text = "";
                Num2.Text = numChance.Substring(0, 1);
                Num3.Text = numChance.Substring(1, 1);
                Num4.Text = numChance.Substring(2, 1);
            }
            if (numChance.Length == 2)
            {
                Num1.Text = "";
                Num2.Text = "";
                Num3.Text = numChance.Substring(0, 1);
                Num4.Text = numChance.Substring(1, 1);
            }
            if (numChance.Length == 1)
            {
                Num1.Text = "";
                Num2.Text = "";
                Num3.Text = "";
                Num4.Text = numChance.Substring(0, 1);
            }

            viewModel.ValorDirecto = transaction.ListaChances[transaction.IndexChanceToEdit].Directo;
            viewModel.ValorCombinado = transaction.ListaChances[transaction.IndexChanceToEdit].Combinado;
            viewModel.ValorPata = transaction.ListaChances[transaction.IndexChanceToEdit].Pata;
            viewModel.ValorUna = transaction.ListaChances[transaction.IndexChanceToEdit].Una;

            transaction.ListaChances.RemoveAt(transaction.IndexChanceToEdit);
        }

        public bool IsDataValid()
        {
            // Validar campos obligatorios
            foreach (var txtBox in textBoxes)
            {
                // Num1 es opcional, si no se coloca, juega solo 3 cifras
                if (txtBox.Name.StartsWith("Num"))
                {
                    if (txtBox.Name.Equals("Num1")) continue;

                    if (txtBox.Text.Equals(string.Empty))
                    {
                        //     txtValidaciones.Text = $"*No ha ingresado el digito {txtBox.Name.Substring(3)}";
                        return false;
                    }
                    continue;
                }

                // Validamos si hay valores en alguno de los dos Directo o Combinado

                if (C1CheckSi.Visibility == Visibility.Visible || C2CheckSi.Visibility == Visibility.Visible)
                {
                    if (viewModel.ValorDirecto == 0 && viewModel.ValorCombinado == 0)
                    {
                        //    txtValidaciones.Text = $"Se debe ingresar valor de apuesta a Directo o Combinado";
                        return false;
                    }
                }


                // Validamos que si haya texto en los txtBox y que no haya caracteres raros
                string caracteresSpeciales = @"[!@#%^&*'""`;]";
                if (Regex.IsMatch(txtBox.Text, caracteresSpeciales))
                {
                    //  txtValidaciones.Text = $"Valor invalido en {txtBox.Name.Substring(3)}";
                    return false;
                }
            }

            //Validar que las cantidades sean mayores a 500 y que sean multiplo de 100
            foreach (var txtBox in textBoxes)
            {
                if (txtBox.Name.StartsWith("Num")) continue;

                var txtVal = txtBox.Text;
                txtVal = new string(txtVal.Where(char.IsDigit).ToArray());

                int val = string.IsNullOrEmpty(txtVal) ? 0 : Convert.ToInt32(txtVal);



                if (val != 0 && val < 500)
                {
                    //     txtValidaciones.Text = $"La apuesta mínima a {txtBox.Name.Substring(3)} debe ser igual o superior a $500";

                    return false;
                }

                if (val != 0 && (val % 100) != 0)
                {
                    //      txtValidaciones.Text = $"La apuesta a {txtBox.Name.Substring(3)} debe ser multiplo de $100";
                    Utilities.ShowModal("en estos momentos la maquina no permite realizar apuestas por valores que no sean multiplos de 100 por favor verifica nuevamente", EModalType.Error);
                    return false;
                }

            }

            return true;


        }
        public void GuardarNumero()
        {
            string numChance = "";

            if (C4CheckSi.Visibility == Visibility.Visible)
            {
                 numChance = Num1.Text + Num2.Text + Num3.Text + Num4.Text;

            }
            if (C3CheckSi.Visibility == Visibility.Visible)
            {
                 numChance =  Num2.Text + Num3.Text + Num4.Text;

            }
            if (C2CheckSi.Visibility == Visibility.Visible)
            {
                 numChance = Num3.Text + Num4.Text;

            }
            if (C1CheckSi.Visibility == Visibility.Visible)
            {
                numChance =  Num4.Text;

            }


            var chance = new Models.Chance
            {
                Loterias = transaction.ListaLoteriasSeleccionadas,
                FechaJuego = DateTime.ParseExact(transaction.Fecha, "dd/MM/yyyy", null),
                Numero = numChance,
                Directo = viewModel.ValorDirecto,
                Combinado = viewModel.ValorCombinado,
                Pata = viewModel.ValorPata,
                Una = viewModel.ValorUna,
            };

            if (transaction.Editar)
            {
                transaction.ListaChances.Insert(transaction.IndexChanceToEdit, chance);
                transaction.Editar = false;
                transaction.IndexChanceToEdit = 0;
                return;
            }

            foreach(var x in transaction.TypeChance)
            {

                if(x.cifrasField == chance.Numero.Count())
                {
                    chance.TipoChance = x.idField;
                }
           
            }

          

            transaction.ListaChances.Add(chance);
        }

        private TextBox GetTextBoxFocused()
        {
            return textBoxes.Where(txtBox => txtBox.IsFocused).FirstOrDefault();
        }

        private void KeyboardBtnDeleteAll(object sender, EventArgs e)
        {
            try
            {

                TextBox target = GetTextBoxFocused();

                switch (target.Name)
                {
                    case "txtDirecto":
                        viewModel.ValorDirecto = 0;
                        break;
                    case "txtCombinado":
                        viewModel.ValorCombinado = 0;
                        break;
                    case "txtPata":
                        viewModel.ValorPata = 0;
                        break;
                    case "txtUna":
                        viewModel.ValorUna = 0;
                        break;
                    default:
                        target.Text = "";
                        break;
                }

            }
            catch (Exception ex)
            {
            }
        }

        private void KeyboardBtnDelete(object sender, EventArgs e)
        {
            try
            {
                TextBox target = GetTextBoxFocused();

                if (!(target.Text.Length > 0))
                {
                    return;
                }

                string txtTarget = target.Text;
                txtTarget = new string(txtTarget.Where(char.IsDigit).ToArray());
                txtTarget = txtTarget.Remove(txtTarget.Length - 1);

                if (target.Name.StartsWith("txt"))
                {
                    txtTarget = txtTarget != string.Empty ? txtTarget : "0";
                    switch (target.Name)
                    {
                        case "txtDirecto":
                            viewModel.ValorDirecto = Convert.ToInt32(txtTarget);
                            break;
                        case "txtCombinado":
                            viewModel.ValorCombinado = Convert.ToInt32(txtTarget);
                            break;
                        case "txtPata":
                            viewModel.ValorPata = Convert.ToInt32(txtTarget);
                            break;
                        case "txtUna":
                            viewModel.ValorUna = Convert.ToInt32(txtTarget);
                            break;
                        default:
                            target.Text = txtTarget;
                            break;
                    }

                    return;
                }

                target.Text = txtTarget;



            }
            catch (Exception ex)
            {
            }
        }

        private void KeyboardTouch(object sender, EventArgs e)
        {
            try
            {
                Image imgNum = (Image)sender;

                TextBox target = GetTextBoxFocused();

                switch (target.Name)
                {
                    case "txtDirecto":
                        viewModel.ValorDirecto = Convert.ToInt32(viewModel.ValorDirecto.ToString() + imgNum.Tag);
                        break;
                    case "txtCombinado":
                        viewModel.ValorCombinado = Convert.ToInt32(viewModel.ValorCombinado.ToString() + imgNum.Tag);
                        break;
                    case "txtPata":
                        viewModel.ValorPata = Convert.ToInt32(viewModel.ValorPata.ToString() + imgNum.Tag);
                        break;
                    case "txtUna":
                        viewModel.ValorUna = Convert.ToInt32(viewModel.ValorUna.ToString() + imgNum.Tag);
                        break;
                    default:
                        target.Text = imgNum.Tag.ToString();
                        break;
                }

            }
            catch (Exception ex)
            {
            }
        }

        public void ValidateChance()
        {
            try
            {
                InhabilitarVista();

                int LotId = 1;
                List<ApuestasValidate> LstApuestas = new List<ApuestasValidate>();

                List<LoteriaValidate> LstLoteria = new List<LoteriaValidate>();

                foreach (var x in transaction.ListaChances)
                {

                    ApuestasValidate Apuestas = new ApuestasValidate();

                    Apuestas.id = LotId++;
                    Apuestas.NumeroApostado = x.Numero;
                    Apuestas.ValorDirecto = x.Directo;
                    Apuestas.ValorCombinado = x.Combinado;
                    Apuestas.ValorPata = x.Pata;
                    Apuestas.ValorUna = x.Una;
                    Apuestas.tipoChance = new TipoChanceValidateM
                    {
                        Id = x.TipoChance
                    };


                    foreach (var y in x.Loterias)
                    {
                        LoteriaValidate Lot = new LoteriaValidate();

                        Lot.codigo = y.CodigoCodesa;

                        LstLoteria.Add(Lot);
                    }


                    Apuestas.ListLoteriasValidate = new ListLoteriasValidate
                    {
                        loteria = new List<LoteriaValidate>()
                    };

                    Apuestas.ListLoteriasValidate.loteria = LstLoteria;

                    LstApuestas.Add(Apuestas);
                }

                RequestValidateChance Request = new RequestValidateChance();

                Request.Ubicacion =  Utilities.GetConfiguration("UbicacionMaquina");

                Request.Subproducto = new SubproductoValidate()
                {

                    CodigoServicio = Convert.ToInt32(Utilities.GetConfigData("CodServicio")),
                    Id = transaction.productSelect.idField

                };

                Request.LstApuestas = new ListApuestasValidate
                {
                    apuestas = new List<ApuestasValidate>()
                };


               
                Request.AsumeIva = false;

                Request.FechaSorteo = transaction.Fecha;

                Request.Transaccion = transaction.IdTransactionAPi;

                Request.LstApuestas.apuestas = LstApuestas;

                Request.codigoApostar = "001";

                Request.idPagador = transaction.IdUser.ToString();

                Request.cedula = "";

                Task.Run(async () =>
                {

                    var Response = AdminPayPlus.ApiIntegration.ValidateChance(Request);

           //         SetCallBacksNull();
           //         timer.CallBackStop?.Invoke(1);

                    Utilities.CloseModal();

                    if (Response != null)
                    {


                        if (Response.Estado)
                        {
                            if(Response.Chance.Listadoapuestas.Apuesta[0].Codigoerror == "*")
                            {

                                transaction.ApuestaValidate = Response.Chance.Listadoapuestas.Apuesta;
                                Utilities.navigator.Navigate(UserControlView.Verificar, transaction);
                            }
                            else
                            {
                                Utilities.ShowModal("No se pudo verificar correctamente el chance por favor intenta nuevamente", EModalType.Error);
                                Utilities.navigator.Navigate(UserControlView.Menu);
                            }

                        }
                        else
                        {
                            Utilities.ShowModal("No se pudo verificar correctamente el chance por favor intenta nuevamente", EModalType.Error);
                            Utilities.navigator.Navigate(UserControlView.Menu);
                        }

                    }
                    else
                    {
                        Utilities.ShowModal("No se pudo verificar correctamente el chance por favor intenta nuevamente", EModalType.Error);
                        Utilities.navigator.Navigate(UserControlView.Menu);

                    }


                });

                Utilities.ShowModal("Estamos Validando la información un momento por favor", EModalType.Preload);

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
                Utilities.ShowModal("No se pudo verificar correctamente el chance por favor intenta nuevamente, por favor intenta nuevamente", EModalType.Error);
                Utilities.navigator.Navigate(UserControlView.Menu);

            }
        }

        private void BtnCancelar(object sender, EventArgs e)
        {
           
                transaction.ListaChances = null;
                transaction.Editar = false;
                transaction.IndexChanceToEdit = 0;
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Dia, transaction);
            
        }

        private void BtnContinuar(object sender, EventArgs e)
        {
            if (IsDataValid())
            {
                GuardarNumero();

                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
                ValidateChance();
            }
        }

        private void BtnAddNum(object sender, EventArgs e)
        {
            //if (IsDataValid())
            //{            

                if (transaction.ListaChances.Count >= 6)
                {
                    Utilities.ShowModal("ya tienes el limite máximo de números apostados",EModalType.Error);
                    return;
                }

                GuardarNumero();
    //            SetCallBacksNull();
     //           timer.CallBackStop?.Invoke(1);
                Utilities.navigator.Navigate(UserControlView.Apuesta, transaction);

              
            //}
        }



        private void InhabilitarVista()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                this.Opacity = 0.3;
                this.IsEnabled = false;
            });
        }

        private void HabilitarVista()
        {
            Dispatcher.BeginInvoke((Action)delegate
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
                    tbTimer.Text = "01:30";
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

        #region Events
        private void Num1_GotFocus(object sender, RoutedEventArgs e)
        {
        }
        private void Num1_TextChanged(object sender, TextChangedEventArgs e)
        {
            Num2.Focus();
        }

        private void Num2_GotFocus(object sender, RoutedEventArgs e)
        {
        }
        private void Num2_TextChanged(object sender, TextChangedEventArgs e)
        {
            Num3.Focus();
        }

        private void Num3_GotFocus(object sender, RoutedEventArgs e)
        {
        }
        private void Num3_TextChanged(object sender, TextChangedEventArgs e)
        {
            Num4.Focus();
        }

        private void Num4_GotFocus(object sender, RoutedEventArgs e)
        {
        }
        private void Num4_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtDirecto.Focus();
        }


        #endregion

        private void SelectCifras_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {

                Image image = (Image)sender;
                string Tag = image.Tag.ToString();

                switch (Tag)
                {

                    case "1":
                        C1CheckSi.Visibility = Visibility.Visible;
                        C2CheckSi.Visibility = Visibility.Hidden;
                        C3CheckSi.Visibility = Visibility.Hidden;
                        C4CheckSi.Visibility = Visibility.Hidden;
                        break;
                    case "2":
                        C1CheckSi.Visibility = Visibility.Hidden;
                        C2CheckSi.Visibility = Visibility.Visible;
                        C3CheckSi.Visibility = Visibility.Hidden;
                        C4CheckSi.Visibility = Visibility.Hidden;
                        break;
                    case "3":
                        C1CheckSi.Visibility = Visibility.Hidden;
                        C2CheckSi.Visibility = Visibility.Hidden;
                        C3CheckSi.Visibility = Visibility.Visible;
                        C4CheckSi.Visibility = Visibility.Hidden;
                        break;
                    case "4":
                        C1CheckSi.Visibility = Visibility.Hidden;
                        C2CheckSi.Visibility = Visibility.Hidden;
                        C3CheckSi.Visibility = Visibility.Hidden;
                        C4CheckSi.Visibility = Visibility.Visible;
                        break;
                };



                if (C4CheckSi.Visibility == Visibility.Visible)
                {
                    BlockC1.Visibility = Visibility.Hidden;
                    BlockC2.Visibility = Visibility.Hidden;
                    BlockC3.Visibility = Visibility.Hidden;
                    BlockC4.Visibility = Visibility.Hidden;
                    Num4.Visibility = Visibility.Visible;
                    Num3.Visibility = Visibility.Visible;
                    Num2.Visibility = Visibility.Visible;
                    Num1.Visibility = Visibility.Visible;
                    txtCombinado.Text = "";
                    txtDirecto.Text = "";
                    txtPata.Text = "";
                    txtUna.Text = "";
                    Num1.Text = "";
                    Num2.Text = "";
                    Num3.Text = "";
                    Num4.Text = "";
                    txtCombinado.Visibility = Visibility.Visible;
                    txtDirecto.Visibility = Visibility.Visible;
                    txtPata.Visibility = Visibility.Visible;
                    txtUna.Visibility = Visibility.Visible;
      

                }
                if (C3CheckSi.Visibility == Visibility.Visible)
                {
                    BlockC1.Visibility = Visibility.Hidden;
                    BlockC2.Visibility = Visibility.Hidden;
                    BlockC3.Visibility = Visibility.Hidden;
                    BlockC4.Visibility = Visibility.Visible;
                    Num4.Visibility = Visibility.Visible;
                    Num3.Visibility = Visibility.Visible;
                    Num2.Visibility = Visibility.Visible;
                    Num1.Visibility = Visibility.Hidden;
                    txtCombinado.Text = "";
                    txtDirecto.Text = "";
                    txtPata.Text = "";
                    txtUna.Text = "";
                    Num1.Text = "";
                    Num2.Text = "";
                    Num3.Text = "";
                    Num4.Text = "";
                    txtCombinado.Visibility = Visibility.Visible;
                    txtDirecto.Visibility = Visibility.Visible;
                    txtPata.Visibility = Visibility.Visible;
                    txtUna.Visibility = Visibility.Visible;
         
                }
                if (C2CheckSi.Visibility == Visibility.Visible)
                {
                    BlockC1.Visibility = Visibility.Hidden;
                    BlockC2.Visibility = Visibility.Hidden;
                    BlockC3.Visibility = Visibility.Visible;
                    BlockC4.Visibility = Visibility.Visible;
                    Num4.Visibility = Visibility.Visible;
                    Num3.Visibility = Visibility.Visible;
                    Num2.Visibility = Visibility.Hidden;
                    Num1.Visibility = Visibility.Hidden;
                    txtCombinado.Text = "";
                    txtDirecto.Text = "";
                    txtPata.Text = "";
                    txtUna.Text = "";
                    Num1.Text = "";
                    Num2.Text = "";
                    Num3.Text = "";
                    Num4.Text = "";
                    txtCombinado.Visibility = Visibility.Hidden;
                    txtDirecto.Visibility = Visibility.Hidden;
                    txtPata.Visibility = Visibility.Visible;
                    txtUna.Visibility = Visibility.Visible;
        

                }
                if (C1CheckSi.Visibility == Visibility.Visible)
                {
                    BlockC1.Visibility = Visibility.Hidden;
                    BlockC2.Visibility = Visibility.Visible;
                    BlockC3.Visibility = Visibility.Visible;
                    BlockC4.Visibility = Visibility.Visible;
                    Num4.Visibility = Visibility.Visible;
                    Num3.Visibility = Visibility.Hidden;
                    Num2.Visibility = Visibility.Hidden;
                    Num1.Visibility = Visibility.Hidden;
                    Num1.Text = "";
                    Num2.Text = "";
                    Num3.Text = "";
                    Num4.Text = "";
                    txtCombinado.Text = "";
                    txtDirecto.Text = "";
                    txtPata.Text = "";
                    txtUna.Text = "";
                    txtCombinado.Visibility = Visibility.Hidden;
                    txtDirecto.Visibility = Visibility.Hidden;
                    txtPata.Visibility = Visibility.Hidden;
                    txtUna.Visibility = Visibility.Visible;
       
                }


            }
            catch(Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void BtnCancelar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BtnCancelar(sender, e);
        }

        private void BtnContinuar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BtnContinuar(sender, e);
        }

        private void SelectCifras_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Image image = (Image)sender;
                string Tag = image.Tag.ToString();

                switch (Tag)
                {
                    case "1":
                        C1CheckSi.Visibility = Visibility.Visible;
                        C2CheckSi.Visibility = Visibility.Hidden;
                        C3CheckSi.Visibility = Visibility.Hidden;
                        C4CheckSi.Visibility = Visibility.Hidden;
                        break;
                    case "2":
                        C1CheckSi.Visibility = Visibility.Hidden;
                        C2CheckSi.Visibility = Visibility.Visible;
                        C3CheckSi.Visibility = Visibility.Hidden;
                        C4CheckSi.Visibility = Visibility.Hidden;
                        break;
                    case "3":
                        C1CheckSi.Visibility = Visibility.Hidden;
                        C2CheckSi.Visibility = Visibility.Hidden;
                        C3CheckSi.Visibility = Visibility.Visible;
                        C4CheckSi.Visibility = Visibility.Hidden;
                        break;
                    case "4":
                        C1CheckSi.Visibility = Visibility.Hidden;
                        C2CheckSi.Visibility = Visibility.Hidden;
                        C3CheckSi.Visibility = Visibility.Hidden;
                        C4CheckSi.Visibility = Visibility.Visible;
                        break;
                }

                if (C4CheckSi.Visibility == Visibility.Visible)
                {
                    BlockC1.Visibility = Visibility.Hidden;
                    BlockC2.Visibility = Visibility.Hidden;
                    BlockC3.Visibility = Visibility.Hidden;
                    BlockC4.Visibility = Visibility.Hidden;
                    Num4.Visibility = Visibility.Visible;
                    Num3.Visibility = Visibility.Visible;
                    Num2.Visibility = Visibility.Visible;
                    Num1.Visibility = Visibility.Visible;
                    txtCombinado.Text = "";
                    txtDirecto.Text = "";
                    txtPata.Text = "";
                    txtUna.Text = "";
                    Num1.Text = "";
                    Num2.Text = "";
                    Num3.Text = "";
                    Num4.Text = "";
                    txtCombinado.Visibility = Visibility.Visible;
                    txtDirecto.Visibility = Visibility.Visible;
                    txtPata.Visibility = Visibility.Visible;
                    txtUna.Visibility = Visibility.Visible;
                }
                if (C3CheckSi.Visibility == Visibility.Visible)
                {
                    BlockC1.Visibility = Visibility.Hidden;
                    BlockC2.Visibility = Visibility.Hidden;
                    BlockC3.Visibility = Visibility.Hidden;
                    BlockC4.Visibility = Visibility.Visible;
                    Num4.Visibility = Visibility.Visible;
                    Num3.Visibility = Visibility.Visible;
                    Num2.Visibility = Visibility.Visible;
                    Num1.Visibility = Visibility.Hidden;
                    txtCombinado.Text = "";
                    txtDirecto.Text = "";
                    txtPata.Text = "";
                    txtUna.Text = "";
                    Num1.Text = "";
                    Num2.Text = "";
                    Num3.Text = "";
                    Num4.Text = "";
                    txtCombinado.Visibility = Visibility.Visible;
                    txtDirecto.Visibility = Visibility.Visible;
                    txtPata.Visibility = Visibility.Visible;
                    txtUna.Visibility = Visibility.Visible;
                }
                if (C2CheckSi.Visibility == Visibility.Visible)
                {
                    BlockC1.Visibility = Visibility.Hidden;
                    BlockC2.Visibility = Visibility.Hidden;
                    BlockC3.Visibility = Visibility.Visible;
                    BlockC4.Visibility = Visibility.Visible;
                    Num4.Visibility = Visibility.Visible;
                    Num3.Visibility = Visibility.Visible;
                    Num2.Visibility = Visibility.Hidden;
                    Num1.Visibility = Visibility.Hidden;
                    txtCombinado.Text = "";
                    txtDirecto.Text = "";
                    txtPata.Text = "";
                    txtUna.Text = "";
                    Num1.Text = "";
                    Num2.Text = "";
                    Num3.Text = "";
                    Num4.Text = "";
                    txtCombinado.Visibility = Visibility.Hidden;
                    txtDirecto.Visibility = Visibility.Hidden;
                    txtPata.Visibility = Visibility.Visible;
                    txtUna.Visibility = Visibility.Visible;
                }
                if (C1CheckSi.Visibility == Visibility.Visible)
                {
                    BlockC1.Visibility = Visibility.Hidden;
                    BlockC2.Visibility = Visibility.Visible;
                    BlockC3.Visibility = Visibility.Visible;
                    BlockC4.Visibility = Visibility.Visible;
                    Num4.Visibility = Visibility.Visible;
                    Num3.Visibility = Visibility.Hidden;
                    Num2.Visibility = Visibility.Hidden;
                    Num1.Visibility = Visibility.Hidden;
                    Num1.Text = "";
                    Num2.Text = "";
                    Num3.Text = "";
                    Num4.Text = "";
                    txtCombinado.Text = "";
                    txtDirecto.Text = "";
                    txtPata.Text = "";
                    txtUna.Text = "";
                    txtCombinado.Visibility = Visibility.Hidden;
                    txtDirecto.Visibility = Visibility.Hidden;
                    txtPata.Visibility = Visibility.Hidden;
                    txtUna.Visibility = Visibility.Visible;
                }
            }
            catch(Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }

        private void Btn_NumRandom_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Btn_NumRandom(sender, e);
        }

        private void KeyboardTouch_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            KeyboardTouch(sender, e);
        }

        private void KeyboardBtnDelete_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            KeyboardBtnDelete(sender, e);
        }

        private void KeyboardBtnDeleteAll_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            KeyboardBtnDeleteAll(sender, e);
        }

        private void BtnAddNum_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            BtnAddNum(sender, e);
        }
    }

    internal class SelectNumViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _valorDirecto = 0;
        public int ValorDirecto
        {
            get
            {
                return _valorDirecto;
            }
            set
            {
                _valorDirecto = value;
                OnPropertyChanged(nameof(ValorDirecto));
            }
        }
        private int _valorCombinado = 0;
        public int ValorCombinado
        {
            get
            {
                return _valorCombinado;
            }
            set
            {
                _valorCombinado = value;
                OnPropertyChanged(nameof(ValorCombinado));
            }
        }
        private int _valorPata = 0;
        public int ValorPata
        {
            get
            {
                return _valorPata;
            }
            set
            {
                _valorPata = value;
                OnPropertyChanged(nameof(ValorPata));
            }
        }
        private int _valorUna = 0;

        public int ValorUna
        {
            get
            {
                return _valorUna;
            }
            set
            {
                _valorUna = value;
                OnPropertyChanged(nameof(ValorUna));
            }
        }

        private string _imgLot2 = string.Empty;

        public string ImgLot2
        {
            get
            {
                return _imgLot2;
            }
            set
            {
                _imgLot2 = value;
                OnPropertyChanged(nameof(ImgLot2));
            }
        }

        private string _imgLot3 = string.Empty;

        public string ImgLot3
        {
            get
            {
                return _imgLot3;
            }
            set
            {
                _imgLot3 = value;
                OnPropertyChanged(nameof(ImgLot3));
            }
        }

        private string _msgValidations;
        public string MsgValidations
        {
            get
            {
                return _msgValidations;
            }
            set
            {
                _msgValidations = value;
                OnPropertyChanged(nameof(MsgValidations));
            }
        }
    }
}
