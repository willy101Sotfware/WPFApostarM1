using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WPFApostar.Classes;
using WPFApostar.DataModel;
using WPFApostar.Services;
using WPFApostar.UserControls;

namespace WPFApostar.Domain.Recorder
{
    public static class VideoRecorder
    {

        const string START = "start";
        const string STOP = "stop";
        private static string _baseAddress;
        private static HttpClient _client;

        static VideoRecorder()
        {
            _baseAddress = "http://localhost:5000/";
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseAddress);
        }
        public static async Task<bool> Start(string paypad, string transaction, int source = 0)
        {



            var filename = $"{paypad}_{transaction}_{DateTime.Now.ToString("ddMMyyyy")}";

            var dataRequest = new
            {
                filename,
                source
            };

            return await RequestControlRecorder(dataRequest, START);

        }

        public static async Task<bool> Stop(int source = 0)
        {
            var dataRequest = new
            {
                source
            };

            return await RequestControlRecorder(dataRequest, STOP);
        }

        private static async Task<bool> RequestControlRecorder(object dataRequest, string endpoint)
        {
            try
            {
                string dataRequestStr = JsonConvert.SerializeObject(dataRequest);

                var content = new StringContent(dataRequestStr, Encoding.UTF8, "Application/json");

                var response = await _client.PostAsync(endpoint, content);

                var result = await response.Content.ReadAsStringAsync();
                if (result == null)
                {
                    AdminPayPlus.SaveLog("VideoRecorder " + "RequestControlRecorder " + "No se obtuvo contenido de la api de grabador");
                    return false;

                }

                var requestresponse = JsonConvert.DeserializeObject<ApiResponse<string>>(result);
                if (requestresponse == null)
                {
                    AdminPayPlus.SaveLog("VideoRecorder " + "RequestControlRecorder " + "Error deserializando la respuesta de la api de grabador");
                    return false;
                }

                if (requestresponse.statusCode == 200)
                {
                    AdminPayPlus.SaveLog("VideoRecorder " + "RequestControlRecorder " + $"Api de grabador respondió correctamente {requestresponse.message}");
                    return true;
                }

                AdminPayPlus.SaveLog("VideoRecorder " + "RequestControlRecorder " + "Api de video no respondió satisfactoriamente " + requestresponse);
                return false;
            }
            catch (Exception ex)
            {
                AdminPayPlus.SaveLog("VideoRecorder " + "RequestControlRecorder Catch " + $"Ocurrió un error en tiempo de ejecución {ex.Message} " + ex);
                return false;
            }

        }

    }
}
