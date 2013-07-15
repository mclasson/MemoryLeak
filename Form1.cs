using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MemoryLeak
{
    public partial class HostWindow : Form
    {
        public event EventHandler ev_Click;
        public HostWindow()
        {
            InitializeComponent();
        }

        private void AllocateObjects(object sender, EventArgs e)
        {
            ((Button)sender).Enabled = false;
            for (int a = 0; a < 10000; a++)
            {
                new Worker(this);
            }
            memoryLabel.Text = "Memory consumption:" + GC.GetTotalMemory(true)/1024;
            

        }

        private void TriggerChildObjects(object sender, EventArgs e)
        {
            countLabel.Text = "InvocationList contains " + (ev_Click == null?0:ev_Click.GetInvocationList().Length) + " objects";
        }

        private void Dispose(object sender, EventArgs e)
        {
            if (ev_Click == null)
                return;
            foreach (var w in ev_Click.GetInvocationList())
            {
                using (var x = w.Target as IDisposable)
                {
                    x.Dispose();
                }
            }
            countLabel.Text = "InvocationList contains " + (ev_Click == null ? 0 : ev_Click.GetInvocationList().Length) + " objects";
            memoryLabel.Text = "Memory consumption:" + GC.GetTotalMemory(true) / 1024;

        }

    }
    class Worker : IDisposable
    {
        public delegate void Click(object sender);
        private readonly byte[] bLoad = new byte[99999];
        HostWindow _host;
        public Worker(HostWindow host)
        {
            _host = host;
            host.ev_Click += HostEventTriggered;
        }

        void HostEventTriggered(object sender, EventArgs args)
        {
            Console.WriteLine("I'm triggered");
        }


        public void Dispose()
        {
            _host.ev_Click -= HostEventTriggered;
        }
    }
}
