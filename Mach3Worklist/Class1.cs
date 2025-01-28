using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mach3Worklist
{
    internal class Token 
    {
       
        public Token()
        {
            Type = Tokenizer.CommandType.Badcommand;
            Command = "";
            Argument = "";
        }
        public Tokenizer.CommandType Type { get; set; }
        public string Command { get; set; }
        public string Argument { get; set; }
    }
}
