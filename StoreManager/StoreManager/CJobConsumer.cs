﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace StoreManager
{
    public class CJobConsumer
    {
        private Queue<StoreJob> m_taskQueue;
        private Queue<StoreJob> m_immediateQueue;
        private int m_interval = 1000 * 60 * 5;
        private bool m_wait;
        private Random rng;
        private Timer m_timer;
        private System.Threading.TimerCallback m_timerCallback;
        private CLog m_log;
        private bool m_processing;
        private String m_immediateProcLog;

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

        public int Interval
        {
            get
            {
                return m_interval;
            }
            set
            {
                m_interval = value;
            }
        }

        public CJobConsumer()
        {
            m_taskQueue = new Queue<StoreJob>();
            m_immediateQueue = new Queue<StoreJob>();
            m_immediateProcLog = "";
            m_timerCallback = m_timer_Tick;
            m_timer = new Timer(m_timer_Tick);
            m_wait = true;
            m_processing = false;
        }

        public CJobConsumer(Queue<StoreJob> sharedQueue)
        {
            m_taskQueue = sharedQueue;
            m_immediateQueue = new Queue<StoreJob>();
            m_immediateProcLog = "";
            rng = new Random();
            m_timerCallback = m_timer_Tick;
            m_timer = new Timer(m_timer_Tick);
            m_wait = true;
            m_processing = false;
        }

        public void Run()
        {
            while(true)
            {
                if (!m_processing && m_immediateQueue.Count > 0)
                {
                    StoreJob job = m_immediateQueue.Dequeue();
                    m_processing = true;
                    job.startJob();
                    if (job.WatchPath != null && !job.WatchPath.Equals(""))
                        cleanFolder(job.WatchPath);
                    m_immediateProcLog = job.ProcessLog;
                    m_processing = false;
                    continue;
                }
                if(m_taskQueue.Count > 0)
                {
                    if (!m_wait && !m_processing)
                    {
                        StoreJob job;
                        ICollection col = (ICollection)m_taskQueue;
                        lock (col.SyncRoot)
                        {
                            job = m_taskQueue.Dequeue();
                        }
                        if (job != null)
                        {
                            m_processing = true;
                            job.startJob();
                        }
                        if(job.WatchPath != null && !job.WatchPath.Equals(""))
                            cleanFolder(job.WatchPath);

                        m_processing = false;
                        wait();
                    }
                }
            }
        }

        public void Wait(int time)
        {
            m_timer.Change(time, (time < 250 ? 250 : time));
            m_wait = true;
        }

        public String ExecuteImmediate(StoreJob job)
        {
            m_immediateQueue.Enqueue(job);
            return m_immediateProcLog;
        }

        private void wait()
        {
            m_timer.Change(m_interval, 250);
            m_wait = true;
        }

        private void m_timer_Tick(object stateInfo)
        {
            m_wait = false;
        }

        public void refreshWait()
        {
            wait();
        }

        private void cleanFolder(String path)
        {
            String msg = " -- " + DateTime.Now.ToString() + " : Removing " + path;
            m_log.WriteLine(msg);
            Console.WriteLine(msg);
            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception ex)
            {
                msg = " -- " + DateTime.Now.ToString() + " : Warning : " + ex.Message;
                m_log.WriteLine(msg);
                Console.WriteLine(msg);
            }
            msg = " -- " + DateTime.Now.ToString() + " : Removed " + path;
            m_log.WriteLine(msg);
            Console.WriteLine(msg);
        }
    }
}
