using Ame.Satel.Integra.Api;
using Ame.Satel.Integra.Api.System;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Ame.Satel.Integra.Tests
{
    public class ConnectTest
    {
        [Fact]
        public async Task OpenConnection()
        {
            var test = (int)0xFF;
            var shist = test << 6;



            //https://www.elektroda.pl/rtvforum/topic3425258.html?sid=b29cd269201671389e05f07dffe217e2
            using var con = new SatelConnection("192.168.8.6");
            var response = await con.SendAsync(0x8c, 0xFF, 0xFF, 0xFF);

            while(true)
            {
                response = await con.SendAsync(0x8c, response[9], response[10], response[11]);
                var cmd = (int)response[5];
            }
            


            var respons1 = con.Read();
            var respons2 = con.Read();
            var respons3 = con.Read();
            var respons4 = con.Read();
            var respons5 = con.Read();
            var respons6 = con.Read();
            var respons7 = con.Read();
            var respons8 = con.Read();
            var respons9 = con.Read();
            var respons10 = con.Read();
            var respons11 = con.Read();
            var respons12 = con.Read();
        }
    }
}
