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

		void PaintWindowBackground(Drawing.Graphics graphics, Drawing.Rectangle windowRect, Drawing.Rectangle paintRect, WidgetState state);

		void PaintGlyph(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, GlyphShape type, PaintTextStyle style);
		void PaintCheck(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state);
		void PaintRadio(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state);
		void PaintIcon(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, string icon);
		
		void PaintButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir, ButtonStyle style);
		void PaintButtonTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, WidgetState state, ButtonStyle style);
		void PaintButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir, ButtonStyle style);
		
		void PaintTextFieldBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, TextFieldStyle style, bool readOnly);
		void PaintTextFieldForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, TextFieldStyle style, bool readOnly);
		
		void PaintScrollerBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, WidgetState state, Direction dir);
		void PaintScrollerHandle(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, WidgetState state, Direction dir);
		void PaintScrollerForeground(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, WidgetState state, Direction dir);
		
		void PaintGroupBox(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, WidgetState state);
		void PaintSepLine(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, WidgetState state, Direction dir);

		void PaintFrameTitleBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Drawing.Rectangle titleRect, WidgetState state, Direction dir);
		void PaintFrameTitleForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Drawing.Rectangle titleRect, WidgetState state, Direction dir);
		void PaintFrameBody(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir);
		
		void PaintTabBand(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir);
		void PaintTabFrame(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir);
		void PaintTabAboveBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, WidgetState state, Direction dir);
		void PaintTabAboveForeground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, WidgetState state, Direction dir);
		void PaintTabSunkenBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, WidgetState state, Direction dir);
		void PaintTabSunkenForeground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle titleRect, WidgetState state, Direction dir);
		
		void PaintArrayBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state);
		void PaintArrayForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state);
		void PaintCellBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state);

		void PaintHeaderBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir);
		void PaintHeaderForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir);

		void PaintToolBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir);
		void PaintToolForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir);

		void PaintMenuBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir, Drawing.Rectangle parentRect, double iconWidth);
		void PaintMenuForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir, Drawing.Rectangle parentRect, double iconWidth);
		void PaintMenuItemBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir, MenuType type, MenuItemType itemType);
		void PaintMenuItemTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, WidgetState state, Direction dir, MenuType type, MenuItemType itemType);
		void PaintMenuItemForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir, MenuType type, MenuItemType itemType);

		void PaintSeparatorBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir, bool optional);
		void PaintSeparatorForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir, bool optional);

		void PaintPaneButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir);
		void PaintPaneButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir);

		void PaintStatusBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state);
		void PaintStatusForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state);
		void PaintStatusItemBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state);
		void PaintStatusItemForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state);

		void PaintRibbonButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state);
		void PaintRibbonButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state);
		void PaintRibbonButtonTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, WidgetState state);
		void PaintRibbonTabBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, double titleHeight, WidgetState state);
		void PaintRibbonTabForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, double titleHeight, WidgetState state);
		void PaintRibbonSectionBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, double titleHeight, WidgetState state);
		void PaintRibbonSectionForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, double titleHeight, WidgetState state);
		void PaintRibbonSectionTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, WidgetState state);

		void PaintTagBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Drawing.Color color, Direction dir);
		void PaintTagForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Drawing.Color color, Direction dir);

		void PaintTooltipBackground(Drawing.Graphics graphics, Drawing.Rectangle rect);
		void PaintTooltipTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text);

		/*
		 * Méthodes de dessin complémentaires.
		 */
		
		void PaintFocusBox(Drawing.Graphics graphics, Drawing.Rectangle rect);
		void PaintTextCursor(Drawing.Graphics graphics, Drawing.Point p1, Drawing.Point p2, bool cursorOn);
		void PaintTextSelectionBackground(Drawing.Graphics graphics, TextLayout.SelectedArea[] areas, WidgetState state);
		void PaintTextSelectionForeground(Drawing.Graphics graphics, TextLayout.SelectedArea[] areas, WidgetState state);
		
		void PaintGeneralTextLayout(Drawing.Graphics graphics, Drawing.Rectangle clipRect, Drawing.Point pos, TextLayout text, WidgetState state, PaintTextStyle style, Drawing.Color backColor);

		void AdaptPictogramColor(ref Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor);

		Drawing.Color ColorCaption { get; }
		Drawing.Color ColorControl { get; }
		Drawing.Color ColorWindow { get; }
		Drawing.Color ColorDisabled { get; }
		Drawing.Color ColorBorder { get; }
		Drawing.Color ColorTextBackground { get; }
		Drawing.Color ColorText(WidgetState state);
		Drawing.Color ColorTextSliderBorder(bool enabled);
		Drawing.Color ColorTextFieldBorder(bool enabled);

		double AlphaVMenu { get; }

		Drawing.Margins GeometryMenuShadow { get; }
		Drawing.Margins GeometryMenuMargins { get; }
		Drawing.Margins GeometryArrayMargins { get; }
		Drawing.Margins GeometryRadioShapeBounds { get; }
		Drawing.Margins GeometryGroupShapeBounds { get; }
		Drawing.Margins GeometryToolShapeBounds { get; }
		Drawing.Margins GeometryButtonShapeBounds { get; }
		Drawing.Margins GeometryRibbonShapeBounds { get; }
		Drawing.Margins GeometryTextFieldShapeBounds { get; }
		Drawing.Margins GeometryListShapeBounds { get; }
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
