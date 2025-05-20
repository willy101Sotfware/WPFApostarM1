using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFApostar.Classes;
using WPFApostar.Models;
using WPFApostar.Services.Object;
using WPFApostar.UserControls;
using WPFApostar.UserControls.Administrator;
using WPFApostar.Services.ObjectIntegration;
using WPFApostar.UserControls.Betplay;
using WPFApostar.UserControls.Chance;
using WPFApostar.UserControls.Recaudo;
using WPFApostar.UserControls.Paquetes;

namespace WPFApostar.Models
{
    public class Navigation : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private UserControl _view;

        public UserControl View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(View)));
            }
        }

        public void Navigate(UserControlView newWindow, object data = null, object complement = null) => Application.Current.Dispatcher.Invoke((Action)delegate
        {
            try
            {
                switch (newWindow)
                {
                    //BetPlay

                    case UserControlView.Config:
                        View = new ConfigurateUC();
                        break;
                    case UserControlView.Main:
                        View = new Main();
                        break;

                    //BetPlay

                    case UserControlView.ReturnMoney:
                        View = new ReturnMoneyUC((Transaction)data);
                        break;
                    case UserControlView.Menu:
                        View = new MenuUC();
                        break;
                    case UserControlView.Login:
                        View = new LoginUC((Transaction)data);
                        break;
                    case UserControlView.Recharge:
                        View = new RechargeUC((Transaction)data);
                        break;
                    case UserControlView.Validate:
                        View = new ValidateUC((Transaction)data);
                        break;
                    case UserControlView.Payment:
                        View = new UserControls.Betplay.PaymentUC((Transaction)data);
                        break;
                    case UserControlView.Finish:
                        View = new FinishUC((Transaction)data);
                        break;
                    case UserControlView.FactureBet:
                        View = new PreviewFacture((Transaction)data);
                        break;

                    ////SuperChance

                    case UserControlView.info:
                        View = new InfoUC((Transaction)data);
                        break;
                    case UserControlView.Form:
                        View = new FormUC((Transaction)data);
                        break;
                    case UserControlView.Dia:
                        View = new DateUC((Transaction)data);
                        break;
                    case UserControlView.Loterias:
                        View = new LotteriesUC((Transaction)data);
                        break;
                    case UserControlView.Apuesta:
                        View = new SelectNum((Transaction)data);
                        break;
                    case UserControlView.Verificar:
                        View = new ConfirmLotteryUC((Transaction)data);
                        break;
                    case UserControlView.PagosChance:
                        View = new UserControls.Chance.PaymentUC((Transaction)data);
                        break;
                    case UserControlView.Adicional:
                        View = new ExtraSelectNumUC((Transaction)data);
                        break;
                    case UserControlView.ReturnChance:
                        View = new ReturnMoneyChance((Transaction)data);
                        break;
                    case UserControlView.FinishChance:
                        View = new FinishChanceUC((Transaction)data);
                        break;

                    //Recaudos

                    case UserControlView.ConsultForm:
                        View = new ConsultFormUC((Transaction)data);
                        break; 
                    case UserControlView.ConsultForm2:
                        View = new ConsultForm2UC((Transaction)data);
                        break;
                    case UserControlView.ConsultForm3:
                        View = new ConsultForm3UC((Transaction)data);
                        break;
                    case UserControlView.TypeRecaudo:
                        View = new TypeRecaudoUC((Transaction)data);
                        break;
                    case UserControlView.SelectCompany:
                        View = new SelectCompanyUC((Transaction)data);
                        break;
                    case UserControlView.Selectoption:
                        View = new SelectOptionUC((Transaction)data);
                        break;
                    case UserControlView.ConsultReference:
                        View = new ConsultReferenceUC((Transaction)data);
                        break;
                    case UserControlView.ScanFacture:
                        View = new ScanFactureUC((Transaction)data);
                        break;       
                    case UserControlView.DataConsult:
                        View = new DataConsultUserControl((Transaction)data);
                        break;
                    case UserControlView.PaymentRecaudo:
                        View = new PaymentUserControl((Transaction)data);
                        break;
                    case UserControlView.ReturnMoneyRe:
                        View = new ReturnMoneyReUC((Transaction)data);
                        break;
                    case UserControlView.SuccesRecaudo:
                        View = new SuccesUserControl((Transaction)data);
                        break;


                    //Paquetes


                    case UserControlView.SelectOperador:
                        View = new SelectOperadorUC((Transaction)data);
                        break;
                    case UserControlView.SelectPaquete:
                        View = new SelectPaqueteUC((Transaction)data);
                        break;
                    case UserControlView.DigitarNumero:
                        View = new DigitarNumeroUC((Transaction)data);
                        break;
                    case UserControlView.ResumenPaquete:
                        View = new ResumenPaquetesUC((Transaction)data);
                        break;
                    case UserControlView.PaymentPaquetes:
                        View = new PaymentPaquetesUC((Transaction)data);
                        break;
                    case UserControlView.ReturnMoneyPaquetes:
                        View = new ReturnMoneyPaquetesUC((Transaction)data);
                        break;
                    case UserControlView.FinishPaquetes:
                        View = new FinishPaquetesUC((Transaction)data);
                        break;




                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Navigate", ex, ex.ToString());
            }
            GC.Collect();
        });
    }
}