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
	/// La classe Line repr�sente une propri�t� d'un objet graphique.
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
				if ( this.type == Type.LineDimension )
				{
					if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
					{
						this.width = 2.0;  // 0.2mm
					}
					else
					{
						this.width = 2.54;  // 0.01in
					}
				}
				else
				{
					if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
					{
						this.width = 5.00;  // 0.5mm
					}
					else
					{
						this.width = 5.08;  // 0.02in
					}
				}
			}

			this.cap   = CapStyle.Round;
			this.join  = JoinStyle.Round;
			this.limit = 5.0;

			this.dash = false;
			this.dashPen = new double[Line.DashMax];
			this.dashGap = new double[Line.DashMax];
			this.initialDashPen = new double[Line.DashMax];
			this.initialDashGap = new double[Line.DashMax];

			if ( this.document.Type == DocumentType.Pictogram )
			{
				this.dashPen[0] = 1.0;
				this.dashGap[0] = 1.0;
			}
			else
			{
				if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
				{
					this.dashPen[0] = 20.0;  // 2.0mm
					this.dashGap[0] = 20.0;
				}
				else
				{
					this.dashPen[0] = 25.4;  // 0.1in
					this.dashGap[0] = 25.4;
				}
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

		public JoinStyle EffectiveJoin
		{
			get
			{
				if ( this.join == JoinStyle.Miter )  return JoinStyle.MiterRevert;
				return this.join;
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


		// Retourne le nom d'un type donn�.
		public static string GetName(StandardDashType type)
		{
			string name = "";
			switch ( type )
			{
				case StandardDashType.Full:        name = Res.Strings.Property.Line.Full;        break;
				case StandardDashType.Line:        name = Res.Strings.Property.Line.Lines;        break;
				case StandardDashType.LineDense:   name = Res.Strings.Property.Line.LineDense;   break;
				case StandardDashType.LineExpand:  name = Res.Strings.Property.Line.LineExpand;  break;
				case StandardDashType.Dot:         name = Res.Strings.Property.Line.Dot;         break;
				case StandardDashType.LineDot:     name = Res.Strings.Property.Line.LineDot;     break;
				case StandardDashType.LineDotDot:  name = Res.Strings.Property.Line.LineDotDot;  break;
				case StandardDashType.Custom:      name = Res.Strings.Property.Line.Custom;      break;
			}
			return name;
		}

		// Retourne l'ic�ne pour un type donn�.
		public static string GetIconText(StandardDashType type)
		{
			switch ( type )
			{
				case StandardDashType.Full:        return "LineFull";
				case StandardDashType.Line:        return "LineLine";
				case StandardDashType.LineDense:   return "LineLineDense";
				case StandardDashType.LineExpand:  return "LineLineExpand";
				case StandardDashType.Dot:         return "LineDot";
				case StandardDashType.LineDot:     return "LineLineDot";
				case StandardDashType.LineDotDot:  return "LineLineDotDot";
				case StandardDashType.Custom:      return "LineCustom";
			}
			return "";
		}

		// Retourne le nom d'un type donn�.
		public static string GetName(CapStyle type)
		{
			string name = "";
			switch ( type )
			{
				case CapStyle.Round:   name = Res.Strings.Property.Line.CapRound;   break;
				case CapStyle.Square:  name = Res.Strings.Property.Line.CapSquare;  break;
				case CapStyle.Butt:    name = Res.Strings.Property.Line.CapButt;    break;
			}
			return name;
		}

		// Retourne l'ic�ne pour un type donn�.
		public static string GetIconText(CapStyle type)
		{
			switch ( type )
			{
				case CapStyle.Round:   return "CapRound";
				case CapStyle.Square:  return "CapSquare";
				case CapStyle.Butt:    return "CapButt";
			}
			return "";
		}

		// Retourne le nom d'un type donn�.
		public static string GetName(JoinStyle type)
		{
			string name = "";
			switch ( type )
			{
				case JoinStyle.Round:  name = Res.Strings.Property.Line.JoinRound;  break;
				case JoinStyle.Miter:  name = Res.Strings.Property.Line.JoinMiter;  break;
				case JoinStyle.Bevel:  name = Res.Strings.Property.Line.JoinBevel;  break;
			}
			return name;
		}

		// Retourne l'ic�ne pour un type donn�.
		public static string GetIconText(JoinStyle type)
		{
			switch ( type )
			{
				case JoinStyle.Round:  return "JoinRound";
				case JoinStyle.Miter:  return "JoinMiter";
				case JoinStyle.Bevel:  return "JoinBevel";
			}
			return "";
		}

		// Donne le petit texte pour les �chantillons.
		public override string SampleText
		{
			get
			{
				return this.document.Modifier.RealToString(this.width);
			}
		}


		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }
		}

		// Indique si le trait est visible.
		public override bool IsVisible(IPaintPort port)
		{
			return ( this.width != 0.0 );
		}

		// Effectue une copie de la propri�t�.
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

		// Compare deux propri�t�s.
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

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override Panels.Abstract CreatePanel(Document document)
		{
			Panels.Abstract.StaticDocument = document;
			return new Panels.Line(document);
		}


		// Retourne la valeur d'engraissement pour la bbox.
		public static double InflateBoundingBoxWidth(Shape shape)
		{
			Line line = shape.PropertyStroke as Line;
			if ( line == null )  return 0.0;
			return line.InflateBoundingBoxWidth();
		}

		// Retourne le facteur d'engraissement pour la bbox.
		public static double InflateBoundingBoxFactor(Shape shape)
		{
			Line line = shape.PropertyStroke as Line;
			if ( line == null )  return 1.0;
			return line.InflateBoundingBoxFactor();
		}

		// Retourne la valeur d'engraissement pour la bbox.
		public double InflateBoundingBoxWidth()
		{
			return this.width*0.5;
		}

		// Retourne le facteur d'engraissement pour la bbox.
		public double InflateBoundingBoxFactor()
		{
			if ( this.join == JoinStyle.Miter )
			{
				return this.Limit;
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

		// Retourne l'engraissement approximatif pour une bbox.
		public static double FatShape(Shape shape)
		{
			if ( shape.PropertyStroke == null )  return 0.0;

			double value = shape.PropertyStroke.Width*0.5;

			if ( shape.PropertyStroke.Join == JoinStyle.Miter )
			{
				value *= shape.PropertyStroke.Limit;
			}
			else if ( shape.PropertyStroke.Cap == CapStyle.Square )
			{
				value *= 1.415;  // augmente de racine de 2
			}
			
			return value;
		}


		// D�but du d�placement global de la propri�t�.
		public override void MoveGlobalStarting()
		{
			if ( !this.document.Modifier.ActiveViewer.SelectorAdaptLine )  return;

			this.InsertOpletProperty();

			this.initialWidth = this.width;

			for ( int i=0 ; i<Line.DashMax ; i++ )
			{
				this.initialDashPen[i] = this.dashPen[i];
				this.initialDashGap[i] = this.dashGap[i];
			}
		}
		
		// Effectue le d�placement global de la propri�t�.
		public override void MoveGlobalProcess(Selector selector)
		{
			if ( !this.document.Modifier.ActiveViewer.SelectorAdaptLine )  return;

			double zoom = selector.GetTransformZoom;

			this.width = this.initialWidth*zoom;

			for ( int i=0 ; i<Line.DashMax ; i++ )
			{
				this.dashPen[i] = this.initialDashPen[i]*zoom;
				this.dashGap[i] = this.initialDashGap[i]*zoom;
			}

			this.document.Notifier.NotifyPropertyChanged(this);
		}

		
		// Donne les valeurs trait/trou d'un traitill�.
		public void GetPenGap(int i, bool printing, out double pen, out double gap)
		{
			pen = this.dashPen[i];
			gap = this.dashGap[i];

			if ( pen == 0.0 )  // pointill� ?
			{
				double min = printing ? 0.5 : 0.00001;
				pen += min;
				gap -= min;
			}
		}

		// Exporte en PDF la propri�t�.
		public void ExportPDF(PDF.Port port, DrawingContext drawingContext, Objects.Abstract obj)
		{
			if ( this.dash )  // traitill� ?
			{
				port.SetLineDash(this.width, this.dashPen[0], this.dashGap[0], this.dashPen[1], this.dashGap[1], this.dashPen[2], this.dashGap[2]);
			}
			else	// trait continu ?
			{
				port.LineWidth = this.width;
			}

			port.LineCap = this.cap;
			port.LineJoin = this.join;
			port.LineMiterLimit = this.limit;
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
		protected double					initialWidth;
		protected double[]					initialDashPen;
		protected double[]					initialDashGap;
		protected static readonly double	step = 0.01;
		public static readonly int			DashMax = 3;
	}
}
