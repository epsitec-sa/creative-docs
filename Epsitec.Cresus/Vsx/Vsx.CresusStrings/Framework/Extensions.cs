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

		public static void ForgetSafely(this Task task)
		{
			task.ContinueWith (t => Extensions.HandleException (t));
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

		public static IDocument ActiveDocument(this ISolution solution, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested ();
			var dte2 = DTE2Provider.GetDTE2 (System.Diagnostics.Process.GetCurrentProcess ().Id);
			cancellationToken.ThrowIfCancellationRequested ();
			var dteActiveDocumentPath = dte2.ActiveDocument.FullName;
			for (int retryCount = 0; retryCount < 3; ++retryCount)
			{
				try
				{
					return solution.Documents (cancellationToken).Where (d => string.Compare (d.FilePath, dteActiveDocumentPath, true) == 0).Single ();
				}
				catch (InvalidOperationException)
				{
				}
			}
			return null;
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
			return node is MemberAccessExpressionSyntax || node is IdentifierNameSyntax;
		}

		public static bool IsInvocation(this CommonSyntaxNode node)
		{
			return node is InvocationExpressionSyntax || (node.Parent != null && node.Parent is InvocationExpressionSyntax);
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
