namespace WPFApostar.Domain.UIServices.ObjectIntegration
{


    public class NumeroChance
    {
        public string numero { get; set; }
        public int directo { get; set; }
        public int una { get; set; }
        public int pata { get; set; }
        public int combinado { get; set; }
    }

    public class LoteriaChance
    {
        public string idLoteria { get; set; }
        public string desLoteria { get; set; }
        public string sorteo { get; set; }
        public string Abrv { get; set; }
        public string nombre { get; set; }
    }

    public class LoteriaLiquidar
    {
        public string idLoteria { get; set; }
        public string sorteo { get; set; }
    }

    //Request Getlotteries

    public class RequestGetLotteries
    {
        public string Ubicacion { get; set; }
        public int Id { get; set; }

        public string FechaS { get; set; }

        public long transaccion { get; set; }
    }


    //Request TypeChance

    public class IdProducto
    {
        public string Ubicacion { get; set; }
        public int Id { get; set; }

        public string FechaS { get; set; }

        public long transaccion { get; set; }
    }


    //Request Validate Chance

    public class RequestValidateChance
    {
        public string Ubicacion { get; set; }
        public SubproductoValidate Subproducto { get; set; }

        public ListApuestasValidate LstApuestas { get; set; }

        public bool AsumeIva { get; set; }

        public string FechaSorteo { get; set; }

        public int Transaccion { get; set; }

        public string codigoApostar { get; set; }

        public string idPagador { get; set; }

        public string cedula { get; set; }
    }

    public class SubproductoValidate
    {
        public int Id { get; set; }

        public long CodigoServicio { get; set; }
    }

    public class ListApuestasValidate
    {
        public List<ApuestasValidate> apuestas { get; set; }
    }



    public class ApuestasValidate
    {
        public int id { get; set; }

        public string NumeroApostado { get; set; }

        public int ValorDirecto { get; set; }

        public int ValorCombinado { get; set; }
        public int ValorPata { get; set; }
        public int ValorUna { get; set; }

        public TipoChanceValidateM tipoChance { get; set; }

        public ListLoteriasValidate ListLoteriasValidate { get; set; }
    }

    public class TipoChanceValidateM
    {
        public int Id { get; set; }
    }

    public class ListLoteriasValidate
    {
        public List<LoteriaValidate> loteria { get; set; }
    }

    public class LoteriaValidate
    {
        public string codigo { get; set; }
    }


    public class RequestSubproducts
    {
        public string Ubicacion { get; set; }

      
    }

    //request NotifyChance

    public class RequestNotifyChance
    {
        public string Ubicacion { get; set; }
        public SubproductoNotify Subproducto { get; set; }

        public ListApuestasNotify LstApuestas { get; set; }

        public bool AsumeIva { get; set; }

        public string FechaSorteo { get; set; }

        public int Transaccion { get; set; }

        public string codigoApostar { get; set; }

        public string idPagador { get; set; }

        public string cedula { get; set; }

        public long Usuarioid { get; set; }
    }

    public class SubproductoNotify
    {
        public int Id { get; set; }

        public long CodigoServicio { get; set; }
    }

    public class ListApuestasNotify
    {
        public List<ApuestasNotify> apuestas { get; set; }
    }

    public class ApuestasNotify
    {
        public int id { get; set; }

        public string NumeroApostado { get; set; }
        public int ValorDirecto { get; set; }
        public int ValorCombinado { get; set; }
        public int ValorPata { get; set; }
        public int ValorUna { get; set; }

        public TipoChanceNotifyM tipoChance { get; set; }

        public ListLoteriasNotify ListLoteriasValidate { get; set; }
    }

    public class TipoChanceNotifyM
    {
        public int Id { get; set; }
    }

    public class ListLoteriasNotify
    {
        public List<LoteriaNotify> loteria { get; set; }
    }

    public class LoteriaNotify
    {
        public string codigo { get; set; }
    }


    //BetPlay

    //Request Trama para notificar la recarga
    public class RequestNotifyBetplay

