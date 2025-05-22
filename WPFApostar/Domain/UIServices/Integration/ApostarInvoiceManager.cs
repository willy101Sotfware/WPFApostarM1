using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WPFApostar.Domain.ApiService.QueueModels;
using WPFApostar.Domain.UIServices.ObjectIntegration;

namespace WPFApostar.Domain.UIServices.Integration
{
    public class ApostarInvoiceManager : IProcedureManagerApsotar
    {
        private HttpClient _client;
        private string _baseAddress;
        private readonly Transaction _ts;

        public ApostarInvoiceManager()
        {
            _baseAddress = AppConfig.Get("apiBaseAddress");
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseAddress);
            _client.DefaultRequestHeaders.Add("DashboardKeyId", AppConfig.Get("apiKeyId"));
            _client.Timeout = TimeSpan.FromMilliseconds(10000);
            _ts = Transaction.Instance;
        }

        private async Task<ResponseApi> GetData(object requestData, string controller)
        {
            try
            {
                string payload = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(payload, Encoding.UTF8, "Application/json");
                var url = AppConfig.Get(controller);

                var response = await _client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();
                
                return new ResponseApi
                {
                    codeError = response.IsSuccessStatusCode ? 200 : 400,
                    data = result,
                    message = response.IsSuccessStatusCode ? "Success" : "Error"
                };
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, $"Error en GetData: {ex.Message}", ex);
                return new ResponseApi
                {
                    codeError = 500,
                    message = ex.Message
                };
            }
        }

        public async Task<ResponseApi> GetLotteries(RequestGetLotteries request)
        {
            try
            {
                var response = await GetData(request, "GetLotteries");
                
                if (response == null)
                {
                    EventLogger.SaveLog(EventType.Error, "Error en GetLotteries: respuesta nula");
                    return new ResponseApi
                    {
                        codeError = 400,
                        message = "Error en la consulta",
                        data = null
                    };
                }
                
                if (response.codeError == 200)
                {
                    return new ResponseApi
                    {
                        codeError = 200,
                        data = response.data,
                        message = "Success"
                    };
                }
                else
                {
                    EventLogger.SaveLog(EventType.Error, $"Error en GetLotteries: {response.message}");
                    return new ResponseApi
                    {
                        codeError = 400,
                        message = response.message,
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en GetLotteries", ex);
                return new ResponseApi
                {
                    codeError = 400,
                    message = ex.Message,
                    data = null
                };
            }
        }

        public async Task<ResponseApi> ValidateChance(RequestValidateChance request)
        {
            try
            {
                var response = await GetData(request, "ValidateChance");

                if (response == null)
                {
                    EventLogger.SaveLog(EventType.Error, "Error en ValidateChance: respuesta nula");
                    return new ResponseApi
                    {
                        codeError = 400,
                        message = "Error en la consulta",
                        data = null
                    };
                }

                if (response.codeError == 200)
                {
                    return new ResponseApi
                    {
                        codeError = 200,
                        data = response.data,
                        message = "Success"
                    };
                }
                else
                {
                    EventLogger.SaveLog(EventType.Error, $"Error en ValidateChance: {response.message}");
                    return new ResponseApi
                    {
                        codeError = 400,
                        message = response.message,
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en ValidateChance", ex);
                return new ResponseApi
                {
                    codeError = 400,
                    message = ex.Message,
                    data = null
                };
            }
        }

        public async Task<ResponseApi> SendAlert(RequestSendAlert request)
        {
            try
            {
                var response = await GetData(request, "SendAlert");

                if (response == null)
                {
                    EventLogger.SaveLog(EventType.Error, "Error en SendAlert: respuesta nula");
                    return new ResponseApi
                    {
                        codeError = 400,
                        message = "Error al enviar alerta",
                        data = null
                    };
                }

                if (response.codeError == 200)
                {
                    return new ResponseApi
                    {
                        codeError = 200,
                        data = response.data,
                        message = "Success"
                    };
                }
                else
                {
                    EventLogger.SaveLog(EventType.Error, $"Error en SendAlert: {response.message}");
                    return new ResponseApi
                    {
                        codeError = 400,
                        message = response.message,
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                EventLogger.SaveLog(EventType.Error, "Error en SendAlert", ex);
                return new ResponseApi
                {
                    codeError = 400,
                    message = ex.Message,
                    data = null
                };
            }
        }

        public async Task<ResponseSavePayer> SavePayer(RequestSavePayer request)
        {
            try
            {
                var response = await GetData(request, "SaveUser");

                if (response == null)
                {
                    EventLogger.SaveLog(EventType.Error, "Error en SavePayer: respuesta nula");
                    return new ResponseSavePayer
                    {
                        Estado = false
                    };
                }

                if (response.codeError == 200)
                {
                    return JsonConvert.DeserializeObject<ResponseSavePayer>(response.data.ToString());
                }
                else
                {
                    EventLogger.SaveLog(EventType.Error, $"Error en SavePayer: {response.message}");
                    return new ResponseSavePayer
                    {
                        Estado = false
                    };
                }
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
                var response = await GetData(request, "ValidatePayForPayer");

                if (response == null)
                {
                    EventLogger.SaveLog(EventType.Error, "Error en ValidatePayer: respuesta nula");
                    return new ResponseValidatePayer
                    {
                        codeError = 500,
                        message = "Error en la validación"
                    };
                }

                if (response.codeError == 200)
                {
                    return JsonConvert.DeserializeObject<ResponseValidatePayer>(response.data.ToString());
                }
                else
                {
                    EventLogger.SaveLog(EventType.Error, $"Error en ValidatePayer: {response.message}");
                    return new ResponseValidatePayer
                    {
                        codeError = 500,
                        message = response.message
                    };
                }
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
                var response = await GetData(request, "SavePayer");

                if (response == null)
                {
                    EventLogger.SaveLog(EventType.Error, "Error en InsertRecord: respuesta nula");
                    return new ResponseInsertRecord
                    {
                        codeError = 500,
                        message = "Error al insertar registro"
                    };
                }

                if (response.codeError == 200)
                {
                    return JsonConvert.DeserializeObject<ResponseInsertRecord>(response.data.ToString());
                }
                else
                {
                    EventLogger.SaveLog(EventType.Error, $"Error en InsertRecord: {response.message}");
                    return new ResponseInsertRecord
                    {
                        codeError = 500,
                        message = response.message
                    };
                }
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
                var response = await GetData(request, "ConsultSubproductosPaquetes");

                if (response == null)
                {
                    EventLogger.SaveLog(EventType.Error, "Error en ConsultSubproductosPaquetes: respuesta nula");
                    return new ResponseConsultSubproductosPaquetes
                    {
                        Estado = false
                    };
                }

                if (response.codeError == 200)
                {
                    return JsonConvert.DeserializeObject<ResponseConsultSubproductosPaquetes>(response.data.ToString());
                }
                else
                {
                    EventLogger.SaveLog(EventType.Error, $"Error en ConsultSubproductosPaquetes: {response.message}");
                    return new ResponseConsultSubproductosPaquetes
                    {
                        Estado = false
                    };
                }
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
                var response = await GetData(request, "ConsultarPaquetes");

                if (response == null)
                {
                    EventLogger.SaveLog(EventType.Error, "Error en ConsultPaquetes: respuesta nula");
                    return new ResponseConsultPaquetes
                    {
                        Estado = false
                    };
                }

                if (response.codeError == 200)
                {
                    return JsonConvert.DeserializeObject<ResponseConsultPaquetes>(response.data.ToString());
                }
                else
                {
                    EventLogger.SaveLog(EventType.Error, $"Error en ConsultPaquetes: {response.message}");
                    return new ResponseConsultPaquetes
                    {
                        Estado = false
                    };
                }
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
                var response = await GetData(request, "GuardarPaquetes");

                if (response == null)
                {
                    EventLogger.SaveLog(EventType.Error, "Error en GuardarPaquete: respuesta nula");
                    return new ResponseGuardarPaquetes
                    {
                        Estado = false
                    };
                }

                if (response.codeError == 200)
                {
                    return JsonConvert.DeserializeObject<ResponseGuardarPaquetes>(response.data.ToString());
                }
                else
                {
                    EventLogger.SaveLog(EventType.Error, $"Error en GuardarPaquete: {response.message}");
                    return new ResponseGuardarPaquetes
                    {
                        Estado = false
                    };
                }
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
                var response = await GetData(request, "GetTokenBetplay");

                if (response == null)
                {
                    EventLogger.SaveLog(EventType.Error, "Error en GetTokenBetplay: respuesta nula");
                    return new ResponseTokenBetplay();
                }

                if (response.codeError == 200)
                {
                    return JsonConvert.DeserializeObject<ResponseTokenBetplay>(response.data.ToString());
                }
                else
                {
                    EventLogger.SaveLog(EventType.Error, $"Error en GetTokenBetplay: {response.message}");
                    return new ResponseTokenBetplay();
                }
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
                var response = await GetData(request, "GetProductsBetPlay");

                if (response == null)
                {
                    EventLogger.SaveLog(EventType.Error, "Error en ConsultSubproductBetplay: respuesta nula");
                    return new ResponseGetProducts
                    {
                        Estado = false
                    };
                }

                if (response.codeError == 200)
                {
                    return JsonConvert.DeserializeObject<ResponseGetProducts>(response.data.ToString());
                }
                else
                {
                    EventLogger.SaveLog(EventType.Error, $"Error en ConsultSubproductBetplay: {response.message}");
                    return new ResponseGetProducts
                    {
                        Estado = false
                    };
                }
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
                var response = await GetData(request, "NotifyPay");

                if (response == null)
                {
                    EventLogger.SaveLog(EventType.Error, "Error en NotifyPayment: respuesta nula");
                    return new ResponseNotifyPayment
                    {
                        Estado = false
                    };
                }

                if (response.codeError == 200)
                {
                    return JsonConvert.DeserializeObject<ResponseNotifyPayment>(response.data.ToString());
                }
                else
                {
                    EventLogger.SaveLog(EventType.Error, $"Error en NotifyPayment: {response.message}");
                    return new ResponseNotifyPayment
                    {
                        Estado = false
                    };
                }
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
    }
}
