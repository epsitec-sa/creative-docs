namespace Epsitec.Common.Widgets
{
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

		void PaintArrow(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction dir);
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

		void PaintTooltipBackground(Drawing.Graphics graphics, Drawing.Rectangle rect);
		void PaintTooltipTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text);

		/*
		 * Méthodes de dessin complémentaires.
		 */
		
		void PaintFocusBox(Drawing.Graphics graphics, Drawing.Rectangle rect);
		void PaintTextCursor(Drawing.Graphics graphics, Drawing.Rectangle rect, bool cursorOn);
		void PaintTextSelectionBackground(Drawing.Graphics graphics, Drawing.Rectangle[] rect);
		void PaintTextSelectionForeground(Drawing.Graphics graphics, Drawing.Rectangle[] rect);
		
		void PaintGeneralTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, WidgetState state);

		void AdaptPictogramColor(ref Drawing.Color color, Drawing.GlyphPaintStyle paintStyle, Drawing.Color uniqueColor);

		Drawing.Color ColorCaption { get; }
		Drawing.Color ColorControl { get; }
		Drawing.Color ColorWindow { get; }
		Drawing.Color ColorDisabled { get; }
		Drawing.Color ColorBorder { get; }
		Drawing.Color ColorTextFieldBorder(bool enabled);

		double AlphaVMenu { get; }

		Drawing.Margins GeometryMenuMargins { get; }
		Drawing.Margins GeometryRadioShapeBounds { get; }
		Drawing.Margins GeometryGroupShapeBounds { get; }
		double GeometryComboRightMargin { get; }
		double GeometryComboBottomMargin { get; }
		double GeometryComboTopMargin { get; }
		double GeometryUpDownRightMargin { get; }
		double GeometryUpDownBottomMargin { get; }
		double GeometryUpDownTopMargin { get; }
		double GeometryScrollerRightMargin { get; }
		double GeometryScrollerBottomMargin { get; }
		double GeometryScrollerTopMargin { get; }
		double GeometryScrollListLeftMargin { get; }
		double GeometryScrollListRightMargin { get; }
		double GeometrySliderLeftMargin { get; }
		double GeometrySliderRightMargin { get; }
	}
	
	public enum Direction
	{
		None, Left, Right, Up, Down
	}
}
