using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe PropertyLine repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class PropertyLine : AbstractProperty
	{
		public PropertyLine(Document document) : base(document)
		{
			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.width = 1.0;
			}
			else
			{
				this.width = 0.2;
			}

			this.cap   = CapStyle.Round;
			this.join  = JoinStyle.Round;
			this.limit = 5.0;

			this.dash = false;
			this.dashPen = new double[PropertyLine.DashMax];
			this.dashGap = new double[PropertyLine.DashMax];

			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.dashPen[0] = 1.0;
				this.dashGap[0] = 1.0;
			}
			else
			{
				this.dashPen[0] = 5.0;
				this.dashGap[0] = 5.0;
			}
			
			for ( int i=1 ; i<PropertyLine.DashMax ; i++ )
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

		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }
		}

		// Effectue une copie de la propri�t�.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyLine p = property as PropertyLine;
			p.width = this.width;
			p.cap   = this.cap;
			p.join  = this.join;
			p.limit = this.limit;
			p.dash  = this.dash;

			for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
			{
				p.dashPen[i] = this.dashPen[i];
				p.dashGap[i] = this.dashGap[i];
			}
		}

		// Compare deux propri�t�s.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyLine p = property as PropertyLine;
			if ( p.width != this.width )  return false;
			if ( p.cap   != this.cap   )  return false;
			if ( p.join  != this.join  )  return false;
			if ( p.limit != this.limit )  return false;
			if ( p.dash  != this.dash  )  return false;

			for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
			{
				if ( p.dashPen[i] != this.dashPen[i] )  return false;
				if ( p.dashGap[i] != this.dashGap[i] )  return false;
			}

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override AbstractPanel CreatePanel(Document document)
		{
			return new PanelLine(document);
		}


		// Engraisse la bbox selon le trait.
		public void InflateBoundingBox(ref Rectangle bbox)
		{
			if ( this.join == JoinStyle.Miter )
			{
				bbox.Inflate(this.width*0.5*this.limit);
			}
			else if ( this.cap == CapStyle.Square )
			{
				bbox.Inflate(this.width*0.5*1.415);  // augmente de racine de 2
			}
			else
			{
				bbox.Inflate(this.width*0.5);
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
			if ( this.dash )  // traitill� ?
			{
				DashedPath dp = new DashedPath();
				dp.DefaultZoom = drawingContext.ScaleX;
				dp.Append(path);

				for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
				{
					if ( this.dashGap[i] == 0.0 )  continue;
					double pen = this.dashPen[i];
					if ( pen == 0.0 )  pen = 0.00001;
					dp.AddDash(pen, this.dashGap[i]);
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
		public void DrawPath(Graphics graphics, DrawingContext drawingContext, Path path, Color color)
		{
			if ( this.width == 0.0 )  return;
			if ( path.IsEmpty )  return;

			if ( this.dash )  // traitill� ?
			{
				DashedPath dp = new DashedPath();
				dp.DefaultZoom = drawingContext.ScaleX;
				dp.Append(path);

				for ( int i=0 ; i<PropertyLine.DashMax ; i++ )
				{
					if ( this.dashGap[i] == 0.0 )  continue;
					double pen = this.dashPen[i];
					if ( pen == 0.0 )  pen = 0.00001;
					dp.AddDash(pen, this.dashGap[i]);
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


		#region Serialization
		// S�rialise la propri�t�.
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

		// Constructeur qui d�s�rialise la propri�t�.
		protected PropertyLine(SerializationInfo info, StreamingContext context) : base(info, context)
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
			else
			{
				this.dashPen = new double[PropertyLine.DashMax];
				this.dashGap = new double[PropertyLine.DashMax];
				this.dashPen[0] = 1.0;
				this.dashGap[0] = 1.0;
				for ( int i=1 ; i<PropertyLine.DashMax ; i++ )
				{
					this.dashPen[i] = 0.0;
					this.dashGap[i] = 0.0;
				}
			}
		}
		#endregion

		
		protected double					width;
		protected CapStyle					cap;
		protected JoinStyle					join;
		protected double					limit;  // longueur (et non angle) !
		protected bool						dash;
		protected double[]					dashPen;
		protected double[]					dashGap;
		protected static readonly double	step = 0.01;
		public static readonly int			DashMax = 3;
	}
}
