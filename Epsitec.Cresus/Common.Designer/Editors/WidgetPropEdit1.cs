//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Editors
{
	/// <summary>
	/// Summary description for WidgetPropEdit1.
	/// </summary>
	public class WidgetPropEdit1 : AbstractPropEdit
	{
		public WidgetPropEdit1()
		{
		}
		
		protected override void FillTabPage()
		{
			base.FillTabPage ();
			
			this.CreatePropPane ("Test 1", 30);
			this.CreatePropPane ("Test 2", 30);
		}
	}
}
