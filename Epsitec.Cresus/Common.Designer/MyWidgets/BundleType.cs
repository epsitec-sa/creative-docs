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
			var topSeparator = new Separator (this);
			topSeparator.PreferredHeight = 1;
			topSeparator.Margins = new Margins (0, 0, 0, 5);
			topSeparator.Dock = DockStyle.Top;

			var bottomSeparator = new Separator (this);
			bottomSeparator.PreferredHeight = 1;
			bottomSeparator.Margins = new Margins (0, 0, 0, 0);
			bottomSeparator.Dock = DockStyle.Bottom;

			double width = 75;

			this.buttonStrings = new IconButtonMark(this);
			this.buttonStrings.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Strings);
			this.buttonStrings.Name = BundleType.Convert(ResourceAccess.Type.Strings);
			this.buttonStrings.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonStrings.MarkDisposition = ButtonMarkDisposition.Below;
			this.buttonStrings.MarkLength = 5;
			this.buttonStrings.PreferredWidth = width;
			this.buttonStrings.MinHeight = 20+5;
			this.buttonStrings.AutoFocus = false;
			this.buttonStrings.Margins = new Margins(2, 0, 0, 0);
			this.buttonStrings.Dock = DockStyle.Left;
			this.buttonStrings.Clicked += this.HandleButtonClicked;

			this.buttonCaptions = new IconButtonMark(this);
			this.buttonCaptions.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Captions);
			this.buttonCaptions.Name = BundleType.Convert(ResourceAccess.Type.Captions);
			this.buttonCaptions.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonCaptions.MarkDisposition = ButtonMarkDisposition.Below;
			this.buttonCaptions.MarkLength = 5;
			this.buttonCaptions.PreferredWidth = width;
			this.buttonCaptions.MinHeight = 20+5;
			this.buttonCaptions.AutoFocus = false;
			this.buttonCaptions.Margins = new Margins(2, 0, 0, 0);
			this.buttonCaptions.Dock = DockStyle.Left;
			this.buttonCaptions.Clicked += this.HandleButtonClicked;

			this.buttonCommands = new IconButtonMark(this);
			this.buttonCommands.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Commands);
			this.buttonCommands.Name = BundleType.Convert(ResourceAccess.Type.Commands);
			this.buttonCommands.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonCommands.MarkDisposition = ButtonMarkDisposition.Below;
			this.buttonCommands.MarkLength = 5;
			this.buttonCommands.PreferredWidth = width;
			this.buttonCommands.MinHeight = 20+5;
			this.buttonCommands.AutoFocus = false;
			this.buttonCommands.Margins = new Margins(2, 0, 0, 0);
			this.buttonCommands.Dock = DockStyle.Left;
			this.buttonCommands.Clicked += this.HandleButtonClicked;

			this.buttonTypes = new IconButtonMark(this);
			this.buttonTypes.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Types);
			this.buttonTypes.Name = BundleType.Convert(ResourceAccess.Type.Types);
			this.buttonTypes.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonTypes.MarkDisposition = ButtonMarkDisposition.Below;
			this.buttonTypes.MarkLength = 5;
			this.buttonTypes.PreferredWidth = width;
			this.buttonTypes.MinHeight = 20+5;
			this.buttonTypes.AutoFocus = false;
			this.buttonTypes.Margins = new Margins(2, 0, 0, 0);
			this.buttonTypes.Dock = DockStyle.Left;
			this.buttonTypes.Clicked += this.HandleButtonClicked;

			this.buttonValues = new IconButtonMark(this);
			this.buttonValues.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Values);
			this.buttonValues.Name = BundleType.Convert(ResourceAccess.Type.Values);
			this.buttonValues.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonValues.MarkDisposition = ButtonMarkDisposition.Below;
			this.buttonValues.MarkLength = 5;
			this.buttonValues.PreferredWidth = width;
			this.buttonValues.MinHeight = 20+5;
			this.buttonValues.AutoFocus = false;
			this.buttonValues.Margins = new Margins(2, 0, 0, 0);
			this.buttonValues.Dock = DockStyle.Left;
			this.buttonValues.Clicked += this.HandleButtonClicked;

			this.buttonFields = new IconButtonMark(this);
			this.buttonFields.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Fields);
			this.buttonFields.Name = BundleType.Convert(ResourceAccess.Type.Fields);
			this.buttonFields.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonFields.MarkDisposition = ButtonMarkDisposition.Below;
			this.buttonFields.MarkLength = 5;
			this.buttonFields.PreferredWidth = width;
			this.buttonFields.MinHeight = 20+5;
			this.buttonFields.AutoFocus = false;
			this.buttonFields.Margins = new Margins(2, 0, 0, 0);
			this.buttonFields.Dock = DockStyle.Left;
			this.buttonFields.Clicked += this.HandleButtonClicked;

			this.buttonEntities = new IconButtonMark(this);
			this.buttonEntities.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Entities);
			this.buttonEntities.Name = BundleType.Convert(ResourceAccess.Type.Entities);
			this.buttonEntities.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonEntities.MarkDisposition = ButtonMarkDisposition.Below;
			this.buttonEntities.MarkLength = 5;
			this.buttonEntities.PreferredWidth = width;
			this.buttonEntities.MinHeight = 20+5;
			this.buttonEntities.AutoFocus = false;
			this.buttonEntities.Margins = new Margins(2, 0, 0, 0);
			this.buttonEntities.Dock = DockStyle.Left;
			this.buttonEntities.Clicked += this.HandleButtonClicked;

			this.buttonForms = new IconButtonMark(this);
			this.buttonForms.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Forms);
			this.buttonForms.Name = BundleType.Convert(ResourceAccess.Type.Forms);
			this.buttonForms.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonForms.MarkDisposition = ButtonMarkDisposition.Below;
			this.buttonForms.MarkLength = 5;
			this.buttonForms.PreferredWidth = width;
			this.buttonForms.MinHeight = 20+5;
			this.buttonForms.AutoFocus = false;
			this.buttonForms.Margins = new Margins(2, 0, 0, 0);
			this.buttonForms.Dock = DockStyle.Left;
			this.buttonForms.Clicked += this.HandleButtonClicked;

			this.buttonPanels = new IconButtonMark(this);
			this.buttonPanels.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Panels);
			this.buttonPanels.Name = BundleType.Convert(ResourceAccess.Type.Panels);
			this.buttonPanels.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonPanels.MarkDisposition = ButtonMarkDisposition.Below;
			this.buttonPanels.MarkLength = 5;
			this.buttonPanels.PreferredWidth = width;
			this.buttonPanels.MinHeight = 20+5;
			this.buttonPanels.AutoFocus = false;
			this.buttonPanels.Margins = new Margins(2, 0, 0, 0);
			this.buttonPanels.Dock = DockStyle.Left;
			this.buttonPanels.Clicked += this.HandleButtonClicked;

			this.UpdateButtons();
		}

		public BundleType(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public ResourceAccess.Type CurrentType
		{
			get
			{
				return this.currentType;
			}

			set
			{
				if (this.currentType != value)
				{
					ResourceAccess.Type oldType = this.currentType;
					this.currentType = value;
					this.UpdateButtons();
					this.OnTypeChanged(oldType);
				}
			}
		}


		private void UpdateButtons()
		{
			BundleType.ActiveButton (this.buttonStrings , this.currentType == ResourceAccess.Type.Strings );
			BundleType.ActiveButton (this.buttonCaptions, this.currentType == ResourceAccess.Type.Captions);
			BundleType.ActiveButton (this.buttonFields  , this.currentType == ResourceAccess.Type.Fields  );
			BundleType.ActiveButton (this.buttonCommands, this.currentType == ResourceAccess.Type.Commands);
			BundleType.ActiveButton (this.buttonTypes   , this.currentType == ResourceAccess.Type.Types   );
			BundleType.ActiveButton (this.buttonValues  , this.currentType == ResourceAccess.Type.Values  );
			BundleType.ActiveButton (this.buttonPanels  , this.currentType == ResourceAccess.Type.Panels  );
			BundleType.ActiveButton (this.buttonEntities, this.currentType == ResourceAccess.Type.Entities);
			BundleType.ActiveButton (this.buttonForms   , this.currentType == ResourceAccess.Type.Forms   );
		}

		private static void ActiveButton(IconButtonMark button, bool active)
		{
			if (active)
			{
				button.ButtonStyle = ButtonStyle.ActivableIcon;
				button.ActiveState = ActiveState.Yes;
			}
			else
			{
				button.ButtonStyle = ButtonStyle.ToolItem;  // mode discret, sans cadre
				button.ActiveState = ActiveState.No;
			}
		}


		void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			IconButtonMark button = sender as IconButtonMark;
			this.CurrentType = BundleType.Convert(button.Name);
		}


		private static ResourceAccess.Type Convert(string name)
		{
			return (ResourceAccess.Type) System.Enum.Parse(typeof(ResourceAccess.Type), name);
		}

		private static string Convert(ResourceAccess.Type type)
		{
			return type.ToString();
		}


		#region Events handler
		protected virtual void OnTypeChanged(ResourceAccess.Type oldType)
		{
			//	Génère un événement pour dire que le type a été changé.
			EventHandler<CancelEventArgs> handler = (EventHandler<CancelEventArgs>) this.GetUserEventHandler("TypeChanged");
			if (handler != null)
			{
				CancelEventArgs e = new CancelEventArgs();
				handler(this, e);

				if (e.Cancel)  // annule le changement de sélection ?
				{
					this.currentType = oldType;
					this.UpdateButtons();
				}
			}
		}

		public event EventHandler<CancelEventArgs> TypeChanged
		{
			add
			{
				this.AddUserEventHandler("TypeChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("TypeChanged", value);
			}
		}
		#endregion


		private ResourceAccess.Type				currentType = ResourceAccess.Type.Strings;
		private IconButtonMark					buttonStrings;
		private IconButtonMark					buttonEntities;
		private IconButtonMark					buttonCaptions;
		private IconButtonMark					buttonFields;
		private IconButtonMark					buttonCommands;
		private IconButtonMark					buttonPanels;
		private IconButtonMark					buttonTypes;
		private IconButtonMark					buttonValues;
		private IconButtonMark					buttonForms;
	}
}
