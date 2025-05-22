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
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public Empresa Empresa { get; set; }
    }

    public class Empresa
    {
        public string Nombre { get; set; }
        public string Nit { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
    }

    public class Listadosubproductos
    {
        public SubProductoGeneral[] Subproductos { get; set; }
    }

    public class SubProductoGeneral
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Valor { get; set; }
    }

    public class ResponseNotifyPayment
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public bool Estado { get; set; }
    }

    public class ResponseSavePayer
    {
        public bool Estado { get; set; }
        public Tercero Tercero { get; set; }
    }

    public class Tercero
    {
        public string Id { get; set; }
    }

    public class ResponseValidatePayer
    {
        public int codeError { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }

    public class ResponseInsertRecord
    {
        public int codeError { get; set; }
        public string message { get; set; }
        public PayerData data { get; set; }
    }

    public class PayerData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
    }

    public class ResponseConsultSubproductosPaquetes
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public Listadosubproductospaquetes Data { get; set; }
    }

    public class Listadosubproductospaquetes
    {
        public Paquete[] Paquetes { get; set; }
    }

    public class Paquete
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
    }

    public class ResponseConsultPaquetes
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public Listadopaquetes Data { get; set; }
    }

    public class Listadopaquetes
    {
        public Paquetes[] Paquetes { get; set; }
    }

    public class Paquetes
    {
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public decimal Valor { get; set; }
    }

    public class ResponseGuardarPaquetes
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }

    #endregion
}
