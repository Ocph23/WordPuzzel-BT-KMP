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

        public ButtonView()
        {
            InitializeComponent();
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
        public Visibility LeftVisible
        {
            get { return _left; }
            set
            {
                if (_left != value)
                {
                    _left = value;
                    NotifyPropertyChanged();
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
                    _right= value;
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
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
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsUsed { get; internal set; }

        

        public void GoUp()
        {
            UpVisible = Visibility.Visible;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
        }

        internal void ShowLeft()
        {
            LeftVisible = Visibility.Visible;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }




    }
}
