using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
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

        private static readonly DependencyPropertyKey ArrayStateSegmentsPropertyKey
      = DependencyProperty.RegisterReadOnly("ArrayStateSegments", typeof(ActiveSegment[]), typeof(TimerIndicator),
          new FrameworkPropertyMetadata(new ActiveSegment[18],
              FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty ArrayStateSegmentsProperty
            = ArrayStateSegmentsPropertyKey.DependencyProperty;

        public ActiveSegment[] ArrayStateSegments
        {
            get { return ( ActiveSegment[] ) GetValue(ArrayStateSegmentsProperty); }
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
            get { return ( Brush ) GetValue(ActiveBrushProperty); }
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
            get { return ( Brush ) GetValue(InactiveBrushProperty); }
            protected set { SetValue(InactiveBrushPropertyKey, value); }
        }


        #endregion

        #region Current Timers Value

        private static readonly DependencyPropertyKey CurrentTimerValuePropertyKey
      = DependencyProperty.RegisterReadOnly("CurrentTimerValue", typeof(TimeSpan), typeof(TimerIndicator),
          new FrameworkPropertyMetadata(new TimeSpan(0, 0, 0),
              FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty CurrentTimerValueProperty
            = CurrentTimerValuePropertyKey.DependencyProperty;

        public TimeSpan CurrentTimerValue
        {
            get { return ( TimeSpan ) GetValue(CurrentTimerValueProperty); }
            protected set { SetValue(CurrentTimerValuePropertyKey, value); }
        }

        #endregion

        #endregion

        #region Dep Property public

        #region Colors

        /// <summary>Цвет заполнения активных индикатров </summary>
        public Color IndicatorActiveColor
        {
            get { return ( Color ) GetValue(IndicatorActiveColorProperty); }
            set { SetValue(IndicatorActiveColorProperty, value); }
        }

        public static readonly DependencyProperty IndicatorActiveColorProperty =
            DependencyProperty.Register("IndicatorActiveColor", typeof(Color), typeof(TimerIndicator), new PropertyMetadata(Colors.Blue, ChangeIndicatorActiveColor));

        private static void ChangeIndicatorActiveColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = ( TimerIndicator ) d;

            control.ActiveBrush = new SolidColorBrush(( Color ) e.NewValue);

        }


        /// <summary>Цвет заполнения неактивных индикатров</summary>
        public Color IndicatorInactiveColor
        {
            get { return ( Color ) GetValue(IndicatorInactiveColorProperty); }
            set { SetValue(IndicatorInactiveColorProperty, value); }
        }

        public static readonly DependencyProperty IndicatorInactiveColorProperty =
            DependencyProperty.Register("IndicatorInactiveColor", typeof(Color), typeof(TimerIndicator), new PropertyMetadata(Colors.Gray, ChangeIndicatorInactiveColor));

        private static void ChangeIndicatorInactiveColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = ( TimerIndicator ) d;

            control.InactiveBrush = new SolidColorBrush(( Color ) e.NewValue);
        }


        #endregion

        #region TypeViewIndicator

        public TypeViewDataIndicator TypeViewDataIndicator
        {
            get { return ( TypeViewDataIndicator ) GetValue(TypeViewDataIndicatorProperty); }
            set { SetValue(TypeViewDataIndicatorProperty, value); }
        }

        public static readonly DependencyProperty TypeViewDataIndicatorProperty =
            DependencyProperty.Register("TypeViewDataIndicator", typeof(TypeViewDataIndicator), typeof(TimerIndicator), new PropertyMetadata(0));

        #endregion


        #region Times

        public TimeSpan Duration
        {
            get { return ( TimeSpan ) GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(TimerIndicator), new PropertyMetadata(new TimeSpan(0, 0, 0), DurationPropChangeCallback), DutationValidate);

        private static bool DutationValidate(object value)
        {
            var val = ( TimeSpan ) value;

            return val.TotalMinutes <= 99;
        }

        private static void DurationPropChangeCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = ( TimerIndicator ) d;

            control._timer.Stop();

            control.CurrentTimerValue = ( TimeSpan ) e.NewValue;
        }



        public double PercentValue
        {
            get { return ( double ) GetValue(PercentValueProperty); }
            set { SetValue(PercentValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Percent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PercentValueProperty =
            DependencyProperty.Register("PercentValue", typeof(double), typeof(TimerIndicator), new PropertyMetadata(0.0), PercentValueValidate);

        private static bool PercentValueValidate(object value)
        {
            var val = ( double ) value;

            return ( ( val >= 0 ) && ( val <= 100 ) );
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

            Duration = new TimeSpan(0, 1, 0);

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

            }
            else
            {
                Stop();
            }
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
            if (countElem == 0)
            {
                throw new ArgumentException("Число эллементов не может быть равно 0");
            }

            ActiveSegment[] resArray = new ActiveSegment[countElem];

            //resArray = Enumerable.Repeat(new ActiveSegment(), countElem).ToArray();

            for (int i = 0; i < countElem; i++)
            {
                resArray[i] = new ActiveSegment();
            }

            var valueOneElem = 100.0 / countElem;

            var percent = ( ( int ) currentValue.TotalSeconds * 100.0 ) / ( int ) duration.TotalSeconds;

            var countFullElem = percent / valueOneElem;

            for (int i = 0; i < ( int ) countFullElem; i++)
            {
                resArray[i] = new ActiveSegment() { Value = 1 };
            }

            if (countFullElem - ( int ) countFullElem != 0)
            {
                resArray[( int ) countFullElem].Value = countFullElem - ( int ) countFullElem;
            }

            return resArray;
        }

        #region public methods

        public void Stop()
        {
            if (_timer == null)
            {
                return;
            }

            _timer.Stop();
        }

        public void RunTimer()
        {
            if (_timer == null)
            {
                return;
            }

            if (Duration == TimeSpan.MinValue)
            {
                throw new ArgumentException("Для запуска необходимо установить начальное время отсчёта (Duration)");
            }

            _timer.Start();
        }

        public void Reset()
        {
            if (_timer == null)
            {
                return;
            }

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

    public class BoolToBrushConverter : FrameworkElement, IValueConverter
    {

        public Color ColorOff
        {
            get { return ( Color ) GetValue(ColorOffProperty); }
            set { SetValue(ColorOffProperty, value); }
        }

        public static readonly DependencyProperty ColorOffProperty =
            DependencyProperty.Register("ColorOff", typeof(Color), typeof(BoolToBrushConverter));

        public Color ColorOn
        {
            get { return ( Color ) GetValue(ColorOnProperty); }
            set { SetValue(ColorOnProperty, value); }
        }

        public static readonly DependencyProperty ColorOnProperty =
            DependencyProperty.Register("ColorOn", typeof(Color), typeof(BoolToBrushConverter));

        private readonly Dictionary<string, Point[]> _paramGradientForSegment = new Dictionary<string, Point[]>
        {
            {"1", new Point[]{new Point(0.028, 0.363), new Point(0.968, 0.636)} },
            {"2", new Point[]{new Point(0.065, 0.175), new Point(0.939, 0.819)} },
            {"3", new Point[]{new Point(0.13, 0.104), new Point(0.863, 0.905)} },
            {"4", new Point[]{new Point(0.226, 0.058), new Point(0.763, 0.95)} },
            {"5", new Point[]{new Point(0.5, 0.0), new Point(0.5, 1)} },
            {"6", new Point[]{new Point(0.726, 0.036), new Point(0.251, 0.952)} },
            {"7", new Point[]{new Point(0.854, 0.083), new Point(0.121, 0.893)} },
            {"8", new Point[]{new Point(0.94, 0.166), new Point(0.056, 0.822)} },
            {"9", new Point[]{new Point(0.985, 0.35), new Point(0.013, 0.631)} },
            {"10", new Point[]{new Point(0.992, 0.653), new Point(0.005, 0.364)} },
            {"11", new Point[]{new Point(0.939, 0.821), new Point(0.065, 0.173)} },
            {"12", new Point[]{new Point(0.877, 0.905), new Point(0.123, 0.082)} },
            {"13", new Point[]{new Point(0.77, 0.962), new Point(0.254, 0.026)} },
            {"14", new Point[]{new Point(0.5, 1), new Point(0.5, 0)} },
            {"15", new Point[]{new Point(0.246, 0.973), new Point(0.734, 0.033)} },
            {"16", new Point[]{new Point(0.115, 0.909), new Point(0.891, 0.092)} },
            {"17", new Point[]{new Point(0.05, 0.803), new Point(0.944, 0.169)} },
            {"18", new Point[]{ new Point(0.03, 0.641), new Point(0.979, 0.360)} }
        };


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (!(value is bool)) return Binding.DoNothing;

            //var val = (bool)value;


            if (!( value is ActiveSegment ))
            {
                return Binding.DoNothing;
            }

            var val = ( ActiveSegment ) value;

            if (val.Value == 0)
            {
                return new SolidColorBrush(ColorOff);
            }

            if (val.Value == 1)
            {
                return new SolidColorBrush(ColorOn);
            }

            var numberSegment = ( string ) parameter;

            var points = _paramGradientForSegment[numberSegment];

            LinearGradientBrush gb = new LinearGradientBrush
            {
                StartPoint = points[0],

                EndPoint = points[1]
            };
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
            if (!( value is TimeSpan ))
            {
                return Binding.DoNothing;
            }

            var val = ( TimeSpan ) value;

            return string.Format("{0:D2}:{1:D2}", ( int ) val.TotalMinutes, val.Seconds);
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

    public enum TypeViewDataIndicator
    {
        Timer,
        Percent
    }


}
