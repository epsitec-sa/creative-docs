using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	public enum StandardDashType
	{
		Full       = 0,
		Line       = 1,
		LineDense  = 2,
		LineExpand = 3,
		Dot        = 4,
		LineDot    = 5,
		LineDotDot = 6,
		Custom     = 100,
	}

	/// <summary>
	/// La classe Line représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Line : Abstract
	{
		public Line(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialise()
		{
			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.width = 1.0;
			}
			else
			{
				this.width = 5.0;  // 0.5mm
			}

			this.cap   = CapStyle.Round;
			this.join  = JoinStyle.Round;
			this.limit = 5.0;

			this.dash = false;
			this.dashPen = new double[Line.DashMax];
			this.dashGap = new double[Line.DashMax];

			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.dashPen[0] = 1.0;
				this.dashGap[0] = 1.0;
			}
			else
			{
				this.dashPen[0] = 50.0;  // 5.0mm
				this.dashGap[0] = 50.0;
			}
			
			for ( int i=1 ; i<Line.DashMax ; i++ )
			{
				this.dashPen[i] = 0.0;
				this.dashGap[i] = 0.0;
			}
		}

		public double Width
		{
			get
			{
				return this.width;
			}
			
			set
			{
				if ( this.width != value )
				{
					this.NotifyBefore();
					this.width = value;
					this.NotifyAfter();
				}
			}
		}

		public CapStyle Cap
		{
			get
			{
				return this.cap;
			}

			set
			{
				if ( this.cap != value )
				{
					this.NotifyBefore();
					this.cap = value;
					this.NotifyAfter();
				}
			}
		}

		public JoinStyle Join
		{
			get
			{
				return this.join;
			}
			
			set
			{
				if ( this.join != value )
				{
					this.NotifyBefore();
					this.join = value;
					this.NotifyAfter();
				}
			}
		}

		public double Limit
		{
			get
			{
				return this.limit;
			}
			
			set
			{
				if ( this.limit != value )
				{
					this.NotifyBefore();
					this.limit = value;
					this.NotifyAfter();
				}
			}
		}

		public StandardDashType StandardDash
		{
			get
			{
				if ( this.dash )
				{
					if ( this.dashPen[0] != 0.0 &&
						 this.dashGap[0] == this.dashPen[0] &&
						 this.dashPen[1] == 0.0 &&
						 this.dashGap[1] == 0.0 &&
						 this.dashPen[2] == 0.0 &&
						 this.dashGap[2] == 0.0 )  return StandardDashType.Line;

					if ( this.dashPen[0] != 0.0 &&
						 this.dashGap[0] == this.dashPen[0]*0.5 &&
						 this.dashPen[1] == 0.0 &&
						 this.dashGap[1] == 0.0 &&
						 this.dashPen[2] == 0.0 &&
						 this.dashGap[2] == 0.0 )  return StandardDashType.LineDense;

					if ( this.dashPen[0] != 0.0 &&
						 this.dashGap[0] == this.dashPen[0]*2.0 &&
						 this.dashPen[1] == 0.0 &&
						 this.dashGap[1] == 0.0 &&
						 this.dashPen[2] == 0.0 &&
						 this.dashGap[2] == 0.0 )  return StandardDashType.LineExpand;

					if ( this.dashPen[0] == 0.0 &&
						 this.dashGap[0] != 0.0 &&
						 this.dashPen[1] == 0.0 &&
						 this.dashGap[1] == 0.0 &&
						 this.dashPen[2] == 0.0 &&
						 this.dashGap[2] == 0.0 )  return StandardDashType.Dot;

					if ( this.dashPen[0] == this.dashGap[0]*2.0 &&
						 this.dashPen[1] == 0.0 &&
						 this.dashGap[1] == this.dashGap[0] &&
						 this.dashPen[2] == 0.0 &&
						 this.dashGap[2] == 0.0 )  return StandardDashType.LineDot;

					if ( this.dashPen[0] == this.dashGap[0]*2.0 &&
						 this.dashPen[1] == 0.0 &&
						 this.dashGap[1] == this.dashGap[0] &&
						 this.dashPen[2] == 0.0 &&
						 this.dashGap[2] == this.dashGap[0] )  return StandardDashType.LineDotDot;

					return StandardDashType.Custom;
				}
				return StandardDashType.Full;
			}
			
			set
			{
				switch ( value )
				{
					case StandardDashType.Full:
						this.Dash = false;
						break;

					case StandardDashType.Line:
						this.Dash = true;
						this.DashPen1 = this.DashGap1;
						this.DashPen2 = 0.0;
						this.DashGap2 = 0.0;
						this.DashPen3 = 0.0;
						this.DashGap3 = 0.0;
						break;

					case StandardDashType.LineDense:
						this.Dash = true;
						this.DashPen1 = this.DashGap1*0.5;
						this.DashPen2 = 0.0;
						this.DashGap2 = 0.0;
						this.DashPen3 = 0.0;
						this.DashGap3 = 0.0;
						break;

					case StandardDashType.LineExpand:
						this.Dash = true;
						this.DashPen1 = this.DashGap1*2.0;
						this.DashPen2 = 0.0;
						this.DashGap2 = 0.0;
						this.DashPen3 = 0.0;
						this.DashGap3 = 0.0;
						break;

					case StandardDashType.Dot:
						this.Dash = true;
						this.DashPen1 = 0.0;
						this.DashPen2 = 0.0;
						this.DashGap2 = 0.0;
						this.DashPen3 = 0.0;
						this.DashGap3 = 0.0;
						break;

					case StandardDashType.LineDot:
						this.Dash = true;
						this.DashPen1 = this.DashGap1*2.0;
						this.DashPen2 = 0.0;
						this.DashGap2 = this.DashGap1;
						this.DashPen3 = 0.0;
						this.DashGap3 = 0.0;
						break;

					case StandardDashType.LineDotDot:
						this.Dash = true;
						this.DashPen1 = this.DashGap1*2.0;
						this.DashPen2 = 0.0;
						this.DashGap2 = this.DashGap1;
						this.DashPen3 = 0.0;
						this.DashGap3 = this.DashGap1;
						break;

					case StandardDashType.Custom:
						this.Dash = true;
						break;
				}
			}
		}

		public double StandardLength
		{
			get
			{
				return this.DashGap1;
			}
			
			set
			{
				//?StandardDashType type = this.StandardDash;
				this.DashGap1 = value;
				//?this.StandardDash = type;
			}
		}

		public bool Dash
		{
			get
			{
				return this.dash;
			}
			
			set
			{
				if ( this.dash != value )
				{
					this.NotifyBefore();
					this.dash = value;
					this.NotifyAfter();
				}
			}
		}

		public double DashPen1
		{
			get
			{
				return this.dashPen[0];
			}
			
			set
			{
				if ( this.dashPen[0] != value )
				{
					this.NotifyBefore();
					this.dashPen[0] = value;
					this.NotifyAfter();
				}
			}
		}

		public double DashGap1
		{
			get
			{
				return this.dashGap[0];
			}
			
			set
			{
				if ( this.dashGap[0] != value )
				{
					this.NotifyBefore();
					this.dashGap[0] = value;
					this.NotifyAfter();
				}
			}
		}

		public double DashPen2
		{
			get
			{
				return this.dashPen[1];
			}
			
			set
			{
				if ( this.dashPen[1] != value )
				{
					this.NotifyBefore();
					this.dashPen[1] = value;
					this.NotifyAfter();
				}
			}
		}

		public double DashGap2
		{
			get
			{
				return this.dashGap[1];
			}
			
			set
			{
				if ( this.dashGap[1] != value )
				{
					this.NotifyBefore();
					this.dashGap[1] = value;
					this.NotifyAfter();
				}
			}
		}

		public double DashPen3
		{
			get
			{
				return this.dashPen[2];
			}
			
			set
			{
				if ( this.dashPen[2] != value )
				{
					this.NotifyBefore();
					this.dashPen[2] = value;
					this.NotifyAfter();
				}
			}
		}

		public double DashGap3
		{
			get
			{
				return this.dashGap[2];
			}
			
			set
			{
				if ( this.dashGap[2] != value )
				{
					this.NotifyBefore();
					this.dashGap[2] = value;
					this.NotifyAfter();
				}
			}
		}

		public double GetDashPen(int rank)
		{
			return this.dashPen[rank];
		}

		public void SetDashPen(int rank, double value)
		{
			if ( this.dashPen[rank] != value )
			{
				this.NotifyBefore();
				this.dashPen[rank] = value;
				this.NotifyAfter();
			}
		}

		public double GetDashGap(int rank)
		{
			return this.dashGap[rank];
		}

		public void SetDashGap(int rank, double value)
		{
			if ( this.dashGap[rank] != value )
			{
				this.NotifyBefore();
				this.dashGap[rank] = value;
				this.NotifyAfter();
			}
		}

		// Indique si un changement de cette propriété modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Line p = property as Line;
			p.width = this.width;
			p.cap   = this.cap;
			p.join  = this.join;
			p.limit = this.limit;
			p.dash  = this.dash;

			for ( int i=0 ; i<Line.DashMax ; i++ )
			{
				p.dashPen[i] = this.dashPen[i];
				p.dashGap[i] = this.dashGap[i];
			}
		}

		// Compare deux propriétés.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Line p = property as Line;
			if ( p.width != this.width )  return false;
			if ( p.cap   != this.cap   )  return false;
			if ( p.join  != this.join  )  return false;
			if ( p.limit != this.limit )  return false;
			if ( p.dash  != this.dash  )  return false;

			for ( int i=0 ; i<Line.DashMax ; i++ )
			{
				if ( p.dashPen[i] != this.dashPen[i] )  return false;
				if ( p.dashGap[i] != this.dashGap[i] )  return false;
			}

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override Panels.Abstract CreatePanel(Document document)
		{
			return new Panels.Line(document);
		}


		// Retourne la valeur d'engraissement pour la bbox.
		public double InflateBoundingBoxWidth()
		{
			return this.width*0.5;
		}

		// Retourne la valeur d'engraissement pour la bbox.
		public double InflateBoundingBoxFactor()
		{
			if ( this.join == JoinStyle.Miter )
			{
				return this.limit;
			}
			else if ( this.cap == CapStyle.Square )
			{
				return 1.415;  // augmente de racine de 2
			}
			else
			{
				return 1.0;
			}
		}


		// Effectue un graphics.Rasterizer.AddOutline.
		public void AddOutline(Graphics graphics, Path path, double addWidth)
		{
			graphics.Rasterizer.AddOutline(path, this.width+addWidth, this.cap, this.join, this.limit);
		}

		// Effectue un port.PaintOutline.
		public void PaintOutline(Printing.PrintPort port, DrawingContext drawingContext, Path path)
		{
			if ( this.dash )  // traitillé ?
			{
				DashedPath dp = new DashedPath();
				dp.DefaultZoom = drawingContext.ScaleX;
				dp.Append(path);

				for ( int i=0 ; i<Line.DashMax ; i++ )
				{
					if ( this.dashGap[i] == 0.0 )  continue;
					double pen, gap;
					this.GetPenGap(i, true, out pen, out gap);
					dp.AddDash(pen, gap);
				}

				port.LineWidth = this.width;
				port.LineCap = this.cap;
				port.LineJoin = this.join;
				port.LineMiterLimit = this.limit;

				using ( Path temp = dp.GenerateDashedPath() )
				{
					port.PaintOutline(temp);
				}
			}
			else	// trait continu ?
			{
				port.LineWidth = this.width;
				port.LineCap = this.cap;
				port.LineJoin = this.join;
				port.LineMiterLimit = this.limit;
				port.PaintOutline(path);
			}
		}

		// Dessine le trait le long d'un chemin.
		public void DrawPath(Graphics graphics, DrawingContext drawingContext, Path path, Drawing.Color color)
		{
			if ( this.width == 0.0 )  return;
			if ( path.IsEmpty )  return;

			if ( this.dash )  // traitillé ?
			{
				DashedPath dp = new DashedPath();
				dp.DefaultZoom = drawingContext.ScaleX;
				dp.Append(path);

				for ( int i=0 ; i<Line.DashMax ; i++ )
				{
					if ( this.dashGap[i] == 0.0 )  continue;
					double pen, gap;
					this.GetPenGap(i, false, out pen, out gap);
					dp.AddDash(pen, gap);
				}

				using ( Path temp = dp.GenerateDashedPath() )
				{
					graphics.Rasterizer.AddOutline(temp, this.width, this.cap, this.join, this.limit);
					graphics.RenderSolid(drawingContext.AdaptColor(color));
				}
			}
			else	// trait continu ?
			{
				graphics.Rasterizer.AddOutline(path, this.width, this.cap, this.join, this.limit);
				graphics.RenderSolid(drawingContext.AdaptColor(color));
			}
		}

		// Dessine le trait le long d'un chemin.
		public void DrawPath(Graphics graphics, DrawingContext drawingContext, Path path, Properties.Gradient gradient, Rectangle bbox)
		{
			if ( this.width == 0.0 )  return;
			if ( path.IsEmpty )  return;

			if ( this.dash )  // traitillé ?
			{
				DashedPath dp = new DashedPath();
				dp.DefaultZoom = drawingContext.ScaleX;
				dp.Append(path);

				for ( int i=0 ; i<Line.DashMax ; i++ )
				{
					if ( this.dashGap[i] == 0.0 )  continue;
					double pen, gap;
					this.GetPenGap(i, false, out pen, out gap);
					dp.AddDash(pen, gap);
				}

				using ( Path temp = dp.GenerateDashedPath() )
				{
					gradient.RenderOutline(graphics, drawingContext, temp, this.width, this.cap, this.join, this.limit, bbox);
				}
			}
			else	// trait continu ?
			{
				gradient.RenderOutline(graphics, drawingContext, path, this.width, this.cap, this.join, this.limit, bbox);
			}
		}

		// Donne les valeurs trait/trou d'un traitillé.
		protected void GetPenGap(int i, bool printing, out double pen, out double gap)
		{
			pen = this.dashPen[i];
			gap = this.dashGap[i];

			if ( pen == 0.0 )  // pointillé ?
			{
				double min = printing ? 0.5 : 0.00001;
				pen += min;
				gap -= min;
			}
		}

		// Dessine le traitillé le long d'un chemin, lorsque le trait n'existe pas.
		public void DrawPathDash(Graphics graphics, DrawingContext drawingContext, Path path, Properties.Gradient gradient)
		{
			if ( this.width != 0.0 && gradient.IsVisible() )  return;
			if ( path.IsEmpty )  return;

			Drawing.Color color = Drawing.Color.FromBrightness(0);
			if ( gradient.IsVisible() )
			{
				if ( gradient.Color1.A > 0 )
				{
					color = gradient.Color1;
				}
				else
				{
					color = gradient.Color2;
				}
			}
			Line.DrawPathDash(graphics, drawingContext, path, 1.0, 0.0, 4.0, color);
		}

		// Dessine un traitillé simple (dash/gap) le long d'un chemin.
		public static void DrawPathDash(Graphics graphics, DrawingContext drawingContext, Path path,
										double width, double dash, double gap, Drawing.Color color)
		{
			if ( path.IsEmpty )  return;

			DashedPath dp = new DashedPath();
			dp.DefaultZoom = drawingContext.ScaleX;
			dp.Append(path);

			dash /= drawingContext.ScaleX;
			gap  /= drawingContext.ScaleX;
			if ( dash == 0.0 )  // juste un point ?
			{
				dash = 0.00001;
				gap -= dash;
			}
			dp.AddDash(dash, gap);

			using ( Path temp = dp.GenerateDashedPath() )
			{
				graphics.Rasterizer.AddOutline(temp, width/drawingContext.ScaleX, CapStyle.Square, JoinStyle.Round, 5.0);
				graphics.RenderSolid(color);
			}
		}


		#region Serialization
		// Sérialise la propriété.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Width", this.width);
			info.AddValue("CapStyle", this.cap);
			info.AddValue("JoinStyle", this.join);
			info.AddValue("Limit", this.limit);
			info.AddValue("Dash", this.dash);
			if ( this.dash )
			{
				info.AddValue("DashPen", this.dashPen);
				info.AddValue("DashGap", this.dashGap);
			}
		}

		// Constructeur qui désérialise la propriété.
		protected Line(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.width = info.GetDouble("Width");
			this.cap = (CapStyle) info.GetValue("CapStyle", typeof(CapStyle));
			this.join = (JoinStyle) info.GetValue("JoinStyle", typeof(JoinStyle));
			this.limit = info.GetDouble("Limit");
			this.dash = info.GetBoolean("Dash");
			if ( this.dash )
			{
				this.dashPen = (double[]) info.GetValue("DashPen", typeof(double[]));
				this.dashGap = (double[]) info.GetValue("DashGap", typeof(double[]));
			}
		}
		#endregion

		
		protected double					width;
		protected CapStyle					cap;
		protected JoinStyle					join;
		protected double					limit;  // longueur (et non angle) !
		protected StandardDashType			standardDash;
		protected bool						dash;
		protected double[]					dashPen;
		protected double[]					dashGap;
		protected static readonly double	step = 0.01;
		public static readonly int			DashMax = 3;
	}
}
