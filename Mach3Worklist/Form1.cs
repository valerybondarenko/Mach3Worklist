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
using Microsoft.VisualBasic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;


namespace Mach3Worklist
{
    public partial class Form1 : Form
    {
        private enum ExeMode
        {
            Line,
            Circle,
            Selective
        }

        private enum ExeStatus
        {
            Non,
            Ready,
            Runing,
            Stoped,
            Сompleted,
            Aborted
        }
        public Form1()
        {
            InitializeComponent();
            listFileName = "";
            this.Text = "Mach3 worklist "+listFileName;
            eMode = ExeMode.Line;
            statusChange(ExeStatus.Ready);


        }
        // РАЗДЕЛ МЕНЮ - ИЗМЕНИТЬ
        private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = "CNC G-code files (*.tap; *.nc) | *.tap; *nc| All files(*.*) | *.*";

            if (dlgOpenFile.ShowDialog() == DialogResult.OK )
            {
                ListViewItem lvItem = new ListViewItem(dlgOpenFile.FileName);
                lvItem.SubItems.Add("0");
                lvItem.SubItems.Add("1");
                lvItem.SubItems.Add("1");
                this.listView1.Items.Add(lvItem);
            }
        }

        private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count > 0)
            {
                this.listView1.Items.Remove(this.listView1.SelectedItems[0]);
            }
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int indexSI = this.listView1.Items.IndexOf(this.listView1.SelectedItems[0]);
            if (indexSI > 0)
            {
                ListViewItem lvItem = this.listView1.SelectedItems[0];
                listView1.Items.RemoveAt(indexSI);
                listView1.Items.Insert(indexSI - 1, lvItem);
            }
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int indexSI = this.listView1.Items.IndexOf(this.listView1.SelectedItems[0]);
            if (indexSI < this.listView1.Items.Count - 1)
            {
                ListViewItem lvItem = this.listView1.SelectedItems[0];
                listView1.Items.RemoveAt(indexSI);
                listView1.Items.Insert(indexSI + 1, lvItem);
            }
        }

        private void openGCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        

        private void quantityEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter your name", "Input Box", "");
            MessageBox.Show("Hello, " + input);
        }
        // РАЗДЕЛ МЕНЮ - ФАЙЛ
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
                    saveList();
                }
                listView1.Items.Clear();
                listFileName = "";
                this.Text = "Mach3 worklist " + listFileName;
                statusChange(ExeStatus.Ready);
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
                statusChange(ExeStatus.Ready);
            }
        }

        // ОБРАБОТКА СОБЫТИЙ СПИСКА

        private void listView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                currentLineIndex = this.listView1.Items.IndexOf(listView1.SelectedItems[0]);
                
            }

        }
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                currentLineIndex = this.listView1.Items.IndexOf(listView1.SelectedItems[0]);
            }
            ListViewHitTestInfo i = listView1.HitTest(e.X, e.Y);
            SelectedLSI = i.SubItem;
            int columnindex = i.Item.SubItems.IndexOf(i.SubItem);
            string columnHeader;
            switch (columnindex)
            {
                case 1:
                    columnHeader = "Количество";
                    break;
                case 2:
                    columnHeader = "Квота";
                    break;
                case 3:
                    columnHeader = "Зона";
                    break;
                default:
                    return;
            }
            
            
            string input = Microsoft.VisualBasic.Interaction.InputBox(columnHeader, "Введите значение",  SelectedLSI.Text);
            string output = string.Concat(input.Where(char.IsDigit));
            if (output.Length > 0)
            { 
            SelectedLSI.Text = output;
            }

        }
        // РАЗДЕЛ МЕНЮ - НАСТРОЙКИ
        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.circleToolStripMenuItem.Checked = true;
            this.lineToolStripMenuItem.Checked = false;
            this.selectiveToolStripMenuItem.Checked = false;
            this.lblMode.Text = "По кругу";
            this.eMode = ExeMode.Circle;
        }

        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.lineToolStripMenuItem.Checked = true;
            this.circleToolStripMenuItem.Checked = false;
            this.selectiveToolStripMenuItem.Checked = false;
            this.lblMode.Text = "Линейно";
            this.eMode = ExeMode.Line;
        }

        private void selectiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectiveToolStripMenuItem.Checked = true;
            this.lineToolStripMenuItem.Checked = false;
            this.circleToolStripMenuItem.Checked = false;
            this.lblMode.Text = "Выборочно";
            this.eMode = ExeMode.Selective;
        }

        // ОБРАБОТКА ВЫПОЛНЕНИЯ ПРОГРАММ 
        private void timer1_Tick(object sender, EventArgs e)
        {
            int quota = System.Convert.ToInt32(this.listView1.SelectedItems[0].SubItems[2].Text);
            int count = System.Convert.ToInt32(this.listView1.SelectedItems[0].SubItems[1].Text);
            this.lblStatus.Text = "Выполняется - " + this.listView1.SelectedItems[0].SubItems[0].Text;

            if (this.eMode == ExeMode.Circle)
            {
                if (count < quota)
                {
                    count++;
                    this.listView1.Items[currentLineIndex].SubItems[1].Text = count.ToString();
                    if (this.listView1.Items.Count > currentLineIndex + 1)
                    {
                        this.listView1.Items[currentLineIndex].Selected = false;
                        currentLineIndex++;
                        this.listView1.Items[currentLineIndex].Selected = true;
                    }
                    else
                    {
                        this.listView1.Items[currentLineIndex].Selected = false;
                        currentLineIndex = 0;
                        this.listView1.Items[currentLineIndex].Selected = true;
                    }
                }
                else
                {
                    statusChange(ExeStatus.Сompleted);
                    return;

                }


            }
            else if (this.eMode == ExeMode.Line)


                    if (quota > count)
                    {
                        count++;
                        this.listView1.Items[currentLineIndex].SubItems[1].Text = count.ToString();
                    }
                    else if (currentLineIndex + 1 < this.listView1.Items.Count)
                    {
                    Page(1);
                    }
                    else
                    {
                        statusChange(ExeStatus.Сompleted);
                        return;
                    }
                

                else if (this.eMode == ExeMode.Selective)
                {

                }

        }

        private void btnStart_Click_1(object sender, EventArgs e)
        {
            // активирует процесс обработки таблицы.
            //  this.timer1 = new System.Windows.Forms.Timer();
            if (this.listView1.SelectedItems.Count < 1) { return;}
            if (eStatus!=ExeStatus.Runing)
            {
                statusChange(ExeStatus.Runing);
            } else if(eStatus == ExeStatus.Runing)
            {
                statusChange(ExeStatus.Stoped);
            }
            
            

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
        private void statusChange(ExeStatus status)
        {
            if (eStatus == status) { return;}
            this.eStatus=status;
            if (this.eStatus == ExeStatus.Runing)
            {
                if (listView1.SelectedIndices.Count > 0)
                {
                    this.btnStart.Text = "Стоп";
                    this.eStatus = ExeStatus.Runing;
                    this.timer1.Enabled = true;
                    this.timer1.Interval = 2000;
                    this.timer1.Start();
                }
                    
            }
            else if (this.eStatus == ExeStatus.Stoped)
            {
                this.timer1.Stop();
                this.btnStart.Text = "Старт";
                this.lblStatus.Text = "Остановлен";
            }
            else if (this.eStatus == ExeStatus.Сompleted)
            {
                this.timer1.Stop();
                this.btnStart.Text = "Старт";
                this.lblStatus.Text = "Завершен";
            }
            else if (this.eStatus == ExeStatus.Ready)
            {
                this.btnStart.Text = "Старт";
                this.lblStatus.Text = "Готов";
                switch (this.eMode)
                {
                    case ExeMode.Line:
                        this.lblMode.Text = "Линейно";
                        break;
                    case ExeMode.Circle:
                        this.lblMode.Text = "По кругу";
                        break;
                    case ExeMode.Selective:
                        this.lblMode.Text = "Выборочно";
                        break;
                }
                
            }
        }
        private void Page(int UpOrDown)
        {
            //Determine if something is selected
            if (listView1.SelectedIndices.Count > 0)
            {
                int oldIndex = listView1.SelectedIndices[0];
                listView1.SelectedIndices.Clear();

                //Use mod!
                int numberOfItems = listView1.Items.Count;
                currentLineIndex = (oldIndex + UpOrDown) % numberOfItems;
                listView1.SelectedIndices.Add(currentLineIndex);
            }
        }



    }
}
