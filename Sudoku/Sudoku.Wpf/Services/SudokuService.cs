using Sudoku.Wpf.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sudoku.Wpf.Services
{
    public interface ISudokuService
    {
        //bool chkMove(int col, int row, int value, int[,] arr);
        // int[,] BSudoku(int value, int py, int px, int[,] arr, ref int counter, ref bool solved);
        ObservableCollection<SudokuValue> GetNew(int Level = 1000, int FieldsLeft = 30);
    }

    public class SudokuService : ISudokuService
    {
        #region Initialize
        public SudokuService(int MaxValue)
        {
            maxValue = MaxValue;
        }
        private int maxValue { get; set; } = 9;
        private int divider { get; set; } = 3;
        #endregion Initialize

        #region Rules
        private bool chkRow(int row, int value, int[,] arr)
        {
            for (int i = 0; i < maxValue; i++)
            {
                if (arr[i, row] == value) return false;
            }
            return true;
        }
        private bool chkCol(int col, int value, int[,] arr)
        {
            for (int i = 0; i < maxValue; i++)
            {
                if (arr[col, i] == value)
                    return false;
            }
            return true;
        }
        private bool chkBlock(int row, int col, int value, int[,] arr)
        {
            int BlockValue = maxValue / divider;
            for (int i = 0; i < BlockValue; i++)
            {
                if (i == col) continue;
                for (int j = 0; j < BlockValue; j++)
                {
                    if (j == row) continue;
                    if (arr[col - (col % divider) + i, row - (row % divider) + j] == value) return false;
                }
            }
            return true;
        }
        private bool chkMove(int row, int col, int value, int[,] arr)
        {
            if (!chkRow(row, value, arr)) return false;
            if (!chkCol(col, value, arr)) return false;
            if (!chkBlock(row, col, value, arr)) return false;
            return true;
        }
        #endregion Rules

        #region Analyse
        private int[] Analyze(int[,] field)
        {
            int arrCol = maxValue * maxValue;
            int[,] PossibleMoves = new int[arrCol, maxValue + 1];
            int[] bestPos = new int[3];
            int tmpval = 0;
            bestPos[0] = 0;
            bestPos[1] = int.MaxValue;
            bestPos[2] = 0;
            for (int i = 0; i < arrCol; i++)
            {
                if (field[i / maxValue, i % maxValue] == 0)
                {
                    for (int val = 1; val <= maxValue; val++)
                    {
                        if (chkMove(i % maxValue, i / maxValue, val, field))
                        {
                            PossibleMoves[i, val] = 1;
                            PossibleMoves[i, val] = 1;
                            PossibleMoves[i, 0]++;
                            tmpval = val;
                        }
                    }
                }
                if (PossibleMoves[i, 0] > 0 && PossibleMoves[i, 0] < bestPos[1])
                {
                    bestPos[2] = tmpval;
                    bestPos[1] = PossibleMoves[i, 0];
                    bestPos[0] = i;
                }
            }
            return bestPos;
        }
        #endregion Analyse

        #region Solve
        private List<int[,]> solutions = new List<int[,]>();
        private int[,] Solve(int value, int px, int py, int[,] field, ref int counter, ref int solved, int mode)
        {
            if (mode == 1 && solved >= 1) return null;
            if (mode == 2 && solved >= 2) return null;

            int[,] workerfield = new int[maxValue, maxValue];
            int bestVal = 1;
            bool chkSkip = false;
            Array.Copy(field, workerfield, field.Length);
            workerfield[px, py] = value;
            for (int x = 0; x < maxValue; x++)
            {
                for (int y = 0; y < maxValue; y++)
                {
                    if (y == 0 && x == 0)
                    {
                        int[] BestPos = Analyze(workerfield);
                        y = BestPos[0] % maxValue;
                        x = BestPos[0] / maxValue;
                        if (BestPos[1] == 1)
                        {
                            bestVal = BestPos[2];
                            chkSkip = true;
                        }
                    }

                    if (workerfield[x, y] == 0)
                    {
                        for (int val = bestVal; val <= maxValue; val++)
                        {
                            if (chkSkip || chkMove(y, x, val, workerfield))
                            {
                                chkSkip = false;
                                Solve(val, x, y, workerfield, ref counter, ref solved, mode);
                            }
                        }
                        counter++;
                        return null;
                    }
                }
            }
            solutions.Add(workerfield);
            solved++;
            return workerfield;
        }
        #endregion Solve

        #region Create
        private Random rnd = new Random();
        private int[] Shuffle()
        {
            int[] ArrayToShuffle = new int[maxValue];
            for (int i = 0; i < maxValue; i++)
            {
                ArrayToShuffle[i] = i + 1;
            }
            return ArrayToShuffle.OrderBy(x => rnd.Next()).ToArray();
        }

        public int[,] Create(int fieldsLeft)
        {
            int row, col, n, counter, solved, selectedIndependentBlocks;
            int[,] OutputField = new int[maxValue, maxValue];
            int[,] tmp = new int[maxValue, maxValue];
            int[,] IndependentBlocks = new int[6, 3] { { 0, 30, 60 }, { 0, 33, 57 }, { 3, 27, 60 }, { 3, 33, 54 }, { 6, 30, 54 }, { 6, 27, 57 } };
            int[] ShuffledArray;
            selectedIndependentBlocks = rnd.Next(5);
            counter = 0;
            solved = 0;
            for (int currentBlock = 0; currentBlock < 3; currentBlock++)
            {
                ShuffledArray = Shuffle();
                row = IndependentBlocks[selectedIndependentBlocks, currentBlock] / maxValue;
                col = IndependentBlocks[selectedIndependentBlocks, currentBlock] % maxValue;
                n = 0;
                for (int i = 0; i < divider; i++)
                {
                    for (int j = 0; j < divider; j++)
                    {
                        OutputField[col - (col % divider) + i, row - (row % divider) + j] = ShuffledArray[n++];
                    }
                }
            }
            Solve(0, 0, 0, OutputField, ref counter, ref solved, 1);
            OutputField = solutions[0];
            n = (maxValue * maxValue) - fieldsLeft;
            Array.Copy(OutputField, tmp, OutputField.Length);
            List<string> usedCords = new List<string>();
            do
            {
                string usedCord = string.Empty;
                do
                {
                    row = rnd.Next(maxValue);
                    col = rnd.Next(maxValue);
                    usedCord = $"{row}_{col}";
                } while (usedCords.Contains(usedCord) && usedCords.Count < maxValue*maxValue);
                usedCords.Add(usedCord);
                solutions.Clear();
                tmp[row, col] = 0;
                counter = 0;
                solved = 0;
                Solve(0, 0, 0, tmp, ref counter, ref solved, 0);
                if (solved == 1)
                {
                    Array.Copy(tmp, OutputField, OutputField.Length);
                    n--;
                }
                else
                {
                    Array.Copy(OutputField, tmp, OutputField.Length);

                }
            } while (n >= 0);

            return OutputField;
        }
        #endregion Create

        public ObservableCollection<SudokuValue> GetNew(int Level = 1000, int FieldsLeft = 30)
        {
            #region old
            //int[,] myArray;
            ////myArray = new int[maxValue, maxValue];
            //myArray = new int[,]
            //{
            //    {0,2,0,0,0,0,0,4,3 },
            //    {0,5,0,3,0,7,6,0,0 },
            //    {0,0,6,0,2,0,0,0,0 },
            //    {0,0,3,0,4,8,0,9,0 },
            //    {0,0,0,0,6,0,0,0,0 },
            //    {0,9,0,1,5,0,2,0,0 },
            //    {0,0,0,0,1,0,3,0,0 },
            //    {0,0,8,5,0,6,0,1,0 },
            //    {7,1,0,0,0,0,0,5,0 },
            //};
            //Solve(0, 0, 0, myArray ,ref counter, ref solved,0);
            //Debug.WriteLine($"Solved in {DateTime.Now - Start}, Backtracking: {counter}");
            #endregion old

            DateTime Start = DateTime.Now;
            int counter = 0;
            int solved = 0;
            int[,] Input;
            do
            {
                Input = Create(FieldsLeft);
                counter = 0;
                solved = 0;
                Solve(0, 0, 0, Input, ref counter, ref solved, 0);
            } while (counter < Level);

            //DataTable dt = new DataTable();
            //for (int i = 0; i < maxValue; i++)
            //{
            //    dt.Columns.Add(i.ToString(), typeof(string));
            //}

            //for (int row = 0; row < maxValue; row++)
            //{
            //    DataRow dr = dt.NewRow();
            //    for (int col = 0; col < maxValue; col++)
            //    {
            //        dr[col] = Input[col,row] == 0?"": Input[col, row].ToString();
            //    }
            //    dt.Rows.Add(dr);
            //}
            //return dt;
            return FillSudoku(Input);
        }
        private ObservableCollection<SudokuValue> FillSudoku(int[,] Values)
        {
            try
            {
                var SudokuValues = new ObservableCollection<SudokuValue>();
                for (int i = 0; i < Values.GetLength(0); i++)
                {
                    for (int j = 0; j < Values.GetLength(1); j++)
                    {

                        SudokuValues.Add(new SudokuValue() { Value = Values[i, j] != 0 ? Values[i, j] : null, PosY = i, PosX = j });

                    }
                }
                return SudokuValues;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new ObservableCollection<SudokuValue>();
            }
        }

        private int[,] GetFieldValues(ObservableCollection<SudokuValue> sudokuValues)
        {
            int X = sudokuValues.Last().PosX;
            int[,] FieldValues = new int[X, X];
            foreach (SudokuValue v in sudokuValues)
            {
                FieldValues[v.PosY, v.PosX] = v.Value == null ? 0 : (int)v.Value;
            }
            return FieldValues;
        }
    }
}
