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

		// Retourne l'alignement par défaut d'un bouton.
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
			//	contenu n'empiète pas sur le titre, ni sur le cadre...
		}

		// Dessine le texte.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			Direction         dir   = this.RootDirection;

			Drawing.Rectangle titleRect = this.textLayout.StandardRectangle;
			Drawing.Point pos = new Drawing.Point(10, 0);
			titleRect.Offset(pos);
			titleRect.Inflate(3, 0);
			Drawing.Rectangle frameRect = new Drawing.Rectangle();
			frameRect = rect;
			frameRect.Top -= System.Math.Floor(frameRect.Height-(titleRect.Bottom+titleRect.Top)/2);

			adorner.PaintGroupBox(graphics, frameRect, titleRect, state, dir);
			adorner.PaintGeneralTextLayout(graphics, pos, this.textLayout, state, dir);
		}
	}
}
