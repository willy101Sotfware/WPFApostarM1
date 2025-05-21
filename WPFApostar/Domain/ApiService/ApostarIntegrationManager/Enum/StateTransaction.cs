namespace WPFApostar.Domain.ApiService.ApostarIntegrationManager.Enum;

public enum StateTransaction
{
    Iniciada = 1,
    Aprobada,
    Cancelada,
    AprobadaErrorDevuelta,
    CanceladaErrorDevuelta,
    AprobadaSinNotificar,
    ErrorServicioTercero
}
