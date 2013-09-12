using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace Epsitec.Cresus.Strings
{
	internal class LowerCaseSmartTagAction : ISmartTagAction
	{
		public LowerCaseSmartTagAction(ITrackingSpan span)
		{
			m_span = span;
			m_snapshot = span.TextBuffer.CurrentSnapshot;
			m_lower = span.GetText (m_snapshot).ToLower ();
			m_display = "Convert to lower case";
		}

		//public ISmartTagSource Source
		//{
		//	get;
		//	private set;
		//}

		#region ISmartTagAction Members

		public ReadOnlyCollection<SmartTagActionSet> ActionSets
		{
			get
			{
				return null;
			}
		}

		public string DisplayText
		{
			get
			{
				return m_display;
			}
		}

		public ImageSource Icon
		{
			get
			{
				return null;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return true;
			}
		}

		public void Invoke()
		{
			m_span.TextBuffer.Replace (m_span.GetSpan (m_snapshot), m_lower);
		}

		#endregion
	
		private ITrackingSpan m_span;
		private string m_lower;
		private string m_display;
		private ITextSnapshot m_snapshot;
	}
}
