using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualStudio.Text;
using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace Epsitec.Cresus.Strings
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

		#endregion

		#region Task

		public static void ForgetSafely(this Task task)
		{
			task.ContinueWith (t => Extensions.HandleException (t));
		} 

		#endregion

		#region Roslyn

		public static IEnumerable<IDocument> Documents(this ISolution solution)
		{
			return solution.Projects.SelectMany (p => p.Documents);
		}

		public static IDocument ActiveDocument(this ISolution solution, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested ();
			var dte2 = DTE2Provider.GetDTE2 (System.Diagnostics.Process.GetCurrentProcess ().Id);
			cancellationToken.ThrowIfCancellationRequested ();
			var dteActiveDocumentPath = dte2.ActiveDocument.FullName;
			return solution.Documents ().Do (_ => cancellationToken.ThrowIfCancellationRequested ()).Where (d => string.Compare (d.FilePath, dteActiveDocumentPath, true) == 0).Single ();
		}

		//public static CommonSyntaxNode GetNarrowestEnclosingNode(this CommonSyntaxNode node, int position)
		//{
		//	if (!node.Span.Contains (position))
		//	{
		//		return null;
		//	}
		//	var narrowestEnclosingNode = node;
		//	foreach (var child in node.ChildNodes ().Where (n => n.Span.Contains (position)))
		//	{
		//		if (narrowestEnclosingNode.Span.Contains (child.Span))
		//		{
		//			narrowestEnclosingNode = child;
		//		}
		//	}
		//	if (narrowestEnclosingNode == node)
		//	{
		//		return node;
		//	}
		//	return narrowestEnclosingNode.GetNarrowestEnclosingNode (position);
		//}

		public static SyntaxNode RemoveTrivias(this SyntaxNode node)
		{
			return new TriviasRemover ().Visit (node);
		}

		public static IEnumerable<TextChange> ToRoslynTextChanges(this INormalizedTextChangeCollection changes)
		{
			return changes.Select (change => new TextChange (new TextSpan (change.OldSpan.Start, change.OldSpan.Length), change.NewText));
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
	
		#endregion
		
		#region Helpers
		
		private class TriviasRemover : SyntaxRewriter
		{
			public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia)
			{
				//if (trivia.Kind == SyntaxKind.WhitespaceTrivia)
				//{
				//	return base.VisitTrivia (trivia);
				//}
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
