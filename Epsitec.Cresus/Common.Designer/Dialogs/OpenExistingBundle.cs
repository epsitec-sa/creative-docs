//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using System.Globalization;

namespace Epsitec.Common.Designer.Dialogs
{
	/// <summary>
	/// Summary description for SelectExistingBundle.
	/// </summary>
	public class OpenExistingBundle : Epsitec.Common.Dialogs.SelectFromList
	{
		public OpenExistingBundle(string command_template, CommandDispatcher command_dispatcher)
			: base ("Ouvrir", "Ressource à ouvrir :", null, command_template, command_dispatcher)
		{
			this.bundle_spec = new SubBundleSpec ();
			this.bundle_spec.Changed += new EventHandler (this.HandleBundleSpecChanged);
		}
		
		
		public void UpdateListContents()
		{
			this.Items = this.GetAvailableNames ();
		}
		
		public string[] GetAvailableNames()
		{
			string   name_filter = Resources.MakeFullName (this.bundle_spec.Prefix, "*");
			string[] array       = Support.Resources.GetBundleIds (name_filter, this.bundle_spec.TypeFilter, this.bundle_spec.ResourceLevel, this.bundle_spec.CultureInfo);
			return array;
		}
		
		
		protected override double				ExtraHeight
		{
			get { return this.bundle_spec.ExtraHeight; }
		}
		
		public override string[]				CommandArgs
		{
			get
			{
				string[] values = new string[3];
				
				values[0] = Resources.MakeFullName (this.bundle_spec.Prefix, this.list.Items[this.list.SelectedIndex]);
				values[1] = this.bundle_spec.ResourceLevel.ToString ();
				values[2] = this.bundle_spec.CultureInfo.TwoLetterISOLanguageName;
				
				return values;
			}
		}
		
		public SubBundleSpec					SubBundleSpec
		{
			get
			{
				return this.bundle_spec;
			}
		}
		
		
		protected override Widget CreateBodyWidget()
		{
			Widget widget = base.CreateBodyWidget ();
			
			Support.ValidationRule rule = new ValidationRule ("name");
			
			rule.AddValidator (new Epsitec.Common.Widgets.Validators.SelectionValidator (this.list));
			rule.AddCommandState ("ValidateDialog");
			
			this.private_dispatcher.ValidationRule.AddValidator (rule);
			
			return widget;
		}

		protected override void AddExtraWidgets(Widget body)
		{
			this.bundle_spec.AddExtraWidgets (body);
		}
		
		
		private void HandleBundleSpecChanged(object sender)
		{
			this.Items = this.GetAvailableNames ();
		}
		
		
		protected SubBundleSpec					bundle_spec;
	}
}
