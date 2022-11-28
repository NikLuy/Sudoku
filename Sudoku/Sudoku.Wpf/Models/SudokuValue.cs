using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Wpf.Models
{
    public class SudokuValue
    {
        public int? Value { get; set; }  
        public int PosX { get; set; }
        public int PosY { get; set; }
    }
}
