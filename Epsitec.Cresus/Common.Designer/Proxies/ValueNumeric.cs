using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Designer.Proxies
{
	/// <summary>
	/// Cette classe permet de représenter une valeur de type 'double'.
	/// </summary>
	public class ValueNumeric : AbstractValue
	{
		public ValueNumeric()
		{
			this.min = 0.0;
			this.max = 100.0;
			this.step = 1.0;
			this.resolution = 1.0;
		}

		public ValueNumeric(double min, double max, double step, double resolution)
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

			StaticText label = new StaticText(box);
			label.Text = this.label;
			label.CaptionId = this.caption.Id;
			label.ContentAlignment = ContentAlignment.MiddleRight;
			label.Margins = new Margins(0, 5, 0, 0);
			label.Dock = DockStyle.Fill;

			this.field = new TextFieldSlider(box);
			this.field.MinValue = (decimal) this.min;
			this.field.MaxValue = (decimal) this.max;
			this.field.Step = (decimal) this.step;
			this.field.Resolution = (decimal) this.resolution;
			this.field.PreferredWidth = 50;
			this.field.Dock = DockStyle.Right;
			this.field.ValueChanged += new EventHandler(this.HandleFieldValueChanged);
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

			System.TypeCode code = System.Type.GetTypeCode(this.value.GetType());

			if (code == System.TypeCode.Int32)
			{
				int value = (int) this.field.Value;
				if ((int) this.value != value)
				{
					this.value = value;
					this.OnValueChanged();
				}
			}
			else if (code == System.TypeCode.Double)
			{
				double value = (int) this.field.Value;
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
		}

		protected override void UpdateInterface()
		{
			//	Met à jour la valeur dans l'interface.
			if (this.field != null)
			{
				this.ignoreChange = true;
				this.field.Value = Types.InvariantConverter.ToDecimal(this.value);
				this.ignoreChange = false;
			}
		}

	
		protected double min;
		protected double max;
		protected double step;
		protected double resolution;
		protected TextFieldSlider field;
	}
}
