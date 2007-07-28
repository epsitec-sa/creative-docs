using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Dolphin
{
	/// <summary>
	/// Permet d'éditer une valeur hexadécimale.
	/// </summary>
	public class TextFieldHexa : AbstractGroup
	{
		public TextFieldHexa() : base()
		{
			this.label = new StaticText(this);
			this.label.ContentAlignment = ContentAlignment.MiddleRight;
			this.label.PreferredWidth = 25;
			this.label.Margins = new Margins(0, 5, 0, 0);
			this.label.Dock = DockStyle.Left;

			this.textField = new TextField(this);
			this.textField.Dock = DockStyle.Left;

			this.leds = new List<Led>();
			for (int i=0; i<12; i++)
			{
				Led led = new Led(this);
				led.PreferredWidth = 20;
				led.PreferredHeight = 20;
				led.Margins = new Margins((i+1)%4 == 0 ? 10:1, 1, 1, 1);
				led.Dock = DockStyle.Right;

				this.leds.Add(led);
			}
		}

		public TextFieldHexa(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.label.Dispose();
			}

			base.Dispose(disposing);
		}


		public int BitCount
		{
			//	Nombre de bits de la valeur (à priori 8 ou 12).
			get
			{
				return this.bitCount;
			}
			set
			{
				if (this.bitCount != value)
				{
					this.bitCount = value;

					double max = TextFieldHexa.GetHexaWidth(16);
					double width = TextFieldHexa.GetHexaWidth(this.bitCount);

					this.textField.PreferredWidth = width;
					this.textField.Margins = new Margins(max-width, 20, 0, 0);

					for (int i=0; i<this.leds.Count; i++)
					{
						this.leds[i].Visibility = (i < this.bitCount);
					}
				}
			}
		}

		public string Label
		{
			//	Label affiché à gauche.
			get
			{
				return this.baseName;
			}
			set
			{
				if (this.baseName != value)
				{
					this.baseName = value;
					this.label.Text = string.Concat("<b>", this.baseName, "</b>");;
				}
			}
		}

		public int HexaValue
		{
			//	Valeur courante.
			get
			{
				return TextFieldHexa.ParseHexa(this.textField.Text);
			}
			set
			{
				if (this.baseValue != value)
				{
					this.baseValue = value;

					string format = string.Format("X{0}", (this.bitCount+3)/4);  // "X2" si 8 bits, "X3" si 12 bits, etc.
					this.textField.Text = this.baseValue.ToString(format);
					
					this.UpdateLeds();
				}
			}
		}


		protected void UpdateLeds()
		{
			//	Met à jour les leds en fonction de la valeur actuelle.
			for (int i=0; i<this.leds.Count; i++)
			{
				this.leds[i].ActiveState = (this.baseValue & (1 << i)) == 0 ? ActiveState.No : ActiveState.Yes;
			}
		}

		protected static double GetHexaWidth(int bitCount)
		{
			//	Retourne la largeur nécessaire pour représenter un certain nombre de bits en hexa.
			return 10 + ((bitCount+3)/4)*10;
		}

		protected static int ParseHexa(string hexa)
		{
			int result;

			if (System.Int32.TryParse(hexa, System.Globalization.NumberStyles.AllowHexSpecifier, System.Globalization.CultureInfo.CurrentCulture, out result))
			{
				return result;
			}
			else
			{
				return 0;
			}
		}


		protected int						bitCount;
		protected int						baseValue = -1;
		protected string					baseName;

		protected StaticText				label;
		protected TextField					textField;
		protected List<Led>					leds;
	}
}
