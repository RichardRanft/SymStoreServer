﻿using System;
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
    public class DefaultInterval
    {
        public int Time = /*1000 * */60 * 1; // five minutes
    }

    public class CStoreManager
    {
        private FileSystemWatcher m_watcher;
        private String m_watchPath;
        private String m_storePath;
        private bool m_initialized;
        private Queue<StoreJob> m_jobQueue;
        private List<StoreJob> m_completedList;
        private List<Tuple<String, String>> m_pathMap;
        private List<String> m_registeredManagers;
        private SmtpClient m_mailClient;
        private String m_mailRecipient;
        private String m_mailSender;
        private NoOpSynchronizeInvoke m_syncObj;
        private Random rng;
        private CLog m_log;
        private int m_depth;
        private CJobConsumer m_consumer;

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
        public String WatchPath
        {
            get
            {
                return m_watchPath;
            }
            set
            {
                m_watchPath = value;
                m_watcher.Path = m_watchPath;
            }
        }

        public String StorePath
        {
            get
            {
                return m_storePath;
            }
            set
            {
                m_storePath = value;
            }
        }

        public String MailRecipient
        {
            get
            {
                return m_mailRecipient;
            }
            set
            {
                m_mailRecipient = value;
            }
        }

        public String MailSender
        {
            get
            {
                return m_mailSender;
            }
            set
            {
                m_mailSender = value;
            }
        }

        public CJobConsumer Consumer
        {
            get
            {
                return m_consumer;
            }
            set
            {
                m_consumer = value;
            }
        }

        public bool Initialized
        {
            get
            {
                return m_initialized;
            }
        }

        public CStoreManager()
        {
            initializeComponents();

            m_initialized = false;
        }

        public CStoreManager(String watchPath, String storePath)
        {
            m_watchPath = watchPath;
            m_storePath = storePath;

            m_initialized = initializeComponents();
        }

        public CStoreManager(String watchPath, String storePath, Queue<StoreJob> syncedQueue)
        {
            m_watchPath = watchPath;
            m_storePath = storePath;

            m_initialized = initializeComponents();

            m_jobQueue = syncedQueue;
        }

        private void readRegTxt()
        {
            StreamReader reader = new StreamReader(m_storePath + "\\reg.txt");
            String id = reader.ReadLine();
            while(id != null && !id.Equals(""))
            {
                m_registeredManagers.Add(id);
                id = reader.ReadLine();
            }
            reader.Close();
        }

        public bool CheckID(String id)
        {
            foreach(String manager in m_registeredManagers)
            {
                if (manager.Equals(id))
                    return true;
            }
            return false;
        }

        private bool initializeComponents()
        {
            m_log = new CLog();
            m_log.Filename = "StoreLog.log";

            m_syncObj = new NoOpSynchronizeInvoke();

            rng = new Random();

            m_pathMap = new List<Tuple<String, String>>();

#if DEBUG
            m_mailRecipient = "rranft@ballytech.com";
#else
            m_mailRecipient = "ENG-SCM@ballytech.com";
#endif
            m_mailSender = "SRES_SymbolStore@ballytech.com";
            m_mailClient = new SmtpClient("mail.ballytech.net", 25);

            m_jobQueue = new Queue<StoreJob>();

            m_completedList = new List<StoreJob>();

            m_registeredManagers = new List<String>();

            try
            {
                m_watcher = new FileSystemWatcher(@m_watchPath);
            }
            catch(Exception ex)
            {
                String msg = " -- Path to watch is invalid : " + ex.Message;
                m_log.WriteLine(msg);
                Console.WriteLine(msg);
                Application.Exit();
            }
            m_watcher.EnableRaisingEvents = true;
            m_watcher.IncludeSubdirectories = true;
            m_watcher.NotifyFilter = System.IO.NotifyFilters.DirectoryName;
            m_watcher.SynchronizingObject = m_syncObj;
            m_watcher.Changed += new System.IO.FileSystemEventHandler(this.m_watcher_Created);
            m_watcher.Created += new System.IO.FileSystemEventHandler(this.m_watcher_Created);

            String[] temp = m_watchPath.Split('\\');
            m_depth = temp.Length;

            readRegTxt();

            return true;
        }

        public void Run()
        {
            if (!m_initialized)
                Application.Exit();

            start();
        }

        public void Run(String watchPath, String storePath)
        {
            // Need watch and store path.
            m_watchPath = watchPath;
            m_storePath = storePath;

            m_watcher.Path = m_watchPath;

            if (!m_initialized)
                Application.Exit();
            if (!Directory.Exists(storePath))
            {
                String msg = " -- Path to symbol store is invalid : The directory name "+ storePath + " is invalid.";
                m_log.WriteLine(msg);
                Console.WriteLine(msg);
                Application.Exit();
            }

            start();
        }

        private void start()
        {
            // just go until force exit or crash
            // fortunately, the file system watcher events fire even in this stupid busy wait.
            while(true)
            {
            }
        }

        private int getDepth(String path)
        {
            String[] temp = path.Split('\\');
            return temp.Length;
        }

        private void m_watcher_Created(object sender, FileSystemEventArgs e)
        {
            bool newjob = true;
            if (m_consumer != null)
                m_consumer.refreshWait();
            ICollection col = (ICollection)m_jobQueue;
            lock (col.SyncRoot)
            {
                foreach (StoreJob j in m_jobQueue)
                {
                    if (e.FullPath == j.WatchPath)
                        newjob = false;
                }
                if (newjob)
                {
                    if (getDepth(e.FullPath) > m_depth + 1)
                        return;
                    StoreJob job = null;
                    if (m_mailRecipient != "")
                        job = new StoreJob(this, m_mailRecipient);
                    else
                        job = new StoreJob(this);
                    job.Log = this.m_log;
                    job.WatchPath = e.FullPath;
                    job.StorePath = m_storePath;
                    job.MailClient = m_mailClient;
                    job.MailSender = m_mailSender;
                    job.FileEvent = e;
                    job.Command = "add";
                    m_jobQueue.Enqueue(job);
                    String msg = " -- " + DateTime.Now.ToString() + " : Creating add Job " + job.ID.ToString() + ":" + e.Name;
                    m_log.WriteLine(msg);
                    Console.WriteLine(msg);
                }
            }
        }

        /// <summary>
        /// Handle POST requests from the HTTP service.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public String HandleRequest(String request)
        {
            // called from MyHttpServer::handlePOSTRequest().
            // probably from MyHttpServer::handleGETRequest() as well - have to use this
            // to get list of transactions and send them back down the wire.
            // request should be = separated: command=param1,param2.
            String response = "";
            String[] command = request.Split('=');
            if (command.Length != 2)
            {
                String msg = " -- " + DateTime.Now.ToString() + " : Received malformed command " + request;
                m_log.WriteLine(msg);
                Console.WriteLine(msg);
            }
            switch (command[0])
            {
                case "del":
                    response = parseLogForResponse(removeSymbolStore(command[1]));
                    break;
                case "count":
                    response = parseLogForResponse(getJobCount(command[1]));
                    break;
            }

            return response;
        }

        private String removeSymbolStore(String param)
        {
            // param should be comma-separated parameters
            String msg = "";
            StoreJob job = null;
            if (m_mailRecipient != "")
                job = new StoreJob(this, m_mailRecipient);
            else
                job = new StoreJob(this);
            String transactID = getTransactionID(param);
            if(transactID.Equals(""))
            {
                msg = " -- " + DateTime.Now.ToString() + " : No matching transaction id for " + param;
                m_log.WriteLine(msg);
                Console.WriteLine(msg);
                return msg;
            }
            job.Log = this.m_log;
            job.StorePath = m_storePath;
            job.Parameters = transactID + "," + param;
            job.MailClient = m_mailClient;
            job.MailSender = m_mailSender;
            job.Command = "del";
            msg = " -- " + DateTime.Now.ToString() + " : Creating del Job " + job.ID.ToString() + ":" + param;
            m_log.WriteLine(msg);
            Console.WriteLine(msg);
            return m_consumer.ExecuteImmediate(job);
        }

        private String getJobCount(String param)
        {
            return m_jobQueue.Count.ToString();
        }

        private String getTransactionID(String version)
        {
            String id = "";
            String filePath = m_storePath + @"\000Admin\history.txt";
            List<String> history = new List<String>();
            String line = "";
            try
            {
                using (StreamReader historyFile = new StreamReader(filePath))
                {
                    while (!historyFile.EndOfStream)
                    {
                        line = historyFile.ReadLine();
                        if (!line.Equals(""))
                            history.Add(line);
                    }
                }
            }
            catch(Exception ex)
            {
                String msg = " -- " + DateTime.Now.ToString() + " : Error reading symbol store history file : " + ex.Message;
                m_log.WriteLine(msg);
                Console.WriteLine(msg);
                return "";
            }

            foreach(String entry in history)
            {
                if(entry.Contains(version))
                {
                    String[] parts = entry.Split(',');
                    id = parts[0];
                    break;
                }
            }

            return id;
        }

        private String parseLogForResponse(String command)
        {
            String httpResponse = "";

            httpResponse = command;

            return httpResponse;
        }
    }
}
