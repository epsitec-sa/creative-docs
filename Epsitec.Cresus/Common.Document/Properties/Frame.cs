using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	//	ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	//	sous peine de plantée lors de la désérialisation.
	public enum FrameType
	{
		None           = 0,			// pas de cadre
		Simple         = 10,		// simple trait
		Thick          = 20,		// bord épais
		Shadow         = 21,		// ombre
		ThickAndSnadow = 22,		// bord épais et ombre
		ShadowAlone    = 30,		// ombre seule
	}

	/// <summary>
	/// La classe Frame représente une propriété d'un cadre.
	/// </summary>
	[System.Serializable()]
	public class Frame : Abstract
	{
		public Frame(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialize()
		{
			base.Initialize ();

			this.frameType = FrameType.None;

			this.frameWidth = 0.0;
			this.frameColor = RichColor.FromBrightness (0);  // noir

			this.marginWidth = 0.0;
			this.backgroundColor = RichColor.FromBrightness (1);  // blanc

			this.shadowSize = 0.0;
			this.shadowColor = RichColor.FromBrightness (0.5);  // gris
			this.shadowOffsetX = 0.0;
			this.shadowOffsetY = 0.0;
		}

		public FrameType FrameType
		{
			get
			{
				return this.frameType;
			}

			set
			{
				if (this.frameType != value)
				{
					this.NotifyBefore();
					this.frameType = value;
					this.AdjustType();
					this.NotifyAfter();
				}
			}
		}

		public double FrameWidth
		{
			get
			{
				return this.frameWidth;
			}

			set
			{
				value = System.Math.Max (value, 0.0);

				if (this.frameWidth != value)
				{
					this.NotifyBefore ();
					this.frameWidth = value;
					this.NotifyAfter ();
				}
			}
		}

		public RichColor FrameColor
		{
			get
			{
				return this.frameColor;
			}

			set
			{
				if (this.frameColor != value)
				{
					this.NotifyBefore ();
					this.frameColor = value;
					this.NotifyAfter ();
				}
			}
		}

		public double MarginWidth
		{
			get
			{
				return this.marginWidth;
			}

			set
			{
				value = System.Math.Max(value, 0.0);

				if (this.marginWidth != value)
				{
					this.NotifyBefore();
					this.marginWidth = value;
					this.NotifyAfter();
				}
			}
		}

		public RichColor BackgroundColor
		{
			get
			{
				return this.backgroundColor;
			}

			set
			{
				if (this.backgroundColor != value)
				{
					this.NotifyBefore ();
					this.backgroundColor = value;
					this.NotifyAfter ();
				}
			}
		}

		public double ShadowSize
		{
			get
			{
				return this.shadowSize;
			}

			set
			{
				value = System.Math.Max(value, 0.0);

				if (this.shadowSize != value)
				{
					this.NotifyBefore();
					this.shadowSize = value;
					this.NotifyAfter();
				}
			}
		}

		public double ShadowOffsetX
		{
			get
			{
				return this.shadowOffsetX;
			}

			set
			{
				if (this.shadowOffsetX != value)
				{
					this.NotifyBefore ();
					this.shadowOffsetX = value;
					this.NotifyAfter ();
				}
			}
		}

		public double ShadowOffsetY
		{
			get
			{
				return this.shadowOffsetY;
			}

			set
			{
				if (this.shadowOffsetY != value)
				{
					this.NotifyBefore ();
					this.shadowOffsetY = value;
					this.NotifyAfter ();
				}
			}
		}

		public RichColor ShadowColor
		{
			get
			{
				return this.shadowColor;
			}

			set
			{
				if (this.shadowColor != value)
				{
					this.NotifyBefore ();
					this.shadowColor = value;
					this.NotifyAfter ();
				}
			}
		}


		public override string SampleText
		{
			//	Donne le petit texte pour les échantillons.
			get
			{
				return Frame.GetName(this.frameType);
			}
		}

		public override void PutStyleBrief(System.Text.StringBuilder builder)
		{
			//	Construit le texte résumé d'un style pour une propriété.
			this.PutStyleBriefPrefix(builder);
			builder.Append(this.SampleText);
			this.PutStyleBriefPostfix(builder);
		}

		public static FrameType ConvType(int index)
		{
			//	Cherche le type correspondant à un index donné.
			//	Ceci détermine l'ordre dans le TextFieldCombo du panneau.
			FrameType type = FrameType.None;
			switch ( index )
			{
				case  0:  type = FrameType.None;            break;
				case  1:  type = FrameType.Simple;          break;
				case  2:  type = FrameType.Thick;           break;
				case  3:  type = FrameType.Shadow;          break;
				case  4:  type = FrameType.ThickAndSnadow;  break;
			}
			return type;
		}

		public static int ConvType(FrameType type)
		{
			//	Cherche le rang d'un type donné.
			for ( int i=0 ; i<100 ; i++ )
			{
				FrameType t = Frame.ConvType(i);
				if ( t == FrameType.None )  break;
				if ( t == type )  return i;
			}
			return -1;
		}

		public static string GetName(FrameType type)
		{
			//	Retourne le nom d'un type donné.
			string name = "";
			switch ( type )
			{
				case FrameType.None:            name = Res.Strings.Property.Frame.None;            break;
				case FrameType.Simple:          name = Res.Strings.Property.Frame.Simple;          break;
				case FrameType.Thick:           name = Res.Strings.Property.Frame.Thick;           break;
				case FrameType.Shadow:          name = Res.Strings.Property.Frame.Shadow;          break;
				case FrameType.ThickAndSnadow:  name = Res.Strings.Property.Frame.ThickAndShadow;  break;
				case FrameType.ShadowAlone:     name = Res.Strings.Property.Frame.ShadowAlone;     break;
			}
			return name;
		}

		public static string GetIconText(FrameType type)
		{
			//	Retourne l'icône pour un type donné.
			switch ( type )
			{
				case FrameType.None:            return "FrameNone";
				case FrameType.Simple:          return "FrameSimple";
				case FrameType.Thick:           return "FrameThick";
				case FrameType.Shadow:          return "FrameShadow";
				case FrameType.ThickAndSnadow:  return "FrameThickAndShadow";
				case FrameType.ShadowAlone:     return "FrameShadowAlone";
			}
			return "";
		}


		public static void GetFieldsParam(FrameType type, out double frameWidth, out double marginWidth, out double shadowSize, out double shadowOffsetX, out double shadowOffsetY)
		{
			//	Retourne les valeurs par défaut et les min/max pour un type donné.
			frameWidth = 0;
			marginWidth = 0;
			shadowSize = 0;
			shadowOffsetX = 0;
			shadowOffsetY = 0;

			switch (type)
			{
				case FrameType.Simple:
					if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
					{
						frameWidth = 2.0;  // 0.2mm
					}
					else
					{
						frameWidth = 2.54;  // 0.01in
					}
					break;

				case FrameType.Thick:
					if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
					{
						frameWidth = 2.0;  // 0.2mm
						marginWidth = 50.0;  // 5mm
					}
					else
					{
						frameWidth = 2.54;  // 0.01in
						marginWidth = 63.5;  // 0.25in
					}
					break;

				case FrameType.Shadow:
					if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
					{
						frameWidth = 2.0;  // 0.2mm
						shadowSize = 25.0;  // 2.5mm
					}
					else
					{
						frameWidth = 2.54;  // 0.01in
						shadowSize = 20.0;  // 2.0mm
					}
					break;

				case FrameType.ThickAndSnadow:
					if ( System.Globalization.RegionInfo.CurrentRegion.IsMetric )
					{
						frameWidth = 2.0;  // 0.2mm
						marginWidth = 50.0;  // 5mm
						shadowSize = 20.0;  // 2.0mm
					}
					else
					{
						frameWidth = 2.54;  // 0.01in
						marginWidth = 63.5;  // 0.25in
						shadowSize = 25.4;  // 0.1in
					}
					break;

				case FrameType.ShadowAlone:
					if (System.Globalization.RegionInfo.CurrentRegion.IsMetric)
					{
						shadowSize = 20.0;  // 2.0mm
						shadowOffsetX = 20.0;  // 2.0mm
						shadowOffsetY = -20.0;  // -2.0mm
					}
					else
					{
						shadowSize = 25.4;  // 0.1in
						shadowOffsetX = 25.4;  // 0.1in
						shadowOffsetY = -25.4;  // -0.1in
					}
					break;
			}
		}
		
		
		public override bool AlterBoundingBox
		{
			//	Indique si un changement de cette propriété modifie la bbox de l'objet.
			get { return true; }
		}


		protected void AdjustType()
		{
			//	Ajuste les paramètres selon le type.
		}

		public override int TotalHandle(Objects.Abstract obj)
		{
			//	Nombre de poignées.
			return 4;
		}

		public override bool IsHandleVisible(Objects.Abstract obj, int rank)
		{
			//	Indique si une poignée est visible.
			if ( !this.document.Modifier.IsPropertiesExtended(this.type) )
			{
				return false;
			}

			return false;
		}
		
		public override Point GetHandlePosition(Objects.Abstract obj, int rank)
		{
			//	Retourne la position d'une poignée.
			Point pos = new Point(0,0);

			return pos;
		}

		public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			//	Modifie la position d'une poignée.
			base.SetHandlePosition(obj, rank, pos);
		}
		
		
		public override void CopyTo(Abstract property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			Frame p = property as Frame;

			p.frameType  = this.frameType;
			p.frameWidth = this.frameWidth;
			p.frameColor = this.frameColor;
			p.marginWidth = this.marginWidth;
			p.backgroundColor = this.backgroundColor;
			p.shadowSize = this.shadowSize;
			p.shadowOffsetX = this.shadowOffsetX;
			p.shadowOffsetY = this.shadowOffsetY;
			p.shadowColor = this.shadowColor;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			Frame p = property as Frame;
			
			if (p.frameType != this.frameType)  return false;
			if ( p.frameWidth != this.frameWidth)  return false;
			if ( p.frameColor != this.frameColor)  return false;
			if ( p.marginWidth != this.marginWidth )  return false;
			if ( p.backgroundColor != this.backgroundColor )  return false;
			if ( p.shadowSize != this.shadowSize )  return false;
			if ( p.shadowOffsetX != this.shadowOffsetX )  return false;
			if ( p.shadowOffsetY != this.shadowOffsetY )  return false;
			if ( p.shadowColor != this.shadowColor)  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			Panels.Abstract.StaticDocument = document;
			return new Panels.Frame(document);
		}


		public void AddShapes(List<Shape> shapes, List<Shape> objectShapes, IPaintPort port, DrawingContext drawingContext, List<Point> points, Properties.Corner corner)
		{
			//	Ajoute les éléments pour dessiner l'objet avec son cadre.
			if (points.Count < 2)
			{
				shapes.AddRange (objectShapes);
			}
			else
			{
				var pp = Frame.Inflate (points, this.marginWidth);
				var path = Frame.GetPolygonPathCorner (drawingContext, pp, corner, false);

				Path shadowPath;
				if (this.shadowSize == 0 || (this.shadowOffsetX == 0 && this.shadowOffsetY == 0))
				{
					shadowPath = path;
				}
				else
				{
					var m = Frame.Move (pp, this.shadowOffsetX, this.shadowOffsetY);
					shadowPath = Frame.GetPolygonPathCorner (drawingContext, m, corner, false);
				}

				//	Ajoute les éléments qui permettront de dessiner le cadre sous l'image.
				if (this.shadowSize > 0)
				{
					var shape = new Shape ();
					shape.Path = shadowPath;
					shape.FillMode = FillMode.EvenOdd;
					shape.SetPropertySurface (port, this.PropertyShadowSurface);

					shapes.Add (shape);
				}

				if (this.FrameType != Properties.FrameType.ShadowAlone)
				{
					var shape = new Shape ();
					shape.Path = path;
					shape.FillMode = FillMode.EvenOdd;
					shape.SetPropertySurface (port, this.PropertyBackgroundSurface);

					shapes.Add (shape);
				}

				//	Ajoute les éléments qui permettront de dessiner l'objet.
				shapes.AddRange (objectShapes);

				//	Ajoute les éléments qui permettront de dessiner le cadre sur l'image.
				if (this.frameWidth > 0)
				{
					var shape = new Shape ();
					shape.Path = path;
					shape.SetPropertyStroke (port, this.PropertyFrameStroke, this.PropertyFrameSurface);

					shapes.Add (shape);
				}
			}
		}

		private static List<Point> Move(List<Point> points, double mx, double my)
		{
			var pp = new List<Point> ();
			var move = new Point (mx, my);

			for (int i = 0; i < points.Count; i++)
			{
				Point p = points[i];

				pp.Add (p+move);
			}

			return pp;
		}

		private static List<Point> Inflate(List<Point> points, double inflate)
		{
			var pp = new List<Point> ();

			for (int i = 0; i < points.Count; i++)
			{
				Point a = points[(i-1 >= 0) ? i-1 : points.Count-1];  // point précédent
				Point p = points[i];                                  // point courant
				Point b = points[(i+1 < points.Count) ? i+1 : 0];     // point suivant

				pp.Add (Frame.InflateCorner (a, p, b, inflate));
			}

			return pp;
		}

		private static Point InflateCorner(Point a, Point p, Point b, double inflate)
		{
			if (inflate == 0)
			{
				return p;
			}
			else
			{
				var aa = Point.Move (p, a, -inflate);
				return Point.Move(aa, b+aa-p, -inflate);
			}
		}

		public static Path GetPolygonPathCorner(DrawingContext drawingContext, List<Point> points, Properties.Corner corner, bool simplify)
		{
			//	Crée le chemin d'un polygone à coins quelconques.
			if (corner == null)
			{
				return Frame.GetPolygonPath (points);
			}
			else
			{
				double min = double.MaxValue;

				for (int i = 0; i < points.Count; i++)
				{
					Point p = points[i];                               // point courant
					Point b = points[(i+1 < points.Count) ? i+1 : 0];  // point suivant

					double d = Point.Distance (p, b);
					min = System.Math.Min (min, d);
				}

				double radius = simplify ? 0.0 : System.Math.Min (corner.Radius, min/2);

				if (corner.CornerType == Properties.CornerType.Right || radius == 0.0)
				{
					return Frame.GetPolygonPath (points);
				}
				else
				{
					Path path = new Path ();
					path.DefaultZoom = Properties.Abstract.DefaultZoom (drawingContext);

					for (int i = 0; i < points.Count; i++)
					{
						Point a = points[(i-1 >= 0) ? i-1 : points.Count-1];  // point précédent
						Point p = points[i];                                  // point courant
						Point b = points[(i+1 < points.Count) ? i+1 : 0];     // point suivant

						Point c1 = Point.Move (p, a, radius);
						Point c2 = Point.Move (p, b, radius);

						if (i == 0)
						{
							path.MoveTo (c1);
						}
						else
						{
							path.LineTo (c1);
						}

						corner.PathCorner (path, c1, p, c2, radius);
					}

					path.Close ();

					return path;
				}
			}
		}

		public static Path GetPolygonPath(List<Point> points)
		{
			//	Crée le chemin d'un polygone à coins droits.
			var path = new Path ();

			for (int i = 0; i < points.Count; i++)
			{
				if (i == 0)
				{
					path.MoveTo (points[i]);
				}
				else
				{
					path.LineTo (points[i]);
				}
			}

			path.Close ();

			return path;
		}


		private Properties.Line PropertyFrameStroke
		{
			//	Retourne une propriété permettant de dessiner le cadre.
			get
			{
				var line = Properties.Abstract.NewProperty (this.document, Properties.Type.LineMode) as Properties.Line;

				line.IsOnlyForCreation = true;
				line.Width = this.frameWidth;
				line.Cap = CapStyle.Round;

				return line;
			}
		}

		private Properties.Gradient PropertyFrameSurface
		{
			//	Retourne une propriété permettant de dessiner le cadre.
			get
			{
				var surface = Properties.Abstract.NewProperty (this.document, Properties.Type.FillGradient) as Properties.Gradient;

				surface.IsOnlyForCreation = true;
				surface.Color1 = this.frameColor;

				return surface;
			}
		}

		private Properties.Gradient PropertyBackgroundSurface
		{
			//	Retourne une propriété permettant de dessiner le cadre.
			get
			{
				var surface = Properties.Abstract.NewProperty (this.document, Properties.Type.FillGradient) as Properties.Gradient;

				surface.IsOnlyForCreation = true;
				surface.Color1 = this.backgroundColor;

				return surface;
			}
		}

		private Properties.Gradient PropertyShadowSurface
		{
			//	Retourne une propriété permettant de dessiner l'ombre.
			get
			{
				var surface = Properties.Abstract.NewProperty (this.document, Properties.Type.FillGradient) as Properties.Gradient;

				surface.IsOnlyForCreation = true;
				surface.Smooth = this.shadowSize;
				surface.Color1 = this.shadowColor;

				return surface;
			}
		}
		

		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise la propriété.
			base.GetObjectData(info, context);

			info.AddValue ("FrameType", this.frameType, typeof (FrameType));

			info.AddValue ("FrameWidth", this.frameWidth, typeof (double));
			info.AddValue ("FrameColor", this.frameColor);
			
			info.AddValue ("MarginWidth", this.marginWidth, typeof (double));
			info.AddValue ("BackgroundColor", this.backgroundColor);
			
			info.AddValue ("ShadowSize", this.shadowSize, typeof (double));
			info.AddValue ("ShadowColor", this.shadowColor);
			info.AddValue ("ShadowOffsetX", this.shadowOffsetX, typeof (double));
			info.AddValue ("ShadowOffsetY", this.shadowOffsetY, typeof (double));
		}

		protected Frame(SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
			//	Constructeur qui désérialise la propriété.
			this.frameType = (FrameType) info.GetValue ("FrameType", typeof (FrameType));
			
			this.frameWidth = (double) info.GetValue ("FrameWidth", typeof (double));
			this.frameColor = (RichColor) info.GetValue ("FrameColor", typeof (RichColor));
			
			this.marginWidth = (double) info.GetValue ("MarginWidth", typeof (double));
			this.backgroundColor = (RichColor) info.GetValue ("BackgroundColor", typeof (RichColor));
			
			this.shadowSize = (double) info.GetValue ("ShadowSize", typeof (double));
			this.shadowColor = (RichColor) info.GetValue ("ShadowColor", typeof (RichColor));
			this.shadowOffsetX = (double) info.GetValue ("ShadowOffsetX", typeof (double));
			this.shadowOffsetY = (double) info.GetValue ("ShadowOffsetY", typeof (double));
		}
		#endregion


		protected FrameType				frameType;

		protected double				frameWidth;
		protected RichColor				frameColor;
		
		protected double				marginWidth;
		protected RichColor				backgroundColor;
		
		protected double				shadowSize;
		protected RichColor				shadowColor;
		protected double				shadowOffsetX;
		protected double				shadowOffsetY;
	}
}
