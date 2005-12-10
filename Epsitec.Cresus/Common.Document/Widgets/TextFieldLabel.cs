using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe TextFieldLabel est un label StaticText suivi d'un TextFieldReal
	/// ou d'un TextField.
	/// </summary>
	public class TextFieldLabel : AbstractGroup
	{
		public enum Type
		{
			TextField,
			TextFieldReal,
			TextFieldUnit,
		}


		public TextFieldLabel()
		{
			//throw new System.InvalidOperationException("Do not user this constructor.");
		}

		public TextFieldLabel(Type type)
		{
			this.type = type;
			this.isUnitPercent = false;
			this.marginWidth = TextFieldLabel.DefaultMarginWidth;

			if ( this.type == Type.TextFieldUnit )
			{
				this.buttonUnit = new Button(this);
				this.buttonUnit.Width = 20;
				this.buttonUnit.AutoFocus = false;
				this.buttonUnit.Dock = DockStyle.Left;
				this.buttonUnit.DockMargins = new Margins(0, 1, 0, 0);
				//?ToolTip.Default.SetToolTip(this.buttonUnit, Res.Strings.TextPanel.Units);  // TODO: voir avec Pierre pourquoi ça plante !
			}
			else
			{
				this.label = new StaticText(this);
				this.label.Alignment = ContentAlignment.MiddleRight;
				this.label.Dock = DockStyle.Fill;
				this.label.DockMargins = new Margins(0, this.marginWidth, 0, 0);
			}
			this.labelVisibility = true;

			if ( this.type == Type.TextField )
			{
				this.textFieldSimple = new TextField(this);
				this.textFieldSimple.Width = TextFieldLabel.DefaultTextWidth;
				this.textFieldSimple.Dock = DockStyle.Right;
				this.textFieldSimple.TabIndex = 0;
				this.textFieldSimple.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.textFieldSimple.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
				this.textFieldSimple.AutoSelectOnFocus = true;
				this.textFieldSimple.SwallowEscape = true;
				this.textFieldSimple.SwallowReturn = true;
			}

			if ( this.type == Type.TextFieldReal )
			{
				this.textFieldReal = new TextFieldReal(this);
				this.textFieldReal.Width = TextFieldLabel.DefaultTextWidth;
				this.textFieldReal.Dock = DockStyle.Right;
				this.textFieldReal.TabIndex = 0;
				this.textFieldReal.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.textFieldReal.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
				this.textFieldReal.AutoSelectOnFocus = true;
				this.textFieldReal.SwallowEscape = true;
				this.textFieldReal.SwallowReturn = true;
			}

			if ( this.type == Type.TextFieldUnit )
			{
				this.textFieldReal = new TextFieldReal(this);
				this.textFieldReal.Width = TextFieldLabel.DefaultTextWidth;
				this.textFieldReal.Dock = DockStyle.None;
				this.textFieldReal.TabIndex = 0;
				this.textFieldReal.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.textFieldReal.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
				this.textFieldReal.AutoSelectOnFocus = true;
				this.textFieldReal.SwallowEscape = true;
				this.textFieldReal.SwallowReturn = true;

				this.textFieldPercent = new TextFieldReal(this);
				this.textFieldPercent.Width = TextFieldLabel.DefaultTextWidth;
				this.textFieldPercent.Dock = DockStyle.None;
				this.textFieldPercent.TabIndex = 0;
				this.textFieldPercent.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;

				this.textFieldPercent.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
				this.textFieldPercent.AutoSelectOnFocus = true;
				this.textFieldPercent.SwallowEscape = true;
				this.textFieldPercent.SwallowReturn = true;
			}
		}

		public TextFieldLabel(Widget embedder, Type type) : this(type)
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if ( this.label != null )  this.label.Dispose();
				if ( this.buttonUnit != null )  this.buttonUnit.Dispose();
				if ( this.textFieldSimple != null )  this.textFieldSimple.Dispose();
				if ( this.textFieldReal != null )  this.textFieldReal.Dispose();
				if ( this.textFieldPercent != null )  this.textFieldPercent.Dispose();
				
				this.label = null;
				this.buttonUnit = null;
				this.textFieldSimple = null;
				this.textFieldReal = null;
				this.textFieldPercent = null;
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


		// Spécifie les bornes pour une dimension.
		public void SetRangeDimension(Document document, double min, double max, double step)
		{
			this.textFieldReal.FactorMinRange = (decimal) min;
			this.textFieldReal.FactorMaxRange = (decimal) max;
			this.textFieldReal.FactorStep     = (decimal) step;

			document.Modifier.AdaptTextFieldRealDimension(this.textFieldReal);
		}

		// Spécifie les bornes pour une valeur en pourcents.
		public void SetRangePercents(Document document, double min, double max, double step)
		{
			TextFieldReal field = (this.type == Type.TextFieldUnit) ? this.textFieldPercent : this.textFieldReal;

			document.Modifier.AdaptTextFieldRealPercent(field);

			field.MinValue   = (decimal) min;
			field.MaxValue   = (decimal) max;
			field.Step       = (decimal) step;
			field.Resolution = 1.0M;
		}

		// Indique l'unité en cours pour le type TextFieldUnit.
		public bool IsUnitPercent
		{
			get
			{
				System.Diagnostics.Debug.Assert(this.type == Type.TextFieldUnit);
				return this.isUnitPercent;
			}

			set
			{
				System.Diagnostics.Debug.Assert(this.type == Type.TextFieldUnit);
				this.isUnitPercent = value;

				this.textFieldReal.Visibility    = !this.isUnitPercent;
				this.textFieldPercent.Visibility =  this.isUnitPercent;

				this.textFieldReal.Dock    = this.isUnitPercent ? DockStyle.None : DockStyle.Fill;
				this.textFieldPercent.Dock = this.isUnitPercent ? DockStyle.Fill : DockStyle.None;

				string text = this.isUnitPercent ? this.labelLongText : this.labelShortText;
				if ( this.buttonUnit.Text != text )
				{
					this.buttonUnit.Text = text;
				}
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
		public TextField TextField
		{
			get
			{
				System.Diagnostics.Debug.Assert(this.textFieldSimple != null);
				return this.textFieldSimple;
			}
		}

		// Donne le widget pour le texte.
		public TextFieldReal TextFieldReal
		{
			get
			{
				System.Diagnostics.Debug.Assert(this.textFieldReal != null);
				if ( this.type == Type.TextFieldUnit )
				{
					return this.isUnitPercent ? this.textFieldPercent : this.textFieldReal;
				}
				else
				{
					return this.textFieldReal;
				}
			}
		}

		// Donne le widget pour le texte (dimension).
		public TextFieldReal TextFieldReal1
		{
			get
			{
				System.Diagnostics.Debug.Assert(this.type == Type.TextFieldUnit);
				return this.textFieldReal;
			}
		}

		// Donne le widget pour le texte (pourcents).
		public TextFieldReal TextFieldReal2
		{
			get
			{
				System.Diagnostics.Debug.Assert(this.type == Type.TextFieldUnit);
				return this.textFieldPercent;
			}
		}

		// Donne le widget pour choisir l'unité.
		public Button ButtonUnit
		{
			get
			{
				System.Diagnostics.Debug.Assert(this.buttonUnit != null);
				return this.buttonUnit;
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
			
			if ( this.label != null )
			{
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
		}


		protected Type						type;
		protected bool						isUnitPercent;
		protected StaticText				label;
		protected Button					buttonUnit;
		protected string					labelShortText;
		protected string					labelLongText;
		protected bool						labelVisibility;
		protected TextField					textFieldSimple;
		protected TextFieldReal				textFieldReal;
		protected TextFieldReal				textFieldPercent;
		protected double					marginWidth;
	}
}
