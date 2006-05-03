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
		public Xline(Document document, bool isStyle, StyleCategory styleCategory) : base(document, isStyle, styleCategory)
		{
			this.label.Text = Res.Strings.TextPanel.Xline.Title;

			this.fixIcon.Text = Misc.Image("TextXline");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Xline.Title);

			this.buttonUnderlined = this.CreateIconButton(Misc.Icon("FontUnderlined"), Res.Strings.Action.FontUnderlined, new MessageEventHandler(this.HandleButtonUnderlineClicked));
			this.buttonOverlined  = this.CreateIconButton(Misc.Icon("FontOverlined"),  Res.Strings.Action.FontOverlined,  new MessageEventHandler(this.HandleButtonOverlineClicked));
			this.buttonStrikeout  = this.CreateIconButton(Misc.Icon("FontStrikeout"),  Res.Strings.Action.FontStrikeout,  new MessageEventHandler(this.HandleButtonStrikeoutClicked));

			this.fieldUnderlinedThickness = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Underlined.Thickness, Res.Strings.TextPanel.Xline.Short.Underlined.Thickness, Res.Strings.TextPanel.Xline.Long.Underlined.Thickness, 0.0,  0.01, 0.0, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleUnderlinedValueChanged));
			this.fieldUnderlinedPosition  = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Underlined.Position,  Res.Strings.TextPanel.Xline.Short.Underlined.Position,  Res.Strings.TextPanel.Xline.Long.Underlined.Position,  0.0,  0.01, 0.0, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleUnderlinedValueChanged));
			this.underlinedColor          = this.CreateColorSample(Res.Strings.TextPanel.Xline.Tooltip.Underlined.Color, new MessageEventHandler(this.HandleSampleColorClicked), new EventHandler(this.HandleSampleColorChanged));

			this.fieldOverlinedThickness  = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Overlined.Thickness,  Res.Strings.TextPanel.Xline.Short.Overlined.Thickness,  Res.Strings.TextPanel.Xline.Long.Overlined.Thickness,  0.0,  0.01, 0.0, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleOverlinedValueChanged));
			this.fieldOverlinedPosition   = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Overlined.Position,   Res.Strings.TextPanel.Xline.Short.Overlined.Position,   Res.Strings.TextPanel.Xline.Long.Overlined.Position,  -0.01, 0.01, 0.0, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleOverlinedValueChanged));
			this.overlinedColor           = this.CreateColorSample(Res.Strings.TextPanel.Xline.Tooltip.Overlined.Color, new MessageEventHandler(this.HandleSampleColorClicked), new EventHandler(this.HandleSampleColorChanged));

			this.fieldStrikeoutThickness  = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Strikeout.Thickness,  Res.Strings.TextPanel.Xline.Short.Strikeout.Thickness,  Res.Strings.TextPanel.Xline.Long.Strikeout.Thickness,  0.0,  0.01, 0.0, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleStrikeoutValueChanged));
			this.fieldStrikeoutPosition   = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Strikeout.Position,   Res.Strings.TextPanel.Xline.Short.Strikeout.Position,   Res.Strings.TextPanel.Xline.Long.Strikeout.Position,   0.0,  0.01, 0.0, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, new EventHandler(this.HandleStrikeoutValueChanged));
			this.strikeoutColor           = this.CreateColorSample(Res.Strings.TextPanel.Xline.Tooltip.Strikeout.Color, new MessageEventHandler(this.HandleSampleColorClicked), new EventHandler(this.HandleSampleColorChanged));
			
			this.buttonClear = this.CreateClearButton(new MessageEventHandler(this.HandleClearClicked));

			this.TextWrapper.Active.Changed  += new EventHandler(this.HandleWrapperChanged);
			this.TextWrapper.Defined.Changed += new EventHandler(this.HandleWrapperChanged);

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.TextWrapper.Active.Changed  -= new EventHandler(this.HandleWrapperChanged);
				this.TextWrapper.Defined.Changed -= new EventHandler(this.HandleWrapperChanged);
			}
			
			base.Dispose(disposing);
		}

		
		public override void UpdateAfterAttach()
		{
			//	Mise à jour après avoir attaché le wrappers.
			this.buttonClear.Visibility = !this.TextWrapper.IsAttachedToDefaultParagraphStyle;
		}


		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				double h = this.LabelHeight;

				if ( this.isExtendedSize )  // panneau étendu ?
				{
					int total = 0;
					if ( this.TextWrapper.Defined.IsUnderlineDefined )  total ++;
					if ( this.TextWrapper.Defined.IsOverlineDefined  )  total ++;
					if ( this.TextWrapper.Defined.IsStrikeoutDefined )  total ++;

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


		public override void OriginColorDeselect()
		{
			//	Désélectionne toutes les origines de couleurs possibles.
			this.underlinedColor.ActiveState = ActiveState.No;
			this.overlinedColor.ActiveState  = ActiveState.No;
			this.strikeoutColor.ActiveState  = ActiveState.No;
		}

		public override void OriginColorSelect(int rank)
		{
			//	Sélectionne l'origine de couleur.
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
				this.ColorToWrapper(this.originFieldColor);
			}
		}

		public override Drawing.RichColor OriginColorGet()
		{
			//	Donne la couleur d'origine.
			if ( this.originFieldColor == null )  return Drawing.RichColor.FromBrightness(0.0);
			return this.originFieldColor.Color;
		}

		protected void ColorToWrapper(ColorSample sample)
		{
			//	Donne la couleur au wrapper.
			if ( sample == this.underlinedColor )
			{
				this.TextWrapper.SuspendSynchronizations();
				Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.TextWrapper.Defined.Underline;
				this.FillUnderlineDefinition(xline, false);
				this.TextWrapper.DefineOperationName("FontXline", Res.Strings.TextPanel.Xline.Title);
				this.TextWrapper.ResumeSynchronizations();
				this.ActionMade();
			}

			if ( sample == this.overlinedColor )
			{
				this.TextWrapper.SuspendSynchronizations();
				Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.TextWrapper.Defined.Overline;
				this.FillOverlineDefinition(xline, false);
				this.TextWrapper.DefineOperationName("FontXline", Res.Strings.TextPanel.Xline.Title);
				this.TextWrapper.ResumeSynchronizations();
				this.ActionMade();
			}

			if ( sample == this.strikeoutColor )
			{
				this.TextWrapper.SuspendSynchronizations();
				Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.TextWrapper.Defined.Strikeout;
				this.FillStrikeoutDefinition(xline, false);
				this.TextWrapper.DefineOperationName("FontXline", Res.Strings.TextPanel.Xline.Title);
				this.TextWrapper.ResumeSynchronizations();
				this.ActionMade();
			}
		}

		
		protected void HandleWrapperChanged(object sender)
		{
			//	Le wrapper associé a changé.
			this.UpdateAfterChanging();
		}

		
		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonUnderlined == null )  return;

			Rectangle rect = this.UsefulZone;

			bool isUnderlined = this.TextWrapper.Defined.IsUnderlineDefined;
			bool isOverlined  = this.TextWrapper.Defined.IsOverlineDefined;
			bool isStrikeout  = this.TextWrapper.Defined.IsStrikeoutDefined;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonUnderlined.SetManualBounds(r);
					r.Offset(20, 0);
					this.buttonOverlined.SetManualBounds(r);
					r.Offset(20+5, 0);
					this.buttonStrikeout.SetManualBounds(r);

					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.SetManualBounds(r);

					if ( isUnderlined )
					{
						r.Offset(0, -25);
						r.Left = rect.Left;
						r.Right = rect.Right-25;
						this.fieldUnderlinedThickness.SetManualBounds(r);
						this.fieldUnderlinedThickness.Visibility = true;
						r.Offset(0, -25);
						this.fieldUnderlinedPosition.SetManualBounds(r);
						this.fieldUnderlinedPosition.Visibility = true;
						r.Offset(0, -25);
						r.Left = rect.Right-25-48;
						r.Width = 48;
						this.underlinedColor.SetManualBounds(r);
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
						this.fieldOverlinedThickness.SetManualBounds(r);
						this.fieldOverlinedThickness.Visibility = true;
						r.Offset(0, -25);
						this.fieldOverlinedPosition.SetManualBounds(r);
						this.fieldOverlinedPosition.Visibility = true;
						r.Offset(0, -25);
						r.Left = rect.Right-25-48;
						r.Width = 48;
						this.overlinedColor.SetManualBounds(r);
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
						this.fieldStrikeoutThickness.SetManualBounds(r);
						this.fieldStrikeoutThickness.Visibility = true;
						r.Offset(0, -25);
						this.fieldStrikeoutPosition.SetManualBounds(r);
						this.fieldStrikeoutPosition.Visibility = true;
						r.Offset(0, -25);
						r.Left = rect.Right-25-48;
						r.Width = 48;
						this.strikeoutColor.SetManualBounds(r);
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
					this.buttonUnderlined.SetManualBounds(r);
					r.Offset(20, 0);
					this.buttonOverlined.SetManualBounds(r);
					r.Offset(20+5, 0);
					this.buttonStrikeout.SetManualBounds(r);
					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.SetManualBounds(r);

					if ( isUnderlined )
					{
						r.Offset(0, -25);
						r.Left = rect.Left;
						r.Width = 60;
						this.fieldUnderlinedThickness.SetManualBounds(r);
						this.fieldUnderlinedThickness.Visibility = true;
						r.Offset(60, 0);
						this.fieldUnderlinedPosition.SetManualBounds(r);
						this.fieldUnderlinedPosition.Visibility = true;
						r.Offset(60+12, 0);
						r.Width = 48;
						this.underlinedColor.SetManualBounds(r);
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
						this.fieldOverlinedThickness.SetManualBounds(r);
						this.fieldOverlinedThickness.Visibility = true;
						r.Offset(60, 0);
						this.fieldOverlinedPosition.SetManualBounds(r);
						this.fieldOverlinedPosition.Visibility = true;
						r.Offset(60+12, 0);
						r.Width = 48;
						this.overlinedColor.SetManualBounds(r);
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
						this.fieldStrikeoutThickness.SetManualBounds(r);
						this.fieldStrikeoutThickness.Visibility = true;
						r.Offset(60, 0);
						this.fieldStrikeoutPosition.SetManualBounds(r);
						this.fieldStrikeoutPosition.Visibility = true;
						r.Offset(60+12, 0);
						r.Width = 48;
						this.strikeoutColor.SetManualBounds(r);
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
				this.buttonUnderlined.SetManualBounds(r);
				r.Offset(20, 0);
				this.buttonOverlined.SetManualBounds(r);
				r.Offset(20+5, 0);
				this.buttonStrikeout.SetManualBounds(r);
				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.SetManualBounds(r);

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


		protected override void UpdateAfterChanging()
		{
			//	Met à jour après un changement du wrapper.
			base.UpdateAfterChanging();
			
			if ( this.TextWrapper.IsAttached == false )  return;
			
			bool underlined   = this.TextWrapper.Active.IsUnderlineDefined;
			bool isUnderlined = this.TextWrapper.Defined.IsUnderlineDefined;

			bool overlined    = this.TextWrapper.Active.IsOverlineDefined;
			bool isOverlined  = this.TextWrapper.Defined.IsOverlineDefined;

			bool strikeout    = this.TextWrapper.Active.IsStrikeoutDefined;
			bool isStrikeout  = this.TextWrapper.Defined.IsStrikeoutDefined;

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
				underlinedThickness = this.TextWrapper.Defined.Underline.Thickness;
				underlinedPosition  = this.TextWrapper.Defined.Underline.Position;
				underlinedColor     = this.TextWrapper.Defined.Underline.DrawStyle;
			}

			if ( isOverlined )
			{
				overlinedThickness = this.TextWrapper.Defined.Overline.Thickness;
				overlinedPosition  = this.TextWrapper.Defined.Overline.Position;
				overlinedColor     = this.TextWrapper.Defined.Overline.DrawStyle;
			}

			if ( isStrikeout )
			{
				strikeoutThickness = this.TextWrapper.Defined.Strikeout.Thickness;
				strikeoutPosition  = this.TextWrapper.Defined.Strikeout.Position;
				strikeoutColor     = this.TextWrapper.Defined.Strikeout.DrawStyle;
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

			string newSignature = this.Signature;
			if ( this.signature != newSignature )
			{
				this.signature = newSignature;
				this.PreferredHeight = this.DefaultHeight;  // adapte la hauteur du panneau
				this.UpdateClientGeometry();
			}
		}


		private void HandleButtonUnderlineClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;
			
			this.TextWrapper.SuspendSynchronizations();
			
			//	Cycle entre divers états:
			//
			//	(A1) Soulignement hérité du style actif
			//	(A2) Pas de soulignement (forcé par un disable local)
			//	(A3) Soulignement défini localement
			//
			//	ou si aucun soulignement n'est défini dans le style actif:
			//
			//	(B1) Pas de soulignement
			//	(B2) Soulignement défini localement
			
			Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.TextWrapper.Defined.Underline;
			
			if ( this.TextWrapper.Active.IsUnderlineDefined )
			{
				if ( this.TextWrapper.Active.Underline.IsDisabled &&
					 this.TextWrapper.Active.Underline.IsEmpty == false )
				{
					//	(A2)
					this.FillUnderlineDefinition(xline, false);  // --> (A3)
					
					if ( xline.EqualsIgnoringIsDisabled(this.TextWrapper.Active.Underline) )
					{
						//	L'état défini par notre souligné local est identique à celui hérité
						//	par le style actif; utilise celui du style dans ce cas.
						this.TextWrapper.Defined.ClearUnderline();  // --> (A1)
					}
				}
				else if ( this.TextWrapper.Defined.IsUnderlineDefined )
				{
					//	(A3) ou (B2)
					this.TextWrapper.Defined.ClearUnderline();  // --> (A1) ou (B1)
				}
				else
				{
					//	(A1)
					xline.IsDisabled = true;  // --> (A2)
				}
			}
			else
			{
				//	(B1)
				this.FillUnderlineDefinition(xline, true);  // --> (B2)
			}
			
			this.TextWrapper.DefineOperationName("FontUnderlined", Res.Strings.Action.FontUnderlined);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
			this.ForceHeightChanged();
		}
		
		private void HandleButtonOverlineClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;
			
			this.TextWrapper.SuspendSynchronizations();
			
			//	Cycle entre divers états:
			//
			//	(A1) Soulignement hérité du style actif
			//	(A2) Pas de soulignement (forcé par un disable local)
			//	(A3) Soulignement défini localement
			//
			//	ou si aucun soulignement n'est défini dans le style actif:
			//
			//	(B1) Pas de soulignement
			//	(B2) Soulignement défini localement
			
			Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.TextWrapper.Defined.Overline;
			
			if ( this.TextWrapper.Active.IsOverlineDefined )
			{
				if ( this.TextWrapper.Active.Overline.IsDisabled &&
					 this.TextWrapper.Active.Overline.IsEmpty == false )
				{
					//	(A2)
					this.FillOverlineDefinition(xline, false);  // --> (A3)
					
					if ( xline.EqualsIgnoringIsDisabled(this.TextWrapper.Active.Overline) )
					{
						//	L'état défini par notre souligné local est identique à celui hérité
						//	par le style actif; utilise celui du style dans ce cas.
						this.TextWrapper.Defined.ClearOverline();  // --> (A1)
					}
				}
				else if ( this.TextWrapper.Defined.IsOverlineDefined )
				{
					//	(A3) ou (B2)
					this.TextWrapper.Defined.ClearOverline();  // --> (A1) ou (B1)
				}
				else
				{
					//	(A1)
					xline.IsDisabled = true;  // --> (A2)
				}
			}
			else
			{
				//	(B1)
				this.FillOverlineDefinition(xline, true);  // --> (B2)
			}
			
			this.TextWrapper.DefineOperationName("FontOverlined", Res.Strings.Action.FontOverlined);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
			this.ForceHeightChanged();
		}
		
		private void HandleButtonStrikeoutClicked(object sender, MessageEventArgs e)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;
			
			this.TextWrapper.SuspendSynchronizations();
			
			//	Cycle entre divers états:
			//
			//	(A1) Soulignement hérité du style actif
			//	(A2) Pas de soulignement (forcé par un disable local)
			//	(A3) Soulignement défini localement
			//
			//	ou si aucun soulignement n'est défini dans le style actif:
			//
			//	(B1) Pas de soulignement
			//	(B2) Soulignement défini localement
			
			Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.TextWrapper.Defined.Strikeout;
			
			if ( this.TextWrapper.Active.IsStrikeoutDefined )
			{
				if ( this.TextWrapper.Active.Strikeout.IsDisabled &&
					 this.TextWrapper.Active.Strikeout.IsEmpty == false )
				{
					//	(A2)
					this.FillStrikeoutDefinition(xline, false);  // --> (A3)
					
					if ( xline.EqualsIgnoringIsDisabled(this.TextWrapper.Active.Strikeout) )
					{
						//	L'état défini par notre souligné local est identique à celui hérité
						//	par le style actif; utilise celui du style dans ce cas.
						this.TextWrapper.Defined.ClearStrikeout();  // --> (A1)
					}
				}
				else if ( this.TextWrapper.Defined.IsStrikeoutDefined )
				{
					//	(A3) ou (B2)
					this.TextWrapper.Defined.ClearStrikeout();  // --> (A1) ou (B1)
				}
				else
				{
					//	(A1)
					xline.IsDisabled = true;  // --> (A2)
				}
			}
			else
			{
				//	(B1)
				this.FillStrikeoutDefinition(xline, true);  // --> (B2)
			}
			
			this.TextWrapper.DefineOperationName("FontStrikeout", Res.Strings.Action.FontStrikeout);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
			this.ForceHeightChanged();
		}
		
		private void HandleUnderlinedValueChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;

			this.TextWrapper.SuspendSynchronizations();
			Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.TextWrapper.Defined.Underline;
			this.FillUnderlineDefinition(xline, false);
			this.TextWrapper.DefineOperationName("FontUnderlined", Res.Strings.Action.FontUnderlined);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleOverlinedValueChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;

			this.TextWrapper.SuspendSynchronizations();
			Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.TextWrapper.Defined.Overline;
			this.FillOverlineDefinition(xline, false);
			this.TextWrapper.DefineOperationName("FontOverlined", Res.Strings.Action.FontOverlined);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleStrikeoutValueChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;

			this.TextWrapper.SuspendSynchronizations();
			Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.TextWrapper.Defined.Strikeout;
			this.FillStrikeoutDefinition(xline, false);
			this.TextWrapper.DefineOperationName("FontStrikeout", Res.Strings.Action.FontStrikeout);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
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
			if ( !this.TextWrapper.IsAttached )  return;

			this.TextWrapper.SuspendSynchronizations();
			this.TextWrapper.Defined.ClearUnderline();
			this.TextWrapper.Defined.ClearOverline();
			this.TextWrapper.Defined.ClearStrikeout();
			this.TextWrapper.DefineOperationName("FontXlineClear", Res.Strings.TextPanel.Clear);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		
		private void FillUnderlineDefinition(Common.Text.Wrappers.TextWrapper.XlineDefinition xline, bool def)
		{
			this.document.Wrappers.FillUnderlineDefinition(xline);

			if ( !def )
			{
				xline.Thickness = (double) this.fieldUnderlinedThickness.TextFieldReal.InternalValue;
				xline.Position  = (double) this.fieldUnderlinedPosition.TextFieldReal.InternalValue;
				xline.DrawStyle = this.GetColorSample(this.underlinedColor);
			}
		}
        
		private void FillOverlineDefinition(Common.Text.Wrappers.TextWrapper.XlineDefinition xline, bool def)
		{
			this.document.Wrappers.FillOverlineDefinition(xline);

			if ( !def )
			{
				xline.Thickness = (double) this.fieldOverlinedThickness.TextFieldReal.InternalValue;
				xline.Position  = (double) this.fieldOverlinedPosition.TextFieldReal.InternalValue;
				xline.DrawStyle = this.GetColorSample(this.overlinedColor);
			}
		}
        
		private void FillStrikeoutDefinition(Common.Text.Wrappers.TextWrapper.XlineDefinition xline, bool def)
		{
			this.document.Wrappers.FillStrikeoutDefinition(xline);

			if ( !def )
			{
				xline.Thickness = (double) this.fieldStrikeoutThickness.TextFieldReal.InternalValue;
				xline.Position  = (double) this.fieldStrikeoutPosition.TextFieldReal.InternalValue;
				xline.DrawStyle = this.GetColorSample(this.strikeoutColor);
			}
		}


		protected string Signature
		{
			//	Donne une signature unique en fonction du type de contenu.
			get
			{
				string signature = "";

				if ( this.document != null && this.TextWrapper.IsAttached )
				{
					if ( this.TextWrapper.Defined.IsUnderlineDefined )  signature += "u";
					if ( this.TextWrapper.Defined.IsOverlineDefined  )  signature += "o";
					if ( this.TextWrapper.Defined.IsStrikeoutDefined )  signature += "s";
				}

				return signature;
			}
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
		protected string					signature = "";

		protected ColorSample				originFieldColor;
		protected int						originFieldRank = -1;
	}
}
