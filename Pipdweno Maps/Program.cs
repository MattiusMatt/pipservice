using System;
using Nancy.Hosting.Self;

namespace Pipdweno_Maps
{
    class Program
    {
        const string DOMAIN = @"http://localhost:7504";

        static void Main(string[] args)
        {
            using (var host = new NancyHost(new Uri(DOMAIN)))
            {
                host.Start();
                Console.ReadLine();
            }
        }
    }
}
