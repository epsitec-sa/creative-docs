using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Epsitec.VisualStudio;
using Microsoft.VisualStudio.Text;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace Epsitec
{
	public static class Extensions
	{
		#region Object

		public static void ThrowIfNull(this object source)
		{
			if (source == null)
			{
				throw new NullReferenceException ("source");
			}
		} 

		#endregion

		#region Linq

		/// <summary>
		/// Performs a task on each element of a sequence and returns the source sequence,
		/// to allow the insertion of side effects into a chain linq queries
		/// </summary>
		/// <typeparam name="T">The type of element of source</typeparam>
		/// <param name="source">The sequence of side effects values</param>
		/// <param name="action">The side effects task</param>
		/// <returns>The source sequence</returns>
		public static IEnumerable<T> Do<T>(this IEnumerable<T> source, Action<T> action)
		{
			return source.Select (x =>
			{
				action (x);
				return x;
			});
		}

		public static IEnumerable<T> AsSequence<T>(this T first)
		{
			yield return first;
		}
		public static IEnumerable<T> AsSequence<T>(T first, params T[] others)
		{
			yield return first;
			foreach (var other in others)
			{
				yield return other;
			}
		}

		#endregion

		#region Task

		//public static Task<T> WithTimeout<T>(this Task<T> task, int millisecondsTimeout)
		//{
		//	return task.WithTimeout (TimeSpan.FromMilliseconds (millisecondsTimeout));
		//}

		//public static Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout)
		//{
		//	var result = new TaskCompletionSource<TResult> (task.AsyncState);
		//	var timer = new Timer (_ => result.TrySetException (new TimeoutException()),
		//		null, timeout, TimeSpan.FromMilliseconds (-1));
		//	task.ContinueWith (t =>
		//	{
		//		timer.Dispose ();
		//		result.TrySetFromTask (t);
		//	});
		//	return result.Task;
		//}

		//public static bool TrySetFromTask<TResult>(this TaskCompletionSource<TResult> resultSetter, Task<TResult> task)
		//{
		//	switch (task.Status)
		//	{
		//		case TaskStatus.RanToCompletion:
		//			return resultSetter.TrySetResult (task.Result);
		//		case TaskStatus.Faulted:
		//			return resultSetter.TrySetException (
		//				task.Exception.InnerExceptions);
		//		case TaskStatus.Canceled:
		//			return resultSetter.TrySetCanceled ();
		//		default:
		//			throw new InvalidOperationException ("The task was not completed.");
		//	}
		//}

		public async static Task WithTimeout(this Task task, int millisecondsTimeout)
		{
			await task.WithTimeout (TimeSpan.FromMilliseconds (millisecondsTimeout));
		}

		public async static Task WithTimeout(this Task task, TimeSpan timeout)
		{
			var cts = new CancellationTokenSource ();
			var delay = Task.Delay (timeout, cts.Token).ForgetSafely ();
			await Task.WhenAny (task, delay);
			if (task.IsCompleted)
			{
				cts.Cancel ();
			}
			else
			{
				throw new TimeoutException ();
			}
		}

		public async static Task<T> WithTimeout<T>(this Task<T> task, int millisecondsTimeout)
		{
			return await task.WithTimeout (TimeSpan.FromMilliseconds (millisecondsTimeout));
		}

		public async static Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
		{
			await ((Task) task).WithTimeout (timeout);
			return await task;
		}

		public static Task ForgetSafely(this Task task)
		{
			// observe exceptions
			task.ContinueWith (t => Extensions.HandleException (t));
			return task;
		} 

		#endregion

		#region Roslyn

		public static IEnumerable<IDocument> Documents(this ISolution solution, CancellationToken cancellationToken = default(CancellationToken))
		{
			//return solution.Projects.SelectMany (p => p.Documents);
			foreach (var project in solution.Projects)
			{
				cancellationToken.ThrowIfCancellationRequested ();
				foreach (var document in project.Documents)
				{
					cancellationToken.ThrowIfCancellationRequested ();
					yield return document;
				}
			}
		}

		public static SyntaxNode RemoveTrivias(this SyntaxNode node)
		{
			return new TriviasRemover ().Visit (node);
		}

		public static IEnumerable<TextChange> ToRoslynTextChanges(this INormalizedTextChangeCollection changes)
		{
			return changes.Select (change => new TextChange (new TextSpan (change.OldSpan.Start, change.OldSpan.Length), change.NewText));
		}

		public static bool IsMemberAccess(this CommonSyntaxNode node)
		{
			return node != null && (node is MemberAccessExpressionSyntax || node is IdentifierNameSyntax || node is AliasQualifiedNameSyntax);
		}

		public static bool IsPropertyOrField(this CommonSyntaxNode node)
		{
			if (node is IdentifierNameSyntax)
			{
				node = node.Parent;
				if (node == null || node.IsMemberAccess() && !node.IsInvocation ())
				{
					return true;
				}
			}
			return node.IsMemberAccess() && !node.IsInvocation ();
		}

		public static bool IsInvocation(this CommonSyntaxNode node)
		{
			return node != null && (node is InvocationExpressionSyntax || (node.Parent != null && node.Parent is InvocationExpressionSyntax));
		} 

		#endregion

		#region VisualStudio

		public static bool ContiguousWith(this TextSpan source, TextSpan other)
		{
			return source.End == other.Start || other.End == source.Start;
		}

		public static bool ContiguousWith(this Span source, Span other)
		{
			return source.End == other.Start || other.End == source.Start;
		}

		#endregion

		#region XLinq

		public static string GetString(this XAttribute attribute)
		{
			return attribute == null ? default (string) : (string) attribute;
		}

		public static string GetStringOrEmpty(this XAttribute attribute)
		{
			return attribute == null ? string.Empty : (string) attribute;
		}
	
		#endregion

		#region Dictionary

		public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, Func<TKey, TValue> valueFactory)
		{
			TValue value;
			if (source.TryGetValue (key, out value))
			{
				return value;
			}
			return source[key] = valueFactory (key);
		}
		#endregion
		
		#region Helpers
		
		private class TriviasRemover : SyntaxRewriter
		{
			public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
			{
				return default (SyntaxTrivia);
			}
		}

		private static void HandleException(Task task)
		{
			if (task.Exception != null)
			{
				var ex = (task.Exception is AggregateException) ? (task.Exception as AggregateException).Flatten ().InnerException : task.Exception;
				System.Diagnostics.Trace.WriteLine (string.Format ("Asynchronous exception swallowed: {0} - {1}" + ex.GetType ().Name, ex.Message));
			}
		}

		#endregion
	}
}
