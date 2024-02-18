using Globals.NET.RabbitMQ;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VoteFormServer
{
    public partial class VoteFormServer : Form
    {
        readonly GlobalReader<bool> Yes;
        readonly Global<int> TotalYes;
        readonly Global<int> TotalNo;
        readonly Global<bool> ServerOnline;
        private int _totalYes = 0;
        private int _totalNo = 0;

        public VoteFormServer()
        {
            InitializeComponent();

            Yes = new GlobalReader<bool>("Yes", handler: Yes_DataChanged);
            TotalYes = new Global<int>("TotalYes") { Value = 0 };
            TotalNo = new Global<int>("TotalNo") { Value = 0 };

            ServerOnline = new Global<bool>("ServerOnline")
            {
                Value = true
            };
        }

        private delegate void HandleInMainThreadDelegate(Control c, int i);

        // Some UI trouble
        private void HandleInMainThread(Control c, int i)
        {
            if (c.InvokeRequired)
            {
                c.BeginInvoke(new HandleInMainThreadDelegate(HandleInMainThread), new object[] { c, i });
            }
            else
            {
                c.Text = i.ToString();
            }
        }

        private void Yes_DataChanged(object sender, GlobalEventData<bool> e)
        {
            if (e.isInitialValue)
            {
                return;
            }

            if (e.Data)
            {
                TotalYes.Value = ++_totalYes;
                HandleInMainThread(lbYesCnt, _totalYes);
            }
            else
            {
                TotalNo.Value = ++_totalNo;
                HandleInMainThread(lbNoCnt, _totalNo);
            }
        }

        private void VoteFormServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            ServerOnline.Value = false;

            Yes.Dispose();
            TotalYes.Dispose();
            TotalNo.Dispose();
            ServerOnline.Dispose();
        }
    }
}
