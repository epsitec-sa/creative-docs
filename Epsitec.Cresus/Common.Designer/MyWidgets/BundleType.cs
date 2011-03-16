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

			this.buttonStrings  = this.CreateButton (ResourceAccess.Type.Strings);
			this.buttonCaptions = this.CreateButton (ResourceAccess.Type.Captions);
			this.buttonCommands = this.CreateButton (ResourceAccess.Type.Commands);
			this.buttonTypes    = this.CreateButton (ResourceAccess.Type.Types);
			this.buttonValues   = this.CreateButton (ResourceAccess.Type.Values);
			this.buttonFields   = this.CreateButton (ResourceAccess.Type.Fields);
			this.buttonEntities = this.CreateButton (ResourceAccess.Type.Entities);
			this.buttonForms    = this.CreateButton (ResourceAccess.Type.Forms);
			this.buttonPanels   = this.CreateButton (ResourceAccess.Type.Panels);

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


		private IconButtonMark CreateButton(ResourceAccess.Type type)
		{
			var button = new IconButtonMark
			{
				Parent = this,
				Text = ResourceAccess.TypeDisplayName (type),
				Name = BundleType.Convert (type),
				MarkDisposition = ButtonMarkDisposition.Below,
				MarkLength = 5,
				PreferredWidth = 75,
				MinHeight = 20+5,
				AutoFocus = false,
				Margins = new Margins (2, 0, 0, 0),
				Dock = DockStyle.Left,
			};

			button.Clicked += this.HandleButtonClicked;

			return button;
		}

		private void UpdateButtons()
		{
			BundleType.ActiveButton (this.buttonStrings,  this.currentType == ResourceAccess.Type.Strings );
			BundleType.ActiveButton (this.buttonCaptions, this.currentType == ResourceAccess.Type.Captions);
			BundleType.ActiveButton (this.buttonFields,   this.currentType == ResourceAccess.Type.Fields  );
			BundleType.ActiveButton (this.buttonCommands, this.currentType == ResourceAccess.Type.Commands);
			BundleType.ActiveButton (this.buttonTypes,    this.currentType == ResourceAccess.Type.Types   );
			BundleType.ActiveButton (this.buttonValues,   this.currentType == ResourceAccess.Type.Values  );
			BundleType.ActiveButton (this.buttonPanels,   this.currentType == ResourceAccess.Type.Panels  );
			BundleType.ActiveButton (this.buttonEntities, this.currentType == ResourceAccess.Type.Entities);
			BundleType.ActiveButton (this.buttonForms,    this.currentType == ResourceAccess.Type.Forms   );
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
