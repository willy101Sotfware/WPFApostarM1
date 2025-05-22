using System.Windows.Controls;
using System.Windows.Input;
using WPFApostar.Domain;
using WPFApostar.Domain.ApiService.Models;

namespace WPFApostar.UserControls.Chance
{
    /// <summary>
    /// Lógica de interacción para InfoUC.xaml
    /// </summary>
    public partial class InfoUC : UserControl
    {

        public Transaction transaction;

        public InfoUC(Transaction Ts)
        {
            InitializeComponent();
            transaction = Ts;
        }

        private void BtnJugar_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Form, transaction);
        }

        private void Btn_jugar(object sender, MouseButtonEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Form, transaction);

        }
    }
}
