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

		void PaintWindowBackground(Drawing.Graphics graphics, Drawing.Rectangle windowRect, Drawing.Rectangle paintRect, WidgetPaintState state);

		void PaintGlyph(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, GlyphShape type, PaintTextStyle style);
		void PaintGlyph(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Drawing.Color color, GlyphShape type, PaintTextStyle style);
		void PaintCheck(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		void PaintRadio(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		void PaintIcon(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, string icon);
		
		void PaintButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir, ButtonStyle style);
		void PaintButtonTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text, WidgetPaintState state, ButtonStyle style);
		void PaintButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Direction dir, ButtonStyle style);
		
		void PaintTextFieldBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, TextFieldStyle style, TextFieldDisplayMode mode, bool readOnly);
		void PaintTextFieldForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, TextFieldStyle style, TextFieldDisplayMode mode, bool readOnly);
		
		void PaintScrollerBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, WidgetPaintState state, Direction dir);
		void PaintScrollerHandle(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, WidgetPaintState state, Direction dir);
		void PaintScrollerForeground(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, WidgetPaintState state, Direction dir);

		void PaintSliderBackground(Drawing.Graphics graphics, Drawing.Rectangle frameRect, Drawing.Rectangle sliderRect, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, WidgetPaintState state, Direction dir);
		void PaintSliderHandle(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, WidgetPaintState state, Direction dir);
		void PaintSliderForeground(Drawing.Graphics graphics, Drawing.Rectangle thumbRect, Drawing.Rectangle tabRect, WidgetPaintState state, Direction dir);
		
		void PaintProgressIndicator(Drawing.Graphics graphics, Drawing.Rectangle rect, ProgressIndicatorStyle style, double progress);

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

		void PaintRibbonTabBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		void PaintRibbonTabForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		void PaintRibbonPageBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		void PaintRibbonPageForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state);
		void PaintRibbonButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, ActiveState active);
		void PaintRibbonButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, ActiveState active);
		void PaintRibbonButtonTextLayout(Drawing.Graphics graphics, Drawing.Rectangle rect, TextLayout text, WidgetPaintState state, ActiveState active);
		void PaintRibbonSectionBackground(Drawing.Graphics graphics, Drawing.Rectangle fullRect, Drawing.Rectangle userRect, Drawing.Rectangle textRect, TextLayout text, WidgetPaintState state);
		void PaintRibbonSectionForeground(Drawing.Graphics graphics, Drawing.Rectangle fullRect, Drawing.Rectangle userRect, Drawing.Rectangle textRect, TextLayout text, WidgetPaintState state);

		void PaintTagBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Drawing.Color color, Direction dir);
		void PaintTagForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetPaintState state, Drawing.Color color, Direction dir);

		void PaintTooltipBackground(Drawing.Graphics graphics, Drawing.Rectangle rect);
		void PaintTooltipTextLayout(Drawing.Graphics graphics, Drawing.Point pos, TextLayout text);

		/*
		 * M�thodes de dessin compl�mentaires.
		 */
		
		void PaintFocusBox(Drawing.Graphics graphics, Drawing.Rectangle rect);
		void PaintTextCursor(Drawing.Graphics graphics, Drawing.Point p1, Drawing.Point p2, bool cursorOn);
		void PaintTextSelectionBackground(Drawing.Graphics graphics, TextLayout.SelectedArea[] areas, WidgetPaintState state, PaintTextStyle style, TextFieldDisplayMode mode);
		void PaintTextSelectionForeground(Drawing.Graphics graphics, TextLayout.SelectedArea[] areas, WidgetPaintState state, PaintTextStyle style, TextFieldDisplayMode mode);
		
		void PaintGeneralTextLayout(Drawing.Graphics graphics, Drawing.Rectangle clipRect, Drawing.Point pos, TextLayout text, WidgetPaintState state, PaintTextStyle style, TextFieldDisplayMode mode, Drawing.Color backColor);

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
		Drawing.Color ColorTextDisplayMode(TextFieldDisplayMode mode);

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
}
