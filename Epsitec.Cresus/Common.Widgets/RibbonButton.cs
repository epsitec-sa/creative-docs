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

			this.ContentAlignment = ContentAlignment.MiddleLeft;
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


		static RibbonButton()
		{
			Helpers.VisualPropertyMetadata metadataHeight = new Helpers.VisualPropertyMetadata(25, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Visual.PreferredHeightProperty.OverrideMetadata(typeof(RibbonSection), metadataHeight);
		}

		
		public override Margins GetShapeMargins()
		{
			return Epsitec.Common.Widgets.Adorners.Factory.Active.GeometryRibbonShapeMargins;
		}


		protected override void OnTextChanged()
		{
			base.OnTextChanged();
			
			this.mainTextSize = this.TextLayout.SingleLineSize;
			this.AdjustSize(ref this.mainTextSize);
		}

		protected void AdjustSize(ref Size size)
		{
			//	Ajuste des dimensions d'un TextLayout.
			size.Width  = System.Math.Ceiling(size.Width);
			size.Height = System.Math.Ceiling(size.Height);
		}

		public Size RequiredSize
		{
			//	Retourne les dimensions requises en fonction du contenu.
			get
			{
				Size size = new Size(0, 0);
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

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine la case.
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			WidgetPaintState state = this.PaintState;
			Point pos = new Point();

			adorner.PaintRibbonButtonBackground(graphics, rect, state);

			pos.X = (rect.Width-this.mainTextSize.Width)/2;
			pos.Y = (rect.Height-this.mainTextSize.Height)/2;
			adorner.PaintRibbonButtonTextLayout(graphics, pos, this.TextLayout, state);
		}


		protected double			marginHeader = 6;
		protected Size				mainTextSize;
	}
}
