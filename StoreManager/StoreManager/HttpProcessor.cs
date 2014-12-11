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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace StoreManager
{
    public class HttpProcessor
    {
        public TcpClient socket;
        public HttpServer srv;

        private Stream inputStream;
        public StreamWriter outputStream;

        public String http_method;
        public String http_url;
        public String http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();

        protected CLog m_log;

        private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

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

        public HttpProcessor(TcpClient s, HttpServer srv)
        {
            this.socket = s;
            this.srv = srv;
        }

        private string streamReadLine(Stream inputStream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n')
                {
                    break;
                }
                if (next_char == '\r')
                {
                    continue;
                }
                if (next_char == -1)
                {
                    Thread.Sleep(1);
                    continue;
                };
                data += Convert.ToChar(next_char);
            }
            return data;
        }

        public void process()
        {
            // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            // "processed" view of the world, and we want the data raw after the headers
            inputStream = new BufferedStream(socket.GetStream());

            // we probably shouldn't be using a streamwriter for all output from handlers either
            outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
            try
            {
                parseRequest();
                readHeaders();
                if (http_method.Equals("GET"))
                {
                    handleGETRequest();
                }
                else if (http_method.Equals("POST"))
                {
                    handlePOSTRequest();
                }
            }
            catch (Exception e)
            {
                String msg = DateTime.Now.ToString() + " : HttpProcessor::process() - Exception: " + e.Message;
                Console.WriteLine(msg);
                m_log.WriteLine(msg);
                writeFailure();
            }
            outputStream.Flush();
            // bs.Flush(); // flush any remaining output
            inputStream = null; outputStream = null; // bs = null;            
            socket.Close();
        }

        public void parseRequest()
        {
            String request = streamReadLine(inputStream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            http_method = tokens[0].ToUpper();
            http_url = tokens[1];
            http_protocol_versionstring = tokens[2];

            String msg = DateTime.Now.ToString() + " : starting: " + request;
            Console.WriteLine(msg);
            m_log.WriteLine(msg);
        }

        public void readHeaders()
        {
            String msg = DateTime.Now.ToString() + " : readHeaders()";
            Console.WriteLine(msg);
            m_log.WriteLine(msg);
            String line;
            bool foundBoundary = false;
            String boundary = "";
            while ((line = streamReadLine(inputStream)) != null)
            {
                if (line.Length > 0 && line.ToString().Contains(boundary.ToString()) )
                {
                    foundBoundary = true;
                }

                if (line.Equals("") && foundBoundary)
                {
                    msg = DateTime.Now.ToString() + " : got headers";
                    Console.WriteLine(msg);
                    m_log.WriteLine(msg);
                    return;
                }

                if (line.Equals(""))
                    continue;

                if (line.Contains("boundary"))
                {
                    String[] parts = line.Split('=');
                    boundary = parts[parts.Length - 1].Replace("-", "");
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                msg = DateTime.Now.ToString() + " : header: " + name + ":" + value;
                Console.WriteLine(msg);
                m_log.WriteLine(msg);
                httpHeaders[name] = value;
            }
        }

        public void handleGETRequest()
        {
            srv.handleGETRequest(this);
        }

        private const int BUF_SIZE = 4096;
        public void handlePOSTRequest()
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            String msg = DateTime.Now.ToString() + " : get post data start";
            Console.WriteLine(msg);
            m_log.WriteLine(msg);
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    throw new Exception(
                        String.Format("POST Content-Length({0}) too big for this simple server",
                          content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0)
                {
                    msg = DateTime.Now.ToString() + " : starting Read, to_read=" + to_read;
                    Console.WriteLine(msg);
                    m_log.WriteLine(msg);

                    int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    msg = DateTime.Now.ToString() + " : read finished, numread=" + numread;
                    Console.WriteLine(msg);
                    m_log.WriteLine(msg);
                    if (numread == 0)
                    {
                        if (to_read == 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("client disconnected during post");
                        }
                    }
                    to_read -= numread;
                    ms.Write(buf, 0, numread);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            msg = DateTime.Now.ToString() + " : get post data end";
            Console.WriteLine(msg);
            m_log.WriteLine(msg);
            srv.handlePOSTRequest(this, new StreamReader(ms));
        }

        public void writeSuccess(string content_type = "text/html")
        {
            outputStream.WriteLine("HTTP/1.0 200 OK");
            outputStream.WriteLine("Content-Type: " + content_type);
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }

        public void writeFailure()
        {
            outputStream.WriteLine("HTTP/1.0 404 File not found");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }
    }
}