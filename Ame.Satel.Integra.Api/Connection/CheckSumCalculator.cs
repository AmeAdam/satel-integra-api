using System;

namespace Ame.Satel.Integra.Api.Connection
{
    public class CheckSumCalculator
    {
        private const int initialCrc = 0x147a;
        public static (byte Low, byte High) Calculate(byte[] data)
        {
            return Calculate(data, 0, data.Length);
        }

        public static (byte Low, byte High) Calculate(byte[] data, int offset, int size)
        {
            int crc = initialCrc;

            for (var i = offset; i < offset + size; i++)
            {
                var b = data[i];
                crc = RotateLeft(crc);
                crc = crc ^ 0xFFFF;
                crc = crc + (crc >> 8) + b;
            }
            var crcAsBytes = BitConverter.GetBytes(crc);
            return (crcAsBytes[0], crcAsBytes[1]);
        }

        private static int RotateLeft(int value)
        {
            var bit = (value & 0x8000) >> 15;
            return ((value << 1) & 0xFFFF) | bit;
        }
    }
}
