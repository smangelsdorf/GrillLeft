using Microsoft.VisualStudio.TestTools.UnitTesting;
using GrillLeft.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrillLeft.Device;
using GrillLeft.Model;
using System.Collections;

namespace GrillLeft.ViewModel.Tests
{
    [TestClass()]
    public class TemperatureSeriesViewModelTests
    {
        private ThermometerState RandomState(DateTime time)
        {
            var data = new byte[16];
            var rand = new Random((int)DateTime.Now.Ticks);
            rand.NextBytes(data);
            data[13] = 1;
            data[12] |= 0xF0;
            return new ThermometerState(ThermometerState.ThermometerChannel.One, data, time);
        }

        [TestMethod()]
        public void ThermometerStateObservableTest()
        {
            var observable = new ThermometerStateObservable();

            var viewModel = new TemperatureSeriesViewModel();
            viewModel.ThermometerStateObservable = observable;

            var t1 = DateTime.Now.Ticks;
            t1 -= t1 % TimeSpan.FromMinutes(1).Ticks;
            var t0 = t1 - TimeSpan.FromSeconds(10).Ticks;
            var t2 = t1 + TimeSpan.FromSeconds(10).Ticks;

            var states = new long[] { t0, t1, t2 }
            .Select(t => RandomState(new DateTime(t)))
            .ToList();

            foreach (var state in states)
            {
                observable.Emit(state);
            }

            Assert.AreEqual(1, viewModel.SeriesCollection.Count);
            IList values = viewModel.SeriesCollection.First().ActualValues;
            Assert.AreEqual(2, values.Count);
        }
    }
}