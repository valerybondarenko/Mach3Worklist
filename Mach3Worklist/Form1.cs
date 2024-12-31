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
            listFileName = "";
            this.Text = "Mach3 worklist "+listFileName;
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
            newList();
        }
        private void newList()
        {
            if (listView1.Items.Count > 0)
            {
                if (savePromt())
                {
                    // сохранить содержимое ListView1 в файл.
                    // если файл уже был сохранен - перезаписываем файл
                    // свойство this.FileName=="";
                    // иначе сохраняем под новым именем
                    saveList();
                }
                listView1.Items.Clear();
                listFileName = "";
                this.Text = "Mach3 worklist " + listFileName;
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
            saveList();
        }
        private void saveList()
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
            if (listFileName != "" && listFileName !=null)
            {
                using (StreamWriter sw = new StreamWriter(listFileName))
                {
                    foreach (ListViewItem item in listView1.Items)
                    {
                        sw.WriteLine("{0}{1}{2}{3}{4}{5}{6}", item.SubItems[0].Text, "\r\n", item.SubItems[1].Text, "\r\n", item.SubItems[2].Text, "\r\n", item.SubItems[3].Text);
                    }
                }
                this.Text = "Mach3 worklist " + listFileName;
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Title = "SaveFileDialog Export2File";
            sfd.Filter = "Mach3 work list (.m3l) | *.m3l";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                listFileName = sfd.FileName.ToString();
            }
            saveList();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newList();
            dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = "Mach3 work list (.m3l) | *.m3l";
            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                string line = null;
                listFileName = dlgOpenFile.FileName;
                this.Text = "Mach3 worklist " + listFileName;
                StreamReader sr = new StreamReader(dlgOpenFile.FileName);
                line = sr.ReadLine();
                while (line != null) 
                {
                    ListViewItem lvItem = new ListViewItem(line);
                    line = sr.ReadLine();
                    lvItem.SubItems.Add(line);
                    line = sr.ReadLine();
                    lvItem.SubItems.Add(line);
                    line = sr.ReadLine();
                    lvItem.SubItems.Add(line);
                    this.listView1.Items.Add(lvItem);
                    line = sr.ReadLine();
                }
                sr.Close();
            }
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int indexSI = this.listView1.Items.IndexOf(this.listView1.SelectedItems[0]);
            if(indexSI > 0)
            {
                ListViewItem lvItem = this.listView1.SelectedItems[0];
                listView1.Items.RemoveAt(indexSI);
                listView1.Items.Insert(indexSI-1, lvItem);
            }
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int indexSI = this.listView1.Items.IndexOf(this.listView1.SelectedItems[0]);
            if (indexSI < this.listView1.Items.Count-1)
            {
                ListViewItem lvItem = this.listView1.SelectedItems[0];
                listView1.Items.RemoveAt(indexSI);
                listView1.Items.Insert(indexSI+1, lvItem);
            }
        }
    }
}
