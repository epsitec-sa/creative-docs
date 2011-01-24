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
			this.shortLongLabelLimit = 80;

			if ( this.type == Type.TextFieldUnit )
			{
				this.buttonUnit = new Button(this);
				this.buttonUnit.PreferredWidth = 20;
				this.buttonUnit.AutoFocus = false;
				this.buttonUnit.Dock = DockStyle.Left;
				this.buttonUnit.Margins = new Margins(0, 1, 0, 0);
				ToolTip.Default.SetToolTip(this.buttonUnit, Res.Strings.TextPanel.Units);
			}
			else
			{
				this.label = new StaticText(this);
				this.label.ContentAlignment = ContentAlignment.MiddleRight;
				this.label.Dock = DockStyle.Fill;
				this.label.Margins = new Margins(0, this.marginWidth, 0, 0);
			}
			this.labelVisibility = true;

			if ( this.type == Type.TextField )
			{
				this.textFieldSimple = new TextField(this);
				this.textFieldSimple.PreferredWidth = TextFieldLabel.DefaultTextWidth;
				this.textFieldSimple.Dock = DockStyle.Right;
				this.textFieldSimple.TabIndex = 0;
				this.textFieldSimple.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.textFieldSimple.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
				this.textFieldSimple.AutoSelectOnFocus = true;
				this.textFieldSimple.SwallowEscapeOnRejectEdition = true;
				this.textFieldSimple.SwallowReturnOnAcceptEdition = true;
			}

			if ( this.type == Type.TextFieldReal )
			{
				this.textFieldReal = new TextFieldReal(this);
				this.textFieldReal.PreferredWidth = TextFieldLabel.DefaultTextWidth;
				this.textFieldReal.Dock = DockStyle.Right;
				this.textFieldReal.TabIndex = 0;
				this.textFieldReal.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.textFieldReal.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
				this.textFieldReal.AutoSelectOnFocus = true;
				this.textFieldReal.SwallowEscapeOnRejectEdition = true;
				this.textFieldReal.SwallowReturnOnAcceptEdition = true;
			}

			if ( this.type == Type.TextFieldUnit )
			{
				this.textFieldReal = new TextFieldReal(this);
				this.textFieldReal.PreferredWidth = TextFieldLabel.DefaultTextWidth;
				this.textFieldReal.Dock = DockStyle.None;
				this.textFieldReal.TabIndex = 0;
				this.textFieldReal.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.textFieldReal.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
				this.textFieldReal.AutoSelectOnFocus = true;
				this.textFieldReal.SwallowEscapeOnRejectEdition = true;
				this.textFieldReal.SwallowReturnOnAcceptEdition = true;

				this.textFieldPercent = new TextFieldReal(this);
				this.textFieldPercent.PreferredWidth = TextFieldLabel.DefaultTextWidth;
				this.textFieldPercent.Dock = DockStyle.None;
				this.textFieldPercent.TabIndex = 0;
				this.textFieldPercent.TabNavigationMode = TabNavigationMode.ActivateOnTab;

				this.textFieldPercent.DefocusAction = DefocusAction.AutoAcceptOrRejectEdition;
				this.textFieldPercent.AutoSelectOnFocus = true;
				this.textFieldPercent.SwallowEscapeOnRejectEdition = true;
				this.textFieldPercent.SwallowReturnOnAcceptEdition = true;
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

#if false
		public override double DefaultWidth
		{
			//	Largeur par défaut.
			get
			{
				return TextFieldLabel.ShortWidth;
			}
		}
#endif


		public double ShortLongLabelLimit
		{
			get
			{
				return this.shortLongLabelLimit;
			}
			set
			{
				if (this.shortLongLabelLimit != value)
				{
					this.shortLongLabelLimit = value;
					this.ManualArrange ();
				}
			}
		}

		public bool ButtonUnitEnable
		{
			//	Etat du bouton pour changer d'unité.
			get
			{
				if ( this.buttonUnit == null )  return false;
				return this.buttonUnit.Enable;
			}

			set
			{
				if ( this.buttonUnit != null )
				{
					this.buttonUnit.Enable = value;
				}
			}
		}


		public void SetRangeDimension(Document document, double min, double max, double def, double step)
		{
			//	Spécifie les bornes pour une dimension.
			this.textFieldReal.FactorMinRange     = (decimal) min;
			this.textFieldReal.FactorMaxRange     = (decimal) max;
			this.textFieldReal.FactorDefaultRange = (decimal) def;
			this.textFieldReal.FactorStep         = (decimal) step;
			this.textFieldReal.FactorResolution   = 10.0M;  // une décimale de moins

			document.Modifier.AdaptTextFieldRealDimension(this.textFieldReal);
		}

		public void SetRangeFontSize(Document document)
		{
			//	Spécifie les bornes pour une taille de fonte.
			this.textFieldReal.UnitType = RealUnitType.Scalar;
			this.textFieldReal.Scale = (decimal) Modifier.FontSizeScale;
			if ( document.Type == DocumentType.Pictogram )
			{
				this.textFieldReal.InternalMinValue     = (decimal) ( 0.1*Modifier.FontSizeScale);
				this.textFieldReal.InternalMaxValue     = (decimal) (24.0*Modifier.FontSizeScale);
				this.textFieldReal.InternalDefaultValue = (decimal) ( 1.2*Modifier.FontSizeScale);
				this.textFieldReal.Step                 = 0.1M;
				this.textFieldReal.Resolution           = 0.01M;
			}
			else
			{
				this.textFieldReal.InternalMinValue     = (decimal) (  1*Modifier.FontSizeScale);
				this.textFieldReal.InternalMaxValue     = (decimal) (240*Modifier.FontSizeScale);
				this.textFieldReal.InternalDefaultValue = (decimal) ( 12*Modifier.FontSizeScale);
				this.textFieldReal.Step                 = 1.0M;
				this.textFieldReal.Resolution           = 0.1M;
			}
		}

		public void SetRangePercents(Document document, double min, double max, double def, double step)
		{
			//	Spécifie les bornes pour une valeur en pourcents.
			TextFieldReal field = (this.type == Type.TextFieldUnit) ? this.textFieldPercent : this.textFieldReal;

			document.Modifier.AdaptTextFieldRealPercent(field);

			field.MinValue     = (decimal) min;
			field.MaxValue     = (decimal) max;
			field.DefaultValue = (decimal) def;
			field.Step         = (decimal) step;
			field.Resolution   = 1.0M;
		}

		public bool IsUnitPercent
		{
			//	Indique l'unité en cours pour le type TextFieldUnit.
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

		public TextField TextField
		{
			//	Donne le widget pour le texte.
			get
			{
				System.Diagnostics.Debug.Assert(this.textFieldSimple != null);
				return this.textFieldSimple;
			}
		}

		public TextFieldReal TextFieldReal
		{
			//	Donne le widget pour le texte.
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

		public TextFieldReal TextFieldReal1
		{
			//	Donne le widget pour le texte (dimension).
			get
			{
				System.Diagnostics.Debug.Assert(this.type == Type.TextFieldUnit);
				return this.textFieldReal;
			}
		}

		public TextFieldReal TextFieldReal2
		{
			//	Donne le widget pour le texte (pourcents).
			get
			{
				System.Diagnostics.Debug.Assert(this.type == Type.TextFieldUnit);
				return this.textFieldPercent;
			}
		}

		public Button ButtonUnit
		{
			//	Donne le widget pour choisir l'unité.
			get
			{
				System.Diagnostics.Debug.Assert(this.buttonUnit != null);
				return this.buttonUnit;
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
					if (this.label.ActualWidth < this.shortLongLabelLimit)
					{
						text = this.labelShortText;
					}
					else
					{
						text = this.labelLongText + " ";
					}
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
		protected double					shortLongLabelLimit;
	}
}
