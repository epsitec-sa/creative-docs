using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Xline permet de choisir les soulignements.
	/// </summary>
	[SuppressBundleSupport]
	public class Xline : Abstract
	{
		public Xline(Document document) : base(document)
		{
			this.label.Text = Res.Strings.TextPanel.Xline.Title;

			this.fixIcon.Text = Misc.Image("TextXline");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Xline.Title);

			this.buttonUnderlined = this.CreateIconButton(Misc.Icon("FontUnderlined"), Res.Strings.Action.Text.Font.Underlined, new MessageEventHandler(this.HandleButtonUnderlineClicked));
			this.buttonOverlined  = this.CreateIconButton(Misc.Icon("FontOverlined"),  Res.Strings.Action.Text.Font.Overlined,  new MessageEventHandler(this.HandleButtonOverlineClicked));
			this.buttonStrikeout  = this.CreateIconButton(Misc.Icon("FontStrikeout"),  Res.Strings.Action.Text.Font.Strikeout,  new MessageEventHandler(this.HandleButtonStrikeoutClicked));

			this.fieldUnderlinedThickness = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Underlined.Thickness, Res.Strings.TextPanel.Xline.Short.Underlined.Thickness, Res.Strings.TextPanel.Xline.Long.Underlined.Thickness, 0.0, 0.01, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleUnderlinedValueChanged));
			this.fieldUnderlinedPosition  = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Underlined.Position,  Res.Strings.TextPanel.Xline.Short.Underlined.Position,  Res.Strings.TextPanel.Xline.Long.Underlined.Position,  0.0, 0.01, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleUnderlinedValueChanged));
			this.underlinedColor          = this.CreateColorSample(Res.Strings.TextPanel.Xline.Tooltip.Underlined.Color, new MessageEventHandler(this.HandleSampleColorClicked), new EventHandler(this.HandleSampleColorChanged));

			this.fieldOverlinedThickness  = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Overlined.Thickness,  Res.Strings.TextPanel.Xline.Short.Overlined.Thickness,  Res.Strings.TextPanel.Xline.Long.Overlined.Thickness,  0.0, 0.01, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleOverlinedValueChanged));
			this.fieldOverlinedPosition   = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Overlined.Position,   Res.Strings.TextPanel.Xline.Short.Overlined.Position,   Res.Strings.TextPanel.Xline.Long.Overlined.Position,   0.0, 0.01, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleOverlinedValueChanged));
			this.overlinedColor           = this.CreateColorSample(Res.Strings.TextPanel.Xline.Tooltip.Overlined.Color, new MessageEventHandler(this.HandleSampleColorClicked), new EventHandler(this.HandleSampleColorChanged));

			this.fieldStrikeoutThickness  = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Strikeout.Thickness,  Res.Strings.TextPanel.Xline.Short.Strikeout.Thickness,  Res.Strings.TextPanel.Xline.Long.Strikeout.Thickness,  0.0, 0.01, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleStrikeoutValueChanged));
			this.fieldStrikeoutPosition   = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Strikeout.Position,   Res.Strings.TextPanel.Xline.Short.Strikeout.Position,   Res.Strings.TextPanel.Xline.Long.Strikeout.Position,   0.0, 0.01, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleStrikeoutValueChanged));
			this.strikeoutColor           = this.CreateColorSample(Res.Strings.TextPanel.Xline.Tooltip.Strikeout.Color, new MessageEventHandler(this.HandleSampleColorClicked), new EventHandler(this.HandleSampleColorChanged));
			
			this.buttonClear = this.CreateClearButton(new MessageEventHandler(this.HandleClearClicked));

			this.document.TextWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.document.TextWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.document.TextWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.document.TextWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
			}
			
			base.Dispose(disposing);
		}

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau étendu ?
				{
					int total = 0;
					if ( this.document.TextWrapper.Defined.IsUnderlineDefined )  total ++;
					if ( this.document.TextWrapper.Defined.IsOverlineDefined  )  total ++;
					if ( this.document.TextWrapper.Defined.IsStrikeoutDefined )  total ++;

					if ( this.IsLabelProperties )  // étendu/détails ?
					{
						h += 30 + 25*total*3;
					}
					else	// étendu/compact ?
					{
						h += 30 + 25*total;
					}

					if ( total == 0 )
					{
						h += 20;  // feedback visuel lorsque le panneau est étendu mais vide (à part les IconButtons)
					}
				}
				else	// panneau réduit ?
				{
					h += 30;
				}

				return h;
			}
		}


		// Désélectionne toutes les origines de couleurs possibles.
		public override void OriginColorDeselect()
		{
			this.underlinedColor.ActiveState = ActiveState.No;
			this.overlinedColor.ActiveState  = ActiveState.No;
			this.strikeoutColor.ActiveState  = ActiveState.No;
		}

		// Sélectionne l'origine de couleur.
		public override void OriginColorSelect(int rank)
		{
			if ( rank != -1 )
			{
				this.originFieldRank = rank;
				if ( rank == 0 )  this.originFieldColor = this.underlinedColor;
				if ( rank == 1 )  this.originFieldColor = this.overlinedColor;
				if ( rank == 2 )  this.originFieldColor = this.strikeoutColor;
			}
			if ( this.originFieldColor == null )  return;

			this.OriginColorDeselect();
			this.originFieldColor.ActiveState = ActiveState.Yes;
		}

		// Retourne le rang de la couleur d'origine.
		public override int OriginColorRank()
		{
			return this.originFieldRank;
		}

		// Modifie la couleur d'origine.
		public override void OriginColorChange(Drawing.RichColor color)
		{
			if ( this.originFieldColor == null )  return;
			
			if ( this.originFieldColor.Color != color )
			{
				this.originFieldColor.Color = color;
				this.ColorToWrapper(this.originFieldColor);
			}
		}

		// Donne la couleur d'origine.
		public override Drawing.RichColor OriginColorGet()
		{
			if ( this.originFieldColor == null )  return Drawing.RichColor.FromBrightness(0.0);
			return this.originFieldColor.Color;
		}

		// Donne la couleur au wrapper.
		protected void ColorToWrapper(ColorSample sample)
		{
			if ( sample == this.underlinedColor )
			{
				this.document.TextWrapper.SuspendSynchronisations();
				Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.document.TextWrapper.Defined.Underline;
				this.FillUnderlineDefinition(xline, false);
				this.document.TextWrapper.ResumeSynchronisations();
			}

			if ( sample == this.overlinedColor )
			{
				this.document.TextWrapper.SuspendSynchronisations();
				Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.document.TextWrapper.Defined.Overline;
				this.FillOverlineDefinition(xline, false);
				this.document.TextWrapper.ResumeSynchronisations();
			}

			if ( sample == this.strikeoutColor )
			{
				this.document.TextWrapper.SuspendSynchronisations();
				Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.document.TextWrapper.Defined.Strikeout;
				this.FillStrikeoutDefinition(xline, false);
				this.document.TextWrapper.ResumeSynchronisations();
			}
		}

		
		// Le wrapper associé a changé.
		protected void HandleWrapperChanged(object sender)
		{
			this.UpdateAfterChanging();
		}

		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonUnderlined == null )  return;

			Rectangle rect = this.UsefulZone;

			bool isUnderlined = this.document.TextWrapper.Defined.IsUnderlineDefined;
			bool isOverlined  = this.document.TextWrapper.Defined.IsOverlineDefined;
			bool isStrikeout  = this.document.TextWrapper.Defined.IsStrikeoutDefined;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonUnderlined.Bounds = r;
					r.Offset(20, 0);
					this.buttonOverlined.Bounds = r;
					r.Offset(20+5, 0);
					this.buttonStrikeout.Bounds = r;

					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.Bounds = r;

					if ( isUnderlined )
					{
						r.Offset(0, -25);
						r.Left = rect.Left;
						r.Right = rect.Right-25;
						this.fieldUnderlinedThickness.Bounds = r;
						this.fieldUnderlinedThickness.Visibility = true;
						r.Offset(0, -25);
						this.fieldUnderlinedPosition.Bounds = r;
						this.fieldUnderlinedPosition.Visibility = true;
						r.Offset(0, -25);
						r.Left = rect.Right-25-48;
						r.Width = 48;
						this.underlinedColor.Bounds = r;
						this.underlinedColor.Visibility = true;
					}
					else
					{
						this.fieldUnderlinedThickness.Visibility = false;
						this.fieldUnderlinedPosition.Visibility = false;
						this.underlinedColor.Visibility = false;
					}

					if ( isOverlined )
					{
						r.Offset(0, -25);
						r.Left = rect.Left;
						r.Right = rect.Right-25;
						this.fieldOverlinedThickness.Bounds = r;
						this.fieldOverlinedThickness.Visibility = true;
						r.Offset(0, -25);
						this.fieldOverlinedPosition.Bounds = r;
						this.fieldOverlinedPosition.Visibility = true;
						r.Offset(0, -25);
						r.Left = rect.Right-25-48;
						r.Width = 48;
						this.overlinedColor.Bounds = r;
						this.overlinedColor.Visibility = true;
					}
					else
					{
						this.fieldOverlinedThickness.Visibility = false;
						this.fieldOverlinedPosition.Visibility = false;
						this.overlinedColor.Visibility = false;
					}

					if ( isStrikeout )
					{
						r.Offset(0, -25);
						r.Left = rect.Left;
						r.Right = rect.Right-25;
						this.fieldStrikeoutThickness.Bounds = r;
						this.fieldStrikeoutThickness.Visibility = true;
						r.Offset(0, -25);
						this.fieldStrikeoutPosition.Bounds = r;
						this.fieldStrikeoutPosition.Visibility = true;
						r.Offset(0, -25);
						r.Left = rect.Right-25-48;
						r.Width = 48;
						this.strikeoutColor.Bounds = r;
						this.strikeoutColor.Visibility = true;
					}
					else
					{
						this.fieldStrikeoutThickness.Visibility = false;
						this.fieldStrikeoutPosition.Visibility = false;
						this.strikeoutColor.Visibility = false;
					}
				}
				else
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonUnderlined.Bounds = r;
					r.Offset(20, 0);
					this.buttonOverlined.Bounds = r;
					r.Offset(20+5, 0);
					this.buttonStrikeout.Bounds = r;
					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.Bounds = r;

					if ( isUnderlined )
					{
						r.Offset(0, -25);
						r.Left = rect.Left;
						r.Width = 60;
						this.fieldUnderlinedThickness.Bounds = r;
						this.fieldUnderlinedThickness.Visibility = true;
						r.Offset(60, 0);
						this.fieldUnderlinedPosition.Bounds = r;
						this.fieldUnderlinedPosition.Visibility = true;
						r.Offset(60+12, 0);
						r.Width = 48;
						this.underlinedColor.Bounds = r;
						this.underlinedColor.Visibility = true;
					}
					else
					{
						this.fieldUnderlinedThickness.Visibility = false;
						this.fieldUnderlinedPosition.Visibility = false;
						this.underlinedColor.Visibility = false;
					}

					if ( isOverlined )
					{
						r.Offset(0, -25);
						r.Left = rect.Left;
						r.Width = 60;
						this.fieldOverlinedThickness.Bounds = r;
						this.fieldOverlinedThickness.Visibility = true;
						r.Offset(60, 0);
						this.fieldOverlinedPosition.Bounds = r;
						this.fieldOverlinedPosition.Visibility = true;
						r.Offset(60+12, 0);
						r.Width = 48;
						this.overlinedColor.Bounds = r;
						this.overlinedColor.Visibility = true;
					}
					else
					{
						this.fieldOverlinedThickness.Visibility = false;
						this.fieldOverlinedPosition.Visibility = false;
						this.overlinedColor.Visibility = false;
					}

					if ( isStrikeout )
					{
						r.Offset(0, -25);
						r.Left = rect.Left;
						r.Width = 60;
						this.fieldStrikeoutThickness.Bounds = r;
						this.fieldStrikeoutThickness.Visibility = true;
						r.Offset(60, 0);
						this.fieldStrikeoutPosition.Bounds = r;
						this.fieldStrikeoutPosition.Visibility = true;
						r.Offset(60+12, 0);
						r.Width = 48;
						this.strikeoutColor.Bounds = r;
						this.strikeoutColor.Visibility = true;
					}
					else
					{
						this.fieldStrikeoutThickness.Visibility = false;
						this.fieldStrikeoutPosition.Visibility = false;
						this.strikeoutColor.Visibility = false;
					}
				}
			}
			else
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				r.Left = rect.Left;
				r.Width = 20;
				this.buttonUnderlined.Bounds = r;
				r.Offset(20, 0);
				this.buttonOverlined.Bounds = r;
				r.Offset(20+5, 0);
				this.buttonStrikeout.Bounds = r;
				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.Bounds = r;

				this.fieldUnderlinedThickness.Visibility = false;
				this.fieldUnderlinedPosition.Visibility = false;
				this.underlinedColor.Visibility = false;
				this.fieldOverlinedThickness.Visibility = false;
				this.fieldOverlinedPosition.Visibility = false;
				this.overlinedColor.Visibility = false;
				this.fieldStrikeoutThickness.Visibility = false;
				this.fieldStrikeoutPosition.Visibility = false;
				this.strikeoutColor.Visibility = false;
			}
		}


		// Met à jour après un changement du wrapper.
		protected override void UpdateAfterChanging()
		{
			base.UpdateAfterChanging();

			bool underlined   = this.document.TextWrapper.Active.IsUnderlineDefined;
			bool isUnderlined = this.document.TextWrapper.Defined.IsUnderlineDefined;

			bool overlined    = this.document.TextWrapper.Active.IsOverlineDefined;
			bool isOverlined  = this.document.TextWrapper.Defined.IsOverlineDefined;

			bool strikeout    = this.document.TextWrapper.Active.IsStrikeoutDefined;
			bool isStrikeout  = this.document.TextWrapper.Defined.IsStrikeoutDefined;

			double underlinedThickness = 0.0;
			double underlinedPosition  = 0.0;
			string underlinedColor     = null;

			double overlinedThickness = 0.0;
			double overlinedPosition  = 0.0;
			string overlinedColor     = null;

			double strikeoutThickness = 0.0;
			double strikeoutPosition  = 0.0;
			string strikeoutColor     = null;

			if ( isUnderlined )
			{
				underlinedThickness = this.document.TextWrapper.Defined.Underline.Thickness;
				underlinedPosition  = this.document.TextWrapper.Defined.Underline.Position;
				underlinedColor     = this.document.TextWrapper.Defined.Underline.DrawStyle;
			}

			if ( isOverlined )
			{
				overlinedThickness = this.document.TextWrapper.Defined.Overline.Thickness;
				overlinedPosition  = this.document.TextWrapper.Defined.Overline.Position;
				overlinedColor     = this.document.TextWrapper.Defined.Overline.DrawStyle;
			}

			if ( isStrikeout )
			{
				strikeoutThickness = this.document.TextWrapper.Defined.Strikeout.Thickness;
				strikeoutPosition  = this.document.TextWrapper.Defined.Strikeout.Position;
				strikeoutColor     = this.document.TextWrapper.Defined.Strikeout.DrawStyle;
			}

			this.ignoreChanged = true;
			
			this.ActiveIconButton(this.buttonUnderlined, underlined, isUnderlined);
			this.ActiveIconButton(this.buttonOverlined,  overlined,  isOverlined );
			this.ActiveIconButton(this.buttonStrikeout,  strikeout,  isStrikeout );

			this.SetTextFieldRealValue(this.fieldUnderlinedThickness.TextFieldReal, underlinedThickness, Common.Text.Properties.SizeUnits.Points, isUnderlined, true);
			this.SetTextFieldRealValue(this.fieldUnderlinedPosition.TextFieldReal,  underlinedPosition,  Common.Text.Properties.SizeUnits.Points, isUnderlined, true);
			this.SetTextFieldRealValue(this.fieldOverlinedThickness.TextFieldReal,  overlinedThickness,  Common.Text.Properties.SizeUnits.Points, isOverlined,  true);
			this.SetTextFieldRealValue(this.fieldOverlinedPosition.TextFieldReal,   overlinedPosition,   Common.Text.Properties.SizeUnits.Points, isOverlined,  true);
			this.SetTextFieldRealValue(this.fieldStrikeoutThickness.TextFieldReal,  strikeoutThickness,  Common.Text.Properties.SizeUnits.Points, isStrikeout,  true);
			this.SetTextFieldRealValue(this.fieldStrikeoutPosition.TextFieldReal,   strikeoutPosition,   Common.Text.Properties.SizeUnits.Points, isStrikeout,  true);

			this.SetColorSample(this.underlinedColor, underlinedColor, isUnderlined, true);
			this.SetColorSample(this.overlinedColor,  overlinedColor,  isOverlined,  true);
			this.SetColorSample(this.strikeoutColor,  strikeoutColor,  isStrikeout,  true);

			if ( this.underlinedColor.ActiveState == ActiveState.Yes ||
				 this.overlinedColor.ActiveState  == ActiveState.Yes ||
				 this.strikeoutColor.ActiveState  == ActiveState.Yes )
			{
				this.OnOriginColorChanged();  // change la couleur dans le ColorSelector
			}

			this.ignoreChanged = false;

			double h = this.DefaultHeight;
			if ( this.Height != h )
			{
				this.Height = h;  // adapte la hauteur du panneau
			}
		}


		private void HandleButtonUnderlineClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;
			
			this.document.TextWrapper.SuspendSynchronisations();
			
			// Cycle entre divers états:
			//
			// (A1) Soulignement hérité du style actif
			// (A2) Pas de soulignement (forcé par un disable local)
			// (A3) Soulignement défini localement
			//
			// ou si aucun soulignement n'est défini dans le style actif:
			//
			// (B1) Pas de soulignement
			// (B2) Soulignement défini localement
			
			Common.Text.Wrappers.TextWrapper.XlineDefinition underline = this.document.TextWrapper.Defined.Underline;
			
			if ( this.document.TextWrapper.Active.IsUnderlineDefined )
			{
				if ( this.document.TextWrapper.Active.Underline.IsDisabled &&
					 this.document.TextWrapper.Active.Underline.IsEmpty == false )
				{
					// (A2)
					this.FillUnderlineDefinition(underline, false);  // --> (A3)
					
					if ( underline.EqualsIgnoringIsDisabled(this.document.TextWrapper.Active.Underline) )
					{
						// L'état défini par notre souligné local est identique à celui hérité
						// par le style actif; utilise celui du style dans ce cas.
						this.document.TextWrapper.Defined.ClearUnderline();  // --> (A1)
					}
				}
				else if ( this.document.TextWrapper.Defined.IsUnderlineDefined )
				{
					// (A3) ou (B2)
					this.document.TextWrapper.Defined.ClearUnderline();  // --> (A1) ou (B1)
				}
				else
				{
					// (A1)
					underline.IsDisabled = true;  // --> (A2)
				}
			}
			else
			{
				// (B1)
				this.FillUnderlineDefinition(underline, true);  // --> (B2)
			}
			
			this.document.TextWrapper.ResumeSynchronisations();
		}
		
		private void HandleButtonOverlineClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;
			
			this.document.TextWrapper.SuspendSynchronisations();
			
			// Cycle entre divers états:
			//
			// (A1) Soulignement hérité du style actif
			// (A2) Pas de soulignement (forcé par un disable local)
			// (A3) Soulignement défini localement
			//
			// ou si aucun soulignement n'est défini dans le style actif:
			//
			// (B1) Pas de soulignement
			// (B2) Soulignement défini localement
			
			Common.Text.Wrappers.TextWrapper.XlineDefinition overline = this.document.TextWrapper.Defined.Overline;
			
			if ( this.document.TextWrapper.Active.IsOverlineDefined )
			{
				if ( this.document.TextWrapper.Active.Overline.IsDisabled &&
					 this.document.TextWrapper.Active.Overline.IsEmpty == false )
				{
					// (A2)
					this.FillOverlineDefinition(overline, false);  // --> (A3)
					
					if ( overline.EqualsIgnoringIsDisabled(this.document.TextWrapper.Active.Overline) )
					{
						// L'état défini par notre souligné local est identique à celui hérité
						// par le style actif; utilise celui du style dans ce cas.
						this.document.TextWrapper.Defined.ClearOverline();  // --> (A1)
					}
				}
				else if ( this.document.TextWrapper.Defined.IsOverlineDefined )
				{
					// (A3) ou (B2)
					this.document.TextWrapper.Defined.ClearOverline();  // --> (A1) ou (B1)
				}
				else
				{
					// (A1)
					overline.IsDisabled = true;  // --> (A2)
				}
			}
			else
			{
				// (B1)
				this.FillOverlineDefinition(overline, true);  // --> (B2)
			}
			
			this.document.TextWrapper.ResumeSynchronisations();
		}
		
		private void HandleButtonStrikeoutClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;
			
			this.document.TextWrapper.SuspendSynchronisations();
			
			// Cycle entre divers états:
			//
			// (A1) Soulignement hérité du style actif
			// (A2) Pas de soulignement (forcé par un disable local)
			// (A3) Soulignement défini localement
			//
			// ou si aucun soulignement n'est défini dans le style actif:
			//
			// (B1) Pas de soulignement
			// (B2) Soulignement défini localement
			
			Common.Text.Wrappers.TextWrapper.XlineDefinition strikeout = this.document.TextWrapper.Defined.Strikeout;
			
			if ( this.document.TextWrapper.Active.IsStrikeoutDefined )
			{
				if ( this.document.TextWrapper.Active.Strikeout.IsDisabled &&
					 this.document.TextWrapper.Active.Strikeout.IsEmpty == false )
				{
					// (A2)
					this.FillStrikeoutDefinition(strikeout, false);  // --> (A3)
					
					if ( strikeout.EqualsIgnoringIsDisabled(this.document.TextWrapper.Active.Strikeout) )
					{
						// L'état défini par notre souligné local est identique à celui hérité
						// par le style actif; utilise celui du style dans ce cas.
						this.document.TextWrapper.Defined.ClearStrikeout();  // --> (A1)
					}
				}
				else if ( this.document.TextWrapper.Defined.IsStrikeoutDefined )
				{
					// (A3) ou (B2)
					this.document.TextWrapper.Defined.ClearStrikeout();  // --> (A1) ou (B1)
				}
				else
				{
					// (A1)
					strikeout.IsDisabled = true;  // --> (A2)
				}
			}
			else
			{
				// (B1)
				this.FillStrikeoutDefinition(strikeout, true);  // --> (B2)
			}
			
			this.document.TextWrapper.ResumeSynchronisations();
		}
		
		private void HandleUnderlinedValueChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;

			this.document.TextWrapper.SuspendSynchronisations();
			Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.document.TextWrapper.Defined.Underline;
			this.FillUnderlineDefinition(xline, false);
			this.document.TextWrapper.ResumeSynchronisations();
		}

		private void HandleOverlinedValueChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;

			this.document.TextWrapper.SuspendSynchronisations();
			Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.document.TextWrapper.Defined.Overline;
			this.FillOverlineDefinition(xline, false);
			this.document.TextWrapper.ResumeSynchronisations();
		}

		private void HandleStrikeoutValueChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;

			this.document.TextWrapper.SuspendSynchronisations();
			Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.document.TextWrapper.Defined.Strikeout;
			this.FillStrikeoutDefinition(xline, false);
			this.document.TextWrapper.ResumeSynchronisations();
		}

		private void HandleSampleColorClicked(object sender, MessageEventArgs e)
		{
			this.originFieldColor = sender as ColorSample;

			this.originFieldRank = -1;
			if ( this.originFieldColor == this.underlinedColor )  this.originFieldRank = 0;
			if ( this.originFieldColor == this.overlinedColor  )  this.originFieldRank = 1;
			if ( this.originFieldColor == this.strikeoutColor  )  this.originFieldRank = 2;

			this.OnOriginColorChanged();
		}

		private void HandleSampleColorChanged(object sender)
		{
			if ( this.ignoreChanged )  return;

			ColorSample cs = sender as ColorSample;
			if ( cs.ActiveState == ActiveState.Yes )
			{
				this.OnOriginColorChanged();
			}

			this.ColorToWrapper(cs);
		}

		private void HandleClearClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.document.TextWrapper.IsAttached )  return;

			this.document.TextWrapper.SuspendSynchronisations();
			this.document.TextWrapper.Defined.ClearUnderline();
			this.document.TextWrapper.Defined.ClearOverline();
			this.document.TextWrapper.Defined.ClearStrikeout();
			this.document.TextWrapper.ResumeSynchronisations();
		}

		
		private void FillUnderlineDefinition(Common.Text.Wrappers.TextWrapper.XlineDefinition xline, bool def)
		{
			xline.IsDisabled = false;

			double thickness;
			double position;
			if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
			{
				thickness = 1.0;  // 0.1mm
				position  = 5.0;  // 0.5mm
			}
			else
			{
				thickness = 1.27;  // 0.005in
				position  = 5.08;  // 0.02in
			}
			string color     = RichColor.ToString(RichColor.FromBrightness(0));

			if ( !def )
			{
				thickness = (double) this.fieldUnderlinedThickness.TextFieldReal.InternalValue;
				position  = (double) this.fieldUnderlinedPosition.TextFieldReal.InternalValue;
				color     = this.GetColorSample(this.underlinedColor);
			}
			
			xline.Thickness      = thickness;
			xline.ThicknessUnits = Common.Text.Properties.SizeUnits.Points;
			xline.Position       = position;
			xline.PositionUnits  = Common.Text.Properties.SizeUnits.Points;
			xline.DrawClass      = "";
			xline.DrawStyle      = color;
		}
        
		private void FillOverlineDefinition(Common.Text.Wrappers.TextWrapper.XlineDefinition xline, bool def)
		{
			xline.IsDisabled = false;

			double thickness;
			double position;
			if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
			{
				thickness = 1.0;  // 0.1mm
				position  = 2.0;  // 0.2mm
			}
			else
			{
				thickness = 1.27;  // 0.005in
				position  = 2.54;  // 0.01in
			}
			string color     = RichColor.ToString(RichColor.FromBrightness(0));

			if ( !def )
			{
				thickness = (double) this.fieldOverlinedThickness.TextFieldReal.InternalValue;
				position  = (double) this.fieldOverlinedPosition.TextFieldReal.InternalValue;
				color     = this.GetColorSample(this.overlinedColor);
			}
			
			xline.Thickness      = thickness;
			xline.ThicknessUnits = Common.Text.Properties.SizeUnits.Points;
			xline.Position       = position;
			xline.PositionUnits  = Common.Text.Properties.SizeUnits.Points;
			xline.DrawClass      = "";
			xline.DrawStyle      = color;
		}
        
		private void FillStrikeoutDefinition(Common.Text.Wrappers.TextWrapper.XlineDefinition xline, bool def)
		{
			xline.IsDisabled = false;

			double thickness;
			double position;
			if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
			{
				thickness =  2.0;  // 0.2mm
				position  = 12.0;  // 1.2mm
			}
			else
			{
				thickness =  2.54;  // 0.01in
				position  = 12.70;  // 0.5in
			}
			string color     = RichColor.ToString(RichColor.FromBrightness(0));

			if ( !def )
			{
				thickness = (double) this.fieldStrikeoutThickness.TextFieldReal.InternalValue;
				position  = (double) this.fieldStrikeoutPosition.TextFieldReal.InternalValue;
				color     = this.GetColorSample(this.strikeoutColor);
			}
			
			xline.Thickness      = thickness;
			xline.ThicknessUnits = Common.Text.Properties.SizeUnits.Points;
			xline.Position       = position;
			xline.PositionUnits  = Common.Text.Properties.SizeUnits.Points;
			xline.DrawClass      = "";
			xline.DrawStyle      = color;
		}
        

		protected IconButton				buttonUnderlined;
		protected IconButton				buttonOverlined;
		protected IconButton				buttonStrikeout;
		protected Widgets.TextFieldLabel	fieldUnderlinedThickness;
		protected Widgets.TextFieldLabel	fieldUnderlinedPosition;
		protected ColorSample				underlinedColor;
		protected Widgets.TextFieldLabel	fieldOverlinedThickness;
		protected Widgets.TextFieldLabel	fieldOverlinedPosition;
		protected ColorSample				overlinedColor;
		protected Widgets.TextFieldLabel	fieldStrikeoutThickness;
		protected Widgets.TextFieldLabel	fieldStrikeoutPosition;
		protected ColorSample				strikeoutColor;
		protected IconButton				buttonClear;

		protected ColorSample				originFieldColor;
		protected int						originFieldRank = -1;
	}
}
