using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mighty_Music.Utils
{
    public static class NamedPipes
    {
        public static event Action<string> MessageReceived;

        public static void StartServer()
        {
            Task.Run(() =>
            {
                var server = new NamedPipeServerStream("MightyMusic");
                server.WaitForConnection();

                var reader = new StreamReader(server);
                var writer = new StreamWriter(server);
                while (true)
                {
                    var message = reader.ReadLine();
                    if (String.IsNullOrWhiteSpace(message) || !File.Exists(message))
                        break;
                    MessageReceived?.Invoke(message);
                }
            });
        }
    }
}
