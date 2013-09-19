using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Epsitec.Cresus.Strings
{
	/// <summary>
	/// One QuickInfoController per document
	/// </summary>
	internal class QuickInfoController : IIntellisenseController
	{
		internal QuickInfoController(ITextView textView, IList<ITextBuffer> subjectBuffers, QuickInfoControllerProvider provider)
		{
			using (new TimeTrace ())
			{
				this.textView = textView;
				this.subjectBuffers = subjectBuffers;
				this.provider = provider;

				this.textView.MouseHover += this.OnTextViewMouseHover;
			}
		}


		#region IIntellisenseController Members
		
		public void Detach(ITextView textView)
		{
			if (this.textView == textView)
			{
				this.textView.MouseHover -= this.OnTextViewMouseHover;
				this.textView = null;
			}
		}

		public void ConnectSubjectBuffer(ITextBuffer subjectBuffer)
		{
		}

		public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer)
		{
		}

		#endregion

		private void OnTextViewMouseHover(object sender, MouseHoverEventArgs e)
		{
			//find the mouse position by mapping down to the subject buffer
			SnapshotPoint? point = this.textView.BufferGraph.MapDownToFirstMatch
			(
				new SnapshotPoint (this.textView.TextSnapshot, e.Position),
				PointTrackingMode.Positive,
				snapshot => this.subjectBuffers.Contains (snapshot.TextBuffer),
				PositionAffinity.Predecessor
			);

			if (point != null)
			{
				ITrackingPoint triggerPoint = point.Value.Snapshot.CreateTrackingPoint (point.Value.Position, PointTrackingMode.Positive);

				if (!this.provider.QuickInfoBroker.IsQuickInfoActive (this.textView))
				{
					this.session = this.provider.QuickInfoBroker.TriggerQuickInfo (this.textView, triggerPoint, true);
				}
			}
		}

		private ITextView textView;
		private IList<ITextBuffer> subjectBuffers;
		private QuickInfoControllerProvider provider;
		private IQuickInfoSession session;
	}
}
