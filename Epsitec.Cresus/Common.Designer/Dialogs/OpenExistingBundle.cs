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
			this.Items = this.GetAvailableNames ();
			this.bundle_spec.Changed += new EventHandler (this.HandleBundleSpecChanged);
		}
		
		
		public string[] GetAvailableNames()
		{
			string   filter = this.bundle_spec.Prefix + "*";
			string[] array  = Support.Resources.GetBundleIds (filter, this.bundle_spec.ResourceLevel, this.bundle_spec.CultureInfo);
			return array;
		}
		
		private void HandleBundleSpecChanged(object sender)
		{
			this.Items = this.GetAvailableNames ();
		}
		
		
		protected override double				ExtraHeight
		{
			get { return this.bundle_spec.ExtraHeight; }
		}
		
		public override string[]				CommandArgs
		{
			get
			{
				string[] values = new string[2];
				
				values[0] = this.bundle_spec.Prefix + this.list.Items[this.list.SelectedIndex];
				values[1] = this.bundle_spec.ResourceLevel.ToString ();
				
				return values;
			}
		}
		
		
		protected override void AddExtraWidgets(Widget body)
		{
			this.bundle_spec.AddExtraWidgets (body);
		}
		
		
		protected SubBundleSpec					bundle_spec = new SubBundleSpec ();
	}
}
