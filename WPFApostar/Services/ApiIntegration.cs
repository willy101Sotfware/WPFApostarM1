using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection;
using System.Text;
using WPFApostar.Classes;
using WPFApostar.Services.ObjectIntegration;

namespace WPFApostar.Services
{
    public class ApiIntegration
    {

        private HttpClient client;
        private string basseAddress;

        private static string Aplicacion;
        private static string Dispositivo;
        private static string Token;
        private static string KeyIntegration;

        public ApiIntegration(string dispositivo)
        {
            Aplicacion = Assembly.GetCallingAssembly().GetName().Name;
            Dispositivo = dispositivo;
        }

        public async Task<ResponseGeneric> GetData(object requestData, string controller, string BaseAddress)
        {
            try
            {

                HttpResponseMessage response = new HttpResponseMessage();

                client = new HttpClient();
                var request = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(request, Encoding.UTF8, "Application/json");
                client.BaseAddress = new Uri(basseAddress);

                response = client.PostAsync(controller, content).GetAwaiter().GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("Error en el servicio del cliente", response.RequestMessage.ToString(), EError.Customer, ELevelError.Medium);
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();

                var ResponseData = JsonConvert.DeserializeObject<ResponseGeneric>(result);

                //if (result != null)
                //{
                //    return result;
                //}

                return ResponseData;
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion GetData Catch", "Fail", ex.Message, null);
            }
            return null;
        }

        public async Task<ResponseGeneric> GetData(string controller, string BaseAddress)
        {
            try
            {

                HttpResponseMessage response = new HttpResponseMessage();

                client = new HttpClient();
                //var request = JsonConvert.SerializeObject(requestData);
                var content = new StringContent("dato", Encoding.UTF8, "Application/json");
                client.BaseAddress = new Uri(basseAddress);
              
               response = client.PostAsync(controller, content).GetAwaiter().GetResult();
               
                if (!response.IsSuccessStatusCode)
                {
                    AdminPayPlus.SaveErrorControl("Error en el servicio del cliente", response.RequestMessage.ToString(), EError.Customer, ELevelError.Medium);
                    return null;
                }

                var result = await response.Content.ReadAsStringAsync();

                var requestData = JsonConvert.DeserializeObject<ResponseGeneric>(result);


                return requestData;

            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion GetData Catch", "Fail", ex.Message, null);
            }
            return null;
        }

        #region ApiBetPlay


        public async Task<ResponseTokenBetplay> GetTokenBetplay(RequesttokenBetplay requesttoken)
        {
            try
            {
                string controller = Utilities.GetConfiguration("GetTokenBetplay");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = await GetData(requesttoken, controller, basseAddress);

                if (response != null)
                {

                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    var ResponseData = JsonConvert.DeserializeObject<ResponseTokenBetplay>(x);

                    AdminPayPlus.SaveLog("ApiIntegration", "Respuesta al metodo GetConsultBetplay", "OK", x, null);


                    if (ResponseData.Token  != null)
                    {

                        return ResponseData;
                    }
                    else
                    {
                        return null;
                    }


                }
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("Response Service GetConsultBetplay Catch : " + ex.Message + null);
            }
            return null;
        }




        public async Task<ResponseGetProducts> GetProductsBetPlay(RequestConsultSubproductBetplay request)
        {
            try
            {
                string controller = Utilities.GetConfiguration("GetProductsBetPlay");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = await GetData(request, controller, basseAddress);

                if (response != null)
                {

                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    var ResponseData = JsonConvert.DeserializeObject<ResponseGetProducts>(x);

                    AdminPayPlus.SaveLog("ApiIntegration", "Respuesta a la ejecucion GetProductsBetPlay Response", "OK", x, null);


                    return ResponseData;

                }
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion GetProductsBetPlay Catch", "Fail", ex.Message, null);
            }
            return null;
        }


