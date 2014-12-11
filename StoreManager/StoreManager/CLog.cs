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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace StoreManager
{
    public class CLog
    {
        private String m_logFileName;
        private StreamWriter m_outputStream;

        public String Filename
        {
            get
            {
                return m_logFileName;
            }
            set
            {
                m_logFileName = value;
            }
        }

        public CLog ()
        {
            m_logFileName = "";
            m_outputStream = null;
        }

        public void WriteLine(String line)
        {
            if (m_outputStream == null)
            {
                try
                {
                    m_outputStream = new StreamWriter(m_logFileName);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(" -- {0} : Error opening logfile : {1}", DateTime.Now.ToString(), ex.Message);
                }
            }

            try
            {
                m_outputStream.WriteLineAsync(line);
                m_outputStream.Flush();
            }
            catch(Exception ex)
            {
                Console.WriteLine(" -- {0} : Error writing to logfile : {1}", DateTime.Now.ToString(), ex.Message);
            }
        }
    }
}
