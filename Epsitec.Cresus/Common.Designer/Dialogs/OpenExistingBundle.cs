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
			this.culture = CultureInfo.CurrentUICulture;
			this.Items = this.GetAvailableNames ();
		}
		
		
		public string[] GetAvailableNames()
		{
			string   filter = this.prefix + "*";
			string[] array  = Support.Resources.GetBundleIds (filter, this.ResourceLevel, this.CultureInfo);
			return array;
		}
		
		
		public ResourceLevel					ResourceLevel
		{
			get
			{
				return this.level;
			}
			set
			{
				if (this.level != value)
				{
					this.level = value;
					
					if (this.window != null)
					{
						RadioButton.Activate (this.window.Root, "level", (int) this.level);
						this.Items = this.GetAvailableNames ();
					}
				}
			}
		}
		
		protected virtual CultureInfo			CultureInfo
		{
			get
			{
				return this.culture;
			}
			set
			{
				if (this.culture != value)
				{
					this.culture = value;
					this.Items = this.GetAvailableNames ();
					this.lang_id.SelectedName = this.CultureInfo.TwoLetterISOLanguageName;
				}
			}
		}
		
		protected override double				ExtraHeight
		{
			get { return 45; }
		}
		
		public override string[]				CommandArgs
		{
			get
			{
				string[] values = new string[2];
				
				values[0] = this.prefix + this.list.Items[this.list.SelectedIndex];
				values[1] = this.level.ToString ();
				
				return values;
			}
		}
		
		
		protected override void AddExtraWidgets(Widget body)
		{
			RadioButton radio;
			
			radio = new RadioButton (body);
			
			double h = radio.DefaultHeight;
			
			radio.Text = "version par <m>d</m>éfaut";
			radio.Bounds = new Drawing.Rectangle (0, h*2, 140, h);
			radio.Group  = "level";
			radio.Index  = (int) ResourceLevel.Default;
			radio.TabIndex = 10;
			radio.ActiveStateChanged += new EventHandler (this.HandleRadioActiveStateChanged);
			
			radio = new RadioButton (body);
			radio.Text = "version <m>t</m>raduite, choix de la langue : ";
			radio.Bounds = new Drawing.Rectangle (0, h*1, 205, h);
			radio.Group  = "level";
			radio.Index  = (int) ResourceLevel.Localised;
			radio.TabIndex = 10;
			radio.ActiveStateChanged += new EventHandler (this.HandleRadioActiveStateChanged);
			
			this.lang_id = new TextFieldCombo (body);
			this.lang_id.Bounds = new Drawing.Rectangle (radio.Right + 4, radio.Bottom - 4, body.Width - radio.Right - 4, this.lang_id.DefaultHeight);
			this.lang_id.TabIndex = 11;
			this.lang_id.Items.Add ("de", "allemand");
			this.lang_id.Items.Add ("en", "anglais");
			this.lang_id.Items.Add ("it", "italien");
			this.lang_id.Items.Add ("fr", "français");
			this.lang_id.IsReadOnly = true;
			this.lang_id.SelectedName = this.CultureInfo.TwoLetterISOLanguageName;
			this.lang_id.SelectedIndexChanged += new Support.EventHandler (this.HandleLangIdSelectedIndexChanged);
			this.lang_id.Focused += new Support.EventHandler (this.HandleLangIdFocused);
			
			radio = new RadioButton (body);
			radio.Text = "version <m>p</m>ersonnalisée";
			radio.Bounds = new Drawing.Rectangle (0, h*0, 140, h);
			radio.Group  = "level";
			radio.Index  = (int) ResourceLevel.Customised;
			radio.TabIndex = 10;
			radio.ActiveStateChanged += new EventHandler (this.HandleRadioActiveStateChanged);
			
			RadioButton.Activate (body, "level", (int) this.level);
		}
		
		
		private void HandleRadioActiveStateChanged(object sender)
		{
			RadioButton radio = sender as RadioButton;
			
			if ((radio.ActiveState == WidgetState.ActiveYes) &&
				(radio.Group == "level"))
			{
				ResourceLevel level = (ResourceLevel) radio.Index;
				this.ResourceLevel = level;
			}
		}
		
		private void HandleLangIdSelectedIndexChanged(object sender)
		{
			TextFieldCombo list = sender as TextFieldCombo;
			
			int    index = list.SelectedIndex;
			string name  = list.Items.GetName (index);
			
			this.CultureInfo = this.FindCultureInfo (name);
		}
		
		private void HandleLangIdFocused(object sender)
		{
			TextFieldCombo list = sender as TextFieldCombo;
			RadioButton radio = RadioButton.FindRadio (list.Parent, "level", (int) ResourceLevel.Localised);
			radio.ActiveState = WidgetState.ActiveYes;
		}
		
		private CultureInfo FindCultureInfo(string two_letter_code)
		{
			CultureInfo[] cultures = CultureInfo.GetCultures (System.Globalization.CultureTypes.NeutralCultures);
			
			for (int i = 0; i < cultures.Length; i++)
			{
				if (cultures[i].TwoLetterISOLanguageName == two_letter_code)
				{
					return cultures[i];
				}
			}
			return null;
		}
		
		protected string						prefix = "file:";
		protected CultureInfo					culture = null;
		protected TextFieldCombo				lang_id;
		protected ResourceLevel					level = ResourceLevel.Default;
	}
}
