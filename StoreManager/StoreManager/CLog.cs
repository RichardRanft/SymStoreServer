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
