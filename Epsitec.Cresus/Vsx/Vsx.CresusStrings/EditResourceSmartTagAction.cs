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
	internal class EditResourceSmartTagAction : ISmartTagAction
	{
		public EditResourceSmartTagAction(ITrackingSpan span)
		{
			m_span = span;
			m_snapshot = span.TextBuffer.CurrentSnapshot;
			m_upper = span.GetText (m_snapshot).ToUpper ();
			m_display = "Convert to upper case";
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
			m_span.TextBuffer.Replace (m_span.GetSpan (m_snapshot), m_upper);
		}

		#endregion
	
		private ITrackingSpan m_span;
		private string m_upper;
		private string m_display;
		private ITextSnapshot m_snapshot;
	}
}
