using Newtonsoft.Json;

namespace WPFApostar.Domain.UIServices.ObjectIntegration
{
    public class ResponseGeneric
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public bool Estado { get; set; }
    }


    #region BetPlay

    //Response GetProducts


    public class ResponseTokenBetplay
    {
        public string Token { get; set; }
    }


    public class ResponseGetProducts
    {
        public Empresa Empresa { get; set; }
        public bool Estado { get; set; }
        public Listadosubproductos Listadosubproductos { get; set; }
    }

    public class Empresa
    {
        public string direccion { get; set; }
        public string nit { get; set; }
        public string nombre { get; set; }
        public string telefono { get; set; }
    }

    public class Listadosubproductos
    {
        public List<SubProductoGeneral> Subproducto { get; set; }
    }

    public class SubProductoGeneral
    {
        public int Codigo { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Codigoservicio { get; set; }
        public float Iva { get; set; }
    }





    //Response Notify Betplay



    public class ResponseNotifyBetPlay
    {
        public Errorfield errorField { get; set; }

        public string codigoasesor { get; set; }
        public string codigoseguridad { get; set; }
        public EmpresaNotBet empresa { get; set; }
        public string estado { get; set; }
        public string fecha { get; set; }
        public string hora { get; set; }
        public string transaccionid { get; set; }
        public string clienteid { get; set; }
        public string valorrecaudo { get; set; }
        public string subproductoid { get; set; }
        public string ValorRecargado { get; set; }
        public string Iva { get; set; }
    }

    public class EmpresaNotBet
    {
        public CiudadNotBet ciudad { get; set; }
        public string direccion { get; set; }
        public string nit { get; set; }
        public string nombre { get; set; }
        public string telefono { get; set; }
    }

    public class CiudadNotBet
    {
        public string codigo { get; set; }
        public DepartamentoNotBet departamento { get; set; }
        public string nombre { get; set; }
    }

    public class DepartamentoNotBet
    {
        public string codigo { get; set; }
        public string nombre { get; set; }
    }


    public class Errorfield
    {
        public Codigofield codigoField { get; set; }
        public string mensajeField { get; set; }
    }

    public class Codigofield
    {
    }

    #endregion

    #region Chance

    //Response GetProducts Chance

    public class ResponseGetProductsChance
    {
        public Empresa empresaField { get; set; }
        public bool estadoField { get; set; }

        public Errorfield errorField { get; set; }

        public SubProductoGeneral[] listadosubproductosField { get; set; }
    }


    //Response GetLotteries Chance


    public class ResponseGetLotteries
    {
        public bool estadoField { get; set; }

        public Errorfield errorField { get; set; }
        public List<Loteria> listadoloteriasField { get; set; }
    }

    public class Loteria
    {
        public string codigoField { get; set; }
        public string idField { get; set; }
        public string nombreField { get; set; }
        public string nombrecortoField { get; set; }
    }

    //Response TypeChance 



    public class ResponseTypeChance
    {
        public bool estadoField { get; set; }
        public Errorfield errorField { get; set; }
        public float ivaField { get; set; }
        public List<Listadotipochancefield> listadotipochanceField { get; set; }
    }

    public class Listadotipochancefield
    {
        public int cifrasField { get; set; }
        public bool combinadoField { get; set; }
        public bool directoField { get; set; }
        public int idField { get; set; }
        public string nombreField { get; set; }
        public bool pataField { get; set; }
        public bool unaField { get; set; }
    }


    //Response ValidateChance


    public class ResponseValidateChance
    {
        public Empresa Empresa { get; set; }
        public object Error { get; set; }
        public bool Estado { get; set; }
        public ChanceValidate Chance { get; set; }
    }


    public class ChanceValidate
    {
        public bool Asumeiva { get; set; }
        public bool Esvalorfijo { get; set; }
        public ListadoApuestasValidate Listadoapuestas { get; set; }
    }

    public class ListadoApuestasValidate
    {
        public List<ApuestaValidate> Apuesta { get; set; }
    }

    public class ApuestaValidate
    {
        public string Codigoerror { get; set; }
        public int Codigovalidacion { get; set; }
        public int Id { get; set; }
        public string Mensajevalidacion { get; set; }
    }



    public class ResponseNotifyChance
    {
        public string Codigoasesor { get; set; }
        public string Codigoseguridad { get; set; }
        public EmpresaNotifyC Empresa { get; set; }
        public bool Estado { get; set; }
        public Errorfield Error { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public Jerarquia Jerarquia { get; set; }
        public string Transaccionid { get; set; }
        public Chance Chance { get; set; }
        public string Consecutivoapuesta { get; set; }
        public string Municipio { get; set; }
        public int Oficina { get; set; }
        public Subproducto Subproducto { get; set; }
    }

    public class EmpresaNotifyC
    {
        public CiudadC Ciudad { get; set; }
        public string Direccion { get; set; }
        public int Nit { get; set; }
        public string Nombre { get; set; }
        public int Telefono { get; set; }
    }

    public class CiudadC
    {
        public int Codigo { get; set; }
        public DepartamentoC Departamento { get; set; }
        public string Nombre { get; set; }
    }

    public class DepartamentoC
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }
    }

    public class Jerarquia
    {
        public int Celulaid { get; set; }
        public int Ciudadid { get; set; }
        public int Departamentooid { get; set; }
        public int Empresaid { get; set; }
        public int Equipoid { get; set; }
        public int Oficinaid { get; set; }
        public int Paisid { get; set; }
        public int Puntoventaid { get; set; }
        public int Subzonaid { get; set; }
        public int Usuarioid { get; set; }
        public int Zonaid { get; set; }
    }

    public class Chance
    {
        public bool Asumeiva { get; set; }
        public float Encimavalorcom { get; set; }
        public float Encimavalordir { get; set; }
        public float Encimavalordir3 { get; set; }
        public float Encimavalordir4 { get; set; }
        public float Encimavalorpat { get; set; }
        public float Encimavaloruna { get; set; }
        public bool Esvalorfijo { get; set; }
        public string Fechasorteo { get; set; }
        public float Iva { get; set; }
        public Listadoapuestas Listadoapuestas { get; set; }
        public string Serie1 { get; set; }
        public int Serie2 { get; set; }
        public float Totalapostado { get; set; }
        public float Totalpagado { get; set; }
        public float Totaliva { get; set; }
    }

    public class Listadoapuestas
    {
        public List<ApuestaNotifyC> Apuesta { get; set; }
    }

    public class ApuestaNotifyC
    {
        public Listadoloterias Listadoloterias { get; set; }
        public int Numeroapostado { get; set; }
        public float Valorapostadocombinado { get; set; }
        public float Valorapostadodirecto { get; set; }
        public float Valorapostadopata { get; set; }
        public float Valorapostadototal { get; set; }
        public float Valorapostadouna { get; set; }
        public float Valoriva { get; set; }
        public float Valortotal { get; set; }
    }

    public class Listadoloterias
    {
        public List<LoteriaNotifyC> Loteria { get; set; }
    }

    public class LoteriaNotifyC
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }
        public string Nombrecorto { get; set; }
    }

    public class Subproducto
    {
        public string Nombre { get; set; }
        public Producto Producto { get; set; }
    }

    public class Producto
    {
        public string Nombre { get; set; }
    }


    #endregion

    #region Recaudo

    #region GetRecaudo

    public class ResponseGetRecaudo
    {
        public Empresa Empresa { get; set; }
        public Errorfield errorField { get; set; }
        public bool Estado { get; set; }
        public Listadorecaudos Listadorecaudos { get; set; }
    }

    public class Listadorecaudos
    {
        public List<Listadorecaudosfield> Recaudo { get; set; }
    }

    public class Listadorecaudosfield
    {
        public int Codigo { get; set; }
        public int Codigosubproducto { get; set; }
        public float Iva { get; set; }
        public string Nombre { get; set; }
    }


    #endregion

    #region GetParameters



    public class ResponseGetParameters
    {
        public bool estadoField { get; set; }

        public Errorfield errorField { get; set; }


        public Recaudofield recaudoField { get; set; }
    }

    public class Recaudofield
    {
        public int codigoField { get; set; }
        public Listadoserviciosfield listadoserviciosField { get; set; }
    }

    public class Listadoserviciosfield
    {
        public Serviciofield servicioField { get; set; }
    }

    public class Serviciofield
    {
        public bool activoField { get; set; }
        public int diasdespuesvencimientoField { get; set; }
        public string horamodificacionrecaudoField { get; set; }
        public int idField { get; set; }
        public List<Listadocamposfield> listadocamposField { get; set; }
        public bool modificafecharecaudoField { get; set; }
        public bool validafechavencimientoField { get; set; }
        public bool validasaldoField { get; set; }
    }

    public class Listadocamposfield
    {
        public string nombreField { get; set; }
        public bool editableField { get; set; }
        public string formatosalidaField { get; set; }
        public int idField { get; set; }
        public string jsonvalidacionesField { get; set; }
        public string etiquetaField { get; set; }
        public string tipoCampoField { get; set; }
        public string tipodatoField { get; set; }
        public bool visibleField { get; set; }
    }



    #endregion

    #region GetConsultValue



    public class ResponseConsultValue
    {
        public bool estadoField { get; set; }

        public Errorfield errorField { get; set; }


        public RecaudofieldData recaudoField { get; set; }
    }

    public class RecaudofieldData
    {
        public int codigoField { get; set; }
        public ListadoserviciosfieldData listadoserviciosField { get; set; }
        public string nombreField { get; set; }
        public Listadoregistrosfield listadoregistrosField { get; set; }
    }

    public class ListadoserviciosfieldData
    {
        public ServiciofieldData servicioField { get; set; }
    }

    public class ServiciofieldData
    {
        public bool activoField { get; set; }
        public int diasdespuesvencimientoField { get; set; }
        public Horamodificacionrecaudofield horamodificacionrecaudoField { get; set; }
        public int idField { get; set; }
        public List<ListadocamposfieldData> listadocamposField { get; set; }
        public bool modificafecharecaudoField { get; set; }
        public bool validafechavencimientoField { get; set; }
        public bool validasaldoField { get; set; }
    }

    public class Horamodificacionrecaudofield
    {
    }

    public class ListadocamposfieldData
    {
        public string nombreField { get; set; }
        public bool editableField { get; set; }
        public string formatosalidaField { get; set; }
        public int idField { get; set; }
        public string jsonvalidacionesField { get; set; }
        public string etiquetaField { get; set; }
        public string tipodatoField { get; set; }
        public bool visibleField { get; set; }
    }

    public class Listadoregistrosfield
    {
        public List<Registrofield> registroField { get; set; }
    }

    public class Registrofield
    {
        public string etiquetaField { get; set; }
        public string valorField { get; set; }
    }




    #endregion

    #region GetNotifyPayment


    public class ResponseNotifyPayment
    {
        public int Codigoseguridad { get; set; }
        public EmpresaNotifyP Empresa { get; set; }
        public bool Estado { get; set; }
        public ErrorAlert Error { get; set; }
        public JerarquiaNotifyP Jerarquia { get; set; }
        public Listadomensajespublicitarios Listadomensajespublicitarios { get; set; }
        public Periodopago Periodopago { get; set; }
        public string Transaccionid { get; set; }
    }

    public class EmpresaNotifyP
    {
        public CiudadNotifyP Ciudad { get; set; }
        public string Direccion { get; set; }
        public int Nit { get; set; }
        public string Nombre { get; set; }
        public int Telefono { get; set; }
    }

    public class CiudadNotifyP
    {
        public int Codigo { get; set; }
        public DepartamentoNotifyP Departamento { get; set; }
        public string Nombre { get; set; }
    }

    public class DepartamentoNotifyP
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }
    }

    public class JerarquiaNotifyP
    {
        public int Celulaid { get; set; }
        public int Ciudadid { get; set; }
        public int Departamentooid { get; set; }
        public int Empresaid { get; set; }
        public int Equipoid { get; set; }
        public int Oficinaid { get; set; }
        public int Paisid { get; set; }
        public int Puntoventaid { get; set; }
        public int Subzonaid { get; set; }
        public int Usuarioid { get; set; }
        public int Zonaid { get; set; }
    }

    public class Listadomensajespublicitarios
    {
    }

    public class Periodopago
    {
    }





    #endregion


    #endregion

    #region PayerAlerts


    #region ResponseSendAlert


    public class ResponseSendAlert
    {
        public EmpresaAlert Empresa { get; set; }
        public ErrorAlert Error { get; set; }
        public bool Estado { get; set; }
    }

    public class EmpresaAlert
    {
        public CiudadAlert Ciudad { get; set; }
        public string Direccion { get; set; }
        public int Nit { get; set; }
        public string Nombre { get; set; }
        public int Telefono { get; set; }
    }

    public class CiudadAlert
    {
        public int Codigo { get; set; }
        public DepartamentoAlert Departamento { get; set; }
        public string Nombre { get; set; }
    }

    public class DepartamentoAlert
    {
        public int Codigo { get; set; }
        public string Nombre { get; set; }
    }

    public class ErrorAlert
    {
        public int codigo { get; set; }

        public string mensaje { get; set; }
    }


    #endregion

    #region ResponseSavePayer


    public class ResponseSavePayer
    {
        public bool Estado { get; set; }
        public Tercero Tercero { get; set; }
    }

    public class Tercero
    {
        public int Id { get; set; }
    }

    //Response ValidatePayer


    public class ResponseValidatePayer
    {
        [JsonProperty("codeError")]
        public int codeError { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<PayerData> data { get; set; }
    }


    //Response ValidatePayer


    public class ResponseInsertRecord
    {
        [JsonProperty("codeError")]
        public int codeError { get; set; }
        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public string data { get; set; }
    }




    public class PayerData
    {
        public int payer_Id { get; set; }
        public string identification { get; set; }
        public int identification_Type { get; set; }
        public string name { get; set; }
        public string birthday { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
    }



    #endregion



    #endregion



    #region ResponsePaquetes


    public class Listadosubproductospaquetes
    {
        public List<Paquete> Paquete { get; set; }
    }

    public class Paquete
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }

    public class ResponseConsultSubproductosPaquetes
    {
        public bool Estado { get; set; }
        public Listadosubproductospaquetes Listadosubproductospaquetes { get; set; }
    }


    public class Listadopaquetes
    {
        public List<Paquetes> Paquetes { get; set; }
    }

    public class Paquetes
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Nombrecorto { get; set; }
        public int Valor { get; set; }
    }

    public class ResponseConsultPaquetes
    {
        public bool Estado { get; set; }
        public Listadopaquetes Listadopaquetes { get; set; }
    }


    public class ResponseGuardarPaquetes
    {
        public bool Estado { get; set; }
        public string Transaccionid { get; set; }
    }








    #endregion



    public class ResponseCalificacion
    {
        [JsonProperty("codeError")]
        public int CodeError { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }


    }
}

