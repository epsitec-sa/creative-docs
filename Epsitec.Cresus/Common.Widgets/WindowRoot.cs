namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe WindowRoot impl�mente le fond de chaque fen�tre. L'utilisateur obtient
	/// en g�n�ral une instance de WindowRoot en appeland WindowFrame.Root.
	/// </summary>
	public class WindowRoot : Widget
	{
		public WindowRoot(WindowFrame frame)
		{
			this.window_frame = frame;
		}
		
		public override bool			IsVisible
		{
			get { return true; }
		}

		
		protected override bool ShortcutHandler(Shortcut shortcut, bool execute_focused)
		{
			if (base.ShortcutHandler (shortcut, execute_focused) == false)
			{
				//	Le raccourci clavier n'a pas �t� consomm�. Il faut voir si le raccourci clavier
				//	est attach� � une commande globale.
				
				//	TODO: g�re les commandes globales
				
				return false;
			}
			
			return true;
		}
		
		public override void Invalidate()
		{
			System.Diagnostics.Debug.Assert (this.parent == null);
			this.window_frame.MarkForRepaint (this.Bounds);
		}
		
		public override void Invalidate(Drawing.Rectangle rect)
		{
			System.Diagnostics.Debug.Assert (this.parent == null);
			this.window_frame.MarkForRepaint (rect);
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics)
		{
			Drawing.Path path = new Drawing.Path ();
			
			double dx = this.Client.Width;
			double dy = this.Client.Height;
			
			path.MoveTo (0, 0);
			path.LineTo (dx, 0);
			path.LineTo (dx, dy);
			path.LineTo (0, dy);
			path.Close ();
			
			graphics.Solid.Color = System.Drawing.Color.LightSalmon;
			graphics.Rasterizer.AddSurface (path);
			graphics.RenderSolid ();
		}
		
		
		protected WindowFrame				window_frame;
	}
}
