using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
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

      //  private static readonly DependencyPropertyKey ArrayStateSegmentsPropertyKey
      //= DependencyProperty.RegisterReadOnly("ArrayStateSegments", typeof(bool[]), typeof(TimerIndicator),
      //    new FrameworkPropertyMetadata(new bool[18],
      //        FrameworkPropertyMetadataOptions.None));

      //  public static readonly DependencyProperty ArrayStateSegmentsProperty
      //      = ArrayStateSegmentsPropertyKey.DependencyProperty;

      //  public bool[] ArrayStateSegments
      //  {
      //      get { return (bool[])GetValue(ArrayStateSegmentsProperty); }
      //      protected set { SetValue(ArrayStateSegmentsPropertyKey, value); }
      //  }

        private static readonly DependencyPropertyKey ArrayStateSegmentsPropertyKey
      = DependencyProperty.RegisterReadOnly("ArrayStateSegments", typeof(ActiveSegment[]), typeof(TimerIndicator),
          new FrameworkPropertyMetadata(new ActiveSegment[18],
              FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty ArrayStateSegmentsProperty
            = ArrayStateSegmentsPropertyKey.DependencyProperty;

        public ActiveSegment[] ArrayStateSegments
        {
            get { return (ActiveSegment[])GetValue(ArrayStateSegmentsProperty); }
            protected set { SetValue(ArrayStateSegmentsPropertyKey, value); }
        }

        #region Colors

        private static readonly DependencyPropertyKey ActiveBrushPropertyKey
      = DependencyProperty.RegisterReadOnly("ActiveBrush", typeof(Brush), typeof(TimerIndicator),
          new FrameworkPropertyMetadata(new SolidColorBrush(),
              FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty ActiveBrushProperty
            = ActiveBrushPropertyKey.DependencyProperty;

        public Brush ActiveBrush
        {
            get { return (Brush)GetValue(ActiveBrushProperty); }
            protected set { SetValue(ActiveBrushPropertyKey, value); }
        }


        private static readonly DependencyPropertyKey InactiveBrushPropertyKey
      = DependencyProperty.RegisterReadOnly("InactiveBrush", typeof(Brush), typeof(TimerIndicator),
          new FrameworkPropertyMetadata(new SolidColorBrush(),
              FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty InactiveBrushProperty
            = InactiveBrushPropertyKey.DependencyProperty;

        public Brush InactiveBrush
        {
            get { return (Brush)GetValue(InactiveBrushProperty); }
            protected set { SetValue(InactiveBrushPropertyKey, value); }
        }


        #endregion

        #region Current Timers Value

        private static readonly DependencyPropertyKey CurrentTimerValuePropertyKey
      = DependencyProperty.RegisterReadOnly("CurrentTimerValue", typeof(TimeSpan), typeof(TimerIndicator),
          new FrameworkPropertyMetadata(new TimeSpan(0,0,0),
              FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty CurrentTimerValueProperty
            = CurrentTimerValuePropertyKey.DependencyProperty;

        public TimeSpan CurrentTimerValue
        {
            get { return (TimeSpan)GetValue(CurrentTimerValueProperty); }
            protected set { SetValue(CurrentTimerValuePropertyKey, value); }
        }

        #endregion

        #endregion

        #region Dep Property public

        #region Colors

        /// <summary>Цвет заполнения активных индикатров </summary>
        public Color IndicatorActiveColor
        {
            get { return (Color)GetValue(IndicatorActiveColorProperty); }
            set { SetValue(IndicatorActiveColorProperty, value); }
        }

        public static readonly DependencyProperty IndicatorActiveColorProperty =
            DependencyProperty.Register("IndicatorActiveColor", typeof(Color), typeof(TimerIndicator), new PropertyMetadata(Colors.Blue, ChangeIndicatorActiveColor));

        private static void ChangeIndicatorActiveColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TimerIndicator)d;

            control.ActiveBrush = new SolidColorBrush((Color)e.NewValue);

        }


        /// <summary>Цвет заполнения неактивных индикатров</summary>
        public Color IndicatorInactiveColor
        {
            get { return (Color)GetValue(IndicatorInactiveColorProperty); }
            set { SetValue(IndicatorInactiveColorProperty, value); }
        }

        public static readonly DependencyProperty IndicatorInactiveColorProperty =
            DependencyProperty.Register("IndicatorInactiveColor", typeof(Color), typeof(TimerIndicator), new PropertyMetadata(Colors.Gray, ChangeIndicatorInactiveColor));

        private static void ChangeIndicatorInactiveColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TimerIndicator)d;

            control.InactiveBrush = new SolidColorBrush((Color)e.NewValue);
        }


        #endregion

        #region Times

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(TimerIndicator), new PropertyMetadata(new TimeSpan(0,0,0), DurationPropChangeCallback), DutationValidate);

        private static bool DutationValidate(object value)
        {
            var val = (TimeSpan)value;

            return val.TotalMinutes <= 99;
        }

        private static void DurationPropChangeCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TimerIndicator)d;

            control._timer.Stop();

            control.CurrentTimerValue = (TimeSpan)e.NewValue;
        }

        #endregion

        #endregion

        public TimerIndicator()
        {
            ActiveBrush = new SolidColorBrush(IndicatorActiveColor);

            InactiveBrush = new SolidColorBrush(IndicatorInactiveColor);

            InitializeComponent();



            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 1)
            };

            _timer.Tick += _timer_Tick;

            Duration = new TimeSpan(0, 3, 0);

            _timer.Start();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            //bool[] tmp = new bool[18];
            //ArrayStateSegments.CopyTo(tmp, 0);

            //tmp[_currentIndex] = false;

            //if (_currentIndex == tmp.Length - 1)
            //    _currentIndex = -1;

            //_currentIndex++;

            //tmp[_currentIndex] = true;

            var tmpVal = CurrentTimerValue - new TimeSpan(0, 0, 1);

            if (tmpVal >= new TimeSpan(0, 0, 0))
            {
                CurrentTimerValue = tmpVal;

                var tmp = GetNewArray(CurrentTimerValue, Duration, ArrayStateSegments.Length);

                ArrayStateSegments = tmp;

            } else
                Stop();
        }

        //private bool[] GetNewArray(TimeSpan currentValue, TimeSpan duration, int countElem)
        //{
        //    if (countElem == 0) throw new ArgumentException("Число эллементов не может быть равно 0");
                 
        //    bool[] resArray = new bool[countElem];

        //    var valueOneElem = 100.0 / countElem;

        //    var percent = ((int)currentValue.TotalSeconds * 100.0) / (int)duration.TotalSeconds;

        //    var countFullElem = Math.Ceiling(percent / valueOneElem);

        //    for (int i = 0; i < countFullElem; i++)
        //    {
        //        resArray[i] = true;
        //    }

        //    return resArray;
        //}

        private ActiveSegment[] GetNewArray(TimeSpan currentValue, TimeSpan duration, int countElem)
        {
            if (countElem == 0) throw new ArgumentException("Число эллементов не может быть равно 0");

            ActiveSegment[] resArray = new ActiveSegment[countElem];

            //resArray = Enumerable.Repeat(new ActiveSegment(), countElem).ToArray();

            for (int i = 0; i < countElem; i++)
            {
                resArray[i] = new ActiveSegment();
            }

            var valueOneElem = 100.0 / countElem;

            var percent = ((int)currentValue.TotalSeconds * 100.0) / (int)duration.TotalSeconds;

            var countFullElem = percent / valueOneElem;

            for (int i = 0; i < (int)countFullElem; i++)
            {
                resArray[i] = new ActiveSegment() { Value = 1 };
            }

            if(countFullElem-(int)countFullElem!=0)
            {
                resArray[(int)countFullElem].Value = countFullElem - (int)countFullElem;
            }

            return resArray;
        }

        #region public methods

        public void Stop()
        {
            if (_timer == null) return;

            _timer.Stop();
        }

        public void RunTimer()
        {
            if (_timer == null) return;

            if (Duration == TimeSpan.MinValue)
            {
                throw new ArgumentException("Для запуска необходимо установить начальное время отсчёта (Duration)");
            }

            _timer.Start();
        }

        public void Reset()
        {
            if (_timer == null) return;

            _timer.Stop();

            CurrentTimerValue = Duration;

        }

        #endregion

        ~TimerIndicator()
        {
            _isDisp = true;
            _timer.Stop();
            _timer.Tick -= _timer_Tick;
            _timer = null;
        }
    }

    public class BoolToBrushConverter :FrameworkElement, IValueConverter
    {
        public Color ColorOff
        {
            get { return (Color)GetValue(ColorOffProperty); }
            set { SetValue(ColorOffProperty, value); }
        }

        public static readonly DependencyProperty ColorOffProperty =
            DependencyProperty.Register("ColorOff", typeof(Color), typeof(BoolToBrushConverter));

        public Color ColorOn
        {
            get { return (Color)GetValue(ColorOnProperty); }
            set { SetValue(ColorOnProperty, value); }
        }

        public static readonly DependencyProperty ColorOnProperty =
            DependencyProperty.Register("ColorOn", typeof(Color), typeof(BoolToBrushConverter));


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (!(value is bool)) return Binding.DoNothing;

            //var val = (bool)value;


            if (!(value is ActiveSegment)) return Binding.DoNothing;

            var val = (ActiveSegment)value;

            var strAngle = (string)parameter;

            var angle = System.Convert.ToDouble(strAngle);

            

            if (val.Value == 0) return new SolidColorBrush(ColorOff);

            if (val.Value == 1) return new SolidColorBrush( ColorOn);

            LinearGradientBrush gb = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 0),

                EndPoint = new Point(0.5, 1),
            };

            gb.RelativeTransform = new RotateTransform(angle-90, 0.5, 0.5);

            gb.GradientStops.Add(new GradientStop(ColorOn, val.Value));
            gb.GradientStops.Add(new GradientStop(ColorOff, val.Value));

            

            return gb;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DurationToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan)) return Binding.DoNothing;

            var val = (TimeSpan)value;

            return string.Format("{0:D2}:{1:D2}", (int)val.TotalMinutes, val.Seconds);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ActiveSegment 
    {
        public double Value { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
