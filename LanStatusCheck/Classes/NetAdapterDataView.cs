using LanStatusCheck.Helpers;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace LanStatusCheck.Classes
{
    public class NetAdapterDataView : NaviItemBase
    {
        #region local variable

        private readonly int _minSpeed = 60;

        private double _upSpeed;

        private double _downSpeed;

        private double _minTimeForChart;

        private double _maxTimeForChart;

        private bool _isUpTextOnAnatation = false;

        private double _maxSpeedForChart = 60;

        private int _tickMajorStepGridLineChart = 30;

        private double _maxSpeedInterfaceDelEmission = 30;

        private Func<double, string> _formatter = (a) => ConvertData(a);

        private int _loadOnInterUp;

        private int _maxCountNodeInChartMin = 50;

        private int _loadOnInterDown;

        private long _totalTransmiteData;

        private long _totalRecivedData;

        private NetworkInterfaceData _dataInterfaceModel;



        #endregion

        #region Property

        #region Prop data interface

        public string NameInter { get { return DataInterfaceModel.Interface.Name; } }

        public string DescInter { get { return DataInterfaceModel.Interface.Description; } }

        public NetworkInterfaceData DataInterfaceModel
        {
            get { return _dataInterfaceModel; }
            set
            {
                _dataInterfaceModel = value;

                _dataInterfaceModel.UpdateData += () => SetParamData();

                SetIdInterface(_dataInterfaceModel.Interface.Id);
            }
        }

        public double UpSpeed
        {
            get { return _upSpeed; }
            set
            {
                _upSpeed = value;

                OnPropertyChanged();
            }
        }

        public double DownSpeed
        {
            get { return _downSpeed; }
            set
            {
                _downSpeed = value;

                OnPropertyChanged();
            }
        }



        public int LoadOnInterUp
        {
            get { return _loadOnInterUp; }
            set
            {
                _loadOnInterUp = value;

                OnPropertyChanged();
            }
        }



        public int LoadOnInterDown
        {
            get { return _loadOnInterDown; }
            set
            {
                _loadOnInterDown = value;

                OnPropertyChanged();
            }
        }

        public long TotalRecivedData
        {
            get { return _totalRecivedData; }
            set
            {
                _totalRecivedData = value;

                OnPropertyChanged();
            }
        }

        public long TotalTransmiteData
        {
            get { return _totalTransmiteData; }
            set
            {
                _totalTransmiteData = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region Prop For Charts

        public double MinTimeForChart
        {
            get { return _minTimeForChart; }
            set
            {
                _minTimeForChart = value;

                OnPropertyChanged();
            }
        }

        public double MaxTimeForChart
        {
            get { return _maxTimeForChart; }
            set
            {
                _maxTimeForChart = value;

                OnPropertyChanged();
            }
        }

        public double MaxSpeedForChart
        {
            get { return _maxSpeedForChart; }
            set
            {
                _maxSpeedForChart = value;

                OnPropertyChanged();
            }
        }

        public Func<double, string> Formatter
        {
            get { return _formatter; }
            set
            {
                _formatter = value;

                OnPropertyChanged();
            }
        }

        public int TickMajorStepGridLineChart
        {
            get { return _tickMajorStepGridLineChart; }
            set
            {
                _tickMajorStepGridLineChart = value;

                OnPropertyChanged();
            }
        }

        public double MaxSpeedInterfaceDelEmission
        {
            get { return _maxSpeedInterfaceDelEmission; }
            set
            {
                _maxSpeedInterfaceDelEmission = value;

                OnPropertyChanged();
            }
        }

        public bool IsUpTextOnAnatation
        {
            get { return _isUpTextOnAnatation; }
            set
            {
                _isUpTextOnAnatation = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region prop from navi panel

        private bool _isOpen = false;

        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                _isOpen = value;

                Debug.WriteLine(value);
            }
        }


        #endregion

        #endregion

        #region collections

        public ObservableCollection<NodeActiveNetInterface> ActivityDataForChart { get; set; }


        #endregion

        #region ctors

        public NetAdapterDataView() : base()
        {

            ActivityDataForChart = new ObservableCollection<NodeActiveNetInterface>();
        }

        public NetAdapterDataView(NetAdapterDataView data) : base()
        {
            ActivityDataForChart = new ObservableCollection<NodeActiveNetInterface>();

            DataInterfaceModel = data.DataInterfaceModel;

            UpSpeed = data.UpSpeed;

            DownSpeed = data.DownSpeed;

            TotalRecivedData = data.TotalRecivedData;

            TotalTransmiteData = data.TotalTransmiteData;

            SetIdInterface(data.DataInterfaceModel.Interface.Id);


        }

        #endregion

        #region public methods 

        public void SetParamData()
        {
            var currentData = _dataInterfaceModel.HistoryDataActivity.Last();

            //Debug.WriteLineIf(NameInter.IndexOf("Локалка") != -1, currentData.ToString());

            LoadOnInterUp = currentData.LoadPerSecUp;

            LoadOnInterDown = currentData.LoadPerSecDown;

            UpSpeed = currentData.UpSpeedKBitSec;

            DownSpeed = currentData.DownSpeedKBitSec;

            TotalRecivedData = currentData.TotalRecivedBytes;

            TotalTransmiteData = currentData.TotalTransmiteBytes;

            if (ActivityDataForChart.Count > _maxCountNodeInChartMin)
            {
                ActivityDataForChart.RemoveAt(0);
            }

            ActivityDataForChart.Add(currentData);

            //Создание эффекта заполнения справо налево

            if (ActivityDataForChart.Count < _maxCountNodeInChartMin)
            {
                var tmpMinTime = ActivityDataForChart[0].Time.Add(-( new TimeSpan(0, 0, _maxCountNodeInChartMin - ActivityDataForChart.Count) ));

                MinTimeForChart = DateTimeAxis.ToDouble(tmpMinTime);
            }
            else
            {
                MinTimeForChart = DateTimeAxis.ToDouble(ActivityDataForChart.First().Time);
            }


            MaxTimeForChart = DateTimeAxis.ToDouble(ActivityDataForChart.Last().Time);

            MaxSpeedForChart = GetMaxSpeedForChart(ActivityDataForChart);

            TickMajorStepGridLineChart = Convert.ToInt32(MaxSpeedForChart / 3);

            if (ActivityDataForChart.Count < HelpersDataTransform.MinNodeInSequence)
            {
                return;
            }

            MaxSpeedInterfaceDelEmission = GetMaxSpeedForChartDelEmissions(ActivityDataForChart);

            IsUpTextOnAnatation = ( ( MaxSpeedForChart - MaxSpeedInterfaceDelEmission ) > ( ( MaxSpeedForChart * 25 ) / 100 ) );
        }




        #endregion

        #region private methods

        private static string ConvertData(double val)
        {
            var gbS = val / 1024;

            return gbS < 1 ? String.Format("{0} Kb\\s", val) : String.Format("{0:F2} Mb\\s", gbS);
        }



        private double GetMaxSpeedForChartDelEmissions(IEnumerable<NodeActiveNetInterface> interActivity)
        {
            var maxDownSpeed = HelpersDataTransform.DeleteEmissinsFromSequence(interActivity.Select(a => a.DownSpeedKBitSec).ToList()).Max();

            var maxUpSpeed = HelpersDataTransform.DeleteEmissinsFromSequence(interActivity.Select(a => a.UpSpeedKBitSec).ToList()).Max();

            var max = Math.Max(maxDownSpeed, maxUpSpeed);

            return max;
        }

        private double GetMaxSpeedForChart(IEnumerable<NodeActiveNetInterface> interActivity)
        {
            var maxDownSpeed = interActivity.Select(a => a.DownSpeedKBitSec).ToList().Max();

            var maxUpSpeed = interActivity.Select(a => a.UpSpeedKBitSec).ToList().Max();

            var max = Math.Max(maxDownSpeed, maxUpSpeed);

            var maxWith10Per = ( max * 15 ) / 100 + max;

            //eturn max > _minSpeed ? max : _minSpeed;
            return maxWith10Per > _minSpeed ? maxWith10Per : _minSpeed;

        }

        #endregion

    }
}
