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
			
			this.InternalState &= ~WidgetInternalState.Focusable;
			this.InternalState &= ~WidgetInternalState.Engageable;

//			this.ContentAlignment = ContentAlignment.MiddleLeft;
			this.ContentAlignment = ContentAlignment.MiddleCenter;
		}
		
		public RibbonButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public RibbonButton(string command, string text) : this()
		{
			this.CommandObject = Command.Get (command);
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
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDy.DefineDefaultValue (RibbonButton.DefaultHeight);
			
			Visual.PreferredHeightProperty.OverrideMetadata(typeof(RibbonButton), metadataDy);
			Visual.MinHeightProperty.OverrideMetadata(typeof(RibbonButton), metadataDy);
		}


		public override Drawing.Margins GetShapeMargins()
		{
			return new Drawing.Margins(5, 5, 0, 0);
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

			WidgetPaintState paintState = this.GetPaintState ();
			adorner.PaintRibbonButtonBackground (graphics, rect, paintState, this.ActiveState);
			adorner.PaintRibbonButtonTextLayout (graphics, rect, this.TextLayout, paintState, this.ActiveState);
		}


		public static readonly double	DefaultHeight = 25+1;

		protected double				marginHeader = 6;
		protected Size					mainTextSize;
	}
}
