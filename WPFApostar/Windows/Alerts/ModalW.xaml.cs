using System.Reflection;
using System.Windows;
using System.Windows.Input;
using WPFApostar.Classes;
using WPFApostar.Classes.UseFull;
using WPFApostar.Models;

namespace WPFApostar.Windows.Alerts
{
    /// <summary>
    /// Lógica de interacción para ModalW.xaml
    /// </summary>
    /// 
  

    public partial class ModalW : Window
    {

        private ModalModel modal;
        private TimerGeneric timer;

        public ModalW(ModalModel modal)
        {
            InitializeComponent();

            this.modal = modal;

            this.DataContext = this.modal;

            ConfigureModal();
        }

        private void ConfigureModal()
        {
            try
            {
                if (this.modal.Timer)
                {
                    ActivateTimer();
                }

                if (this.modal.TypeModal == EModalType.Preload)
                {
                    this.BtnOk.Visibility = Visibility.Hidden;
                    this.BtnYes.Visibility = Visibility.Hidden;
                    this.BtnNo.Visibility = Visibility.Hidden;
                    this.LblMessageTouch.Visibility = Visibility.Hidden;
                    GifLoadder.Visibility = Visibility.Visible;
                }
                else if (this.modal.TypeModal == EModalType.NotExistAccount)
                {
                    this.BtnOk.Visibility = Visibility.Visible;
                    this.BtnYes.Visibility = Visibility.Hidden;
                    this.BtnNo.Visibility = Visibility.Hidden;
                    this.LblMessageTouch.Visibility = Visibility.Hidden;
                    GifLoadder.Visibility = Visibility.Hidden;
                }
                else if (this.modal.TypeModal == EModalType.MaxAmount || this.modal.TypeModal == EModalType.Error)
                {
                    this.BtnOk.Visibility = Visibility.Visible;
                    this.BtnYes.Visibility = Visibility.Hidden;
                    this.BtnNo.Visibility = Visibility.Hidden;
                    this.LblMessageTouch.Visibility = Visibility.Hidden;
                    GifLoadder.Visibility = Visibility.Hidden;
                }
                else if (this.modal.TypeModal == EModalType.Information || this.modal.TypeModal == EModalType.NoPaper)
                {
                    this.BtnOk.Visibility = Visibility.Hidden;
                    this.BtnYes.Visibility = Visibility.Visible;
                    this.LblMessageTouch.Visibility = Visibility.Hidden;
                    this.BtnNo.Visibility = Visibility.Visible;
                    GifLoadder.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, ex.ToString());
            }
        }


        private void Grid_TouchDown(object sender, MouseButtonEventArgs e)
        {
            if (this.modal.TypeModal == EModalType.MaxAmount || this.modal.TypeModal == EModalType.Error)
            {
                try
                {
                    SetCallBacksNull();
                    timer.CallBackStop?.Invoke(1);
                }
                catch { }

                this.DialogResult = true;
            }
        }

        private void BtnOk_TouchDown(object sender, MouseButtonEventArgs e)
        {
            if (modal.Timer)
            {
                SetCallBacksNull();
                timer.CallBackStop?.Invoke(1);
            }

            this.DialogResult = true;
        }

        private void BtnYes_TouchDown(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnNo_TouchDown(object sender, MouseButtonEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ActivateTimer()
        {
            timer = new TimerGeneric(Utilities.GetConfiguration("TimerModal"));
            timer.CallBackClose = response =>
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    DialogResult = true;
                });
            };
        }

        private void SetCallBacksNull()
        {
            timer.CallBackClose = null;
            timer.CallBackTimer = null;
        }
    }
}
