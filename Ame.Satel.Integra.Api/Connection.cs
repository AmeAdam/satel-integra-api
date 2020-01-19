using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Ame.Satel.Integra.Api
{
    public class Connection : IDisposable
    {
        private SemaphoreSlim sync = new SemaphoreSlim(1,1);
        private const byte frameSynchronizationMarker = 0xFE;
        private const byte frameCompletedMarker = 0x0D;
        private TcpClient tcpClient;
        private NetworkStream stream;
        private CancellationTokenSource cancelationToken = new CancellationTokenSource();

        public string Hostname { get; }
        public int Port { get; }

        public Connection(string hostname, int port = 7094)
        {
            Hostname = hostname;
            Port = port;
            tcpClient = new TcpClient(Hostname, Port);
            stream = tcpClient.GetStream();
        }

        public async Task<ArraySegment<byte>> SendAsync(CancellationToken externalCancellationToken, params byte[] packet)
        {
            try
            {
                using (var cancelation = CancellationTokenSource.CreateLinkedTokenSource(externalCancellationToken, cancelationToken.Token))
                {
                    await sync.WaitAsync(cancelation.Token);
                    await Write(packet, cancelation.Token);
                    return Read();
                }
            }
            finally
            {
                sync.Release();
            }
        }

        public async Task<ArraySegment<byte>> SendAsync(params byte[] packet)
        {
            return await SendAsync(CancellationToken.None, packet);
        }

        private async Task Write(byte[] packet, CancellationToken token)
        {
            stream.WriteByte(frameSynchronizationMarker);
            stream.WriteByte(frameSynchronizationMarker);
            foreach (var b in packet)
            {
                stream.WriteByte(b);
                if (b == frameSynchronizationMarker)
                    stream.WriteByte(0xF0);
            }
            var crc = CheckSumCalculator.Calculate(packet);
            stream.WriteByte(crc.High);
            stream.WriteByte(crc.Low);
            stream.WriteByte(frameSynchronizationMarker);
            stream.WriteByte(frameCompletedMarker);
            await stream.FlushAsync(token);
        }

        private void SkipBeginMarker()
        {
            while (!cancelationToken.IsCancellationRequested)
            {
                int current = stream.ReadByte();
                while (current != frameSynchronizationMarker)
                {
                    current = stream.ReadByte();
                }
                current = stream.ReadByte();
                if (current == frameSynchronizationMarker)
                {
                    return;
                }
            }
        }

        private IEnumerable<byte> ReadContent()
        {
            while (!cancelationToken.IsCancellationRequested)
            {
                int currnet = stream.ReadByte();
                if (currnet == frameSynchronizationMarker)
                {
                    currnet = stream.ReadByte();
                    if (currnet == frameCompletedMarker)
                        yield break;
                    else
                        yield return frameSynchronizationMarker;
                }
                yield return (byte)currnet;
            }
        }

        private ArraySegment<byte> Read()
        {
            SkipBeginMarker();
            var content = ReadContent().ToArray();
            var crc = CheckSumCalculator.Calculate(content, 0, content.Length - 2);
            if (crc.High != content[content.Length - 2] || crc.Low != content[content.Length - 1])
                throw new InvalidDataException("CRC verification failed");

            var result = new ArraySegment<byte>(content, 0, content.Length - 2);
            return result;

        }

        public void Dispose()
        {
            cancelationToken.Cancel();
            stream?.Dispose();
            tcpClient?.Dispose();
        }
    }
}
