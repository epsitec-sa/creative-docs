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

			TextField field = new TextField(box);
			field.IsReadOnly = true;
			field.PreferredWidth = 88;
			field.Dock = DockStyle.Right;
			field.Clicked += new MessageEventHandler(this.HandleFieldClicked);
			this.field = field;

			this.UpdateInterface();

			this.widgetInterface = box;
			return box;
		}

		private void HandleFieldClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque la ligne éditable est cliquée.
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
		}

		protected override void UpdateInterface()
		{
			//	Met à jour la valeur dans l'interface.
			if (this.field != null)
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

				this.ignoreChange = true;
				this.field.Text = text;
				this.field.SelectAll();
				this.field.Cursor = 0;
				ToolTip.Default.SetToolTip(this.field, text);
				this.ignoreChange = false;
			}
		}


		protected TextField field;
	}
}
