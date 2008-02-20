//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (Panel))]

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>Panel</c> class is used as the (local) root in a widget tree
	/// built by the dynamic user interface designer.
	/// </summary>
	public class PanelActivationEventArgs : Support.CancelEventArgs
	{
		public PanelActivationEventArgs(Panel panel, PanelStack stack, string focusWidgetName)
		{
			this.panel = panel;
			this.stack = stack;
			this.focusWidgetName = focusWidgetName;
		}


		public Panel Panel
		{
			get
			{
				return this.panel;
			}
		}

		public PanelStack PanelStack
		{
			get
			{
				return this.stack;
			}
		}

		public string FocusWidgetName
		{
			get
			{
				return this.focusWidgetName;
			}
		}

		private Panel panel;
		private PanelStack stack;
		private string focusWidgetName;
	}
}
