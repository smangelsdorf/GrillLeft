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
using static GrillLeft.Model.ThermometerState;

namespace GrillLeft.ViewModel
{
    using LiveCharts.Configurations;
    using System.Collections.Specialized;
    using ThermometerChannelGroupedObserver = IGroupedObservable<ThermometerChannel, ThermometerState>;

    internal class TemperatureSeriesViewModel
    {
        private readonly SeriesCollection seriesCollection;

        private IDisposable Subscription;
        private IDisposable ReleaseMax;

        internal TemperatureSeriesViewModel(Action<Action> dispatcher, Action updater)
        {
            seriesCollection = new SeriesCollection();
            Dispatcher = dispatcher;
            Updater = updater;

            MinTime = DateTime.Now.Ticks;
            MaxTime = MinTime + TimeSpan.FromHours(1).Ticks;
        }

        internal IObservable<ThermometerState> ThermometerStateObservable
        {
            set
            {
                if (Subscription != null) Subscription.Dispose();
                Subscription = value.GroupBy(st => st.Channel)
                    .Subscribe(new ThermometerStateObserver(seriesCollection, Dispatcher, Updater));

                ReleaseMax = value.Subscribe(t =>
                {
                    if (MaxTime < t.Time.Ticks)
                    {
                        MaxTime = double.NaN;
                        ReleaseMax.Dispose();
                        Console.WriteLine("Exceeded max, dropped max");
                    }
                });
            }
        }

        public SeriesCollection SeriesCollection
        {
            get
            {
                return seriesCollection;
            }
        }

        public Func<double, string> YFormatter
        {
            get
            {
                return (d) => String.Format("{0:0.0}", d);
            }
        }

        public Func<double, string> XFormatter
        {
            get
            {
                return (dt) => new DateTime((long)dt).ToString("HH:mm");
            }
        }

        public double MinTime { get; private set; }
        public double MaxTime { get; private set; }

        internal Action<Action> Dispatcher { get; set; }
        internal Action Updater { get; set; }

        private class ThermometerStateObserver : IObserver<ThermometerChannelGroupedObserver>
        {
            private static readonly long ONE_MINUTE = TimeSpan.FromMinutes(1).Ticks;

            private readonly SeriesCollection SeriesCollection;
            private readonly IList<IDisposable> Subscriptions;
            private readonly CartesianMapper<AveragingDateTimePoint> Mapper;
            private readonly Action<Action> Dispatcher;
            private readonly Action Updater;

            internal ThermometerStateObserver(SeriesCollection seriesCollection, Action<Action> dispatcher, Action updater)
            {
                SeriesCollection = seriesCollection;
                Subscriptions = new List<IDisposable>();
                Mapper = Mappers.Xy<AveragingDateTimePoint>()
                    .X(p => p.DateTime.Ticks).Y(p => p.Value);
                Dispatcher = dispatcher;
                Updater = updater;
            }

            void IObserver<ThermometerChannelGroupedObserver>.OnNext(IGroupedObservable<ThermometerChannel, ThermometerState> observable)
            {
                Dispatcher.Invoke(() =>
                {
                    var values = new AppendOnlyChartValues<AveragingDateTimePoint>(Mapper);
                    INotifyCollectionChanged notifier = values;
                    notifier.CollectionChanged += NewElementAdded;

                    var series = new LineSeries()
                    {
                        Title = $"Channel {(int)observable.Key}",
                        Values = values,
                        LineSmoothness = 0,
                        PointGeometry = null
                    };

                    SeriesCollection.Add(series);

                    var subscription = AggregatePoints(observable).Subscribe(values);
                    Subscriptions.Add(subscription);
                });
            }

            private void NewElementAdded(object sender, NotifyCollectionChangedEventArgs e)
            {
                Updater();
            }

            private IObservable<AveragingDateTimePoint> AggregatePoints(IObservable<ThermometerState> observable)
            {
                AveragingDateTimePoint point = null;

                return observable
                    .Where(st => st.RawTemperature != 0x8FFF)
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
                            var result = Observable.Return(point);
                            point = p;
                            return result;
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