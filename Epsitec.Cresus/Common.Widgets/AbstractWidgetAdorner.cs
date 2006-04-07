//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractWidgetAdorner offre les services de base permettant de décorer
	/// un widget au moyen d'une peinture appliquée par-dessus celui-ci.
	/// </summary>
	public abstract class AbstractWidgetAdorner : Window.IPostPaintHandler
	{
		public AbstractWidgetAdorner()
		{
		}
		
		
		public Widget							Widget
		{
			get
			{
				return this.widget;
			}
			
			set
			{
				if (this.widget != value)
				{
					if (this.widget != null)
					{
						this.DetachWidget (this.widget);
					}
					
					this.widget = value;
					
					if (this.widget != null)
					{
						this.AttachWidget (this.widget);
					}
				}
			}
		}
		
		
		protected virtual void AttachWidget(Widget widget)
		{
			widget.PaintForeground += new PaintEventHandler (this.HandleWidgetPaintForeground);
			widget.Invalidate ();
		}
		
		protected virtual void DetachWidget(Widget widget)
		{
			widget.PaintForeground -= new PaintEventHandler (this.HandleWidgetPaintForeground);
			widget.Invalidate ();
		}
		
		
		private void HandleWidgetPaintForeground(object sender, PaintEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.widget == sender);
			
			//	La peinture des "ornements" se fait après-coup, dans une dernière phase
			//	d'affichage, afin d'être sûr qu'aucun widget ne couvre notre dessin.
			
			this.widget.Window.QueuePostPaintHandler (this, e.Graphics, e.ClipRectangle);
		}
		
		
		protected abstract void PaintDecoration(Drawing.Graphics graphics, Drawing.Rectangle repaint);
		
		#region IPostPaintHandler Members
		void Window.IPostPaintHandler.Paint(Drawing.Graphics graphics, Drawing.Rectangle repaint)
		{
			this.PaintDecoration (graphics, repaint);
		}
		#endregion
		
		
		protected Widget						widget;

	}
}
