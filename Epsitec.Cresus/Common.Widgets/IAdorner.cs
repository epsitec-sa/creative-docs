namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// L'interface IAdorner donne acc�s aux routines de dessin des divers
	/// �l�ments de l'interface graphique.
	/// </summary>
	public interface IAdorner
	{
		void PaintButtonBackground(Drawing.Rectangle rect, Drawing.Graphics graphics, ButtonStyle style, WidgetState state);
		void PaintButtonForeground(Drawing.Rectangle rect, Drawing.Graphics graphics, ButtonStyle style, WidgetState state);
	}
}
