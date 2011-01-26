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

			this.shadowInflate = 0.0;
			this.shadowSize = 0.0;
			this.shadowColor = RichColor.FromAGray (60.0/255.0, 0);  // noir très transparent (alpha = 60)
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

		public double ShadowInflate
		{
			get
			{
				return this.shadowInflate;
			}

			set
			{
				if (this.shadowInflate != value)
				{
					this.NotifyBefore ();
					this.shadowInflate = value;
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
				value = System.Math.Max (value, 0.0);

				if (this.shadowSize != value)
				{
					this.NotifyBefore ();
					this.shadowSize = value;
					this.NotifyAfter ();
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


		public static void GetFieldsParam(FrameType type, out double frameWidth, out double marginWidth, out double shadowInflate, out double shadowSize, out double shadowOffsetX, out double shadowOffsetY)
		{
			//	Retourne les valeurs par défaut et les min/max pour un type donné.
			frameWidth = 0;
			marginWidth = 0;
			shadowInflate = 0;
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
						shadowInflate = -20.0;  // -2.0mm
						shadowSize = 40.0;  // 4.0mm
						shadowOffsetX = 40.0;  // 4.0mm
						shadowOffsetY = -40.0;  // -4.0mm
					}
					else
					{
						shadowInflate = -25.4;  // -0.1mm
						shadowSize = 50.8;  // 0.2in
						shadowOffsetX = 50.8;  // 0.2in
						shadowOffsetY = -50.8;  // -0.2in
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

			if (this.frameType == Properties.FrameType.None)
			{
				return false;
			}

			if (rank == 0)  // margin width ?
			{
				if (this.frameType == Properties.FrameType.Thick ||
					this.frameType == Properties.FrameType.ThickAndSnadow)
				{
					return true;
				}
			}

			if (rank == 1 || rank == 2 || rank == 3)  // shadow inflate/size/offset ?
			{
				if (this.frameType == Properties.FrameType.Shadow ||
					this.frameType == Properties.FrameType.ShadowAlone ||
					this.frameType == Properties.FrameType.ThickAndSnadow)
				{
					return true;
				}
			}

			return false;
		}

		public override Point GetHandlePosition(Objects.Abstract obj, int rank)
		{
			//	Retourne la position d'une poignée.
			Point pos = new Point ();
			var polygon = obj.PropertyHandleSupport;

			if (rank == 0)  // margin width ?
			{
				if (this.frameType == Properties.FrameType.Thick ||
					this.frameType == Properties.FrameType.ThickAndSnadow)
				{
					polygon = polygon.Inflate (this.marginWidth);
					pos = Point.Scale (polygon.Points[0], polygon.Points[1], 0.5);
				}
			}

			if (rank == 1)  // shadow inflate ?
			{
				polygon = polygon.Move (this.shadowOffsetX, this.shadowOffsetY);
				polygon = polygon.Inflate (this.marginWidth+this.shadowInflate);
				return polygon.Points[0];
			}

			if (rank == 2)  // shadow size ?
			{
				polygon = polygon.Move (this.shadowOffsetX, this.shadowOffsetY);
				polygon = polygon.Inflate (this.marginWidth+this.shadowInflate+this.shadowSize);
				return polygon.Points[0];
			}
			
			if (rank == 3)  // shadow offset ?
			{
				var center = polygon.Center;
				pos = new Point (center.X+this.shadowOffsetX, center.Y+this.shadowOffsetY);
			}

			return pos;
		}

		public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			//	Modifie la position d'une poignée.
			var polygon = obj.PropertyHandleSupport;

			if (rank == 0)  // margin width ?
			{
				var p = Point.Scale (polygon.Points[0], polygon.Points[1], 0.5);
				this.MarginWidth = Point.Distance (p, pos);
			}

			if (rank == 1)  // shadow inflate ?
			{
				var center = polygon.Center;
				double fx = (polygon.Points[0].X < center.X) ? -1 : 1;
				polygon = polygon.Move (this.shadowOffsetX, this.shadowOffsetY);
				polygon = polygon.Inflate (this.marginWidth);
				this.ShadowInflate = (pos.X-polygon.Points[0].X)*fx;
			}

			if (rank == 2)  // shadow size ?
			{
				var center = polygon.Center;
				double fx = (polygon.Points[0].X < center.X) ? -1 : 1;
				polygon = polygon.Move (this.shadowOffsetX, this.shadowOffsetY);
				polygon = polygon.Inflate (this.marginWidth+this.shadowInflate);
				this.ShadowSize = (pos.X-polygon.Points[0].X)*fx;
			}

			if (rank == 3)  // shadow offset ?
			{
				var center = polygon.Center;
				this.ShadowOffsetX = pos.X-center.X;
				this.ShadowOffsetY = pos.Y-center.Y;
			}

			base.SetHandlePosition (obj, rank, pos);
		}


		public override void DrawEdit(Graphics graphics, DrawingContext drawingContext, Objects.Abstract obj)
		{
			//	Dessine les traits de construction avant les poignées.
			if (!obj.IsSelected ||
				 obj.IsGlobalSelected ||
				 obj.IsEdited ||
				 (!this.IsHandleVisible (obj, 0) && !this.IsHandleVisible (obj, 1)))
			{
				return;
			}

			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/drawingContext.ScaleX;

			var polygon = obj.PropertyHandleSupport;
			var color = Drawing.Color.FromBrightness (0.6);

			if (this.shadowOffsetX != 0 && this.shadowOffsetY != 0)
			{
				var center = polygon.Center;
				var pa = new Point (center.X+this.shadowOffsetX, center.Y+this.shadowOffsetY);

				double radius = 3.0/drawingContext.ScaleX;
				graphics.AddCircle (center, radius);

				if (Point.Distance (center, pa) > radius)
				{
					center = Point.Move (center, pa, radius);
					graphics.AddLine (pa, Geometry.ComputeArrowExtremity (center, pa, 0.4, 0.2, 0));
					graphics.AddLine (pa, Geometry.ComputeArrowExtremity (center, pa, 0.4, 0.2, 1));  // flèche
					graphics.AddLine (center, pa);
				}
			}

			graphics.RenderSolid (color);
			graphics.LineWidth = initialWidth;

			if (this.frameType == Properties.FrameType.Shadow ||
				this.frameType == Properties.FrameType.ShadowAlone ||
				this.frameType == Properties.FrameType.ThickAndSnadow)
			{
				polygon = polygon.Move (this.shadowOffsetX, this.shadowOffsetY);

				polygon = polygon.Inflate (this.marginWidth+this.shadowInflate);
				Drawer.DrawPathDash (graphics, drawingContext, polygon.PolygonPath, 1.0, 4.0, 8.0, color);

				if (this.shadowSize > 0)
				{
					polygon = polygon.Inflate (this.shadowSize);
					Drawer.DrawPathDash (graphics, drawingContext, polygon.PolygonPath, 1.0, 4.0, 8.0, color);
				}
			}
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
			p.shadowInflate = this.shadowInflate;
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
			if ( p.shadowInflate != this.shadowInflate )  return false;
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


		public void AddShapes(Objects.Abstract obj, List<Shape> shapes, List<Shape> objectShapes, IPaintPort port, DrawingContext drawingContext, List<Polygon> polygons, Properties.Corner corner)
		{
			//	Ajoute les éléments pour dessiner l'objet avec son cadre.
			if (polygons == null || polygons.Count == 0 || polygons[0].Points.Count < 2)
			{
				shapes.AddRange (objectShapes);
			}
			else
			{
				var pp = Polygon.Inflate (polygons, this.marginWidth);
				var path = Polygon.GetPolygonPathCorner (drawingContext, pp, corner, false);

				//	Ajoute les éléments qui permettront de dessiner le cadre sous l'image.
				if (this.shadowSize > 0)
				{
					var pp1 = Polygon.Inflate (polygons, this.marginWidth+this.shadowInflate);
					var pp2 = Polygon.Move (pp1, this.shadowOffsetX, this.shadowOffsetY);
					var shadowPath = Polygon.GetPolygonPathCorner (drawingContext, pp2, corner, false);

					var shape = new Shape ();
					shape.Path = shadowPath;
					shape.SetPropertySurface (port, this.GetPropertyFrameShadow (obj));

					shapes.Add (shape);
				}

				if (this.FrameType != Properties.FrameType.ShadowAlone)
				{
					var shape = new Shape ();
					shape.Path = path;
					shape.SetPropertySurface (port, this.GetPropertyFrameBackground (obj));

					shapes.Add (shape);
				}

				//	Ajoute les éléments qui permettront de dessiner l'objet.
				shapes.AddRange (objectShapes);

				//	Ajoute les éléments qui permettront de dessiner le cadre sur l'image.
				if (this.frameWidth > 0)
				{
					var shape = new Shape ();
					shape.Path = path;
					shape.SetPropertyStroke (port, this.GetPropertyFrameStroke (obj), this.GetPropertyFrameSurface (obj));

					shapes.Add (shape);
				}
			}
		}


		private Properties.Line GetPropertyFrameStroke (Objects.Abstract obj)
		{
			//	Retourne une propriété additionnelle permettant de dessiner le trait du cadre.
			var line = obj.AdditionnalPropertyFrameStroke;

			line.IsOnlyForCreation = true;
			line.Width = this.frameWidth;
			line.Cap = CapStyle.Round;

			return line;
		}

		private Properties.Gradient GetPropertyFrameSurface (Objects.Abstract obj)
		{
			//	Retourne une propriété additionnelle permettant de dessiner la surface du cadre.
			var surface = obj.AdditionnalPropertyFrameSurface;

			surface.IsOnlyForCreation = true;
			surface.Color1 = this.frameColor;

			return surface;
		}

		private Properties.Gradient GetPropertyFrameBackground (Objects.Abstract obj)
		{
			//	Retourne une propriété additionnelle permettant de dessiner le fond du cadre.
			var surface = obj.AdditionnalPropertyFrameBackground;

			surface.IsOnlyForCreation = true;
			surface.Color1 = this.backgroundColor;

			return surface;
		}

		private Properties.Gradient GetPropertyFrameShadow (Objects.Abstract obj)
		{
			//	Retourne une propriété additionnelle permettant de dessiner l'ombre du cadre.
			var surface = obj.AdditionnalPropertyFrameShadow;

			surface.IsOnlyForCreation = true;
			surface.Smooth = this.shadowSize;
			surface.Color1 = this.shadowColor;

			return surface;
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

			info.AddValue ("ShadowInflate", this.shadowInflate, typeof (double));
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

			this.shadowInflate = (double) info.GetValue ("ShadowInflate", typeof (double));
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

		protected double				shadowInflate;
		protected double				shadowSize;
		protected RichColor				shadowColor;
		protected double				shadowOffsetX;
		protected double				shadowOffsetY;
	}
}
