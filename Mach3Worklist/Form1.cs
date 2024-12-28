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

        private void openGCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // активирует процесс обработки таблицы.
            // меняет текст кнопки на "Стоп"
            // все елементы управления списком становятся недоступными
            // в Mach3 загружается программа G-code из активной строки
            // после выполнения текущей программы взависимости от режима 
            // или повторяется выполнение программы из текущей строки, или 
            // происходит переход к программе из следующей строки 
            // доступные режимы:
            //      1 - по одной из каждой строки
            //      2 - до конца каждой строки
            //      3 - выборочно по кнопке из зоны обработки
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                this.listView1.Items.Remove(this.listView1.SelectedItems[0]);
            }
        }

        private void quantityEditToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
