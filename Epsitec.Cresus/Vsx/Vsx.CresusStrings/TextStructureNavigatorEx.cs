using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;

namespace Epsitec.Cresus.Strings
{
	public static class TextStructureNavigatorEx
	{
		public static IEnumerable<SnapshotSpan> GetSpanOfEnclosings(this ITextStructureNavigator navigator, SnapshotSpan startSpan)
		{
			startSpan.ThrowIfNull ();
			var span = startSpan;
			do
			{
				span = navigator.GetSpanOfEnclosing (span);
				yield return span;
			}
			while (span.Length != span.Snapshot.Length);
		}
		public static IEnumerable<SnapshotSpan> GetSpanOfFirstChilds(this ITextStructureNavigator navigator, SnapshotSpan startSpan)
		{
			startSpan.ThrowIfNull ();
			var previousSpan = default(SnapshotSpan);
			var span = startSpan;
			while(span != previousSpan)
			{
				span = navigator.GetSpanOfFirstChild (span);
				yield return span;
				previousSpan = span;
			}
		}
		public static IEnumerable<SnapshotSpan> GetSpanOfPreviousSiblings(this ITextStructureNavigator navigator, SnapshotSpan startSpan)
		{
			startSpan.ThrowIfNull ();
			var span = startSpan;
			do
			{
				span = navigator.GetSpanOfPreviousSibling (span);
				yield return span;
			}
			while (span.Length != span.Snapshot.Length);
		}
		public static IEnumerable<SnapshotSpan> GetSpanOfNextSiblings(this ITextStructureNavigator navigator, SnapshotSpan startSpan)
		{
			startSpan.ThrowIfNull ();
			var span = startSpan;
			do
			{
				span = navigator.GetSpanOfNextSibling (span);
				yield return span;
			}
			while (span.Length != span.Snapshot.Length);
		}
		public static IEnumerable<SnapshotSpan> GetSpanContext(this ITextStructureNavigator navigator, SnapshotSpan startSpan, int contextSideLength = 4)
		{

			var previous = navigator.GetSpanOfPreviousSiblings (startSpan).Take (contextSideLength).Reverse ();
			foreach (var span in previous)
			{
				yield return span;
			}
			yield return startSpan;
			var next = navigator.GetSpanOfNextSiblings (startSpan).Take (contextSideLength);
			foreach (var span in next)
			{
				yield return span;
			}
		}
	}
}
