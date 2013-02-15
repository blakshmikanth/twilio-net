﻿using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Twilio.Activities
{

    /// <summary>
    /// Simple <see cref="SynchronizationContext"/> implementation that simply buffers callbacks to be executed when
    /// Run is invoked manually.
    /// </summary>
    class RunnableSynchronizationContext : SynchronizationContext
    {

        /// <summary>
        /// Set of actions to be dispatched.
        /// </summary>
        BlockingCollection<Tuple<SendOrPostCallback, object>> actions =
            new BlockingCollection<Tuple<SendOrPostCallback, object>>();

        public override void Post(SendOrPostCallback d, object state)
        {
            actions.Add(new Tuple<SendOrPostCallback, object>(d, state));
        }

        /// <summary>
        /// Runs the actions posted to the synchronization context until completion.
        /// </summary>
        public void Run()
        {
            // continue until we're complete
            foreach (var item in actions.GetConsumingEnumerable())
                item.Item1(item.Item2);

            // finish any left over items added as a result of being complete
            foreach (var item in actions.GetConsumingEnumerable())
                item.Item1(item.Item2);
        }

        /// <summary>
        /// Completes the synchronization context.
        /// </summary>
        public void Complete()
        {
            actions.CompleteAdding();
        }

    }

}
