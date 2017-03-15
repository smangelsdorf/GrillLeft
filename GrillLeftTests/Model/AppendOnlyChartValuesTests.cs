using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using GrillLeft.Model;
using System.Collections;
using LiveCharts.Configurations;
using LiveCharts;
using LiveCharts.Definitions.Series;
using GrillLeftTests.Stub;
using LiveCharts.Helpers;

namespace GrillLeftTests.Model
{
    [TestClass]
    public class AppendOnlyChartValuesTests
    {
        private struct TestPoint
        {
            internal readonly double X;
            internal readonly double Y;

            public TestPoint(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        private static Random Random = new Random();

        private TestPoint RandomTestPoint()
        {
            return new TestPoint(Random.NextDouble(), Random.NextDouble());
        }

        private IEnumerable<TestPoint> RandomTestPoints(int count)
        {
            return Enumerable.Range(0, count).Select(_ => RandomTestPoint());
        }

        private AppendOnlyChartValues<TestPoint> NewCollection()
        {
            var mapper = Mappers.Xy<TestPoint>().X(p => p.X).Y(p => p.Y);
            return new AppendOnlyChartValues<TestPoint>(mapper);
        }

        [TestMethod]
        public void IndexOperatorTest()
        {
            IList list = NewCollection();

            var points = RandomTestPoints(5).ToArray();

            for (var i = 0; i < points.Count(); ++i)
            {
                list[i] = points[i];
            }

            for (var i = 0; i < points.Count(); ++i)
            {
                var item = (TestPoint)list[i];
                Assert.AreEqual(item.X, points[i].X);
            }

            Assert.AreEqual(points.Count(), list.Count);
        }

        [TestMethod]
        public void AddTest()
        {
            IList list = NewCollection();

            var points = RandomTestPoints(5).ToArray();

            foreach (var item in points)
            {
                list.Add(item);
            }

            for (var i = 0; i < points.Count(); ++i)
            {
                var item = (TestPoint)list[i];
                Assert.AreEqual(item.X, points[i].X);
            }

            Assert.AreEqual(points.Count(), list.Count);
        }

        [TestMethod]
        public void GetEnumerableTest()
        {
            IList list = NewCollection();
            ICollection collection = list;

            var points = RandomTestPoints(5).ToList();
            foreach (var item in points)
            {
                list.Add(item);
            }

            var enumerator = collection.GetEnumerator();
            var result = new List<TestPoint>();
            while (enumerator.MoveNext())
            {
                result.Add((TestPoint)enumerator.Current);
            }

            Assert.AreEqual(points.Count, result.Count);
            foreach (var entry in points.Zip(result, (a, b) => Tuple.Create(a, b)))
            {
                Assert.AreEqual(entry.Item1, entry.Item2);
            }
        }

        [TestMethod]
        public void GetPointsTest()
        {
            IChartValues chartValues = NewCollection();
            IList list = chartValues;

            var points = RandomTestPoints(5).ToList();
            foreach (var item in points)
            {
                list.Add(item);
            }

            var chartPoints = chartValues.GetPoints(null);
            Assert.AreEqual(points.Count, chartPoints.Count());
            foreach (var entry in points.Zip(chartPoints, (a, b) => Tuple.Create(a, b)))
            {
                Assert.AreEqual(entry.Item1.X, entry.Item2.X);
                Assert.AreEqual(entry.Item1.Y, entry.Item2.Y);
            }
        }

        [TestMethod]
        public void GetTrackerTest()
        {
            var seriesView = new TestSeriesView();

            IChartValues chartValues = NewCollection();
            var tracker = chartValues.GetTracker(seriesView);

            Assert.IsNotNull(tracker);
            Assert.IsTrue(tracker == chartValues.GetTracker(seriesView));
            Assert.IsFalse(tracker == chartValues.GetTracker(new TestSeriesView()));
        }

        [TestMethod]
        public void InitializeTest()
        {
            var seriesView = new TestSeriesView();

            IChartValues chartValues = NewCollection();
            IList list = chartValues;
            var tracker = chartValues.GetTracker(seriesView);

            var points = RandomTestPoints(50).ToList();
            foreach (var item in points)
            {
                list.Add(item);
            }

            var minX = points.Select(i => i.X).Min();
            var maxX = points.Select(i => i.X).Max();
            var minY = points.Select(i => i.Y).Min();
            var maxY = points.Select(i => i.Y).Max();

            chartValues.Initialize(seriesView);
            Assert.AreEqual(minX, tracker.XLimit.Min);
            Assert.AreEqual(maxX, tracker.XLimit.Max);
            Assert.AreEqual(minY, tracker.YLimit.Min);
            Assert.AreEqual(maxY, tracker.YLimit.Max);
        }

        [TestMethod]
        public void AddRangeTest()
        {
            IChartValues chartValues = NewCollection();
            INoisyCollection noisy = chartValues;
            IList list = chartValues;

            var points = RandomTestPoints(5).ToList();
            noisy.AddRange(points.Select(i => (object)i));

            for (var i = 0; i < points.Count(); ++i)
            {
                var item = (TestPoint)list[i];
                Assert.AreEqual(item.X, points[i].X);
            }

            Assert.AreEqual(points.Count(), list.Count);
        }

        [TestMethod]
        public void OnNextTest()
        {
            var values = NewCollection();
            IObserver<TestPoint> observer = values;
            IList list = values;

            var points = RandomTestPoints(5).ToArray();

            foreach (var item in points)
            {
                observer.OnNext(item);
            }

            Assert.AreEqual(points.Count(), list.Count);

            for (var i = 0; i < points.Count(); ++i)
            {
                var item = (TestPoint)list[i];
                Assert.AreEqual(item.X, points[i].X);
            }
        }
    }
}