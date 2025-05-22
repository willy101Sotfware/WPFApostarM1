using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using WPFApostar.Domain.UIServices.ObjectIntegration;

namespace WPFApostar.Domain.UIServices.Integration
{
    public class ApostarInvoiceManager : IProcedureManagerApsotar
    {
        private readonly HttpClient _client;
        private readonly string _baseAddress;

        public ApostarInvoiceManager()
        {
            _baseAddress = AppConfig.Get("apiBaseAddress");
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseAddress);
            _client.DefaultRequestHeaders.Add("DashboardKeyId", AppConfig.Get("apiKeyId"));
            _client.Timeout = TimeSpan.FromMilliseconds(10000);
        }

        public async Task<ResponseGeneric> GetLotteries(RequestGetLotteries request)
        {
            try
            {
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");
                var url = AppConfig.Get("GetLotteries");

                var response = await _client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                
                return new ResponseGeneric
                {
                    ResponseCode = response.IsSuccessStatusCode ? EResponseCode.Success : EResponseCode.Error,
                    ResponseData = result,
                    ResponseMessage = response.IsSuccessStatusCode ? "Success" : "Error"
                };
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en GetLotteries", ex);
                return new ResponseGeneric
                {
                    ResponseCode = EResponseCode.Error,
                    ResponseMessage = ex.Message
                };
            }
        }

        public async Task<ResponseGeneric> ValidateChance(RequestValidateChance request)
        {
            try
            {
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");
                var url = AppConfig.Get("ValidateChance");

                var response = await _client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                
                return new ResponseGeneric
                {
                    ResponseCode = response.IsSuccessStatusCode ? EResponseCode.Success : EResponseCode.Error,
                    ResponseData = result,
                    ResponseMessage = response.IsSuccessStatusCode ? "Success" : "Error"
                };
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en ValidateChance", ex);
                return new ResponseGeneric
                {
                    ResponseCode = EResponseCode.Error,
                    ResponseMessage = ex.Message
                };
            }
        }

        public async Task<ResponseGeneric> SendAlert(RequestSendAlert request)
        {
            try
            {
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");
                var url = AppConfig.Get("SendAlert");

                var response = await _client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                
                return new ResponseGeneric
                {
                    ResponseCode = response.IsSuccessStatusCode ? EResponseCode.Success : EResponseCode.Error,
                    ResponseData = result,
                    ResponseMessage = response.IsSuccessStatusCode ? "Success" : "Error"
                };
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en SendAlert", ex);
                return new ResponseGeneric
                {
                    ResponseCode = EResponseCode.Error,
                    ResponseMessage = ex.Message
                };
            }
        }

        public async Task<ResponseSavePayer> SavePayer(RequestSavePayer request)
        {
            try
            {
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");
                var url = AppConfig.Get("SaveUser");

                var response = await _client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                
                return JsonConvert.DeserializeObject<ResponseSavePayer>(result);
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en SavePayer", ex);
                return new ResponseSavePayer
                {
                    Estado = false
                };
            }
        }

        public async Task<ResponseValidatePayer> ValidatePayer(RequestValidatePayer request)
        {
            try
            {
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");
                var url = AppConfig.Get("ValidatePayForPayer");

                var response = await _client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                
                return JsonConvert.DeserializeObject<ResponseValidatePayer>(result);
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en ValidatePayer", ex);
                return new ResponseValidatePayer
                {
                    codeError = 500,
                    message = ex.Message
                };
            }
        }

        public async Task<ResponseInsertRecord> InsertRecord(RequestInsertRecord request)
        {
            try
            {
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");
                var url = AppConfig.Get("SavePayer");

                var response = await _client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                
                return JsonConvert.DeserializeObject<ResponseInsertRecord>(result);
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en InsertRecord", ex);
                return new ResponseInsertRecord
                {
                    codeError = 500,
                    message = ex.Message
                };
            }
        }

        public async Task<ResponseConsultSubproductosPaquetes> ConsultSubproductosPaquetes(RequestConsultSubproductosPaquetes request)
        {
            try
            {
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");
                var url = AppConfig.Get("ConsultSubproductosPaquetes");

                var response = await _client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                
                return JsonConvert.DeserializeObject<ResponseConsultSubproductosPaquetes>(result);
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en ConsultSubproductosPaquetes", ex);
                return new ResponseConsultSubproductosPaquetes
                {
                    Estado = false
                };
            }
        }

        public async Task<ResponseConsultPaquetes> ConsultPaquetes(RequestConsultPaquetes request)
        {
            try
            {
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");
                var url = AppConfig.Get("ConsultarPaquetes");

                var response = await _client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                
                return JsonConvert.DeserializeObject<ResponseConsultPaquetes>(result);
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en ConsultPaquetes", ex);
                return new ResponseConsultPaquetes
                {
                    Estado = false
                };
            }
        }

        public async Task<ResponseGuardarPaquetes> GuardarPaquete(RequestGuardarPaquete request)
        {
            try
            {
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");
                var url = AppConfig.Get("GuardarPaquetes");

                var response = await _client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                
                return JsonConvert.DeserializeObject<ResponseGuardarPaquetes>(result);
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en GuardarPaquete", ex);
                return new ResponseGuardarPaquetes
                {
                    Estado = false
                };
            }
        }

        public async Task<ResponseTokenBetplay> GetTokenBetplay(RequesttokenBetplay request)
        {
            try
            {
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");
                var url = AppConfig.Get("GetTokenBetplay");

                var response = await _client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                
                return JsonConvert.DeserializeObject<ResponseTokenBetplay>(result);
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en GetTokenBetplay", ex);
                return new ResponseTokenBetplay();
            }
        }

        public async Task<ResponseGetProducts> ConsultSubproductBetplay(RequestConsultSubproductBetplay request)
        {
            try
            {
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");
                var url = AppConfig.Get("GetProductsBetPlay");

                var response = await _client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                
                return JsonConvert.DeserializeObject<ResponseGetProducts>(result);
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en ConsultSubproductBetplay", ex);
                return new ResponseGetProducts
                {
                    Estado = false
                };
            }
        }

        public async Task<ResponseNotifyPayment> NotifyPayment(RequestNotifyRecaudo request)
        {
            try
            {
                string payload = JsonConvert.SerializeObject(request);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");
                var url = AppConfig.Get("NotifyPay");

                var response = await _client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                
                return JsonConvert.DeserializeObject<ResponseNotifyPayment>(result);
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en NotifyPayment", ex);
                return new ResponseNotifyPayment
                {
                    Estado = false
                };
            }
        }

        public Task<ResponseGeneric> GetLotteries(RequestGetLotteries request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseGeneric> ValidateChance(RequestValidateChance request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseGeneric> SendAlert(RequestSendAlert request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseSavePayer> SavePayer(RequestSavePayer request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseValidatePayer> ValidatePayer(RequestValidatePayer request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseInsertRecord> InsertRecord(RequestInsertRecord request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseConsultSubproductosPaquetes> ConsultSubproductosPaquetes(RequestConsultSubproductosPaquetes request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseConsultPaquetes> ConsultPaquetes(RequestConsultPaquetes request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseGuardarPaquetes> GuardarPaquete(RequestGuardarPaquete request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseTokenBetplay> GetTokenBetplay(RequesttokenBetplay request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseGetProducts> ConsultSubproductBetplay(RequestConsultSubproductBetplay request)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseNotifyPayment> NotifyPayment(RequestNotifyRecaudo request)
        {
            throw new NotImplementedException();
        }
    }
}
