using GameWordPuzzel.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    public partial class GameBoard : Window
    {
        public GameBoard()
        {
            InitializeComponent();
            this.Loaded += GameBoard_Loaded;
        }

        public List<string> DataSource { get; private set; }
        public bool Start { get; private set; }
        public List<ButtonView> ListSelected { get; private set; }
        public Brush CurrentColor { get; private set; }
       // public List<string> ListResult { get;  set; }
        private List<string> NotAccepted = new List<string>();

        private Random random = new Random();
       private  Random rrow = new Random();
        private Random rarah = new Random();
        private int columns;
        private int rows;

        private void GameBoard_Loaded(object sender, RoutedEventArgs e)
        {
            this.RefreshBoard();
        }

        private void RefreshBoard()
        {
            var x =10;
            this.columns = x;
            this.rows = x;
            DataSource = new List<string>();
            DataSource.Add("MAKANAN");
            DataSource.Add("MINUM");
            DataSource.Add("TIDUR");
            DataSource.Add("KERJASAMA");
            DataSource.Add("JALAN");
            DataSource.Add("WISATA");
            DataSource.Add("QUIZ");
            DataSource.Add("SERAKAH");
            DataSource.Add("KEHIDUPAN");
            DataSource.Add("MUSIK");
            DataSource.Add("BIOLA");

            var datas = new List<string>();
            //ListResult = new List<string>();
            dataToSearchView.Children.Clear();
            canvas.Children.Clear();
            canvas.ColumnDefinitions.Clear();
            canvas.RowDefinitions.Clear();
            foreach (var item in this.DataSource)
            {
                datas.Add(item);
     
            }

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
                    var button = new ButtonView(new Cordinate(this.columns,this.rows)) { FontSize = 30-x, Row = i, Column = j, Name = string.Format("R{0}C{1}", i, j) };
                    button.main.Content = Helper.RandomString(random);
                    button.MouseEnter += Button_MouseEnter; ;
                    button.main.Click += Main_Click;
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    this.canvas.Children.Add(button);
                }
            }

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
                        var locals = item.GetLocalValueEnumerator();
                        while (locals.MoveNext())
                        {
                            var a = locals.Current.Property;
                        }
                        //   item.Background.ClearValue(item.Background);
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
                                Debug.WriteLine(string.Format("{0} From LRC IF in col {1} and row {2}", item, col, row));
                                completed = true;

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
                                Debug.WriteLine(string.Format("{0} From LRC ELSE IF in col {1} and row {2}", item, col, row));
                            }

                        }
                        catch (Exception ex)
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
                                Debug.WriteLine(string.Format("{0} From LRC ELSE in col {1} and row {2}", item, col, row));
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
                                Debug.WriteLine(string.Format("{0} From RLC IF in col {1} and row {2}", item, col, row));
                            }


                        }
                        catch (Exception ex)
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
                                Debug.WriteLine(string.Format("{0} From RLC Else IF in col {1} and row {2}", item, col, row));
                            }

                        }
                        catch (Exception ex)
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
                                Debug.WriteLine(string.Format("{0} From RLC Else in col {1} and row {2}", item, col, row));
                                completed = true;
                            }

                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(string.Format("{0} From RLC Else", ex.Message));
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
                NotAccepted.Add(item);
               // Debug.WriteLine("{0} Not Implement Diagonal", item);
               

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
            else if (row - item.Length >= 0)
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
                        }else
                        {
                            NotAccepted.Add(item);
                        }
                    }
                    catch (Exception ex)
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
                    }
                    catch (Exception ex)
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
                            NotAccepted.Add(item);
                        }
                    }

                }
            }
            return dataposition;

        }


        private void SetObjectColorAndUsed(ButtonView btn)
        {
          //  btn.main.Background = CurrentColor;
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
                                            ClearSelectedHelp(sb);
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
                                            
                                            ClearSelectedHelp(sb);
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
                                            ClearSelectedHelp(sb);
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
                                            ClearSelectedHelp(sb);
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
                                            ClearSelectedHelp(sb);
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
                                if (l.FirstCount > findtext.Length)
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
                                            ClearSelectedHelp(sb);
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
                                if (l.SecoundCount > findtext.Length)
                                {
                                    await Task.Factory.StartNew(() => SetUsed(startobj, ArahPanah.HorizontalToRight));
                                    sb.Add(startobj);
                                    datake++;
                                    int m = j;
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
                                            ClearSelectedHelp(sb);
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
                               
                                if (l.FirstCount > findtext.Length)
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
                                            ClearSelectedHelp(sb);
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

        private void ClearSelectedHelp(List<ButtonView> sb)
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

        private void SetUsed(ButtonView item,ArahPanah arah)
        {
            item.SetArah(arah);
            item.SetIsUsed(CurrentColor);
            Thread.Sleep(500);

        }

        private ButtonView GetButtonView(IEnumerable<ButtonView> datas, int i, int j)
        {
            return datas.Where(O => O.Row == i && O.Column == j).FirstOrDefault();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SolutionWithBackTractAsync();
        }
    }
}
