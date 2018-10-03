﻿using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LanStatusCheck.Classes
{
    public class NetAdapterDataView : INotifyPropertyChanged
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

        private int _loadOnInterface;

        private int _maxCountNodeInChartMin = 50;



        #endregion

        #region Property

        public string NameInter { get { return DataInterface.Interface.Name; } }

        public string DescInter { get { return DataInterface.Interface.Description; } }

        public NetworkInterfaceData DataInterface { get; set; }

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

        

        public int LoadOnInterface
        {
            get { return _loadOnInterface; }
            set
            {
                _loadOnInterface = value;

                OnPropertyChanged();
            }
        }


        //public int LoadOnInterface { get; set; }


        public double MinTimeForChart
        {
            get { return _minTimeForChart; }
            set
            {
                _minTimeForChart = value;

                OnPropertyChanged();
            }
        }

        public double  MaxTimeForChart
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

        #region collections

        public ObservableCollection<NodeActiveNetInterface> ActivityDataForChart{ get; set; }


        #endregion

        #region ctors

        public NetAdapterDataView()
        {

            ActivityDataForChart = new ObservableCollection<NodeActiveNetInterface>();


        }

        public NetAdapterDataView(NetAdapterDataView data)
        {
            ActivityDataForChart = new ObservableCollection<NodeActiveNetInterface>();

            UpSpeed = data.UpSpeed;

            DownSpeed = data.DownSpeed;

            DataInterface = data.DataInterface;
        }

        #endregion

        #region public methods 

        public void SetParamData(double upSpeed, double downSpeed)
        {
            UpSpeed = upSpeed;

            DownSpeed = downSpeed;

            if (ActivityDataForChart.Count > _maxCountNodeInChartMin)
                ActivityDataForChart.RemoveAt(0);

            LoadOnInterface = GetPercentLoadOnInterface(upSpeed, downSpeed, DataInterface.Interface.Speed / 1024.0);

            ActivityDataForChart.Add(new NodeActiveNetInterface { DownSpeed = downSpeed, UpSpeed = upSpeed, Time = DateTime.Now, LoadPerInSec = LoadOnInterface });

            if (ActivityDataForChart.Count < _maxCountNodeInChartMin)
            {
                var tmpMinTime = ActivityDataForChart[0].Time.Add(-(new TimeSpan(0, 0, _maxCountNodeInChartMin - ActivityDataForChart.Count)));

                MinTimeForChart = DateTimeAxis.ToDouble(tmpMinTime);
            }
            else
            {
                MinTimeForChart = DateTimeAxis.ToDouble(ActivityDataForChart.First().Time);
            }


            MaxTimeForChart = DateTimeAxis.ToDouble(ActivityDataForChart.Last().Time);

            if (ActivityDataForChart.Count < HelpersDataTransform.MinNodeInSequence) return;

            MaxSpeedForChart = GetMaxSpeedForChart(ActivityDataForChart);

            MaxSpeedInterfaceDelEmission = GetMaxSpeedForChartDelEmissions(ActivityDataForChart);

            IsUpTextOnAnatation = ((MaxSpeedForChart - MaxSpeedInterfaceDelEmission) > ((MaxSpeedForChart * 25) / 100));

            TickMajorStepGridLineChart = Convert.ToInt32(MaxSpeedForChart / 3);
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
            var maxDownSpeed = HelpersDataTransform.DeleteEmissinsFromSequence(interActivity.Select(a => a.DownSpeed).ToList()).Max();

            var maxUpSpeed = HelpersDataTransform.DeleteEmissinsFromSequence(interActivity.Select(a => a.UpSpeed).ToList()).Max();

            var max = Math.Max(maxDownSpeed, maxUpSpeed);

            Debug.WriteLine(max);

            return max;
        }

        private double GetMaxSpeedForChart(IEnumerable<NodeActiveNetInterface> interActivity)
        {
            var maxDownSpeed = interActivity.Select(a => a.DownSpeed).ToList().Max();

            var maxUpSpeed = interActivity.Select(a => a.UpSpeed).ToList().Max();

            var max = Math.Max(maxDownSpeed, maxUpSpeed);

            var maxWith10Per = (max * 15) / 100 + max;

            //eturn max > _minSpeed ? max : _minSpeed;
            return maxWith10Per > _minSpeed ? maxWith10Per : _minSpeed;

        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
