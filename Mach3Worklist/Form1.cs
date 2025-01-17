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
using Mach4;
using System.Runtime.InteropServices;


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
        private enum M3Status
        {
            Non,
            GCodeLoading,
            GCodeRuning,
            GCodeCompleted,
            GCodeError
        }
        public Form1()
        {
            InitializeComponent();
            listFileName = "";
            this.Text = "Mach3 worklist "+listFileName;
            // здесь будет загрузка сохраненных настроек
            eMode = ExeMode.Line;
            if (GetMachInstance())
                eStatus = ExeStatus.Ready;
            updateUI(eStatus);
            timer1.Enabled = true;
            timer1.Interval = 50;
            timer1.Start();
        }

        // Раздел взаимодействие с Mach3
        public bool GetMachInstance()
        {
            try
            {
                _mach = (IMach4)Marshal.GetActiveObject("Mach4.Document");
                _mInst = (IMyScriptObject)_mach.GetScriptDispatch();
                return true; 
            }
            catch
            {
                _mach = null;
                _mInst = null;
                return false;
            }
        }
        // МЕНЮ
        // РАЗДЕЛ МЕНЮ - ФАЙЛ
        // Новый ворклист
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
                updateUI(ExeStatus.Ready);
            }
        }
        // подтверждение сохранения изменений
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
        // Сохранить ворклист
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveList();
        }
        private void saveList()
        {
            if (this.listFileName == "")
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Mach3 work list (.m3l) | *.m3l";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    listFileName = sfd.FileName.ToString();
                }
            }
            if (listFileName != "" && listFileName != null)
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
        // Сохранить ворклист под новым именем
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Mach3 work list (.m3l) | *.m3l";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                listFileName = sfd.FileName.ToString();
            }
            saveList();
        }
        // Открыть ворклист
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
                updateUI(ExeStatus.Ready);
            }
        }

        // РАЗДЕЛ МЕНЮ - СПИСОК
        // Новая строка
        private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = "CNC G-code files (*.tap; *.nc) | *.tap; *nc| All files(*.*) | *.*";

            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                ListViewItem lvItem = new ListViewItem(dlgOpenFile.FileName);
                lvItem.SubItems.Add("0");
                lvItem.SubItems.Add("1");
                lvItem.SubItems.Add("1");
                this.listView1.Items.Add(lvItem);
            }
        }

        // Удалить строку
        private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                this.listView1.Items.Remove(this.listView1.SelectedItems[0]);
            }
        }

        // Переместить строку вверх
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

        // Переместить строку вниз
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

        // Открыть программу G-code для просмотра и изменения
        private void openGCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        // ВВОД ЗНАЧЕНИЙ В СПИСОК
        // Количество - выполнено
        private void quantityEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count < 1) { return; }
            SelectedLSI = listView1.SelectedItems[0].SubItems[1];
            string input = Microsoft.VisualBasic.Interaction.InputBox("Количество", "Введите значение", SelectedLSI.Text);
            string output = string.Concat(input.Where(char.IsDigit));
            if (output.Length > 0)
            {
                SelectedLSI.Text = output;
            }
        }
        // Квота - выполнить
        private void quotaEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count < 1) { return; }
            SelectedLSI = listView1.SelectedItems[0].SubItems[2];
            string input = Microsoft.VisualBasic.Interaction.InputBox("Квота", "Введите значение", SelectedLSI.Text);
            string output = string.Concat(input.Where(char.IsDigit));
            if (output.Length > 0)
            {
                SelectedLSI.Text = output;
            }
        }
        // Зона - номер рабочей зоны
        private void zoneEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count < 1) { return; }
            SelectedLSI = listView1.SelectedItems[0].SubItems[3];
            string input = Microsoft.VisualBasic.Interaction.InputBox("Зона", "Введите значение", SelectedLSI.Text);
            string output = string.Concat(input.Where(char.IsDigit));
            if (output.Length > 0)
            {
                SelectedLSI.Text = output;
            }
        }

        // ОБРАБОТКА СОБЫТИЙ СПИСКА
        // Клик на списке, сохраняем индекс текущей строки
        private void listView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                currentLineIndex = this.listView1.Items.IndexOf(listView1.SelectedItems[0]);
            }
        }

        // Клик правой кнопкой мыши для вызова контекстного меню
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(listView1, e.X, e.Y);
            }
        }

        // Двойной клик по списку. Вызывает ввод значений количества, квоты и зоны
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
        // Выбор режима обхода списка по кругу
        // пока не достигнута квота выполняем по 1 разу программу из текущей строки
        // с переходом на следующую
        // список выполнен когда во всех строках количество равно квоте
        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.circleToolStripMenuItem.Checked = true;
            this.lineToolStripMenuItem.Checked = false;
            this.selectiveToolStripMenuItem.Checked = false;
            this.lblMode.Text = "По кругу";
            this.eMode = ExeMode.Circle;
        }
        // Выбор режима обхода списка линейно
        // пока в текущей строке количество не сравняется с квотой
        // выполняем программу из текущей строки, затем переходим к следующей строке
        // список выполнен когда в последней строке количество достигнет квоты
        private void lineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.lineToolStripMenuItem.Checked = true;
            this.circleToolStripMenuItem.Checked = false;
            this.selectiveToolStripMenuItem.Checked = false;
            this.lblMode.Text = "Линейно";
            this.eMode = ExeMode.Line;
        }
        // Выбор режима обхода списка по выбору внешней кнопкой
        // в этом режиме производится опрос Mach3 на активацию
        // входов  функцией IsActive(Input);
        // привязка входов производится в экземпляре класса Zone
        // Список просматривается сверху вниз каждый раз после срабатывания
        // внешней кнопки.  
        // 
        private void selectiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.selectiveToolStripMenuItem.Checked = true;
            this.lineToolStripMenuItem.Checked = false;
            this.circleToolStripMenuItem.Checked = false;
            this.lblMode.Text = "Выборочно";
            this.eMode = ExeMode.Selective;
        }

        // Подключение к Mach3 если прервалось соединение
        private void connectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(GetMachInstance())
            updateUI(ExeStatus.Ready);
        }


        // ОБРАБОТКА ВЫПОЛНЕНИЯ ПРОГРАММ 
        // Первый этап выполнения программы
        // Если список не выполнен - загружается программа из текущей строки списка
        private void firstStep()
        {
            if (!worklistComplete()) { _mInst.LoadRun(this.listView1.Items[currentLineIndex].Text);}
        }
        // После выполнения программы увеличивается количество в текущей строке
        // 
        private void nextStep()
        {
            int quota = System.Convert.ToInt32(this.listView1.SelectedItems[0].SubItems[2].Text);
            int count = System.Convert.ToInt32(this.listView1.SelectedItems[0].SubItems[1].Text);
            if (m3Status != M3Status.GCodeCompleted) { return; }
            count++;
            if (this.eMode == ExeMode.Circle)
            {
                if (count <= quota)
                {
                    
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
                    updateUI(ExeStatus.Сompleted);
                    return;
                }
            }
            else if (this.eMode == ExeMode.Line)
                if (count <= quota )
                {
                   // count++;
                    this.listView1.Items[currentLineIndex].SubItems[1].Text = count.ToString();
                }
                else if (currentLineIndex + 1 < this.listView1.Items.Count)
                {
                    this.listView1.Items[currentLineIndex].Selected = false;
                    currentLineIndex++;
                    this.listView1.Items[currentLineIndex].Selected = true;
                }
                else
                {
                    updateUI(ExeStatus.Сompleted);
                    return;
                }
            else if (this.eMode == ExeMode.Selective)
            {

            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if(_mInst == null) { return; }
            if (_mInst.IsLoading()==1) { m3Status = M3Status.GCodeLoading;}
            if (m3Status == M3Status.GCodeLoading&&_mInst.GetOEMLed(804)) {m3Status = M3Status.GCodeRuning;}
            if(m3Status==M3Status.GCodeRuning&& _mInst.GetOEMLed(804) == false)
            {
                if (_mInst.IsEstop()==1) { m3Status = M3Status.GCodeError; } 
                else 
                { 
                    m3Status = M3Status.GCodeCompleted;
                    nextStep();
                    if(worklistComplete())
                    {
                        eStatus = ExeStatus.Сompleted;
                        updateUI(eStatus);
                    }
                    if (eStatus == ExeStatus.Сompleted) { return; }
                    updateUI(ExeStatus.Runing);
                }
            }
        }

        private void btnStart_Click_1(object sender, EventArgs e)
        {
            // активирует процесс обработки таблицы.
            if (this.listView1.SelectedItems.Count < 1) { return;}
            if(eStatus==ExeStatus.Non) { return; }
            if (eStatus == ExeStatus.Runing)
            {
                updateUI(ExeStatus.Stoped);
            }
            else
            {
                updateUI(ExeStatus.Runing);
                firstStep();
            }
        }

        private void updateUI(ExeStatus status)
        {

            if (status == ExeStatus.Runing)
            {
                if (listView1.SelectedIndices.Count > 0)
                {
                    this.btnStart.Text = "Стоп";
                    this.lblStatus.Text = "Выполняется " + this.listView1.Items[currentLineIndex].Text;
                }
            }
            else if (status == ExeStatus.Stoped)
            {
                this.btnStart.Text = "Старт";
                this.lblStatus.Text = "Остановлен";
            }
            else if (status == ExeStatus.Сompleted)
            {
               // this.timer1.Stop();
                this.btnStart.Text = "Старт";
                this.lblStatus.Text = "Завершен";
            }
            else if (status == ExeStatus.Ready)
            {
                if (!GetMachInstance())
                {
                    lblStatus.Text = "Нет подключения";
                    eStatus = ExeStatus.Non;
                    return;
                }
                else
                {
                    lblStatus.Text = "Подключено к - " + _mInst.GetActiveProfileName();
                }
                this.btnStart.Text = "Старт";

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
            else if (status == ExeStatus.Aborted) 
            {
                lblStatus.Text = "Авария!!!";
            }
        }
        // Проверка на полное выполнение списка
        // Список выполнен если в режиме обхода по кругу количество равно квоте в каждой строке
        // в режиме линейно количество равно квоте в последней строке списка 
        // режим выборочно ......
        private bool worklistComplete()
        {
            int stepCount = this.listView1.Items.Count;
            int index = currentLineIndex;
            if (eMode == ExeMode.Line) 
            {
                stepCount -= currentLineIndex;
            }
            for (int i = 0; i < stepCount; i++)
            {
                int quota = System.Convert.ToInt32(this.listView1.Items[index].SubItems[2].Text);
                int count = System.Convert.ToInt32(this.listView1.Items[index].SubItems[1].Text);
                if (count != quota)
                {
                    currentLineIndex = index;
                    return false;
                }
                index++;
                if (index== this.listView1.Items.Count)
                {
                    index = 0;
                }
            }
            return true;
        }
        
    }
}
