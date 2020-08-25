using System;
using System.Threading.Tasks;
using NetMQ.Sockets;
using NetMQ;
using System.Text;

// independent server process
// creates a data stream on launch (pub/sub)
// also responds to a simple echo request (synchronous request/response)

// https://netmq.readthedocs.io/en/latest/poller/

namespace server
{
    class Program
    {
        static string respConnectionString = "@tcp://localhost:5556";

        static async Task Main(string[] args)
        {
            Console.WriteLine("ZeroMQ server");

            // setup response socket for request/response operations        
            var resp = new ResponseSocket(respConnectionString);
            resp.ReceiveReady += ResponseHandler;
            
            while(true)
            {
                //Task.Delay(1000).Wait();
            }
        }

        // Invoked whenever atleast one message is ready for the response socket
        static void ResponseHandler(object s, NetMQSocketEventArgs e)
        {
            while (e.Socket.TryReceiveFrameBytes(out var frame))
            {
                // Interpret frame data
                var rx = Encoding.UTF8.GetString(frame, 0, frame.Length);
                Console.WriteLine("got: " + rx);
                 
                 // Send an echo
                e.Socket.SendFrame(frame);
                Console.WriteLine("sent: " + rx);
            }
        }

    }
}
