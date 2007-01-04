//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.UI.Panels;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (SearchPanel))]

namespace Epsitec.Common.UI.Panels
{
	internal class SearchPanel : EditPanel
	{
		public SearchPanel()
		{
		}

		public SearchPanel(Panel owner)
			: base (owner)
		{
		}

		public override PanelMode PanelMode
		{
			get
			{
				return PanelMode.Search;
			}
		}
	}
}
