using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe TextFieldLabel est un label StaticText suivi d'un TextFieldReal
	/// ou d'un TextField.
	/// </summary>
	public class TextFieldLabel : Widget
	{
		public TextFieldLabel()
		{
			//throw new System.InvalidOperationException("Do not user this constructor.");
		}

		public TextFieldLabel(bool simply)
		{
			this.marginWidth = TextFieldLabel.DefaultMarginWidth;

			this.labelVisibility = true;
			this.label = new StaticText(this);
			this.label.Alignment = ContentAlignment.MiddleRight;
			this.label.Dock = DockStyle.Fill;
			this.label.DockMargins = new Margins(0, this.marginWidth, 0, 0);

			if ( simply )
			{
				this.textFieldSimple = new TextField(this);
				this.textFieldSimple.Width = TextFieldLabel.DefaultTextWidth;
				this.textFieldSimple.Dock = DockStyle.Right;
			}
			else
			{
				this.textFieldReal = new TextFieldReal(this);
				this.textFieldReal.Width = TextFieldLabel.DefaultTextWidth;
				this.textFieldReal.Dock = DockStyle.Right;
			}
		}

		public TextFieldLabel(Widget embedder, bool simply) : this(simply)
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if ( this.label != null )  this.label.Dispose();
				if ( this.textFieldSimple != null )  this.textFieldSimple.Dispose();
				if ( this.textFieldReal != null )  this.textFieldReal.Dispose();
				
				this.label = null;
				this.textFieldSimple = null;
				this.textFieldReal = null;
			}
			
			base.Dispose(disposing);
		}


		// Largeur par défaut.
		public override double DefaultWidth
		{
			get
			{
				return TextFieldLabel.ShortWidth;
			}
		}


		// Texte court du label.
		public string LabelShortText
		{
			get
			{
				return this.labelShortText;
			}

			set
			{
				this.labelShortText = value;
			}
		}

		// Texte long du label.
		public string LabelLongText
		{
			get
			{
				return this.labelLongText;
			}

			set
			{
				this.labelLongText = value;
			}
		}

		// Visibilité du label.
		public bool LabelVisibility
		{
			get
			{
				return this.labelVisibility;
			}

			set
			{
				this.labelVisibility = value;
			}
		}

		// Donne le widget pour le texte.
		public TextFieldReal TextFieldReal
		{
			get
			{
				System.Diagnostics.Debug.Assert(this.textFieldReal != null);
				return this.textFieldReal;
			}
		}

		// Donne le widget pour le texte.
		public TextField TextField
		{
			get
			{
				System.Diagnostics.Debug.Assert(this.textFieldSimple != null);
				return this.textFieldSimple;
			}
		}

		// Largeur en mode court.
		public static double ShortWidth
		{
			get
			{
				return TextFieldLabel.DefaultLabelWidth +
					   TextFieldLabel.DefaultMarginWidth +
					   TextFieldLabel.DefaultTextWidth;
			}
		}

		// Largeur par défaut du label.
		public static double DefaultLabelWidth
		{
			get
			{
				return 10;
			}
		}

		// Largeur par défaut de la marge entre le label et le texte.
		public static double DefaultMarginWidth
		{
			get
			{
				return 2;
			}
		}

		// Largeur par défaut du texte.
		public static double DefaultTextWidth
		{
			get
			{
				return 48;
			}
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			
			if ( this.label == null )  return;

			string text = "";
			if ( this.labelVisibility )
			{
				if ( this.label.Width < 80 )  text = this.labelShortText;
				else                          text = this.labelLongText+" ";
			}

			if ( this.label.Text != text )
			{
				this.label.Text = text;
			}
		}


		protected StaticText				label;
		protected string					labelShortText;
		protected string					labelLongText;
		protected bool						labelVisibility;
		protected TextFieldReal				textFieldReal;
		protected TextField					textFieldSimple;
		protected double					marginWidth;
	}
}