    { 
    public string Ubicacion { get; set; }
    public string Token { get; set; }
        public ulong ClienteId { get; set; }

        public subproducto subproducto { get; set; }

        public int valor { get; set; }

        public ulong transaccionId { get; set; }

        public long Usuarioid { get; set; }
    }

    public class subproducto
    {
        public int Id { get; set; }
    }

    //Recaudo

    //Request GetRecaudos

    public class RequestGetRecaudos
    {
        public int codigo { get; set; }

        public double IdTrx { get; set; }
    }

    //Request Consult Value


    public class RequestConsultValue
    {
        public Iddepartamento idDepartamento { get; set; }
        public Recaudo Recaudo { get; set; }
        public int IdTrx { get; set; }
        public int Servicio { get; set; }
        public Listadocamposm ListadoCamposM { get; set; }
    }

    public class requestIdDepartamento
    {
        public int codigo { get; set; }
    }

    public class RecaudoData
    {
        public int codigo { get; set; }
    }

    public class Listadocamposm
    {
        public List<Camposm> camposM { get; set; }
    }

    public class Camposm
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string valor { get; set; }
    }


    //Request GetParameters


    public class RequestGetParameters
    {
        public Iddepartamento idDepartamento { get; set; }
        public Recaudo recaudo { get; set; }
        public int IdTrx { get; set; }
    }

    public class Iddepartamento
    {
        public int codigo { get; set; }
    }

    public class Recaudo
    {
        public int codigo { get; set; }
    }


    //Request NotifyPayment

    public class RequestNotifyRecaudo
    {
        public Iddepartamento IdDepartamento { get; set; }
        public RecaudoNotify Recaudo { get; set; }
        public int IdTrx { get; set; }
        public Servicionotify ServicioNotify { get; set; }
    }

    public class RecaudoNotify
    {
        public int codigo { get; set; }
        public int valor { get; set; }
    }

    public class Servicionotify
    {
        public int id { get; set; }
        public Listadocamponotify listadoCampoNotify { get; set; }
    }

    public class Listadocamponotify
    {
        public List<Camposm> camposM { get; set; }
    }


    //Request SavePayer


    public class RequestSavePayer
    {
        public int identificacion { get; set; }
        public string NombreUser { get; set; }
        public string Correo { get; set; }
        public int transaccion { get; set; }
    }

    //Request SendAlert


    public class RequestSendAlert
    {
        public string TransaccionChance { get; set; }
        public int transaccion { get; set; }

        public string codigoApostar { get; set; }

        public string idPagador { get; set; }

        public string cedula { get; set; }
    }

    //Request RequestValidatePayer

    public class RequestValidatePayer
    {
        public string identificacion { get; set; }


    }

    //Request RequestInsertRecord

    public class RequestInsertRecord
    {
        public string identificacion { get; set; }
        public int tipo_Identificacion { get; set; }
        public string nombre_Completo { get; set; }
        public string fecha_Nacimiento { get; set; }
        public string numero_Celular { get; set; }
        public string email { get; set; }

    }


    //Request Recargas Paquetes


    public class RequestConsultSubproductosPaquetes
    {
        public string Ubicacion { get; set; }
        public int Transacciondistribuidorid { get; set; }
    }


    public class RequestConsultPaquetes
    {
        public string Ubicacion { get; set; }
        public int Transacciondistribuidorid { get; set; }
        public string Codigosubproducto { get; set; }
    }


    public class RequestGuardarPaquete
    {
        public string Ubicacion { get; set; }
        public string Codigosubproducto { get; set; }
        public int Valor { get; set; }
        public string Numero { get; set; }
        public string Id { get; set; }
        public int Transacciondistribuidorid { get; set; }

        public long Usuarioid { get; set; }
    }


    public class RequesttokenBetplay
    {
        public string Ubicacion { get; set; }
        public string Transacciondistribuidorid { get; set; }
        public string Transaccionclienteid { get; set; }
    }

    public class RequestConsultSubproductBetplay
    {
        public string Ubicacion { get; set; }
        public string Transacciondistribuidorid { get; set; }
        public string Token { get; set; }
    }


}
