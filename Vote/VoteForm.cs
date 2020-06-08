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

namespace Vote
{
    public partial class FormVote : Form
    {
        // true if yes, false if no
        readonly Global<bool> Yes;
        readonly GlobalReader<int> TotalYes;
        readonly GlobalReader<int> TotalNo;
        readonly GlobalReader<bool> ServerOnline;

        public FormVote()
        {
            InitializeComponent();

            Yes = new Global<bool>("Yes");
            TotalYes = new GlobalReader<int>("TotalYes", handler: TotalYes_DataChanged);
            TotalNo = new GlobalReader<int>("TotalNo", handler: TotalNo_DataChanged);
            ServerOnline = new GlobalReader<bool>("ServerOnline", false, ServerOnline_DataChanged);
        }

        private delegate void HandleInMainThreadDelegate2(Control c, GlobalEventData<bool> e);

        private void HandleInMainThread2(Control c, GlobalEventData<bool> e)
        {
            if (c.InvokeRequired)
            {
                c.BeginInvoke(new HandleInMainThreadDelegate2(HandleInMainThread2), new object[] { c, e });
            }
            else
            {
                Enable(e.Data);
            }
        }

        private void ServerOnline_DataChanged(object sender, GlobalEventData<bool> e)
        {
            if (e.isDefault)
            {
                return;
            }

            HandleInMainThread2(lbYes, e);
        }

        private delegate void HandleInMainThreadDelegate(Control c, GlobalEventData<int> e);

        private void HandleInMainThread(Control c, GlobalEventData<int> e)
        {
            if (c.InvokeRequired)
            {
                c.BeginInvoke(new HandleInMainThreadDelegate(HandleInMainThread), new object[] { c, e });
            }
            else
            {
                c.Text = e.Data.ToString();
            }
        }

        private void TotalNo_DataChanged(object sender, GlobalEventData<int> e)
        {
            HandleInMainThread(lbNoCnt, e);            
        }

        private void TotalYes_DataChanged(object sender, GlobalEventData<int> e)
        {
            HandleInMainThread(lbYesCnt, e);
        }

        private void Enable(bool doEnable)
        {
            btnYes.Enabled = doEnable;
            btnNo.Enabled = doEnable;
        }

        private void BtnYes_Click(object sender, EventArgs e)
        {
            Yes.Value = true;
            Enable(false);
        }

        private void BtnNo_Click(object sender, EventArgs e)
        {
            Yes.Value = false;
            Enable(false);            
        }
    }
}
