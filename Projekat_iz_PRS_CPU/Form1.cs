using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Speech.Synthesis;
using System.Management.Instrumentation;
using System.Management;


namespace Projekat_iz_PRS_CPU
{
    public partial class Form1 : Form
    {
        static System.Threading.Thread message;
        static System.Threading.Thread cpu;
        static System.Threading.Thread ram;
        static System.Threading.Thread graph;
        static System.Threading.Thread program;

        int cval;
        int rval;
        String mesg = "";



        static System.Diagnostics.Process process = new System.Diagnostics.Process();
        static System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        static SpeechSynthesizer synth = new SpeechSynthesizer();
        static PerformanceCounter cpuCount = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
        static PerformanceCounter memCount = new PerformanceCounter("Memory", "% Committed Bytes in Use");
        static Stopwatch stopWatch = new Stopwatch();
        static Stopwatch stopWatch1 = new Stopwatch();
        static TimeSpan ts;
        static TimeSpan ts1;

        private double[] cpuArray = new double[60];
        private double[] ramArray = new double[60];


       

        public Form1()
        {
            
            InitializeComponent();

            cpuChart.Series["CPU"].Points.AddY(0);
            cpuChart.Series["RAM"].Points.AddY(0);


            circularProgressBar1.Text = "Calculating...";
            circularProgressBar2.Text = "Calculating...";



            
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }

            if (coreCount == 1)
            {
                monitor_message();
                Thread.Sleep(15400);
                monitor_ram();

                Thread.Sleep(3000);
                monitor_cpu();

                Thread.Sleep(3500);
                getPerformanceCounters();


            }
            else if (coreCount == 2)
            {
                cpu = new System.Threading.Thread(start1);
                cpu.Start();

            }
            else if (coreCount == 3)
            {


            }

            else

            {

                program = new System.Threading.Thread(start);

                program.Start();
            }


        }






        private void start()
        {
            cpu = new System.Threading.Thread(monitor_cpu);

            ram = new System.Threading.Thread(monitor_ram);

            message = new System.Threading.Thread(monitor_message);

            graph = new System.Threading.Thread(getPerformanceCounters);


            message.Start();
            Thread.Sleep(15400);
            ram.Start();

            Thread.Sleep(3000);
            cpu.Start();

            Thread.Sleep(3500);
            graph.Start();

        }


        private void start1()
        {
            monitor_message();
            Thread.Sleep(15400);
            monitor_ram();
            Thread.Sleep(3000);
            monitor_cpu();
            Thread.Sleep(3500);
            getPerformanceCounters();
        }




            public void monitor_message()
            {
            mesg = "Hey CPU Monitor here!!!!!";
            mesg_text.Invoke(new MethodInvoker(setmesg));
            aSpeak(mesg);

            mesg = "CPU Monitor is monitoring your CPU and RAM usage now!!!!!";
            mesg_text.Invoke(new MethodInvoker(setmesg));
            aSpeak(mesg);

            mesg = "In case of extremely high usage appropriate messages will be sent and necessary measures will be taken!!!!!";
            mesg_text.Invoke(new MethodInvoker(setmesg));
            aSpeak(mesg);

            

        }

