namespace Epsitec.Common.Widgets
{
	public enum PaintTextStyle
	{
		StaticText,
		TextField,
		Group,
		Status,
		Button,
		CheckButton,
		RadioButton,
		List,
		Array,
		Header,
		HMenu,
		VMenu,
	}

	/// <summary>
	/// L'interface IAdorner donne accès aux routines de dessin des divers
	/// éléments de l'interface graphique.
	/// </summary>
	public interface IAdorner
	{
		/*
		 * Le dessin de l'ombre se fait en principe sur le bord droit et sur le bord
		 * inférieur (orientation normale, éclairage venant du coin supérieur gauche).
		 * 
		 * Lorsqu'un widget est tourné, Widget.RootDirection indique comment peindre
		 * l'ombre. Les cas suivants doivent être traités :
		 * 
		 * Direction.Up     => ombre en bas et à droite (standard, pas de rotation)
		 * Direction.Left   => ombre à gauche et en bas
		 * Direction.Down   => ombre en haut et à gauche
		 * Direction.Right  => ombre à droite et en haut
		 */

		void PaintWindowBackground(Drawing.Graphics graphics, Drawing.Rectangle windowRect, Drawing.Rectangle paintRect, WidgetPaintState state);

		void PaintGlyph(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, GlyphShape type, PaintTextStyle style);
		void PaintGlyph(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Drawing.Color color, GlyphShape type, PaintTextStyle style);
		void PaintCheck(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		void PaintRadio(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		void PaintIcon(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, string icon);
		
		void PaintButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir, ButtonStyle style);
		void PaintButtonTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, WidgetPaintState state, ButtonStyle style);
		void PaintButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir, ButtonStyle style);
		
