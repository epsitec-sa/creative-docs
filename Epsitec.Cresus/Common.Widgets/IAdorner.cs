namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'interface IAdorner donne acc�s aux routines de dessin des divers
	/// �l�ments de l'interface graphique.
	/// </summary>
	public interface IAdorner
	{
		/*
		 * Le dessin de l'ombre se fait en principe sur le bord droit et sur le bord
		 * inf�rieur (orientation normale, �clairage venant du coin sup�rieur gauche).
		 * 
		 * Lorsqu'un widget est tourn�, Widget.RootDirection indique comment peindre
		 * l'ombre. Les cas suivants doivent �tre trait�s :
		 * 
		 * Direction.Up     => ombre en bas et � droite (standard, pas de rotation)
		 * Direction.Left   => ombre � gauche et en bas
		 * Direction.Down   => ombre en haut et � gauche
		 * Direction.Right  => ombre � droite et en haut
		 */

		void PaintWindowBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow);

		void PaintArrow(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, Direction dir);
		void PaintCheck(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow);
		void PaintRadio(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow);
		void PaintIcon(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, string icon);
		
		void PaintButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, ButtonStyle style);
		void PaintButtonTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, WidgetState state, Direction shadow, ButtonStyle style);
		void PaintButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, ButtonStyle style);
		
		void PaintTextFieldBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, TextFieldStyle style, bool readOnly);
		void PaintTextFieldForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, TextFieldStyle style, bool readOnly);
		
		void PaintScrollerBackground(Drawing.Graphics graphics, Drawing.Rectangle frame_rect, Drawing.Rectangle tab_rect, WidgetState state, Direction shadow);
		void PaintScrollerHandle(Drawing.Graphics graphics, Drawing.Rectangle frame_rect, Drawing.Rectangle tab_rect, WidgetState state, Direction shadow);
		void PaintScrollerForeground(Drawing.Graphics graphics, Drawing.Rectangle frame_rect, Drawing.Rectangle tab_rect, WidgetState state, Direction shadow);
		
		void PaintGroupBox(Drawing.Graphics graphics, Drawing.Rectangle frame_rect, Drawing.Rectangle title_rect, WidgetState state, Direction shadow);
		void PaintSepLine(Drawing.Graphics graphics, Drawing.Rectangle frame_rect, Drawing.Rectangle title_rect, WidgetState state, Direction shadow);

		void PaintFrameTitleBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Drawing.Rectangle title_rect, WidgetState state, Direction shadow);
		void PaintFrameTitleForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, Drawing.Rectangle title_rect, WidgetState state, Direction shadow);
		void PaintFrameBody(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow);
		
		void PaintTabBand(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow);
		void PaintTabFrame(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow);
		void PaintTabAboveBackground(Drawing.Graphics graphics, Drawing.Rectangle frame_rect, Drawing.Rectangle title_rect, WidgetState state, Direction shadow);
		void PaintTabAboveForeground(Drawing.Graphics graphics, Drawing.Rectangle frame_rect, Drawing.Rectangle title_rect, WidgetState state, Direction shadow);
		void PaintTabSunkenBackground(Drawing.Graphics graphics, Drawing.Rectangle frame_rect, Drawing.Rectangle title_rect, WidgetState state, Direction shadow);
		void PaintTabSunkenForeground(Drawing.Graphics graphics, Drawing.Rectangle frame_rect, Drawing.Rectangle title_rect, WidgetState state, Direction shadow);
		
		void PaintArrayBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow);
		void PaintCellBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow);

		void PaintHeaderBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, Direction type);
		void PaintHeaderForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, Direction type);

		void PaintToolBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, Direction type);
		void PaintToolForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, Direction type);

		void PaintMenuBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, Drawing.Rectangle parentRect, double iconWidth);
		void PaintMenuForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, Drawing.Rectangle parentRect, double iconWidth);

		void PaintMenuItemBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, MenuType type, MenuItemType itemType);
		void PaintMenuItemTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, WidgetState state, Direction shadow, MenuType type, MenuItemType itemType);
		void PaintMenuItemForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, MenuType type, MenuItemType itemType);

		void PaintSeparatorBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, Direction type, bool optional);
		void PaintSeparatorForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, Direction type, bool optional);

		void PaintStatusBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow);
		void PaintStatusForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow);

		void PaintTooltipBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, Direction shadow);
		void PaintTooltipTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, Direction shadow);

		/*
		 * M�thodes de dessin compl�mentaires.
		 */
		
		void PaintFocusBox(Drawing.Graphics graphics, Drawing.Rectangle rect);
		void PaintTextCursor(Drawing.Graphics graphics, Drawing.Rectangle rect, bool cursor_on);
		void PaintTextSelectionBackground(Drawing.Graphics graphics, Drawing.Rectangle[] rect);
		void PaintTextSelectionForeground(Drawing.Graphics graphics, Drawing.Rectangle[] rect);
		
		void PaintGeneralTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, WidgetState state, Direction shadow);

		void AdaptEnabledTextColor(ref Drawing.Color color);
		void AdaptDisabledTextColor(ref Drawing.Color color, Drawing.Color uniqueColor);

		Drawing.Color GetColorCaption();
		Drawing.Color GetColorControl();
		Drawing.Color GetColorWindow();
		Drawing.Color GetColorBorder();
	}
	
	public enum Direction
	{
		None, Left, Right, Up, Down
	}
}
