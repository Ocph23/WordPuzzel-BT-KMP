﻿using GameWordPuzzel.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GameWordPuzzel.Views
{
    /// <summary>
    /// Interaction logic for GameBoard.xaml
    /// </summary>
    public partial class GameBoard : Window,INotifyPropertyChanged
    {
        public GameBoard(MainWindow main, Cordinate cordinate)
        {
            InitializeComponent();
            this.main = main;
            this.columns = cordinate.Column;
            this.rows = cordinate.Row;
            this.Loaded += GameBoard_Loaded;
            this.DataContext = this;
           
        }

        public List<string> DataSource { get; private set; }
        public bool Start { get; private set; }
        public List<ButtonView> ListSelected { get; private set; }
        public Brush CurrentColor { get; private set; }
        public int SleepTime { get; private set; }
        public ObservableCollection<Kategori> Kategories { get;  set; }

        // public List<string> ListResult { get;  set; }
        private List<string> NotAccepted = new List<string>();

        private Random random = new Random();
       private  Random rrow = new Random();
        private Random rarah = new Random();
        private int columns;
        private int rows;
        private Kategori _selectedCategories;
        private MainWindow main;

        private void GameBoard_Loaded(object sender, RoutedEventArgs e)
        {
            DataSource = new List<string>();
            using (var db = new OcphDbContext())
            {
                var cats = db.Categories.Select().ToList();
                foreach (var item in cats)
                {
                    item.Words = db.Words.Where(O => O.KategoriId == item.Id).ToList();
                }
                this.Kategories = new ObservableCollection<Kategori>(cats);
                this.combo.ItemsSource = Kategories;
            }

            this.RefreshBoard();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Kategori CategorySelected
        {
            get { return _selectedCategories; }
            set
            {
                if (_selectedCategories != value)
                {
                    _selectedCategories = value;
                    DataSource.Clear();
                    if (value.Words == null)
                        value.Words = new List<Kata>();

                    foreach (var item in value.Words.Where(O=>O.Nilai.Length<=columns))
                    {
                       DataSource.Add(item.Nilai);
                    }

                    RefreshBoard();

                    NotifyPropertyChanged();
                }
            }
        }

        public Dictionary<ArahPanah, List<DataPosition>> DataKMP { get; private set; }

        private void RefreshBoard()
        {

            console.Document.Blocks.Clear();
            var datas = Helper.ShuffleList<string>(DataSource).Take(columns+columns/2);
            dataToSearchView.Children.Clear();
            canvas.Children.Clear();
            canvas.ColumnDefinitions.Clear();
            canvas.RowDefinitions.Clear();
            

            for (int i = 0; i < rows; i++)
            {
                canvas.RowDefinitions.Add(new RowDefinition());

            }
            for (int j = 0; j < columns; j++)
            {
                canvas.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (var i = 0; i < canvas.RowDefinitions.Count; i++)
            {
                for (var j = 0; j < canvas.ColumnDefinitions.Count; j++)
                {
                    var button = new ButtonView(new Cordinate(this.columns,this.rows)) { FontSize = 30-columns, Row = i, Column = j, Name = string.Format("R{0}C{1}", i, j) };
                    button.main.Content = Helper.RandomString(random);
                    button.MouseEnter += Button_MouseEnter; ;
                    button.main.Click += Main_Click;
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    this.canvas.Children.Add(button);
                }
            }

            /*
             Percobaan
             */
            var test = "KAKIKIKIRSASASSASSSS";
            DataPosition dp = new DataPosition();
            int c=0,r=0;
            foreach (ButtonView btn in canvas.Children)
            {
                if (btn.Column == c && btn.Row==r)
                {
                    btn.main.Content = test.Substring(c, 1);
                    dp.AddButton(btn);
                    SetObjectColorAndUsed(btn);
                    c++;r++;
                }
            }
            dataToSearchView.Children.Add(new BorderLabel("KIKIR"));

            //Random text
            CurrentColor = Brushes.Red;
            bool RandomCompleted = false;
            while (!RandomCompleted)
            {
                foreach (var item in datas.OrderByDescending(O => O.Length))
                {
                    try
                    {
                        var arah = Helper.RandomArah(rarah);
                        var row = Helper.RandomPosision(rrow,rows);
                        var col = Helper.RandomPosision(rrow,columns);
                        var dataposition = new DataPosition(item);


                        DataPosition data = null;
                        if (arah == Arah.Horizontal)
                        {
                            data = HorizontalPlace(item, row, col);
                          
                        }
                        else if (arah == Arah.Vertical)
                        {
                            data = VerticalPlace(item, row, col);
                           

                        }
                        else if (arah == Arah.Diagonal)
                        {
                             data = DiagonalPlace(item, row, col);
                        }

                        if(data!=null && data.Datas.Count>0)
                        {
                            dataToSearchView.Children.Add(new BorderLabel(data.Content));
                        }
                      

                    }
                    catch (Exception ex)
                    {
                        throw new SystemException(ex.Message);
                    }


                }
                RandomCompleted = true;
              
            }


        }

        private void Main_Click(object sender, RoutedEventArgs e)
        {

            this.Start = !Start;
            var c = (Button)sender;
            var grid = (Grid)c.Parent;
            var btn = (ButtonView)grid.Parent;
          //  btn.ShowLeft();
            if (Start)
            {
                this.ListSelected = new List<ButtonView>();
                CurrentColor = Helper.PickBrush();
                // grid.Children.Add(Helper.NewBorder(CurrentColor));
                btn.SetIsUsed(CurrentColor);
                btn.IsStarter = true;
            }
            else
            {
                if (btn.IsStarter)
                {
                    foreach (var item in ListSelected)
                    {
                        ClearBorder(item.main);
                    }
                    ListSelected.Clear();
                }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (var item in ListSelected)
                    {
                        sb.Append(item.main.Content);
                    }

                    if(DataSource.Where(O=>O==sb.ToString()).Count()>0)
                    {
                        foreach (UIElement item in dataToSearchView.Children)
                        {
                            if (item.DependencyObjectType.Name == "BorderLabel")
                            {
                                BorderLabel border = (BorderLabel)item;
                                if (border.Name == sb.ToString())
                                {
                                    border.Found();
                                }

                            }
                        }

                        //ListResult.Add(sb.ToString());
                    }else
                    {
                        ClearSelected();
                    }


                    
                  
                }

            }

            this.ListSelected.Add(btn);

        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            var btn = (ButtonView)sender;
          //  Grid grid = (Grid)btn.main.Parent;
            if (this.Start == true)
            {
                var Avaliable = ListSelected.Where(O => O.Column == btn.Column && O.Row == btn.Row).FirstOrDefault();
                var lastBtn = ListSelected.LastOrDefault();
                if (lastBtn == btn)
                {


                }
                else if (lastBtn != null && lastBtn != btn && Avaliable==null)
                {
                    if (lastBtn.IsStarter && ListSelected.Count==1)
                    {

                        //  grid.Children.Add(Helper.NewBorder(CurrentColor));
                        btn.SetIsUsed(CurrentColor);
                        this.ListSelected.Add(btn);
                    }
                    else if (!lastBtn.IsStarter && ListSelected.Count > 1)
                    {
                        var starter = ListSelected.Where(O => O.IsStarter).FirstOrDefault();

                        if (starter.Row==btn.Row && lastBtn.Column-starter.Column>=0 && btn.Column-lastBtn.Column==1)
                        {
                            //Horizontal Left To Right
                            //  grid.Children.Add(Helper.NewBorder(CurrentColor));
                            btn.SetIsUsed(CurrentColor);
                            this.ListSelected.Add(btn);
                        }
                        else if (starter.Row == btn.Row && lastBtn.Column - starter.Column < 0 && lastBtn.Column- btn.Column  == 1)
                        {
                            //Horizontal Right To Left
                            // grid.Children.Add(Helper.NewBorder(CurrentColor));
                            btn.SetIsUsed(CurrentColor);
                            this.ListSelected.Add(btn);
                        }
                        else if (starter.Column == btn.Column && lastBtn.Row - starter.Row >= 0 && btn.Row - lastBtn.Row == 1)
                        {
                            //Verical Top To Bottom
                            //grid.Children.Add(Helper.NewBorder(CurrentColor));
                            btn.SetIsUsed(CurrentColor);
                            this.ListSelected.Add(btn);
                        }
                        else if (starter.Column == btn.Column && lastBtn.Row - starter.Row < 0 && lastBtn.Row- btn.Row  == 1)
                        {
                            //Vertical Bottom To Top
                            // grid.Children.Add(Helper.NewBorder(CurrentColor));
                            btn.SetIsUsed(CurrentColor);
                            this.ListSelected.Add(btn);
                        }
                        else if (starter.Column < lastBtn.Column && starter.Row<lastBtn.Row && btn.Column-lastBtn.Column==1 && btn.Row-lastBtn.Row==1)
                        {
                            //Diagonal Left To Right To Bottom
                            //  grid.Children.Add(Helper.NewBorder(CurrentColor));
                            btn.SetIsUsed(CurrentColor);
                            this.ListSelected.Add(btn);
                        }
                        else if (starter.Column > lastBtn.Column && starter.Row > lastBtn.Row && btn.Column - lastBtn.Column < 0 && btn.Row - lastBtn.Row <0)
                        {
                            //Diagonal Left To Right To Top
                            // grid.Children.Add(Helper.NewBorder(CurrentColor));
                            btn.SetIsUsed(CurrentColor);
                            this.ListSelected.Add(btn);
                        }
                        else if (starter.Column > lastBtn.Column && starter.Row < lastBtn.Row && btn.Column - lastBtn.Column <0 && btn.Row - lastBtn.Row == 1)
                        {
                            //Diagonal Right To Left To Bottom
                            // grid.Children.Add(Helper.NewBorder(CurrentColor));
                            btn.SetIsUsed(CurrentColor);
                            this.ListSelected.Add(btn);
                        }
                        else if (starter.Column < lastBtn.Column && starter.Row > lastBtn.Row && btn.Column - lastBtn.Column ==1 && btn.Row - lastBtn.Row < 0)
                        {
                            //Diagonal Right To left Top
                            //  grid.Children.Add(Helper.NewBorder(CurrentColor));
                            btn.SetIsUsed(CurrentColor);
                            this.ListSelected.Add(btn);
                        }
                    }
                    
                }
                else
                {
                    foreach (var item in ListSelected)
                    {
                        //var locals = item.GetLocalValueEnumerator();
                        //while (locals.MoveNext())
                        //{
                        //    var a = locals.Current.Property;
                        //}
                        ////   item.Background.ClearValue(item.Background);
                    }
                }

            }
        }

        private void ClearSelected()
        {
            foreach (var item in ListSelected)
            {
                ClearBorder(item.main);
            }
            this.Start = false;
            this.ListSelected.Clear();
        }

        public void ClearBorder(Button item)
        {
            var grid = (Grid)item.Parent;
            UIElement border = null;
            foreach (UIElement b in grid.Children)
            {
                if (b.DependencyObjectType.Name == "Border")
                {
                    border = b;
                }
            }

            if (border != null)
                grid.Children.Remove(border);
        }

        private DataPosition DiagonalPlace(string item, int row, int col)
        {

            var dataposition = new DataPosition(item);
            int[,] matrix = new int[rows, columns];
            var lrc = Helper.FindOnesFromLeftToRight(matrix, row, col);
            var rlc = Helper.FindOnesFromRightToLeft(matrix, row, col);
            if (lrc.Number >= item.Length)
            {
                if (lrc.SecoundCount >= item.Length)
                {
                    bool completed = false;
                    int error = 0;
                    while (!completed)
                    {

                        try
                        {
                            var index = 0;
                            int colTest = col;
                            int rowTest = row;
                            if (Helper.FindOnesFromLeftToRight(matrix, row, col).Number < item.Length)
                            {
                                throw new SystemException();
                            }
                            else
                            {


                                Dictionary<int, string> indexUseds = new Dictionary<int, string>();
                                foreach (ButtonView btn in canvas.Children)
                                {
                                    if (btn.Column == colTest && btn.Row == rowTest && index < item.Length)
                                    {
                                        if (btn.IsUsed)
                                        {
                                            if (item.Substring(index, 1).ToString() != btn.main.Content.ToString())
                                            {
                                                indexUseds.Add(index, btn.main.Content.ToString());
                                                throw new SystemException();
                                            }
                                        }
                                        rowTest++;
                                        colTest++;
                                        index += 1;
                                    }
                                }
                                index = 0;
                                foreach (ButtonView btn in canvas.Children)
                                {
                                    if (btn.Column == col && btn.Row == row && index < item.Length)
                                    {
                                        btn.main.Content = item.Substring(index, 1);
                                        dataposition.Datas.Add(btn);
                                        SetObjectColorAndUsed(btn);
                                        row++;
                                        col++;
                                        index += 1;
                                    }
                                }
                                
                                completed = true;
                                console.AppendText(string.Format("{0} from {1}\r", item, ArahPanah.DiagonalToDownRight));

                            }
                        }
                        catch (Exception)
                        {
                            error++;
                            if (error <= columns)
                            {
                                col--;
                                row--;

                                if (row < 0 || col < 0)
                                {
                                    completed = true;
                                    Console.WriteLine("Error");
                                    completed = true;
                                }
                            }
                            else
                            {
                                NotAccepted.Add(item);
                                completed = true;
                            }

                        }
                    }

                }
                else if (lrc.FirstCount >= item.Length)
                {
                    bool completed = false;
                    int error = 0;
                    col = lrc.CollStart;
                    row = lrc.RowStart;
                    while (!completed)
                    {
                        try
                        {
                            var index = item.Length - 1;
                            int colTest = col;
                            int rowTest = row;
                            if (Helper.FindOnesFromLeftToRight(matrix, row, col).Number < item.Length)
                            {
                                throw new SystemException();
                            }
                            else
                            {
                                Dictionary<int, string> indexUseds = new Dictionary<int, string>();
                                foreach (ButtonView btn in canvas.Children)
                                {
                                    if (btn.Column == colTest && btn.Row == rowTest && index < item.Length)
                                    {
                                        if (btn.IsUsed)
                                        {
                                            if (item.Substring(index, 1).ToString() != btn.main.Content.ToString())
                                            {
                                                indexUseds.Add(index, btn.main.Content.ToString());
                                                throw new SystemException();
                                            }
                                        }
                                        rowTest++;
                                        colTest++;
                                        index--;
                                    }
                                }
                                index = item.Length - 1;
                                foreach (ButtonView btn in canvas.Children)
                                {
                                    if (btn.Column == col && btn.Row == row && index < item.Length)
                                    {
                                        btn.main.Content = item.Substring(index, 1);
                                        dataposition.Datas.Add(btn);
                                        SetObjectColorAndUsed(btn);
                                        row++;
                                        col++;
                                        index--;
                                    }
                                }
                                completed = true;
                                console.AppendText(string.Format("{0} from {1}\r", item, ArahPanah.DiagonalToUpLeft));
                            }

                        }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                        catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                        {
                            error++;
                            if (error <= rows-1)
                            {
                                col--;
                                row--;
                                if (row < 0 || col < 0)
                                {
                                    completed = true;
                                    NotAccepted.Add(item);
                                }
                            }
                            else
                            {
                                NotAccepted.Add(item);
                                completed = true;
                            }

                        }
                    }

                }
                else
                {
                    bool completed = false;
                    int error = 0;
                    col = lrc.CollStart;
                    row = lrc.RowStart;
                    while (!completed)
                    {
                        try
                        {
                            var index = 0;
                            int colTest = col;
                            int rowTest = row;
                            if (Helper.FindOnesFromLeftToRight(matrix, row, col).Number < item.Length)
                            {
                                throw new SystemException();
                            }
                            else
                            {
                                Dictionary<int, string> indexUseds = new Dictionary<int, string>();
                                foreach (ButtonView btn in canvas.Children)
                                {
                                    if (btn.Column == colTest && btn.Row == rowTest && index < item.Length)
                                    {
                                        if (btn.IsUsed)
                                        {
                                            if (item.Substring(index, 1).ToString() != btn.main.Content.ToString())
                                            {
                                                indexUseds.Add(index, btn.main.Content.ToString());
                                                throw new SystemException();
                                            }
                                        }
                                        rowTest++;
                                        colTest++;
                                        index += 1;
                                    }
                                }
                                index = 0;
                                foreach (ButtonView btn in canvas.Children)
                                {
                                    if (btn.Column == col && btn.Row == row && index < item.Length)
                                    {
                                        btn.main.Content = item.Substring(index, 1);
                                        dataposition.Datas.Add(btn);
                                        SetObjectColorAndUsed(btn);
                                        row++;
                                        col++;
                                        index += 1;
                                    }
                                }
                                completed = true;
                                console.AppendText(string.Format("{0} from {1}\r", item,"LR"));
                            }

                        }
                        catch (Exception)
                        {
                            error++;
                            if (error <= rows)
                            {
                                col--;
                                row--;
                                if (row < 0 || col < 0)
                                {
                                    NotAccepted.Add(item);
                                    completed = true;
                                }
                            }
                            else
                            {
                                NotAccepted.Add(item);
                                completed = true;
                            }



                        }
                    }


                }

            }
            else if (rlc.Number >= item.Length)
            {
                if (rlc.SecoundCount >= item.Length)
                {
                    bool completed = false;
                    int error = 0;
                    while (!completed)
                    {
                        try
                        {
                            var index = 0;
                            int colTest = col;
                            int rowTest = row;

                            if (Helper.FindOnesFromRightToLeft(matrix, row, col).Number < item.Length)
                            {
                                throw new SystemException("From RLC IF min lenght");
                            }
                            else
                            {
                                Dictionary<int, string> indexUseds = new Dictionary<int, string>();
                                foreach (ButtonView btn in canvas.Children)
                                {
                                    if (btn.Column == colTest && btn.Row == rowTest && index < item.Length)
                                    {
                                        if (btn.IsUsed)
                                        {
                                            if (item.Substring(index, 1).ToString() != btn.main.Content.ToString())
                                            {
                                                indexUseds.Add(index, btn.main.Content.ToString());
                                                throw new SystemException("From RLC  IF Using");
                                            }
                                        }
                                        rowTest++;
                                        colTest--;
                                        index += 1;
                                    }
                                }
                                index = 0;
                                foreach (ButtonView btn in canvas.Children)
                                {
                                    if (btn.Column == col && btn.Row == row && index < item.Length)
                                    {
                                        btn.main.Content = item.Substring(index, 1);
                                        dataposition.Datas.Add(btn);
                                        SetObjectColorAndUsed(btn);
                                        row++;
                                        col--;
                                        index += 1;
                                    }
                                }
                                completed = true;
                                console.AppendText(string.Format("{0} from {1}\r", item, ArahPanah.DiagonalToDownLeft));
                            }


                        }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                        catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                        {

                            error++;
                            if (error <= rows)
                            {
                                col++;
                                row--;
                                if (row < 0 || col < 0)
                                {
                                    NotAccepted.Add(item);
                                    completed = true;
                                }
                            }
                            else
                            {
                                NotAccepted.Add(item);
                                completed = true;
                            }
                        }
                    }

                }
                else if (rlc.FirstCount >= item.Length)
                {
                    bool completed = false;
                    int error = 0;
                    col = rlc.CollStart;
                    row = rlc.RowStart;
                    while (!completed)
                    {
                        try
                        {

                            var index = item.Length - 1;
                            int colTest = col;
                            int rowTest = row;
                            if (Helper.FindOnesFromRightToLeft(matrix, row, col).Number < item.Length)
                            {
                                throw new SystemException("From RLC ELSE IF min lenght");
                            }
                            else
                            {
                                Dictionary<int, string> indexUseds = new Dictionary<int, string>();
                                foreach (ButtonView btn in canvas.Children)
                                {
                                    if (btn.Column == colTest && btn.Row == rowTest && index < item.Length)
                                    {
                                        if (btn.IsUsed)
                                        {
                                            if (item.Substring(index, 1).ToString() != btn.main.Content.ToString())
                                            {
                                                indexUseds.Add(index, btn.main.Content.ToString());
                                                throw new SystemException("");
                                            }
                                        }
                                        rowTest--;
                                        colTest++;
                                        index--;
                                    }
                                }
                                index = item.Length - 1;
                                foreach (ButtonView btn in canvas.Children)
                                {
                                    if (btn.Column == col && btn.Row == row && index < item.Length)
                                    {
                                        btn.main.Content = item.Substring(index, 1);
                                        dataposition.Datas.Add(btn);
                                        SetObjectColorAndUsed(btn);
                                        row++;
                                        col--;
                                        index--;
                                    }
                                }
                                completed = true;
                                console.AppendText(string.Format("{0} from {1}\r", item, ArahPanah.DiagonalToUpRight));
                            }

                        }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                        catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                        {

                            error++;
                            if (error <= rows)
                            {
                                col++;
                                // row++;
                                if (row > rows || col > rows)
                                {
                                    NotAccepted.Add(item);
                                    completed = true;
                                }
                            }
                            else
                            {
                                NotAccepted.Add(item);
                                completed = true;
                            }
                        }
                    }

                }
                else
                {

                    bool completed = false;
                    int error = 0;
                    col = rlc.CollStart;
                    row = rlc.RowStart;
                    while (!completed)
                    {
                        try
                        {
                            var index = 0;
                            int colTest = col;
                            int rowTest = row;
                            if (Helper.FindOnesFromRightToLeft(matrix, row, col).Number < item.Length)
                            {
                                throw new SystemException("From RLC ELSE min lenght");
                            }
                            else
                            {
                                Dictionary<int, string> indexUseds = new Dictionary<int, string>();
                                foreach (ButtonView btn in canvas.Children)
                                {
                                    if (btn.Column == colTest && btn.Row == rowTest && index < item.Length)
                                    {
                                        if (btn.IsUsed)
                                        {
                                            if (item.Substring(index, 1).ToString() != btn.main.Content.ToString())
                                            {
                                                indexUseds.Add(index, btn.main.Content.ToString());
                                                throw new SystemException("From RLC ELSE Using");
                                            }
                                        }
                                        rowTest++;
                                        colTest--;
                                        index += 1;
                                    }
                                }
                                index = 0;
                                foreach (ButtonView btn in canvas.Children)
                                {
                                    if (btn.Column == col && btn.Row == row && index < item.Length)
                                    {
                                        btn.main.Content = item.Substring(index, 1);
                                        dataposition.Datas.Add(btn);
                                        SetObjectColorAndUsed(btn);
                                        row++;
                                        col--;
                                        index += 1;
                                    }
                                }
                                console.AppendText(string.Format("{0} from {1}\r", item,"Rl Normal"));
                                completed = true;
                            }

                        }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                        catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                        {
                            error++;
                            if (error <= rows)
                            {
                                col--;
                                row--;
                                if (row < 0 || col < 0)
                                {
                                    NotAccepted.Add(item);
                                    completed = true;
                                }
                            }
                            else
                            {
                                NotAccepted.Add(item);
                                completed = true;
                            }
                        }
                    }

                }
            }
            else
            {
                dataposition = null;
                NotAccepted.Add(item);
                console.AppendText(string.Format("{0} from {1} \r", item,"Normal Not Acceptable"));


            }
            return dataposition;

        }

        private DataPosition VerticalPlace(string item, int row, int col)
        {
            var dataposition = new DataPosition(item);
            if (row + item.Length <= rows)
            {
                bool completed = false;
                int error = 0;
                while (!completed)
                {
                    try
                    {
                        var index = 0;
                        int colTest = col;
                        int rowTest = row;
                        Dictionary<int, string> indexUseds = new Dictionary<int, string>();
                        foreach (ButtonView btn in canvas.Children)
                        {
                            if (btn.Column == colTest && btn.Row == rowTest && index < item.Length)
                            {
                                if (btn.IsUsed)
                                {
                                    if (item.Substring(index, 1).ToString() != btn.main.Content.ToString())
                                    {
                                        indexUseds.Add(index, btn.main.Content.ToString());
                                        throw new SystemException();
                                    }
                                }
                                rowTest += 1;
                                index += 1;
                            }
                        }
                        index = 0;
                        foreach (ButtonView btn in canvas.Children)
                        {
                            if (btn.Column == col && btn.Row == row && index < item.Length)
                            {
                                btn.main.Content = item.Substring(index, 1);
                                dataposition.Datas.Add(btn);
                                SetObjectColorAndUsed(btn);
                                row += 1;
                                index += 1;
                            }
                        }
                        completed = true;
                        console.AppendText(string.Format("{0} from {1}\r", item, ArahPanah.VerticalToBottom));

                    }
                    catch (Exception)
                    {
                        error++;
                        if (error <= rows)
                        {
                            if (col >= rows)
                                col = 0;
                            else
                                col++;
                        }
                        else
                        {
                            NotAccepted.Add(item);
                            completed = true;
                        }
                    }

                }


            }
            else if ((row+1) - item.Length >= 0)
            {
                bool completed = false;
                int error = 0;
                while (!completed)
                {
                    var index = item.Length - 1;
                    row = rows - item.Length;
                    try
                    {
                        int colTest = col;
                        int rowTest = row;
                        Dictionary<int, string> indexUseds = new Dictionary<int, string>();
                        foreach (ButtonView btn in canvas.Children)
                        {
                            if (btn.Column == colTest && btn.Row == rowTest && index < item.Length)
                            {
                                if (btn.IsUsed)
                                {
                                    if (item.Substring(index, 1).ToString() != btn.main.Content.ToString())
                                    {
                                        indexUseds.Add(index, btn.main.Content.ToString());
                                        throw new SystemException();
                                    }
                                }
                                rowTest += 1;
                                index -= 1;
                            }
                        }
                        index = item.Length - 1;
                        row = rows - item.Length;
                        foreach (ButtonView btn in canvas.Children)
                        {
                            if (btn.Column == col && btn.Row == row && index < item.Length)
                            {

                                if (index >= 0)
                                {
                                    btn.main.Content = item.Substring(index, 1);
                                    dataposition.Datas.Add(btn);
                                    SetObjectColorAndUsed(btn);
                                    row += 1;
                                    index -= 1;
                                }
                            }
                        }
                        completed = true;
                        console.AppendText(string.Format("{0} from {1}\r", item, ArahPanah.VerticalToTop));
                    }
                    catch (Exception)
                    {
                        error++;
                        if (error <= rows)
                        {
                            if (col >= columns-1)
                                col = 0;
                            else
                                col++;
                        }
                        else
                        {
                            NotAccepted.Add(item);
                            completed = true;
                        }
                    }

                }
            }
            else
            {
                row = 0;
                bool completed = false; ;
                int error = 0;
                dataposition.Datas.Clear();
                while (!completed)
                {
                    try
                    {
                        var index = 0;
                        int colTest = col;
                        int rowTest = row;
                        Dictionary<int, string> indexUseds = new Dictionary<int, string>();
                        foreach (ButtonView btn in canvas.Children)
                        {
                            if (btn.Column == colTest && btn.Row == rowTest && index < item.Length)
                            {
                                if (btn.IsUsed)
                                {
                                    if (item.Substring(index, 1).ToString() != btn.main.Content.ToString())
                                    {
                                        indexUseds.Add(index, btn.main.Content.ToString());
                                        throw new SystemException();
                                    }
                                }
                                rowTest += 1;
                                index += 1;
                            }
                        }
                        index = 0;
                        foreach (ButtonView btn in canvas.Children)
                        {
                            if (btn.Column == col && btn.Row == row && index < item.Length)
                            {

                                btn.main.Content = item.Substring(index, 1);
                                dataposition.Datas.Add(btn);
                                SetObjectColorAndUsed(btn);
                                row += 1;
                                index += 1;
                            }
                        }
                        completed = true;
                        console.AppendText(string.Format("{0} from {1}\r", item,"Vertical Normal"));
                    }
                    catch (Exception)
                    {
                        error++;
                        if (error <= columns-1)
                        {
                            if (col >= columns - 1)
                                col = 0;
                            else
                                col++;
                        }
                        else
                        {
                            NotAccepted.Add(item);
                            completed = true;
                        }

                    }

                }
            }
            return dataposition;
        }

        private DataPosition HorizontalPlace(string item, int row, int col)
        {
            var dataposition = new DataPosition(item);
            if (col + item.Length <= columns-1)
            {
                bool completed = false;
                int error = 0;
                while (!completed)
                {
                    try
                    {
                        var index = 0;
                        int colTest = col;
                        int rowTest = row;
                        Dictionary<int, string> indexUseds = new Dictionary<int, string>();
                        dataposition.Datas.Clear();
                        foreach (ButtonView btn in canvas.Children)
                        {
                            if (btn.Column == colTest && btn.Row == rowTest && index < item.Length)
                            {
                                if (btn.IsUsed)
                                {
                                    if (item.Substring(index, 1).ToString() != btn.main.Content.ToString())
                                    {
                                        indexUseds.Add(index, btn.main.Content.ToString());
                                        throw new SystemException();
                                    }

                                }
                                dataposition.Datas.Add(btn);
                                colTest += 1;
                                index += 1;
                            }
                        }

                        index = 0;
                        if(dataposition.Datas.Count>0)
                        {
                            foreach(var btn in dataposition.Datas)
                            {
                                btn.main.Content = item.Substring(index, 1);
                                SetObjectColorAndUsed(btn);
                                index++;
                            }
                            completed = true;
                            console.AppendText(string.Format("{0} from {1}\r",item,ArahPanah.HorizontalToRight));

                        }else
                        {
                            NotAccepted.Add(item);
                        }
                    }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                    catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                    {
                        error++;
                        if (error <= columns - 1)
                        {
                            if (row >= rows-1)
                                row = 0;
                            else
                                row++;
                        }
                        else
                        {
                            completed = true;
                            NotAccepted.Add(item);
                        }

                    }

                }


            }
            else if (col - item.Length >= 0)
            {
                var error = 0;
                bool completed = false; ;
                while (!completed)
                {
                    var index = item.Length - 1;
                    col = rows-1 - item.Length;
                    try
                    {
                        int colTest = col;
                        int rowTest = row;
                        Dictionary<int, string> indexUseds = new Dictionary<int, string>();
                        foreach (ButtonView btn in canvas.Children)
                        {
                            if (btn.Column == colTest && btn.Row == rowTest && index < item.Length)
                            {
                                if (btn.IsUsed)
                                {
                                    if (item.Substring(index, 1).ToString() != btn.main.Content.ToString())
                                    {
                                        indexUseds.Add(index, btn.main.Content.ToString());
                                        throw new SystemException();
                                    }
                                }
                                colTest += 1;
                                index -= 1;
                            }
                        }
                        index = item.Length - 1;
                        col = rows-1 - item.Length;
                        foreach (ButtonView btn in canvas.Children)
                        {
                            if (btn.Column == col && btn.Row == row && index < item.Length)
                            {

                                if (index >= 0)
                                {
                                    btn.main.Content = item.Substring(index, 1);
                                    dataposition.Datas.Add(btn);
                                    SetObjectColorAndUsed(btn);
                                    col += 1;
                                    index -= 1;
                                }
                            }
                        }
                        completed = true;
                        console.AppendText(string.Format("{0} from {1}\r", item, ArahPanah.HorizontalToLeft));
                    }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                    catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                    {
                        error++;
                        if (error <= rows-1)
                        {
                            if (row >= rows-1)
                                row = 0;
                            else
                                row++;
                        }
                        else
                        {
                            completed = true;
                            NotAccepted.Add(item);
                        }


                    }

                }
            }
            else
            {
                col = this.columns-item.Length;
                bool completed = false; ;
                var error = 0;

                while (!completed)
                {
                    try
                    {
                        var index = 0;
                        int colTest = col;
                        int rowTest = row;
                        Dictionary<int, string> indexUseds = new Dictionary<int, string>();
                        foreach (ButtonView btn in canvas.Children)
                        {
                            if (btn.Column == colTest && btn.Row == rowTest && index < item.Length)
                            {
                                if (btn.IsUsed)
                                {
                                    if (item.Substring(index, 1).ToString() != btn.main.Content.ToString())
                                    {
                                        indexUseds.Add(index, btn.main.Content.ToString());
                                        throw new SystemException();
                                    }


                                }
                                colTest += 1;
                                index += 1;
                            }
                        }
                        index = 0;
                        foreach (ButtonView btn in canvas.Children)
                        {
                            if (btn.Column == col && btn.Row == row && index < item.Length)
                            {

                                btn.main.Content = item.Substring(index, 1);
                                dataposition.Datas.Add(btn);
                                SetObjectColorAndUsed(btn);
                                col += 1;
                                index += 1;
                            }
                        }
                        completed = true;
                        console.AppendText(string.Format("{0} from {1}\r", item, "Horizontal Normal"));
                    }
                    catch (Exception)
                    {
                        error++;
                        if (error <= rows-1)
                        {
                            if (row >= rows-1)
                                row = 0;
                            else
                                row++;
                        }
                        else
                        {
                            completed = true;
                            NotAccepted.Add(item);
                        }
                    }

                }
            }
            return dataposition;

        }


        private void SetObjectColorAndUsed(ButtonView btn)
        {
            //btn.main.Background = CurrentColor;
            btn.IsUsed = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.RefreshBoard();   
        }

        private async Task SolutionWithBackTractAsync()
        {
            string findtext = "";
            BorderLabel bl = null;
            foreach(var item in dataToSearchView.Children)
            {
                 bl = (BorderLabel)item;
                if(bl!=null && !bl.IsFound)
                {
                    findtext = bl.Name;
                    break;
                }
            }


            Console.WriteLine(findtext);
            //Backtrack

            List<ButtonView> sb = new List<ButtonView>();
            int datake = 0;
            bool isComplete = false;
            var Datas = canvas.Children.OfType<ButtonView>();

            while (!isComplete)
            {
                for (var i = 0; i < rows; i++)
                {
                    for (var j = 0; j < columns; j++)
                    {
                      
                        datake = 0;
                        var startobj = this.GetButtonView(Datas, i, j);
                        await Task.Factory.StartNew(() => SetUsed(startobj, ArahPanah.HorizontalToRight));
                        if (findtext.Substring(datake, 1) == startobj.main.Content.ToString())
                        {
                            //FInd Horzontal To Right 
                            if (!isComplete)
                            {
                                if(columns-j>=findtext.Length)
                                {
                                    sb.Add(startobj);
                                    datake++;
                                    for (var n = j + 1; n < columns; n++)
                                    {
                                       var obj = GetButtonView(Datas, i, n);
                                        await Task.Factory.StartNew(() => SetUsed(obj, ArahPanah.HorizontalToRight));
                                        if (obj.main.Content.ToString() == findtext.Substring(datake, 1))
                                        {
                                            sb.Add(obj);
                                            datake++;
                                        }
                                        else
                                        {
                                            datake = 0;
                                            ClearSelectedBT(sb);
                                            ClearItemHelp(obj);
                                            break;

                                        }
                                        if (sb.Count == findtext.Length)
                                        {
                                            isComplete = true;
                                            SetFounded(sb);
                                            bl.Found();
                                            break;
                                        }
                                    }
                                }
                               
                            }

                            //FInd Horzontal To Left : "
                            if (!isComplete)
                            {
                                if (j+1-findtext.Length >= 0)
                                {
                                    await Task.Factory.StartNew(() => SetUsed(startobj, ArahPanah.HorizontalToRight));
                                    sb.Add(startobj);
                                    datake++;
                                    for (var n = j - 1; n >= (j+1-findtext.Length); n--)
                                    {
                                       var obj = GetButtonView(Datas, i, n);
                                        await Task.Factory.StartNew(() => SetUsed(obj, ArahPanah.HorizontalToLeft));
                                        if (obj.main.Content.ToString() == findtext.Substring(datake, 1))
                                        {
                                            sb.Add(obj);
                                            datake++;
                                        }
                                        else
                                        {
                                            datake = 0;
                                            
                                            ClearSelectedBT(sb);
                                            ClearItemHelp(obj);
                                            break;

                                        }
                                        if (sb.Count== findtext.Length)
                                        {
                                            Console.WriteLine("Found : " + sb.ToString());
                                            isComplete = true;
                                            SetFounded(sb);
                                            bl.Found();
                                            break;
                                        }
                                    }
                                }

                            }

                            //Vertical to Bottom
                            if (!isComplete)
                            {
                                if (rows - i >= findtext.Length)
                                {
                                    await Task.Factory.StartNew(() => SetUsed(startobj, ArahPanah.HorizontalToRight));
                                    sb.Add(startobj);
                                    datake++;
                                    for (var n = i + 1; n < rows; n++)
                                    {
                                       var obj = GetButtonView(Datas, n, j);
                                        await Task.Factory.StartNew(() => SetUsed(obj, ArahPanah.VerticalToBottom));
                                        if (obj.main.Content.ToString() == findtext.Substring(datake, 1))
                                        {
                                            sb.Add(obj);
                                            datake++;
                                        }
                                        else
                                        {
                                            datake = 0;
                                            ClearSelectedBT(sb);
                                            ClearItemHelp(obj);
                                            break;

                                        }
                                        if (sb.Count== findtext.Length)
                                        {
                                            isComplete = true;
                                            SetFounded(sb);
                                            bl.Found();
                                            break;
                                        }
                                    }
                                }

                            }

                            if (!isComplete)
                            {
                                //"FInd Vertical To TOP : ";
                                if (i - findtext.Length >= 0)
                                {
                                    await Task.Factory.StartNew(() => SetUsed(startobj, ArahPanah.HorizontalToRight));
                                    sb.Add(startobj);
                                    datake++;
                                    for (var n = i - 1; n > 0; n--)
                                    {
                                       var obj = GetButtonView(Datas, n, j);
                                        await Task.Factory.StartNew(() => SetUsed(obj, ArahPanah.VerticalToTop));
                                        if (obj.main.Content.ToString() == findtext.Substring(datake, 1))
                                        {
                                            sb.Add(obj);
                                            datake++;

                                        }
                                        else
                                        {
                                            datake = 0;
                                            ClearSelectedBT(sb);
                                            ClearItemHelp(obj);
                                            break;

                                        }
                                        if (sb.Count == findtext.Length)
                                        {
                                            isComplete = true;
                                            SetFounded(sb);
                                            bl.Found();
                                            break;
                                        }
                                    }
                                }

                            }

                            if (!isComplete)
                            {
                               // "Diagonal Left To Right To Bottom : ";
                                var l =Helper.FindOnesFromLeftToRight((new int[rows, columns]), i, j);
                                if (l.SecoundCount >= findtext.Length)
                                {
                                    await Task.Factory.StartNew(() => SetUsed(startobj, ArahPanah.HorizontalToRight));
                                    sb.Add(startobj);
                                    datake++;
                                    int m = j+1;
                                    for (var n = i + 1; n < rows; n++)
                                    {
                                      var  obj = GetButtonView(Datas, n, m);
                                        await Task.Factory.StartNew(() => SetUsed(obj, ArahPanah.DiagonalToDownRight));
                                        if (obj.main.Content.ToString()== findtext.Substring(datake, 1))
                                        {
                                            sb.Add(obj);
                                            datake++;

                                        }
                                        else
                                        {
                                            datake = 0;
                                            ClearSelectedBT(sb);
                                            ClearItemHelp(obj);
                                            break;

                                        }
                                        m++;

                                        if (sb.Count == findtext.Length)
                                        {
                                            isComplete = true;
                                            SetFounded(sb);
                                            bl.Found();
                                            break;
                                        }

                                    }
                                }

                            }

                            if (!isComplete)
                            {
                                datake = 0;
                              //  "Diagonal Left To Right To Up : ";
                                var l = Helper.FindOnesFromLeftToRight((new int[rows, columns]), i, j);
                                if (l.FirstCount >= findtext.Length)
                                {
                                    await Task.Factory.StartNew(() => SetUsed(startobj, ArahPanah.HorizontalToRight));
                                    sb.Add(startobj);
                                    datake++;
                                    int m = j - 1;
                                    for (var n = i - 1; n < rows; n--)
                                    {

                                       var obj = GetButtonView(Datas, n, m);
                                        await Task.Factory.StartNew(() => SetUsed(obj, ArahPanah.DiagonalToUpLeft));
                                        if (obj.main.Content.ToString() == findtext.Substring(datake, 1))
                                        {
                                            sb.Add(obj);
                                            datake++;

                                        }
                                        else
                                        {
                                            datake = 0;
                                            ClearSelectedBT(sb);
                                            ClearItemHelp(obj);
                                            break;

                                        }
                                        m--;

                                        if (sb.Count== findtext.Length)
                                        {
                                            isComplete = true;
                                            SetFounded(sb);
                                            bl.Found();
                                            break;
                                        }

                                    }
                                }

                            }


                            if (!isComplete)
                            {
                               // "Diagonal Right To Left To Bottom : ";
                                var l = Helper.FindOnesFromRightToLeft((new int[rows, columns]), i, j);
                                if (l.SecoundCount >= findtext.Length)
                                {
                                    await Task.Factory.StartNew(() => SetUsed(startobj, ArahPanah.HorizontalToRight));
                                    sb.Add(startobj);
                                    datake++;
                                    int m = j-1;
                                    for (var n = i + 1; n < rows; n++)
                                    {

                                       var obj = GetButtonView(Datas, n, m);
                                        await Task.Factory.StartNew(() => SetUsed(obj, ArahPanah.DiagonalToDownLeft));
                                        if (obj.main.Content.ToString()== findtext.Substring(datake, 1))
                                        {
                                            sb.Add(obj);
                                            datake++;

                                        }
                                        else
                                        {
                                            datake = 0;
                                            ClearSelectedBT(sb);
                                            ClearItemHelp(obj);
                                            break;

                                        }
                                        m--;

                                        if (sb.Count== findtext.Length)
                                        {
                                            isComplete = true;
                                            SetFounded(sb);
                                            bl.Found();
                                            break;
                                        }

                                    }
                                }

                            }

                            if (!isComplete)
                            {
                                //"Diagonal  Right to Left To Up : ";
                                var l = Helper.FindOnesFromRightToLeft((new int[rows, columns]), i, j);
                               
                                if (l.FirstCount >= findtext.Length)
                                {
                                    await Task.Factory.StartNew(() => SetUsed(startobj, ArahPanah.HorizontalToRight));
                                    sb.Add(startobj);
                                    datake++;
                                    int m = j + 1;

                                    for (var n = i - 1; n < rows; n--)
                                    {

                                       var obj = GetButtonView(Datas, n, m);
                                        await Task.Factory.StartNew(() => SetUsed(obj, ArahPanah.DiagonalToUpRight));
                                        if (obj.main.Content.ToString()== findtext.Substring(datake, 1))
                                        {
                                            sb.Add(obj);
                                            datake++;

                                        }
                                        else
                                        {
                                            datake = 0;
                                            ClearSelectedBT(sb);
                                            ClearItemHelp(obj);
                                            break;

                                        }
                                        m++;
                                        

                                        if (sb.Count == findtext.Length)
                                        {
                                            SetFounded(sb);
                                            bl.Found();
                                            isComplete = true;
                                            break;
                                        }

                                    }
                                }

                            }
                        }else
                        {
                            ClearItemHelp(startobj);
                        }

                        if (isComplete)
                            break;






                    }

                    if (isComplete)
                    {
                        break;
                    }
                    Console.WriteLine("New Row");

                }

                if (!isComplete)
                    isComplete = true;
                else
                {
                   
                }
            }
        }

        private void ClearItemHelp(ButtonView item)
        {
           
            item.LeftVisible = Visibility.Hidden;
            item.RightVisible = Visibility.Hidden;
            item.UpVisible = Visibility.Hidden;
            item.UpRightVisible = Visibility.Hidden;
            item.UpLeftVisible = Visibility.Hidden;
            item.DownVisible = Visibility.Hidden;
            item.DownLeftVisible = Visibility.Hidden;
            item.DownRightVisible = Visibility.Hidden;
            if (!item.IsFounded)
            {
                item.ClearUsed();
            }
        }

        private void SetFounded(List<ButtonView> sb)
        {
           foreach(var item in sb)
            {
                item.IsFounded = true;
            }
        }

        private void ClearSelectedBT(List<ButtonView> sb)
        {
          foreach(var item in sb)
            {
                item.LeftVisible = Visibility.Hidden;
                item.RightVisible = Visibility.Hidden;
                item.UpVisible = Visibility.Hidden;
                item.UpRightVisible = Visibility.Hidden;
                item.UpLeftVisible = Visibility.Hidden;
                item.DownVisible = Visibility.Hidden;
                item.DownLeftVisible = Visibility.Hidden;
                item.DownRightVisible = Visibility.Hidden;

                if (!item.IsFounded)
                {
                    item.ClearUsed();
                }
            }
            sb.Clear();
        }


        private void ClearSelectedKMP(List<ButtonView> sb)
        {
            foreach (var item in sb)
            {
                item.LeftVisible = Visibility.Hidden;
                item.RightVisible = Visibility.Hidden;
                item.UpVisible = Visibility.Hidden;
                item.UpRightVisible = Visibility.Hidden;
                item.UpLeftVisible = Visibility.Hidden;
                item.DownVisible = Visibility.Hidden;
                item.DownLeftVisible = Visibility.Hidden;
                item.DownRightVisible = Visibility.Hidden;

                if (!item.IsFounded)
                {
                    item.ClearUsed();
                }
            }
        }

        private void SetUsed(ButtonView item,ArahPanah arah)
        {
            item.SetIsUsed(CurrentColor);
            item.SetArah(arah);
            Thread.Sleep(this.SleepTime);
        }

        private ButtonView GetButtonView(IEnumerable<ButtonView> datas, int i, int j)
        {
            return datas.Where(O => O.Row == i && O.Column == j).FirstOrDefault();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            SolutionWithBackTractAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.SleepTime =Convert.ToInt32( ((Slider)sender).Value);
        }

        private void KMP_Click(object sender, RoutedEventArgs e)
        {
             SolutionWithKMAsync();
        }

        private async void SolutionWithKMAsync()
        {
            // Data Vertical Horizontal To Right
            console.Document.Blocks.Clear();
            string findtext = "";
            BorderLabel bl = null;
            foreach (var data in dataToSearchView.Children)
            {
                bl = (BorderLabel)data;
                if (bl != null && !bl.IsFound)
                {
                    findtext = bl.Name;
                    console.AppendText(string.Format("Data dicari adalah : '{0}' \r", findtext, Environment.NewLine));
                    break;
                }
            }

            var IsFounded = false;

            if(this.DataKMP==null)
            {
              DataKMP= GetDataKM(canvas);
            }

            foreach (var item in DataKMP)
            {

                 if (item.Key == ArahPanah.HorizontalToRight)
                {
                    console.AppendText(string.Format("Pencarian dimulai dari : '{0}' ke '{1}' \r", "Kiri", "Kanan", Environment.NewLine));
                    foreach(var data in item.Value)
                    {
                        var result = await KMPSearchAsync(findtext, data, ArahPanah.HorizontalToRight);
                        if (result.Item1)
                            break;
                    }
                    
                }

                if (!IsFounded && item.Key == ArahPanah.HorizontalToLeft)
                {
                    console.AppendText(string.Format("Pencarian dimulai dari : '{0}' ke '{1}' \r", "Kiri", "Kanan", Environment.NewLine));

                    var result =await KMPSearchAsync(findtext, item.Value[0],ArahPanah.HorizontalToLeft);
                    if (result.Item1)
                        break;
                }
                if (!IsFounded && item.Key == ArahPanah.VerticalToBottom)
                {
                    console.AppendText(string.Format("Pencarian dimulai dari : '{0}' ke '{1}' \r", "Atas", "Bawah", Environment.NewLine));
                    var result = await KMPSearchAsync(findtext, item.Value[0], ArahPanah.VerticalToBottom);
                    if (result.Item1)
                        break;

                }
                if (!IsFounded && item.Key == ArahPanah.VerticalToTop)
                {
                    console.AppendText(string.Format("Pencarian dimulai dari : '{0}' ke '{1}' \r", "Bawah", "Atas", Environment.NewLine));
                    var result = await KMPSearchAsync(findtext, item.Value[0],ArahPanah.VerticalToTop);
                    if (result.Item1)
                        break;

                }
                if (!IsFounded && item.Key == ArahPanah.DiagonalToDownRight)
                {
                   
                }
                if (!IsFounded && item.Key == ArahPanah.DiagonalToUpLeft)
                { }
                if (!IsFounded && item.Key == ArahPanah.DiagonalToDownLeft)
                { }
                if (!IsFounded && item.Key == ArahPanah.DiagonalToUpRight)
                {

                }

            }
        }


        private Dictionary<ArahPanah, List<DataPosition>> GetDataKM( Grid canvas)
        {
            Dictionary<ArahPanah, List<DataPosition>> list = new Dictionary<ArahPanah, List<DataPosition>>();
            List<DataPosition> DataHorizintalToRight = new List<DataPosition>();
            List<DataPosition> DataHorizintalToLeft = new List<DataPosition>();
            List<DataPosition> VerticalToBottom = new List<DataPosition>();
            List<DataPosition> VerticalToUp = new List<DataPosition>();
            List<DataPosition> DiagonalLRB = new List<DataPosition>();
            List<DataPosition> DiagonalLRU = new List<DataPosition>();
            List<DataPosition> DiagonalRLB = new List<DataPosition>();
            List<DataPosition> DiagonalRLU = new List<DataPosition>();

            for (var i = 0; i < rows; i++)
            {
                var sb = new DataPosition();
                var sb1 = new DataPosition();
                var vb = new DataPosition();
                var vt = new DataPosition();
                int m = columns - 1;
                for (var j = 0; j < columns; j++)
                {
                    sb.AddButton(GetButtonView(canvas.Children.OfType<ButtonView>(), i, j));
                    sb1.AddButton(GetButtonView(canvas.Children.OfType<ButtonView>(), i, m));
                    vb.AddButton(GetButtonView(canvas.Children.OfType<ButtonView>(), j, i));
                    vt.AddButton(GetButtonView(canvas.Children.OfType<ButtonView>(), m, i));
                    m--;
                }


                DataHorizintalToRight.Add(sb);
                DataHorizintalToLeft.Add(sb1);
                VerticalToBottom.Add(vb);
                VerticalToUp.Add(vt);

                Console.WriteLine();
            }

            //Diagobal
            for (var i = 0; i < columns; i++)
            {
                var drlb = new DataPosition();
                var drlb1 = new DataPosition();
                var drlu = new DataPosition();
                var drlu1 = new DataPosition();

                var c = 0;
                var z = columns - 1;

                for (var j = i; j >= 0 && c < columns; j--)
                {
                    drlb.AddButton(GetButtonView(canvas.Children.OfType<ButtonView>(), c, j));
                    drlu.AddButton(GetButtonView(canvas.Children.OfType<ButtonView>(), j, c));

                    c++;

                }

                c = columns - 1;

                for (var j = i; j < columns && c < columns; j++)
                {
                    drlb1.AddButton(GetButtonView(canvas.Children.OfType<ButtonView>(), j, c));
                    drlu1.AddButton(GetButtonView(canvas.Children.OfType<ButtonView>(), c, j));
                    c--;
                }


                DiagonalRLB.Add(drlb);
                DiagonalRLB.Add(drlb1);

                DiagonalRLU.Add(drlu);
                DiagonalRLU.Add(drlu1);



                //Left To Right

                var dlrb = new DataPosition() ;
                var dlrb1 = new DataPosition();
                c = i;
                for (var j = 0; j < columns && c < columns; j++)
                {
                    dlrb.AddButton(GetButtonView(canvas.Children.OfType<ButtonView>(), j, c));
                    dlrb1.AddButton(GetButtonView(canvas.Children.OfType<ButtonView>(), c, j));
                    c++;
                }


                var dlru = new DataPosition(); 
                var dlru1 = new DataPosition();
                c = columns - 1;
                for (var j = i; j >= 0; j--)
                {
                    dlru.AddButton(GetButtonView(canvas.Children.OfType<ButtonView>(), j, c));
                    dlru1.AddButton(GetButtonView(canvas.Children.OfType<ButtonView>(), c, j));
                    c--;
                }


                DiagonalLRB.Add(dlrb);
                DiagonalLRB.Add(dlrb1);

                DiagonalLRU.Add(dlru);
                DiagonalLRU.Add(dlru1);

            }

            list.Add(ArahPanah.DiagonalToDownLeft, DiagonalRLB);
            list.Add(ArahPanah.DiagonalToDownRight,DiagonalLRB);
            list.Add(ArahPanah.DiagonalToUpLeft, DiagonalLRU);
            list.Add(ArahPanah.DiagonalToUpRight, DiagonalRLU);
            list.Add(ArahPanah.HorizontalToLeft, DataHorizintalToLeft);
            list.Add(ArahPanah.HorizontalToRight, DataHorizintalToRight);
            list.Add(ArahPanah.VerticalToBottom, VerticalToBottom);
            list.Add(ArahPanah.VerticalToTop,VerticalToUp);
            return list;

        }


        //KM Prccess
        private async Task<Tuple<bool, int>> KMPSearchAsync(string pat, DataPosition dp,ArahPanah arah)
        {
            string txt = dp.Content;
            int M = pat.Length;
            int N = txt.Length;
            var found = false;
            int index=0;
            

            // create lps[] that will hold the longest
            // prefix suffix values for pattern
            int[] lps = new int[M];
            int j = 0;  // index for pat[]

            // Preprocess the pattern (calculate lps[]
            // array)
            lps = PreKMP(pat);

            int i = 0;  // index for txt[]
            while (i < N)
            {
                var p = pat.Substring(j, 1);
                var b = txt.Substring(i, 1);
                if (pat.Substring(j, 1) == txt.Substring(i, 1))
                {
                    var item = dp.Datas[i];
                   await Task.Factory.StartNew(()=> SetUsedKM(item,Brushes.Green, arah));
                    j++;
                    i++;
                }

                if (j == M)
                {
                    index = (i - j);
                    console.AppendText(string.Format("Found : '{0}' On Index '{1}' \r ", pat, index));
                    j = lps[j - 1];
                    found = true;
                    break;
                   
                }

                // mismatch after j matches
                else if (i < N && pat.Substring(j, 1) != txt.Substring(i, 1))
                {
                    // Do not match lps[0..lps[j-1]] characters,
                    // they will match anyway
                    var item = dp.Datas[i];
                    await Task.Factory.StartNew(() => SetUsedKM(item, Brushes.Red, ArahPanah.None));
                    if (j != 0)
                    {
                        j = lps[j - 1];
                        if(j>0)
                        {
                            this.ClearSelectedKMP(dp.Datas);
                            var z = i - j;
                            for(var l =0;l<j;l++)
                            {
                                item = dp.Datas[z];
                                await Task.Factory.StartNew(() => SetUsedKM(item, Brushes.Coral, arah));
                                z++;
                            }
                        }



                        item = dp.Datas[i];
                        await Task.Factory.StartNew(() => SetUsedKM(item, Brushes.Yellow, arah));
                    }
                    else
                    {
                        i = i + 1;
                        if (i < N)
                        {


                            item = dp.Datas[i];
                            await Task.Factory.StartNew(() => SetUsedKM(item, Brushes.Yellow, ArahPanah.None));
                        }
                        this.ClearSelectedKMP(dp.Datas);
                    }
                   

                   

                }else if(i<N)
                {
                    var item = dp.Datas[i];
                    await Task.Factory.StartNew(() => SetUsedKM(item, Brushes.Green, arah));
                }else
                {
                    this.ClearSelectedKMP(dp.Datas);
                }
            }
            return Tuple.Create(found, index);
        }


        private async Task<object> KMPHorizontalToLeftAsync(string findtext, DataPosition dataPosition)
        {
            string txt = dataPosition.Content;
            string pat = findtext;
            DataPosition dp = dataPosition;
            int M = findtext.Length;
            int N = txt.Length;
            var found = false;
            int index = 0;


            // create lps[] that will hold the longest
            // prefix suffix values for pattern
            int[] lps = new int[M];
            int j = 0;  // index for pat[]

            // Preprocess the pattern (calculate lps[]
            // array)
            lps = PreKMP(pat);

            int i = 0;  // index for txt[]
            while (i < N)
            {
                var p = pat.Substring(j, 1);
                var b = txt.Substring(i, 1);
                if (pat.Substring(j, 1) == txt.Substring(i, 1))
                {
                    var item = dp.Datas[i];
                    await Task.Factory.StartNew(() => SetUsedKM(item, Brushes.Green, ArahPanah.HorizontalToRight));
                    j++;
                    i++;
                }

                if (j == M)
                {
                    index = (i - j);
                    console.AppendText(string.Format("Found : '{0}' On Index '{1}' \r ", pat, index));
                    j = lps[j - 1];
                    found = true;
                    break;

                }

                // mismatch after j matches
                else if (i < N && pat.Substring(j, 1) != txt.Substring(i, 1))
                {
                    // Do not match lps[0..lps[j-1]] characters,
                    // they will match anyway
                    var item = dp.Datas[i];
                    await Task.Factory.StartNew(() => SetUsedKM(item, Brushes.Red, ArahPanah.None));
                    if (j != 0)
                    {
                        j = lps[j - 1];
                        if (j > 0)
                        {
                            this.ClearSelectedKMP(dp.Datas);
                            var z = i - j;
                            for (var l = 0; l < j; l++)
                            {
                                item = dp.Datas[z];
                                await Task.Factory.StartNew(() => SetUsedKM(item, Brushes.Coral, ArahPanah.None));
                                z++;
                            }
                        }



                        item = dp.Datas[i];
                        await Task.Factory.StartNew(() => SetUsedKM(item, Brushes.Yellow, ArahPanah.None));
                    }
                    else
                    {
                        i = i + 1;
                        item = dp.Datas[i];
                        await Task.Factory.StartNew(() => SetUsedKM(item, Brushes.Yellow, ArahPanah.None));
                        this.ClearSelectedKMP(dp.Datas);
                    }




                }
                else
                {
                    var item = dp.Datas[i];
                    await Task.Factory.StartNew(() => SetUsedKM(item, Brushes.Green, ArahPanah.HorizontalToRight));
                }
            }
            return Tuple.Create(found, index);
        }

        private  int[] PreKMP(string w)
        {
            List<int> p = new List<int>();
            p.Add(0);
            int j = 0;
            for (int i = 1; i < w.Length; i++)
            {
                while (j > 0 && w[j] != w[i])
                    j = p[j - 1];

                if (w[j] == w[i])
                    j++;
                p.Add(j);
            }
            var sb = new StringBuilder();
            p.ForEach(n => sb.Append(string.Format("{0}\t", n.ToString())));
            console.AppendText(string.Format("Prefix : '{0}' \r ", sb.ToString()));
            return p.ToArray();
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            main.Show();
            this.Close();
        }

        //KMP SetUsed
        private void SetUsedKM(ButtonView item,Brush currentColor, ArahPanah arah)
        {
            item.SetIsUsed(currentColor);
            item.SetArah(arah);
            Thread.Sleep(this.SleepTime);
        }
    }
}
