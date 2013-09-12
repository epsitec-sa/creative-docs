using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	internal class TestSmartTagger : ITagger<TestSmartTag>, IDisposable
	{
		public TestSmartTagger(ITextBuffer buffer, ITextView view, TestSmartTaggerProvider provider)
		{
			m_buffer = buffer;
			m_view = view;
			m_provider = provider;
			m_view.LayoutChanged += OnLayoutChanged;
		}

		public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

		public IEnumerable<ITagSpan<TestSmartTag>> GetTags(NormalizedSnapshotSpanCollection spans)
		{
			ITextSnapshot snapshot = m_buffer.CurrentSnapshot;
			if (snapshot.Length == 0)
				yield break; //don't do anything if the buffer is empty 

			//set up the navigator
			ITextStructureNavigator navigator = m_provider.NavigatorService.GetTextStructureNavigator (m_buffer);

			foreach (var span in spans)
			{
				ITextCaret caret = m_view.Caret;
				SnapshotPoint point;

				if (caret.Position.BufferPosition > 0)
					point = caret.Position.BufferPosition - 1;
				else
					yield break;

				TextExtent extent = navigator.GetExtentOfWord (point);
				//don't display the tag if the extent has whitespace 
				if (extent.IsSignificant)
					yield return new TagSpan<TestSmartTag> (extent.Span, new TestSmartTag (GetSmartTagActions (extent.Span)));
				else
					yield break;
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			Dispose (true);
			GC.SuppressFinalize (this);
		}

		#endregion

		private void Dispose(bool disposing)
		{
			if (!this.m_disposed)
			{
				if (disposing)
				{
					m_view.LayoutChanged -= OnLayoutChanged;
					m_view = null;
				}

				m_disposed = true;
			}
		}

		private ReadOnlyCollection<SmartTagActionSet> GetSmartTagActions(SnapshotSpan span)
		{
			List<SmartTagActionSet> actionSetList = new List<SmartTagActionSet> ();
			List<ISmartTagAction> actionList = new List<ISmartTagAction> ();

			ITrackingSpan trackingSpan = span.Snapshot.CreateTrackingSpan (span, SpanTrackingMode.EdgeInclusive);
			actionList.Add (new UpperCaseSmartTagAction (trackingSpan));
			actionList.Add (new LowerCaseSmartTagAction (trackingSpan));
			SmartTagActionSet actionSet = new SmartTagActionSet (actionList.AsReadOnly ());
			actionSetList.Add (actionSet);
			return actionSetList.AsReadOnly ();
		}

		private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
		{
			ITextSnapshot snapshot = e.NewSnapshot;
			//don't do anything if this is just a change in case 
			if (!snapshot.GetText ().ToLower ().Equals (e.OldSnapshot.GetText ().ToLower ()))
			{
				SnapshotSpan span = new SnapshotSpan (snapshot, new Span (0, snapshot.Length));
				EventHandler<SnapshotSpanEventArgs> handler = this.TagsChanged;
				if (handler != null)
				{
					handler (this, new SnapshotSpanEventArgs (span));
				}
			}
		}
		
		private ITextBuffer m_buffer;
		private ITextView m_view;
		private TestSmartTaggerProvider m_provider;
		private bool m_disposed;
	}
}
