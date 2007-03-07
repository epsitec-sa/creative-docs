using Epsitec.Common.Drawing;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.GlyphGroup))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// GlyphGroup est un groupe tr�s sp�cial, dont le premier enfant doit �tre un GlyphButton.
	/// Les enfants ne doivent d�finir aucun Docking. Ce groupe est utile pour associer un bouton
	/// "combo" � un petit group de boutons. Exemple typique: les boutons undo/redo/v.
	/// </summary>
	public class GlyphGroup : AbstractGroup
	{
		public GlyphGroup()
		{
		}
		
		public GlyphGroup(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		
		static GlyphGroup()
		{
			Types.DependencyPropertyMetadata metadata = Visual.ContentAlignmentProperty.DefaultMetadata.Clone ();

			metadata.DefineDefaultValue (ContentAlignment.TopLeft);

			Visual.ContentAlignmentProperty.OverrideMetadata (typeof (GlyphGroup), metadata);
		}


		protected override void UpdateClientGeometry()
		{
			//	Positionne les enfants d'une fa�on tr�s sp�ciale, en partant de l'id�e que le premier enfant
			//	est un GlyphButton dont la propri�t� ContentAlignment d�termine l'empilement des autres enfants.
			base.UpdateClientGeometry();

			if (this.Children.Count == 0)
			{
				return;
			}

			GlyphButton button = this.Children[0] as GlyphButton;
			if (button == null)
			{
				return;
			}

			Rectangle bounds = this.Client.Bounds;
			button.SetManualBounds(bounds);  // le GlyphButton occupe toute la surface

			if (button.ContentAlignment == ContentAlignment.MiddleRight ||
				button.ContentAlignment == ContentAlignment.BottomRight ||
				button.ContentAlignment == ContentAlignment.TopRight    )
			{
				//	Enfants de gauche � droite.
				Point pos = bounds.BottomLeft;
				for (int i=1; i<this.Children.Count; i++)
				{
					Widget children = this.Children[i] as Widget;

					Rectangle rect = new Rectangle(pos.X, pos.Y, children.ActualSize.Width, bounds.Height);
					children.SetManualBounds(rect);
					pos.X += children.ActualSize.Width;
				}
			}

			if (button.ContentAlignment == ContentAlignment.MiddleLeft ||
				button.ContentAlignment == ContentAlignment.BottomLeft ||
				button.ContentAlignment == ContentAlignment.TopLeft    )
			{
				//	Enfants de droite � gauche.
				Point pos = bounds.BottomRight;
				for (int i=1; i<this.Children.Count; i++)
				{
					Widget children = this.Children[i] as Widget;

					pos.X -= children.ActualSize.Width;
					Rectangle rect = new Rectangle(pos.X, pos.Y, children.ActualSize.Width, bounds.Height);
					children.SetManualBounds(rect);
				}
			}

			if (button.ContentAlignment == ContentAlignment.TopCenter ||
				button.ContentAlignment == ContentAlignment.TopLeft   ||
				button.ContentAlignment == ContentAlignment.TopRight  )
			{
				//	Enfants de bas en haut.
				Point pos = bounds.BottomLeft;
				for (int i=1; i<this.Children.Count; i++)
				{
					Widget children = this.Children[i] as Widget;

					Rectangle rect = new Rectangle(pos.X, pos.Y, bounds.Width, children.ActualSize.Height);
					children.SetManualBounds(rect);
					pos.Y += children.ActualSize.Height;
				}
			}

			if (button.ContentAlignment == ContentAlignment.BottomCenter ||
				button.ContentAlignment == ContentAlignment.BottomLeft   ||
				button.ContentAlignment == ContentAlignment.BottomRight  )
			{
				//	Enfants de haut en bas.
				Point pos = bounds.TopLeft;
				for (int i=1; i<this.Children.Count; i++)
				{
					Widget children = this.Children[i] as Widget;

					pos.Y -= children.ActualSize.Height;
					Rectangle rect = new Rectangle(pos.X, pos.Y, bounds.Width, children.ActualSize.Height);
					children.SetManualBounds(rect);
				}
			}
		}
	}
}
