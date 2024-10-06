using System.Net;

namespace CheckConnectInternet
{
    internal class Internet
    {
        public static bool connect() {
            try {
                Dns.GetHostEntry("dotnet.beget.tech");
                return true;
            }
            catch { 
                return false;
            }
        }
    }
}
