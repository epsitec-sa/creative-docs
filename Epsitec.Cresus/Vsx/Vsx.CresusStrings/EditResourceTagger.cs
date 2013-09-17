using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;

namespace Epsitec.Cresus.Strings
{
	internal class EditResourceTagger : ITagger<EditResourceSmartTag>, IDisposable
	{
		public EditResourceTagger(ITextBuffer textBuffer, ITextView textView, EditResourceTaggerProvider provider)
		{
			Trace.WriteLine ("EditResourceTagger()");
			this.subjectBuffer = textBuffer;
			this.textView = textView;
			this.provider = provider;

			//this.textView.MouseHover += this.OnTextViewMouseHover;
			this.textView.LayoutChanged += this.OnLayoutChanged;
			this.textView.Caret.PositionChanged += this.OnCaretPositionChanged;
		}

		#region ITagger<EditResourceSmartTag> Members

		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		public IEnumerable<ITagSpan<EditResourceSmartTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			if (this.symbolSpan.HasValue)
			{
				foreach (var span in spans)
				{
					if (span == this.symbolSpan.Value)
					{
						yield return this.CreateTagSpan (span);
					}
				}
			}
			else
			{
				yield break;
			}
			//ITextSnapshot snapshot = this.subjectBuffer.CurrentSnapshot;
			//if (snapshot.Length == 0)
			//	yield break; //don't do anything if the buffer is empty 

			////set up the navigator
			//ITextStructureNavigator navigator = this.provider.NavigatorService.GetTextStructureNavigator (this.subjectBuffer);

			//foreach (var span in spans)
			//{
			//	ITextCaret caret = this.textView.Caret;
			//	SnapshotPoint point;

			//	if (caret.Position.BufferPosition > 0)
			//		point = caret.Position.BufferPosition - 1;
			//	else
			//		yield break;

			//	TextExtent extent = navigator.GetExtentOfWord (point);
			//	//don't display the tag if the extent has whitespace 
			//	if (extent.IsSignificant)
			//		yield return new TagSpan<EditResourceSmartTag> (extent.Span, new EditResourceSmartTag (GetSmartTagActions (extent.Span)));
			//	else
			//		yield break;
			//}


			//if (this.tagSpan != null)
			//{
			//	yield return this.tagSpan;
			//}
		}

		#endregion


		#region IDisposable Members

		public void Dispose()
		{
			//this.textView.MouseHover -= this.OnTextViewMouseHover;
			this.textView.LayoutChanged -= this.OnLayoutChanged;
			this.textView.Caret.PositionChanged -= this.OnCaretPositionChanged;
			this.textView = null;
		}

		#endregion

		private ReadOnlyCollection<SmartTagActionSet> GetSmartTagActions(SnapshotSpan span)
		{
			List<SmartTagActionSet> actionSetList = new List<SmartTagActionSet> ();
			List<ISmartTagAction> actionList = new List<ISmartTagAction> ();

			ITrackingSpan trackingSpan = span.Snapshot.CreateTrackingSpan (span, SpanTrackingMode.EdgeInclusive);
			actionList.Add (new EditResourceSmartTagAction (trackingSpan));
			SmartTagActionSet actionSet = new SmartTagActionSet (actionList.AsReadOnly ());
			actionSetList.Add (actionSet);
			return actionSetList.AsReadOnly ();
		}

		private void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
		{
			this.ProcessTag (e.NewPosition.BufferPosition);
		}

		private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
		{
			this.ProcessTag (this.textView.Caret.Position.BufferPosition);
		}

		private void ProcessTag(SnapshotPoint point)
		{
			var newSymbolSpan = this.CreateSymbolSpan (point);
			if (newSymbolSpan.HasValue)
			{
				if (this.symbolSpan.HasValue && this.symbolSpan == newSymbolSpan)
				{
					return;
				}
			}

			this.oldSymbolSpan = this.symbolSpan;
			if (this.oldSymbolSpan.HasValue)
			{
				this.symbolSpan = null;
				this.RaiseTagsChanged (this.oldSymbolSpan.Value);
			}

			this.symbolSpan = newSymbolSpan;
			if (this.symbolSpan.HasValue)
			{
				this.RaiseTagsChanged (this.symbolSpan.Value);
			}
		}

