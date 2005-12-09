using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe TextFieldUnit est un widget permettant d'éditer une valeur avec une unité.
	/// </summary>
	public class TextFieldUnit : TextField
	{
		public TextFieldUnit()
		{
			this.scale          = 1;
			this.minValue       = 0;
			this.maxValue       = 100;
			this.defaultValue   = 0;
			this.step           = 1;

			this.scalePercent   = 0.01;
			this.minPercent     = 0;
			this.maxPercent     = 100;
			this.defaultPercent = 0;
			this.stepPercent    = 5;
		}

		public TextFieldUnit(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public void SetValue(double value, Text.Properties.SizeUnits units)
		{
			if ( units == Common.Text.Properties.SizeUnits.Percent )
			{
				value /= this.scalePercent;
			}
			else
			{
				value /= this.scale;
			}

			this.Text = Misc.ConvertDoubleToString(value, units, 0);
		}

		public void GetValue(out double value, out Text.Properties.SizeUnits units)
		{
			double min = this.minValue;
			double max = this.maxValue;
			double def = this.defaultValue;

			if ( this.Text.EndsWith("%") )
			{
				min = this.minPercent;
				max = this.maxPercent;
				def = this.defaultPercent;
			}

			Misc.ConvertStringToDouble(out value, out units, this.Text, min, max, def);

			if ( units == Common.Text.Properties.SizeUnits.Percent )
			{
				value *= this.scalePercent;
			}
			else
			{
				value *= this.scale;
			}
		}


		public void IncrementValue(double delta)
		{
			double value;
			Text.Properties.SizeUnits units;
			this.GetValue(out value, out units);

			if ( units == Common.Text.Properties.SizeUnits.Percent )
			{
				value += this.stepPercent*delta;
			}
			else
			{
				value += this.step*delta;
			}

			this.SetValue(value, units);
		}


		public double Scale
		{
			get
			{
				return this.scale;
			}

			set
			{
				this.scale = value;
			}
		}

		public double MinValue
		{
			get
			{
				return this.minValue / this.scale;
			}

			set
			{
				this.minValue = value * this.scale;
			}
		}

		public double MaxValue
		{
			get
			{
				return this.maxValue / this.scale;
			}

			set
			{
				this.maxValue = value * this.scale;
			}
		}

		public double DefaultValue
		{
			get
			{
				return this.defaultValue / this.scale;
			}

			set
			{
				this.defaultValue = value * this.scale;
			}
		}

		public double Step
		{
			get
			{
				return this.step / this.scale;
			}

			set
			{
				this.step = value * this.scale;
			}
		}


		public double ScalePercent
		{
			get
			{
				return this.scalePercent;
			}

			set
			{
				this.scalePercent = value;
			}
		}

		public double MinPercent
		{
			get
			{
				return this.minPercent / this.scalePercent;
			}

			set
			{
				this.minPercent = value * this.scalePercent;
			}
		}

		public double MaxPercent
		{
			get
			{
				return this.maxPercent / this.scalePercent;
			}

			set
			{
				this.maxPercent = value * this.scalePercent;
			}
		}

		public double DefaultPercent
		{
			get
			{
				return this.defaultPercent / this.scalePercent;
			}

			set
			{
				this.defaultPercent = value * this.scalePercent;
			}
		}

		public double StepPercent
		{
			get
			{
				return this.stepPercent / this.scalePercent;
			}

			set
			{
				this.stepPercent = value * this.scalePercent;
			}
		}



		protected double						scale;
		protected double						minValue;
		protected double						maxValue;
		protected double						defaultValue;
		protected double						step;

		protected double						scalePercent;
		protected double						minPercent;
		protected double						maxPercent;
		protected double						defaultPercent;
		protected double						stepPercent;
	}
}
