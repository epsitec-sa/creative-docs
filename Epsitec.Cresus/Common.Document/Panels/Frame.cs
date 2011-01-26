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
			this.fieldSamples = new TextFieldCombo (this);
			this.fieldSamples.IsReadOnly = true;
			foreach (Sample sample in Frame.Samples)
			{
				this.fieldSamples.Items.Add (sample.Text);
			}
			this.fieldSamples.TextChanged += this.HandleFieldSamplesTextChanged;
			this.fieldSamples.TabIndex = 0;

			this.grid = new RadioIconGrid (this);
			this.grid.SelectionChanged += this.HandleTypeChanged;
			this.grid.TabIndex = 1;
			this.grid.TabNavigationMode = TabNavigationMode.ActivateOnTab;

			this.AddRadioIcon (Properties.FrameType.None);
			this.AddRadioIcon (Properties.FrameType.OnlyFrame);
			this.AddRadioIcon (Properties.FrameType.FrameAndShadow);
			this.AddRadioIcon (Properties.FrameType.OnlyShadow);

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

			this.fieldShadowInflate = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldShadowInflate.LabelShortText = Res.Strings.Panel.Frame.Short.ShadowInflate;
			this.fieldShadowInflate.ShortLongLabelLimit = 50;
			this.fieldShadowInflate.LabelLongText  = Res.Strings.Panel.Frame.Long.ShadowInflate;
			this.fieldShadowInflate.TextFieldReal.FactorMinRange = -0.1M;
			this.fieldShadowInflate.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldShadowInflate.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension (this.fieldShadowInflate.TextFieldReal);
			this.fieldShadowInflate.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldShadowInflate.TabIndex = 6;
			this.fieldShadowInflate.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldShadowInflate, Res.Strings.Panel.Frame.Tooltip.ShadowInflate);

			this.fieldShadowColor = new ColorSample (this);
			this.fieldShadowColor.DragSourceFrame = true;
			this.fieldShadowColor.Clicked += this.HandleFieldColorClicked;
			this.fieldShadowColor.ColorChanged += this.HandleFieldColorChanged;
			this.fieldShadowColor.TabIndex = 7;
			this.fieldShadowColor.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldShadowColor, Res.Strings.Panel.Frame.Tooltip.ShadowColor);

			this.fieldShadowSize = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldShadowSize.LabelShortText = Res.Strings.Panel.Frame.Short.ShadowSize;
			this.fieldShadowSize.ShortLongLabelLimit = 50;
			this.fieldShadowSize.LabelLongText  = Res.Strings.Panel.Frame.Long.ShadowSize;
			this.fieldShadowSize.TextFieldReal.FactorMinRange = 0.0M;
			this.fieldShadowSize.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldShadowSize.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension (this.fieldShadowSize.TextFieldReal);
			this.fieldShadowSize.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldShadowSize.TabIndex = 8;
			this.fieldShadowSize.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldShadowSize, Res.Strings.Panel.Frame.Tooltip.ShadowSize);

			this.fieldShadowOffsetX = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldShadowOffsetX.LabelShortText = Res.Strings.Panel.Frame.Short.ShadowOffsetX;
			this.fieldShadowOffsetX.ShortLongLabelLimit = 50;
			this.fieldShadowOffsetX.LabelLongText  = Res.Strings.Panel.Frame.Long.ShadowOffsetX;
			this.fieldShadowOffsetX.TextFieldReal.FactorMinRange = -0.1M;
			this.fieldShadowOffsetX.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldShadowOffsetX.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension (this.fieldShadowOffsetX.TextFieldReal);
			this.fieldShadowOffsetX.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldShadowOffsetX.TabIndex = 9;
			this.fieldShadowOffsetX.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldShadowOffsetX, Res.Strings.Panel.Frame.Tooltip.ShadowOffsetX);

			this.fieldShadowOffsetY = new Widgets.TextFieldLabel (this, Widgets.TextFieldLabel.Type.TextFieldReal);
			this.fieldShadowOffsetY.LabelShortText = Res.Strings.Panel.Frame.Short.ShadowOffsetY;
			this.fieldShadowOffsetY.ShortLongLabelLimit = 50;
			this.fieldShadowOffsetY.LabelLongText  = Res.Strings.Panel.Frame.Long.ShadowOffsetY;
			this.fieldShadowOffsetY.TextFieldReal.FactorMinRange = -0.1M;
			this.fieldShadowOffsetY.TextFieldReal.FactorMaxRange = 0.1M;
			this.fieldShadowOffsetY.TextFieldReal.FactorStep = 1.0M;
			this.document.Modifier.AdaptTextFieldRealDimension (this.fieldShadowOffsetY.TextFieldReal);
			this.fieldShadowOffsetY.TextFieldReal.EditionAccepted += this.HandleFieldChanged;
			this.fieldShadowOffsetY.TabIndex = 10;
			this.fieldShadowOffsetY.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip (this.fieldShadowOffsetY, Res.Strings.Panel.Frame.Tooltip.ShadowOffsetY);

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
				this.fieldBackgroundColor.Clicked += this.HandleFieldColorClicked;
				this.fieldBackgroundColor.ColorChanged += this.HandleFieldColorChanged;

				this.fieldShadowSize.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldShadowSize.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldShadowColor.Clicked += this.HandleFieldColorClicked;
				this.fieldShadowColor.ColorChanged += this.HandleFieldColorChanged;

				this.fieldShadowOffsetX.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;
				this.fieldShadowOffsetY.TextFieldReal.EditionAccepted -= this.HandleFieldChanged;

				this.grid = null;
				this.fieldFrameWidth = null;
				this.fieldFrameColor = null;
				this.fieldMarginWidth = null;
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
					h += 30+25*6;
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
			this.fieldBackgroundColor.Color = p.BackgroundColor;

			this.fieldShadowInflate.TextFieldReal.InternalValue = (decimal) p.ShadowInflate;
			this.fieldShadowSize.TextFieldReal.InternalValue = (decimal) p.ShadowSize;
			this.fieldShadowColor.Color = p.ShadowColor;

			this.fieldShadowOffsetX.TextFieldReal.InternalValue = (decimal) p.ShadowOffsetX;
			this.fieldShadowOffsetY.TextFieldReal.InternalValue = (decimal) p.ShadowOffsetY;

			this.UpdateFieldSample ();

			this.EnableWidgets ();
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
			p.BackgroundColor = this.fieldBackgroundColor.Color;

			p.ShadowInflate = (double) this.fieldShadowInflate.TextFieldReal.InternalValue;
			p.ShadowSize = (double) this.fieldShadowSize.TextFieldReal.InternalValue;
			p.ShadowColor = this.fieldShadowColor.Color;

			p.ShadowOffsetX = (double) this.fieldShadowOffsetX.TextFieldReal.InternalValue;
			p.ShadowOffsetY = (double) this.fieldShadowOffsetY.TextFieldReal.InternalValue;

			this.UpdateFieldSample ();
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


		protected void UpdateFieldSample()
		{
			var frame = this.property as Properties.Frame;

			string text = Res.Strings.Panel.Frame.Custom;
			foreach (Sample sample in Frame.Samples)
			{
				if (sample.Compare (frame))
				{
					text = sample.Text;
					break;
				}
			}

			this.ignoreChanged = true;
			this.fieldSamples.Text = text;
			this.ignoreChanged = false;
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.grid == null )  return;

			Rectangle rect = this.UsefulZone;

			Rectangle r = rect;
			r.Bottom = r.Top-20;
			r.Left = rect.Left;
			r.Right = rect.Right;
			this.fieldSamples.SetManualBounds (r);

			rect.Top = r.Bottom-5;
			rect.Bottom = rect.Top-20;
			r = rect;
			r.Width = 22*4;
			r.Inflate (1);
			this.grid.SetManualBounds (r);

			if (this.isExtendedSize)
			{
				r.Top = rect.Top-25;
				r.Bottom = r.Top-20;
				r.Left = rect.Left;
				r.Right = rect.Right;

				Frame.UpdateFieldAndColorGeometry (this.fieldFrameWidth, this.fieldFrameColor, r);

				r.Offset (0, -25);
				Frame.UpdateFieldAndColorGeometry (this.fieldMarginWidth, this.fieldBackgroundColor, r);

				r.Offset (0, -25);
				Frame.UpdateFieldAndColorGeometry (this.fieldShadowInflate, this.fieldShadowColor, r);

				r.Offset (0, -25);
				Frame.UpdateFieldAndColorGeometry (this.fieldShadowSize, null, r);

				r.Offset (0, -25);
				Frame.UpdateFieldAndColorGeometry (this.fieldShadowOffsetX, this.fieldShadowOffsetY, r);

				var type = (Properties.FrameType) this.grid.SelectedValue;

				bool frame = (type == Properties.FrameType.OnlyFrame || type == Properties.FrameType.FrameAndShadow);
				this.fieldFrameWidth.Enable = frame;
				this.fieldFrameColor.Enable = frame;
				this.fieldMarginWidth.Enable = frame;
				this.fieldBackgroundColor.Enable = frame;

				bool shadow = (type == Properties.FrameType.OnlyShadow || type == Properties.FrameType.FrameAndShadow);
				this.fieldShadowInflate.Enable = shadow;
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
				this.fieldBackgroundColor.Visibility = false;

				this.fieldShadowInflate.Visibility = false;
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


		private void HandleTypeChanged(object sender)
		{
			//	Le type a été changé.
			if (this.ignoreChanged)
			{
				return;
			}

			var type = (Properties.FrameType) this.grid.SelectedValue;

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

		private void HandleFieldSamplesTextChanged(object sender)
		{
			if (this.ignoreChanged)
			{
				return;
			}

			TextFieldCombo field = sender as TextFieldCombo;
			foreach (Sample sample in Frame.Samples)
			{
				if (field.Text == sample.Text)
				{
					this.grid.SelectedValue = (int) sample.FrameType;

					this.fieldFrameWidth.TextFieldReal.InternalValue = (decimal) sample.FrameWidth;
					this.fieldMarginWidth.TextFieldReal.InternalValue = (decimal) sample.MarginWidth;
					this.fieldShadowInflate.TextFieldReal.InternalValue = (decimal) sample.ShadowInflate;
					this.fieldShadowSize.TextFieldReal.InternalValue = (decimal) sample.ShadowSize;
					this.fieldShadowOffsetX.TextFieldReal.InternalValue = (decimal) sample.ShadowOffsetX;
					this.fieldShadowOffsetY.TextFieldReal.InternalValue = (decimal) sample.ShadowOffsetY;

					this.OnChanged ();
					return;
				}
			}
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
			public Sample(string text, Properties.FrameType frameType, double frameWidth, double marginWidth, double shadowInflate, double shadowSize, double shadowOffsetX, double shadowOffsetY)
			{
				this.Text          = text;
				this.FrameType     = frameType;
				this.FrameWidth    = frameWidth;
				this.MarginWidth   = marginWidth;
				this.ShadowInflate = shadowInflate;
				this.ShadowSize    = shadowSize;
				this.ShadowOffsetX = shadowOffsetX;
				this.ShadowOffsetY = shadowOffsetY;
			}

			public bool Compare(Properties.Frame frame)
			{
				//	Compare un exemple avec une propriété.
				return (frame.FrameType     == this.FrameType     &&
						frame.FrameWidth    == this.FrameWidth    &&
						frame.MarginWidth   == this.MarginWidth   &&
						frame.ShadowInflate == this.ShadowInflate &&
						frame.ShadowSize    == this.ShadowSize    &&
						frame.ShadowOffsetX == this.ShadowOffsetX &&
						frame.ShadowOffsetY == this.ShadowOffsetY);
			}

			public string					Text;
			public Properties.FrameType		FrameType;
			public double					FrameWidth;
			public double					MarginWidth;
			public double					ShadowInflate;
			public double					ShadowSize;
			public double					ShadowOffsetX;
			public double					ShadowOffsetY;
		}

		//	Liste des exemples accessibles avec la liste déroulante.
		static private Sample[] Samples =
		{
			new Sample(Res.Strings.Panel.Frame.Sample01, Properties.FrameType.None,             0.0,   0.0,   0.0,   0.0,   0.0,   0.0),

			new Sample(Res.Strings.Panel.Frame.Sample02, Properties.FrameType.OnlyFrame,        2.0,   0.0,   0.0,   0.0,   0.0,   0.0),
			new Sample(Res.Strings.Panel.Frame.Sample03, Properties.FrameType.OnlyFrame,       10.0,   0.0,   0.0,   0.0,   0.0,   0.0),
			new Sample(Res.Strings.Panel.Frame.Sample04, Properties.FrameType.OnlyFrame,        2.0,  50.0,   0.0,   0.0,   0.0,   0.0),
			new Sample(Res.Strings.Panel.Frame.Sample05, Properties.FrameType.OnlyFrame,       10.0,  50.0,   0.0,   0.0,   0.0,   0.0),
			
			new Sample(Res.Strings.Panel.Frame.Sample06, Properties.FrameType.FrameAndShadow,   2.0,   0.0,   0.0,  20.0,   0.0,   0.0),
			new Sample(Res.Strings.Panel.Frame.Sample07, Properties.FrameType.FrameAndShadow,  10.0,   0.0,   0.0,  20.0,   0.0,   0.0),
			new Sample(Res.Strings.Panel.Frame.Sample08, Properties.FrameType.FrameAndShadow,   2.0,  50.0,   0.0,  20.0,   0.0,   0.0),
			new Sample(Res.Strings.Panel.Frame.Sample09, Properties.FrameType.FrameAndShadow,  10.0,  50.0,   0.0,  20.0,   0.0,   0.0),
			
			new Sample(Res.Strings.Panel.Frame.Sample10, Properties.FrameType.OnlyShadow,       0.0,   0.0, -20.0,  40.0,   0.0,   0.0),
			new Sample(Res.Strings.Panel.Frame.Sample11, Properties.FrameType.OnlyShadow,       0.0,   0.0, -20.0,  40.0,  40.0, -40.0),
			new Sample(Res.Strings.Panel.Frame.Sample12, Properties.FrameType.OnlyShadow,       0.0,   0.0, -20.0,  40.0, -40.0, -40.0),
			new Sample(Res.Strings.Panel.Frame.Sample13, Properties.FrameType.OnlyShadow,       0.0,   0.0, -20.0,  40.0,  40.0,  40.0),
			new Sample(Res.Strings.Panel.Frame.Sample14, Properties.FrameType.OnlyShadow,       0.0,   0.0, -20.0,  40.0, -40.0,  40.0),
		};
		#endregion


		protected TextFieldCombo			fieldSamples;
		protected RadioIconGrid				grid;

		protected Widgets.TextFieldLabel	fieldFrameWidth;
		protected ColorSample				fieldFrameColor;

		protected Widgets.TextFieldLabel	fieldMarginWidth;
		protected ColorSample				fieldBackgroundColor;

		protected Widgets.TextFieldLabel	fieldShadowInflate;
		protected Widgets.TextFieldLabel	fieldShadowSize;
		protected ColorSample				fieldShadowColor;
		protected Widgets.TextFieldLabel	fieldShadowOffsetX;
		protected Widgets.TextFieldLabel	fieldShadowOffsetY;

		protected ColorSample				originFieldColor;
		protected int						originFieldRank = -1;
	}
}
