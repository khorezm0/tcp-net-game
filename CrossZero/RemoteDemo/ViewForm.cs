using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetBase;

namespace RemoteDemo
{
    public partial class ViewForm : Form
    {
        public Peer server;

        public ViewForm()
        {
            InitializeComponent();
        }

        private void ViewForm_Load(object sender, EventArgs e)
        {
            server.onReceiveMessage += (m) =>
            {
                MessageBox.Show(m);
            };
            server.onReceiveBitmap += (b) =>
            {
                pictureBox1.Image = b;
                pictureBox1.Invalidate();
            };
        }
    }
}
