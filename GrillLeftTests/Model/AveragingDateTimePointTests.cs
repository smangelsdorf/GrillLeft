using Microsoft.VisualStudio.TestTools.UnitTesting;
using GrillLeft.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrillLeft.Model.Tests
{
    [TestClass()]
    public class AveragingDateTimePointTests
    {
        [TestMethod()]
        public void ValueTest()
        {
            var rand = new Random();

            var time = new DateTime(rand.Next());
            var total = rand.NextDouble() * 1e10;
            var count = rand.Next();

            var point = new AveragingDateTimePoint(time, total, count);
            Assert.AreEqual(total / count, point.Value);
        }

        [TestMethod()]
        public void ConcatTest()
        {
            var rand = new Random();

            var time1 = new DateTime(rand.Next());
            var total1 = rand.NextDouble() * 1e10;
            var count1 = rand.Next();
            var point1 = new AveragingDateTimePoint(time1, total1, count1);

            var time2 = new DateTime(rand.Next());
            var total2 = rand.NextDouble() * 1e10;
            var count2 = rand.Next();
            var point2 = new AveragingDateTimePoint(time2, total2, count2);

            var point = point1.Concat(point2);
            Assert.AreEqual(new DateTime[] { time1, time2 }.Max(), point.DateTime);
            Assert.AreEqual(total1 + total2, point.Total);
            Assert.AreEqual(count1 + count2, point.Count);
        }
    }
}