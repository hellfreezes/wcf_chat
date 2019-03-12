using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace ChatHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string version = "v0.01";

            using (var host = new ServiceHost(typeof(wcf_chat.ServiceChat)))
            {
                host.Open();
                Console.WriteLine("Версия: " + version);
                Console.WriteLine("Хост стартовал");
                Console.ReadLine();
            }
        }
    }
}
