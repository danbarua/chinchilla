using System.Collections.Concurrent;
using System.Linq;
using Chinchilla.Logging;
using Chinchilla.Threading;

namespace Chinchilla
{
    public class WorkerPoolDeliveryStrategy : DeliveryStrategy
    {
        private readonly ILogger logger = Logger.Create<WorkerPoolDeliveryStrategy>();

        private readonly BlockingCollection<IDelivery> deliveries = new BlockingCollection<IDelivery>(
            new ConcurrentQueue<IDelivery>());

        private readonly IThreadFactory threadFactory = new ThreadFactory();

        private IWorkerPoolWorker[] workers = new IWorkerPoolWorker[0];

        public WorkerPoolDeliveryStrategy()
        {
            NumWorkers = 1;
        }

        public int NumWorkers { get; set; }

        public override bool IsStartable
        {
            get { return NumWorkers > 0; }
        }

        public override void Start()
        {
            logger.DebugFormat("Starting {0}", this);

            if (NumWorkers <= 0)
            {
                throw new ChinchillaException(
                    "Could not start the worker pool delivery queue " +
                    "because the number of configured worker threads is zero");
            }

            workers = Enumerable
                .Range(0, NumWorkers)
                .Select(ordinal => new WorkerPoolWorker(ordinal, threadFactory, deliveries, connectedProcessor))
                .Cast<IWorkerPoolWorker>()
                .ToArray();

            foreach (var worker in workers)
            {
                worker.Start();
            }
        }

        public override void Deliver(IDelivery delivery)
        {
            deliveries.Add(delivery);
        }

        public override WorkerState[] GetWorkerStates()
        {
            return workers.Select(t => t.GetState()).ToArray();
        }

        public override IWorkersController GetWorkersController()
        {
            if (workers.Length == 0)
            {
                throw new ChinchillaException(
                    "Could not get a workers controller, this delivery strategy " +
                    "has no workers yet, has it been started?");
            }

            return new WorkerPoolWorkersController(workers);
        }

        public override void Stop()
        {
            logger.DebugFormat("Stopping {0}", this);

            foreach (var worker in workers)
            {
                worker.Stop();
            }

            deliveries.CompleteAdding();

            foreach (var worker in workers)
            {
                worker.Join();
            }

            logger.DebugFormat("Stopped {0}", this);
        }

        public override string ToString()
        {
            return string.Format("[WorkerPoolDeliveryStrategy NumWorkers={0}]", NumWorkers);
        }
    }
}