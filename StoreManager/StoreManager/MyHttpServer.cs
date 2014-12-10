using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace StoreManager
{
    public class MyHttpServer : HttpServer
    {
        public MyHttpServer(int port)
            : base(port)
        {
        }

        public MyHttpServer(byte[] address, int port)
            : base(address, port)
        {
        }

        public override void handleGETRequest(HttpProcessor p)
        {
            String url = cleanURL(p.http_url.ToLower());

            String msg = DateTime.Now.ToString() + " : GET request: " + url;
            Console.WriteLine(msg);
            m_log.WriteLine(msg);

            if (url.Contains(".png"))
            {

                Stream fs = null;
                try
                {
                    fs = File.Open(HTTPRoot + url, FileMode.Open);
                }
                catch(Exception ex)
                {
                    msg = DateTime.Now.ToString() + " : request failed: " + url + " : " + ex.Message;
                    Console.WriteLine(msg);
                    m_log.WriteLine(msg);
                    p.writeFailure();
                    return;
                }

                p.writeSuccess("image/png");
                fs.CopyTo(p.outputStream.BaseStream);
                p.outputStream.BaseStream.Flush();
                fs.Close();
            }

            if (url.Contains(".jpg"))
            {
                Stream fs = null;
                try
                {
                    fs = File.Open(HTTPRoot + url, FileMode.Open);
                }
                catch (Exception ex)
                {
                    msg = DateTime.Now.ToString() + " : request failed: " + url + " : " + ex.Message;
                    Console.WriteLine(msg);
                    m_log.WriteLine(msg);
                    p.writeFailure();
                    return;
                }

                p.writeSuccess("image/jpg");
                fs.CopyTo(p.outputStream.BaseStream);
                p.outputStream.BaseStream.Flush();
                fs.Close();
            }

            if (url.Contains(".pdb") || url.Contains(".exe") || url.Contains(".dll"))
            {
                Stream fs = null;
                try
                {
                    fs = File.Open(HTTPRoot + url, FileMode.Open);
                }
                catch (Exception ex)
                {
                    msg = DateTime.Now.ToString() + " : request failed: " + url + " : " + ex.Message;
                    Console.WriteLine(msg);
                    m_log.WriteLine(msg);
                    p.writeFailure();
                    return;
                }

                p.writeSuccess("application/octet-stream");
                fs.CopyTo(p.outputStream.BaseStream);
                p.outputStream.BaseStream.Flush();
                fs.Close();
                return;
            }

            String page = getPage(url);
            if (page != "")
            {
                p.writeSuccess();
                p.outputStream.Write(page);
            }
            else
            {
                // send test page
                msg = DateTime.Now.ToString() + " : request failed: " + url + " : Page not found.";
                Console.WriteLine(msg);
                m_log.WriteLine(msg);
                p.writeFailure();
            }
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            String msg = DateTime.Now.ToString() + " : POST request: " + p.http_url;
            Console.WriteLine(msg);
            m_log.WriteLine(msg);

            // handle a few specific POST types
            if (p.httpHeaders["Content-Type"].ToString() == "multipart/form-data")
            {
                String line1 = inputData.ReadLine();
                line1 = line1.Replace("-", "");
                String terminalMarker = "\n-----------------------" + line1 + "--";
                String line2 = inputData.ReadLine();
                String line3 = inputData.ReadLine();
                String line4 = inputData.ReadLine();
                string data = inputData.ReadToEnd();
                data = data.Replace(terminalMarker, "");

                String[] parts = line2.Split(';');
                String[] filenameAttrib = parts[parts.Length - 1].Split('=');
                String filename = filenameAttrib[1].Replace("\"", "");
                using (StreamWriter writer = new StreamWriter("." + p.http_url + "/" + filename))
                {
                    writer.Write(data);
                }

                p.writeSuccess();
                p.outputStream.WriteLine("<html><body><h1>test server</h1>");
                p.outputStream.WriteLine("<a href=/test>return</a><p>");
                p.outputStream.WriteLine("postbody: <pre>{0}</pre>", filename);
            }
            else if (p.httpHeaders["Content-Type"].ToString() == "application/x-www-form-urlencoded")
            {
                string data = inputData.ReadToEnd();
                String[] attribs = data.Split('&');
                String response = "";
                foreach(String command in attribs)
                {
                    response = m_manager.HandleRequest(command);
                }

                p.writeSuccess();
                p.outputStream.WriteLine("<html><body><h1>test server</h1>");
                p.outputStream.WriteLine("postbody: <pre>");
                foreach(String part in attribs)
                    p.outputStream.WriteLine("<p>{0}</p>", response);
                p.outputStream.WriteLine("</pre>");
            }
            else
            {
                string data = inputData.ReadToEnd();

                p.writeSuccess();
                p.outputStream.WriteLine("<html><body><h1>test server</h1>");
                p.outputStream.WriteLine("<a href=/test>return</a><p>");
                p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data);
            }
        }

        private String cleanURL(String urlString)
        {
            String clean = "";
            urlString = urlString.Replace("//", "/");
            String[] parts = urlString.Split('/');
            
            if (parts.Length < 3)
                return urlString;

            if(parts[2].Contains(parts[1]))
            {
                parts[1] = "";
            }
            foreach(String part in parts)
            {
                clean += "/" + part;
            }
            clean = clean.Replace("//", "/");

            return clean;
        }

        private String getPage(String pageName)
        {
            String[] pageFilename = pageName.Split('/');
            String fileName = pageFilename[pageFilename.Length - 1];
            String folder = m_httpRoot + pageName.Remove(pageName.LastIndexOf('/'));
            if (pageName.Equals(""))
                return "";
            String[] files = Directory.GetFiles(folder);
            String page = "";
            foreach (String file in files)
            {
                if (file.Contains(fileName))
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        page = reader.ReadToEnd();
                    }
                }
            }
            return page;
        }
    }
}