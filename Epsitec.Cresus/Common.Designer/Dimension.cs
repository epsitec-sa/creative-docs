using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Description d'une cote pour PanelEditor.
	/// </summary>
	public class Dimension
	{
		//	Types possibles pour les différentes cotes.
		public enum Type
		{
			None,
			Width,
			Height,
			MarginLeft,
			MarginRight,
			MarginBottom,
			MarginTop,
			PaddingLeft,
			PaddingRight,
			PaddingBottom,
			PaddingTop,
			GridColumn,
			GridRow,
			GridWidth,
			GridHeight,
			GridMarginLeft,
			GridMarginRight,
			GridMarginBottom,
			GridMarginTop,
		}


		public Dimension(MyWidgets.PanelEditor editor, Widget obj, Type type)
		{
			//	Crée une cote.
			this.editor = editor;
			this.objectModifier = editor.ObjectModifier;
			this.context = editor.Context;

			this.obj = obj;
			this.type = type;
			this.column = -1;
			this.row = -1;
		}

		public Dimension(MyWidgets.PanelEditor editor, Widget obj, Type type, int column, int row)
		{
			//	Crée une cote.
			this.editor = editor;
			this.objectModifier = editor.ObjectModifier;
			this.context = editor.Context;

			this.obj = obj;
			this.type = type;
			this.column = column;
			this.row = row;
		}


		public Type DimensionType
		{
			//	Retourne le type d'une cote.
			get
			{
				return this.type;
			}
		}

		public int ColumnOrRow
		{
			//	Retourne le rang de la ligne ou de la colonne (selon le type).
			get
			{
				if (this.type == Type.GridColumn)
				{
					return this.column;
				}
				else if (this.type == Type.GridRow)
				{
					return this.row;
				}
				else
				{
					return -1;
				}
			}
		}


		public void DrawBackground(Graphics graphics)
		{
			//	Dessine une cote.
			Rectangle bounds = this.objectModifier.GetActualBounds(this.obj);
			Margins margins = this.objectModifier.GetMargins(this.obj);
			Rectangle ext = bounds;
			ext.Inflate(this.objectModifier.GetMargins(this.obj));
			Margins padding = this.objectModifier.GetPadding(this.obj);
			Rectangle inside = this.objectModifier.GetFinalPadding(this.obj);

			Color red    = Color.FromAlphaRgb(0.6, 255.0/255.0, 180.0/255.0, 130.0/255.0);
			Color orange = Color.FromAlphaRgb(0.6, 255.0/255.0, 220.0/255.0, 130.0/255.0);
			Color yellow = Color.FromAlphaRgb(0.6, 255.0/255.0, 255.0/255.0, 130.0/255.0);
			Color border = Color.FromBrightness(0.5);  // gris

			Rectangle r, box;
			Path path;
			Point p, p1, p2;
			double value;

			box = this.TextBox;
			graphics.Align(ref box);
			box.Offset(0.5, 0.5);

			switch (this.type)
			{
				case Type.Width:
					r = box;
					r.Top = ext.Bottom;
					graphics.AddFilledRectangle(r);
					graphics.RenderSolid(red);
					graphics.AddRectangle(r);
					graphics.RenderSolid(border);

					value = this.Value;
					if (value == bounds.Width)  // forme rectangulaire simple ?
					{
						p1 = new Point(box.Right, ext.Bottom);
						p2 = new Point(box.Left, ext.Bottom);
					}
					else
					{
						r = box;
						double half = System.Math.Max(System.Math.Floor(value/2), 5);
						double middle = System.Math.Floor(r.Center.X)+0.5;
						r.Left = middle-half;
						r.Width = half*2;
						half = System.Math.Floor(value/2);
						p1 = new Point(middle-half, ext.Bottom);
						p2 = new Point(p1.X+half*2, ext.Bottom);
						path = new Path();
						path.MoveTo(p1);
						path.LineTo(r.TopLeft);
						path.LineTo(r.BottomLeft);
						path.LineTo(r.BottomRight);
						path.LineTo(r.TopRight);
						path.LineTo(p2);
						path.Close();
						graphics.Rasterizer.AddOutline(path);
						graphics.RenderSolid(border);

						this.DrawSpring(graphics, new Point(box.Left, box.Center.Y), new Point(r.Left, box.Center.Y), border);
						this.DrawSpring(graphics, new Point(box.Right, box.Center.Y), new Point(r.Right, box.Center.Y), border);
					}
					break;

				case Type.Height:
					r = box;
					r.Left = ext.Right;
					graphics.AddFilledRectangle(r);
					graphics.RenderSolid(red);
					graphics.AddRectangle(r);
					graphics.RenderSolid(border);

					value = this.Value;
					if (value == bounds.Height)  // forme rectangulaire simple ?
					{
						p1 = new Point(ext.Right, box.Top);
						p2 = new Point(ext.Right, box.Bottom);
					}
					else
					{
						r = box;
						double half = System.Math.Max(System.Math.Floor(value/2), 5);
						double middle = System.Math.Floor(r.Center.Y)+0.5;
						r.Bottom = middle-half;
						r.Height = half*2;
						half = System.Math.Floor(value/2);
						p1 = new Point(ext.Right, middle-half);
						p2 = new Point(ext.Right, p1.Y+half*2);
						path = new Path();
						path.MoveTo(p1);
						path.LineTo(r.BottomLeft);
						path.LineTo(r.BottomRight);
						path.LineTo(r.TopRight);
						path.LineTo(r.TopLeft);
						path.LineTo(p2);
						path.Close();
						graphics.Rasterizer.AddOutline(path);
						graphics.RenderSolid(border);

						this.DrawSpring(graphics, new Point(box.Center.X, box.Bottom), new Point(box.Center.X, r.Bottom), border);
						this.DrawSpring(graphics, new Point(box.Center.X, box.Top), new Point(box.Center.X, r.Top), border);
					}
					break;

				case Type.MarginLeft:
					path = new Path();
					p = new Point(box.Right, ext.Bottom);
					path.MoveTo(p);
					p.Y = box.Bottom;
					path.LineTo(p);
					p.X -= box.Width;
					path.LineTo(p);
					p.Y += box.Height;
					path.LineTo(p);
					p.Y = ext.Bottom;
					p.X = box.Right-margins.Left;
					path.LineTo(p);
					path.Close();
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(orange);
					graphics.Rasterizer.AddOutline(path);
					graphics.RenderSolid(border);
					break;

				case Type.MarginRight:
					path = new Path();
					p = new Point(box.Left, ext.Bottom);
					path.MoveTo(p);
					p.Y = box.Bottom;
					path.LineTo(p);
					p.X += box.Width;
					path.LineTo(p);
					p.Y += box.Height;
					path.LineTo(p);
					p.Y = ext.Bottom;
					p.X = box.Left+margins.Right;
					path.LineTo(p);
					path.Close();
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(orange);
					graphics.Rasterizer.AddOutline(path);
					graphics.RenderSolid(border);
					break;

				case Type.MarginBottom:
					path = new Path();
					p = new Point(ext.Right, box.Top);
					path.MoveTo(p);
					p.X = box.Right;
					path.LineTo(p);
					p.Y -= box.Height;
					path.LineTo(p);
					p.X -= box.Width;
					path.LineTo(p);
					p.X = ext.Right;
					p.Y = box.Top-margins.Bottom;
					path.LineTo(p);
					path.Close();
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(orange);
					graphics.Rasterizer.AddOutline(path);
					graphics.RenderSolid(border);
					break;

				case Type.MarginTop:
					path = new Path();
					p = new Point(ext.Right, box.Bottom);
					path.MoveTo(p);
					p.X = box.Right;
					path.LineTo(p);
					p.Y += box.Height;
					path.LineTo(p);
					p.X -= box.Width;
					path.LineTo(p);
					p.X = ext.Right;
					p.Y = box.Bottom+margins.Top;
					path.LineTo(p);
					path.Close();
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(orange);
					graphics.Rasterizer.AddOutline(path);
					graphics.RenderSolid(border);
					break;

				case Type.PaddingLeft:
					path = new Path();
					path.MoveTo(inside.Left, inside.Center.Y);
					path.LineTo(box.BottomRight);
					path.LineTo(box.BottomLeft);
					path.LineTo(box.TopLeft);
					path.LineTo(box.TopRight);
					path.Close();
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(yellow);
					graphics.Rasterizer.AddOutline(path);
					graphics.RenderSolid(border);
					break;

				case Type.PaddingRight:
					path = new Path();
					path.MoveTo(inside.Right, inside.Center.Y);
					path.LineTo(box.BottomLeft);
					path.LineTo(box.BottomRight);
					path.LineTo(box.TopRight);
					path.LineTo(box.TopLeft);
					path.Close();
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(yellow);
					graphics.Rasterizer.AddOutline(path);
					graphics.RenderSolid(border);
					break;

				case Type.PaddingBottom:
					path = new Path();
					path.MoveTo(inside.Center.X, inside.Bottom);
					path.LineTo(box.TopLeft);
					path.LineTo(box.BottomLeft);
					path.LineTo(box.BottomRight);
					path.LineTo(box.TopRight);
					path.Close();
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(yellow);
					graphics.Rasterizer.AddOutline(path);
					graphics.RenderSolid(border);
					break;

				case Type.PaddingTop:
					path = new Path();
					path.MoveTo(inside.Center.X, inside.Top);
					path.LineTo(box.BottomLeft);
					path.LineTo(box.TopLeft);
					path.LineTo(box.TopRight);
					path.LineTo(box.BottomRight);
					path.Close();
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(yellow);
					graphics.Rasterizer.AddOutline(path);
					graphics.RenderSolid(border);
					break;

				case Type.GridColumn:
					r = box;
					r.Bottom = ext.Top;
					graphics.AddFilledRectangle(r);
					graphics.RenderSolid(PanelsContext.ColorHiliteSurface);
					graphics.AddRectangle(r);
					graphics.RenderSolid(border);
					break;

				case Type.GridRow:
					r = box;
					r.Right = ext.Left;
					graphics.AddFilledRectangle(r);
					graphics.RenderSolid(PanelsContext.ColorHiliteSurface);
					graphics.AddRectangle(r);
					graphics.RenderSolid(border);
					break;
			}

			this.DrawText(graphics, box);
		}

		public void DrawMark(Graphics graphics)
		{
			//	Dessine la marque de longueur d'une cote.
			Rectangle bounds = this.objectModifier.GetActualBounds(this.obj);
			Margins margins = this.objectModifier.GetMargins(this.obj);
			Rectangle ext = bounds;
			ext.Inflate(this.objectModifier.GetMargins(this.obj));

			Rectangle r, box;
			Point p1, p2;
			double value;

			box = this.TextBox;
			graphics.Align(ref box);
			box.Offset(0.5, 0.5);

			switch (this.type)
			{
				case Type.Width:
					r = box;
					r.Top = ext.Bottom;

					value = this.Value;
					if (value == bounds.Width)  // forme rectangulaire simple ?
					{
						p1 = new Point(box.Right, ext.Bottom);
						p2 = new Point(box.Left, ext.Bottom);
					}
					else
					{
						double half = System.Math.Floor(value/2);
						double middle = System.Math.Floor(r.Center.X)+0.5;
						p1 = new Point(middle-half, ext.Bottom);
						p2 = new Point(p1.X+half*2, ext.Bottom);
					}
					this.DrawLine(graphics, p1, p2);
					break;

				case Type.Height:
					r = box;
					r.Left = ext.Right;

					value = this.Value;
					if (value == bounds.Height)  // forme rectangulaire simple ?
					{
						p1 = new Point(ext.Right, box.Top);
						p2 = new Point(ext.Right, box.Bottom);
					}
					else
					{
						double half = System.Math.Floor(value/2);
						double middle = System.Math.Floor(r.Center.Y)+0.5;
						p1 = new Point(ext.Right, middle-half);
						p2 = new Point(ext.Right, p1.Y+half*2);
					}
					this.DrawLine(graphics, p1, p2);
					break;

				case Type.MarginLeft:
					p2 = new Point(box.Right, ext.Bottom);
					p1 = new Point(box.Right-margins.Left, ext.Bottom);
					this.DrawLine(graphics, p1, p2);
					break;

				case Type.MarginRight:
					p1 = new Point(box.Left, ext.Bottom);
					p2 = new Point(box.Left+margins.Right, ext.Bottom);
					this.DrawLine(graphics, p1, p2);
					break;

				case Type.MarginBottom:
					p2 = new Point(ext.Right, box.Top);
					p1 = new Point(ext.Right, box.Top-margins.Bottom);
					this.DrawLine(graphics, p1, p2);
					break;

				case Type.MarginTop:
					p1 = new Point(ext.Right, box.Bottom);
					p2 = new Point(ext.Right, box.Bottom+margins.Top);
					this.DrawLine(graphics, p1, p2);
					break;
			}
		}

		public void DrawHilite(Graphics graphics, bool dark)
		{
			//	Dessine la cote survolée par la souris.
			Rectangle bounds = this.objectModifier.GetActualBounds(this.obj);
			double alpha = dark ? 1.0 : 0.5;
			Color hilite = Color.FromAlphaRgb(alpha, 255.0/255.0, 124.0/255.0, 37.0/255.0);
			Color border = Color.FromBrightness(0);
			double t = 20;

			Rectangle box = this.TextBox;
			graphics.Align(ref box);
			box.Offset(0.5, 0.5);

			//	Dessine des triangles rouges up/down.
			Path path = new Path();
			if (this.type == Type.GridColumn || this.type == Type.GridRow)
			{
				path.AppendRectangle(box);
			}
			else
			{
				path.MoveTo(box.TopLeft);
				path.LineTo(box.Center.X-t/2, box.Top);
				path.LineTo(box.Center.X, box.Top+t/2);
				path.LineTo(box.Center.X+t/2, box.Top);
				path.LineTo(box.TopRight);
				path.LineTo(box.BottomRight);
				path.LineTo(box.Center.X+t/2, box.Bottom);
				path.LineTo(box.Center.X, box.Bottom-t/2);
				path.LineTo(box.Center.X-t/2, box.Bottom);
				path.LineTo(box.BottomLeft);
				path.Close();
			}
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(hilite);
			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(border);

			double value = this.Value;

			if (this.type == Type.Width && value != bounds.Width)
			{
				Rectangle r = box;
				double half = System.Math.Max(System.Math.Floor(value/2), 5);
				double middle = System.Math.Floor(r.Center.X)+0.5;
				r.Left = middle-half;
				r.Width = half*2;
				graphics.AddLine(r.Left, box.Bottom, r.Left, box.Top);
				graphics.AddLine(r.Right, box.Bottom, r.Right, box.Top);
				graphics.RenderSolid(border);

				this.DrawSpring(graphics, new Point(box.Left, box.Center.Y), new Point(r.Left, box.Center.Y), border);
				this.DrawSpring(graphics, new Point(box.Right, box.Center.Y), new Point(r.Right, box.Center.Y), border);
			}

			if (this.type == Type.Height && value != bounds.Height)
			{
				Rectangle r = box;
				double half = System.Math.Max(System.Math.Floor(value/2), 5);
				double middle = System.Math.Floor(r.Center.Y)+0.5;
				r.Bottom = middle-half;
				r.Height = half*2;
				graphics.AddLine(box.Left, r.Bottom, box.Right, r.Bottom);
				graphics.AddLine(box.Left, r.Top, box.Right, r.Top);
				graphics.RenderSolid(border);

				this.DrawSpring(graphics, new Point(box.Center.X, box.Bottom), new Point(box.Center.X, r.Bottom), border);
				this.DrawSpring(graphics, new Point(box.Center.X, box.Top), new Point(box.Center.X, r.Top), border);
			}

			//	Redessine la valeur par dessus.
			this.DrawText(graphics, box);

			//	Dessine les signes +/-.
			if (this.type != Type.GridColumn && this.type != Type.GridRow)
			{
				Point p = new Point(box.Center.X-1, box.Top+t/4-2);
				graphics.Align(ref p);
				p.X += 0.5;
				p.Y += 0.5;
				graphics.AddLine(p.X-2, p.Y, p.X+2, p.Y);
				graphics.AddLine(p.X, p.Y-2, p.X, p.Y+2);

				p = new Point(box.Center.X-1, box.Bottom-t/4+1);
				graphics.Align(ref p);
				p.X += 0.5;
				p.Y += 0.5;
				graphics.AddLine(p.X-2, p.Y, p.X+2, p.Y);

				graphics.RenderSolid(border);
			}
		}


		public double Value
		{
			//	Valeur réelle représentée par la cote.
			get
			{
				switch (this.type)
				{
					case Type.Width:
						return this.objectModifier.GetWidth(this.obj);

					case Type.Height:
						return this.objectModifier.GetHeight(this.obj);

					case Type.MarginLeft:
						return this.objectModifier.GetMargins(this.obj).Left;

					case Type.MarginRight:
						return this.objectModifier.GetMargins(this.obj).Right;

					case Type.MarginBottom:
						return this.objectModifier.GetMargins(this.obj).Bottom;

					case Type.MarginTop:
						return this.objectModifier.GetMargins(this.obj).Top;

					case Type.PaddingLeft:
						return this.objectModifier.GetPadding(this.obj).Left;

					case Type.PaddingRight:
						return this.objectModifier.GetPadding(this.obj).Right;

					case Type.PaddingBottom:
						return this.objectModifier.GetPadding(this.obj).Bottom;

					case Type.PaddingTop:
						return this.objectModifier.GetPadding(this.obj).Top;

					default:
						return 0;
				}
			}

			set
			{
				Margins m;

				switch (this.type)
				{
					case Type.Width:
						value = System.Math.Max(value, 0);
						this.objectModifier.SetWidth(this.obj, value);
						break;

					case Type.Height:
						value = System.Math.Max(value, 0);
						this.objectModifier.SetHeight(this.obj, value);
						break;

					case Type.MarginLeft:
						value = System.Math.Max(value, 0);
						m = this.objectModifier.GetMargins(this.obj);
						m.Left = value;
						this.objectModifier.SetMargins(this.obj, m);
						break;

					case Type.MarginRight:
						value = System.Math.Max(value, 0);
						m = this.objectModifier.GetMargins(this.obj);
						m.Right = value;
						this.objectModifier.SetMargins(this.obj, m);
						break;

					case Type.MarginBottom:
						value = System.Math.Max(value, 0);
						m = this.objectModifier.GetMargins(this.obj);
						m.Bottom = value;
						this.objectModifier.SetMargins(this.obj, m);
						break;

					case Type.MarginTop:
						value = System.Math.Max(value, 0);
						m = this.objectModifier.GetMargins(this.obj);
						m.Top = value;
						this.objectModifier.SetMargins(this.obj, m);
						break;

					case Type.PaddingLeft:
						value = System.Math.Max(value, 0);
						m = this.objectModifier.GetPadding(this.obj);
						m.Left = value;
						this.objectModifier.SetPadding(this.obj, m);
						break;

					case Type.PaddingRight:
						value = System.Math.Max(value, 0);
						m = this.objectModifier.GetPadding(this.obj);
						m.Right = value;
						this.objectModifier.SetPadding(this.obj, m);
						break;

					case Type.PaddingBottom:
						value = System.Math.Max(value, 0);
						m = this.objectModifier.GetPadding(this.obj);
						m.Bottom = value;
						this.objectModifier.SetPadding(this.obj, m);
						break;

					case Type.PaddingTop:
						value = System.Math.Max(value, 0);
						m = this.objectModifier.GetPadding(this.obj);
						m.Top = value;
						this.objectModifier.SetPadding(this.obj, m);
						break;
				}

				this.editor.Invalidate();
			}
		}

		public bool Detect(Point mouse)
		{
			//	Détecte si la souris est dans la cote.
			return this.TextBox.Contains(mouse);
		}


		protected void DrawLine(Graphics graphics, Point p1, Point p2)
		{
			//	Dessine le trait d'une cote.
			double d = Point.Distance(p1, p2);

			if (d < 1)
			{
				graphics.AddFilledCircle(p1, 2);
				graphics.RenderSolid(Color.FromBrightness(0));
				return;
			}

			double e = 3;
			double i = 1;

			if (p1.Y == p2.Y)  // horizontal ?
			{
				Size se = new Size(0, e);
				Size si = new Size(0, i);

				p1.Y += 0.5;
				p2.Y += 0.5;

				graphics.AddLine(p1, p2);
				if (d > 1)
				{
					graphics.AddLine(p1-si, p1+se);
					graphics.AddLine(p2-si, p2+se);
				}
				graphics.RenderSolid(Color.FromBrightness(0));
			}

			if (p1.X == p2.X)  // vertical ?
			{
				Size se = new Size(e, 0);
				Size si = new Size(i, 0);

				p1.X += 0.5;
				p2.X += 0.5;

				graphics.AddLine(p1, p2);
				if (d > 1)
				{
					graphics.AddLine(p1-si, p1+se);
					graphics.AddLine(p2-si, p2+se);
				}
				graphics.RenderSolid(Color.FromBrightness(0));
			}
		}

		protected void DrawSpring(Graphics graphics, Point p1, Point p2, Color color)
		{
			//	Dessine un petit ressort horizontal ou vertical d'une cote.
			if (Point.Distance(p1, p2) < 8)
			{
				graphics.AddLine(p1, p2);
			}
			else
			{
				Point p1a = Point.Scale(p1, p2, Dimension.attachmentScale);
				Point p2a = Point.Scale(p2, p1, Dimension.attachmentScale);

				graphics.AddLine(p1, p1a);
				graphics.AddLine(p2, p2a);

				double dim = Dimension.attachmentThickness;
				double length = Point.Distance(p1a, p2a);
				int loops = (int) (length/(dim*2));
				loops = System.Math.Max(loops, 1);
				Misc.AddSpring(graphics, p1a, p2a, dim, loops);
			}

			graphics.RenderSolid(color);

			graphics.AddFilledCircle(p1, 1.5);
			graphics.AddFilledCircle(p2, 1.5);
			graphics.RenderSolid(color);
		}

		protected void DrawText(Graphics graphics, Rectangle box)
		{
			//	Dessine la valeur d'une cote avec des petits caractères.
			if (this.type == Type.Height       ||
				this.type == Type.MarginBottom ||
				this.type == Type.MarginTop    ||
				this.type == Type.PaddingLeft  ||
				this.type == Type.PaddingRight )  // texte vertical ?
			{
				Point center = box.Center;
				Transform it = graphics.Transform;
				graphics.RotateTransformDeg(-90, center.X, center.Y);
				graphics.AddText(box.Left, box.Bottom, box.Width, box.Height, this.StringValue, Font.DefaultFont, 9.0, ContentAlignment.MiddleCenter);
				graphics.Transform = it;
			}
			else if (this.type == Type.GridRow)
			{
				Point center = box.Center;
				Transform it = graphics.Transform;
				graphics.RotateTransformDeg(90, center.X, center.Y);
				graphics.AddText(box.Left, box.Bottom, box.Width, box.Height, this.StringValue, Font.DefaultFont, 9.0, ContentAlignment.MiddleCenter);
				graphics.Transform = it;
			}
			else  // texte horizontal ?
			{
				graphics.AddText(box.Left, box.Bottom, box.Width, box.Height, this.StringValue, Font.DefaultFont, 9.0, ContentAlignment.MiddleCenter);
			}

			graphics.RenderSolid(Color.FromRgb(0, 0, 0));
		}

		protected string StringValue
		{
			//	Retourne la chaîne à afficher comme valeur de la cote.
			get
			{
				switch (this.type)
				{
					case Type.GridColumn:
						return Dimension.ToAlpha(this.column);  // A..ZZ

					case Type.GridRow:
						return (this.row+1).ToString(System.Globalization.CultureInfo.InvariantCulture);  // 1..n

					default:
						int i = (int) System.Math.Floor(this.Value+0.5);
						return i.ToString();
				}
			}
		}

		protected static string ToAlpha(int n)
		{
			//	Retourne un nombre en une chaîne en base 26 (donc A..Z, AA..AZ, BA..BZ, etc.).
			string text = "";

			do
			{
				int digit = n%26;
				char c = (char) ('A'+digit);
				text = text.Insert(0, c.ToString());

				n /= 26;
			}
			while (n != 0);

			return text;
		}

		protected Rectangle TextBox
		{
			//	Retourne le rectangle pour le texte d'une cote.
			get
			{
				Rectangle bounds = this.objectModifier.GetActualBounds(this.obj);
				Margins margins = this.objectModifier.GetMargins(this.obj);
				Rectangle ext = bounds;
				ext.Inflate(this.objectModifier.GetMargins(this.obj));

				double d = 26;
				double h = 12;
				double e = 10;
				double pw = 20;
				double ph = 12;
				double l;

				Rectangle box;

				switch (this.type)
				{
					case Type.Width:
						box = bounds;
						box.Bottom = ext.Bottom-d;
						box.Height = h;
						return box;

					case Type.Height:
						box = bounds;
						box.Left = ext.Right+d-h;
						box.Width = h;
						return box;

					case Type.MarginLeft:
						l = System.Math.Max(e, margins.Left);
						return new Rectangle(bounds.Left-l, ext.Bottom-d, l, h);

					case Type.MarginRight:
						l = System.Math.Max(e, margins.Right);
						return new Rectangle(bounds.Right, ext.Bottom-d, l, h);

					case Type.MarginTop:
						l = System.Math.Max(e, margins.Top);
						return new Rectangle(ext.Right+d-h, bounds.Top, h, l);

					case Type.MarginBottom:
						l = System.Math.Max(e, margins.Bottom);
						return new Rectangle(ext.Right+d-h, bounds.Bottom-l, h, l);

					case Type.PaddingLeft:
						box = this.objectModifier.GetFinalPadding(this.obj);
						return new Rectangle(bounds.Left-ph, box.Center.Y-pw/2, ph, pw);

					case Type.PaddingRight:
						box = this.objectModifier.GetFinalPadding(this.obj);
						return new Rectangle(bounds.Right, box.Center.Y-pw/2, ph, pw);

					case Type.PaddingTop:
						box = this.objectModifier.GetFinalPadding(this.obj);
						return new Rectangle(box.Center.X-pw/2, bounds.Top, pw, ph);

					case Type.PaddingBottom:
						box = this.objectModifier.GetFinalPadding(this.obj);
						return new Rectangle(box.Center.X-pw/2, bounds.Bottom-ph, pw, ph);

					case Type.GridColumn:
						box = this.objectModifier.GetGridCellArea(this.obj, this.column, 0, 1, 1);
						box.Bottom = ext.Top+d;
						box.Height = h;
						return box;

					case Type.GridRow:
						box = this.objectModifier.GetGridCellArea(this.obj, 0, this.row, 1, 1);
						box.Left = ext.Left-d-h;
						box.Width = h;
						return box;

					default:
						return Rectangle.Empty;
				}
			}
		}


		public static readonly double		margin = 38;
		protected static readonly double	attachmentThickness = 2.0;
		protected static readonly double	attachmentScale = 0.3;

		protected MyWidgets.PanelEditor		editor;
		protected ObjectModifier			objectModifier;
		protected PanelsContext				context;
		protected Widget					obj;
		protected Type						type;
		protected int						column;
		protected int						row;
	}
}
