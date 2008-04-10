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

			this.fieldLeft = new TextFieldSlider(box);
			this.fieldLeft.MinValue = (decimal) this.min;
			this.fieldLeft.MaxValue = (decimal) this.max;
			this.fieldLeft.Step = (decimal) this.step;
			this.fieldLeft.Resolution = (decimal) this.resolution;
			this.fieldLeft.PreferredWidth = 50;
			this.fieldLeft.Dock = DockStyle.Right;
			this.fieldLeft.ValueChanged += new EventHandler(this.HandleFieldValueChanged);

			this.fieldRight = new TextFieldSlider(box);
			this.fieldRight.MinValue = (decimal) this.min;
			this.fieldRight.MaxValue = (decimal) this.max;
			this.fieldRight.Step = (decimal) this.step;
			this.fieldRight.Resolution = (decimal) this.resolution;
			this.fieldRight.PreferredWidth = 50;
			this.fieldRight.Dock = DockStyle.Right;
			this.fieldRight.ValueChanged += new EventHandler(this.HandleFieldValueChanged);

			this.fieldTop = new TextFieldSlider(box);
			this.fieldTop.MinValue = (decimal) this.min;
			this.fieldTop.MaxValue = (decimal) this.max;
			this.fieldTop.Step = (decimal) this.step;
			this.fieldTop.Resolution = (decimal) this.resolution;
			this.fieldTop.PreferredWidth = 50;
			this.fieldTop.Dock = DockStyle.Right;
			this.fieldTop.ValueChanged += new EventHandler(this.HandleFieldValueChanged);

			this.fieldBottom = new TextFieldSlider(box);
			this.fieldBottom.MinValue = (decimal) this.min;
			this.fieldBottom.MaxValue = (decimal) this.max;
			this.fieldBottom.Step = (decimal) this.step;
			this.fieldBottom.Resolution = (decimal) this.resolution;
			this.fieldBottom.PreferredWidth = 50;
			this.fieldBottom.Dock = DockStyle.Right;
			this.fieldBottom.ValueChanged += new EventHandler(this.HandleFieldValueChanged);

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

#if false
			TextFieldSlider field = sender as TextFieldSlider;
			System.TypeCode code = System.Type.GetTypeCode(this.value.GetType());

			if (code == System.TypeCode.Int32)
			{
				int value = (int) field.Value;
				if ((int) this.value != value)
				{
					this.value = value;
					this.OnValueChanged();
				}
			}
			else if (code == System.TypeCode.Double)
			{
				double value = (int) field.Value;
				if ((double) this.value != value)
				{
					this.value = value;
					this.OnValueChanged();
				}
			}
			else
			{
				throw new System.NotImplementedException();
			}
#endif
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
