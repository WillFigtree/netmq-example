using System;
using System.Threading.Tasks;
using System.Text;
using NetMQ.Sockets;
using NetMQ;

namespace intra_app_comms
{
    class Program
    {
        static string echoAddress = "tcp://localhost:5556";
        static string streamAddress = "tcp://localhost:5557";

        static async Task Main(string[] args)
        {
            using (var responseSocket = new ResponseSocket("tcp://*:5555"))
            using (var requestSocket = new RequestSocket("tcp://localhost:5555"))
            {
                Console.WriteLine("requestSocket : Sending 'Hello'");
                requestSocket.SendFrame("Hello");
                var message = responseSocket.ReceiveFrameString();
                Console.WriteLine("responseSocket : Server Received '{0}'", message);
                Console.WriteLine("responseSocket Sending 'World'");
                responseSocket.SendFrame("World");
                message = requestSocket.ReceiveFrameString();
                Console.WriteLine("requestSocket : Received '{0}'", message);
                Console.ReadLine();
            }

            Console.WriteLine("ZeroMQ intra-app communications demo");
            Console.WriteLine("Use CTRL+C shortcut to cancel at any time");

            var echoServerTask = EchoServer();
            var streamTask = StreamServer();
            var streamToConsoleTask = PushStreamToConsole();
            var consoleToEchoTask = RelayConsoleToEchoServer();
            

            await Task.WhenAll(echoServerTask, streamTask, streamToConsoleTask, consoleToEchoTask);
        }

        // Creates a server which sends received data back to the sender
        static async Task EchoServer()
        {
            // TODO
            Console.WriteLine("Starting echo server");

            // setup response socket for request/response operations        
            var resp = new ResponseSocket("@" + echoAddress);
            resp.ReceiveReady += ResponseHandler;

            // loop forever
            while(true)
            {
                await Task.Delay(10000).ConfigureAwait(false);
            }

            void ResponseHandler(object s, NetMQSocketEventArgs e)
            {
                while (e.Socket.TryReceiveFrameBytes(out var frame))
                {
                    // Interpret frame data
                    var rx = Encoding.UTF8.GetString(frame, 0, frame.Length);
                    Console.WriteLine("echo server got: " + rx);
                    
                    // Send an echo
                    e.Socket.SendFrame(frame);
                    Console.WriteLine("echo server sent: " + rx);
                }
            }  
        }

        // Continuously prompts the user for input and relays it to the echo server
        static async Task RelayConsoleToEchoServer()
        {
            // TODO
            Console.WriteLine("Relaying console prompts to echo server");
            using var req = new RequestSocket(">" + echoAddress);

            // continuously prompt for user input
            while(true)
            {
                // Read the console without blocking
                var userInput = await Task.Run(() => Console.ReadLine());

                var txBytes = Encoding.UTF8.GetBytes(userInput);
                req.SendFrame(txBytes);
                Console.WriteLine("console sent: " + userInput);

                // wait for echo response
                var rxBytes = req.ReceiveFrameBytes();
                var rx = Encoding.UTF8.GetString(rxBytes);
                Console.WriteLine("console got: " + rx);

                await Task.Delay(200);
            }
            
        }

        // Streams data continuously
        static async Task StreamServer()
        {
            // TODO

            Console.WriteLine("Starting stream server");
        }

        // Subscribes to the stream server and pushes the data to the console
        static async Task PushStreamToConsole()
        {
            // TODO
            Console.WriteLine("Pushing stream to console");
        }
    }
}
