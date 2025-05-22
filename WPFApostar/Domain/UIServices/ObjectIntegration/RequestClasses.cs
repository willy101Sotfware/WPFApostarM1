using System;
using System.Collections.Generic;

namespace WPFApostar.Domain.UIServices.ObjectIntegration
{
    // Clases de solicitud para la API
    public class RequestGetLotteries
    {
        public string UserId { get; set; }
    }

    public class RequestValidateChance
    {
        public string ChanceId { get; set; }
        public string UserId { get; set; }
    }

    public class RequestSendAlert
    {
        public string UserId { get; set; }
        public string Message { get; set; }
    }

    public class RequestSavePayer
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Document { get; set; }
    }

    public class RequestValidatePayer
    {
        public string UserId { get; set; }
        public string Document { get; set; }
    }

    public class RequestInsertRecord
    {
        public string UserId { get; set; }
        public string RecordData { get; set; }
    }

    public class RequestConsultSubproductosPaquetes
    {
        public string UserId { get; set; }
    }

    public class RequestConsultPaquetes
    {
        public string UserId { get; set; }
    }

    public class RequestGuardarPaquete
    {
        public string UserId { get; set; }
        public string PackageData { get; set; }
    }

    public class RequesttokenBetplay
    {
        public string UserId { get; set; }
    }

    public class RequestConsultSubproductBetplay
    {
        public string UserId { get; set; }
    }

    public class RequestNotifyRecaudo
    {
        public string UserId { get; set; }
        public string PaymentData { get; set; }
    }
}
