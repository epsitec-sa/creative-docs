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


		public string NameToCreate
		{
			get
			{
				return this.nameToCreate;
			}
			set
			{
				this.nameToCreate = value;
			}
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
			this.buttonGoto.Clicked += this.HandleButtonGotoClicked;

			this.buttonClear = new IconButton(box);
			this.buttonClear.CaptionId = Res.Captions.Form.LabelReplacementClear.Id;
			this.buttonClear.AutoFocus = false;
			this.buttonClear.Dock = DockStyle.Right;
			this.buttonClear.Clicked += this.HandleButtonClearClicked;

			this.buttonCaption = new Button(box);
			this.buttonCaption.Text = "Choisir";
			this.buttonCaption.PreferredWidth = 22*2;
			this.buttonCaption.Dock = DockStyle.Right;
			this.buttonCaption.Clicked += this.HandleButtonCaptionClicked;

			this.UpdateInterface();

			this.widgetInterface = box;
			return box;
		}

		private void HandleButtonCaptionClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton caption "Choisir" est cliqué.
			Druid druid = (Druid) this.value;
			var result = this.application.DlgLabelReplacement(this.nameToCreate, ref druid);

			if (result != Common.Dialogs.DialogResult.Yes)  // annuler ?
			{
				return;
			}

			this.value = druid;
			this.OnValueChanged();
			this.UpdateInterface();
		}

		private void HandleButtonClearClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton clear est cliqué.
			this.value = Druid.Empty;  // plus de label de remplacement, donc on réutilise le label standard
			this.OnValueChanged();
			this.UpdateInterface();
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
				ToolTip.Default.SetToolTip(this.buttonCaption, text);

				this.buttonClear.Enable = !string.IsNullOrEmpty(text);
				this.buttonGoto.Enable = !string.IsNullOrEmpty(text);
			}
		}


		protected string nameToCreate;
		protected Button buttonCaption;
		protected IconButton buttonClear;
		protected IconButton buttonGoto;
	}
}