		void PaintTextFieldBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, TextFieldStyle style, TextDisplayMode mode, bool readOnly);
		void PaintTextFieldForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, TextFieldStyle style, TextDisplayMode mode, bool readOnly);
		
		void PaintScrollerBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, WidgetPaintState state, Direction dir);
		void PaintScrollerHandle(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, WidgetPaintState state, Direction dir);
		void PaintScrollerForeground(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, WidgetPaintState state, Direction dir);
		
		void PaintSliderBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, WidgetPaintState state, Direction dir);
		void PaintSliderHandle(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, WidgetPaintState state, Direction dir);
		void PaintSliderForeground(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, WidgetPaintState state, Direction dir);
		
		void PaintGroupBox(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, WidgetPaintState state);
		void PaintSepLine(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, WidgetPaintState state, Direction dir);

		void PaintFrameTitleBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Drawing.Rectangle titleRect, WidgetPaintState state, Direction dir);
		void PaintFrameTitleForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Drawing.Rectangle titleRect, WidgetPaintState state, Direction dir);
		void PaintFrameBody(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir);
		
		void PaintTabBand(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir);
		void PaintTabFrame(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir);
		void PaintTabAboveBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, WidgetPaintState state, Direction dir);
		void PaintTabAboveForeground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, WidgetPaintState state, Direction dir);
		void PaintTabSunkenBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, WidgetPaintState state, Direction dir);
		void PaintTabSunkenForeground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, WidgetPaintState state, Direction dir);
		
		void PaintArrayBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		void PaintArrayForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		void PaintCellBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);

		void PaintHeaderBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir);
		void PaintHeaderForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir);

		void PaintToolBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir);
		void PaintToolForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir);

		void PaintMenuBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir, Drawing.Rectangle parentRect, double iconWidth);
		void PaintMenuForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir, Drawing.Rectangle parentRect, double iconWidth);
		void PaintMenuItemBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir, MenuOrientation type, MenuItemType itemType);
		void PaintMenuItemTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, WidgetPaintState state, Direction dir, MenuOrientation type, MenuItemType itemType);
		void PaintMenuItemForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir, MenuOrientation type, MenuItemType itemType);

		void PaintSeparatorBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir, bool optional);
		void PaintSeparatorForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir, bool optional);

		void PaintPaneButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir);
		void PaintPaneButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir);

		void PaintStatusBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		void PaintStatusForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		void PaintStatusItemBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		void PaintStatusItemForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);

		void PaintRibbonButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		void PaintRibbonButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		void PaintRibbonButtonTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, WidgetPaintState state);
		void PaintRibbonTabBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, double titleHeight, WidgetPaintState state);
		void PaintRibbonTabForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, double titleHeight, WidgetPaintState state);
		void PaintRibbonSectionBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, double titleHeight, WidgetPaintState state);
		void PaintRibbonSectionForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, double titleHeight, WidgetPaintState state);
		void PaintRibbonSectionTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, WidgetPaintState state);

		void PaintTagBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Drawing.Color color, Direction dir);
		void PaintTagForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Drawing.Color color, Direction dir);

		void PaintTooltipBackground(Drawing.Graphics graphics, Drawing.Rectangle rect);
		void PaintTooltipTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text);

		/*
		 * Méthodes de dessin complémentaires.
		 */
		
		void PaintFocusBox(Drawing.Graphics graphics, Drawing.Rectangle rect);
		void PaintTextCursor(Drawing.Graphics graphics, Drawing.Point p1, Drawing.Point p2, bool cursorOn);
		void PaintTextSelectionBackground(Drawing.Graphics graphics, TextLayout.SelectedArea[] areas, WidgetPaintState state, PaintTextStyle style, TextDisplayMode mode);
		void PaintTextSelectionForeground(Drawing.Graphics graphics, TextLayout.SelectedArea[] areas, WidgetPaintState state, PaintTextStyle style, TextDisplayMode mode);
		
		void PaintGeneralTextLayout(Drawing.Graphics graphics, Drawing.Rectangle clipRect, Drawing.Point pos, TextLayout text, WidgetPaintState state, PaintTextStyle style, TextDisplayMode mode, Drawing.Color backColor);

		void AdaptPictogramColor(ref Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor);

		Drawing.Color ColorCaption { get; }
		Drawing.Color ColorControl { get; }
		Drawing.Color ColorWindow { get; }
		Drawing.Color ColorDisabled { get; }
		Drawing.Color ColorBorder { get; }
		Drawing.Color ColorTextBackground { get; }
		Drawing.Color ColorText(WidgetPaintState state);
		Drawing.Color ColorTextSliderBorder(bool enabled);
		Drawing.Color ColorTextFieldBorder(bool enabled);
		Drawing.Color ColorTextDisplayMode(TextDisplayMode mode);

		double AlphaMenu { get; }

		Drawing.Margins GeometryMenuShadow { get; }
		Drawing.Margins GeometryMenuMargins { get; }
		Drawing.Margins GeometryArrayMargins { get; }
		Drawing.Margins GeometryRadioShapeMargins { get; }
		Drawing.Margins GeometryGroupShapeMargins { get; }
		Drawing.Margins GeometryToolShapeMargins { get; }
		Drawing.Margins GeometryThreeStateShapeMargins { get; }
		Drawing.Margins GeometryButtonShapeMargins { get; }
		Drawing.Margins GeometryRibbonShapeMargins { get; }
		Drawing.Margins GeometryTextFieldShapeMargins { get; }
		Drawing.Margins GeometryListShapeMargins { get; }
		double GeometryComboRightMargin { get; }
		double GeometryComboBottomMargin { get; }
		double GeometryComboTopMargin { get; }
		double GeometryUpDownWidthFactor { get; }
		double GeometryUpDownRightMargin { get; }
		double GeometryUpDownBottomMargin { get; }
		double GeometryUpDownTopMargin { get; }
		double GeometryScrollerRightMargin { get; }
		double GeometryScrollerBottomMargin { get; }
		double GeometryScrollerTopMargin { get; }
		double GeometryScrollListXMargin { get; }
		double GeometryScrollListYMargin { get; }
		double GeometrySliderLeftMargin { get; }
		double GeometrySliderRightMargin { get; }
		double GeometrySliderBottomMargin { get; }
	}
	
	public enum Direction
	{
		None, Left, Right, Up, Down
	}
}
