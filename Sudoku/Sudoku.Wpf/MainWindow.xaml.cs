using Sudoku.Wpf.Models;
using Sudoku.Wpf.Services;
using Sudoku.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sudoku.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int MaxValue = 9;
        private ISudokuService sudokuRules;

        public MainWindow()
        {
            InitializeComponent();

            sudokuRules = new SudokuService(MaxValue);
            Sudoku = sudokuRules.GetNew(30,45);
            SudoukuItem.ItemsSource = Sudoku;
            tbLevel.Text = Level.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        protected void SetProperty<T>(string propertyName, ref T field, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
        }


        private ObservableCollection<SudokuValue>? _sudoku;
        public ObservableCollection<SudokuValue>? Sudoku
        {
            get { return _sudoku; }
            set { SetProperty(nameof(Sudoku), ref _sudoku, value); }
        }

        public int Level = 3;
        public int Trys = 100; 
        public int Fieldsleft = 20;



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(tbLevel.Text, out Level))
                Level = 1;
            else if (Level > 10)
                Level = 10;

            switch (Level)
            {
                case 1:
                    Trys = 10;
                    Fieldsleft = 60;
                    break;
                case 2:
                    Trys = 20;
                    Fieldsleft = 55;
                    break;
                case 3:
                    Trys = 30;
                    Fieldsleft = 45;
                    break;
                case 4:
                    Trys = 40;
                    Fieldsleft = 40;
                    break;
                case 5:
                    Trys = 100;
                    Fieldsleft = 35;
                    break;
                case 6:
                    Trys = 200;
                    Fieldsleft = 30;
                    break;
                case 7:
                    Trys = 300;
                    Fieldsleft = 25;
                    break;
                case 8:
                    Trys = 350;
                    Fieldsleft = 20;
                    break;
                case 9:
                    Trys = 400;
                    Fieldsleft = 15;
                    break;
                case 10:
                    Trys = 450;
                    Fieldsleft = 10;
                    break;
            }      
            tbLevel.Text = Level.ToString();
            SudoukuItem.ItemsSource = new ObservableCollection<SudokuValue>();
            Sudoku = sudokuRules.GetNew(Level, Fieldsleft);
            SudoukuItem.ItemsSource = Sudoku;
        }
    }
}
