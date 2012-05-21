using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.TextPanels
{
	/// <summary>
	/// La classe Xline permet de choisir les soulignements.
	/// </summary>
	public class Xline : Abstract
	{
		public Xline(Document document, bool isStyle, StyleCategory styleCategory) : base(document, isStyle, styleCategory)
		{
			this.label.Text = Res.Strings.TextPanel.Xline.Title;

			this.fixIcon.Text = Misc.Image("TextXline");
			ToolTip.Default.SetToolTip(this.fixIcon, Res.Strings.TextPanel.Xline.Title);

			this.buttonUnderline = this.CreateIconButton(Misc.Icon("FontUnderline"), Res.Strings.Action.FontUnderline, this.HandleButtonUnderlineClicked);
			this.buttonOverline  = this.CreateIconButton(Misc.Icon("FontOverline"),  Res.Strings.Action.FontOverline,  this.HandleButtonOverlineClicked);
			this.buttonStrikeout = this.CreateIconButton(Misc.Icon("FontStrikeout"), Res.Strings.Action.FontStrikeout, this.HandleButtonStrikeoutClicked);

			this.fieldUnderlineThickness = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Underline.Thickness, Res.Strings.TextPanel.Xline.Short.Underline.Thickness, Res.Strings.TextPanel.Xline.Long.Underline.Thickness, 0.0,  0.01, 0.0, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, this.HandleUnderlineValueChanged);
			this.fieldUnderlinePosition  = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Underline.Position,  Res.Strings.TextPanel.Xline.Short.Underline.Position,  Res.Strings.TextPanel.Xline.Long.Underline.Position,  0.0,  0.01, 0.0, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, this.HandleUnderlineValueChanged);
			this.underlineColor          = this.CreateColorSample(Res.Strings.TextPanel.Xline.Tooltip.Underline.Color, this.HandleSampleColorClicked, this.HandleSampleColorChanged);

			this.fieldOverlineThickness  = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Overline.Thickness,  Res.Strings.TextPanel.Xline.Short.Overline.Thickness,  Res.Strings.TextPanel.Xline.Long.Overline.Thickness,  0.0,  0.01, 0.0, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, this.HandleOverlineValueChanged);
			this.fieldOverlinePosition   = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Overline.Position,   Res.Strings.TextPanel.Xline.Short.Overline.Position,   Res.Strings.TextPanel.Xline.Long.Overline.Position,  -0.01, 0.01, 0.0, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, this.HandleOverlineValueChanged);
			this.overlineColor           = this.CreateColorSample(Res.Strings.TextPanel.Xline.Tooltip.Overline.Color, this.HandleSampleColorClicked, this.HandleSampleColorChanged);

			this.fieldStrikeoutThickness  = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Strikeout.Thickness,  Res.Strings.TextPanel.Xline.Short.Strikeout.Thickness,  Res.Strings.TextPanel.Xline.Long.Strikeout.Thickness,  0.0,  0.01, 0.0, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, this.HandleStrikeoutValueChanged);
			this.fieldStrikeoutPosition   = this.CreateTextFieldLabel(Res.Strings.TextPanel.Xline.Tooltip.Strikeout.Position,   Res.Strings.TextPanel.Xline.Short.Strikeout.Position,   Res.Strings.TextPanel.Xline.Long.Strikeout.Position,   0.0,  0.01, 0.0, 0.1, Widgets.TextFieldLabel.Type.TextFieldReal, this.HandleStrikeoutValueChanged);
			this.strikeoutColor           = this.CreateColorSample(Res.Strings.TextPanel.Xline.Tooltip.Strikeout.Color, this.HandleSampleColorClicked, this.HandleSampleColorChanged);
			
			this.buttonClear = this.CreateClearButton(this.HandleClearClicked);

			this.TextWrapper.Active.Changed  += this.HandleWrapperChanged;
			this.TextWrapper.Defined.Changed += this.HandleWrapperChanged;

			this.isNormalAndExtended = true;
			this.UpdateAfterChanging();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.TextWrapper.Active.Changed  -= this.HandleWrapperChanged;
				this.TextWrapper.Defined.Changed -= this.HandleWrapperChanged;
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
			this.underlineColor.ActiveState = ActiveState.No;
			this.overlineColor.ActiveState  = ActiveState.No;
			this.strikeoutColor.ActiveState = ActiveState.No;
		}

		public override void OriginColorSelect(int rank)
		{
			//	Sélectionne l'origine de couleur.
			if ( rank != -1 )
			{
				this.originFieldRank = rank;
				if ( rank == 0 )  this.originFieldColor = this.underlineColor;
				if ( rank == 1 )  this.originFieldColor = this.overlineColor;
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
			if ( sample == this.underlineColor )
			{
				this.TextWrapper.SuspendSynchronizations();
				Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.TextWrapper.Defined.Underline;
				this.FillUnderlineDefinition(xline, false);
				this.TextWrapper.DefineOperationName("FontXline", Res.Strings.TextPanel.Xline.Title);
				this.TextWrapper.ResumeSynchronizations();
				this.ActionMade();
			}

			if ( sample == this.overlineColor )
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

			if ( this.buttonUnderline == null )  return;

			Rectangle rect = this.UsefulZone;

			bool isUnderline = this.TextWrapper.Defined.IsUnderlineDefined;
			bool isOverline  = this.TextWrapper.Defined.IsOverlineDefined;
			bool isStrikeout = this.TextWrapper.Defined.IsStrikeoutDefined;

			if ( this.isExtendedSize )
			{
				Rectangle r = rect;
				r.Bottom = r.Top-20;

				if ( this.IsLabelProperties )
				{
					r.Left = rect.Left;
					r.Width = 20;
					this.buttonUnderline.SetManualBounds(r);
					r.Offset(20, 0);
					this.buttonOverline.SetManualBounds(r);
					r.Offset(20+5, 0);
					this.buttonStrikeout.SetManualBounds(r);

					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.SetManualBounds(r);

					if ( isUnderline )
					{
						r.Offset(0, -25);
						r.Left = rect.Left;
						r.Right = rect.Right-25;
						this.fieldUnderlineThickness.SetManualBounds(r);
						this.fieldUnderlineThickness.Visibility = true;
						r.Offset(0, -25);
						this.fieldUnderlinePosition.SetManualBounds(r);
						this.fieldUnderlinePosition.Visibility = true;
						r.Offset(0, -25);
						r.Left = rect.Right-25-48;
						r.Width = 48;
						this.underlineColor.SetManualBounds(r);
						this.underlineColor.Visibility = true;
					}
					else
					{
						this.fieldUnderlineThickness.Visibility = false;
						this.fieldUnderlinePosition.Visibility = false;
						this.underlineColor.Visibility = false;
					}

					if ( isOverline )
					{
						r.Offset(0, -25);
						r.Left = rect.Left;
						r.Right = rect.Right-25;
						this.fieldOverlineThickness.SetManualBounds(r);
						this.fieldOverlineThickness.Visibility = true;
						r.Offset(0, -25);
						this.fieldOverlinePosition.SetManualBounds(r);
						this.fieldOverlinePosition.Visibility = true;
						r.Offset(0, -25);
						r.Left = rect.Right-25-48;
						r.Width = 48;
						this.overlineColor.SetManualBounds(r);
						this.overlineColor.Visibility = true;
					}
					else
					{
						this.fieldOverlineThickness.Visibility = false;
						this.fieldOverlinePosition.Visibility = false;
						this.overlineColor.Visibility = false;
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
					this.buttonUnderline.SetManualBounds(r);
					r.Offset(20, 0);
					this.buttonOverline.SetManualBounds(r);
					r.Offset(20+5, 0);
					this.buttonStrikeout.SetManualBounds(r);
					r.Left = rect.Right-20;
					r.Width = 20;
					this.buttonClear.SetManualBounds(r);

					if ( isUnderline )
					{
						r.Offset(0, -25);
						r.Left = rect.Left;
						r.Width = 60;
						this.fieldUnderlineThickness.SetManualBounds(r);
						this.fieldUnderlineThickness.Visibility = true;
						r.Offset(60, 0);
						this.fieldUnderlinePosition.SetManualBounds(r);
						this.fieldUnderlinePosition.Visibility = true;
						r.Offset(60+12, 0);
						r.Width = 48;
						this.underlineColor.SetManualBounds(r);
						this.underlineColor.Visibility = true;
					}
					else
					{
						this.fieldUnderlineThickness.Visibility = false;
						this.fieldUnderlinePosition.Visibility = false;
						this.underlineColor.Visibility = false;
					}

					if ( isOverline )
					{
						r.Offset(0, -25);
						r.Left = rect.Left;
						r.Width = 60;
						this.fieldOverlineThickness.SetManualBounds(r);
						this.fieldOverlineThickness.Visibility = true;
						r.Offset(60, 0);
						this.fieldOverlinePosition.SetManualBounds(r);
						this.fieldOverlinePosition.Visibility = true;
						r.Offset(60+12, 0);
						r.Width = 48;
						this.overlineColor.SetManualBounds(r);
						this.overlineColor.Visibility = true;
					}
					else
					{
						this.fieldOverlineThickness.Visibility = false;
						this.fieldOverlinePosition.Visibility = false;
						this.overlineColor.Visibility = false;
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
				this.buttonUnderline.SetManualBounds(r);
				r.Offset(20, 0);
				this.buttonOverline.SetManualBounds(r);
				r.Offset(20+5, 0);
				this.buttonStrikeout.SetManualBounds(r);
				r.Left = rect.Right-20;
				r.Width = 20;
				this.buttonClear.SetManualBounds(r);

				this.fieldUnderlineThickness.Visibility = false;
				this.fieldUnderlinePosition.Visibility = false;
				this.underlineColor.Visibility = false;
				this.fieldOverlineThickness.Visibility = false;
				this.fieldOverlinePosition.Visibility = false;
				this.overlineColor.Visibility = false;
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
			
			bool underline   = this.TextWrapper.Active.IsUnderlineDefined;
			bool isUnderline = this.TextWrapper.Defined.IsUnderlineDefined;

			bool overline     = this.TextWrapper.Active.IsOverlineDefined;
			bool isOverline  = this.TextWrapper.Defined.IsOverlineDefined;

			bool strikeout    = this.TextWrapper.Active.IsStrikeoutDefined;
			bool isStrikeout  = this.TextWrapper.Defined.IsStrikeoutDefined;

			double underlineThickness = 0.0;
			double underlinePosition  = 0.0;
			string underlineColor     = null;

			double overlineThickness = 0.0;
			double overlinePosition  = 0.0;
			string overlineColor     = null;

			double strikeoutThickness = 0.0;
			double strikeoutPosition  = 0.0;
			string strikeoutColor     = null;

			if ( isUnderline )
			{
				underlineThickness = this.TextWrapper.Defined.Underline.Thickness;
				underlinePosition  = this.TextWrapper.Defined.Underline.Position;
				underlineColor     = this.TextWrapper.Defined.Underline.DrawStyle;
			}

			if ( isOverline )
			{
				overlineThickness = this.TextWrapper.Defined.Overline.Thickness;
				overlinePosition  = this.TextWrapper.Defined.Overline.Position;
				overlineColor     = this.TextWrapper.Defined.Overline.DrawStyle;
			}

			if ( isStrikeout )
			{
				strikeoutThickness = this.TextWrapper.Defined.Strikeout.Thickness;
				strikeoutPosition  = this.TextWrapper.Defined.Strikeout.Position;
				strikeoutColor     = this.TextWrapper.Defined.Strikeout.DrawStyle;
			}

			this.ignoreChanged = true;
			
			this.ActiveIconButton(this.buttonUnderline, underline, isUnderline);
			this.ActiveIconButton(this.buttonOverline,  overline,  isOverline );
			this.ActiveIconButton(this.buttonStrikeout, strikeout, isStrikeout );

			this.SetTextFieldRealValue(this.fieldUnderlineThickness.TextFieldReal, underlineThickness, Common.Text.Properties.SizeUnits.Points, isUnderline, true);
			this.SetTextFieldRealValue(this.fieldUnderlinePosition.TextFieldReal,  underlinePosition,  Common.Text.Properties.SizeUnits.Points, isUnderline, true);
			this.SetTextFieldRealValue(this.fieldOverlineThickness.TextFieldReal,  overlineThickness,  Common.Text.Properties.SizeUnits.Points, isOverline,  true);
			this.SetTextFieldRealValue(this.fieldOverlinePosition.TextFieldReal,   overlinePosition,   Common.Text.Properties.SizeUnits.Points, isOverline,  true);
			this.SetTextFieldRealValue(this.fieldStrikeoutThickness.TextFieldReal, strikeoutThickness, Common.Text.Properties.SizeUnits.Points, isStrikeout,  true);
			this.SetTextFieldRealValue(this.fieldStrikeoutPosition.TextFieldReal,  strikeoutPosition,  Common.Text.Properties.SizeUnits.Points, isStrikeout,  true);

			this.SetColorSample(this.underlineColor, underlineColor, isUnderline, true);
			this.SetColorSample(this.overlineColor,  overlineColor,  isOverline,  true);
			this.SetColorSample(this.strikeoutColor, strikeoutColor, isStrikeout,  true);

			if ( this.underlineColor.ActiveState == ActiveState.Yes ||
				 this.overlineColor.ActiveState  == ActiveState.Yes ||
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
			
			this.TextWrapper.DefineOperationName("FontUnderline", Res.Strings.Action.FontUnderline);
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
			
			this.TextWrapper.DefineOperationName("FontOverline", Res.Strings.Action.FontOverline);
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
		
		private void HandleUnderlineValueChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;

			this.TextWrapper.SuspendSynchronizations();
			Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.TextWrapper.Defined.Underline;
			this.FillUnderlineDefinition(xline, false);
			this.TextWrapper.DefineOperationName("FontUnderline", Res.Strings.Action.FontUnderline);
			this.TextWrapper.ResumeSynchronizations();
			this.ActionMade();
		}

		private void HandleOverlineValueChanged(object sender)
		{
			if ( this.ignoreChanged )  return;
			if ( !this.TextWrapper.IsAttached )  return;

			this.TextWrapper.SuspendSynchronizations();
			Common.Text.Wrappers.TextWrapper.XlineDefinition xline = this.TextWrapper.Defined.Overline;
			this.FillOverlineDefinition(xline, false);
			this.TextWrapper.DefineOperationName("FontOverline", Res.Strings.Action.FontOverline);
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
			if ( this.originFieldColor == this.underlineColor )  this.originFieldRank = 0;
			if ( this.originFieldColor == this.overlineColor  )  this.originFieldRank = 1;
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
				xline.Thickness = (double) this.fieldUnderlineThickness.TextFieldReal.InternalValue;
				xline.Position  = (double) this.fieldUnderlinePosition.TextFieldReal.InternalValue;
				xline.DrawStyle = this.GetColorSample(this.underlineColor);
			}
		}
        
		private void FillOverlineDefinition(Common.Text.Wrappers.TextWrapper.XlineDefinition xline, bool def)
		{
			this.document.Wrappers.FillOverlineDefinition(xline);

			if ( !def )
			{
				xline.Thickness = (double) this.fieldOverlineThickness.TextFieldReal.InternalValue;
				xline.Position  = (double) this.fieldOverlinePosition.TextFieldReal.InternalValue;
				xline.DrawStyle = this.GetColorSample(this.overlineColor);
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
        

		protected IconButton				buttonUnderline;
		protected IconButton				buttonOverline;
		protected IconButton				buttonStrikeout;
		protected Widgets.TextFieldLabel	fieldUnderlineThickness;
		protected Widgets.TextFieldLabel	fieldUnderlinePosition;
		protected ColorSample				underlineColor;
		protected Widgets.TextFieldLabel	fieldOverlineThickness;
		protected Widgets.TextFieldLabel	fieldOverlinePosition;
		protected ColorSample				overlineColor;
		protected Widgets.TextFieldLabel	fieldStrikeoutThickness;
		protected Widgets.TextFieldLabel	fieldStrikeoutPosition;
		protected ColorSample				strikeoutColor;
		protected IconButton				buttonClear;
		protected string					signature = "";

		protected ColorSample				originFieldColor;
		protected int						originFieldRank = -1;
	}
}
