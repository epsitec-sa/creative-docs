using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Summary description for BundleName.
	/// </summary>
	public class BundleName : Epsitec.Common.Dialogs.AbstractOkCancel
	{
		public BundleName(string command_template, CommandDispatcher command_dispatcher) : base ("Nom de la ressource", command_template, command_dispatcher)
		{
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
			
			this.AddValueWidget ("name", this.text);
			
			Support.ValidationRule rule = new ValidationRule ("name");
			
			rule.Validators.Add (new Widgets.Validators.RegexValidator (this.text, @"\A\w(\w|[_])*\z"));
			rule.CommandStates.Add (CommandState.Find ("ValidateDialog", this.private_dispatcher));
			
			this.private_dispatcher.Validators.Add (rule);
			
			return body;
		}
		
		
		protected TextField						text;
	}
}
