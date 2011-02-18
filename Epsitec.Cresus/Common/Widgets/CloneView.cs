using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe <c>CloneView</c> implémente un widget qui prend l'aspect d'un autre.
	/// </summary>
	public class CloneView : Widget
	{
		public CloneView()
		{
		}
		
		public CloneView(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		/// <summary>
		/// Détermine le widget dont on prend l'aspect.
		/// </summary>
		/// <value>Le widget dont on prend l'aspect.</value>
		public Widget Model
		{
			get
			{
				return this.model;
			}
			set
			{
				this.model = value;
			}
		}


		public override void PaintHandler(Graphics graphics, Rectangle repaint, IPaintFilter paintFilter)
		{
			if (this.model == null)
			{
				base.PaintHandler(graphics, repaint, paintFilter);
			}
			else
			{
				Drawing.Size displaySize = this.ActualSize;
				Drawing.Point modelPos = this.model.ActualLocation;
				Drawing.Size modelSize = this.model.ActualSize;
				
				if (modelSize != displaySize)
				{
					//	Le modèle n'a pas la bonne taille. Il va falloir changer
					//	temporairement sa taille.
					
					//	Attache le contexte de layout directement au modèle; de
					//	cette manière le modèle sera considéré comme la base de
					//	l'arbre des widgets et son conteneur ne sera pas affecté
					//	par le changement de taille :
					
					Layouts.LayoutContext context = Helpers.VisualTree.GetLayoutContext (this.model);
					Layouts.LayoutContext.SetLayoutContext (this.model, context);

					//	Modifie (brutalement) les dimensions du modèle, sans pour
					//	autant le déplacer, puis force un "arrange" de son contenu
					//	sans toucher au reste :
					
					this.model.SetBounds (new Drawing.Rectangle (modelPos, displaySize));
					Layouts.LayoutContext.AddToArrangeQueue (this.model);
					context.ExecuteArrange ();
					
					//	Peint normalement le modèle dans notre propre vue :
					
					this.PaintModelView (graphics, repaint, paintFilter);

					//	Remet la taille initiale du modèle et refait ensuite le
					//	layout de son contenu. Une fois de plus, le conteneur n'y
					//	aura vu que du feu :
					
					this.model.SetBounds (new Drawing.Rectangle (modelPos, modelSize));
					Layouts.LayoutContext.AddToArrangeQueue (this.model);
					context.ExecuteArrange ();

					//	Finalement, détache le contexte de layout du modèle pour
					//	qu'il réintègre sa position normale dans l'arbre de layout :

					Layouts.LayoutContext.ClearLayoutContext (this.model);
				}
				else
				{
					this.PaintModelView (graphics, repaint, paintFilter);
				}
			}
		}

		private void PaintModelView(Graphics graphics, Rectangle repaint, IPaintFilter paintFilter)
		{
			Drawing.Point offset = this.model.ActualLocation - this.ActualLocation;
			Drawing.Point pos = this.MapClientToRoot (Drawing.Point.Zero);
			Drawing.Point originalClipOffset = graphics.ClipOffset;
			Drawing.Transform originalTransform = graphics.Transform;
			Drawing.Transform graphicsTransform = Drawing.Transform.CreateTranslationTransform (-offset);

			Drawing.Point clipOffset = pos - this.model.MapClientToRoot (this.model.GetClipBounds ()).Location;

			repaint.Offset (offset);

			graphics.Transform = graphicsTransform.MultiplyBy (originalTransform);
			graphics.ClipOffset = clipOffset;

			this.model.PaintHandler (graphics, repaint, paintFilter);

			graphics.Transform = originalTransform;
			graphics.ClipOffset = originalClipOffset;
		}


		protected Widget				model;
	}
}