        public ResponseNotifyBetPlay NotifyPayment(RequestNotifyBetplay Machine)
        {
            try
            {

                string controller = Utilities.GetConfiguration("NotifyRecharge");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = GetData(Machine, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {


                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    string jsonLimpio = System.Text.RegularExpressions.Regex.Unescape(x).Trim('"');

                    jsonLimpio = jsonLimpio.Replace(@"\", "");

                    var requestData = JsonConvert.DeserializeObject<ResponseNotifyBetPlay>(jsonLimpio);

                    AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion NotifyPayment Response", "OK", x, null);


                    return requestData;

                }
            }
               catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion NotifyPayment Catch", "Fail", ex.Message, null);
            }
            return null;
        }

        #endregion

        //Chance

        #region ApiChance

        public ResponseGetProducts GetProductsChance(RequestSubproducts request)
        {
            try
            {


                string controller = Utilities.GetConfiguration("GetProductsChance");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = GetData(request, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    var ResponseData = JsonConvert.DeserializeObject<ResponseGetProducts>(x);

                    AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion GetProductsChance Response", "OK", x, null);


                    return ResponseData;

                }
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion GetProductsChance Catch", "Fail", ex.Message, null);
            }
            return null;
        }

        public ResponseGetLotteries GetLotteries(RequestGetLotteries Machine)
        {
            try
            {
                var y = JsonConvert.SerializeObject(Machine);

                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion GetLotteries Request", "OK", y, null);

                string controller = Utilities.GetConfiguration("GetLotteries");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = GetData(Machine, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    var requestData = JsonConvert.DeserializeObject<ResponseGetLotteries>(x);

                    AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion GetLotteries Response", "OK", x, null);

                    return requestData; 
                }
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion GetLotteries Catch", "Fail", ex.Message, null);
            }
            return null;
        }

        public ResponseTypeChance TypeChance(IdProducto Machine)
        {
            try
            {
                var y = JsonConvert.SerializeObject(Machine);

                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion TypeChance Request", "OK", y, null);

                string controller = Utilities.GetConfiguration("TypeChance");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = GetData(Machine, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    var requestData = JsonConvert.DeserializeObject<ResponseTypeChance>(x);

                    AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion TypeChance Response", "OK", x, null);

                    return requestData;
                }
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion TypeChance Catch", "Fail", ex.Message, null);
            }
            return null;
        }

        public ResponseValidateChance ValidateChance(RequestValidateChance Machine)
        {
            try
            {

                var y = JsonConvert.SerializeObject(Machine);

                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion ValidateChance Request", "OK", y, null);

                string controller = Utilities.GetConfiguration("ValidateChance");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = GetData(Machine, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    var requestData = JsonConvert.DeserializeObject<ResponseValidateChance>(x);

                    AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion ValidateChance Response", "OK", x, null);

                    return requestData;
                }
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion ValidateChance Catch", "Fail", ex.Message, null);
            }
            return null;
        }

        public ResponseNotifyChance NotifyChance(RequestNotifyChance Machine)
        {
            try
            {

                var y = JsonConvert.SerializeObject(Machine);

                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion NotifyChance Request", "OK", y, null);

                string controller = Utilities.GetConfiguration("NotifyChance");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = GetData(Machine, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    var requestData = JsonConvert.DeserializeObject<ResponseNotifyChance>(x);

                    AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion NotifyChance Response", "OK", x, null);

                    return requestData;
                }
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion NotifyChance Catch", "Fail", ex.Message, null);
            }
            return null;
        }

        //Payer and SendAlert

        public ResponseSavePayer SavePayerChance(RequestSavePayer Machine)
        {
            try
            {
                string controller = Utilities.GetConfiguration("SaveUser");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = GetData(Machine, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    var requestData = JsonConvert.DeserializeObject<ResponseSavePayer>(x);

                    AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion SavePayerChance Response", "OK", x, null);

                    return requestData;
                }
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion SavePayerChance Catch", "Fail", ex.Message, null);
            }
            return null;
        }

        public ResponseSendAlert SendAlert(RequestSendAlert Machine)
        {
            try
            {
                string controller = Utilities.GetConfiguration("SendAlert");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = GetData(Machine, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {
                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    var requestData = JsonConvert.DeserializeObject<ResponseSendAlert>(x);

                    AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion SendAlert Response", "OK", x, null);

                    return requestData;
                }
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion SendAlert Catch", "Fail", ex.Message, null);
            }
            return null;
        }

        #endregion

        #region ApiRecaudo

        public ResponseGetRecaudo GetRecaudos(RequestGetRecaudos request)
        {
            try
            {
                string controller = Utilities.GetConfiguration("GetRecaudo");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = GetData(request,controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {

                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    var ResponseData = JsonConvert.DeserializeObject<ResponseGetRecaudo>(x);

                    AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion GetRecaudos Response", "OK", x, null);

                    return ResponseData;

                }
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion GetRecaudos Catch", "Fail", ex.Message, null);
            }
            return null;
        }

        public ResponseGetParameters GetParameters(RequestGetParameters request)
        {
            try
            {
                string controller = Utilities.GetConfiguration("GetParameters");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = GetData(request, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {

                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    var ResponseData = JsonConvert.DeserializeObject<ResponseGetParameters>(x);

                    AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion GetParameters Response", "OK", x, null);

                    return ResponseData;

                }
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion GetParameters Catch", "Fail", ex.Message, null);
            }
            return null;
        }

        public ResponseConsultValue ConsultValueRecaudo(RequestConsultValue request)
        {
            try
            {
                string controller = Utilities.GetConfiguration("ConsultRecaudo");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = GetData(request, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {

                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    var ResponseData = JsonConvert.DeserializeObject<ResponseConsultValue>(x);

                    AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion ConsultValueRecaudo Response", "OK", x, null);


                    return ResponseData;

                }
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion ConsultValueRecaudo Catch", "Fail", ex.Message, null);
            }
            return null;
        }

        public ResponseNotifyPayment NotifyPaymentRecaudo(RequestNotifyRecaudo request)
        {
            try
            {
                var y = JsonConvert.SerializeObject(request);

                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion NotifyPaymentRecaudo Request", "OK", y, null);

                string controller = Utilities.GetConfiguration("NotifyPay");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = GetData(request, controller, basseAddress).GetAwaiter().GetResult();

                if (response != null)
                {

                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    var ResponseData = JsonConvert.DeserializeObject<ResponseNotifyPayment>(x);

                    AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion NotifyPaymentRecaudo Response", "OK", x, null);


                    return ResponseData;

                }
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion NotifyPaymentRecaudo Catch", "Fail", ex.Message, null);
            }
            return null;
        }




        #endregion


        #region ApiPaquetes

        public async Task<ResponseConsultSubproductosPaquetes> ConsultSubproductosPaquetes(RequestConsultSubproductosPaquetes request)
        {
            try
            {


                var y = JsonConvert.SerializeObject(request);

                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion ConsultSubproductosPaquetes Request", "OK", y, null);


                string controller = Utilities.GetConfiguration("ConsultSubproductosPaquetes");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = await GetData(request, controller, basseAddress);

                if (response != null)
                {

                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion ConsultSubproductosPaquetes Response", "OK", x, null);

                    string jsonLimpio = System.Text.RegularExpressions.Regex.Unescape(x).Trim('"');

                    jsonLimpio = jsonLimpio.Replace(@"\", "");

                    var ResponseData = JsonConvert.DeserializeObject<ResponseConsultSubproductosPaquetes>(jsonLimpio);

                    return ResponseData;

                }
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion ConsultSubproductosPaquetes Catch", "Fail", ex.Message, null);
            }
            return null;
        }



        public async Task<ResponseConsultPaquetes> ConsultPaquetes(RequestConsultPaquetes request)
        {
            try
            {


                var y = JsonConvert.SerializeObject(request);

                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion ConsultPaquetes Request", "OK", y, null);


                string controller = Utilities.GetConfiguration("ConsultarPaquetes");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = await GetData(request, controller, basseAddress);

                if (response != null)
                {

                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion ConsultPaquetes Response", "OK", x, null);

                    string jsonLimpio = System.Text.RegularExpressions.Regex.Unescape(x).Trim('"');

                    jsonLimpio = jsonLimpio.Replace(@"\", "");

                    var ResponseData = JsonConvert.DeserializeObject<ResponseConsultPaquetes>(jsonLimpio);

                    return ResponseData;

                }
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion ConsultPaquetes Catch", "Fail", ex.Message, null);
            }
            return null;
        }

        public async Task<ResponseGuardarPaquetes> GuardarPaquetes(RequestGuardarPaquete request)
        {
            try
            {


                var y = JsonConvert.SerializeObject(request);


                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion GuardarPaquetes Request", "OK", y, null);


                string controller = Utilities.GetConfiguration("GuardarPaquetes");

                basseAddress = Utilities.GetConfiguration("basseAddressIntegration");

                var response = await GetData(request, controller, basseAddress);

                if (response != null)
                {

                    var x = JsonConvert.SerializeObject(response.ResponseData);

                    AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion GuardarPaquetes Response", "OK", x, null);

                    string jsonLimpio = System.Text.RegularExpressions.Regex.Unescape(x).Trim('"');

                    jsonLimpio = jsonLimpio.Replace(@"\", "");

                    var ResponseData = JsonConvert.DeserializeObject<ResponseGuardarPaquetes>(jsonLimpio);

                    return ResponseData;

                }

                }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("ApiIntegration", "entrando a la ejecucion GuardarPaquetes Catch", "Fail", ex.Message, null);
            }
            return null;
        }



        #endregion
    }
}
