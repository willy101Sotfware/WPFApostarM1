using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPFApostar.Domain;
using WPFApostar.Domain.ApiService.Models;
using WPFApostar.Domain.UIServices.ObjectIntegration;
using LotteriesViewModel = WPFApostar.Domain.ApiService.Models.LotteriesViewModel;

namespace WPFApostar.UserControls.Chance
{
    /// <summary>
    /// Lógica de interacción para LotteriesUC.xaml
    /// </summary>
    public partial class LotteriesUC : UserControl
    {


        private ObservableCollection<LotteriesViewModel> LstLotteriesModel;

        Transaction Transaction;

        LoteriaLiquidar loterias;

        LoteriaChance loteria;

        CollectionViewSource view = new CollectionViewSource();

        LotteriesViewModel Selectedlotteries = new LotteriesViewModel();


        public LotteriesUC(Transaction transaction)
        {
            InitializeComponent();
            Transaction = transaction;
            this.DataContext = Transaction;
            LstLotteriesModel = new ObservableCollection<LotteriesViewModel>();
            LoadLotteriesAsync();
        //    Transaction.LoteriaLiquidar = new List<LoteriaLiquidar>();
        //    Transaction.LoteriaChance = new List<LoteriaChance>();
        }

        private void Btn_CancelarTouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Menu);
        }

        public async Task LoadLotteriesAsync()
        {
            try
            {



                //foreach (var Loteria in Transaction.LotteryList.listadoloteriasField)
                //{

                //   LstLotteriesModel.Add(new LotteriesViewModel
                //    {
                //        ImageData =
                //         Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                //        Assembly.GetEntryAssembly().Location),
                //        "Loterias", Loteria.nombreField.ToString() + ".png"))),
                //        Tag = Loteria.codigoField.ToString(),
                //        IdLoteria = Loteria.idField.ToString(),
                //        abreviatura = Loteria.nombrecortoField.ToString(),
                //        ImageDataS = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                //        Assembly.GetEntryAssembly().Location),
                //        "LoteriasS", Loteria.nombreField.ToString() + ".png"))),
                //        IsSelect = Utilities.LoadImageFromFile(new Uri(Path.Combine(Path.GetDirectoryName(
                //        Assembly.GetEntryAssembly().Location),
                //        "Loterias", Loteria.nombreField.ToString() + ".png"))),
                //        nombreField = Loteria.nombreField.ToString(),
                //   });
                  

                //};

             

                view.Source = LstLotteriesModel;
                this.DataContext = view;
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveErrorControl(JsonConvert.SerializeObject(ex), "Load lotteries", EError.Aplication, ELevelError.Medium);
            }
        }

        private void Btn_SelectLotterie(object sender, TouchEventArgs e)
        {

           
        

        }

        private void Btn_ContinuarTouchDown(object sender, TouchEventArgs e)
        {
            try
            {

                if (Transaction.LoteriaChance.Count > 0)
                {
                    Utilities.navigator.Navigate(UserControlView.Apuesta, Transaction);
                }
                else
                {
                    Utilities.ShowModal("Debes seleccionar minimo una loteria", EModalType.Error);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }



        }

        private void BtnAceptar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Transaction.LoteriaChance.Count > 0)
            {
                Utilities.navigator.Navigate(UserControlView.Apuesta, Transaction);
            }
            else
            {
                Utilities.ShowModal("Debes seleccionar minimo una loteria", EModalType.Error);
            }
        }

    }
}
