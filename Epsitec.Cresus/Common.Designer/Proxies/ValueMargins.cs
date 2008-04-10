using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe permet de représenter quatre valeurs de type 'Margins'.
	/// </summary>
	public class ValueMargins : AbstractValue
	{
		public ValueMargins()
		{
			this.min = 0.0;
			this.max = 100.0;
			this.step = 1.0;
			this.resolution = 1.0;
		}

		public ValueMargins(double min, double max, double step, double resolution)
		{
			this.min = min;
			this.max = max;
			this.step = step;
			this.resolution = resolution;
		}

		public double Min
		{
			get
			{
				return this.min;
			}
			set
			{
				this.min = value;
			}
		}

		public double Max
		{
			get
			{
				return this.max;
			}
			set
			{
				this.max = value;
			}
		}

		public double Step
		{
			get
			{
				return this.step;
			}
			set
			{
				this.step = value;
			}
		}

		public double Resolution
		{
			get
			{
				return this.resolution;
			}
			set
			{
				this.resolution = value;
			}
		}


		public override Widget CreateInterface(Widget parent)
		{
			//	Crée les widgets permettant d'éditer la valeur.
			FrameBox box = new FrameBox(parent);
			ToolTip.Default.SetToolTip(box, this.caption.Description);

			FrameBox top = new FrameBox(box);
			top.Dock = DockStyle.Top;
			top.Margins = new Margins(0, 0, 0, -1);

			FrameBox bottom = new FrameBox(box);
			bottom.Dock = DockStyle.Top;

			//	Ligne supérieure:
			FrameBox x = new FrameBox(top);
			x.PreferredWidth = 50;
			x.Dock = DockStyle.Right;
			x.Margins = new Margins(-1, 0, 0, 0);

			this.fieldTop = new TextFieldSlider(top);
			this.fieldTop.MinValue = (decimal) this.min;
			this.fieldTop.MaxValue = (decimal) this.max;
			this.fieldTop.Step = (decimal) this.step;
			this.fieldTop.Resolution = (decimal) this.resolution;
			this.fieldTop.PreferredWidth = 50;
			this.fieldTop.Dock = DockStyle.Right;
			this.fieldTop.Margins = new Margins(-1, 0, 0, 0);
			this.fieldTop.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
			ToolTip.Default.SetToolTip(this.fieldTop, Res.Strings.Panel.Margins.Top);

			StaticText label = new StaticText(top);
			label.Text = this.label;
			label.CaptionId = this.caption.Id;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Margins = new Margins(0, 5, 0, 5);
			label.Dock = DockStyle.Fill;

			//	Ligne inférieure:
			this.fieldRight = new TextFieldSlider(bottom);
			this.fieldRight.MinValue = (decimal) this.min;
			this.fieldRight.MaxValue = (decimal) this.max;
			this.fieldRight.Step = (decimal) this.step;
			this.fieldRight.Resolution = (decimal) this.resolution;
			this.fieldRight.PreferredWidth = 50;
			this.fieldRight.Dock = DockStyle.Right;
			this.fieldRight.Margins = new Margins(-1, 0, 0, 0);
			this.fieldRight.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
			ToolTip.Default.SetToolTip(this.fieldRight, Res.Strings.Panel.Margins.Right);

			this.fieldBottom = new TextFieldSlider(bottom);
			this.fieldBottom.MinValue = (decimal) this.min;
			this.fieldBottom.MaxValue = (decimal) this.max;
			this.fieldBottom.Step = (decimal) this.step;
			this.fieldBottom.Resolution = (decimal) this.resolution;
			this.fieldBottom.PreferredWidth = 50;
			this.fieldBottom.Dock = DockStyle.Right;
			this.fieldBottom.Margins = new Margins(-1, 0, 0, 0);
			this.fieldBottom.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
			ToolTip.Default.SetToolTip(this.fieldBottom, Res.Strings.Panel.Margins.Bottom);

			this.fieldLeft = new TextFieldSlider(bottom);
			this.fieldLeft.MinValue = (decimal) this.min;
			this.fieldLeft.MaxValue = (decimal) this.max;
			this.fieldLeft.Step = (decimal) this.step;
			this.fieldLeft.Resolution = (decimal) this.resolution;
			this.fieldLeft.PreferredWidth = 50;
			this.fieldLeft.Dock = DockStyle.Right;
			this.fieldLeft.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
			ToolTip.Default.SetToolTip(this.fieldLeft, Res.Strings.Panel.Margins.Left);

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

			TextFieldSlider field = sender as TextFieldSlider;
			Margins m = (Margins) this.value;

			if (field == this.fieldLeft)
			{
				m.Left = (double) field.Value;
			}
			else if (field == this.fieldRight)
			{
				m.Right = (double) field.Value;
			}
			else if (field == this.fieldTop)
			{
				m.Top = (double) field.Value;
			}
			else if (field == this.fieldBottom)
			{
				m.Bottom = (double) field.Value;
			}

			if ((Margins) this.value != m)
			{
				this.value = m;
				this.OnValueChanged();
			}
		}

		protected override void UpdateInterface()
		{
			//	Met à jour la valeur dans l'interface.
			if (this.fieldLeft != null)
			{
				this.ignoreChange = true;

				Margins m = (Margins) this.value;
				TextFieldSlider field;

				field = this.fieldLeft as TextFieldSlider;
				field.Value = (decimal) m.Left;

				field = this.fieldRight as TextFieldSlider;
				field.Value = (decimal) m.Right;

				field = this.fieldTop as TextFieldSlider;
				field.Value = (decimal) m.Top;

				field = this.fieldBottom as TextFieldSlider;
				field.Value = (decimal) m.Bottom;

				this.ignoreChange = false;
			}
		}


		protected double min;
		protected double max;
		protected double step;
		protected double resolution;
		protected TextFieldSlider fieldLeft;
		protected TextFieldSlider fieldRight;
		protected TextFieldSlider fieldTop;
		protected TextFieldSlider fieldBottom;
	}
}
