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

namespace StoreManager
{
    public partial class TaskThread : Component
    {
        // This class is a restructured version of the CopyThread class that creates threads to complete
        // anonymous delegate tasks.
        private Dictionary<object, AsyncOperation> userStateDictionary =
            new Dictionary<object, AsyncOperation>();
        private SendOrPostCallback onCompletedDelegate;

        // Assign your Event Handler callback to this member before starting the thread
        public delegate void TaskCompletedEventHandler(object sender, TaskCompletedEventArgs e);

        public delegate bool TaskDelegate(Data param);

        // Assign a delegate of the function that you wish to complete to this member
        // before starting the tread.
        public TaskDelegate Task;

        public event EventHandler<TaskCompletedEventArgs> TaskCompleted;

        public TaskThread()
        {
            //InitializeComponent();
            InitializeDelegates();
        }

        public TaskThread(IContainer container)
        {
            container.Add(this);

            //InitializeComponent();
            InitializeDelegates();
        }

        private void InitializeDelegates()
        {
            Task = delegate(Data tempData)
            {
                return false;
            };

            onCompletedDelegate = TaskCompletion;
        }

        // Call this member to begin processing the assigned function
        public void Start(Data input, object taskId)
        {
            AsyncOperation asyncOp = AsyncOperationManager.CreateOperation(taskId);

            lock (userStateDictionary)
            {
                if (userStateDictionary.ContainsKey(taskId))
                    throw new ArgumentException("taskId must be unique", "taskId");

                userStateDictionary[taskId] = asyncOp;
            }

            Action<Data, AsyncOperation> taskDelegate = DoTask;
            taskDelegate.BeginInvoke(input, asyncOp, null, null);
        }

        public void TaskCompletion(object operationState)
        {
            var e = operationState as TaskCompletedEventArgs;

            OnTaskCompleted(e);
        }

        public void OnTaskCompleted(TaskCompletedEventArgs e)
        {
            if (TaskCompleted != null)
            {
                TaskCompleted(this, e);
            }
        }

        private void DoTask(Data input, AsyncOperation asyncOp)
        {
            Exception e = null;
            bool output = false;
            try
            {
                output = Task(input);
            }
            catch (Exception ex)
            {
                e = ex;
                MessageBox.Show(e.ToString());
            }

            this.CompletionMethod(output, e, false, asyncOp);
        }

        private void CompletionMethod(bool output, Exception ex,
            bool cancelled, AsyncOperation asyncOp)
        {
            lock (userStateDictionary)
            {
                userStateDictionary.Remove(asyncOp.UserSuppliedState);
            }

            asyncOp.PostOperationCompleted(onCompletedDelegate,
                new TaskCompletedEventArgs(output, ex, cancelled,
                    asyncOp.UserSuppliedState));
        }
    };
}
