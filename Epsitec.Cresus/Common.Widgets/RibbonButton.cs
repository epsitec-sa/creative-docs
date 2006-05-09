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
			Helpers.VisualPropertyMetadata metadataHeight = new Helpers.VisualPropertyMetadata(RibbonButton.DefaultHeight, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Visual.PreferredHeightProperty.OverrideMetadata(typeof(RibbonButton), metadataHeight);
			Visual.MinHeightProperty.OverrideMetadata(typeof(RibbonButton), metadataHeight);
		}

		
		protected override void OnTextChanged()
		{
			//	Appelé lorsque le texte du bouton change.
			base.OnTextChanged();
			
			this.mainTextSize = this.TextLayout.SingleLineSize;
			this.mainTextSize.Width  = System.Math.Ceiling(this.mainTextSize.Width);
			this.mainTextSize.Height = System.Math.Ceiling(this.mainTextSize.Height);

			Size required = this.RequiredSize;
			required.Height = System.Math.Max(required.Height, this.MinHeight);
			this.MinSize = required;
			this.PreferredWidth = required.Width;
		}

		public Size RequiredSize
		{
			//	Retourne les dimensions requises en fonction du contenu.
			get
			{
				double dx = this.marginHeader*2 + this.mainTextSize.Width;
				double dy = this.mainTextSize.Height;
				return new Size(dx, dy);
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

			if (this.ActiveState != ActiveState.Yes)
			{
				rect.Bottom += 1;
			}

			//?adorner.PaintRibbonButtonBackground(graphics, rect, state);
			rect.Deflate(0.5);
			graphics.AddRectangle(rect);
			graphics.RenderSolid(Color.FromRgb(1,0,0));
			rect.Inflate(0.5);

			adorner.PaintRibbonButtonTextLayout(graphics, rect, this.TextLayout, state, this.ActiveState);
		}


		public static readonly double	DefaultHeight = 25+1;

		protected double				marginHeader = 6;
		protected Size					mainTextSize;
	}
}
