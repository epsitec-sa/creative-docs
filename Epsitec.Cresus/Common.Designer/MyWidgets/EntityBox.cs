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
		public EntityBox() : base()
		{
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
			//	Etat de la boîte.
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


		public double GetBestHeight()
		{
			//?return this.GetBestFitSize().Height;
			if (this.isExtended)
			{
				double h = this.table[0].ActualHeight + this.table[0].Margins.Top + this.table[0].Margins.Bottom;
				return EntityBox.headerHeight + h*this.table.Count + EntityBox.footerHeight + 16;
			}
			else
			{
				return EntityBox.headerHeight + 6;
			}
		}


		protected void UpdateExtendButton()
		{
			this.extendButton.GlyphShape = this.isExtended ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;

			foreach (StaticText st in this.table)
			{
				st.Visibility = this.isExtended;
			}
		}

		protected void UpdateTable()
		{
			this.table = new List<StaticText>();

			for (int i=0; i<fields.Count; i++)
			{
				string field = this.fields[i];

				StaticText st = new StaticText(this);
				st.Text = field;
				st.Margins = new Margins(20, 20, 0, (i==this.fields.Count-1) ? EntityBox.footerHeight+8 : 2);
				st.Dock = DockStyle.Top;

				this.table.Add(st);
			}
		}


		protected override void ProcessMessage(Message message, Point pos)
		{
			if (message.MessageType == MessageType.MouseMove)
			{
				this.HiliteWidget(pos);
			}
		}

		protected void HiliteWidget(Point pos)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
			Widget finded = this.FindChild(pos);

			foreach (StaticText st in this.table)
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


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Rectangle bounds = this.Client.Bounds;
			bounds.Deflate(1);

			Path path = this.PathRoundRectangle(bounds, EntityBox.roundRectRadius);

			graphics.Rasterizer.AddSurface(path);
			Color c1 = adorner.ColorCaption;
			Color c2 = adorner.ColorCaption;
			c1.A = 0.4;
			c2.A = 0.1;
			this.RenderHorizontalGradient(graphics, bounds, c1, c2);

			if (this.isExtended)
			{
				graphics.AddFilledRectangle(bounds.Left, bounds.Bottom+EntityBox.footerHeight, bounds.Width, bounds.Height-EntityBox.footerHeight-EntityBox.headerHeight);
				graphics.RenderSolid(Color.FromBrightness(1));

				Rectangle shadow = new Rectangle(bounds.Left, bounds.Top-EntityBox.headerHeight-10, bounds.Width, 10);
				graphics.AddFilledRectangle(shadow);
				this.RenderVerticalGradient(graphics, shadow, Color.FromAlphaRgb(0.0, 0, 0, 0), Color.FromAlphaRgb(0.2, 0, 0, 0));
			}

			graphics.Rasterizer.AddOutline(path, 2);
			if (this.isExtended)
			{
				graphics.AddLine(bounds.Left, bounds.Top-EntityBox.headerHeight-0.5, bounds.Right, bounds.Top-EntityBox.headerHeight-0.5);
				graphics.AddLine(bounds.Left+2, bounds.Bottom+EntityBox.footerHeight+0.5, bounds.Right-2, bounds.Bottom+EntityBox.footerHeight+0.5);
			}
			graphics.RenderSolid(Color.FromBrightness(0));
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
			//	Génère un événement pour dire que la géométrie a changé.
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
		protected static readonly double headerHeight = 30;
		protected static readonly double footerHeight = 10;

		protected bool isExtended;
		protected string title;
		protected StaticText staticTitle;
		protected GlyphButton extendButton;
		protected List<string> fields;
		protected List<StaticText> table;
	}
}
