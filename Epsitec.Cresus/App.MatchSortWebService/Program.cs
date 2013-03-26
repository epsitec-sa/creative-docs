using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.MatchSortWebService
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Mat[CH]Sort Web Service Provider");
            Console.WriteLine("initialize...");
            var uri = "http://localhost:9000/";
            var ws = new MatchSortWebService(uri);
            Console.WriteLine("Starting WS listening on {0}",uri);
            ws.StartWebService ();
            Console.WriteLine("running, press enter to abort");
            Console.ReadLine ();
            ws.StopWebService ();
        }
    }
}
