namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'interface IAdorner donne accès aux routines de dessin des divers
	/// éléments de l'interface graphique.
	/// </summary>
	public interface IAdorner
	{
		void PaintArrow(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, Direction dir);
		void PaintCheck(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow);
		void PaintRadio(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow);
		void PaintIcon(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, string icon);
		
		void PaintButtonBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, ButtonStyle style);
		void PaintButtonForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow, ButtonStyle style);
		
		void PaintTabSunkenBackground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow);
		void PaintTabSunkenForeground(Drawing.Graphics graphics, Drawing.Rectangle rect, WidgetState state, Direction shadow);
		
		void PaintTabFrameBackground(Drawing.Graphics graphics, Drawing.Rectangle frame_rect, Drawing.Rectangle tab_rect, WidgetState state, Direction shadow);
		void PaintTabFrameForeground(Drawing.Graphics graphics, Drawing.Rectangle frame_rect, Drawing.Rectangle tab_rect, WidgetState state, Direction shadow);
	}
	
	public enum Direction
	{
		None, Left, Right, Up, Down
	}
}
