

using WPFApostar.Domain.ApiService.ApostarIntegrationManager.Enum;
using WPFApostar.Domain.ApiService.ApostarIntegrationManager.ModelsApostar;
using WPFApostar.Domain.Recorder;
using WPFApostar.ViewModel;

namespace WPFApostar.Domain.UIServices;

public class Transaction
{
    // Patron de Diseño Singleton
    private static Transaction? _instance;
    public static Transaction Instance
    {
        get
        {
            if (_instance == null)
                _instance = new Transaction();
            return _instance;
        }
    }

    public static void Reset()
    {
        _instance = null;
    }

    private Transaction() { }

    public TransactionDto ApiDto { get; set; }

    public int IdTransaccionApi { get; set; }

    public int IdPaypad { get; set; } = 3;
    public string? TipoRecaudo { get; set; }
    public string? TipoOperacion { get; set; }
    public TypeTransaction TipoTransaccion { get; set; }
    public TypePayment TipoPago { get; set; }
    public StateTransaction EstadoTransaccion { get; set; }
    public string EstadoTransaccionVerb { get; set; }
    public string? Referencia { get; set; }
    public string? Documento { get; set; }
    public string? Descripcion { get; set; }
    public string? FechaVencimiento { get; set; }
    public decimal TotalSinRedondear { get; set; }
    public decimal Total { get; set; }
    public decimal TotalDevuelta { get; set; }
    public decimal TotalIngresado { get; set; }


    public bool DevueltaCorrecta { get; set; }
    public PaymentViewModel DatosPago { get; set; }
    public string Calificacion { get; set; }


    public string Token { get; set; }





}
