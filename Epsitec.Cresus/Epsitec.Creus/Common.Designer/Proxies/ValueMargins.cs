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
		public ValueMargins(DesignerApplication application) : base(application)
		{
			this.min = 0.0;
			this.max = 100.0;
			this.step = 1.0;
			this.resolution = 1.0;
		}

		public ValueMargins(DesignerApplication application, double min, double max, double step, double resolution) : base(application)
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
			this.buttonLink = new CheckButton(top);
			this.buttonLink.AutoToggle = false;
			this.buttonLink.Text = Res.Strings.Panel.Margins.Link;
			this.buttonLink.PreferredWidth = 50-5-1;
			this.buttonLink.Dock = DockStyle.Right;
			this.buttonLink.Margins = new Margins(5, 0, 0, 0);
			this.buttonLink.Clicked += this.HandleButtonLinkClicked;

			this.fieldTop = new TextFieldSlider(top);
			this.fieldTop.MinValue = (decimal) this.min;
			this.fieldTop.MaxValue = (decimal) this.max;
			this.fieldTop.Step = (decimal) this.step;
			this.fieldTop.Resolution = (decimal) this.resolution;
			this.fieldTop.PreferredWidth = 50;
			this.fieldTop.Dock = DockStyle.Right;
			this.fieldTop.Margins = new Margins(-1, 0, 0, 0);
			this.fieldTop.ValueChanged += this.HandleFieldValueChanged;
			ToolTip.Default.SetToolTip(this.fieldTop, Res.Strings.Panel.Margins.Top);

			if (!this.hasHiddenLabel)
			{
				StaticText label = new StaticText(top);
				label.Text = this.label;
				label.CaptionId = this.caption.Id;
				label.ContentAlignment = ContentAlignment.MiddleRight;
				label.Margins = new Margins(0, 5, 0, 5);
				label.Dock = DockStyle.Fill;
			}

			//	Ligne inférieure:
			this.fieldRight = new TextFieldSlider(bottom);
			this.fieldRight.MinValue = (decimal) this.min;
			this.fieldRight.MaxValue = (decimal) this.max;
			this.fieldRight.Step = (decimal) this.step;
			this.fieldRight.Resolution = (decimal) this.resolution;
			this.fieldRight.PreferredWidth = 50;
			this.fieldRight.Dock = DockStyle.Right;
			this.fieldRight.Margins = new Margins(-1, 0, 0, 0);
			this.fieldRight.ValueChanged += this.HandleFieldValueChanged;
			ToolTip.Default.SetToolTip(this.fieldRight, Res.Strings.Panel.Margins.Right);

			this.fieldBottom = new TextFieldSlider(bottom);
			this.fieldBottom.MinValue = (decimal) this.min;
			this.fieldBottom.MaxValue = (decimal) this.max;
			this.fieldBottom.Step = (decimal) this.step;
			this.fieldBottom.Resolution = (decimal) this.resolution;
			this.fieldBottom.PreferredWidth = 50;
			this.fieldBottom.Dock = DockStyle.Right;
			this.fieldBottom.Margins = new Margins(-1, 0, 0, 0);
			this.fieldBottom.ValueChanged += this.HandleFieldValueChanged;
			ToolTip.Default.SetToolTip(this.fieldBottom, Res.Strings.Panel.Margins.Bottom);

			this.fieldLeft = new TextFieldSlider(bottom);
			this.fieldLeft.MinValue = (decimal) this.min;
			this.fieldLeft.MaxValue = (decimal) this.max;
			this.fieldLeft.Step = (decimal) this.step;
			this.fieldLeft.Resolution = (decimal) this.resolution;
			this.fieldLeft.PreferredWidth = 50;
			this.fieldLeft.Dock = DockStyle.Right;
			this.fieldLeft.ValueChanged += this.HandleFieldValueChanged;
			ToolTip.Default.SetToolTip(this.fieldLeft, Res.Strings.Panel.Margins.Left);

			this.UpdateInterface();

			this.widgetInterface = box;
			return box;
		}

		private void HandleButtonLinkClicked(object sender, MessageEventArgs e)
		{
			//	Appelé lorsque le bouton "liés" a été cliqué.
			if (this.buttonLink.ActiveState == ActiveState.No)
			{
				this.buttonLink.ActiveState = ActiveState.Yes;

				Margins m = (Margins) this.value;
				m.Left   = m.Top;
				m.Bottom = m.Top;
				m.Right  = m.Top;

				if ((Margins) this.value != m)
				{
					this.value = m;
					this.OnValueChanged();
					this.UpdateInterface();
				}
			}
			else
			{
				this.buttonLink.ActiveState = ActiveState.No;
			}

			this.UpdateLink(false);
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

			if (field == this.fieldTop)
			{
				m.Top = (double) field.Value;

				if (this.buttonLink.ActiveState == ActiveState.Yes)
				{
					m.Left   = (double) field.Value;
					m.Bottom = (double) field.Value;
					m.Right  = (double) field.Value;

					this.ignoreChange = true;
					this.fieldLeft.Value   = (decimal) m.Left;
					this.fieldBottom.Value = (decimal) m.Left;
					this.fieldRight.Value  = (decimal) m.Left;
					this.ignoreChange = false;
				}
			}
			else if (field == this.fieldLeft)
			{
				if (this.buttonLink.ActiveState == ActiveState.No)
				{
					m.Left = (double) field.Value;
				}
			}
			else if (field == this.fieldBottom)
			{
				if (this.buttonLink.ActiveState == ActiveState.No)
				{
					m.Bottom = (double) field.Value;
				}
			}
			else if (field == this.fieldRight)
			{
				if (this.buttonLink.ActiveState == ActiveState.No)
				{
					m.Right = (double) field.Value;
				}
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
				this.fieldLeft.Value   = (decimal) m.Left;
				this.fieldRight.Value  = (decimal) m.Right;
				this.fieldTop.Value    = (decimal) m.Top;
				this.fieldBottom.Value = (decimal) m.Bottom;

				this.UpdateLink(true);

				this.ignoreChange = false;
			}
		}

		protected void UpdateLink(bool changeState)
		{
			if (changeState)
			{
				Margins m = (Margins) this.value;
				bool link = (m.Left == m.Right && m.Left == m.Top && m.Left == m.Bottom);

				this.buttonLink.ActiveState = link ? ActiveState.Yes : ActiveState.No;
			}

			this.fieldLeft.Enable   = (this.buttonLink.ActiveState == ActiveState.No);
			this.fieldBottom.Enable = (this.buttonLink.ActiveState == ActiveState.No);
			this.fieldRight.Enable  = (this.buttonLink.ActiveState == ActiveState.No);
		}


		protected double min;
		protected double max;
		protected double step;
		protected double resolution;
		protected TextFieldSlider fieldLeft;
		protected TextFieldSlider fieldRight;
		protected TextFieldSlider fieldTop;
		protected TextFieldSlider fieldBottom;
		protected CheckButton buttonLink;
	}
}
