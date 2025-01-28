using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;


namespace Mach3Worklist
{
    internal class Tokenizer
    {
        public enum CommandType
        {
            Badcommand,
            LineNum,
            Message,
            Coment,
            GCode,
            MCode,
            Axis,
            Parameter,
            Variable
        }
        public Tokenizer()
        {
            commands = new Dictionary<string, CommandType>();
            stringLine = "";
            commands.Add("(", CommandType.Message);
            commands.Add("%", CommandType.Coment);
            commands.Add(";", CommandType.Coment);
            commands.Add("#", CommandType.Variable);
            commands.Add("N", CommandType.LineNum);
            commands.Add("G", CommandType.GCode);
            commands.Add("M", CommandType.MCode);
            commands.Add("O", CommandType.Parameter);
            commands.Add("T", CommandType.Parameter);
            commands.Add("S", CommandType.Parameter);
            commands.Add("F", CommandType.Parameter);
            commands.Add("L", CommandType.Parameter);
            commands.Add("P", CommandType.Parameter);
            commands.Add("R", CommandType.Parameter);
            commands.Add("I", CommandType.Parameter);
            commands.Add("J", CommandType.Parameter);
            commands.Add("K", CommandType.Parameter);
            commands.Add("X", CommandType.Axis);
            commands.Add("Y", CommandType.Axis);
            commands.Add("Z", CommandType.Axis); 
            commands.Add("A", CommandType.Axis);
            commands.Add("B", CommandType.Axis);
            commands.Add("C", CommandType.Axis);
            commands.Add("U", CommandType.Axis);
            commands.Add("V", CommandType.Axis);
            commands.Add("W", CommandType.Axis);
        }
        private Dictionary<string, CommandType> commands;
        private string stringLine;
        private int stringLineIndex;         
        public void tokenize(string sValue, int iValue)
        {
            if (stringLine == null) 
            {
                return;
            }
            int cursor = 0;
            CommandType command = CommandType.Badcommand;
            string key = "";
            stringLine = sValue;
            stringLineIndex = iValue;
            while (cursor < stringLine.Length)
            {
                key = stringLine.Substring(cursor,1);
                if (commands.TryGetValue(key, out command)) 
                {
                    switch (command)
                    {
                        case CommandType.Badcommand:

                            break;
                        case CommandType.Coment:

                            break;
                    }
                } else { command = CommandType.Badcommand; }

                cursor++;
            }

        }

    }
}
