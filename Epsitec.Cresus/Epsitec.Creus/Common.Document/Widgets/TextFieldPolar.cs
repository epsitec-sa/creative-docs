using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe TextFieldPolar permet d'éditer une coordonnée polaire.
	/// </summary>
	public class TextFieldPolar : AbstractGroup
	{
		public TextFieldPolar()
		{
			this.marginWidth = TextFieldPolar.DefaultMarginWidth;

			this.label = new StaticText(this);
			this.label.ContentAlignment = ContentAlignment.MiddleRight;
			this.label.Dock = DockStyle.Fill;
			this.label.Margins = new Margins(0, this.marginWidth, 0, 0);

			this.textFieldA = new TextFieldReal(this);
			this.textFieldA.InternalMinValue     = -90.0M;
			this.textFieldA.InternalMaxValue     = 90.0M;
			this.textFieldA.InternalDefaultValue = 0.0M;
			this.textFieldA.Step                 = 1.0M;
			this.textFieldA.Resolution           = 0.1M;
			this.textFieldA.TextSuffix = "°";
			this.textFieldA.PreferredWidth = TextFieldPolar.DefaultTextWidth;
			this.textFieldA.Dock = DockStyle.Right;
			this.textFieldA.Margins = new Margins(this.marginWidth, 0, 0, 0);
			this.textFieldA.TabIndex = 1;
			this.textFieldA.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.textFieldA.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.textFieldA.AutoSelectOnFocus = true;
			this.textFieldA.SwallowEscapeOnRejectEdition = true;
			this.textFieldA.SwallowReturnOnAcceptEdition = true;

			this.textFieldR = new TextFieldReal(this);
			this.textFieldR.InternalMinValue     = -100.0M;
			this.textFieldR.InternalMaxValue     = 100.0M;
			this.textFieldR.InternalDefaultValue = 0.0M;
			this.textFieldR.Step                 = 5.0M;
			this.textFieldR.Resolution           = 0.1M;
			this.textFieldR.TextSuffix = "%";
			this.textFieldR.PreferredWidth = TextFieldPolar.DefaultTextWidth;
			this.textFieldR.Dock = DockStyle.Right;
			this.textFieldR.TabIndex = 0;
			this.textFieldR.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.textFieldR.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
			this.textFieldR.AutoSelectOnFocus = true;
			this.textFieldR.SwallowEscapeOnRejectEdition = true;
			this.textFieldR.SwallowReturnOnAcceptEdition = true;
		}

		public TextFieldPolar(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if ( this.label != null )  this.label.Dispose();
				if ( this.textFieldR != null )  this.textFieldR.Dispose();
				if ( this.textFieldA != null )  this.textFieldA.Dispose();
				
				this.label = null;
				this.textFieldR = null;
				this.textFieldA = null;
			}
			
			base.Dispose(disposing);
		}

#if false
		public override double DefaultWidth
		{
			//	Largeur par défaut.
			get
			{
				return TextFieldPolar.ShortWidth;
			}
		}
#endif


		public string LabelText
		{
			//	Texte du label.
			get
			{
				return this.label.Text;
			}

			set
			{
				this.label.Text = value;
			}
		}

		public TextFieldReal TextFieldR
		{
			//	Donne le widget pour le texte.
			get
			{
				return this.textFieldR;
			}
		}

		public TextFieldReal TextFieldA
		{
			//	Donne le widget pour le texte.
			get
			{
				return this.textFieldA;
			}
		}

		public static double ShortWidth
		{
			//	Largeur en mode court.
			get
			{
				return TextFieldPolar.DefaultLabelWidth +
					   TextFieldPolar.DefaultMarginWidth +
					   TextFieldPolar.DefaultTextWidth;
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


		protected StaticText				label;
		protected TextFieldReal				textFieldR;
		protected TextFieldReal				textFieldA;
		protected double					marginWidth;
	}
}
