//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using System.Globalization;

namespace Epsitec.Common.Designer.Dialogs
{
	public class SubBundleSpec
	{
		public SubBundleSpec()
		{
			this.culture = CultureInfo.CurrentUICulture;
		}
		
		
		public string							Prefix
		{
			get
			{
				return this.prefix;
			}
		}
		
		public string							TypeFilter
		{
			get
			{
				return this.type_filter;
			}
			set
			{
				if (this.type_filter != value)
				{
					this.type_filter = value;
					this.OnChanged ();
				}
			}
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
					
					if (this.container != null)
					{
						RadioButton.Activate (this.container, "level", (int) this.level);
					}
					
					this.OnChanged ();
				}
			}
		}
		
		public CultureInfo						CultureInfo
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
					
					if (this.lang_id != null)
					{
						this.lang_id.SelectedName = this.CultureInfo.TwoLetterISOLanguageName;
					}
					
					this.OnChanged ();
				}
			}
		}
		
		public double							ExtraHeight
		{
			get
			{
				return this.detailed_choice ? 45 : 0;
			}
		}
		
		public bool								DetailedChoice
		{
			get
			{
				return this.detailed_choice;
			}
			set
			{
				this.detailed_choice = value;
			}
		}
		
		
		public void AddExtraWidgets(Widget container)
		{
			System.Diagnostics.Debug.Assert (this.container == null);
			System.Diagnostics.Debug.Assert (container != null);
			
			this.container = container;
			
			if (this.detailed_choice == false)
			{
				return;
			}
			
			RadioButton radio;
			
			radio = new RadioButton (container);
			
			double h = radio.DefaultHeight;
			
			radio.Text = "version par <m>d</m>éfaut";
			radio.Bounds = new Drawing.Rectangle (0, h*2, 140, h);
			radio.Group  = "level";
			radio.Index  = (int) ResourceLevel.Default;
			radio.TabIndex = 10;
			radio.ActiveStateChanged += new EventHandler (this.HandleRadioActiveStateChanged);
			
			radio = new RadioButton (container);
			radio.Text = "version <m>t</m>raduite, choix de la langue : ";
			radio.Bounds = new Drawing.Rectangle (0, h*1, 205, h);
			radio.Group  = "level";
			radio.Index  = (int) ResourceLevel.Localised;
			radio.TabIndex = 10;
			radio.ActiveStateChanged += new EventHandler (this.HandleRadioActiveStateChanged);
			
			this.lang_id = new TextFieldCombo (container);
			this.lang_id.Bounds = new Drawing.Rectangle (radio.Right + 4, radio.Bottom - 4, container.Width - radio.Right - 4, this.lang_id.DefaultHeight);
			this.lang_id.TabIndex = 11;
			this.lang_id.Items.Add ("de", "allemand");
			this.lang_id.Items.Add ("en", "anglais");
			this.lang_id.Items.Add ("it", "italien");
			this.lang_id.Items.Add ("fr", "français");
			this.lang_id.IsReadOnly = true;
			this.lang_id.SelectedName = this.CultureInfo.TwoLetterISOLanguageName;
			this.lang_id.SelectedIndexChanged += new Support.EventHandler (this.HandleLangIdSelectedIndexChanged);
			this.lang_id.Focused += new Support.EventHandler (this.HandleLangIdFocused);
			
			radio = new RadioButton (container);
			radio.Text = "version <m>p</m>ersonnalisée";
			radio.Bounds = new Drawing.Rectangle (0, h*0, 140, h);
			radio.Group  = "level";
			radio.Index  = (int) ResourceLevel.Customised;
			radio.TabIndex = 10;
			radio.ActiveStateChanged += new EventHandler (this.HandleRadioActiveStateChanged);
			
			RadioButton.Activate (container, "level", (int) this.level);
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
			System.Diagnostics.Debug.Assert (this.lang_id == sender);
			
			int    index = this.lang_id.SelectedIndex;
			string name  = this.lang_id.Items.GetName (index);
			
			this.CultureInfo = Resources.FindCultureInfo (name);
		}
		
		private void HandleLangIdFocused(object sender)
		{
			System.Diagnostics.Debug.Assert (this.lang_id == sender);
			
			RadioButton radio = RadioButton.FindRadio (this.lang_id.Parent, "level", (int) ResourceLevel.Localised);
			radio.ActiveState = WidgetState.ActiveYes;
		}
		
		
		protected virtual void OnChanged()
		{
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}
		
		
		public event Support.EventHandler		Changed;
		
		
		protected Widget						container;
		protected TextFieldCombo				lang_id;
		
		protected bool							detailed_choice = false;
		protected string						type_filter;
		protected string						prefix = "file";
		protected CultureInfo					culture;
		protected ResourceLevel					level = ResourceLevel.Default;
	}
}
