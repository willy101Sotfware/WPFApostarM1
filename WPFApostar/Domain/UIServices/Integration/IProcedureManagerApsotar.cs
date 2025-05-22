
using WPFApostar.Domain.UIServices.ObjectIntegration;

namespace WPFApostar.Domain.UIServices.Integration;

public interface IProcedureManagerApsotar
{
    Task<ResponseApi> GetLotteries(RequestGetLotteries request);
    Task<ResponseApi> ValidateChance(RequestValidateChance request);
    Task<ResponseApi> SendAlert(RequestSendAlert request);
    Task<ResponseSavePayer> SavePayer(RequestSavePayer request);
    Task<ResponseValidatePayer> ValidatePayer(RequestValidatePayer request);
    Task<ResponseInsertRecord> InsertRecord(RequestInsertRecord request);
    Task<ResponseConsultSubproductosPaquetes> ConsultSubproductosPaquetes(RequestConsultSubproductosPaquetes request);
    Task<ResponseConsultPaquetes> ConsultPaquetes(RequestConsultPaquetes request);
    Task<ResponseGuardarPaquetes> GuardarPaquete(RequestGuardarPaquete request);
    Task<ResponseTokenBetplay> GetTokenBetplay(RequesttokenBetplay request);
    Task<ResponseGetProducts> ConsultSubproductBetplay(RequestConsultSubproductBetplay request);
    Task<ResponseNotifyPayment> NotifyPayment(RequestNotifyRecaudo request);
}
