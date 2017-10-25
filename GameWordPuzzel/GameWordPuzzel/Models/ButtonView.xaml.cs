using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace GameWordPuzzel.Models
{
    /// <summary>
    /// Interaction logic for ButtonView.xaml
    /// </summary>
    public partial class ButtonView : UserControl,INotifyPropertyChanged
    {
        private Visibility _left;
        private Visibility _right;
        private Visibility _up;
        private Visibility _upright;
        private Visibility _upleft;
        private Visibility _down;
        private Visibility _downleft;
        private Visibility _downright;
        private Thickness _borderSize;
        private Brush _borderColor= Brushes.Transparent;
        private bool _isUsed;

        public ButtonView(Cordinate maxrowcol)
        {
            InitializeComponent();
            this.MaxRowCol = maxrowcol;
            this.DataContext = this;
            LeftVisible = Visibility.Hidden;
            RightVisible = Visibility.Hidden;
            UpVisible = Visibility.Hidden;
            UpRightVisible = Visibility.Hidden;
            UpLeftVisible = Visibility.Hidden;
            DownVisible = Visibility.Hidden;
            DownLeftVisible = Visibility.Hidden;
            DownRightVisible = Visibility.Hidden;
        }

        public int Row { get; internal set; }
        public int Column { get; internal set; }
        public bool IsStarter { get; internal set; }

        public Thickness BorderSize
        {
            get { return _borderSize; }
            set
            {
                if(_borderSize!=value)
                {
                    _borderSize = value;
                    OnNotifyPropertyChanged();
                }
            }
        }


        public Brush BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                if(_borderColor!=value)
                {
                    _borderColor = value;
                    OnNotifyPropertyChanged();
                }
            }
        }

        public Visibility LeftVisible
        {
            get { return _left; }
            set
            {
                if (_left != value)
                {
                    if (value == Visibility.Visible && this.Column !=0)
                    {
                        value = Visibility.Hidden;
                    }
                    _left = value;
                    OnNotifyPropertyChanged();
                }
            }
        }

        public Visibility RightVisible
        {
            get { return _right; }
            set
            {
                if (_right!= value)
                {
                    if(value== Visibility.Visible && this.Column==MaxRowCol.Column-1)
                    {
                        value = Visibility.Hidden;
                    }
                    _right = value;
                    OnNotifyPropertyChanged();
                }
            }
        }

        public Visibility UpVisible
        {
            get { return _up; }
            set
            {
                if (_up != value)
                {
                    _up = value;
                    OnNotifyPropertyChanged();
                }
            }
        }

        public Visibility UpRightVisible
        {
            get { return _upright; }
            set
            {
                if (_upright != value)
                {
                    _upright = value;
                    OnNotifyPropertyChanged();
                }
            }
        }

        public Visibility UpLeftVisible
        {
            get { return _upleft; }
            set
            {
                if (_upleft!= value)
                {
                    _upleft= value;
                    OnNotifyPropertyChanged();
                }
            }
        }

        public Visibility DownVisible
        {
            get { return _down; }
            set
            {
                if (_down != value)
                {
                    _down = value;
                    OnNotifyPropertyChanged();
                }
            }
        }

        public Visibility DownLeftVisible
        {
            get { return _downleft; }
            set
            {
                if (_downleft != value)
                {
                    _downleft = value;
                    OnNotifyPropertyChanged();
                }
            }
        }

        public Visibility DownRightVisible
        {
            get { return _downright; }
            set
            {
                if (_downright != value)
                {
                    _downright = value;
                    OnNotifyPropertyChanged();
                }
            }
        }

        public bool IsUsed {
            get
            {
                return _isUsed;
            }
            set
            {
                _isUsed = value;
                OnNotifyPropertyChanged();
            }
        }

        public bool IsFounded { get;  set; }
        public Cordinate MaxRowCol { get; private set; }

        public void SetIsUsed(Brush currentColor)
        {
            this.BorderSize= new Thickness(10);
            this.BorderColor = currentColor;

        }

        internal void ShowLeft()
        {
            LeftVisible = Visibility.Visible;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnNotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal void ClearUsed()
        {
            this.BorderSize = new Thickness(0);
            this.BorderColor = Brushes.Transparent;
        }

        internal void SetArah(ArahPanah arah)
        {
            switch (arah)
            {

                case ArahPanah.HorizontalToRight:
                    RightVisible = Visibility.Visible;
                    break;
                case ArahPanah.HorizontalToLeft:
                    LeftVisible = Visibility.Visible;
                    break;
                case ArahPanah.VerticalToTop:
                    UpVisible = Visibility.Visible;
                    break;
                case ArahPanah.VerticalToBottom:
                    DownVisible = Visibility.Visible;
                    break;
                case ArahPanah.DiagonalToDownRight:
                    DownRightVisible = Visibility.Visible;
                    break;
                case ArahPanah.DiagonalToDownLeft:
                    DownLeftVisible = Visibility.Visible;
                    break;
                case ArahPanah.DiagonalToUpRight:
                    UpRightVisible = Visibility.Visible;
                    break;
                case ArahPanah.DiagonalToUpLeft:
                    UpLeftVisible = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
    }
}
