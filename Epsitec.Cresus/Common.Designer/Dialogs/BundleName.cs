using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Summary description for BundleName.
	/// </summary>
	public class BundleName : AbstractOkCancel
	{
		public BundleName(string command_template, CommandDispatcher command_dispatcher) : base (command_template, command_dispatcher)
		{
		}
		
		public override string[]				CommandArgs
		{
			get
			{
				return new string[1] { this.text.Text };
			}
		}
		
		
		protected override Widget CreateBodyWidget()
		{
			Widget body = new Widget ();
			
			body.Size = new Drawing.Size (320, 80);
			
			StaticText label;
			
			label        = new StaticText (body);
			label.Bounds = new Drawing.Rectangle (0, body.Height - 21, 110, 21);
			label.Text   = "Nom de la ressource : ";
			
			this.text        = new TextField (body);
			this.text.Bounds = new Drawing.Rectangle (label.Right + 8, label.Bottom - 1, body.Width - label.Right - 8, label.Height);
			this.text.TabIndex = 1;
			this.text.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			return body;
		}
		
		protected TextField						text;
	}
}
