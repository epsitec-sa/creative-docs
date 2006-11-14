//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.UI.Panels
{
	internal class EditPanel : Panel
	{
		public EditPanel(Panel owner)
		{
			this.owner = owner;
		}

		public override Panel Owner
		{
			get
			{
				return this.owner;
			}
		}

		private Panel owner;
	}
}
