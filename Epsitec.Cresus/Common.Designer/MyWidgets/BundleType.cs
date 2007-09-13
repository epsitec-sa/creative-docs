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
			sep.Margins = new Margins(0, 0, 0, 0);
			sep.Dock = DockStyle.Bottom;

			double width = 75;

			this.buttonStrings = new IconButtonMark(this);
			this.buttonStrings.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Strings);
			this.buttonStrings.Name = BundleType.Convert(ResourceAccess.Type.Strings);
			this.buttonStrings.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonStrings.SiteMark = ButtonMarkDisposition.Below;
			this.buttonStrings.MarkDimension = 5;
			this.buttonStrings.PreferredWidth = width;
			this.buttonStrings.MinHeight = 20+5;
			this.buttonStrings.AutoFocus = false;
			this.buttonStrings.Margins = new Margins(2, 0, 10, 0);
			this.buttonStrings.Dock = DockStyle.Left;
			this.buttonStrings.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonCaptions = new IconButtonMark(this);
			this.buttonCaptions.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Captions);
			this.buttonCaptions.Name = BundleType.Convert(ResourceAccess.Type.Captions);
			this.buttonCaptions.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonCaptions.SiteMark = ButtonMarkDisposition.Below;
			this.buttonCaptions.MarkDimension = 5;
			this.buttonCaptions.PreferredWidth = width;
			this.buttonCaptions.MinHeight = 20+5;
			this.buttonCaptions.AutoFocus = false;
			this.buttonCaptions.Margins = new Margins(2, 0, 10, 0);
			this.buttonCaptions.Dock = DockStyle.Left;
			this.buttonCaptions.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonCommands = new IconButtonMark(this);
			this.buttonCommands.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Commands);
			this.buttonCommands.Name = BundleType.Convert(ResourceAccess.Type.Commands);
			this.buttonCommands.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonCommands.SiteMark = ButtonMarkDisposition.Below;
			this.buttonCommands.MarkDimension = 5;
			this.buttonCommands.PreferredWidth = width;
			this.buttonCommands.MinHeight = 20+5;
			this.buttonCommands.AutoFocus = false;
			this.buttonCommands.Margins = new Margins(2, 0, 10, 0);
			this.buttonCommands.Dock = DockStyle.Left;
			this.buttonCommands.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonTypes = new IconButtonMark(this);
			this.buttonTypes.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Types);
			this.buttonTypes.Name = BundleType.Convert(ResourceAccess.Type.Types);
			this.buttonTypes.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonTypes.SiteMark = ButtonMarkDisposition.Below;
			this.buttonTypes.MarkDimension = 5;
			this.buttonTypes.PreferredWidth = width;
			this.buttonTypes.MinHeight = 20+5;
			this.buttonTypes.AutoFocus = false;
			this.buttonTypes.Margins = new Margins(2, 0, 10, 0);
			this.buttonTypes.Dock = DockStyle.Left;
			this.buttonTypes.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonValues = new IconButtonMark(this);
			this.buttonValues.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Values);
			this.buttonValues.Name = BundleType.Convert(ResourceAccess.Type.Values);
			this.buttonValues.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonValues.SiteMark = ButtonMarkDisposition.Below;
			this.buttonValues.MarkDimension = 5;
			this.buttonValues.PreferredWidth = width;
			this.buttonValues.MinHeight = 20+5;
			this.buttonValues.AutoFocus = false;
			this.buttonValues.Margins = new Margins(2, 0, 10, 0);
			this.buttonValues.Dock = DockStyle.Left;
			this.buttonValues.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonFields = new IconButtonMark(this);
			this.buttonFields.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Fields);
			this.buttonFields.Name = BundleType.Convert(ResourceAccess.Type.Fields);
			this.buttonFields.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonFields.SiteMark = ButtonMarkDisposition.Below;
			this.buttonFields.MarkDimension = 5;
			this.buttonFields.PreferredWidth = width;
			this.buttonFields.MinHeight = 20+5;
			this.buttonFields.AutoFocus = false;
			this.buttonFields.Margins = new Margins(2, 0, 10, 0);
			this.buttonFields.Dock = DockStyle.Left;
			this.buttonFields.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonEntities = new IconButtonMark(this);
			this.buttonEntities.Text = "Entités"; // ResourceAccess.TypeDisplayName(ResourceAccess.Type.Entities);
			this.buttonEntities.Name = BundleType.Convert(ResourceAccess.Type.Entities);
			this.buttonEntities.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonEntities.SiteMark = ButtonMarkDisposition.Below;
			this.buttonEntities.MarkDimension = 5;
			this.buttonEntities.PreferredWidth = width;
			this.buttonEntities.MinHeight = 20+5;
			this.buttonEntities.AutoFocus = false;
			this.buttonEntities.Margins = new Margins(2, 0, 10, 0);
			this.buttonEntities.Dock = DockStyle.Left;
			this.buttonEntities.Clicked += new MessageEventHandler(this.HandleButtonClicked);

			this.buttonPanels = new IconButtonMark(this);
			this.buttonPanels.Text = ResourceAccess.TypeDisplayName(ResourceAccess.Type.Panels);
			this.buttonPanels.Name = BundleType.Convert(ResourceAccess.Type.Panels);
			this.buttonPanels.ButtonStyle = ButtonStyle.ActivableIcon;
			this.buttonPanels.SiteMark = ButtonMarkDisposition.Below;
			this.buttonPanels.MarkDimension = 5;
			this.buttonPanels.PreferredWidth = width;
			this.buttonPanels.MinHeight = 20+5;
			this.buttonPanels.AutoFocus = false;
			this.buttonPanels.Margins = new Margins(2, 0, 10, 0);
			this.buttonPanels.Dock = DockStyle.Left;
			this.buttonPanels.Clicked += new MessageEventHandler(this.HandleButtonClicked);

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


		protected void UpdateButtons()
		{
			this.buttonStrings.ActiveState  = (this.currentType == ResourceAccess.Type.Strings ) ? ActiveState.Yes : ActiveState.No;
			this.buttonCaptions.ActiveState = (this.currentType == ResourceAccess.Type.Captions) ? ActiveState.Yes : ActiveState.No;
			this.buttonFields.ActiveState   = (this.currentType == ResourceAccess.Type.Fields  ) ? ActiveState.Yes : ActiveState.No;
			this.buttonCommands.ActiveState = (this.currentType == ResourceAccess.Type.Commands) ? ActiveState.Yes : ActiveState.No;
			this.buttonTypes.ActiveState    = (this.currentType == ResourceAccess.Type.Types   ) ? ActiveState.Yes : ActiveState.No;
			this.buttonValues.ActiveState   = (this.currentType == ResourceAccess.Type.Values  ) ? ActiveState.Yes : ActiveState.No;
			this.buttonPanels.ActiveState   = (this.currentType == ResourceAccess.Type.Panels  ) ? ActiveState.Yes : ActiveState.No;
			this.buttonEntities.ActiveState = (this.currentType == ResourceAccess.Type.Entities) ? ActiveState.Yes : ActiveState.No;
		}


		void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			IconButtonMark button = sender as IconButtonMark;
			this.CurrentType = BundleType.Convert(button.Name);
		}


		protected static ResourceAccess.Type Convert(string name)
		{
			return (ResourceAccess.Type) System.Enum.Parse(typeof(ResourceAccess.Type), name);
		}

		protected static string Convert(ResourceAccess.Type type)
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


		protected ResourceAccess.Type		currentType = ResourceAccess.Type.Strings;
		protected IconButtonMark			buttonStrings;
		protected IconButtonMark			buttonEntities;
		protected IconButtonMark			buttonCaptions;
		protected IconButtonMark			buttonFields;
		protected IconButtonMark			buttonCommands;
		protected IconButtonMark			buttonPanels;
		protected IconButtonMark			buttonTypes;
		protected IconButtonMark			buttonValues;
	}
}