		//private void OnTextViewMouseHover(object sender, MouseHoverEventArgs e)
		//{
		//	//find the mouse position by mapping down to the subject buffer
		//	SnapshotPoint? point = this.textView.BufferGraph.MapDownToFirstMatch
		//	(
		//		new SnapshotPoint (this.textView.TextSnapshot, e.Position),
		//		PointTrackingMode.Positive,
		//		snapshot => this.subjectBuffer == snapshot.TextBuffer,
		//		PositionAffinity.Predecessor
		//	);

		//	if (point != null)
		//	{
		//		if (this.tagSpan == null)
		//		{
		//			this.tagSpan = this.CreateTagSpan (point.Value);
		//			if (this.tagSpan != null)
		//			{
		//				this.RaiseTagsChanged (this.tagSpan.Span);
		//			}
		//		}
		//		else if (!this.tagSpan.Span.Contains (point.Value))
		//		{
		//			var span = this.tagSpan.Span;
		//			this.tagSpan = null;
		//			this.RaiseTagsChanged (span);

		//			this.tagSpan = this.CreateTagSpan (point.Value);
		//			if (this.tagSpan != null)
		//			{
		//				this.RaiseTagsChanged (this.tagSpan.Span);
		//			}
		//		}
		//	}
		//}

		//private SnapshotPoint? GetTriggerPoint(ITextSnapshot textSnapshot, ITrackingPoint triggerPoint)
		//{
		//	return this.textView.BufferGraph.MapDownToSnapshot (triggerPoint.GetPoint (this.textView.TextBuffer.CurrentSnapshot), PointTrackingMode.Negative, textSnapshot, PositionAffinity.Successor);
		//}

		private Epsitec.VisualStudio.ResourceSymbolInfoProvider ResourceSymbolInfoProvider
		{
			get
			{
				return this.provider.ResourceSymbolInfoProvider;
			}
		}

		private async Task<SnapshotSpan?> CreateSymbolSpanAsync(SnapshotPoint point)
		{
			var symbolInfo = await this.ResourceSymbolInfoProvider.GetResourceSymbolInfoAsync (point, false).ConfigureAwait (false);
			if (symbolInfo != null)
			{
				var textSpan = symbolInfo.SyntaxNode.Span;
				var span = Span.FromBounds (textSpan.Start, textSpan.End);
				return new SnapshotSpan (point.Snapshot, span);
			}
			return null;
		}

		private SnapshotSpan? CreateSymbolSpan(SnapshotPoint point)
		{
			var symbolSpanTask = this.CreateSymbolSpanAsync (point);
			if (symbolSpanTask.Wait (100))
			{
				return symbolSpanTask.Result;
			}
			return null;
		}

		private async Task<TagSpan<EditResourceSmartTag>> CreateTagSpanAsync(SnapshotPoint point)
		{
			var symbolSpan = await this.CreateSymbolSpanAsync (point).ConfigureAwait (false);
			if (symbolSpan.HasValue)
			{
				return this.CreateTagSpan (symbolSpan.Value);
			}
			return null;
		}

		private TagSpan<EditResourceSmartTag> CreateTagSpan(SnapshotSpan symbolSpan)
		{
			return new TagSpan<EditResourceSmartTag> (symbolSpan, new EditResourceSmartTag (this.GetSmartTagActions (symbolSpan)));
		}

		private TagSpan<EditResourceSmartTag> CreateTagSpan(SnapshotPoint point)
		{
			var tagSpanTask = this.CreateTagSpanAsync (point);
			if (tagSpanTask.Wait (100))
			{
				return tagSpanTask.Result;
			}
			return null;
		}

		private void RaiseTagsChanged(SnapshotSpan span)
		{
			var handler = this.TagsChanged;
			if (handler != null)
			{
				handler (this, new SnapshotSpanEventArgs (span));
			}
		}


		private ITextBuffer subjectBuffer;
		private ITextView textView;
		private EditResourceTaggerProvider provider;

		private SnapshotSpan? symbolSpan;
		private SnapshotSpan? oldSymbolSpan;
	}
}