        public void monitor_ram()
        {
            rval = (int)memCount.NextValue();
            circularProgressBar2.Invoke(new MethodInvoker(setram));

            String ramMesg = String.Format("You are currently using {0}% of your RAM", rval);
            aSpeak(ramMesg);

            int[] rflag = new int[10];
            int j = 0;
            int jx = 0;

            stopWatch1.Start();

            while (true)
            {
               


                rval = (int)memCount.NextValue();
                circularProgressBar2.Invoke(new MethodInvoker(setram));


                if (rval > 90)
                {

                    rflag[j++] = 1;
                    if (j != 0)
                    {

                        if (rflag[j - 1] == 0)
                        {

                            stopWatch1.Stop();
                            ts1 = stopWatch1.Elapsed;
                            stopWatch1.Reset();
                            stopWatch1.Start();
                        }
                    }

                    else if (jx != 0)
                    {
                        if (rflag[9] == 0)
                        {
                            
                            stopWatch1.Stop();
                            ts1 = stopWatch.Elapsed;
                            stopWatch1.Reset();
                            stopWatch1.Start();
                        }
                    }
                    if (j == 10)
                        j = 0;
                }
                else
                {
                   
                    rflag[j++] = 0;
                    stopWatch1.Stop();
                    ts1 = stopWatch1.Elapsed;
                    stopWatch1.Reset();
                    stopWatch1.Start();
                    if (j == 10)
                        j = 0;
                }
                if (rflag.Sum() == 10)
                {
                    ts1 = stopWatch1.Elapsed;
                   
                    if (ts1.Seconds > 10)
                    {


                        mesg = "ALERT!! Your RAM Usage is extremely High";
                        mesg_text.Invoke(new MethodInvoker(setmesg));
                        aSpeak(mesg);

                        mesg = "System will start shutdown process in 10 seconds";
                        mesg_text.Invoke(new MethodInvoker(setmesg));
                        aSpeak(mesg);

                        mesg = "Please save your work and Brace for Impact";
                        mesg_text.Invoke(new MethodInvoker(setmesg));
                        aSpeak(mesg);

                       


                        aSpeak("10");
                        //Thread.Sleep(300);
                        aSpeak("9");
                        //Thread.Sleep(300);
                        aSpeak("8");
                        //Thread.Sleep(300);
                        aSpeak("7");
                        //Thread.Sleep(300);
                        aSpeak("6");
                        //Thread.Sleep(300);
                        aSpeak("5");
                        //Thread.Sleep(300);
                        aSpeak("4");
                        //Thread.Sleep(300);
                        aSpeak("3");
                        //Thread.Sleep(300);
                        aSpeak("2");
                        //Thread.Sleep(300);
                        aSpeak("1");
                        //Thread.Sleep(300);

                        Process me = Process.GetCurrentProcess();
                        foreach (Process p in Process.GetProcesses())
                        {
                            if (p.Id != me.Id && IntPtr.Zero != p.MainWindowHandle && false == p.ProcessName.Equals("explorer", StringComparison.CurrentCultureIgnoreCase))
                            {
                                // p.CloseMainWindow();
                            }
                        }

                        stopWatch1.Reset();
                        stopWatch1.Start();
                    }
                }
                jx++;
                Thread.Sleep(100);
            }

        }

