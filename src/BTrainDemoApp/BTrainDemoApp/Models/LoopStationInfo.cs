using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTrainDemoApp.Models
{
    public class Position
    {
        public int Row { get; }

        public int Col { get; }

        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }

    public class LoopStationInfo
    {
        public Position ArrowRight { get; }

        public Position ArrowLeft { get; }

        public Position Station { get; }

        public string StationName { get; }

        public string DirOuter { get; }

        public string DirInner { get; }

        public string NextOuter { get; }

        public string NextInner { get; }

        public bool IsUpper { get; }

        public LoopStationInfo(int row, int col, string name, string dirOuter, string dirInner, string nextOuter, string nextInner, bool isUpper = true)
        {
            StationName = name;
            DirOuter = dirOuter;
            DirInner = dirInner;
            NextOuter = nextOuter;
            NextInner = nextInner;

            Station = new Position(row, col);

            ArrowLeft = new Position(row, col + 1);
            ArrowRight = new Position(row, col - 1);
            IsUpper = isUpper;
        }
    }
}
