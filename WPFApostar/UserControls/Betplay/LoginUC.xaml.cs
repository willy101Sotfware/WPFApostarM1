using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFApostar.Classes;
using WPFApostar.Classes.UseFull;
using WPFApostar.Models;

namespace WPFApostar.UserControls.Betplay
{
    /// <summary>
    /// Lógica de interacción para LoginUC.xaml
    /// </summary>
    public partial class LoginUC : UserControl
    {

        public Transaction Transaction;
        public bool txtcedula = false;
        public bool txtvalidar = false;
        private TimerGeneric timer;


        public LoginUC(Transaction Ts)
        {
            InitializeComponent();
            Transaction= Ts;
            ActivateTimer();
        }

        private void Keyboard_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {

                if (txtcedula == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtCedula.Text += Tag;
                }

                if (txtvalidar == true)
                {
                    Image image = (Image)sender;
                    string Tag = image.Tag.ToString();
                    TxtValidate.Text += Tag;

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool Validate()
        {

            try
            {
                if (!string.IsNullOrEmpty(TxtValidate.Text) && !string.IsNullOrEmpty(TxtCedula.Text))
                {

                    if (TxtValidate.Text == TxtCedula.Text)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void Btn_DeleteTouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                string val = TxtCedula.Text;
                string val2 = TxtValidate.Text;

                if (txtcedula == true)
                {
                    if (val.Length > 0)
                    {
                        TxtCedula.Text = val.Remove(val.Length - 1);
                        // TxtValidate.Text = val2.Remove(val.Length - 1);
                    }
                }

                if (txtvalidar == true)
                {
                    if (val2.Length > 0)
                    {
                        //    TxtCedula.Text = val.Remove(val.Length - 1);
                        TxtValidate.Text = val2.Remove(val2.Length - 1);
                    }

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void Btn_DeleteAllTouchDown(object sender, TouchEventArgs e)
        {
            try
            {

                if (txtcedula == true)
                {
                    TxtCedula.Text = string.Empty;
                }

                if (txtvalidar == true)
                {
                    TxtValidate.Text = string.Empty;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        private void Btn_ContinuarTouchDown(object sender, EventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);

            if (Validate())
            {
                Transaction.Document = TxtCedula.Text;

                Utilities.navigator.Navigate(UserControlView.Recharge, Transaction);


            }
            else
            {
                Utilities.ShowModal("El documento ingresado no coincide, por favor verifique la información", EModalType.Error);
                ActivateTimer();
           //     Utilities.navigator.Navigate(UserControlView.Login);
            }

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

        private void Btn_CancelarTouchDown(object sender, EventArgs e)
        {
            SetCallBacksNull();
            timer.CallBackStop?.Invoke(1);
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        private void focusTxtCedula(object sender, RoutedEventArgs e)
        {
            txtcedula = true;
            txtvalidar = false;
        }

        private void focusTxtvalidar(object sender, RoutedEventArgs e)
        {
            txtcedula = false;
            txtvalidar = true;
        }
    }
}
