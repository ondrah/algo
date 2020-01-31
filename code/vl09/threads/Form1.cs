using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Comparison of non- and threaded timers with UI feedback.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region TIMERS

        /// <summary>
        /// Threaded timer. The logic runs in a separate thread.
        /// </summary>
        System.Timers.Timer tim = new System.Timers.Timer(1000) { AutoReset = true };

        /// <summary>
        /// Unthreaded timer. The logic runs in the main thread.
        /// </summary>
        System.Windows.Forms.Timer tim2 = new System.Windows.Forms.Timer();

        private void Form1_Load(object sender, EventArgs e)
        {
            tim.Elapsed += Tim_Elapsed;
            tim.Start();

            tim2.Interval = 1000;
            tim2.Tick += Tim2_Tick;
            tim2.Start();
        }

        int nr_tim1_invocations;
        int nr_tim2_invocations;

        /// <summary>
        /// Print current time and the number of invocations. Note that this code runs in the main thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tim2_Tick(object sender, EventArgs e)
        {
            nr_tim2_invocations++;
            label2.Text = DateTime.Now.ToString("hh:mm:ss") + " (" + nr_tim2_invocations.ToString() + ")";
        }

        /// <summary>
        /// Print current time and the number of invocations. Note that this code runs in a separate thread.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tim_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            nr_tim1_invocations++;
            label1.Invoke((MethodInvoker)(() => label1.Text = DateTime.Now.ToString("hh:mm:ss") + " (" + nr_tim1_invocations.ToString() + ")"));
        }
        #endregion

        /// <summary>
        /// Lenghty algorithm running in background with UI progress feedback.
        /// </summary>
        #region BACKGROUND_ALGORITHM
        int progress;

        delegate void updateFn();

        private void PerformLongComputation(updateFn u)
        {
            progress = 0;

            for(int i = 0; i < 10; i ++)
            {
                progress += 10;
                u?.Invoke();
                Thread.Sleep(1000);
            }
        }

        private void StandardUpdateProgress()
        {
            progressBar1.Value = progress;
        }


        /// <summary>
        /// "Blocking" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            PerformLongComputation(StandardUpdateProgress);
        }


        private void PerformLongComputationThread()
        {
            PerformLongComputation(() => { progressBar1.Invoke((MethodInvoker)(() => { progressBar1.Value = progress; }) ); });
        }

        /// <summary>
        /// "direct ui update" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(PerformLongComputationThread);
            t.IsBackground = true;
            t.Start();
        }


        System.Windows.Forms.Timer refreshTim = new System.Windows.Forms.Timer();

        /// <summary>
        /// "indirect ui update" button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(() => PerformLongComputation(null));
            t.IsBackground = true;
            t.Start();

            refreshTim.Interval = 200;
            refreshTim.Tick += RefreshTim_Tick;
            refreshTim.Start();
        }

        private void RefreshTim_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = progress;
            if (progress >= 100) {
                refreshTim.Stop();
            }
        }
        #endregion

        /// <summary>
        /// A simple deadlock situation demonstration.
        /// </summary>
        #region DEADLOCK
        object knife = new object();
        object fork = new object();

        int nrA, nrB;

        private void Message(string m)
        {
            // Explain why threadName is assigned here
            string threadName = Thread.CurrentThread.Name;
            textBox1.Invoke((MethodInvoker)(() => { textBox1.AppendText(threadName + ": " + m + "\r\n"); }));
        }

        private void PersonA()
        {
            for (; ; )
            {
                Thread.Sleep(500);
                Message("locking knife");
                lock (knife)
                {
                    Message("knife locked");

                    Thread.Sleep(500);
                    Message("locking fork");
                    lock (fork)
                    {
                        Message("fork locked");
                        Thread.Sleep(500);
                    }
                    Message("fork released");
                }
                Message("knife released");
            }
        }

        private void PersonB()
        {
            for (; ; )
            {
                Thread.Sleep(500);
                Message("locking fork");
                lock (fork)
                {
                    Message("fork locked");

                    Thread.Sleep(500);
                    Message("locking knife");
                    lock (knife)
                    {
                        Message("knife locked");
                        Thread.Sleep(500);
                    }
                    Message("knife released");
                }
                Message("fork released");
            }
        }

        List<Thread> lt = new List<Thread>();

        /// <summary>
        /// "add a" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            Thread tpa = new Thread(PersonA) { IsBackground = true };
            tpa.Name = "A" + (nrA++).ToString();
            lt.Add(tpa);
            tpa.Start();
        }

        /// <summary>
        /// "add b" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            Thread tpb = new Thread(PersonB) { IsBackground = true };
            tpb.Name = "B" + (nrB++).ToString();
            lt.Add(tpb);
            tpb.Start();
        }

        /// <summary>
        /// "reset" button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            foreach(var t in lt)
            {
                t.Abort();
            }
            Message("All threads aborted.");

            nrA = nrB = 0;
        }
        #endregion

        
        #region PRODUCER_CONSUMER_SYNCHRONIZATION
        /// <summary>
        /// Shared timestamp.
        /// </summary>
        string timestamp = "";

        /// <summary>
        /// Lengthy helper creation method for appending text to the new timestamp string.
        /// </summary>
        /// <param name="str"></param>
        private void AppendToTimestamp(string str)
        {
            timestamp += str;
            Thread.Sleep(100);
        }

        /// <summary>
        /// Create a new timestamp using DateTime.Now.
        /// </summary>
        private void UpdateTimestamp()
        {
            DateTime n = DateTime.Now;

            timestamp = "";
            AppendToTimestamp(n.ToString("yyyy"));
            AppendToTimestamp("-");
            AppendToTimestamp(n.ToString("MM"));
            AppendToTimestamp("-");
            AppendToTimestamp(n.ToString("dd"));
            AppendToTimestamp(" ");
            AppendToTimestamp(n.ToString("hh"));
            AppendToTimestamp(":");
            AppendToTimestamp(n.ToString("mm"));
            AppendToTimestamp(":");
            AppendToTimestamp(n.ToString("ss"));
        }

        Thread tProducerU, tConsumerU;
        volatile bool runningProducerU, runningConsumerU;

        /// <summary>
        /// "unsynced start" button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (tProducerU != null || tConsumerU != null)
                return;

            runningProducerU = runningConsumerU = true;

            tProducerU = new Thread(() =>
            {
                while(runningProducerU)
                {
                    UpdateTimestamp();

                    label5.Invoke((MethodInvoker)(() => label5.Text = timestamp));

                    Thread.Sleep(1000);
                }
            })
            { IsBackground = true };

            tProducerU.Start();

            tConsumerU = new Thread(() =>
            {
                while(runningConsumerU)
                {
                    label6.Invoke((MethodInvoker)(() => label6.Text = timestamp));
                    Thread.Sleep(1500);
                }
            })
            { IsBackground = true };

            tConsumerU.Start();
        }

        /// <summary>
        /// "unsynced stop" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            if (tProducerU != null)
            {
                runningProducerU = false;
                // Explain here, why it's no good idea to call tProducerU.Join() here.
                tProducerU = null;
            }

            if(tConsumerU != null)
            {
                runningConsumerU = false;
                tConsumerU = null;
            }
        }

        Thread tProducerS, tConsumerS;

        volatile bool runningProducerS, runningConsumerS;

        object tLock = new object();

        /// <summary>
        /// "synced start" button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            if (tProducerS != null || tConsumerS != null)
                return;

            runningProducerS = runningConsumerS = true;

            tProducerS = new Thread(() =>
            {
                while (runningProducerS)
                {
                    lock(tLock)
                    {
                        UpdateTimestamp();

                        label5.Invoke((MethodInvoker)(() => label5.Text = timestamp));
                    }

                    Thread.Sleep(1000);
                }
            })
            { IsBackground = true };

            tProducerS.Start();

            tConsumerS = new Thread(() =>
            {
                while (runningConsumerS)
                {
                    lock(tLock)
                    {
                        label6.Invoke((MethodInvoker)(() => label6.Text = timestamp));
                    }

                    Thread.Sleep(1500);
                }
            })
            { IsBackground = true };

            tConsumerS.Start();
        }

        /// <summary>
        /// "synced stop" button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            if (tProducerS != null)
            {
                runningProducerS = false;
                tProducerS = null;
            }

            if (tConsumerS != null)
            {
                runningConsumerS = false;
                tConsumerS = null;
            }
        }
        #endregion

        /// <summary>
        /// Elementary methods for advanced synchronization methods.
        /// </summary>
        #region WAIT_AND_PULSE
        List<Thread> pulseThreads = new List<Thread>();

        object lockObj = new object();

        private void Message2(string msg)
        {
            string threadName = Thread.CurrentThread.Name;
            textBox2.Invoke((MethodInvoker)(() => textBox2.AppendText(threadName + ": " + msg + "\r\n")));
        }

        volatile bool flPulseThreadsRunning;

        private void PulseThread()
        {
            while(flPulseThreadsRunning)
            {
                Thread.Sleep(1000);

                lock (lockObj)
                {
                    Message2("Waiting now!");
                    Monitor.Wait(lockObj);
                    Message2("Woke up and running!");
                }
            }
        }

        /// <summary>
        /// "go" button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            flPulseThreadsRunning = true;

            for (int i = 0; i < 6; i++)
            {
                Thread nt = new Thread(PulseThread) { IsBackground = true };
                pulseThreads.Add(nt);
                nt.Name = "T" + i.ToString();
                nt.Start();
            }
        }

        /// <summary>
        /// "Stop" button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            flPulseThreadsRunning = false;
        }

        /// <summary>
        /// "Pulse" button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            textBox2.AppendText("Sending Pulse\r\n");
            lock (lockObj)
            {
                Monitor.Pulse(lockObj);
            }
        }

        /// <summary>
        /// "Pulse All" button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            textBox2.AppendText("Sending PulseAll\r\n");
            lock (lockObj)
            {
                Monitor.PulseAll(lockObj);
            }
        }
        #endregion
    }
}
