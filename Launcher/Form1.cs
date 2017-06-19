using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;

namespace Launcher
{
    public partial class Form1 : Form
    {
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetGetConnectedState(out int lpdwFlags, int dwReserved);


        string username;
        string password;
        string exeLoc;
        DateTime expDate = new DateTime(2017, 06, 20);
        DateTime currDate;
        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            exeLoc = System.Environment.CurrentDirectory + "/clock.mp3";
            Console.WriteLine(exeLoc);

            currDate = GetNistTime();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label4.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //label4.Text = "*Error: username and password not set";
            username = textBox1.Text;
            password = textBox2.Text;
            //MessageBox.Show("Logging In");

            int desc;
            bool isConnected = InternetGetConnectedState(out desc, 0);
            if (isConnected)
            {
                if (currDate < expDate)
                {
                    //Console.WriteLine(GetNistTime());
                    //MessageBox.Show(isConnected.ToString());
                    Process.Start(exeLoc);

                    Application.Exit();
                }
                else
                {
                    label4.Text = "*Error: Trail period expired.";
                    MessageBox.Show("Incorrect Password!\nPlease Contact SysAdmin.");
                }
            }
            else
            {
                label4.Text = "*Error: Internet required to verify details";
                MessageBox.Show("Need to verify your credentials. Please Connect to net!");
            }
        }

        public static DateTime GetNistTime()
        {
            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
            var response = myHttpWebRequest.GetResponse();
            string todaysDates = response.Headers["date"];
            return DateTime.ParseExact(todaysDates,
                                       "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                       CultureInfo.InvariantCulture.DateTimeFormat,
                                       DateTimeStyles.AssumeUniversal);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string embeddedFileName = "clock.mp3";

            var currentAssembly = Assembly.GetExecutingAssembly();
            var arrResources = currentAssembly.GetManifestResourceNames();
            foreach (var resourceName in arrResources)
            {
                if (resourceName.ToUpper().EndsWith(embeddedFileName.ToUpper()))
                {
                    using (var resourceToSave = currentAssembly.GetManifestResourceStream(resourceName))
                    {
                        using (var output = File.OpenWrite(System.Environment.CurrentDirectory+"/clock.mp3"))
                            resourceToSave.CopyTo(output);
                        resourceToSave.Close();
                    }
                }
            }
            Process.Start(exeLoc);
        }
    }
}
