//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Adorners
{
	/// <summary>
	/// La classe AbstractAdorner définit les méthodes et propriétés communes
	/// aux divers adorners. Ceci permet d'éviter la multiplication du code
	/// pour les réglages partagés par beaucoup d'adorners.
	/// </summary>
	public abstract class AbstractAdorner : IAdorner
	{
		protected AbstractAdorner()
		{
			this.RefreshColors();
		}
		
		
		protected virtual void RefreshColors()
		{
			this.colorBlack             = Drawing.Color.FromRgb(0,0,0);
			this.colorWhite             = Drawing.Color.FromRgb(1,1,1);
			this.colorControl           = Drawing.Color.FromName("Control");
			this.colorControlLight      = Drawing.Color.FromName("ControlLight");
			this.colorControlLightLight = Drawing.Color.FromName("ControlLightLight");
			this.colorControlDark       = Drawing.Color.FromName("ControlDark");
			this.colorControlDarkDark   = Drawing.Color.FromName("ControlDarkDark");
			this.colorCaption           = Drawing.Color.FromName("ActiveCaption");
			this.colorCaptionText       = Drawing.Color.FromName("ActiveCaptionText");
			this.colorInfo              = Drawing.Color.FromName("Info");
		}


		protected static bool IsThreeState2(Widgets.WidgetPaintState state)
		{
			//	Indique si un IconButton en mode ThreeState est 2 pixels plus haut.
			if ( (state&WidgetPaintState.ThreeState) == 0 )  return false;

			return ( (state&WidgetPaintState.ActiveYes)   != 0 ||
					 (state&WidgetPaintState.ActiveMaybe) != 0 );
		}

		protected static Drawing.Path PathThreeState2Frame(Drawing.Rectangle rect, Widgets.WidgetPaintState state)
		{
			//	Donne le chemin d'un IconButton en mode ThreeState.
			Drawing.Path path = new Drawing.Path();

			if ( AbstractAdorner.IsThreeState2(state) )
			{
				path.MoveTo(rect.Left, rect.Bottom);
				path.LineTo(rect.Left, rect.Top-2);
				path.LineTo(rect.Left+2, rect.Top);
				path.LineTo(rect.Right-2, rect.Top);
				path.LineTo(rect.Right, rect.Top-2);
				path.LineTo(rect.Right, rect.Bottom);
				path.Close();
			}
			else
			{
				path.MoveTo(rect.Left, rect.Bottom);
				path.LineTo(rect.Left, rect.Top);
				path.LineTo(rect.Right, rect.Top);
				path.LineTo(rect.Right, rect.Bottom);
				path.Close();
			}

			return path;
		}
		
		
		#region IAdorner Members
		public abstract void PaintWindowBackground(Drawing.Graphics graphics, Drawing.Rectangle windowRect, Drawing.Rectangle paintRect, Widgets.WidgetPaintState state);
		public abstract void PaintGlyph(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.GlyphShape type, Widgets.PaintTextStyle style);
		public abstract void PaintGlyph(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Drawing.Color color, Widgets.GlyphShape type, Widgets.PaintTextStyle style);
		public abstract void PaintCheck(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintRadio(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintIcon(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, string icon);
		public abstract void PaintButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, Widgets.ButtonStyle style);
		public abstract void PaintButtonTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, Widgets.WidgetPaintState state, Widgets.ButtonStyle style);
		public abstract void PaintButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, Widgets.ButtonStyle style);
		public abstract void PaintTextFieldBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.TextFieldStyle style, TextDisplayMode mode, bool readOnly);
		public abstract void PaintTextFieldForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.TextFieldStyle style, TextDisplayMode mode, bool readOnly);
		public abstract void PaintScrollerBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintScrollerHandle(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintScrollerForeground(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintSliderBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintSliderHandle(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintSliderForeground(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintGroupBox(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state);
		public abstract void PaintSepLine(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintFrameTitleBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintFrameTitleForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintFrameBody(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintTabBand(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintTabFrame(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintTabAboveBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintTabAboveForeground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintTabSunkenBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintTabSunkenForeground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintArrayBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintArrayForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintCellBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintHeaderBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintHeaderForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintToolBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintToolForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintMenuBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, Drawing.Rectangle parentRect, double iconWidth);
		public abstract void PaintMenuForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, Drawing.Rectangle parentRect, double iconWidth);
		public abstract void PaintMenuItemBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, Widgets.MenuOrientation type, Widgets.MenuItemType itemType);
		public abstract void PaintMenuItemTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, Widgets.WidgetPaintState state, Widgets.Direction dir, Widgets.MenuOrientation type, Widgets.MenuItemType itemType);
		public abstract void PaintMenuItemForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, Widgets.MenuOrientation type, Widgets.MenuItemType itemType);
		public abstract void PaintSeparatorBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, bool optional);
		public abstract void PaintSeparatorForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir, bool optional);
		public abstract void PaintPaneButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintPaneButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Widgets.Direction dir);
		public abstract void PaintStatusBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintStatusForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintStatusItemBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintStatusItemForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintRibbonButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintRibbonButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state);
		public abstract void PaintRibbonButtonTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, Widgets.WidgetPaintState state);
		public abstract void PaintRibbonTabBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, double titleHeight, Widgets.WidgetPaintState state);
		public abstract void PaintRibbonTabForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, double titleHeight, Widgets.WidgetPaintState state);
		public abstract void PaintRibbonSectionBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, double titleHeight, Widgets.WidgetPaintState state);
		public abstract void PaintRibbonSectionForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, double titleHeight, Widgets.WidgetPaintState state);
		public abstract void PaintRibbonSectionTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, Widgets.WidgetPaintState state);
		public abstract void PaintTagBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Drawing.Color color, Widgets.Direction dir);
		public abstract void PaintTagForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Widgets.WidgetPaintState state, Drawing.Color color, Widgets.Direction dir);
		public abstract void PaintTooltipBackground(Drawing.Graphics graphics, Drawing.Rectangle rect);
		public abstract void PaintTooltipTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text);
		public abstract void PaintFocusBox(Drawing.Graphics graphics, Drawing.Rectangle rect);
		public abstract void PaintTextCursor(Drawing.Graphics graphics, Drawing.Point p1, Drawing.Point p2, bool cursorOn);
		public abstract void PaintTextSelectionBackground(Drawing.Graphics graphics, Widgets.TextLayout.SelectedArea[] areas, Widgets.WidgetPaintState state, PaintTextStyle style, TextDisplayMode mode);
		public abstract void PaintTextSelectionForeground(Drawing.Graphics graphics, Widgets.TextLayout.SelectedArea[] areas, Widgets.WidgetPaintState state, PaintTextStyle style, TextDisplayMode mode);
		public abstract void PaintGeneralTextLayout(Drawing.Graphics graphics, Drawing.Rectangle clipRect, Drawing.Point pos, TextLayout text, Widgets.WidgetPaintState state, Widgets.PaintTextStyle style, TextDisplayMode mode, Drawing.Color backColor);
		public abstract void AdaptPictogramColor(ref Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor);
		
		public virtual Drawing.Color ColorCaption
		{
			get
			{
				return this.colorCaption;
			}
		}

		public virtual Drawing.Color ColorControl
		{
			get
			{
				return this.colorControl;
			}
		}

		public abstract Drawing.Color ColorWindow
		{
			get;
		}

		public virtual Drawing.Color ColorDisabled
		{
			get
			{
				return Drawing.Color.Empty;
			}
		}

		public virtual Drawing.Color ColorBorder
		{
			get
			{
				return this.colorControlDarkDark;
			}
		}

		public virtual Drawing.Color ColorTextBackground
		{
			get
			{
				return this.colorControlLightLight;
			}
		}

		public abstract Drawing.Color ColorText(Widgets.WidgetPaintState state);

		public abstract Drawing.Color ColorTextSliderBorder(bool enabled);

		public abstract Drawing.Color ColorTextFieldBorder(bool enabled);

		public abstract Drawing.Color ColorTextDisplayMode(TextDisplayMode mode);

		public virtual double AlphaMenu
		{
			get
			{
				return 1.0;
			}
		}

		public abstract Drawing.Margins GeometryMenuShadow
		{
			get;
		}

		public abstract Drawing.Margins GeometryMenuMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryArrayMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryRadioShapeMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryGroupShapeMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryToolShapeMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryThreeStateShapeMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryButtonShapeMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryRibbonShapeMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryTextFieldShapeMargins
		{
			get;
		}

		public abstract Drawing.Margins GeometryListShapeMargins
		{
			get;
		}

		public abstract double GeometryComboRightMargin
		{
			get;
		}

		public abstract double GeometryComboBottomMargin
		{
			get;
		}

		public abstract double GeometryComboTopMargin
		{
			get;
		}

		public abstract double GeometryUpDownWidthFactor
		{
			get;
		}

		public abstract double GeometryUpDownRightMargin
		{
			get;
		}

		public abstract double GeometryUpDownBottomMargin
		{
			get;
		}

		public abstract double GeometryUpDownTopMargin
		{
			get;
		}

		public abstract double GeometryScrollerRightMargin
		{
			get;
		}

		public abstract double GeometryScrollerBottomMargin
		{
			get;
		}

		public abstract double GeometryScrollerTopMargin
		{
			get;
		}

		public abstract double GeometryScrollListXMargin
		{
			get;
		}

		public abstract double GeometryScrollListYMargin
		{
			get;
		}

		public abstract double GeometrySliderLeftMargin
		{
			get;
		}

		public abstract double GeometrySliderRightMargin
		{
			get;
		}

		public abstract double GeometrySliderBottomMargin
		{
			get;
		}
		#endregion
		
		protected Drawing.Color		colorBlack;
		protected Drawing.Color		colorWhite;
		protected Drawing.Color		colorControl;
		protected Drawing.Color		colorControlLight;
		protected Drawing.Color		colorControlLightLight;
		protected Drawing.Color		colorControlDark;
		protected Drawing.Color		colorControlDarkDark;
		protected Drawing.Color		colorCaption;
		protected Drawing.Color		colorCaptionNF;  // NF = no focused
		protected Drawing.Color		colorCaptionText;
		protected Drawing.Color		colorInfo;
	}
}
