using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mach3Worklist
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgOpenFile = new OpenFileDialog();
            
            if(dlgOpenFile.ShowDialog() == DialogResult.OK )
            {
                ListViewItem lvItem = new ListViewItem(dlgOpenFile.FileName);
                lvItem.SubItems.Add("0");
                lvItem.SubItems.Add("1");
                lvItem.SubItems.Add("1");
                this.listView1.Items.Add(lvItem);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
