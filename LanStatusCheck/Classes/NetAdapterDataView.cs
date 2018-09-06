using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LanStatusCheck.Classes
{
    public class NetAdapterDataView : INotifyPropertyChanged
    {
        #region local variable

        private string _nameNetInter;

        private double _upSpeed;

        private double _downSpeed;

        private int _maxCountNodeInChartMin = 50;

        #endregion
        
        #region Property

        public string NameInter
        {
            get { return _nameNetInter; }
            set
            {
                _nameNetInter = value;

                OnPropertyChanged();
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

        private double _minTimeForChart;

        public double MinTimeForChart
        {
            get { return _minTimeForChart; }
            set
            {
                _minTimeForChart = value;

                OnPropertyChanged();
            }
        }

        private double _maxTimeForChart;

        public double  MaxTimeForChart
        {
            get { return _maxTimeForChart; }
            set
            {
                _maxTimeForChart = value;

                OnPropertyChanged();
            }
        }




        #endregion

        #region collections

        public ObservableCollection<NodeActiveNetInterface> ActivityDataForChart{ get; set; }


        #endregion

        #region ctor

        public NetAdapterDataView()
        {
            ActivityDataForChart = new ObservableCollection<NodeActiveNetInterface>();
        }

        public NetAdapterDataView(NetAdapterDataView data)
        {
            ActivityDataForChart = new ObservableCollection<NodeActiveNetInterface>();

            NameInter = data.NameInter;

            UpSpeed = data.UpSpeed;

            DownSpeed = data.DownSpeed;
        }

        #endregion

        #region public methods 

        public void SetParamData(double upSpeed, double downSpeed)
        {
            UpSpeed = upSpeed;

            DownSpeed = downSpeed;

            
            if (ActivityDataForChart.Count > _maxCountNodeInChartMin)
                ActivityDataForChart.RemoveAt(0);

            ActivityDataForChart.Add(new NodeActiveNetInterface { DownSpeed = downSpeed, UpSpeed = upSpeed, Time = DateTime.Now });


            if(ActivityDataForChart.Count< _maxCountNodeInChartMin)
            {
                var tmpMinTime = ActivityDataForChart[0].Time.Add(-(new TimeSpan(0, 0, _maxCountNodeInChartMin - ActivityDataForChart.Count)));

                MinTimeForChart = DateTimeAxis.ToDouble(tmpMinTime);
            }
            else
            {
                MinTimeForChart = DateTimeAxis.ToDouble(ActivityDataForChart.First().Time);
            }


            MaxTimeForChart = DateTimeAxis.ToDouble(ActivityDataForChart.Last().Time);
        }


        #endregion



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class NodeActiveNetInterface
    {
        public double UpSpeed { get; set; }

        public double DownSpeed { get; set; }

        public DateTime Time { get; set; }

        public NodeActiveNetInterface()
        {

        }
    }
}
