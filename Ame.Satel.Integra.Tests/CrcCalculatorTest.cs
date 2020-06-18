using Xunit;
using Ame.Satel.Integra.Api;
using Ame.Satel.Integra.Api.Connection;

namespace Ame.Satel.Integra.Tests
{
    public class CrcCalculatorTest
    {
        [Fact]
        public void CalculateCrcForSingleCommand()
        {
            var data = new byte[] { 0x09 };
            var crc = CheckSumCalculator.Calculate(data);
            Assert.Equal(0xD7, crc.High);
            Assert.Equal(0xEB, crc.Low);
        }

        [Fact]
        public void CalculateCrcForLongCommand()
        {
            var data = new byte[] { 0x1C, 0xD7, 0x09 };
            var crc = CheckSumCalculator.Calculate(data);
            Assert.Equal(0x5E, crc.High);
            Assert.Equal(0x13, crc.Low);

        }
    }
}
