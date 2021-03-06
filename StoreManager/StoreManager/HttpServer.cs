﻿//The MIT License (MIT)

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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace StoreManager
{
    public abstract class HttpServer
    {
        protected int m_port;
        protected IPAddress m_address;
        TcpListener listener;
        bool is_active = true;
        protected String m_httpRoot;
        protected CLog m_log;
        protected CStoreManager m_manager;

        public CStoreManager Manager
        {
            get
            {
                return m_manager;
            }
            set
            {
                m_manager = value;
            }
        }

        public CLog Log
        {
            get
            {
                return m_log;
            }
            set
            {
                m_log = value;
            }
        }

        public String HTTPRoot
        {
            get
            {
                return m_httpRoot;
            }
            set
            {
                m_httpRoot = value;
            }
        }

        public String Address
        {
            get
            {
                return m_address.ToString();
            }
        }

        public int Port
        {
            get
            {
                return m_port;
            }
        }

        public HttpServer(int port)
        {
            m_log = new CLog();
            m_log.Filename = "server.log";
            this.m_port = port;
            if (m_address == null)
            {
                byte[] ipParts = { 127, 0, 0, 1 };
                m_address = new IPAddress(ipParts);
            }
        }

        public HttpServer(byte[] address, int port)
        {
            m_log = new CLog();
            m_log.Filename = "server.log";
            this.m_address = new IPAddress(address);
            this.m_port = port;
        }

        public void listen()
        {
            if (m_address == null)
            {
                byte[] ipParts = { 127, 0, 0, 1 };
                m_address = new IPAddress(ipParts);
            }
            listener = new TcpListener(m_address, m_port);
            try
            {
                listener.Start();
            }
            catch(Exception ex)
            {
                String msg = DateTime.Now.ToString() + " : Exception in HttpServer::listen() : " + ex.Message;
                Console.WriteLine(msg);
                m_log.WriteLine(msg);
                return;
            }
            while (is_active)
            {
                TcpClient s = listener.AcceptTcpClient();
                HttpProcessor processor = new HttpProcessor(s, this);
                processor.Log = this.m_log;
                Thread thread = new Thread(new ThreadStart(processor.process));
                thread.Start();
                Thread.Sleep(1);
            }
        }

        public abstract void handleGETRequest(HttpProcessor p);
        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
    }
}