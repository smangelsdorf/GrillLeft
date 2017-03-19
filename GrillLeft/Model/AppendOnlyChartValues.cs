using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts.Definitions.Series;
using LiveCharts.Helpers;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using LiveCharts.Configurations;
using LiveCharts.Dtos;

namespace GrillLeft.Model
{
    internal class AppendOnlyChartValues<T> : IChartValues, IObserver<T>
    {
        private static readonly int INCREMENT = 2000;

        private struct Entry
        {
            internal readonly T Item;
            internal readonly ChartPoint ChartPoint;

            internal Entry(CartesianMapper<T> mapper, int index, T item)
            {
                Item = item;
                ChartPoint = new ChartPoint();
                mapper.Evaluate(index, item, ChartPoint);
            }
        }

        private readonly IList<Entry[]> Storage;
        private readonly IDictionary<ISeriesView, PointTracker> Trackers;
        private readonly CartesianMapper<T> Mapper;
        private readonly IList<NotifyCollectionChangedEventHandler> CollectionChangedNotifiers;

        private int Count;
        private double MinX, MaxX, MinY, MaxY;

        internal AppendOnlyChartValues(CartesianMapper<T> mapper)
        {
            Mapper = mapper;
            Storage = new List<Entry[]>();
            Trackers = new Dictionary<ISeriesView, PointTracker>();
            CollectionChangedNotifiers = new List<NotifyCollectionChangedEventHandler>();
            Count = 0;

            MinX = double.PositiveInfinity;
            MaxX = double.NegativeInfinity;
            MinY = double.PositiveInfinity;
            MaxY = double.NegativeInfinity;
        }

        private void Append(T value)
        {
            var n = Count / INCREMENT;
            while (Storage.Count < n + 1)
            {
                Storage.Add(new Entry[INCREMENT]);
            }

            var entry = new Entry(Mapper, Count, value);
            Storage[n][Count % INCREMENT] = entry;
            Count += 1;

            if (entry.ChartPoint.X < MinX) MinX = entry.ChartPoint.X;
            if (entry.ChartPoint.X > MaxX) MaxX = entry.ChartPoint.X;
            if (entry.ChartPoint.Y < MinY) MinY = entry.ChartPoint.Y;
            if (entry.ChartPoint.Y > MaxY) MaxY = entry.ChartPoint.Y;

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value);
            CollectionChangedNotifiers.ForEach(a => a.Invoke(this, args));
        }

        private IEnumerable<Entry> GetEntryEnumerable()
        {
            return Storage.SelectMany(a => a).Take(Count);
        }

        object IList.this[int index]
        {
            get
            {
                var n = index / INCREMENT;
                return Storage[n][index % INCREMENT].Item;
            }

            set
            {
                if (index != Count) throw new InvalidOperationException("Append-only data structure");
                Append((T)value);
            }
        }

        int IList.Add(object value)
        {
            var result = Count;
            Append((T)value);
            return result;
        }

        void IList.Clear()
        {
            throw new NotImplementedException("Append-only data structure");
        }

        bool IList.Contains(object value)
        {
            throw new NotImplementedException("Search not permitted");
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        int IList.IndexOf(object value)
        {
            throw new NotImplementedException("Search not permitted");
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException("Append-only data structure");
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException("Append-only data structure");
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException("Append-only data structure");
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException("Exfiltrate not permitted");
        }

        int ICollection.Count
        {
            get
            {
                return Count;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEntryEnumerable().Select(e => e.Item).GetEnumerator();
        }

        IEnumerable<ChartPoint> IChartValues.GetPoints(ISeriesView seriesView)
        {
            return GetEntryEnumerable().Select(e => e.ChartPoint);
        }

        PointTracker IChartValues.GetTracker(ISeriesView view)
        {
            PointTracker tracker;

            if (Trackers.TryGetValue(view, out tracker)) return tracker;

            tracker = new PointTracker();
            Trackers[view] = tracker;

            return tracker;
        }

        void IChartValues.Initialize(ISeriesView seriesView)
        {
            IChartValues values = this;
            var tracker = values.GetTracker(seriesView);
            tracker.XLimit = new CoreLimit(MinX, MaxX);
            tracker.YLimit = new CoreLimit(MinY, MaxY);
        }

        void IChartValues.InitializeStep(ISeriesView seriesView)
        {
        }

        void IChartValues.CollectGarbage(ISeriesView seriesView)
        {
        }

        void INoisyCollection.InsertRange(int index, IEnumerable<object> collection)
        {
            throw new NotImplementedException("Append-only data structure");
        }

        void INoisyCollection.AddRange(IEnumerable<object> items)
        {
            foreach (var item in items)
            {
                Append((T)item);
            }
        }

        void IObserver<T>.OnNext(T value)
        {
            Append(value);
        }

        void IObserver<T>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<T>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add
            {
                CollectionChangedNotifiers.Add(value);
            }

            remove
            {
                throw new NotImplementedException("Not necessary");
            }
        }

        event NoisyCollectionCollectionChanged<object> INoisyCollection.NoisyCollectionChanged
        {
            add
            {
                throw new NotImplementedException("Not necessary");
            }

            remove
            {
                throw new NotImplementedException("Not necessary");
            }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                throw new NotImplementedException("Not necessary");
            }

            remove
            {
                throw new NotImplementedException("Not necessary");
            }
        }
    }
}