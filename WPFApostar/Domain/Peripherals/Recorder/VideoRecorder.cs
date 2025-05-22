using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Text;
using WPFApostar.Domain.UIServices.Integration;

namespace WPFApostar.Domain.Peripherals.Recorder;


public class VideoRecorder : IDisposable
{
    private const string StartEndpoint = "api/Camera/start";
    private const string StopEndpoint = "api/Camera/stop";
    private readonly HttpClient _client;
    private readonly string _baseAddress;
    private bool _isRecording;
    private readonly Transaction _transaction;
    private bool _lastOperationFailed = false;
    private static SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public VideoRecorder(Transaction transaction)
    {
        _transaction = transaction;
        try
        {
            _baseAddress = ConfigurationManager.AppSettings["CameraApi:BaseAddress"] ?? "http://localhost:5001";
            _client = new HttpClient { BaseAddress = new Uri(_baseAddress) };
            _client.Timeout = TimeSpan.FromSeconds(5);
            _isRecording = false;
        }
        catch
        {

            _client = new HttpClient();
        }
    }

    public async Task<bool> StartAsync(int source = 0)
    {

        if (_isRecording || _lastOperationFailed)
        {
            return _isRecording;
        }


        await _semaphore.WaitAsync();

        try
        {

            if (_isRecording)
            {
                return true;
            }

            var filename = $"{_transaction.TipoTransaccion}_{_transaction.Documento}_{DateTime.Now:ddMMyyyy}";
            var requestData = new { filename, source };

            string jsonData = JsonConvert.SerializeObject(requestData);
            using var content = new StringContent(jsonData, Encoding.UTF8, "application/json");


            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));

            using var response = await _client.PostAsync(StartEndpoint, content, cts.Token);

            if (response.IsSuccessStatusCode)
            {
                _isRecording = true;
                _lastOperationFailed = false;
                return true;
            }


            if ((int)response.StatusCode == 500)
            {
                _lastOperationFailed = true;
            }

            return false;
        }
        catch (Exception ex)
        {

            System.Diagnostics.Debug.WriteLine($"Error en StartAsync: {ex.Message}");
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> StopAsync(int source = 0)
    {

        if (!_isRecording)
        {
            return true;
        }


        await _semaphore.WaitAsync();

        try
        {

            if (!_isRecording)
            {
                return true;
            }

            var requestData = new { source };


            bool success = false;
            for (int attempt = 0; attempt < 5; attempt++)
            {
                try
                {

                    string jsonData = JsonConvert.SerializeObject(requestData);
                    using var content = new StringContent(jsonData, Encoding.UTF8, "application/json");


                    using var cts = new CancellationTokenSource();
                    cts.CancelAfter(TimeSpan.FromSeconds(10));

                    System.Diagnostics.Debug.WriteLine($"Intento {attempt + 1} de detener grabación...");
                    using var response = await _client.PostAsync(StopEndpoint, content, cts.Token);

                    System.Diagnostics.Debug.WriteLine($"Respuesta del servidor: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine("Detención exitosa");
                        success = true;
                        break;
                    }


                    string responseContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Contenido de respuesta: {responseContent}");


                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine($"Error en intento {attempt + 1} de StopAsync: {ex.Message}");


                    if (ex is HttpRequestException)
                    {
                        await Task.Delay(1500);
                    }
                    else
                    {
                        await Task.Delay(1000);
                    }
                }
            }


            if (!success)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("Intentando detener con endpoint alternativo...");
                    string jsonData = JsonConvert.SerializeObject(requestData);
                    using var content = new StringContent(jsonData, Encoding.UTF8, "application/json");


                    string alternativeEndpoint = StopEndpoint;
                    if (StopEndpoint.EndsWith("/stop"))
                    {
                        alternativeEndpoint = StopEndpoint.Replace("/stop", "/forcestop");
                    }

                    using var cts = new CancellationTokenSource();
                    cts.CancelAfter(TimeSpan.FromSeconds(10));

                    using var response = await _client.PostAsync(alternativeEndpoint, content, cts.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        System.Diagnostics.Debug.WriteLine("Detención forzada exitosa");
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error en intento final de StopAsync: {ex.Message}");
                }
            }


            _isRecording = false;

            return success;
        }
        catch (Exception ex)
        {

            System.Diagnostics.Debug.WriteLine($"Error en StopAsync: {ex.Message}");


            _isRecording = false;
            return false;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public bool IsRecording => _isRecording;

    public bool IsCameraAvailable => !_lastOperationFailed;

    public void Dispose()
    {

        if (_isRecording)
        {
            try
            {

                StopAsync().Wait(3000);
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine($"Error en Dispose: {ex.Message}");
                _isRecording = false;
            }
        }

        _client?.Dispose();
    }
}