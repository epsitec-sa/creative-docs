using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant de choisir une catégorie de bundle.
	/// </summary>
	public class BundleType : Widget
	{
		public BundleType() : base()
		{
			Separator sep = new Separator(this);
			sep.PreferredHeight = 1;
			sep.Margins = new Margins(0, 0, 10, 0);
			sep.Dock = DockStyle.Bottom;

			this.buttonStrings = new IconButtonMark(this);
			this.buttonStrings.Text = Res.Strings.BundleType.Strings;
			this.buttonStrings.Name = "Texts";
			this.buttonStrings.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonStrings.SiteMark = SiteMark.OnBottom;
			this.buttonStrings.MarkDimension = 5;
			this.buttonStrings.PreferredWidth = 100;
			this.buttonStrings.MinHeight = 20+5;
			this.buttonStrings.AutoFocus = false;
			this.buttonStrings.Margins = new Margins(10, 0, 10, 0);
			this.buttonStrings.Dock = DockStyle.Left;
			this.buttonStrings.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonPanels = new IconButtonMark(this);
			this.buttonPanels.Text = Res.Strings.BundleType.Panels;
			this.buttonPanels.Name = "Panels";
			this.buttonPanels.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonPanels.SiteMark = SiteMark.OnBottom;
			this.buttonPanels.MarkDimension = 5;
			this.buttonPanels.PreferredWidth = 100;
			this.buttonPanels.MinHeight = 20+5;
			this.buttonPanels.AutoFocus = false;
			this.buttonPanels.Margins = new Margins(2, 0, 10, 0);
			this.buttonPanels.Dock = DockStyle.Left;
			this.buttonPanels.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonScripts = new IconButtonMark(this);
			this.buttonScripts.Text = Res.Strings.BundleType.Scripts;
			this.buttonScripts.Name = "Scripts";
			this.buttonScripts.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonScripts.SiteMark = SiteMark.OnBottom;
			this.buttonScripts.MarkDimension = 5;
			this.buttonScripts.PreferredWidth = 100;
			this.buttonScripts.MinHeight = 20+5;
			this.buttonScripts.AutoFocus = false;
			this.buttonScripts.Margins = new Margins(2, 0, 10, 0);
			this.buttonScripts.Dock = DockStyle.Left;
			this.buttonScripts.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.UpdateButtons();
		}

		public BundleType(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public string CurrentType
		{
			get
			{
				return this.currentType;
			}

			set
			{
				if (this.currentType != value)
				{
					this.currentType = value;
					this.UpdateButtons();
				}
			}
		}


		protected void UpdateButtons()
		{
			this.buttonStrings.ActiveState = (this.currentType == "Texts"  ) ? ActiveState.Yes : ActiveState.No;
			this.buttonPanels .ActiveState = (this.currentType == "Panels" ) ? ActiveState.Yes : ActiveState.No;
			this.buttonScripts.ActiveState = (this.currentType == "Scripts") ? ActiveState.Yes : ActiveState.No;
		}


		void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			IconButtonMark button = sender as IconButtonMark;
			this.CurrentType = button.Name;
		}


		protected string					currentType = "Texts";
		protected IconButtonMark			buttonStrings;
		protected IconButtonMark			buttonPanels;
		protected IconButtonMark			buttonScripts;
	}
}
