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
			this.gridSamples = new RadioIconGrid (this);
			this.gridSamples.SelectionChanged += this.HandleSampleChanged;
			this.gridSamples.TabIndex = 0;
			this.gridSamples.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			int rank = 0;
			foreach (Sample sample in Frame.Samples)
			{
				this.gridSamples.AddRadioIcon (Misc.Icon (sample.Icon), sample.Text, rank++, false);
			}

			this.gridType = new RadioIconGrid (this);
			this.gridType.SelectionChanged += this.HandleTypeChanged;
			this.gridType.TabIndex = 1;
			this.gridType.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioTypeIcon (Properties.FrameType.None);
			this.AddRadioTypeIcon (Properties.FrameType.OnlyFrame);
			this.AddRadioTypeIcon (Properties.FrameType.FrameAndShadow);
			this.AddRadioTypeIcon (Properties.FrameType.OnlyShadow);

			this.fieldFrameWidth = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldFrameWidth.LabelShortText = Res.Strings.Panel.Frame.Short.FrameWidth;
			this.fieldFrameWidth.LabelLongText  = Res.Strings.Panel.Frame.Long.FrameWidth;
			this.fieldFrameWidth.ShortLongLabelLimit = 50;
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
			ToolTip.Default.SetToolTip (this.fieldFrameColor, Res.Strings.Panel.Frame.Tooltip.FrameColor);

			this.fieldMarginWidth = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldMarginWidth.LabelShortText = Res.Strings.Panel.Frame.Short.MarginWidth;
			this.fieldMarginWidth.LabelLongText  = Res.Strings.Panel.Frame.Long.MarginWidth;
			this.fieldMarginWidth.ShortLongLabelLimit = 50;
			this.fieldMarginWidth.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldMarginWidth.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldMarginWidth.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension (this.fieldMarginWidth.TextFieldReal);
			this.fieldMarginWidth.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldMarginWidth.TabIndex = 4;
			this.fieldMarginWidth.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldMarginWidth, Res.Strings.Panel.Frame.Tooltip.MarginWidth);

			this.fieldBackgroundColor = new ColorSample (this);
			this.fieldBackgroundColor.DragSourceFrame = true;
			this.fieldBackgroundColor.Clicked += this.HandleFieldColorClicked;
			this.fieldBackgroundColor.ColorChanged += this.HandleFieldColorChanged;
			this.fieldBackgroundColor.TabIndex = 5;
			this.fieldBackgroundColor.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldBackgroundColor, Res.Strings.Panel.Frame.Tooltip.BackgroundColor);

			this.fieldMarginConcavity = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldMarginConcavity.LabelShortText = Res.Strings.Panel.Frame.Short.MarginConcavity;
			this.fieldMarginConcavity.LabelLongText  = Res.Strings.Panel.Frame.Long.MarginConcavity;
			this.fieldMarginConcavity.ShortLongLabelLimit = 50;
			this.fieldMarginConcavity.TextFieldReal.FactorMinRange = -0.1M;
			this.fieldMarginConcavity.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldMarginConcavity.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension (this.fieldMarginConcavity.TextFieldReal);
			this.fieldMarginConcavity.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldMarginConcavity.TabIndex = 6;
			this.fieldMarginConcavity.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldMarginConcavity, Res.Strings.Panel.Frame.Tooltip.MarginConcavity);

			this.fieldShadowInflate = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldShadowInflate.LabelShortText = Res.Strings.Panel.Frame.Short.ShadowInflate;
			this.fieldShadowInflate.LabelLongText  = Res.Strings.Panel.Frame.Long.ShadowInflate;
			this.fieldShadowInflate.ShortLongLabelLimit = 50;
			this.fieldShadowInflate.TextFieldReal.FactorMinRange = -0.1M;
			this.fieldShadowInflate.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldShadowInflate.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension (this.fieldShadowInflate.TextFieldReal);
			this.fieldShadowInflate.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldShadowInflate.TabIndex = 7;
			this.fieldShadowInflate.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldShadowInflate, Res.Strings.Panel.Frame.Tooltip.ShadowInflate);

			this.fieldShadowColor = new ColorSample (this);
			this.fieldShadowColor.DragSourceFrame = true;
			this.fieldShadowColor.Clicked += this.HandleFieldColorClicked;
			this.fieldShadowColor.ColorChanged += this.HandleFieldColorChanged;
			this.fieldShadowColor.TabIndex = 8;
			this.fieldShadowColor.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldShadowColor, Res.Strings.Panel.Frame.Tooltip.ShadowColor);

			this.fieldShadowSize = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldShadowSize.LabelShortText = Res.Strings.Panel.Frame.Short.ShadowSize;
			this.fieldShadowSize.LabelLongText  = Res.Strings.Panel.Frame.Long.ShadowSize;
			this.fieldShadowSize.ShortLongLabelLimit = 50;
			this.fieldShadowSize.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldShadowSize.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldShadowSize.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension (this.fieldShadowSize.TextFieldReal);
			this.fieldShadowSize.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldShadowSize.TabIndex = 9;
			this.fieldShadowSize.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldShadowSize, Res.Strings.Panel.Frame.Tooltip.ShadowSize);

			this.fieldShadowConcavity = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldShadowConcavity.LabelShortText = Res.Strings.Panel.Frame.Short.ShadowConcavity;
			this.fieldShadowConcavity.LabelLongText  = Res.Strings.Panel.Frame.Long.ShadowConcavity;
			this.fieldShadowConcavity.ShortLongLabelLimit = 50;
			this.fieldShadowConcavity.TextFieldReal.FactorMinRange = -0.1M;
			this.fieldShadowConcavity.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldShadowConcavity.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension (this.fieldShadowConcavity.TextFieldReal);
			this.fieldShadowConcavity.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldShadowConcavity.TabIndex = 10;
			this.fieldShadowConcavity.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldShadowConcavity, Res.Strings.Panel.Frame.Tooltip.ShadowConcavity);

			this.fieldShadowOffsetX = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldShadowOffsetX.LabelShortText = Res.Strings.Panel.Frame.Short.ShadowOffsetX;
			this.fieldShadowOffsetX.LabelLongText  = Res.Strings.Panel.Frame.Long.ShadowOffsetX;
			this.fieldShadowOffsetX.ShortLongLabelLimit = 50;
			this.fieldShadowOffsetX.TextFieldReal.FactorMinRange = -0.1M;
			this.fieldShadowOffsetX.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldShadowOffsetX.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension (this.fieldShadowOffsetX.TextFieldReal);
			this.fieldShadowOffsetX.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldShadowOffsetX.TabIndex = 11;
			this.fieldShadowOffsetX.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldShadowOffsetX, Res.Strings.Panel.Frame.Tooltip.ShadowOffsetX);

			this.fieldShadowOffsetY = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldShadowOffsetY.LabelShortText = Res.Strings.Panel.Frame.Short.ShadowOffsetY;
			this.fieldShadowOffsetY.LabelLongText  = Res.Strings.Panel.Frame.Long.ShadowOffsetY;
			this.fieldShadowOffsetY.ShortLongLabelLimit = 50;
			this.fieldShadowOffsetY.TextFieldReal.FactorMinRange = -0.1M;
			this.fieldShadowOffsetY.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldShadowOffsetY.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension (this.fieldShadowOffsetY.TextFieldReal);
			this.fieldShadowOffsetY.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldShadowOffsetY.TabIndex = 12;
			this.fieldShadowOffsetY.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldShadowOffsetY, Res.Strings.Panel.Frame.Tooltip.ShadowOffsetY);

			this.isNormalAndExtended = true;
		}

		protected void AddRadioTypeIcon(Properties.FrameType type)
		{
			this.gridType.AddRadioIcon (Misc.Icon (Properties.Frame.GetIconText (type)), Properties.Frame.GetName (type), (int) type, false);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.gridSamples.SelectionChanged -= HandleSampleChanged;
				this.gridType.SelectionChanged -= HandleTypeChanged;

				this.fieldFrameWidth.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldFrameColor.Clicked += this.HandleFieldColorClicked;
				this.fieldFrameColor.ColorChanged += this.HandleFieldColorChanged;

				this.fieldMarginWidth.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldMarginConcavity.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldBackgroundColor.Clicked += this.HandleFieldColorClicked;
				this.fieldBackgroundColor.ColorChanged += this.HandleFieldColorChanged;

				this.fieldShadowSize.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldShadowSize.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldShadowColor.Clicked += this.HandleFieldColorClicked;
				this.fieldShadowColor.ColorChanged += this.HandleFieldColorChanged;

				this.fieldShadowOffsetX.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldShadowOffsetY.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;

				this.gridSamples = null;
				this.gridType = null;
				this.fieldFrameWidth = null;
				this.fieldFrameColor = null;
				this.fieldMarginWidth = null;
				this.fieldMarginConcavity = null;
				this.fieldBackgroundColor = null;
				this.fieldShadowSize = null;
				this.fieldShadowSize = null;
				this.fieldShadowColor = null;
				this.fieldShadowOffsetX = null;
				this.fieldShadowOffsetY = null;
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
					h += 30+25*8+10;
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

			this.gridType.SelectedValue = (int) p.FrameType;

			this.fieldFrameWidth.TextFieldReal.InternalValue = (decimal) p.FrameWidth;
			this.fieldFrameColor.Color = p.FrameColor;

			this.fieldMarginWidth.TextFieldReal.InternalValue = (decimal) p.MarginWidth;
			this.fieldMarginConcavity.TextFieldReal.InternalValue = (decimal) p.MarginConcavity;
			this.fieldBackgroundColor.Color = p.BackgroundColor;

			this.fieldShadowInflate.TextFieldReal.InternalValue = (decimal) p.ShadowInflate;
			this.fieldShadowConcavity.TextFieldReal.InternalValue = (decimal) p.ShadowConcavity;
			this.fieldShadowSize.TextFieldReal.InternalValue = (decimal) p.ShadowSize;
			this.fieldShadowColor.Color = p.ShadowColor;

			this.fieldShadowOffsetX.TextFieldReal.InternalValue = (decimal) p.ShadowOffsetX;
			this.fieldShadowOffsetY.TextFieldReal.InternalValue = (decimal) p.ShadowOffsetY;

			this.UpdateGridSample ();

			this.EnableWidgets ();
			this.ignoreChanged = false;
		}

		protected override void WidgetsToProperty()
		{
			//	Widgets -> propriété.
			var p = this.property as Properties.Frame;
			if ( p == null )  return;

			p.FrameType = (Properties.FrameType) this.gridType.SelectedValue;

			p.FrameWidth = (double) this.fieldFrameWidth.TextFieldReal.InternalValue;
			p.FrameColor = this.fieldFrameColor.Color;

			p.MarginWidth = (double) this.fieldMarginWidth.TextFieldReal.InternalValue;
			p.MarginConcavity = (double) this.fieldMarginConcavity.TextFieldReal.InternalValue;
			p.BackgroundColor = this.fieldBackgroundColor.Color;

			p.ShadowInflate = (double) this.fieldShadowInflate.TextFieldReal.InternalValue;
			p.ShadowConcavity = (double) this.fieldShadowConcavity.TextFieldReal.InternalValue;
			p.ShadowSize = (double) this.fieldShadowSize.TextFieldReal.InternalValue;
			p.ShadowColor = this.fieldShadowColor.Color;

			p.ShadowOffsetX = (double) this.fieldShadowOffsetX.TextFieldReal.InternalValue;
			p.ShadowOffsetY = (double) this.fieldShadowOffsetY.TextFieldReal.InternalValue;

			this.UpdateGridSample ();
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
			this.fieldBackgroundColor.ActiveState = ActiveState.No;
			this.fieldShadowColor.ActiveState = ActiveState.No;
		}

		public override void OriginColorSelect(int rank)
		{
			//	Sélectionne l'origine de couleur.
			if ( rank != -1 )
			{
				this.originFieldRank = rank;
				     if ( rank == 0 )  this.originFieldColor = this.fieldFrameColor;
				else if ( rank == 1 )  this.originFieldColor = this.fieldBackgroundColor;
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


		protected void UpdateGridSample()
		{
			var frame = this.property as Properties.Frame;

			int sel = -1;
			int rank = 0;
			foreach (Sample sample in Frame.Samples)
			{
				if (sample.Compare (frame))
				{
					sel = rank;;
					break;
				}

				rank++;
			}

			this.ignoreChanged = true;
			this.gridSamples.SelectedValue = sel;
			this.ignoreChanged = false;
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.gridType == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Inflate (1);
			this.gridSamples.SetManualBounds (r);

			rect.Offset (0, -25);

			r = rect;
			r.Bottom = r.Top-20;
			r.Inflate (1);
			this.gridType.SetManualBounds (r);

			if (this.isExtendedSize)
			{
				r.Offset (0, -25);
				Frame.UpdateFieldAndColorGeometry (this.fieldFrameWidth, this.fieldFrameColor, r);

				r.Offset (0, -25);
				Frame.UpdateFieldAndColorGeometry (this.fieldMarginWidth, this.fieldBackgroundColor, r);

				r.Offset (0, -25);
				Frame.UpdateFieldAndColorGeometry (this.fieldMarginConcavity, null, r);

				r.Offset (0, -25-10);
				Frame.UpdateFieldAndColorGeometry (this.fieldShadowInflate, this.fieldShadowColor, r);

				r.Offset (0, -25);
				Frame.UpdateFieldAndColorGeometry (this.fieldShadowSize, null, r);

				r.Offset (0, -25);
				Frame.UpdateFieldAndColorGeometry (this.fieldShadowConcavity, null, r);

				r.Offset (0, -25);
				Frame.UpdateFieldAndColorGeometry (this.fieldShadowOffsetX, this.fieldShadowOffsetY, r);

				var type = (Properties.FrameType) this.gridType.SelectedValue;

				bool frame = (type == Properties.FrameType.OnlyFrame || type == Properties.FrameType.FrameAndShadow);
				this.fieldFrameWidth.Enable = frame;
				this.fieldFrameColor.Enable = frame;
				this.fieldMarginWidth.Enable = frame;
				this.fieldMarginConcavity.Enable = frame;
				this.fieldBackgroundColor.Enable = frame;

				bool shadow = (type == Properties.FrameType.OnlyShadow || type == Properties.FrameType.FrameAndShadow);
				this.fieldShadowInflate.Enable = shadow;
				this.fieldShadowConcavity.Enable = shadow;
				this.fieldShadowSize.Enable = shadow;
				this.fieldShadowColor.Enable = shadow;
				this.fieldShadowOffsetX.Enable = shadow;
				this.fieldShadowOffsetY.Enable = shadow;
			}
			else
			{
				this.fieldFrameWidth.Visibility = false;
				this.fieldFrameColor.Visibility = false;

				this.fieldMarginWidth.Visibility = false;
				this.fieldMarginConcavity.Visibility = false;
				this.fieldBackgroundColor.Visibility = false;

				this.fieldShadowInflate.Visibility = false;
				this.fieldShadowConcavity.Visibility = false;
				this.fieldShadowSize.Visibility = false;
				this.fieldShadowColor.Visibility = false;

				this.fieldShadowOffsetX.Visibility = false;
				this.fieldShadowOffsetY.Visibility = false;
			}
		}

		private static void UpdateFieldAndColorGeometry(Widget field, Widget sample, Rectangle rect)
		{
			rect.Width -= Widgets.TextFieldLabel.DefaultTextWidth+5;
			field.SetManualBounds (rect);
			field.Visibility = true;

			if (sample != null)
			{
				rect.Left = rect.Right+5;
				rect.Width = Widgets.TextFieldLabel.DefaultTextWidth;
				sample.SetManualBounds (rect);
				sample.Visibility = true;
			}
		}


		private void HandleSampleChanged(object sender)
		{
			//	L'exemple a changé.
			if (this.ignoreChanged)
			{
				return;
			}

			int rank = this.gridSamples.SelectedValue;
			var sample = Frame.Samples[rank];

			this.gridType.SelectedValue = (int) sample.FrameType;

			decimal factor = 1.0M;

			if (this.document.Type == DocumentType.Pictogram)
			{
				factor = 0.2M;
			}
			else
			{
				if (!System.Globalization.RegionInfo.CurrentRegion.IsMetric)
				{
					factor = 1.27M;  // en inch
				}
			}

			this.fieldFrameWidth.TextFieldReal.InternalValue = (decimal) sample.FrameWidth*factor;
			this.fieldMarginWidth.TextFieldReal.InternalValue = (decimal) sample.MarginWidth*factor;
			this.fieldMarginConcavity.TextFieldReal.InternalValue = (decimal) sample.MarginConcavity*factor;
			this.fieldShadowInflate.TextFieldReal.InternalValue = (decimal) sample.ShadowInflate*factor;
			this.fieldShadowConcavity.TextFieldReal.InternalValue = (decimal) sample.ShadowConcavity*factor;
			this.fieldShadowSize.TextFieldReal.InternalValue = (decimal) sample.ShadowSize*factor;
			this.fieldShadowOffsetX.TextFieldReal.InternalValue = (decimal) sample.ShadowOffsetX*factor;
			this.fieldShadowOffsetY.TextFieldReal.InternalValue = (decimal) sample.ShadowOffsetY*factor;

			this.OnChanged ();
		}

		private void HandleTypeChanged(object sender)
		{
			//	Le type a été changé.
			if (this.ignoreChanged)
			{
				return;
			}

			var type = (Properties.FrameType) this.gridType.SelectedValue;

			if (type == Properties.FrameType.OnlyFrame ||
				type == Properties.FrameType.FrameAndShadow)
			{
				if (this.fieldFrameWidth.TextFieldReal.InternalValue == 0)
				{
					this.fieldFrameWidth.TextFieldReal.InternalValue = 2.0M;
				}
			}

			if (type == Properties.FrameType.OnlyShadow ||
				type == Properties.FrameType.FrameAndShadow)
			{
				if (this.fieldShadowSize.TextFieldReal.InternalValue == 0)
				{
					this.fieldShadowSize.TextFieldReal.InternalValue = 40.0M;
				}
			}

			this.EnableWidgets ();
			this.OnChanged ();
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
			if (this.originFieldColor == this.fieldBackgroundColor)  this.originFieldRank = 1;
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


		#region Sample
		/// <summary>
		/// La structure Sample permet de décrire un exemple de cadre.
		/// </summary>
		private struct Sample
		{
			//	Constructeur d'un exemple.
			public Sample(string text, string icon, Properties.FrameType frameType, double frameWidth, double marginWidth, double marginConcavity, double shadowInflate, double shadowConcavity, double shadowSize, double shadowOffsetX, double shadowOffsetY)
			{
				this.Text            = text;
				this.Icon            = icon;
				this.FrameType       = frameType;
				this.FrameWidth      = frameWidth;
				this.MarginWidth     = marginWidth;
				this.MarginConcavity = marginConcavity;
				this.ShadowInflate   = shadowInflate;
				this.ShadowConcavity = shadowConcavity;
				this.ShadowSize      = shadowSize;
				this.ShadowOffsetX   = shadowOffsetX;
				this.ShadowOffsetY   = shadowOffsetY;
			}

			public bool Compare(Properties.Frame frame)
			{
				//	Compare un exemple avec une propriété.
				return (frame.FrameType       == this.FrameType       &&
						frame.FrameWidth      == this.FrameWidth      &&
						frame.MarginWidth     == this.MarginWidth     &&
						frame.MarginConcavity == this.MarginConcavity &&
						frame.ShadowInflate   == this.ShadowInflate   &&
						frame.ShadowConcavity == this.ShadowConcavity &&
						frame.ShadowSize      == this.ShadowSize      &&
						frame.ShadowOffsetX   == this.ShadowOffsetX   &&
						frame.ShadowOffsetY   == this.ShadowOffsetY   );
			}

			public string					Text;
			public string					Icon;
			public Properties.FrameType		FrameType;
			public double					FrameWidth;
			public double					MarginWidth;
			public double					MarginConcavity;
			public double					ShadowInflate;
			public double					ShadowConcavity;
			public double					ShadowSize;
			public double					ShadowOffsetX;
			public double					ShadowOffsetY;
		}

		//	Liste des exemples accessibles avec la liste déroulante.
		static private Sample[] Samples =
		{
			//																									<-------frame------> <------------shadow-------------->
			new Sample (Res.Strings.Panel.Frame.Sample01, "FrameSample01", Properties.FrameType.None,             0.0,   0.0,   0.0,   0.0,   0.0,   0.0,   0.0,   0.0),  // pas de cadre
			new Sample (Res.Strings.Panel.Frame.Sample02, "FrameSample02", Properties.FrameType.OnlyFrame,        2.0,   0.0,   0.0,   0.0,   0.0,   0.0,   0.0,   0.0),  // cadre fin sans bordure
			new Sample (Res.Strings.Panel.Frame.Sample03, "FrameSample03", Properties.FrameType.FrameAndShadow,   2.0,   0.0,   0.0,   0.0, -10.0,  20.0,   0.0,   0.0),  // cadre fin sans bordure avec halo
			new Sample (Res.Strings.Panel.Frame.Sample04, "FrameSample04", Properties.FrameType.OnlyFrame,       10.0,   0.0,   0.0,   0.0,   0.0,   0.0,   0.0,   0.0),  // cadre épais sans bordure
			new Sample (Res.Strings.Panel.Frame.Sample05, "FrameSample05", Properties.FrameType.FrameAndShadow,   2.0,  50.0,   0.0,   0.0, -10.0,  20.0,   0.0,   0.0),  // cadre avec bordure et halo
			new Sample (Res.Strings.Panel.Frame.Sample06, "FrameSample06", Properties.FrameType.FrameAndShadow,   2.0,  50.0,   0.0, -20.0,   0.0,  40.0,  20.0, -20.0),  // cadre avec bordure et ombre
			new Sample (Res.Strings.Panel.Frame.Sample07, "FrameSample07", Properties.FrameType.OnlyShadow,       0.0,   0.0,   0.0, -20.0,   0.0,  40.0,  20.0, -20.0),  // petite ombre
			new Sample (Res.Strings.Panel.Frame.Sample08, "FrameSample08", Properties.FrameType.OnlyShadow,       0.0,   0.0,   0.0, -40.0,   0.0,  80.0,  40.0, -40.0),  // grande ombre
		};
		#endregion


		protected RadioIconGrid				gridSamples;
		protected RadioIconGrid				gridType;

		protected Widgets.TextFieldLabel	fieldFrameWidth;
		protected ColorSample				fieldFrameColor;

		protected Widgets.TextFieldLabel	fieldMarginWidth;
		protected Widgets.TextFieldLabel	fieldMarginConcavity;
		protected ColorSample				fieldBackgroundColor;

		protected Widgets.TextFieldLabel	fieldShadowInflate;
		protected Widgets.TextFieldLabel	fieldShadowConcavity;
		protected Widgets.TextFieldLabel	fieldShadowSize;
		protected ColorSample				fieldShadowColor;
		protected Widgets.TextFieldLabel	fieldShadowOffsetX;
		protected Widgets.TextFieldLabel	fieldShadowOffsetY;

		protected ColorSample				originFieldColor;
		protected int						originFieldRank = -1;
	}
}