        public void monitor_cpu()
        {
            
            cval = (int)cpuCount.NextValue();
            circularProgressBar1.Invoke(new MethodInvoker(setcpu));



            String cpuMesg = String.Format("Current CPU Usage is {0}%", cval);

            aSpeak(cpuMesg);

            int[] cflag = new int[10];
            int i = 0;
            int ix = 0;

            stopWatch.Start();


            while (true)
            {
                cval = (int)cpuCount.NextValue();
                circularProgressBar1.Invoke(new MethodInvoker(setcpu));
                


                if (cval > 10)
                {

                    cflag[i++] = 1;
                    if (i != 0)
                    {

                        if (cflag[i - 1] == 0)
                        {
                           // mesg = "previous cflag 0";
                            

                            stopWatch.Stop();
                            ts = stopWatch.Elapsed;
                            stopWatch.Reset();
                            stopWatch.Start();
                        }
                    }

                    else if (ix != 0)
                    {
                        if (cflag[9] == 0)
                        {
                           //mesg = "cflag 9 is 0";
                           
                            stopWatch.Stop();
                            ts = stopWatch.Elapsed;
                            stopWatch.Reset();
                            stopWatch.Start();
                        }
                    }
                    if (i == 10)
                        i = 0;
                }
                else
                {
                   // mesg = "cval less than 10";
                  
                    cflag[i++] = 0;
                    stopWatch.Stop();
                    ts = stopWatch.Elapsed;
                    stopWatch.Reset();
                    stopWatch.Start();
                    if (i == 10)
                        i = 0;
                }
                if (cflag.Sum() == 10)
                {
                    ts = stopWatch.Elapsed;
                  //  mesg = "cval sum = 10, ts = "+ts.Seconds.ToString();
                
                    if (ts.Seconds > 10)
                    {
                        mesg = "ALERT!! Your CPU Usage is extremely High";
                        mesg_text.Invoke(new MethodInvoker(setmesg));
                        aSpeak(mesg);

                        mesg = "System will start shutdown process in 10 seconds";
                        mesg_text.Invoke(new MethodInvoker(setmesg));
                        aSpeak(mesg);

                        mesg = "Please save your work and Brace for Impact";
                        mesg_text.Invoke(new MethodInvoker(setmesg));
                        aSpeak(mesg);

                        


                        aSpeak("10");
                        //Thread.Sleep(300);
                        aSpeak("9");
                        //Thread.Sleep(300);
                        aSpeak("8");
                        //Thread.Sleep(300);
                        aSpeak("7");
                        //Thread.Sleep(300);
                        aSpeak("6");
                        //Thread.Sleep(300);
                        aSpeak("5");
                        //Thread.Sleep(300);
                        aSpeak("4");
                        //Thread.Sleep(300);
                        aSpeak("3");
                        //Thread.Sleep(300);
                        aSpeak("2");
                        //Thread.Sleep(300);
                        aSpeak("1");
                        //Thread.Sleep(300);


                        /* Process cmd = new Process();
                            cmd.StartInfo.FileName = "cmd.exe";
                            cmd.StartInfo.RedirectStandardInput = true;
                            cmd.StartInfo.RedirectStandardOutput = true;
                            cmd.StartInfo.CreateNoWindow = true;
                            cmd.StartInfo.UseShellExecute = false;
                            cmd.Start();

                           // cmd.StandardInput.WriteLine("shutdown /l");
                            cmd.StandardInput.WriteLine("Taskkill / opera.exe");

                            cmd.StandardInput.Flush();
                            cmd.StandardInput.Close();
                            cmd.WaitForExit();
                            Console.WriteLine(cmd.StandardOutput.ReadToEnd());
                            */

                        Process me = Process.GetCurrentProcess();
                        foreach (Process p in Process.GetProcesses())
                        {
                            if (p.Id != me.Id && IntPtr.Zero != p.MainWindowHandle && false == p.ProcessName.Equals("explorer", StringComparison.CurrentCultureIgnoreCase))
                            {
                               // p.CloseMainWindow();
                            }
                        }


                        stopWatch.Reset();
                        stopWatch.Start();
                    }
                }
                ix++;
                Thread.Sleep(100);
            }

        }





        private void getPerformanceCounters()
        {

            while (true)
            {
                cpuArray[cpuArray.Length - 1] = Math.Round(cpuCount.NextValue(), 0);
                ramArray[ramArray.Length - 1] = Math.Round(memCount.NextValue(), 0);

                Array.Copy(cpuArray, 1, cpuArray, 0, cpuArray.Length - 1);
                Array.Copy(ramArray, 1, ramArray, 0, ramArray.Length - 1);

                if (cpuChart.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { UpdateCpuChart(); });
                }
                else
                {
                  
                }

                Thread.Sleep(650);
            }
        }

        private void UpdateCpuChart()
        {
            cpuChart.Series["CPU"].Points.Clear();
            cpuChart.Series["RAM"].Points.Clear();

            for (int i = 0; i < cpuArray.Length - 1; ++i)
            {
                cpuChart.Series["CPU"].Points.AddY(cpuArray[i]);
                cpuChart.Series["RAM"].Points.AddY(ramArray[i]);
            }
            
        }












        public void setcpu()
        {

            circularProgressBar1.Text = string.Format("{0:0.00}%", cval);
            circularProgressBar1.Value = (int)cval;
        }

        public void setram()
        {

            circularProgressBar2.Text = string.Format("{0:0.00}%", rval);
            circularProgressBar2.Value = (int)rval;
            
        }

        public void setmesg()
        {
         
            mesg_text.Text = mesg;
        }

        public static void aSpeak(String mesg)
        {
            synth.Speak(mesg);
        }

        int k=0;
        private void button1_Click(object sender, EventArgs e)
        {
           
            if (k == 0)
            {
                button1.Text = "Start";
                k = 1;

                stopWatch.Stop();
                stopWatch1.Stop();


            }
            else if(k == 1)
            {
                button1.Text = "Pause";
                k = 0;

                stopWatch.Start();
                stopWatch1.Start();
            }

        }




        private void button2_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
            Application.Exit();
        }
    }



  



}
