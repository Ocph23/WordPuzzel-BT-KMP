using GameWordPuzzel.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public List<string> ListResult { get;  set; }
        private List<string> NotAccepted = new List<string>();

        private Random random = new Random();
       private  Random rrow = new Random();
        private Random rcolumn = new Random();
        private Random rarah = new Random();

        private void GameBoard_Loaded(object sender, RoutedEventArgs e)
        {
            this.RefreshBoard();
        }

        private void RefreshBoard()
        {
            var columns = 10;
            var rows = 10;
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

            var datas = new List<string>();
            ListResult = new List<string>();
            dataToSearchView.Children.Clear();
            canvas.Children.Clear();
            canvas.ColumnDefinitions.Clear();
            canvas.RowDefinitions.Clear();
            foreach (var item in this.DataSource)
            {
                datas.Add(item);
                dataToSearchView.Children.Add(new BorderLabel(item));
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
                    var button = new ButtonView { FontSize = 20, Row = i, Column = j, Name = string.Format("R{0}C{1}", i, j) };
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
                        var row = Helper.RandomPosisionRow(rrow);
                        var col = Helper.RandomPosisionColumn(rcolumn);
                        var dataposition = new DataPosition(item);



                        if (arah == Arah.Horizontal)
                        {
                            DataPosition data = HorizontalPlace(item, row, col);
                        }
                        else if (arah == Arah.Vertical)
                        {
                            DataPosition data = VerticalPlace(item, row, col);

                        }
                        else if (arah == Arah.Diagonal)
                        {
                            DataPosition data = DiagonalPlace(item, row, col);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new SystemException(ex.Message);
                    }


                }

                if (NotAccepted.Count == 0)
                {
                    RandomCompleted = true;
                }
                else
                {
                    datas.Clear();
                    foreach (var item in NotAccepted)
                    {
                        datas.Add(item);
                    }
                    NotAccepted.Clear();
                }
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
                grid.Children.Add(Helper.NewBorder(CurrentColor));

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

                        ListResult.Add(sb.ToString());
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
            Grid grid = (Grid)btn.main.Parent;
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
                      
                        grid.Children.Add(Helper.NewBorder(CurrentColor));
                        this.ListSelected.Add(btn);
                    }
                    else if (!lastBtn.IsStarter && ListSelected.Count > 1)
                    {
                        var starter = ListSelected.Where(O => O.IsStarter).FirstOrDefault();

                        if (starter.Row==btn.Row && lastBtn.Column-starter.Column>=0 && btn.Column-lastBtn.Column==1)
                        {
                            //Horizontal Left To Right
                            grid.Children.Add(Helper.NewBorder(CurrentColor));
                            this.ListSelected.Add(btn);
                        }
                        else if (starter.Row == btn.Row && lastBtn.Column - starter.Column < 0 && lastBtn.Column- btn.Column  == 1)
                        {
                            //Horizontal Right To Left
                            grid.Children.Add(Helper.NewBorder(CurrentColor));
                            this.ListSelected.Add(btn);
                        }
                        else if (starter.Column == btn.Column && lastBtn.Row - starter.Row >= 0 && btn.Row - lastBtn.Row == 1)
                        {
                            //Verical Top To Bottom
                            grid.Children.Add(Helper.NewBorder(CurrentColor));
                            this.ListSelected.Add(btn);
                        }
                        else if (starter.Column == btn.Column && lastBtn.Row - starter.Row < 0 && lastBtn.Row- btn.Row  == 1)
                        {
                            //Vertical Bottom To Top
                            grid.Children.Add(Helper.NewBorder(CurrentColor));
                            this.ListSelected.Add(btn);
                        }
                        else if (starter.Column < lastBtn.Column && starter.Row<lastBtn.Row && btn.Column-lastBtn.Column==1 && btn.Row-lastBtn.Row==1)
                        {
                            //Diagonal Left To Right To Bottom
                            grid.Children.Add(Helper.NewBorder(CurrentColor));
                            this.ListSelected.Add(btn);
                        }
                        else if (starter.Column > lastBtn.Column && starter.Row > lastBtn.Row && btn.Column - lastBtn.Column < 0 && btn.Row - lastBtn.Row <0)
                        {
                            //Diagonal Left To Right To Top
                            grid.Children.Add(Helper.NewBorder(CurrentColor));
                            this.ListSelected.Add(btn);
                        }
                        else if (starter.Column > lastBtn.Column && starter.Row < lastBtn.Row && btn.Column - lastBtn.Column <0 && btn.Row - lastBtn.Row == 1)
                        {
                            //Diagonal Right To Left To Bottom
                            grid.Children.Add(Helper.NewBorder(CurrentColor));
                            this.ListSelected.Add(btn);
                        }
                        else if (starter.Column < lastBtn.Column && starter.Row > lastBtn.Row && btn.Column - lastBtn.Column ==1 && btn.Row - lastBtn.Row < 0)
                        {
                            //Diagonal Right To left Top
                            grid.Children.Add(Helper.NewBorder(CurrentColor));
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
            const int rows = 10;
            const int columns = 10;
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
                            if (error <= 9)
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
                            if (error <= 9)
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
                            if (error <= 9)
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
                            if (error <= 9)
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
                            if (error <= 9)
                            {
                                col++;
                                // row++;
                                if (row > 9 || col > 9)
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
                            if (error <= 9)
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
                Debug.WriteLine("{0} Not Implement Diagonal", item);

            }
            return dataposition;

        }

        private DataPosition VerticalPlace(string item, int row, int col)
        {
            var dataposition = new DataPosition(item);
            if (row + item.Length <= 9)
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
                        if (error <= 9)
                        {
                            if (col >= 9)
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
                    row = 9 - item.Length;
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
                        row = 9 - item.Length;
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
                        if (error <= 9)
                        {
                            if (col >= 9)
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
                    catch (Exception ex)
                    {
                        error++;
                        if (error <= 9)
                        {
                            if (col >= 9)
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
            if (col + item.Length <= 9)
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
                        if (error <= 9)
                        {
                            if (row >= 9)
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
                    col = 9 - item.Length;
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
                        col = 9 - item.Length;
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
                        if (error <= 9)
                        {
                            if (row >= 9)
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
                col = 9 - item.Length;
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
                    catch (Exception ex)
                    {
                        error++;
                        if (error <= 9)
                        {
                            if (row >= 9)
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
           // btn.main.Background = CurrentColor;
            btn.IsUsed = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.RefreshBoard();   
        }
    }
}
