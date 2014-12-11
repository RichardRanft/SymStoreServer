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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
    public class JobCompleteEventArgs : EventArgs
    {
        public bool Success = false;
    }

    public delegate void CompletedEventHandler(object sender, JobCompleteEventArgs e);

    public class StoreJob
    {
        private String m_path;
        private int m_depth;
        private int m_id;
        private Random rng;
        private String m_sourceUNC;
        private String m_storeFolder;
        private String m_storeUNC;
        private String m_command;
        private String m_cmdParams;
        private SmtpClient m_mailClient;

        private String m_recipient;
        private String m_sender;
        private CStoreManager m_parent;
        private String m_processLog;
        private String m_httpLog;

        private CLog m_log;

        private FileSystemEventArgs m_fileArgs;
        private JobCompleteEventArgs m_completeArgs;

        public event CompletedEventHandler Complete;

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
            int dwLogonType, int dwLogonProvider, out SafeTokenHandle phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        public String Command
        {
            get
            {
                return m_command;
            }
            set
            {
                m_command = value;
            }
        }

        public String Parameters
        {
            get
            {
                return m_cmdParams;
            }
            set
            {
                m_cmdParams = value;
            }
        }

        public FileSystemEventArgs FileEvent
        {
            set
            {
                m_fileArgs = value;
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

        public SmtpClient MailClient
        {
            set
            {
                m_mailClient = value;
            }
        }
        
        public int ID
        {
            get
            {
                return m_id;
            }
        }

        public String StorePath
        {
            get
            {
                return m_storeFolder;
            }
            set
            {
                m_storeFolder = value;
            }
        }
        
        public String WatchPath
        {
            get
            {
                return m_path;
            }
            set
            {
                m_path = value;
                String[] temp = m_path.Split('\\');
                m_depth = temp.Length;
            }
        }

        public String MailSender
        {
            get
            {
                return m_sender;
            }
            set
            {
                m_sender = value;
            }
        }

        public int Depth
        {
            get
            {
                return m_depth;
            }
        }

        public String ProcessLog
        {
            get
            {
                return m_processLog;
            }
        }

        public StoreJob()
        {
            m_completeArgs = new JobCompleteEventArgs();
            rng = new Random();
            m_id = rng.Next();
            m_recipient = m_parent.MailRecipient;
            m_sender = m_parent.MailSender;
        }

        public StoreJob(String recipient)
        {
            m_completeArgs = new JobCompleteEventArgs();
            rng = new Random();
            m_id = rng.Next();
            m_recipient = recipient;
            m_sender = m_parent.MailSender;
            Complete += new CompletedEventHandler(OnComplete);
        }

        public StoreJob(CStoreManager parent)
        {
            m_parent = parent;
            m_completeArgs = new JobCompleteEventArgs();
            rng = new Random();
            m_id = rng.Next();
            m_recipient = m_parent.MailRecipient;
            m_sender = m_parent.MailSender;
        }

        public StoreJob(CStoreManager parent, String recipient)
        {
            m_parent = parent;
            m_completeArgs = new JobCompleteEventArgs();
            rng = new Random();
            m_id = rng.Next();
            m_recipient = recipient;
            m_sender = m_parent.MailSender;
            Complete += new CompletedEventHandler(OnComplete);
        }

        protected virtual void OnComplete(object sender, JobCompleteEventArgs e)
        {
            if (Complete != null)
                Complete(this, e);
            String msg = " -- " + DateTime.Now.ToString() + " : Completed Job " + m_id.ToString() + ":" + m_fileArgs.Name;
            m_log.WriteLine(msg);
            Console.WriteLine(msg);
        }

        public bool startJob()
        {
            String msg = "";
            if(m_fileArgs != null)
                msg = " -- " + DateTime.Now.ToString() + " : Beginning Job " + m_id.ToString() + ":" + m_fileArgs.Name;
            else
                msg = " -- " + DateTime.Now.ToString() + " : Beginning Job " + m_id.ToString() + ":" + m_command;
            m_log.WriteLine(msg);
            Console.WriteLine(msg);

            if (!m_command.Equals("del") && m_fileArgs == null)
            {
                msg = " -- " + DateTime.Now.ToString() + " : Error - No file arguments set";
                m_log.WriteLine(msg);
                Console.WriteLine(msg);
            }
            if (m_mailClient == null)
            {
                msg = " -- " + DateTime.Now.ToString() + " : Error - No mail client set";
                m_log.WriteLine(msg);
                Console.WriteLine(msg);
            }

            bool result = true;

            if (!m_command.Equals("del"))
            {
                try
                {
                    m_sourceUNC = GetUNCPath(m_fileArgs.FullPath) + m_fileArgs.FullPath.Remove(0, 2);
                }
                catch (Exception ex)
                {
                    sendStoreErrorNotification(ex.Message);
                    msg = " -- " + DateTime.Now.ToString() + " : Error processing Job " + m_id.ToString() + ":" + m_fileArgs.Name + " : " + ex.Message;
                    m_log.WriteLine(msg);
                    Console.WriteLine(msg);
                    return false;
                }
            }

            try
            {
                m_storeUNC = GetUNCPath(m_storeFolder) + m_storeFolder.Remove(0, 2);
            }
            catch (Exception ex)
            {
                sendStoreErrorNotification(ex.Message);
                msg = " -- " + DateTime.Now.ToString() + " : Error processing Job " + m_id.ToString() + ":" + m_fileArgs.Name + " : " + ex.Message;
                m_log.WriteLine(msg);
                Console.WriteLine(msg);
                return false;
            }

            String releasePath = "";
            if (!m_command.Equals("del"))
            {
                String[] parts = m_fileArgs.FullPath.Split('\\');
                releasePath = parts[parts.Length - 1];
            }

            Data p = new Data();
            switch (m_command)
            {
                case "add":
                    p.srcURI = m_sourceUNC;
                    p.storeURI = m_storeUNC;
                    p.product = releasePath;
                    p.recurse = "true";
                    p.comment = releasePath + "-" + DateTime.Now.ToString();
                    break;
                case "del":
                    String[] paramParts = m_cmdParams.Split(',');
                    p.storeURI = m_storeUNC;
                    p.iD = paramParts[0];
                    p.recurse = "true";
                    break;
            }

            StartStore(p);
            return result;
        }

        private bool Store(Data param)
        {
            String sourcePath = "";
            String storePath = "";
            
            if (param.srcPath != null)
                sourcePath = param.srcURI;
            else if (param.srcShare != null)
                sourcePath = param.srcShare;
            else if (param.srcURI != null)
                sourcePath = param.srcURI;
            if (m_command.Equals("add") && sourcePath.Equals(""))
            {
                sendStoreErrorNotification("No valid symbols path provided for add.");
                String msg = " -- " + DateTime.Now.ToString() + " : Error processing Job " + m_id.ToString() + ":" + m_fileArgs.Name + ": No valid watch path provided for add";
                m_log.WriteLine(msg);
                Console.WriteLine(msg);
                return false;
            }

            if (param.storePath != null)
                storePath = param.storeURI;
            else if (param.storeURI != null)
                storePath = param.storeURI;
            else
            {
                sendStoreErrorNotification("No valid store path provided.");
                String msg = " -- " + DateTime.Now.ToString() + " : Error processing Job " + m_id.ToString() + ":" + m_fileArgs.Name + ": No valid store path provided";
                m_log.WriteLine(msg);
                Console.WriteLine(msg);
                return false;
            }
            
            bool ret = false;
            
            try
            {
                sourcePath = sourcePath.EndsWith(@"\") ? sourcePath : sourcePath + @"\";
                storePath = storePath.EndsWith(@"\") ? storePath : storePath + @"\";
                
                Process proc = new Process();
                switch(m_command)
                {
                    case "add":
                        if (Directory.Exists(sourcePath))
                        {
                            // check our source indexing and keep log
                            proc.StartInfo.WorkingDirectory = storePath;
                            proc.StartInfo.FileName = storePath + @"\walk.cmd";
                            proc.StartInfo.Arguments = @sourcePath + @"\\*.pdb " + storePath + @"\\srctool.exe -c ";
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.RedirectStandardOutput = true;
                            proc.Start();
                            m_httpLog = "srctool.exe log: \n" + proc.StandardOutput.ReadToEnd();

                            // store symbol files and keep log
                            proc = new Process();
                            proc.StartInfo.WorkingDirectory = storePath;
                            proc.StartInfo.FileName = storePath + @"\symstore.exe";
                            proc.StartInfo.Arguments = m_command + " " + getStoreArgs(param);
                            proc.StartInfo.UseShellExecute = false;
                            proc.StartInfo.RedirectStandardOutput = true;
                            proc.Start();
                            m_processLog = "symstore.exe log: \n" + proc.StandardOutput.ReadToEnd();
                        }
                        ret = true;
                        break;
                    case "del":
                        // remove symbol reference and keep log
                        proc = new Process();
                        proc.StartInfo.WorkingDirectory = storePath;
                        proc.StartInfo.FileName = storePath + @"\symstore.exe";
                        proc.StartInfo.Arguments = m_command + " " + getStoreArgs(param);
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.Start();
                        m_processLog = "symstore.exe log: \n" + proc.StandardOutput.ReadToEnd();

                        ret = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                sendStoreErrorNotification(ex.Message);
                String msg = "";
                if(!m_command.Equals("del"))
                    msg = " -- " + DateTime.Now.ToString() + " : Error processing Job " + m_id.ToString() + ":" + m_fileArgs.Name + ": " + ex.Message;
                else
                    msg = " -- " + DateTime.Now.ToString() + " : Error processing Job " + m_id.ToString() + ":" + m_command + ": " + ex.Message;

                m_log.WriteLine(msg);
                Console.WriteLine(msg);
                ret = false;
            }
            return ret;
        }

        private bool StartStore(Data param)
        {
            bool result = Store(param);
            if (result)
            {
                String msg = "";
                if(m_fileArgs != null)
                    msg = " -- " + DateTime.Now.ToString() + " : Completed Job " + m_id.ToString() + ":" + m_fileArgs.Name;
                else
                    msg = " -- " + DateTime.Now.ToString() + " : Completed Job " + m_id.ToString() + ":" + m_command + ": " + m_cmdParams;
                m_log.WriteLine(msg);
                Console.WriteLine(msg);
                sendNotification();
            }
            return result;
        }

        public void sendNotification()
        {
            switch(m_command)
            {
                case "add":
                    sendAddNotification();
                    break;
                case "del":
                    sendDelNotification();
                    break;
            }
        }

        private void sendAddNotification()
        {
            String changeFolder = m_fileArgs.FullPath;
            String[] parts = changeFolder.Split('\\');
            String releasePath = parts[parts.Length - 1];

            String[] gameParts = releasePath.Split(' ');
            String gameName = "";
            for (int i = 0; i < gameParts.Length - 2; i++)
            {
                gameName += gameParts[i] + " ";
            }
            gameName.Trim();

            String moduleName = "";
            if (gameParts.Length > 2)
                moduleName = gameParts[gameParts.Length - 2];
            else
                moduleName = gameParts[gameParts.Length - 1];

            String dropFolder = @m_storeUNC;

            String messageBody;
            using (StreamReader infile = new StreamReader("MsgTemplate.html"))
            {
                messageBody = infile.ReadToEnd();
            }

            messageBody = messageBody.Replace("[PATH]", releasePath);
            messageBody = messageBody.Replace("[DATETIME]", DateTime.Now.ToString());
            messageBody = messageBody.Replace("[TIMEZONE]", TimeZone.CurrentTimeZone.StandardName);
            messageBody = messageBody.Replace("[MODULENAME]", m_sourceUNC);
            messageBody = messageBody.Replace("[DROPFOLDER]", dropFolder);
            messageBody += m_processLog.Replace("\n", "<br />");
            messageBody += "<br /><br />";
            messageBody += m_httpLog.Replace("\n", "<br />");

            MailMessage message = new MailMessage(m_sender, m_recipient);
            message.Subject = "BUILD Symbols Stored : " + releasePath;
            message.Body = messageBody;

            System.Net.Mime.ContentType mimeType = new System.Net.Mime.ContentType("text/html");
            AlternateView alt = AlternateView.CreateAlternateViewFromString(messageBody, mimeType);
            message.AlternateViews.Add(alt);
            String msg;
            try
            {
                m_mailClient.Send(message);
            }
            catch (SmtpException ex)
            {
                sendErrorNotification(message, ex.Message);
                msg = " -- " + DateTime.Now.ToString() + " : Error sending notification " + m_id.ToString() + ":" + m_fileArgs.Name + " : " + ex.Message;
                m_log.WriteLine(msg);
                Console.WriteLine(msg);
            }
            msg = " -- " + DateTime.Now.ToString() + " : SrcTool log " + m_id.ToString() + ":" + Environment.NewLine + m_httpLog;
            m_log.WriteLine(msg);
            Console.WriteLine(msg);
            msg = " -- " + DateTime.Now.ToString() + " : SymStore log " + m_id.ToString() + ":" + Environment.NewLine + m_processLog;
            m_log.WriteLine(msg);
            Console.WriteLine(msg);
        }

        private void sendDelNotification()
        {
            String messageBody;
            using (StreamReader infile = new StreamReader("DelTemplate.html"))
            {
                messageBody = infile.ReadToEnd();
            }
            String[] paramParts = m_cmdParams.Split(',');
            messageBody = messageBody.Replace("[VERSION]", paramParts[1]);
            messageBody = messageBody.Replace("[DATETIME]", DateTime.Now.ToString());
            messageBody = messageBody.Replace("[TIMEZONE]", TimeZone.CurrentTimeZone.StandardName);
            messageBody = messageBody.Replace("[TRANSACTION]", paramParts[0]);
            messageBody += m_processLog.Replace("\n", "<br />");

            MailMessage message = new MailMessage(m_sender, m_recipient);
            message.Subject = "BUILD Symbols Removed : " + paramParts[1];
            message.Body = messageBody;

            System.Net.Mime.ContentType mimeType = new System.Net.Mime.ContentType("text/html");
            AlternateView alt = AlternateView.CreateAlternateViewFromString(messageBody, mimeType);
            message.AlternateViews.Add(alt);
            String msg;
            try
            {
                m_mailClient.Send(message);
            }
            catch (SmtpException ex)
            {
                sendErrorNotification(message, ex.Message);
                msg = " -- " + DateTime.Now.ToString() + " : Error sending notification " + m_id.ToString() + ":" + paramParts[1] + " : " + ex.Message;
                m_log.WriteLine(msg);
                Console.WriteLine(msg);
            }
            msg = " -- " + DateTime.Now.ToString() + " : SymStore log " + m_id.ToString() + ":" + Environment.NewLine + m_processLog;
            m_log.WriteLine(msg);
            Console.WriteLine(msg);
        }

        private void sendErrorNotification(MailMessage failMail, String msg)
        {
            MailMessage message = new MailMessage(failMail.Sender.ToString(), "rranft@ballytech.com");
            message.Subject = "Delivery Alert Failure Notification";
            String msgBody = "Date : " + DateTime.Now + " " + TimeZone.CurrentTimeZone.StandardName + Environment.NewLine + Environment.NewLine;
            msgBody += "Message alert email delivery failed: " + msg + Environment.NewLine + Environment.NewLine;

            msgBody += "---------------------------------------------------------" + Environment.NewLine + Environment.NewLine;
            msgBody += failMail.Body + Environment.NewLine + Environment.NewLine;
            msgBody += "---------------------------------------------------------" + Environment.NewLine + Environment.NewLine;
            try
            {
                m_mailClient.Send(message);
            }
            catch (SmtpException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void sendStoreErrorNotification(String msg)
        {
            MailMessage message = new MailMessage(m_sender, m_recipient);
            message.Subject = "Store Failure Notification";
            String msgBody = "Date : " + DateTime.Now + " " + TimeZone.CurrentTimeZone.StandardName + Environment.NewLine + Environment.NewLine;
            msgBody += "Release Build Store Failure: " + msg + Environment.NewLine + Environment.NewLine;
            msgBody += m_processLog == null ? "" : m_processLog;

            message.Body = msgBody;
            try
            {
                m_mailClient.Send(message);
            }
            catch (SmtpException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Given a local mapped drive letter, determine if it is a network drive. If so, return the server share.
        /// </summary>
        /// <param name="mappedDrive"></param>
        /// <returns>The server path that the drive maps to ~ "////XXXXXX//ZZZZ", or the disk name if it is not a network share.</returns>
        private String GetUNCPath(String mappedDrive)
        {
            //Query to return all the local computer's drives.
            //See http://msdn.microsoft.com/en-us/library/ms186146.aspx, or search "WMI Queries"

            // DriveType:
            // 0    Unknown
            // 1    No Root Directory
            // 2    Removable Disk
            // 3    Local Disk
            // 4    Network Drive
            // 5    Compact Disk
            // 6    RAM Disk

            SelectQuery selectWMIQuery = new SelectQuery("Win32_LogicalDisk");
            ManagementObjectSearcher driveSearcher = new ManagementObjectSearcher(selectWMIQuery);

            //Soem variables to be used inside and out of the foreach.
            ManagementPath path = null;
            ManagementObject networkDrive = null;
            bool found = false;
            String serverName = null;
            String driveName = null;
            String[] driveParts = null;
            UInt32 driveType;

            //Check each disk, determine if it is a network drive, and then return the real server path.
            foreach (ManagementObject disk in driveSearcher.Get())
            {
                path = disk.Path;
                driveParts = path.RelativePath.Split('\"');
                driveName = driveParts[1];

                if (mappedDrive.ToLower().Contains(driveName.ToLower()))
                {
                    networkDrive = new ManagementObject(path);
                    driveType = Convert.ToUInt32(networkDrive["DriveType"]);

                    if (driveType == 4) // Network drive
                    {
                        serverName = Convert.ToString(networkDrive["ProviderName"]);
                        found = true;
                        break;
                    }
                    else if (driveType == 2 || driveType == 3 || driveType == 5 || driveType == 6) // Local/Removable/Compact/RAM disk
                    {
                        // This does not map a network drive, but still returns a valid path.  For Store operations, 
                        // it is not desireable to fail just because the source or target is not a network share.  This
                        // should allow us to Store to or from (only from in the case of optical drives I think) any other 
                        // valid disk on the machine.
                        // On the other hand, we're not even going to try storing to/from other two drive types.  I haven't
                        // done any research but they just sound unsafe....
                        serverName = driveName;
                        found = true;
                    }
                    else
                    {
                        throw new DirectoryNotFoundException("The drive " + mappedDrive + " was found, but is not a network drive. Were your network drives mapped correctly?");
                    }
                }
            }

            if (!found)
            {
                throw new DirectoryNotFoundException("The drive " + mappedDrive + " was not found. Were your network drives mapped correctly?");
            }

            return serverName;
        }

        private String getStoreArgs(Data dat)
        {
            String args = "";
        
            if(dat.srcURI != null)
                args += "/f " + @dat.srcURI + " ";          // /f (full network path)
            if(dat.storeURI != null)
                args += "/s " + @dat.storeURI + " ";        // /s (full network path)
            if(dat.srcPath != null)
                args += "/f " + @dat.srcPath + @"\*.* ";    // /f (local path)
            if(dat.storePath != null)
                args += "/s " + @dat.storePath + " ";       // /s (local path)
            if(dat.srcShare != null)
                args += "/g " + @dat.srcShare + " ";        // /g
            if(dat.prefix != null)
                args += "/m " + dat.prefix + " ";           // /m
            if(dat.symbolVisibility != null)
                args += "/h " + dat.symbolVisibility + " "; // /h {PUB|PRI}
            if(dat.iD != null)
                args += "/i " + dat.iD + " ";               // /i
            if(dat.usePointers != null)
                args += "/p "+ dat.usePointers + " ";       // /p
            if(dat.message != null)
                args += "-:MSG " + dat.message + " ";       // -:MSG
            if(dat.relative != null)
                args += "-:REL " + dat.relative + " ";      // -:REL (requires /p)
            if(dat.recurse != null)
                args += "/r ";                              // /r
            if(dat.product != null)
                args += "/t " + dat.product + " ";          // /t
            if(dat.version != null)
                args += "/v " + dat.version + " ";          // /v
            if(dat.comment != null)
                args += "/c \"" + dat.comment + "\" ";          // /c
            if(dat.logfile != null)
                args += "/d " + dat.logfile + " ";          // /d
            if(dat.verbose != null)
                args += "/o ";                              // /o
            if(dat.indexfile != null)
                args += "/x " + dat.indexfile + " ";        // /x
            if(dat.appendindex != null)
                args += "/a ";                              // /a
            if(dat.readIndex != null)
                args += "/y " + dat.readIndex + " ";        // /y
            if(dat.appendindexfile != null)
                args += "/yi " + dat.appendindexfile + " "; // /yi
            if(dat.onlyVisibility != null)
                args += "/z " + dat.onlyVisibility + " ";   // /z {PUB|PRI}
            if(dat.compress != null)
                args += "/compress " + dat.compress + " ";  // /compress

            return args;
        }
    }

    public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeTokenHandle()
            : base(true)
        {
        }

        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr handle);

        protected override bool ReleaseHandle()
        {
            return CloseHandle(handle);
        }
    }
}
