using GrillLeft.Device;
using GrillLeft.Model;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using static GrillLeft.Device.ThermometerState;

namespace GrillLeft.ViewModel
{
    using LiveCharts.Configurations;
    using ThermometerChannelGroupedObserver = IGroupedObservable<ThermometerChannel, ThermometerState>;

    internal class TemperatureSeriesViewModel
    {
        internal readonly SeriesCollection SeriesCollection;

        private IDisposable Subscription;

        public TemperatureSeriesViewModel()
        {
            SeriesCollection = new SeriesCollection();
        }

        internal IObservable<ThermometerState> ThermometerStateObservable
        {
            set
            {
                if (Subscription != null) Subscription.Dispose();
                Subscription = value.GroupBy(st => st.Channel)
                    .Subscribe(new ThermometerStateObserver(SeriesCollection));
            }
        }

        private class ThermometerStateObserver : IObserver<ThermometerChannelGroupedObserver>
        {
            private static readonly long ONE_MINUTE = TimeSpan.FromMinutes(1).Ticks;

            private readonly SeriesCollection SeriesCollection;
            private readonly IList<IDisposable> Subscriptions;
            private readonly CartesianMapper<AveragingDateTimePoint> Mapper;

            public ThermometerStateObserver(SeriesCollection seriesCollection)
            {
                SeriesCollection = seriesCollection;
                Subscriptions = new List<IDisposable>();
                Mapper = Mappers.Xy<AveragingDateTimePoint>()
                    .X(p => p.DateTime.Ticks).Y(p => p.Value);
            }

            void IObserver<ThermometerChannelGroupedObserver>.OnNext(IGroupedObservable<ThermometerChannel, ThermometerState> observable)
            {
                var values = new AppendOnlyChartValues<AveragingDateTimePoint>(Mapper);

                var series = new LineSeries()
                {
                    Title = $"Channel {(int)observable.Key}",
                    Values = values,
                    LineSmoothness = 0
                };

                SeriesCollection.Add(series);

                var subscription = AggregatePoints(observable).Subscribe(values);
                Subscriptions.Add(subscription);
            }

            private IObservable<AveragingDateTimePoint> AggregatePoints(IObservable<ThermometerState> observable)
            {
                AveragingDateTimePoint point = null;

                return observable
                    .Select(st => new AveragingDateTimePoint(st.Time, st.TemperatureValue))
                    .SelectMany(p =>
                    {
                        if (point == null)
                        {
                            point = p;
                            return Observable.Empty<AveragingDateTimePoint>();
                        }
                        else if (point.DateTime.Ticks / ONE_MINUTE == p.DateTime.Ticks / ONE_MINUTE)
                        {
                            point = point.Concat(p);
                            return Observable.Empty<AveragingDateTimePoint>();
                        }
                        else
                        {
                            return Observable.Return(point);
                        }
                    });
            }

            void IObserver<IGroupedObservable<ThermometerChannel, ThermometerState>>.OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            void IObserver<IGroupedObservable<ThermometerChannel, ThermometerState>>.OnCompleted()
            {
                throw new NotImplementedException();
            }
        }
    }
}