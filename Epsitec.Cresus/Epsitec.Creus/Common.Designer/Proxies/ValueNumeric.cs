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
		public ValueNumeric(DesignerApplication application) : base(application)
		{
			this.min = 0.0;
			this.max = 100.0;
			this.step = 1.0;
			this.resolution = 1.0;
		}

		public ValueNumeric(DesignerApplication application, double min, double max, double step, double resolution) : base(application)
		{
			this.min = min;
			this.max = max;
			this.step = step;
			this.resolution = resolution;
		}

		public bool IsOnlyIncDec
		{
			//	Indique s'il s'agit d'une valeur avec seulement des boutons +/-, et donc une
			//	valeur qui n'est pas directement éditable.
			get
			{
				return this.isOnlyIncDec;
			}
			set
			{
				this.isOnlyIncDec = value;
			}
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

			if (!this.hasHiddenLabel)
			{
				StaticText label = new StaticText(box);
				label.Text = this.label;
				label.CaptionId = this.caption.Id;
				label.ContentAlignment = ContentAlignment.MiddleRight;
				label.Margins = new Margins(0, 5, 0, 0);
				label.Dock = DockStyle.Fill;
			}

			if (this.isOnlyIncDec)
			{
				FrameBox buttons = new FrameBox(box);
				buttons.PreferredWidth = 10;
				buttons.Dock = DockStyle.Right;

				this.buttonInc = new GlyphButton(buttons);
				this.buttonInc.GlyphShape = GlyphShape.ArrowUp;
				this.buttonInc.PreferredHeight = parent.PreferredHeight/2+1;
				this.buttonInc.Dock = DockStyle.Top;
				this.buttonInc.Margins = new Margins(0, 0, 0, -1);
				this.buttonInc.Clicked += this.HandleButtonClicked;

				this.buttonDec = new GlyphButton(buttons);
				this.buttonDec.PreferredHeight = parent.PreferredHeight/2+1;
				this.buttonDec.GlyphShape = GlyphShape.ArrowDown;
				this.buttonDec.Dock = DockStyle.Bottom;
				this.buttonDec.Margins = new Margins(0, 0, -1, 0);
				this.buttonDec.Clicked += this.HandleButtonClicked;

				this.field = new TextField(box);
				this.field.IsReadOnly = true;
				this.field.PreferredWidth = 24;
				this.field.Dock = DockStyle.Right;
				this.field.Margins = new Margins(0, -1, 0, 0);
			}
			else
			{
				TextFieldSlider field = new TextFieldSlider(box);
				field.MinValue = (decimal) this.min;
				field.MaxValue = (decimal) this.max;
				field.Step = (decimal) this.step;
				field.Resolution = (decimal) this.resolution;
				field.PreferredWidth = 50;
				field.Dock = DockStyle.Right;
				field.ValueChanged += this.HandleFieldValueChanged;
				this.field = field;
			}

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
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsqu'un bouton -/+ a été cliqué.
			int value = (int) this.value;

			if (sender == this.buttonDec)
			{
				if (value > (int) this.min)
				{
					value--;
				}
			}

			if (sender == this.buttonInc)
			{
				if (value < (int) this.max)
				{
					value++;
				}
			}

			if ((int) this.value != value)
			{
				this.value = value;
				this.OnValueChanged();
			}
		}

		protected override void UpdateInterface()
		{
			//	Met à jour la valeur dans l'interface.
			if (this.field != null)
			{
				this.ignoreChange = true;

				if (this.isOnlyIncDec)
				{
					field.Text = this.value.ToString();
				}
				else
				{
					TextFieldSlider field = this.field as TextFieldSlider;
					field.Value = Types.InvariantConverter.ToDecimal(this.value);
				}

				this.ignoreChange = false;
			}
		}


		protected bool isOnlyIncDec;
		protected double min;
		protected double max;
		protected double step;
		protected double resolution;
		protected AbstractTextField field;
		protected GlyphButton buttonDec;
		protected GlyphButton buttonInc;
	}
}
