namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe WindowRoot implémente le fond de chaque fenêtre. L'utilisateur obtient
	/// en général une instance de WindowRoot en appelant Window.Root.
	/// </summary>
	[Support.SuppressBundleSupport]
	public class WindowRoot : AbstractGroup
	{
		public WindowRoot(Window window)
		{
			this.window    = window;
			this.BackColor = Drawing.Color.Empty;
		}
		
		public override bool			IsVisible
		{
			get { return true; }
		}
		
		public override Window			Window
		{
			get { return this.window; }
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.window = null;
			}
			
			base.Dispose (disposing);
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
			System.Diagnostics.Debug.Assert (this.parent == null);
			
			if (this.window != null)
			{
				this.window.MarkForRepaint (this.Bounds);
			}
		}
		
		public override void Invalidate(Drawing.Rectangle rect)
		{
			System.Diagnostics.Debug.Assert (this.parent == null);
			
			if (this.window != null)
			{
				this.window.MarkForRepaint (this.MapClientToParent (rect));
			}
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			double dx = this.Client.Width;
			double dy = this.Client.Height;
			
			double x1 = System.Math.Max (clip_rect.Left, 0);
			double y1 = System.Math.Max (clip_rect.Bottom, 0);
			double x2 = System.Math.Min (clip_rect.Right, dx);
			double y2 = System.Math.Min (clip_rect.Top, dy);
			
			if (this.BackColor.IsValid)
			{
				if (this.BackColor.A != 1.0)
				{
					graphics.Pixmap.Erase (new System.Drawing.Rectangle ((int) x1, (int) y1, (int) x2 - (int) x1, (int) y2 - (int) y1));
				}
				if (this.BackColor.A > 0.0)
				{
					graphics.SolidRenderer.Color = this.BackColor;
					graphics.AddFilledRectangle (x1, y1, x2-x1, y2-y1);
					graphics.RenderSolid ();
				}
			}
			else
			{
				IAdorner adorner = Widgets.Adorner.Factory.Active;
				Drawing.Rectangle rect = new Drawing.Rectangle(x1, y1, x2-x1, y2-y1);
				adorner.PaintWindowBackground(graphics, this.Client.Bounds, rect, WidgetState.None);
			}
		}
		
		internal void NotifyAdornerChanged()
		{
			this.HandleAdornerChanged ();
		}
		
		protected Window					window;
	}
}
