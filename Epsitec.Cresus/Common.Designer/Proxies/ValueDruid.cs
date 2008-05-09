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
		public ValueDruid()
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

			TextFieldEx field = new TextFieldEx(box);
			field.PreferredWidth = 88;
			field.Dock = DockStyle.Right;
			field.EditionAccepted += new EventHandler(this.HandleFieldValueChanged);
			this.field = field;

			this.UpdateInterface();

			this.widgetInterface = box;
			return box;
		}

		private void HandleFieldValueChanged(object sender)
		{
			//	Appelé lorsque la valeur représentée dans l'interface a changé.
			if (this.ignoreChange)
			{
				return;
			}

			TextFieldEx field = sender as TextFieldEx;
			if (string.IsNullOrEmpty(field.Text))
			{
				this.value = Druid.Empty;
			}
			else
			{
				this.value = Druid.Parse(field.Text);
			}
			this.OnValueChanged();
		}

		protected override void UpdateInterface()
		{
			//	Met à jour la valeur dans l'interface.
			if (this.field != null)
			{
				this.ignoreChange = true;

				Druid druid = (Druid) this.value;
				if (druid.IsEmpty)
				{
					field.Text = "";
				}
				else
				{
					field.Text = druid.ToString();
				}

				this.ignoreChange = false;
			}
		}


		protected TextFieldEx field;
	}
}
