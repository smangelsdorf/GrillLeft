using LiveCharts.Definitions.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Definitions.Points;

namespace GrillLeftTests.Stub
{
    internal class TestSeriesView : ISeriesView
    {
        IChartValues ISeriesView.ActualValues
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        object ISeriesView.Configuration
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        bool ISeriesView.DataLabels
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ISeriesView.IsFirstDraw
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool ISeriesView.IsSeriesVisible
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        Func<ChartPoint, string> ISeriesView.LabelPoint
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        SeriesAlgorithm ISeriesView.Model { get; set; }

        int ISeriesView.ScalesXAt
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        int ISeriesView.ScalesYAt
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        string ISeriesView.Title
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IChartValues ISeriesView.Values
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        void ISeriesView.DrawSpecializedElements()
        {
            throw new NotImplementedException();
        }

        void ISeriesView.Erase(bool removeFromView)
        {
            throw new NotImplementedException();
        }

        Func<ChartPoint, string> ISeriesView.GetLabelPointFormatter()
        {
            throw new NotImplementedException();
        }

        IChartPointView ISeriesView.GetPointView(ChartPoint point, string label)
        {
            throw new NotImplementedException();
        }

        void ISeriesView.InitializeColors()
        {
            throw new NotImplementedException();
        }

        void ISeriesView.OnSeriesUpdatedFinish()
        {
            throw new NotImplementedException();
        }

        void ISeriesView.OnSeriesUpdateStart()
        {
            throw new NotImplementedException();
        }

        void ISeriesView.PlaceSpecializedElements()
        {
            throw new NotImplementedException();
        }
    }
}