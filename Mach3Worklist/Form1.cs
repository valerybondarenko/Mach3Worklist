using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count > 0) 
            {
                if (savePromt())
                {
                    // сохранить содержимое ListView1 в файл.
                    // если файл уже был сохранен - перезаписываем файл
                    // свойство this.FileName=="";
                    // иначе сохраняем под новым именем
                }
                listView1.Items.Clear();
            }
        }
        private bool savePromt()
        {
            string message = "Сохранить изменения?";
            string caption = "Новый список";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            result = MessageBox.Show(message, caption, buttons);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                return true;
            }
            return false;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listFileName == "") 
            {
                SaveFileDialog sfd = new SaveFileDialog();

                sfd.Title = "SaveFileDialog Export2File";
                sfd.Filter = "Mach3 work list (.m3l) | *.m3l";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    listFileName = sfd.FileName.ToString();
                }               
            }
            if (listFileName != "")
            {
                using (StreamWriter sw = new StreamWriter(listFileName))
                {
                    foreach (ListViewItem item in listView1.Items)
                    {
                        sw.WriteLine("{0}{1}{2}{3}{4}{5}{6}", item.SubItems[0].Text, " ", item.SubItems[1].Text, " ", item.SubItems[2].Text, " ", item.SubItems[3].Text);
                    }
                }
            }
        }
        private void export2File(ListView lv, string splitter)
        {
            string filename = "";
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Title = "SaveFileDialog Export2File";
            sfd.Filter = "Text File (.txt) | *.txt";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                filename = sfd.FileName.ToString();
                if (filename != "")
                {
                    using (StreamWriter sw = new StreamWriter(filename))
                    {
                        foreach (ListViewItem item in lv.Items)
                        {
                            sw.WriteLine("{0}{1}{2}", item.SubItems[0].Text, splitter, item.SubItems[1].Text);
                        }
                    }
                }
            }
        }
    }
}
