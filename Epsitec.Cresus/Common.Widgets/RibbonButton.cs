using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe RibbonButton représente un bouton pour sélectionner un ruban.
	/// </summary>
	public class RibbonButton : AbstractButton
	{
		public RibbonButton()
		{
			this.AutoCapture = false;
			this.AutoFocus   = false;
			this.AutoEngage  = false;
			
			this.InternalState &= ~InternalState.Focusable;
			this.InternalState &= ~InternalState.Engageable;

			this.Alignment = Drawing.ContentAlignment.MiddleLeft;
		}
		
		public RibbonButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public RibbonButton(string command, string text) : this()
		{
			this.Command  = command;
			this.Text     = text;
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		
		#region Serialization support
		protected override bool ShouldSerializeLocation()
		{
			return false;
		}
		
		protected override bool ShouldSerializeSize()
		{
			return false;
		}
		#endregion
		
		
		public override Drawing.Rectangle GetShapeBounds()
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Inflate(adorner.GeometryRibbonShapeBounds);
			return rect;
		}


		protected override void OnTextChanged()
		{
			base.OnTextChanged();
			
			this.mainTextSize = this.TextLayout.SingleLineSize;
			this.AdjustSize(ref this.mainTextSize);
		}

		protected void AdjustSize(ref Drawing.Size size)
		{
			//	Ajuste des dimensions d'un TextLayout.
			size.Width  = System.Math.Ceiling(size.Width);
			size.Height = System.Math.Ceiling(size.Height);
		}

		public Drawing.Size RequiredSize
		{
			//	Retourne les dimensions requises en fonction du contenu.
			get
			{
				Drawing.Size size = new Drawing.Size(0, 0);
				size.Width = this.marginHeader*2 + this.mainTextSize.Width;
				size.Height = this.mainTextSize.Height;
				return size;
			}
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie de la case du menu.
			base.UpdateClientGeometry();

			if ( this.TextLayout != null )  this.TextLayout.LayoutSize = this.mainTextSize;
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			//	Dessine la case.
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect  = this.Client.Bounds;
			WidgetState       state = this.PaintState;
			Drawing.Point     pos   = new Drawing.Point();

			adorner.PaintRibbonButtonBackground(graphics, rect, state);

			pos.X = (rect.Width-this.mainTextSize.Width)/2;
			pos.Y = (rect.Height-this.mainTextSize.Height)/2;
			adorner.PaintRibbonButtonTextLayout(graphics, pos, this.TextLayout, state);
		}


		protected double			marginHeader = 6;
		protected Drawing.Size		mainTextSize;
	}
}
