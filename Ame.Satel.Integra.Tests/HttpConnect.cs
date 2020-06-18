using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ame.Satel.Integra.Tests
{
    public class HttpConnect
    {
        [Fact]
        public async Task Connect()
        {
            try
            {
                var cooki = new CookieContainer();
                HttpMessageHandler handler = new HttpClientHandler { CookieContainer = cooki};
                var client = new HttpClient(handler);
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                client.DefaultRequestHeaders.Add("User-Agent", "AME");

                var res = await client.PostAsync("http://192.168.8.2/api/user/login", new StringContent("username=admin&password=Prudnik1!", Encoding.UTF8, "application/xml"));

                var homePage = await client.GetStringAsync("http://192.168.8.2/html/home.html");

                //var resLogOut = await client.PostAsync("http://192.168.8.8/boafrm/formLogoutHtm", new StringContent("", Encoding.UTF8, "application/xml"));

                client.Dispose();
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
