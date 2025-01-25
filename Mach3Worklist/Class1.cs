using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mach3Worklist
{
    internal class Token : TokenBase
    {
       public enum CommandType
        {
            Badcommand,
            Coment,
            GCode,
            MCode
        }
        public Token()
        {
            Type = CommandType.Badcommand;
            Command = "";
            Argument = "";
        }

        public CommandType Type { get; set; }
        public string Command { get; set; }
        public string Argument { get; set; }
    }
}
