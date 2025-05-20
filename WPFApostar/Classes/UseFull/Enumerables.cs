using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFApostar.Classes
{

    public enum EMEIMEssages
    {
        MeiConnected,
        MeiBillEscrow,
        MeiBillStacked,
        MeiBillRejected,
        MeiCashBoxRemoved,
        MeiCashBoxAttached,
        MeiJamDetected,
        MeiJamResolved,
        MeiCheated,
        MeiDisconnected,
        MeiPauseDetected,
        MeiPauseResolved,
        MeiPowerUp,
        MeiPowerUpComplete,
        MeiBillInScrowOnPowerUp,
        MeiStackerFull,
        MeiStackerFullResolved,
        MeiStallDetected,
        MeiStallResolved
    }
    public enum EResponseCode
    {
        Error = 300,
        NotFound = 404,
        OK = 200
    }
    
    public enum ELogType
    {
        General = 0,
        Error = 1,
        Device = 2
    }
    
    public enum ETypeTramites
    {
    
        [Description("Recarga BetPlay")]
        BetPlay = 56,
        [Description("SuperChance")]
        SuperChance = 57,
        [Description("Recarga Celular")]
        RecargasCel = 58,
        [Description("Paquetes Celular")]
        PaquetesCel = 59,
        [Description("Recaudos")]
        Recaudos = 72 ,
       [Description("Chance")]
        Chance = 73
    }

    public enum EModalType
    {
        Cancell = 0,
        NotExistAccount = 1,
        Error = 2,
        MaxAmount = 3,
        Information = 4,
        Preload = 5,
        NoPaper = 6
    }

    public enum EError
    {
        Printer = 1,
        Nopapper = 2,
        Device = 3,
        Aplication = 5,
        Api = 6,
        Customer = 7,
        Internet = 8
    }

    public enum ELevelError
    {
        Mild = 3,
        Medium = 2,
        Strong = 1,
    }

    public enum UserControlView
    {

        //Betplay

        Login,
        Config,
        Admin,
        Main,
        Menu,
        Recharge,
        Validate,
        Payment,
        FactureBet,
        Finish,     
        ReturnMoney,

        //Superchance

        Form,
        Dia,
        info,
        Loterias,
        Apuesta,
        Adicional,
        Eliminar,
        ReturnChance,
        NuevaApuesta,
        Politicas,
        Verificar,
        PagosChance,
        Imprimir,
        FinishChance,

        //Recaudo

        ConsultForm,
        ConsultForm2,
        ConsultForm3,
        TypeRecaudo,
        Selectoption,
        SelectCompany,
        ConsultReference,
        ScanFacture,
        DataConsult,
        ReturnMoneyRe,
        PaymentRecaudo,
        SuccesRecaudo,


            //Paquetes


         SelectOption,
        SelectOperador,
        SelectPaquete,
        DigitarNumero,
        VaLidarPaquete,
        ResumenPaquete,
        PaymentPaquetes,
        SuccesRechatge,
        FactureRec,
        FinishPaquetes,
        ReturnMoneyPaquetes

    }

    public enum ETransactionState
    {
        Initial = 1,
        Success = 2,
        CancelError = 6,
        Cancel = 3,
        Error = 5,
        AprobadosinNotificar = 7,
        ErrorService = 4
    }

    public enum ETransactionType
    {
        Payment = 3,
        BetPlay = 5,
        SuperChance = 57,
    }

    public enum ETypeAdministrator
    {
        Balancing = 1,
        Upload = 2,
        Finished = 3,
        ReUploat = 4
    }
}
