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
			
			this.CreatePropPane ("Name",		new UI.Adapters.StringAdapter ());
			this.CreatePropPane ("Text",		new UI.Adapters.StringAdapter ());
			this.CreatePropPane ("Command",		new UI.Adapters.StringAdapter ());
			this.CreatePropPane ("Location",	new UI.Adapters.Num2Adapter ());
			this.CreatePropPane ("Size",		new UI.Adapters.Num2Adapter ());
			this.CreatePropPane ("TabIndex",	new UI.Adapters.StringAdapter ());
			this.CreatePropPane ("Group",		new UI.Adapters.StringAdapter ());
		}
	}
}
