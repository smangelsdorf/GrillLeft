using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GrillLeft.Device
{
    internal class ThermometerStateObservable : IObservable<ThermometerState>, IDisposable
    {
        private class Subscription : IDisposable
        {
            private Delegate OnDispose;

            internal Subscription(Delegate onDispose)
            {
                this.OnDispose = onDispose;
            }

            public void Dispose()
            {
                OnDispose.DynamicInvoke();
            }
        }

        private readonly IDictionary<Int32, IObserver<ThermometerState>> Subscribers;

        private Int32 SubscriberNumber;

        internal ThermometerStateObservable()
        {
            this.Subscribers = new Dictionary<Int32, IObserver<ThermometerState>>();
        }

        public IDisposable Subscribe(IObserver<ThermometerState> observer)
        {
            var i = Interlocked.Increment(ref SubscriberNumber);
            Subscribers.Add(i, observer);
            Func<Object> f = () => Subscribers.Remove(i);

            return new Subscription(f);
        }

        internal void Emit(ThermometerState state)
        {
            foreach (var subscriber in Subscribers.Values)
            {
                subscriber.OnNext(state);
            }
        }

        public void Dispose()
        {
            foreach (var subscriber in Subscribers.Values)
            {
                subscriber.OnCompleted();
            }
        }
    }
}