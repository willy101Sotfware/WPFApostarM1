using System.Windows.Controls;
using System.Windows.Input;
using WPFApostar.Classes;
using WPFApostar.Classes.UseFull;
using WPFApostar.Models;


namespace WPFApostar.UserControls.Recaudo
{
    /// <summary>
    /// Lógica de interacción para SelectOptionUC.xaml
    /// </summary>
    public partial class SelectOptionUC : UserControl
    {
        private Transaction Transaction;
        private TimerGeneric timer;

        public SelectOptionUC(Transaction Ts)
        {
            Transaction = Ts;
            this.DataContext = Transaction;
            InitializeComponent();
        }

        private void BtnScan_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.ScanFacture, Transaction);
        }

        private void BtnReference_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.ConsultReference, Transaction);

        }

        private void BtnCancelar_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }
    }
}
