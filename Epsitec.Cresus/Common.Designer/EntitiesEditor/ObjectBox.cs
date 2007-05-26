using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.EntitiesEditor
{
	/// <summary>
	/// Boîte pour représenter une entité.
	/// </summary>
	public class ObjectBox : AbstractObject
	{
		public enum ConnectionAnchor
		{
			Left,
			Right,
			Bottom,
			Top,
		}


		public ObjectBox(Editor editor) : base(editor)
		{
#if true // provisoire
			this.fields = new List<string>();
#endif

			this.isExtended = false;
		}


		public string Title
		{
			//	Titre au sommet de la boîte.
			get
			{
				return this.title;
			}
			set
			{
				if (this.title != value)
				{
					this.title = value;
				}
			}
		}

		public void SetContent(string content)
		{
			//	Provisoire...
			string[] list = content.Split(';');

			foreach (string text in list)
			{
				this.fields.Add(text);
			}
		}

		public bool IsExtended
		{
			//	Etat de la boîte (compact ou étendu).
			//	En mode compact, seul le titre est visible.
			//	En mode étendu, les champs sont visibles.
			get
			{
				return this.isExtended;
			}
			set
			{
				if (this.isExtended != value)
				{
					this.isExtended = value;
					this.editor.Invalidate();
				}
			}
		}

		public bool IsHilited
		{
			//	Est-ce que la boîte est survolée par la souris ?
			//	Si la boîte est survolée, on peut la déplacer globalement.
			get
			{
				return this.isHilited;
			}
			set
			{
				if (this.isHilited != value)
				{
					this.isHilited = value;
					this.editor.Invalidate();
				}
			}
		}


		public double GetBestHeight()
		{
			//	Retourne la hauteur requise selon le nombre de champs définis.
			if (this.isExtended)
			{
				return ObjectBox.headerHeight + (ObjectBox.fieldHeight+1)*this.fields.Count + ObjectBox.footerHeight + 15;
			}
			else
			{
				return ObjectBox.headerHeight + 6;
			}
		}

		public double GetConnectionVerticalPosition(int rank)
		{
			//	Retourne la position verticale pour un trait de liaison.
			if (this.isExtended && rank < this.fields.Count)
			{
				Rectangle rect = this.GetFieldBounds(rank);
				return rect.Center.Y;
			}
			else
			{
				return this.bounds.Center.Y;
			}
		}

		public Point GetConnectionDestination(double posv, ConnectionAnchor anchor)
		{
			//	Retourne la position où accrocher la destination.
			Rectangle bounds = this.bounds;

			switch (anchor)
			{
				case ConnectionAnchor.Left:
					if (posv >= bounds.Bottom+ObjectBox.roundRectRadius && posv <= bounds.Top-ObjectBox.roundRectRadius)
					{
						return new Point(bounds.Left, posv);
					}
					else
					{
						return new Point(bounds.Left, bounds.Center.Y);
					}

				case ConnectionAnchor.Right:
					if (posv >= bounds.Bottom+ObjectBox.roundRectRadius && posv <= bounds.Top-ObjectBox.roundRectRadius)
					{
						return new Point(bounds.Right, posv);
					}
					else
					{
						return new Point(bounds.Right, bounds.Center.Y);
					}

				case ConnectionAnchor.Bottom:
					return new Point(bounds.Center.X, bounds.Bottom);

				case ConnectionAnchor.Top:
					return new Point(bounds.Center.X, bounds.Top);
			}

			return Point.Zero;
		}

		protected Rectangle GetFieldBounds(int rank)
		{
			//	Retourne le rectangle occupé par un champ.
			Rectangle rect = this.bounds;

			rect.Bottom = rect.Top - ObjectBox.headerHeight - ObjectBox.fieldHeight*(rank+1) - 12;
			rect.Height = ObjectBox.fieldHeight;

			return rect;
		}


		public bool Hilite(Point pos)
		{
			//	Met en évidence la boîte selon la position de la souris.
			//	Si la souris est dans cette boîte, retourne true.
			this.HiliteBox(pos);
			this.HiliteField(pos);

			if (pos.IsZero)
			{
				return false;
			}
			else
			{
				return this.bounds.Contains(pos);
			}
		}

		protected void HiliteBox(Point pos)
		{
			//	Met en évidence la boîte, selon la position de la souris.
			//	Si la boîte est survolée, on peut la déplacer globalement.
			if (pos.IsZero || !this.bounds.Contains(pos))
			{
				this.IsHilited = false;
			}
			else
			{
				//	Retourne true si on est dans l'en-tête ou le pied.
				this.IsHilited = (pos.Y >= this.bounds.Top-ObjectBox.headerHeight-4 ||
								  pos.Y <= this.bounds.Bottom+ObjectBox.footerHeight);
			}
		}

		protected void HiliteField(Point pos)
		{
			//	Colore le champ visé par la souris.
#if false
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Widget finded = null;
			if (!pos.IsZero)
			{
				finded = this.FindChild(pos);
			}

			foreach (StaticText st in this.table)
			{
				if (st.Name != "Separator")
				{
					if (st == finded)
					{
						Color color = adorner.ColorCaption;
						color.A = 0.1;
						st.BackColor = color;
					}
					else
					{
						st.BackColor = Color.Empty;
					}
				}
			}
#endif
		}



		public override void Draw(Graphics graphics)
		{
			//	Dessine l'objet.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			//	Dessine l'ombre.
			Rectangle bounds = this.bounds;
			bounds.Offset(ObjectBox.shadowOffset, -(ObjectBox.shadowOffset));
			this.PaintShadow(graphics, bounds, ObjectBox.roundRectRadius+ObjectBox.shadowOffset, (int)ObjectBox.shadowOffset, 0.2);

			//	Construit le chemin du cadre arrondi.
			bounds = this.bounds;
			bounds.Deflate(1);
			Path path = this.PathRoundRectangle(bounds, ObjectBox.roundRectRadius);

			//	Dessine l'intérieur en blanc.
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(Color.FromBrightness(1));

			//	Dessine l'intérieur en dégradé.
			graphics.Rasterizer.AddSurface(path);
			Color c1 = adorner.ColorCaption;
			Color c2 = adorner.ColorCaption;
			c1.A = this.IsHilited ? 0.6 : 0.4;
			c2.A = this.IsHilited ? 0.2 : 0.1;
			this.RenderHorizontalGradient(graphics, bounds, c1, c2);

			//	Dessine en blanc la zone pour les champs.
			if (this.isExtended)
			{
				Rectangle inside = new Rectangle(bounds.Left, bounds.Bottom+ObjectBox.footerHeight, bounds.Width, bounds.Height-ObjectBox.footerHeight-ObjectBox.headerHeight);
				graphics.AddFilledRectangle(inside);
				graphics.RenderSolid(Color.FromBrightness(1));
				graphics.AddFilledRectangle(inside);
				Color ci1 = adorner.ColorCaption;
				Color ci2 = adorner.ColorCaption;
				ci1.A = this.IsHilited ? 0.2 : 0.1;
				ci2.A = 0.0;
				this.RenderHorizontalGradient(graphics, inside, ci1, ci2);

				Rectangle shadow = new Rectangle(bounds.Left, bounds.Top-ObjectBox.headerHeight-8, bounds.Width, 8);
				graphics.AddFilledRectangle(shadow);
				this.RenderVerticalGradient(graphics, shadow, Color.FromAlphaRgb(0.0, 0, 0, 0), Color.FromAlphaRgb(0.3, 0, 0, 0));
			}

			//	Dessine le titre.
			Font font = Font.GetFont("Tahoma", "Bold");

			graphics.AddText(bounds.Left, bounds.Top-ObjectBox.headerHeight, bounds.Width, ObjectBox.headerHeight, this.title, font, 16, ContentAlignment.MiddleCenter);
			graphics.RenderSolid(Color.FromBrightness(0));

			//	Dessine les noms des champs.
			if (this.isExtended)
			{
				font = Font.GetFont("Tahoma", "Regular");

				Color color = Color.FromBrightness(0.9);
				if (this.isHilited)
				{
					color = adorner.ColorCaption;
					color.A = 0.3;
				}

				for (int i=0; i<this.fields.Count; i++)
				{
					Rectangle rect = this.GetFieldBounds(i);

					graphics.AddText(rect.Left+10, rect.Bottom, rect.Width-20, ObjectBox.fieldHeight, this.fields[i], font, 11, ContentAlignment.MiddleLeft);
					graphics.RenderSolid(Color.FromBrightness(0));

					graphics.AddLine(rect.Left, rect.Bottom+0.5, rect.Right, rect.Bottom+0.5);
					graphics.RenderSolid(color);

					rect.Offset(0, -ObjectBox.fieldHeight);
				}
			}

			//	Dessine le cadre en noir.
			graphics.Rasterizer.AddOutline(path, 2);
			if (this.isExtended)
			{
				graphics.AddLine(bounds.Left, bounds.Top-ObjectBox.headerHeight-0.5, bounds.Right, bounds.Top-ObjectBox.headerHeight-0.5);
				graphics.AddLine(bounds.Left+2, bounds.Bottom+ObjectBox.footerHeight+0.5, bounds.Right-2, bounds.Bottom+ObjectBox.footerHeight+0.5);
			}
			graphics.RenderSolid(this.IsHilited ? adorner.ColorCaption : Color.FromBrightness(0));
		}




		protected static readonly double roundRectRadius = 16;
		protected static readonly double shadowOffset = 6;
		protected static readonly double headerHeight = 32;
		protected static readonly double footerHeight = 10;
		protected static readonly double fieldHeight = 20;

		protected bool isExtended;
		protected bool isHilited;
		protected string title;
		protected List<string> fields;
	}
}
