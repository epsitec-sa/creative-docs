using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// La classe TextFieldLabel est un label StaticText suivi d'un TextFieldReal.
	/// </summary>
	public class TextFieldLabel : AbstractGroup
	{
		public TextFieldLabel()
		{
			this.marginWidth = TextFieldLabel.DefaultMarginWidth;

			this.label = new StaticText(this);
			this.label.ContentAlignment = ContentAlignment.MiddleRight;
			this.label.Dock = DockStyle.Fill;
			this.label.Margins = new Margins(0, this.marginWidth, 0, 0);
			this.labelVisibility = true;

			this.textFieldReal = new TextFieldReal(this);
			this.textFieldReal.PreferredWidth = TextFieldLabel.DefaultTextWidth;
			this.textFieldReal.Dock = DockStyle.Right;
			this.textFieldReal.TabIndex = 1;
			this.textFieldReal.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.textFieldReal.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.textFieldReal.AutoSelectOnFocus = true;
			this.textFieldReal.SwallowEscapeOnRejectEdition = true;
			this.textFieldReal.SwallowReturnOnAcceptEdition = true;
		}

		public TextFieldLabel(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if ( this.label != null )  this.label.Dispose();
				if ( this.textFieldReal != null )  this.textFieldReal.Dispose();
				
				this.label = null;
				this.textFieldReal = null;
			}
			
			base.Dispose(disposing);
		}


		public void SetRangeDimension(double min, double max, double def, double step)
		{
			//	Spécifie les bornes pour une dimension.
			this.textFieldReal.InternalMinValue     = (decimal) min;
			this.textFieldReal.InternalMaxValue     = (decimal) max;
			this.textFieldReal.InternalDefaultValue = (decimal) def;
			this.textFieldReal.Step                 = (decimal) step;
			this.textFieldReal.Resolution           = 10.0M;  // une décimale de moins
		}


		public string LabelShortText
		{
			//	Texte court du label.
			get
			{
				return this.labelShortText;
			}

			set
			{
				this.labelShortText = value;
			}
		}

		public string LabelLongText
		{
			//	Texte long du label.
			get
			{
				return this.labelLongText;
			}

			set
			{
				this.labelLongText = value;
			}
		}

		public bool LabelVisibility
		{
			//	Visibilité du label.
			get
			{
				return this.labelVisibility;
			}

			set
			{
				this.labelVisibility = value;
			}
		}

		public TextFieldReal TextFieldReal
		{
			//	Donne le widget pour le texte.
			get
			{
				return this.textFieldReal;
			}
		}

		public static double ShortWidth
		{
			//	Largeur en mode court.
			get
			{
				return TextFieldLabel.DefaultLabelWidth +
					   TextFieldLabel.DefaultMarginWidth +
					   TextFieldLabel.DefaultTextWidth;
			}
		}

		public static double DefaultLabelWidth
		{
			//	Largeur par défaut du label.
			get
			{
				return 10;
			}
		}

		public static double DefaultMarginWidth
		{
			//	Largeur par défaut de la marge entre le label et le texte.
			get
			{
				return 2;
			}
		}

		public static double DefaultTextWidth
		{
			//	Largeur par défaut du texte.
			get
			{
				return 48;
			}
		}

		protected override void ManualArrange()
		{
 			base.ManualArrange();
			
			if ( this.label != null )
			{
				string text = "";
				if ( this.labelVisibility )
				{
					if ( this.label.ActualWidth < 80 )  text = this.labelShortText;
					else                                text = this.labelLongText+" ";
				}

				if ( this.label.Text != text )
				{
					this.label.Text = text;
				}
			}
		}


		protected StaticText				label;
		protected string					labelShortText;
		protected string					labelLongText;
		protected bool						labelVisibility;
		protected TextFieldReal				textFieldReal;
		protected double					marginWidth;
	}
}
