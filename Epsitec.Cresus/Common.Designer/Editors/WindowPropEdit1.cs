//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Editors
{
	/// <summary>
	/// Summary description for WindowPropEdit1.
	/// </summary>
	public class WindowPropEdit1 : AbstractPropEdit
	{
		public WindowPropEdit1(Application application) : base (application)
		{
		}
		
		protected override void FillTabPage()
		{
			this.AddPropPane (this.CreatePropPane ("Name", new Common.UI.Adapters.StringAdapter ()));
			this.AddPropPane (this.CreatePropPane ("Text", new UI.TextRefAdapter (this.application)));
			this.AddPropPane (this.CreatePropPane ("Size", new Common.UI.Adapters.Num2Adapter ()));
			this.AddPropPane (this.CreatePropPane ("WindowType",   new Common.UI.Adapters.EnumAdapter (new Types.EnumType (typeof (WindowType)))));
			this.AddPropPane (this.CreatePropPane ("WindowStyles", new Common.UI.Adapters.FlagsAdapter (new Types.EnumType (typeof (WindowStyles)))));
			
			this.TabPage.TabTitle = "Attributes";
		}
	}
}
