//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Editors
{
	/// <summary>
	/// Summary description for DataWidgetPropEdit1.
	/// </summary>
	public class DataWidgetPropEdit1 : AbstractPropEdit
	{
		public DataWidgetPropEdit1(Application application) : base (application)
		{
		}
		
		protected override void FillTabPage()
		{
			this.AddPropPane (this.CreatePropPane ("Name",		     new Common.UI.Adapters.StringAdapter ()));
			this.AddPropPane (this.CreatePropPane ("HasCaption",     new Common.UI.Adapters.BooleanAdapter ()));
			this.AddPropPane (this.CreatePropPane ("CaptionWidth",   new Common.UI.Adapters.Num1Adapter ()));
			this.AddPropPane (this.CreatePropPane ("Representation", new Common.UI.Adapters.EnumAdapter (new Types.EnumType (typeof (Common.UI.Data.Representation)))));
			this.AddPropPane (this.CreatePropPane ("Location",	     new Common.UI.Adapters.Num2Adapter ()));
			this.AddPropPane (this.CreatePropPane ("Size",		     new Common.UI.Adapters.Num2Adapter ()));
			this.AddPropPane (this.CreatePropPane ("TabIndex",	     new Common.UI.Adapters.StringAdapter ()));
			this.AddPropPane (this.CreatePropPane ("Layout",	     new Common.UI.Adapters.LayoutAdapter ()));
			
			this.TabPage.TabTitle = "Attributes";
		}
	}
}
