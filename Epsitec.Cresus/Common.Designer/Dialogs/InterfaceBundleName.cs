//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using System.Globalization;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// La classe InterfaceBundleName présente un dialogue permettant de saisir le nom
	/// d'un bundle à créer pour réaliser un élément d'interface graphique.
	/// </summary>
	public class InterfaceBundleName : Epsitec.Common.Dialogs.AbstractOkCancel
	{
		public InterfaceBundleName(string command_template, CommandDispatcher command_dispatcher) : base (title, command_template, command_dispatcher)
		{
		}
		
		
		public override string[]				CommandArgs
		{
			get
			{
				string[] values = new string[5];
				
				values[0] = this.bundle_spec.Prefix;
				values[1] = this.text.Text;
				values[2] = this.bundle_spec.ResourceLevel.ToString ();
				values[3] = this.bundle_spec.CultureInfo.TwoLetterISOLanguageName;
				values[4] = Common.UI.InterfaceType.DialogWindow.ToString ();
				
				return values;
			}
		}
		
		
		protected override Widget CreateBodyWidget()
		{
			Widget body  = new Widget ();
			double extra = this.bundle_spec.ExtraHeight;
			
			body.Size = new Drawing.Size (320, 48 + extra);
			
			StaticText label;
			
			label        = new StaticText (body);
			label.Bounds = new Drawing.Rectangle (0, body.Height - 21, 110, 21);
			label.Text   = "Nom de la ressource : ";
			
			this.text        = new TextField (body);
			this.text.Bounds = new Drawing.Rectangle (label.Right + 8, label.Bottom - 1, body.Width - label.Right - 8, label.Height);
			this.text.TabIndex = 1;
			this.text.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			
			this.bundle_spec.AddExtraWidgets (body);
			
			Support.ValidationRule rule = new ValidationRule ("name");
			
			rule.AddValidator (new Epsitec.Common.Widgets.Validators.RegexValidator (this.text, Support.RegexFactory.AlphaNumName));
			rule.AddCommandState ("ValidateDialog");
			
			this.private_dispatcher.AddValidator (rule);
			
			return body;
		}
		
		
		
		private const string					title = "Nouvelle interface";	//	TODO: L10n
		
		protected TextField						text;
		protected SubBundleSpec					bundle_spec = new SubBundleSpec ();
	}
}
