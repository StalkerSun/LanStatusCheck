using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LanStatusCheck.Components
{
    /// <summary>
    /// Interaction logic for TimerIndicator.xaml
    /// </summary>
    public partial class TimerIndicator : UserControl
    {
        #region Dep Property



        public StateSegment[] ArrayStateSegments
        {
            get { return (StateSegment[])GetValue(ArrayStateSegmentsProperty); }
            set { SetValue(ArrayStateSegmentsProperty, value); }
        }

        public static readonly DependencyProperty ArrayStateSegmentsProperty =
            DependencyProperty.Register("ArrayStateSegments", typeof(StateSegment[]), typeof(TimerIndicator), new FrameworkPropertyMetadata(new StateSegment[18]));


        private Timer _timer;

        private int _currentIndex = 0;



        #endregion

        public TimerIndicator()
        {
            InitializeComponent();

            _timer = new Timer(1);

            _timer.Elapsed += _timer_Elapsed;

            _timer.Start();


        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                StateSegment[] tmp = new StateSegment[18];
                ArrayStateSegments.CopyTo(tmp, 0);

                tmp[_currentIndex].State = false;

                if (_currentIndex == tmp.Length - 1)
                    _currentIndex = -1;

                _currentIndex++;

                tmp[_currentIndex].State = true;
                SetValue(ArrayStateSegmentsProperty, tmp);
            }
            );


            


        }

        public override void BeginInit()
        {
            base.BeginInit();

            for (int i = 0; i < ArrayStateSegments.Length; i++)
            {
                ArrayStateSegments[i] = new StateSegment() { State = false };
            }


            //ArrayStateSegments[0] = true;
            //ArrayStateSegments[2] = true;
            //ArrayStateSegments[4] = true;
            //ArrayStateSegments[6] = true;
            //ArrayStateSegments[8] = true;
        }
    }

    public class BoolToBrushConverter : IValueConverter
    {
        public Brush ColorOff { get; set; }

        public Brush ColorOn { get; set; }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is StateSegment)) return Binding.DoNothing;

            var val = (StateSegment)value;

            return val.State ? ColorOn : ColorOff;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StateSegment : INotifyPropertyChanged
    {
        private bool _state;

        public bool State
        {
            get { return _state; }
            set
            {
                _state = value;

                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return State.ToString();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
