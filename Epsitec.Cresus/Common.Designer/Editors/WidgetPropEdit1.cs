//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Editors
{
	/// <summary>
	/// Summary description for WidgetPropEdit1.
	/// </summary>
	public class WidgetPropEdit1 : AbstractPropEdit
	{
		public WidgetPropEdit1(Application application) : base (application)
		{
		}
		
		protected override void FillTabPage()
		{
			this.AddPropPane (this.CreatePropPane ("Name",		new Common.UI.Adapters.StringAdapter ()));
			this.AddPropPane (this.CreatePropPane ("Text",		new UI.TextRefAdapter (this.Application)));
			this.AddPropPane (this.CreatePropPane ("Command",	new UI.CommandAdapter (this.Application)));
			this.AddPropPane (this.CreatePropPane ("Location",	new Common.UI.Adapters.Num2Adapter ()));
			this.AddPropPane (this.CreatePropPane ("Size",		new Common.UI.Adapters.Num2Adapter ()));
			this.AddPropPane (this.CreatePropPane ("TabIndex",	new Common.UI.Adapters.StringAdapter ()));
			this.AddPropPane (this.CreatePropPane ("Group",		new Common.UI.Adapters.StringAdapter ()));
//@			this.AddPropPane (this.CreatePropPane ("Layout",	new Common.UI.Adapters.LayoutAdapter ()));
			
			this.TabPage.TabTitle = "Attributes";
		}
	}
}
