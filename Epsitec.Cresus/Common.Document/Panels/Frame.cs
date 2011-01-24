using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Panels
{
	/// <summary>
	/// La classe Frame permet de choisir un cadre.
	/// </summary>
	public class Frame : Abstract
	{
		public Frame(Document document)
			: base (document)
		{
			this.grid = new RadioIconGrid(this);
			this.grid.SelectionChanged += this.HandleTypeChanged;
			this.grid.TabIndex = 0;
			this.grid.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon (Properties.FrameType.None);
			this.AddRadioIcon (Properties.FrameType.Simple);
			this.AddRadioIcon (Properties.FrameType.White);
			this.AddRadioIcon (Properties.FrameType.Shadow);
			this.AddRadioIcon (Properties.FrameType.WhiteAndSnadow);

			this.fieldFrameWidth = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldFrameWidth.LabelShortText = Res.Strings.Panel.Frame.Short.FrameWidth;
			this.fieldFrameWidth.LabelLongText  = Res.Strings.Panel.Frame.Long.FrameWidth;
			this.fieldFrameWidth.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldFrameWidth.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldFrameWidth.TextFieldReal.FactorStep = 0.1M;
			this.document.Modifier.AdaptTextFieldRealDimension (this.fieldFrameWidth.TextFieldReal);
			this.fieldFrameWidth.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldFrameWidth.TabIndex = 2;
			this.fieldFrameWidth.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldFrameWidth, Res.Strings.Panel.Frame.Tooltip.FrameWidth);

			this.fieldFrameColor = new ColorSample (this);
			this.fieldFrameColor.DragSourceFrame = true;
			this.fieldFrameColor.Clicked += this.HandleFieldColorClicked;
			this.fieldFrameColor.ColorChanged += this.HandleFieldColorChanged;
			this.fieldFrameColor.TabIndex = 3;
			this.fieldFrameColor.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldFrameColor, Res.Strings.Panel.Gradient.Tooltip.Color1);

			this.fieldMarginWidth = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldMarginWidth.LabelShortText = Res.Strings.Panel.Frame.Short.MarginWidth;
			this.fieldMarginWidth.LabelLongText  = Res.Strings.Panel.Frame.Long.MarginWidth;
			this.fieldMarginWidth.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldMarginWidth.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldMarginWidth.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension (this.fieldMarginWidth.TextFieldReal);
			this.fieldMarginWidth.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldMarginWidth.TabIndex = 3;
			this.fieldMarginWidth.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldMarginWidth, Res.Strings.Panel.Frame.Tooltip.MarginWidth);

			this.fieldMarginColor = new ColorSample (this);
			this.fieldMarginColor.DragSourceFrame = true;
			this.fieldMarginColor.Clicked += this.HandleFieldColorClicked;
			this.fieldMarginColor.ColorChanged += this.HandleFieldColorChanged;
			this.fieldMarginColor.TabIndex = 3;
			this.fieldMarginColor.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldMarginColor, Res.Strings.Panel.Gradient.Tooltip.Color1);

			this.fieldShadowSize = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldShadowSize.LabelShortText = Res.Strings.Panel.Frame.Short.ShadowSize;
			this.fieldShadowSize.LabelLongText  = Res.Strings.Panel.Frame.Long.ShadowSize;
			this.fieldShadowSize.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldShadowSize.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldShadowSize.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension (this.fieldShadowSize.TextFieldReal);
			this.fieldShadowSize.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldShadowSize.TabIndex = 4;
			this.fieldShadowSize.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldShadowSize, Res.Strings.Panel.Frame.Tooltip.ShadowSize);

			this.fieldShadowColor = new ColorSample (this);
			this.fieldShadowColor.DragSourceFrame = true;
			this.fieldShadowColor.Clicked += this.HandleFieldColorClicked;
			this.fieldShadowColor.ColorChanged += this.HandleFieldColorChanged;
			this.fieldShadowColor.TabIndex = 3;
			this.fieldShadowColor.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldShadowColor, Res.Strings.Panel.Gradient.Tooltip.Color1);

			this.isNormalAndExtended = true;
		}

		protected void AddRadioIcon(Properties.FrameType type)
		{
			this.grid.AddRadioIcon (Misc.Icon (Properties.Frame.GetIconText (type)), Properties.Frame.GetName (type), (int) type, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.grid.SelectionChanged -= HandleTypeChanged;

				this.fieldFrameWidth.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldFrameColor.Clicked += this.HandleFieldColorClicked;
				this.fieldFrameColor.ColorChanged += this.HandleFieldColorChanged;

				this.fieldMarginWidth.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldMarginColor.Clicked += this.HandleFieldColorClicked;
				this.fieldMarginColor.ColorChanged += this.HandleFieldColorChanged;

				this.fieldShadowSize.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldShadowColor.Clicked += this.HandleFieldColorClicked;
				this.fieldShadowColor.ColorChanged += this.HandleFieldColorChanged;

				this.grid = null;
				this.fieldFrameWidth = null;
				this.fieldFrameColor = null;
				this.fieldMarginWidth = null;
				this.fieldMarginColor = null;
				this.fieldShadowSize = null;
				this.fieldShadowColor = null;
			}
			
			base.Dispose(disposing);
		}

		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau étendu ?
				{
					h += 30;

					if (this.HasFrame)
					{
						h += 50;
					}

					if (this.HasShadow)
					{
						h += 25;
					}
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
			}
		}

		protected override void PropertyToWidgets()
		{
			//	Propriété -> widgets.
			base.PropertyToWidgets();

			var p = this.property as Properties.Frame;
			if ( p == null )  return;

			this.ignoreChanged = true;

			this.grid.SelectedValue = (int) p.FrameType;

			this.fieldFrameWidth.TextFieldReal.InternalValue = (decimal) p.FrameWidth;
			this.fieldFrameColor.Color = p.FrameColor;
			
			this.fieldMarginWidth.TextFieldReal.InternalValue = (decimal) p.MarginWidth;
			this.fieldMarginColor.Color = p.MarginColor;
			
			this.fieldShadowSize.TextFieldReal.InternalValue = (decimal) p.ShadowSize;
			this.fieldShadowColor.Color = p.ShadowColor;

			this.EnableWidgets();
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			var p = this.property as Properties.Frame;
			if ( p == null )  return;

			p.FrameType = (Properties.FrameType) this.grid.SelectedValue;

			p.FrameWidth = (double) this.fieldFrameWidth.TextFieldReal.InternalValue;
			p.FrameColor = this.fieldFrameColor.Color;
			
			p.MarginWidth = (double) this.fieldMarginWidth.TextFieldReal.InternalValue;
			p.MarginColor = this.fieldMarginColor.Color;

			p.ShadowSize = (double) this.fieldShadowSize.TextFieldReal.InternalValue;
			p.ShadowColor = this.fieldShadowColor.Color;
		}

		protected void EnableWidgets()
		{
			//	Grise les widgets nécessaires.
			this.UpdateClientGeometry ();
		}


		public override void OriginColorDeselect()
		{
			//	Désélectionne toutes les origines de couleurs possibles.
			this.fieldFrameColor.ActiveState = ActiveState.No;
			this.fieldMarginColor.ActiveState = ActiveState.No;
			this.fieldShadowColor.ActiveState = ActiveState.No;
		}

		public override void OriginColorSelect(int rank)
		{
			//	Sélectionne l'origine de couleur.
			if ( rank != -1 )
			{
				this.originFieldRank = rank;
				     if ( rank == 0 )  this.originFieldColor = this.fieldFrameColor;
				else if ( rank == 1 )  this.originFieldColor = this.fieldMarginColor;
				else                   this.originFieldColor = this.fieldShadowColor;
			}
			if ( this.originFieldColor == null )  return;

			this.OriginColorDeselect();
			this.originFieldColor.ActiveState = ActiveState.Yes;
		}

		public override int OriginColorRank()
		{
			//	Retourne le rang de la couleur d'origine.
			return this.originFieldRank;
		}

		public override void OriginColorChange(Drawing.RichColor color)
		{
			//	Modifie la couleur d'origine.
			if ( this.originFieldColor == null )  return;
			
			if ( this.originFieldColor.Color != color )
			{
				this.originFieldColor.Color = color;
				this.OnChanged();
			}
		}

		public override Drawing.RichColor OriginColorGet()
		{
			//	Donne la couleur d'origine.
			if ( this.originFieldColor == null )  return Drawing.RichColor.FromBrightness(0.0);
			return this.originFieldColor.Color;
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.grid == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Inflate(1);
			this.grid.SetManualBounds(r);

			if (this.isExtendedSize)
			{
				r.Top = rect.Top-25;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Right = rect.Right;

				if (this.HasFrame)
				{
					Frame.UpdateFieldAndColorGeometry (this.fieldFrameWidth, this.fieldFrameColor, r);
					r.Offset (0, -25);

					Frame.UpdateFieldAndColorGeometry (this.fieldMarginWidth, this.fieldMarginColor, r);
					r.Offset (0, -25);
				}
				else
				{
					this.fieldFrameWidth.Visibility = false;
					this.fieldFrameColor.Visibility = false;
					this.fieldMarginWidth.Visibility = false;
					this.fieldMarginColor.Visibility = false;
				}

				if (this.HasShadow)
				{
					Frame.UpdateFieldAndColorGeometry (this.fieldShadowSize, this.fieldShadowColor, r);
					r.Offset (0, -25);
				}
				else
				{
					this.fieldShadowSize.Visibility = false;
					this.fieldShadowColor.Visibility = false;
				}
			}
			else
			{
				this.fieldFrameWidth.Visibility = false;
				this.fieldFrameColor.Visibility = false;

				this.fieldMarginWidth.Visibility = false;
				this.fieldMarginColor.Visibility = false;

				this.fieldShadowSize.Visibility = false;
				this.fieldShadowColor.Visibility = false;
			}
		}

		private static void UpdateFieldAndColorGeometry(Widgets.TextFieldLabel field, ColorSample sample, Rectangle rect)
		{
			field.Visibility = true;
			sample.Visibility = true;

			rect.Width -= Widgets.TextFieldLabel.DefaultTextWidth+5;
			field.SetManualBounds (rect);

			rect.Left = rect.Right+5;
			rect.Width = Widgets.TextFieldLabel.DefaultTextWidth;
			sample.SetManualBounds (rect);
		}


		private bool HasFrame
		{
			get
			{
				Properties.FrameType type = (Properties.FrameType) this.grid.SelectedValue;
				return type == Properties.FrameType.Simple || type == Properties.FrameType.White || type == Properties.FrameType.WhiteAndSnadow;
			}
		}

		private bool HasShadow
		{
			get
			{
				Properties.FrameType type = (Properties.FrameType) this.grid.SelectedValue;
				return type == Properties.FrameType.Shadow || type == Properties.FrameType.WhiteAndSnadow;
			}
		}


		private void HandleTypeChanged(object sender)
		{
			//	Le type a été changé.
			if (this.ignoreChanged)
			{
				return;
			}

			this.HeightChanged ();
			this.EnableWidgets ();

			//	Met les valeurs par défaut correspondant au type choisi.
			Properties.FrameType type = (Properties.FrameType) this.grid.SelectedValue;
			double frameWidth, marginWidth, shadowSize;
			Properties.Frame.GetFieldsParam (type, out frameWidth, out marginWidth, out shadowSize);
			this.fieldFrameWidth.TextFieldReal.InternalValue = (decimal) frameWidth;
			this.fieldMarginWidth.TextFieldReal.InternalValue = (decimal) marginWidth;
			this.fieldShadowSize.TextFieldReal.InternalValue = (decimal) shadowSize;

			this.OnChanged();
		}

		private void HandleFieldChanged(object sender)
		{
			//	Un champ a été changé.
			if ( this.ignoreChanged )  return;
			this.OnChanged();
		}

		private void HandleFieldColorClicked(object sender, MessageEventArgs e)
		{
			this.originFieldColor = sender as ColorSample;

			this.originFieldRank = -1;
			if (this.originFieldColor == this.fieldFrameColor)  this.originFieldRank = 0;
			if (this.originFieldColor == this.fieldMarginColor)  this.originFieldRank = 1;
			if (this.originFieldColor == this.fieldShadowColor)  this.originFieldRank = 2;

			this.OnOriginColorChanged ();
		}

		private void HandleFieldColorChanged(object sender)
		{
			ColorSample cs = sender as ColorSample;
			if (cs.ActiveState == ActiveState.Yes)
			{
				this.OnOriginColorChanged ();
			}

			this.OnChanged ();
		}


		protected RadioIconGrid				grid;

		protected Widgets.TextFieldLabel	fieldFrameWidth;
		protected ColorSample				fieldFrameColor;

		protected Widgets.TextFieldLabel	fieldMarginWidth;
		protected ColorSample				fieldMarginColor;

		protected Widgets.TextFieldLabel	fieldShadowSize;
		protected ColorSample				fieldShadowColor;

		protected ColorSample				originFieldColor;
		protected int						originFieldRank = -1;
	}
}
