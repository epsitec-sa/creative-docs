using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Boîte pour représenter une entité.
	/// </summary>
	public class EntityBox : Widget
	{
		public enum ConnectionAnchor
		{
			Left,
			Right,
			Bottom,
			Top,
		}


		public EntityBox() : base()
		{
			this.AutoEngage = true;
			this.InternalState |= InternalState.Engageable;

#if true // provisoire
			this.fields = new List<string>();
#endif

			this.isExtended = false;

			Widget header = new Widget(this);
			header.Margins = new Margins(8, 8, 8, 8+EntityBox.headerHeight-28);
			header.Dock = DockStyle.Top;

			this.staticTitle = new StaticText(header);
			this.staticTitle.ContentAlignment = ContentAlignment.MiddleCenter;
			this.staticTitle.Dock = DockStyle.Fill;

			this.extendButton = new GlyphButton(header);
			this.extendButton.Margins = new Margins(0, 0, 2, 2);
			this.extendButton.Dock = DockStyle.Right;
			this.extendButton.Clicked += new MessageEventHandler(HandleExtendButtonClicked);

			this.UpdateTable();
			this.UpdateExtendButton();
		}

		public EntityBox(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.extendButton.Clicked -= new MessageEventHandler(HandleExtendButtonClicked);
			}
			
			base.Dispose(disposing);
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
					this.staticTitle.Text = string.Concat("<font size=\"120%\"><b>", this.title, "</b></font>");
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

			this.UpdateTable();
			this.UpdateExtendButton();
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
					this.Invalidate();
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
					this.UpdateSeparators();
					this.Invalidate();
				}
			}
		}


		public double GetBestHeight()
		{
			//	Retourne la hauteur requise selon le nombre de champs définis.
			if (this.isExtended)
			{
				return EntityBox.headerHeight + (EntityBox.fieldHeight+1)*this.fields.Count + EntityBox.footerHeight + 15;
			}
			else
			{
				return EntityBox.headerHeight + 6;
			}
		}

		public double GetConnectionVerticalPosition(int rank)
		{
			//	Retourne la position verticale pour un trait de liaison.
			if (this.isExtended && this.table != null && rank < this.fields.Count)
			{
				return this.Client.Bounds.Top - (EntityBox.headerHeight + 8 + (EntityBox.fieldHeight+1)*rank + EntityBox.fieldHeight/2);
			}
			else
			{
				return this.Client.Bounds.Center.Y;
			}
		}

		public Point GetConnectionDestination(double posv, ConnectionAnchor anchor)
		{
			//	Retourne la position où accrocher la destination.
			Rectangle bounds = this.ActualBounds;

			switch (anchor)
			{
				case ConnectionAnchor.Left:
					if (posv >= bounds.Bottom+EntityBox.roundRectRadius && posv <= bounds.Top-EntityBox.roundRectRadius)
					{
						return new Point(bounds.Left, posv);
					}
					else
					{
						return new Point(bounds.Left, bounds.Center.Y);
					}

				case ConnectionAnchor.Right:
					if (posv >= bounds.Bottom+EntityBox.roundRectRadius && posv <= bounds.Top-EntityBox.roundRectRadius)
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


		protected void UpdateExtendButton()
		{
			//	Met à jour le bouton pour étendre on compacter.
			this.extendButton.GlyphShape = this.isExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;

			foreach (StaticText st in this.table)
			{
				st.Visibility = this.isExtended;
			}
		}

		protected void UpdateTable()
		{
			//	Met à jour la table en fonction des champs.
			this.table = new List<StaticText>();

			for (int i=0; i<fields.Count; i++)
			{
				string field = this.fields[i];

				StaticText container = new StaticText(this);
				container.PreferredHeight = EntityBox.fieldHeight;
				container.Margins = new Margins(2, 2, 0, 0);
				container.Dock = DockStyle.Top;
				this.table.Add(container);

				StaticText st = new StaticText(container);
				st.Text = field;
				st.ContentAlignment = ContentAlignment.MiddleLeft;
				st.Margins = new Margins(20, 20, 0, 0);
				st.Dock = DockStyle.Fill;

				StaticText sep = new StaticText(this);
				sep.Name = "Separator";
				sep.BackColor = Color.FromBrightness(0.9);
				sep.PreferredHeight = 1;
				sep.Margins = new Margins(2, 2, 0, 0);
				sep.Dock = DockStyle.Top;
				this.table.Add(sep);
			}
		}

		protected void UpdateSeparators()
		{
			//	Met à jour la couleur des séparateurs.
			Color color = Color.FromBrightness(0.9);

			if (this.isHilited)
			{
				IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
				color = adorner.ColorCaption;
				color.A = 0.3;
			}

			foreach (StaticText st in this.table)
			{
				if (st.Name == "Separator")
				{
					st.BackColor = color;
				}
			}
		}


		public override Margins GetShapeMargins()
		{
			//	Retourne les marges supplémentaires à droite et en bas, pour dessiner l'ombre.
			return new Margins(0, EntityBox.shadowOffset, 0, EntityBox.shadowOffset);
		}

		public bool Hilite(Point pos)
		{
			//	Met en évidence la boîte selon la position de la souris.
			//	Si la souris est dans cette boîte, retourne true.
			if (!pos.IsZero)
			{
				pos = this.MapParentToClient(pos);
			}

			this.HiliteBox(pos);
			this.HiliteField(pos);

			if (pos.IsZero)
			{
				return false;
			}
			else
			{
				return this.Client.Bounds.Contains(pos);
			}
		}

		protected void HiliteBox(Point pos)
		{
			//	Met en évidence la boîte, selon la position de la souris.
			//	Si la boîte est survolée, on peut la déplacer globalement.
			if (pos.IsZero || !this.Client.Bounds.Contains(pos))
			{
				this.IsHilited = false;
			}
			else
			{
				//	Retourne true si on est dans l'en-tête ou le pied.
				this.IsHilited = (pos.Y >= this.Client.Bounds.Top-EntityBox.headerHeight-4 ||
								  pos.Y <= this.Client.Bounds.Bottom+EntityBox.footerHeight);
			}
		}

		protected void HiliteField(Point pos)
		{
			//	Colore le champ visé par la souris.
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
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			//	Dessine l'ombre.
			Rectangle bounds = this.Client.Bounds;
			bounds.Offset(EntityBox.shadowOffset, -(EntityBox.shadowOffset));
			this.PaintShadow(graphics, bounds, EntityBox.roundRectRadius+EntityBox.shadowOffset, (int)EntityBox.shadowOffset, 0.2);

			//	Construit le chemin du cadre arrondi.
			bounds = this.Client.Bounds;
			bounds.Deflate(1);
			Path path = this.PathRoundRectangle(bounds, EntityBox.roundRectRadius);

			//	Peint l'intérieur en blanc.
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(Color.FromBrightness(1));

			//	Peint l'intérieur en dégradé.
			graphics.Rasterizer.AddSurface(path);
			Color c1 = adorner.ColorCaption;
			Color c2 = adorner.ColorCaption;
			c1.A = this.IsHilited ? 0.6 : 0.4;
			c2.A = this.IsHilited ? 0.2 : 0.1;
			this.RenderHorizontalGradient(graphics, bounds, c1, c2);

			//	Peint en blanc la zone pour les champs.
			if (this.isExtended)
			{
				Rectangle inside = new Rectangle(bounds.Left, bounds.Bottom+EntityBox.footerHeight, bounds.Width, bounds.Height-EntityBox.footerHeight-EntityBox.headerHeight);
				graphics.AddFilledRectangle(inside);
				graphics.RenderSolid(Color.FromBrightness(1));
				graphics.AddFilledRectangle(inside);
				Color ci1 = adorner.ColorCaption;
				Color ci2 = adorner.ColorCaption;
				ci1.A = this.IsHilited ? 0.2 : 0.1;
				ci2.A = 0.0;
				this.RenderHorizontalGradient(graphics, inside, ci1, ci2);

				Rectangle shadow = new Rectangle(bounds.Left, bounds.Top-EntityBox.headerHeight-8, bounds.Width, 8);
				graphics.AddFilledRectangle(shadow);
				this.RenderVerticalGradient(graphics, shadow, Color.FromAlphaRgb(0.0, 0, 0, 0), Color.FromAlphaRgb(0.3, 0, 0, 0));
			}

			//	Peint le cadre en noir.
			graphics.Rasterizer.AddOutline(path, 2);
			if (this.isExtended)
			{
				graphics.AddLine(bounds.Left, bounds.Top-EntityBox.headerHeight-0.5, bounds.Right, bounds.Top-EntityBox.headerHeight-0.5);
				graphics.AddLine(bounds.Left+2, bounds.Bottom+EntityBox.footerHeight+0.5, bounds.Right-2, bounds.Bottom+EntityBox.footerHeight+0.5);
			}
			graphics.RenderSolid(this.IsHilited ? adorner.ColorCaption : Color.FromBrightness(0));
		}


		protected void PaintShadow(Graphics graphics, Rectangle rect, double radius, int smooth, double alpha)
		{
			//	Dessine une ombre douce.
			alpha /= smooth;

			for (int i=0; i<smooth; i++)
			{
				Path path = this.PathRoundRectangle(rect, radius);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(Color.FromAlphaRgb(alpha, 0, 0, 0));

				rect.Deflate(1);
				radius -= 1;
			}
		}

		protected void RenderHorizontalGradient(Graphics graphics, Rectangle rect, Color leftColor, Color rightColor)
		{
			//	Peint la surface avec un dégradé horizontal.
			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.X;
			graphics.GradientRenderer.SetColors(leftColor, rightColor);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = new Transform();
			Point center = rect.Center;
			t.Scale(rect.Width/100/2, rect.Height/100/2);
			t.Translate(center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		protected void RenderVerticalGradient(Graphics graphics, Rectangle rect, Color bottomColor, Color topColor)
		{
			//	Peint la surface avec un dégradé vertical.
			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.Y;
			graphics.GradientRenderer.SetColors(bottomColor, topColor);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = new Transform();
			Point center = rect.Center;
			t.Scale(rect.Width/100/2, rect.Height/100/2);
			t.Translate(center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		protected Path PathRoundRectangle(Rectangle rect, double radius)
		{
			//	Retourne le chemin d'un rectangle à coins arrondis.
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			radius = System.Math.Min(radius, System.Math.Min(dx, dy)/2);

			Path path = new Path();
			path.MoveTo (ox+radius, oy);
			path.LineTo (ox+dx-radius, oy);
			path.CurveTo(ox+dx, oy, ox+dx, oy+radius);
			path.LineTo (ox+dx, oy+dy-radius);
			path.CurveTo(ox+dx, oy+dy, ox+dx-radius, oy+dy);
			path.LineTo (ox+radius, oy+dy);
			path.CurveTo(ox, oy+dy, ox, oy+dy-radius);
			path.LineTo (ox, oy+radius);
			path.CurveTo(ox, oy, ox+radius, oy);
			path.Close();

			return path;
		}


		private void HandleExtendButtonClicked(object sender, MessageEventArgs e)
		{
			this.IsExtended = !this.IsExtended;
			this.UpdateExtendButton();
			this.OnGeometryChanged();
		}


		#region Event
		protected virtual void OnGeometryChanged()
		{
			//	Génère un événement pour dire que la géométrie a changé (changement compact/étendu).
			EventHandler handler = (EventHandler) this.GetUserEventHandler("GeometryChanged");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event Support.EventHandler GeometryChanged
		{
			add
			{
				this.AddUserEventHandler("GeometryChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler("GeometryChanged", value);
			}
		}
		#endregion


		protected static readonly double roundRectRadius = 16;
		protected static readonly double shadowOffset = 6;
		protected static readonly double headerHeight = 32;
		protected static readonly double footerHeight = 10;
		protected static readonly double fieldHeight = 20;

		protected bool isExtended;
		protected bool isHilited;
		protected string title;
		protected StaticText staticTitle;
		protected GlyphButton extendButton;
		protected List<string> fields;
		protected List<StaticText> table;
	}
}
