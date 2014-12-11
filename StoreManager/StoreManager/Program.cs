//The MIT License (MIT)

//Copyright (c) 2014 Richard Ranft

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;
using System.Security;
using System.Management;
using System.Net.Mail;

namespace StoreManager
{
    class Program
    {
        [MTAThread]
        static int Main(string[] args)
        {
            String watchPath = "";
            String storePath = "";
            String interval = "300000";
            String address = "127.0.0.1:80";
            int ms = 300000;
            if(args.Length < 3)
            {
                if (File.Exists("storemanager.ini"))
                {
                    Console.WriteLine(" -- loading storemanager.ini ... ");
                    CSettings settings = new CSettings("storemanager.ini");
                    if(settings.LoadSettings())
                    {
                        foreach (KeyValuePair<String, String> attribute in settings.GetSection("[Default]"))
                        {
                            switch(attribute.Key.ToLower())
                            {
                                case "watchpath":
                                    Console.WriteLine(" -- WATCHPATH = {0}", attribute.Value);
                                    watchPath = attribute.Value;
                                    break;
                                case "storepath":
                                    Console.WriteLine(" -- STOREPATH = {0}", attribute.Value);
                                    storePath = attribute.Value;
                                    break;
                                case "interval":
                                    Console.WriteLine(" -- INTERVAL = {0}", attribute.Value);
                                    interval = attribute.Value;
                                    break;
                                case "address":
                                    Console.WriteLine(" -- ADDRESS = {0}", attribute.Value);
                                    address = attribute.Value;
                                    break;
                            }
                        }
                    }
                }
            }
            if (watchPath == "")
                watchPath = args[0];
            if (storePath == "")
                storePath = args[1];
            if (args.Length >= 3 && args[3] != "" && address == "127.0.0.1:80")
                address = args[3];
            try
            {
                ms = Convert.ToInt32(interval);
            }
            catch(Exception ex)
            {
                Console.WriteLine("{0} : Program::Main() - Exception: {1}", DateTime.Now.ToString(), ex.Message);
                Console.WriteLine("{0} : Starting with default interval of {1}", DateTime.Now.ToString(), ms.ToString());
                ms = 300000;
            }

            if (watchPath == "" || storePath == "")
            {
                Console.WriteLine("\n[usage]:");
                Console.WriteLine("StoreManager <path to watch> <path to symbol store> <ip address:port>");
                Console.WriteLine("You can also create a file called storemanager.ini that can contain the following fields:");
                Console.WriteLine("\nWATCHPATH=<path to watch> - fully qualified path to a folder or share to watch for drops");
                Console.WriteLine("\nSTOREPATH=<path to symbol store> - fully qualified path to the symbol store");
                Console.WriteLine("\nINTERVAL=<time in milliseconds> - wait time between tasks");
                Console.WriteLine("\nADDRESS=<ip address:port> - ip address to serve symbols from");
                return abort();
            }

            HttpServer httpServer;
            String[] ipAddress = address.ToString().Split(':');
            if (ipAddress.Length != 2)
                return abort();
            String[] ipParts = ipAddress[0].ToString().Split('.');
            if (ipParts.Length != 4)
                return abort();
            byte[] ip = {0, 0, 0, 0};
            for(int i = 0; i < ipParts.Length; i++)
            {
                ip[i] = Convert.ToByte(ipParts[i]);
            }
            int port = Convert.ToInt16(ipAddress[1]);
            httpServer = new MyHttpServer(ip, port);
            httpServer.HTTPRoot = @".";
            Thread thread = new Thread(new ThreadStart(httpServer.listen));
            thread.Start();
            Console.WriteLine("HTTPServer running on address {0}:{1}", httpServer.Address, httpServer.Port);

            Queue<StoreJob> SharedQueue = new Queue<StoreJob>();
            CJobConsumer consumer = new CJobConsumer(SharedQueue);
            CStoreManager manager = new CStoreManager(watchPath, storePath, SharedQueue);
            httpServer.Manager = manager;
            consumer.Log = manager.Log;
            httpServer.Log = manager.Log;
            consumer.Interval = ms;
            manager.Consumer = consumer;
            consumer.Run();
            manager.Run();

            return 0;
        }

        public static int abort()
        {
            Console.WriteLine("Aborting!");
            return 1;
        }
    }
}
