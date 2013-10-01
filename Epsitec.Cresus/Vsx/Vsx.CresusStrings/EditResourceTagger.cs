using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Epsitec.Tools;
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

		private Epsitec.VisualStudio.EngineSource EngineSource
		{
			get
			{
				return this.provider.EngineSource;
			}
		}

		private CresusDesigner CresusDesigner
		{
			get
			{
				return this.EngineSource.CresusDesigner;
			}
		}

		//private async Task<Epsitec.VisualStudio.Engine> EngineAsync(CancellationToken cancellationToken)
		//{
		//	return await this.EngineSource.EngineAsync (cancellationToken);
		//}


		private void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e)
		{
			this.ProcessTag (e.NewPosition.BufferPosition);
		}

		private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
		{
			if (this.symbolInfo != null)
			{
				var removeSpan = this.symbolInfo.SnapshotSpan;
				this.symbolInfo = null;
				this.RaiseTagsChanged (removeSpan);
			}
			this.ProcessTag (this.textView.Caret.Position.BufferPosition);
		}

		private void ProcessTag(SnapshotPoint point)
		{
			using (new TimeTrace ())
			{
				var cts = new CancellationTokenSource (Config.MaxAsyncDelay);
				try
				{
					var task = this.CreateSymbolInfoAsync (point, cts.Token);
					task.Wait (cts.Token);
					var newSymbolInfo = task.Result;
					if (newSymbolInfo != this.symbolInfo)
					{
						this.RemoveCurrentTag (point.Snapshot);
						this.SetCurrentTag (newSymbolInfo, point.Snapshot);
					}
				}
				catch (OperationCanceledException)
				{
				}
			}
		}

		private void RemoveCurrentTag(ITextSnapshot snapshot)
		{
			if (this.symbolInfo != null)
			{
				Debug.Assert (snapshot == this.symbolInfo.Snapshot);
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
			await this.EngineSource.ActiveTextBufferAsync (this.textBuffer, cancellationToken).ConfigureAwait (false);
			return await this.EngineSource.GetResourceSymbolInfoAsync (point, cancellationToken).ConfigureAwait(false);
		}

		private TagSpan<SmartTag> CreateTagSpan(ResourceSymbolInfo symbolInfo)
		{
			SnapshotSpan span = symbolInfo.SnapshotSpan;
			var actions = EditResourceTagger.GetSmartTagActions (this.CresusDesigner, symbolInfo);
			var smartTag = new SmartTag (SmartTagType.Factoid, actions);
			return new TagSpan<SmartTag> (span, smartTag);
		}

		private static ReadOnlyCollection<SmartTagActionSet> GetSmartTagActions(CresusDesigner cresusDesigner, ResourceSymbolInfo symbolInfo)
		{
			var actions = new ReadOnlyCollection<ISmartTagAction> (EditResourceTagger.EnumerateSmartTagActions (cresusDesigner, symbolInfo).ToList ());
			return new ReadOnlyCollection<SmartTagActionSet> (new SmartTagActionSet[] { new SmartTagActionSet (actions) });
		}

		private static IEnumerable<ISmartTagAction> EnumerateSmartTagActions(CresusDesigner cresusDesigner, ResourceSymbolInfo symbolInfo)
		{
			var resources = symbolInfo.Resources;
			var count = resources.Count;
			if (count > 0)
			{
				if (count == 1)
				{
					yield return new EditResourceSmartTagAction (cresusDesigner, resources.First(), Config.EditResourceSmartTagMenu);
				}
				else
				{
					foreach (var item in resources.OrderBy (item => item.SymbolName))
					{
						yield return new EditResourceSmartTagAction (cresusDesigner, item, Config.GetEditResourceSmartTagMenu (item.SymbolName));
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
