using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTrainDemoApp.Models
{
    public class StationInfo
    {
        public string NameFull { get; }

        public string NameKana { get; }

        public string NameEng { get; }

        public string NextOut { get; }

        public string NextIn { get; }

        public StationInfo(string name, string kana, string eng, string nextOut, string nextIn)
        {
            NameFull = name;
            NameKana = kana;
            NameEng = eng;
            NextOut = nextOut;
            NextIn = nextIn;
        }
    }
}
