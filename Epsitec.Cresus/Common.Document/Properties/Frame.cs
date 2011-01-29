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
		None           = 0,		// ni cadre ni ombre
		OnlyFrame      = 1,		// cadre
		OnlyShadow     = 2,		// ombre
		FrameAndShadow = 3,		// cadre et ombre
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
				case  1:  type = FrameType.OnlyFrame;       break;
				case  2:  type = FrameType.FrameAndShadow;  break;
				case  3:  type = FrameType.OnlyShadow;      break;
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
				case FrameType.None:           name = Res.Strings.Property.Frame.None;            break;
				case FrameType.OnlyFrame:      name = Res.Strings.Property.Frame.OnlyFrame;       break;
				case FrameType.FrameAndShadow: name = Res.Strings.Property.Frame.FrameAndShadow;  break;
				case FrameType.OnlyShadow:     name = Res.Strings.Property.Frame.OnlyShadow;      break;
			}
			return name;
		}

		public static string GetIconText(FrameType type)
		{
			//	Retourne l'icône pour un type donné.
			switch ( type )
			{
				case FrameType.None:            return "FrameNone";
				case FrameType.OnlyFrame:       return "FrameOnlyFrame";
				case FrameType.FrameAndShadow:  return "FrameFrameAndShadow";
				case FrameType.OnlyShadow:      return "FrameOnlyShadow";
			}
			return "";
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
				if (this.frameType == Properties.FrameType.OnlyFrame ||
					this.frameType == Properties.FrameType.FrameAndShadow)
				{
					return true;
				}
			}

			if (rank == 1 || rank == 2 || rank == 3)  // shadow inflate/size/offset ?
			{
				if (this.frameType == Properties.FrameType.OnlyShadow ||
					this.frameType == Properties.FrameType.FrameAndShadow)
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

			if (polygon != null && polygon.Count >= 2)
			{
				if (rank == 0)  // margin width ?
				{
					polygon = polygon.Inflate (this.marginWidth);
					pos = Point.Scale (polygon.GetPoint (0), polygon.GetPoint (1), 0.5);
				}

				if (rank == 1)  // shadow inflate ?
				{
					polygon = polygon.Move (this.shadowOffsetX, this.shadowOffsetY);
					polygon = polygon.Inflate (this.marginWidth+this.shadowInflate);
					return polygon.GetPoint (0);
				}

				if (rank == 2)  // shadow size ?
				{
					polygon = polygon.Move (this.shadowOffsetX, this.shadowOffsetY);
					polygon = polygon.Inflate (this.marginWidth+this.shadowInflate+this.shadowSize);
					return polygon.GetPoint (0);
				}

				if (rank == 3)  // shadow offset ?
				{
					var center = polygon.Center;
					pos = new Point (center.X+this.shadowOffsetX, center.Y+this.shadowOffsetY);
				}
			}

			return pos;
		}

		public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			//	Modifie la position d'une poignée.
			var polygon = obj.PropertyHandleSupport;

			if (polygon != null)
			{
				if (rank == 0)  // margin width ?
				{
					var p = Point.Scale (polygon.GetPoint (0), polygon.GetPoint (1), 0.5);
					this.MarginWidth = Point.Distance (p, pos);
				}

				if (rank == 1)  // shadow inflate ?
				{
					var center = polygon.Center;
					double fx = (polygon.GetPoint (0).X < center.X) ? -1 : 1;
					polygon = polygon.Move (this.shadowOffsetX, this.shadowOffsetY);
					polygon = polygon.Inflate (this.marginWidth);
					this.ShadowInflate = (pos.X-polygon.GetPoint (0).X)*fx;
				}

				if (rank == 2)  // shadow size ?
				{
					var center = polygon.Center;
					double fx = (polygon.GetPoint (0).X < center.X) ? -1 : 1;
					polygon = polygon.Move (this.shadowOffsetX, this.shadowOffsetY);
					polygon = polygon.Inflate (this.marginWidth+this.shadowInflate);
					this.ShadowSize = (pos.X-polygon.GetPoint (0).X)*fx;
				}

				if (rank == 3)  // shadow offset ?
				{
					var center = polygon.Center;
					this.ShadowOffsetX = pos.X-center.X;
					this.ShadowOffsetY = pos.Y-center.Y;
				}
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

			var polygon = obj.PropertyHandleSupport;

			if (polygon != null)
			{
				var color = Drawing.Color.FromRgb (0.0, 0.6, 1.0);  // cyan

				double initialWidth = graphics.LineWidth;
				graphics.LineWidth = 1.0/drawingContext.ScaleX;

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

				if (this.frameType == Properties.FrameType.OnlyShadow ||
					this.frameType == Properties.FrameType.FrameAndShadow)
				{
					polygon = polygon.Move (this.shadowOffsetX, this.shadowOffsetY);

					polygon = polygon.Inflate (this.marginWidth+this.shadowInflate);
					Drawer.DrawPathDash (graphics, drawingContext, polygon.PolygonPath, 1.0, 4.0, 6.0, color);

					if (this.shadowSize > 0)
					{
						polygon = polygon.Inflate (this.shadowSize);
						Drawer.DrawPathDash (graphics, drawingContext, polygon.PolygonPath, 1.0, 4.0, 6.0, color);
					}
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
			if (polygons == null || polygons.Count == 0 || polygons[0].Count < 2)
			{
				shapes.AddRange (objectShapes);
			}
			else
			{
				var pp = Polygon.Inflate (polygons, this.marginWidth);
				var path = Polygon.GetPolygonPathCorner (drawingContext, pp, corner, false);

				//	Ajoute les éléments qui permettront de dessiner le cadre sous l'image.
				if (this.frameType == Properties.FrameType.OnlyShadow ||
					this.frameType == Properties.FrameType.FrameAndShadow)
				{
					var pp1 = Polygon.Inflate (polygons, this.marginWidth+this.shadowInflate);
					var pp2 = Polygon.Move (pp1, this.shadowOffsetX, this.shadowOffsetY);
					var shadowPath = Polygon.GetPolygonPathCorner (drawingContext, pp2, corner, false);

					var shape = new Shape ();
					shape.Path = shadowPath;
					shape.SetPropertySurface (port, this.GetPropertyFrameShadow (obj));
					shapes.Add (shape);

					//	Ajoute ce qu'il faut pour inclure les traits de construction (DrawEdit).
					var polygon = obj.PropertyHandleSupport;
					if (polygon != null)
					{
						polygon = polygon.Move (this.shadowOffsetX, this.shadowOffsetY);
						polygon = polygon.Inflate (this.marginWidth+this.shadowInflate+this.shadowSize);

						shape = new Shape ();
						shape.Path = polygon.PolygonPath;
						shape.Type = Common.Document.Type.Surface;
						shape.Aspect = Aspect.InvisibleBox;
						shapes.Add (shape);
					}
				}

				if (this.FrameType == Properties.FrameType.OnlyFrame ||
					this.FrameType == Properties.FrameType.FrameAndShadow)
				{
					var shape = new Shape ();
					shape.Path = path;
					shape.SetPropertySurface (port, this.GetPropertyFrameBackground (obj));
					shapes.Add (shape);
				}

				//	Ajoute les éléments qui permettront de dessiner l'objet.
				shapes.AddRange (objectShapes);

				//	Ajoute les éléments qui permettront de dessiner le cadre sur l'image.
				if (this.FrameType == Properties.FrameType.OnlyFrame ||
					this.FrameType == Properties.FrameType.FrameAndShadow)
				{
					if (this.frameWidth > 0)
					{
						var shape = new Shape ();
						shape.Path = path;
						shape.SetPropertyStroke (port, this.GetPropertyFrameStroke (obj), this.GetPropertyFrameSurface (obj));
						shapes.Add (shape);
					}
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
