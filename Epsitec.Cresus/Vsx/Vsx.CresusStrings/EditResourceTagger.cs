using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Epsitec.VisualStudio;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;

namespace Epsitec.Cresus.Strings
{
	internal class EditResourceTagger : ITagger<SmartTag>, IDisposable
	{
		public EditResourceTagger(ITextBuffer textBuffer, ITextView textView, EditResourceTaggerProvider provider)
		{
			using (new TimeTrace ())
			{
				this.textBuffer = textBuffer;
				this.textView = textView;
				this.provider = provider;

				this.textView.LayoutChanged += this.OnLayoutChanged;
				this.textView.Caret.PositionChanged += this.OnCaretPositionChanged;
			}
		}


		#region ITagger<EditResourceSmartTag> Members

		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		public IEnumerable<ITagSpan<SmartTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			if (this.symbolInfo != null)
			{
				foreach (var span in spans)
				{
					if (span.Span == this.symbolInfo.Span)
					{
						yield return this.CreateTagSpan (this.symbolInfo);
					}
				}
			}
			else
			{
				yield break;
			}
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			this.textView.LayoutChanged -= this.OnLayoutChanged;
			this.textView.Caret.PositionChanged -= this.OnCaretPositionChanged;
		}

		#endregion


		private static SnapshotSpan? CreateSymbolSpan(ResourceSymbolInfo symbolInfo, ITextSnapshot snapshot)
		{
			if (symbolInfo != null)
			{
				var textSpan = symbolInfo.SyntaxNode.Span;
				var span = Span.FromBounds (textSpan.Start, textSpan.End);
				return new SnapshotSpan (snapshot, span);
			}
			return null;
		}

		private Epsitec.VisualStudio.ResourceSymbolInfoProvider ResourceSymbolInfoProvider
		{
			get
			{
				return this.provider.ResourceSymbolInfoProvider;
			}
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
			using (new TimeTrace ())
			{
				var cts = new CancellationTokenSource (Config.MaxSmartTagDelay);
				try
				{
					this.ProcessTagAsync (point, cts.Token).Wait (cts.Token);
				}
				catch (OperationCanceledException)
				{
				}
			}
		}

		private async Task ProcessTagAsync(SnapshotPoint point, CancellationToken cancellationToken)
		{
			var newSymbolInfo = await this.CreateSymbolInfoAsync (point, cancellationToken);
			if (newSymbolInfo != this.symbolInfo)
			{
				this.RemoveCurrentTag (point.Snapshot);
				this.SetCurrentTag (newSymbolInfo, point.Snapshot);
			}
		}

		private void RemoveCurrentTag(ITextSnapshot snapshot)
		{
			if (this.symbolInfo != null)
			{
				Debug.Assert(snapshot == this.symbolInfo.Snapshot);
				var removeSpan = this.symbolInfo.SnapshotSpan;
				this.symbolInfo = null;
				this.RaiseTagsChanged (removeSpan);
			}
		}

		private void SetCurrentTag(ResourceSymbolInfo newSymbolInfo, ITextSnapshot snapshot)
		{
			this.symbolInfo = newSymbolInfo;
			if (this.symbolInfo != null)
			{
				Debug.Assert (snapshot == this.symbolInfo.Snapshot);
				var addSpan = this.symbolInfo.SnapshotSpan;
				this.RaiseTagsChanged (addSpan);
			}
		}

		private async Task<ResourceSymbolInfo> CreateSymbolInfoAsync(SnapshotPoint point, CancellationToken cancellationToken)
		{
			this.ResourceSymbolInfoProvider.ActiveDocumentSource.TextBuffer = this.textBuffer;
			return await this.ResourceSymbolInfoProvider.GetResourceSymbolInfoAsync (point, cancellationToken).ConfigureAwait (false);
		}

		private TagSpan<SmartTag> CreateTagSpan(ResourceSymbolInfo symbolInfo)
		{
			SnapshotSpan span = symbolInfo.SnapshotSpan;
			return new TagSpan<SmartTag> (span, new SmartTag (SmartTagType.Factoid, EditResourceTagger.GetSmartTagActions (symbolInfo)));
		}

		private static ReadOnlyCollection<SmartTagActionSet> GetSmartTagActions(ResourceSymbolInfo symbolInfo)
		{
			var actions = new ReadOnlyCollection<ISmartTagAction> (EditResourceTagger.EnumerateSmartTagActions (symbolInfo).ToList ());
			return new ReadOnlyCollection<SmartTagActionSet> (new SmartTagActionSet[] { new SmartTagActionSet (actions) });
		}

		private static IEnumerable<ISmartTagAction> EnumerateSmartTagActions(ResourceSymbolInfo symbolInfo)
		{
			var resources = symbolInfo.Resources;
			var count = resources.Count;
			if (count > 0)
			{
				if (count == 1)
				{
					yield return new EditResourceSmartTagAction (resources.First(), Config.EditResourceSmartTagMenu);
				}
				else
				{
					foreach (var item in resources.OrderBy (map => map.SymbolName()))
					{
						yield return new EditResourceSmartTagAction (item, Config.GetEditResourceSmartTagMenu(item.SymbolName()));
					}
				}
			}
		}

		private void RaiseTagsChanged(SnapshotSpan span)
		{
			var handler = this.TagsChanged;
			if (handler != null)
			{
				handler (this, new SnapshotSpanEventArgs (span));
			}
		}


		private readonly EditResourceTaggerProvider provider;
		private readonly ITextView textView;
		private readonly ITextBuffer textBuffer;

		private ResourceSymbolInfo symbolInfo;
	}
}
