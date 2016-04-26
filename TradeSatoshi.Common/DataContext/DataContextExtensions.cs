using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace TradeSatoshi.Common.Data
{
	public static class DataContextExtensions
	{
		public static List<T> ToListNoLock<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = query.ToList();
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<List<T>> ToListNoLockAsync<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				List<T> toReturn = await query.ToListAsync();
				scope.Complete();
				return toReturn;
			}
		}

		public static List<T> ToListNoLock<T>(this IOrderedQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = query.ToList();
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<List<T>> ToListNoLockAsync<T>(this IOrderedQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				List<T> toReturn = await query.ToListAsync();
				scope.Complete();
				return toReturn;
			}
		}

		public static int CountNoLock<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				int toReturn = query.Count();
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<int> CountNoLockAsync<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				int toReturn = await query.CountAsync();
				scope.Complete();
				return toReturn;
			}
		}

		public static int CountNoLock<T>(this IOrderedQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				int toReturn = query.Count();
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<int> CountNoLockAsync<T>(this IOrderedQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				int toReturn = await query.CountAsync();
				scope.Complete();
				return toReturn;
			}
		}

		public static T FirstOrDefaultNoLock<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = query.FirstOrDefault();
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<T> FirstOrDefaultNoLockAsync<T>(this IQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = await query.FirstOrDefaultAsync();
				scope.Complete();
				return toReturn;
			}
		}

		public static T FirstOrDefaultNoLock<T>(this IOrderedQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = query.FirstOrDefault();
				scope.Complete();
				return toReturn;
			}
		}

		public static async Task<T> FirstOrDefaultNoLockAsync<T>(this IOrderedQueryable<T> query)
		{
			using (
				var scope = new TransactionScope(TransactionScopeOption.Required,
					new TransactionOptions() {IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted},
					TransactionScopeAsyncFlowOption.Enabled))
			{
				var toReturn = await query.FirstOrDefaultAsync();
				scope.Complete();
				return toReturn;
			}
		}
	}
}