using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe permet de représenter une valeur de type Druid.
	/// </summary>
	public class ValueDruid : AbstractValue
	{
		public ValueDruid(DesignerApplication application) : base(application)
		{
		}


		public override Widget CreateInterface(Widget parent)
		{
			//	Crée les widgets permettant d'éditer la valeur.
			FrameBox box = new FrameBox(parent);
			ToolTip.Default.SetToolTip(box, this.caption.Description);

			if (!this.hasHiddenLabel)
			{
				StaticText label = new StaticText(box);
				label.Text = this.label;
				label.CaptionId = this.caption.Id;
				label.ContentAlignment = ContentAlignment.MiddleRight;
				label.Margins = new Margins(0, 5, 0, 0);
				label.Dock = DockStyle.Fill;
			}

			this.buttonGoto = new IconButton(box);
			this.buttonGoto.CaptionId = Res.Captions.Editor.LocatorGoto.Id;
			this.buttonGoto.AutoFocus = false;
			this.buttonGoto.Dock = DockStyle.Right;
			this.buttonGoto.Clicked += new MessageEventHandler(this.HandleButtonGotoClicked);

			this.buttonCaption = new Button(box);
			this.buttonCaption.Text = "Choisir";
			this.buttonCaption.PreferredWidth = 66;
			this.buttonCaption.Dock = DockStyle.Right;
			this.buttonCaption.Clicked += new MessageEventHandler(this.HandleButtonCaptionClicked);

			this.UpdateInterface();

			this.widgetInterface = box;
			return box;
		}

		private void HandleButtonCaptionClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton caption est cliqué.
#if false
			ResourceAccess.Type type = ResourceAccess.Type.Captions;
			List<Druid> exclude = null;
			Types.StructuredTypeClass typeClass = Types.StructuredTypeClass.None;
			Druid druid = (Druid) this.value;
			bool isNullable = false;
			Common.Dialogs.DialogResult result = this.application.DlgResourceSelector(Dialogs.ResourceSelector.Operation.Selection, this.application.CurrentModule, type, ref typeClass, ref druid, ref isNullable, exclude, Druid.Empty);

			if (result != Common.Dialogs.DialogResult.Yes)  // annuler ?
			{
				return;
			}

			this.value = druid;
			this.OnValueChanged();
			this.UpdateInterface();
#else
			Druid druid = (Druid) this.value;
			Common.Dialogs.DialogResult result = this.application.DlgLabelReplacement(null, ref druid);

			if (result != Common.Dialogs.DialogResult.Yes)  // annuler ?
			{
				return;
			}

			this.value = druid;
			this.OnValueChanged();
			this.UpdateInterface();
#endif
		}

		private void HandleButtonGotoClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton goto est cliqué.
			Druid druid = (Druid) this.value;
			Module module = this.application.SearchModule(druid);
			if (module == null)
			{
				return;
			}

			this.application.LocatorGoto(module.ModuleId.Name, ResourceAccess.Type.Captions, -1, druid, null);
		}

		protected override void UpdateInterface()
		{
			//	Met à jour la valeur dans l'interface.
			if (this.buttonCaption != null)
			{
				string text = null;
				Druid druid = (Druid) this.value;
				if (druid.IsValid)
				{
					Module module = this.application.SearchModule(druid);
					if (module != null)
					{
						CultureMap cultureMap = module.AccessCaptions.Accessor.Collection[druid];
						if (cultureMap != null)
						{
							text = cultureMap.Name;
						}
					}
				}

				//?this.buttonCaption.Text = text;
				ToolTip.Default.SetToolTip(this.buttonCaption, text);

				this.buttonGoto.Enable = !string.IsNullOrEmpty(text);
			}
		}


		protected Button buttonCaption;
		protected IconButton buttonGoto;
	}
}
