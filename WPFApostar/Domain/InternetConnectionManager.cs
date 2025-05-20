using System.Net.NetworkInformation;

namespace WPFApostar.Domain
{
    public static class InternetConnectionManager
    {
        private static CancellationTokenSource _cts = new CancellationTokenSource();
        public static async Task<bool> IsConnected()
        {
            return await Task.Run(() =>
            {
                try
                {
                    string host = "8.8.8.8";

                    Ping p = new Ping();

                    PingReply reply = p.Send(host, 3000);

                    if (reply.Status == IPStatus.Success)
                    {
                        return true;
                    }
                }
                catch { }

                return false;
            });
        }


        public static void StopVerifyConnection()
        {
            _cts.Cancel();
        }



    }
}
