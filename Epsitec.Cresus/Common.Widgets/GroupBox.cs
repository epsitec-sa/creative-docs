namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// Summary description for GroupBox.
	/// </summary>
	public class GroupBox : AbstractGroup
	{
		public GroupBox()
		{
		}
		
		public GroupBox(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		
		// Retourne l'alignement par d�faut d'un bouton.
		public override Drawing.ContentAlignment DefaultAlignment
		{
			get
			{
				return Drawing.ContentAlignment.TopLeft;
			}
		}
		
		protected override void AdjustDockBounds(ref Drawing.Rectangle bounds)
		{
			base.AdjustDockBounds (ref bounds);
			
			//	TODO: augmenter les marges gauche, droite, haute et basse pour que le
			//	contenu n'empi�te pas sur le titre, ni sur le cadre...
		}

		public override Drawing.Rectangle GetShapeBounds()
		{
			Drawing.Rectangle rect = base.GetShapeBounds();
			rect.Inflate(Widgets.Adorner.Factory.Active.GeometryGroupShapeBounds);
			return rect;
		}

		// Dessine le texte.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;

			Drawing.Rectangle titleRect = this.TextLayout.StandardRectangle;
			Drawing.Point pos = new Drawing.Point(10, 0);
			titleRect.Offset(pos);
			titleRect.Inflate(3, 0);
			Drawing.Rectangle frameRect = new Drawing.Rectangle();
			frameRect = rect;
			frameRect.Top -= System.Math.Floor(frameRect.Height-(titleRect.Bottom+titleRect.Top)/2);

			adorner.PaintGroupBox(graphics, frameRect, titleRect, state);
			adorner.PaintGeneralTextLayout(graphics, pos, this.TextLayout, state, PaintTextStyle.Group, this.BackColor);
		}
	}
}
