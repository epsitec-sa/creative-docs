namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe WindowRoot implémente le fond de chaque fenêtre. L'utilisateur obtient
	/// en général une instance de WindowRoot en appeland WindowFrame.Root.
	/// </summary>
	[Support.SuppressBundleSupport]
	public class WindowRoot : AbstractGroup
	{
		public WindowRoot(WindowFrame frame)
		{
			this.window_frame = frame;
		}
		
		public override bool			IsVisible
		{
			get { return true; }
		}
		
		public override WindowFrame		WindowFrame
		{
			get { return this.window_frame; }
		}
		
		
		protected override bool ShortcutHandler(Shortcut shortcut, bool execute_focused)
		{
			if (base.ShortcutHandler (shortcut, execute_focused) == false)
			{
				//	Le raccourci clavier n'a pas été consommé. Il faut voir si le raccourci clavier
				//	est attaché à une commande globale.
				
				//	TODO: gère les commandes globales
				
				return false;
			}
			
			return true;
		}
		
		public override void Invalidate()
		{
			if (this.window_frame != null)
			{
				System.Diagnostics.Debug.Assert (this.parent == null);
				this.window_frame.MarkForRepaint (this.Bounds);
			}
		}
		
		public override void Invalidate(Drawing.Rectangle rect)
		{
			System.Diagnostics.Debug.Assert (this.parent == null);
			this.window_frame.MarkForRepaint (rect);
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			double dx = this.Client.Width;
			double dy = this.Client.Height;
			
			double x1 = System.Math.Max (clip_rect.Left, 0);
			double y1 = System.Math.Max (clip_rect.Bottom, 0);
			double x2 = System.Math.Min (clip_rect.Right, dx);
			double y2 = System.Math.Min (clip_rect.Top, dy);
			
			if (this.BackColor.A != 1.0)
			{
				graphics.Pixmap.Erase (new System.Drawing.Rectangle ((int) x1, (int) y1, (int) x2 - (int) x1, (int) y2 - (int) y1));
			}
			if (this.BackColor.A > 0.0)
			{
#if false
				graphics.SolidRenderer.Color = this.BackColor;
				graphics.AddFilledRectangle (x1, y1, x2-x1, y2-y1);
				graphics.RenderSolid ();
#else
				IAdorner adorner = Widgets.Adorner.Factory.Active;
				Drawing.Rectangle rect = new Drawing.Rectangle(x1, y1, x2-x1, y2-y1);
				adorner.PaintWindowBackground(graphics, rect, WidgetState.None, Direction.None);
#endif
			}
		}
		
		
		protected WindowFrame				window_frame;
	}
}
