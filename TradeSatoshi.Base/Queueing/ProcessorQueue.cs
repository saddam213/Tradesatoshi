using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TradeSatoshi.Base.Queueing
{
	/// <summary>
	/// A synchronous blocking queue for processing items in a FIFO scenario
	/// </summary>
	public sealed class ProcessorQueue<T, U> : IDisposable
	{
		/// <summary>
		/// The function to be called when items are processed in the queue
		/// </summary>
		private readonly Func<T, Task<U>> _processFunction;

		/// <summary>
		/// The queue
		/// </summary>
		private readonly BlockingCollection<ProcessorQueueItem<T, U>> _processQueue = new BlockingCollection<ProcessorQueueItem<T, U>>();

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessorQueue"/> class.
		/// </summary>
		/// <param name="processFunction">The function to be called when items are processed in the queue</param>
		public ProcessorQueue(Func<T, Task<U>> processFunction)
		{
			_processFunction = processFunction;
			Task.Factory.StartNew(async () => await ProcessQueue(), TaskCreationOptions.LongRunning);
		}

		/// <summary>
		/// Queues an item to be processed
		/// </summary>
		/// <param name="item">The object to be passed to the function.</param>
		/// <returns>The completion task to be awaited on</returns>
		public Task<U> QueueItem(T item)
		{
			var queueItem = new ProcessorQueueItem<T, U>(item);
			_processQueue.TryAdd(queueItem);
			return queueItem.CompletionSource.Task;
		}

		/// <summary>
		/// Processes the queue.
		/// </summary>
		private async Task ProcessQueue()
		{
			ProcessorQueueItem<T, U> queueItem;
			while (_processQueue.TryTake(out queueItem, Timeout.Infinite))
			{
				var input = queueItem.Item;
				var tcs = queueItem.CompletionSource;

				try
				{
					tcs.SetResult(await _processFunction(input));
				}
				catch (Exception ex)
				{
					tcs.SetException(ex);
				}
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_processQueue.CompleteAdding();
			_processQueue.Dispose();
			GC.SuppressFinalize(this);
		}
	}

	public sealed class ProcessorQueue<T> : IDisposable
	{
		/// <summary>
		/// The function to be called when items are processed in the queue
		/// </summary>
		private readonly Func<T, Task> _processFunction;

		/// <summary>
		/// The queue
		/// </summary>
		private readonly BlockingCollection<T> _processQueue = new BlockingCollection<T>();

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessorQueue"/> class.
		/// </summary>
		/// <param name="processFunction">The function to be called when items are processed in the queue</param>
		public ProcessorQueue(Func<T, Task> processFunction)
		{
			_processFunction = processFunction;
			Task.Factory.StartNew(async () => await ProcessQueue(), TaskCreationOptions.LongRunning);
		}

		/// <summary>
		/// Queues an item to be processed
		/// </summary>
		/// <param name="item">The object to be passed to the function.</param>
		/// <returns>The completion task to be awaited on</returns>
		public void QueueItem(T item)
		{
			_processQueue.TryAdd(item);
		}

		/// <summary>
		/// Processes the queue.
		/// </summary>
		private async Task ProcessQueue()
		{
			T queueItem;
			while (_processQueue.TryTake(out queueItem, Timeout.Infinite))
			{
				await _processFunction(queueItem);
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_processQueue.CompleteAdding();
		}
	}

	public sealed class ProcessorQueueVoid<U> : IDisposable
	{
		/// <summary>
		/// The function to be called when items are processed in the queue
		/// </summary>
		private readonly Func<Task<U>> _processFunction;

		/// <summary>
		/// The queue
		/// </summary>
		private readonly BlockingCollection<ProcessorQueueItem<U>> _processQueue = new BlockingCollection<ProcessorQueueItem<U>>();

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessorQueue"/> class.
		/// </summary>
		/// <param name="processFunction">The function to be called when items are processed in the queue</param>
		public ProcessorQueueVoid(Func<Task<U>> processFunction)
		{
			_processFunction = processFunction;
			Task.Factory.StartNew(async () => await ProcessQueue(), TaskCreationOptions.LongRunning);
		}

		/// <summary>
		/// Queues an item to be processed
		/// </summary>
		/// <param name="item">The object to be passed to the function.</param>
		/// <returns>The completion task to be awaited on</returns>
		public Task<U> QueueItem()
		{
			var queueItem = new ProcessorQueueItem<U>();
			_processQueue.TryAdd(queueItem);
			return queueItem.CompletionSource.Task;
		}

		/// <summary>
		/// Processes the queue.
		/// </summary>
		private async Task ProcessQueue()
		{
			ProcessorQueueItem<U> queueItem;
			while (_processQueue.TryTake(out queueItem, Timeout.Infinite))
			{
				var tcs = queueItem.CompletionSource;
				try
				{
					tcs.SetResult(await _processFunction());
				}
				catch (Exception ex)
				{
					tcs.SetException(ex);
				}
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_processQueue.CompleteAdding();
			_processQueue.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
