using System;
using System.Text;
using NetMQ;
using NetMQ.Sockets;

namespace client
{
    class Program
    {
    static string reqConnectionString = ">tcp://localhost:5556";

        static void Main(string[] args)
        {
            Console.WriteLine("ZeroMQ client");

            // connect to the server
            Console.WriteLine("connecting to server...");
            using (var req = new RequestSocket(reqConnectionString))
            {
                Console.WriteLine("send msg: ");
                
                while (true)
                {
                    // send message
                    var tx = Console.ReadLine();
                    var txBytes = Encoding.UTF8.GetBytes(tx);
                    req.SendFrame(txBytes);
                    Console.WriteLine("sent: " + tx);

                    // wait for echo response
                    var rxBytes = req.ReceiveFrameBytes();
                    var rx = Encoding.UTF8.GetString(rxBytes);
                    Console.WriteLine("got: " + rx);
                }
            }
        }
    }
}
