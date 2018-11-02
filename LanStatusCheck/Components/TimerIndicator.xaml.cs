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
using System.Windows.Threading;

namespace LanStatusCheck.Components
{
    /// <summary>
    /// Interaction logic for TimerIndicator.xaml
    /// </summary>
    public partial class TimerIndicator : UserControl
    {
        #region local variable

        private DispatcherTimer _timer;

        private int _currentIndex = 0;

        private bool _isDisp = false;


        #endregion


        #region Dep Property local

        private static readonly DependencyPropertyKey ArrayStateSegmentsPropertyKey
      = DependencyProperty.RegisterReadOnly("ArrayStateSegments", typeof(bool[]), typeof(UserControl),
          new FrameworkPropertyMetadata(new bool[18],
              FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty ArrayStateSegmentsProperty
            = ArrayStateSegmentsPropertyKey.DependencyProperty;

        public bool[] ArrayStateSegments
        {
            get { return (bool[])GetValue(ArrayStateSegmentsProperty); }
            protected set { SetValue(ArrayStateSegmentsPropertyKey, value); }
        }

        #endregion

        public TimerIndicator()
        {

            InitializeComponent();

            _timer = new DispatcherTimer();

            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            _timer.Tick += _timer_Tick;

            _timer.Start();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
                    bool[] tmp = new bool[18];
                    ArrayStateSegments.CopyTo(tmp, 0);

                    tmp[_currentIndex] = false;

                    if (_currentIndex == tmp.Length - 1)
                        _currentIndex = -1;

                    _currentIndex++;

                    tmp[_currentIndex] = true;

                    ArrayStateSegments = tmp;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            if(!_isDisp)
            {
                Dispatcher.Invoke(() =>
                {
                    bool[] tmp = new bool[18];
                    ArrayStateSegments.CopyTo(tmp, 0);

                    tmp[_currentIndex] = false;

                    if (_currentIndex == tmp.Length - 1)
                        _currentIndex = -1;

                    _currentIndex++;

                    tmp[_currentIndex] = true;

                    ArrayStateSegments = tmp;
                });
            }
        }

        ~TimerIndicator()
        {
            _isDisp = true;
            _timer.Stop();
            _timer.Tick -= _timer_Tick;
            _timer = null;
        }
    }

    public class BoolToBrushConverter : IValueConverter
    {
        public Brush ColorOff { get; set; }

        public Brush ColorOn { get; set; }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool)) return Binding.DoNothing;

            var val = (bool)value;

            return val ? ColorOn : ColorOff;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
}
