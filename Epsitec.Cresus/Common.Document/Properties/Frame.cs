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
			this.shadowInflate = 0.0;
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


		public static void GetFieldsParam(FrameType type, out double frameWidth, out double marginWidth, out double shadowSize, out double shadowInflate, out double shadowOffsetX, out double shadowOffsetY)
		{
			//	Retourne les valeurs par défaut et les min/max pour un type donné.
			frameWidth = 0;
			marginWidth = 0;
			shadowSize = 0;
			shadowInflate = 0;
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
						shadowSize = 40.0;  // 4.0mm
						shadowInflate = -20.0;  // -2.0mm
						shadowOffsetX = 40.0;  // 4.0mm
						shadowOffsetY = -40.0;  // -4.0mm
					}
					else
					{
						shadowSize = 50.8;  // 0.2in
						shadowInflate = -25.4;  // -0.1mm
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
			p.shadowInflate = this.shadowInflate;
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
			if ( p.shadowInflate != this.shadowInflate )  return false;
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


		public void AddShapes(List<Shape> shapes, List<Shape> objectShapes, IPaintPort port, DrawingContext drawingContext, List<Polygon> polygons, Properties.Corner corner)
		{
			//	Ajoute les éléments pour dessiner l'objet avec son cadre.
			if (polygons == null || polygons.Count == 0 || polygons[0].Points.Count < 2)
			{
				shapes.AddRange (objectShapes);
			}
			else
			{
				bool exact = true;

				var pp = Polygon.Inflate (polygons, this.marginWidth, exact);
				var path = Polygon.GetPolygonPathCorner (drawingContext, pp, corner, false);

				//	Ajoute les éléments qui permettront de dessiner le cadre sous l'image.
				if (this.shadowSize > 0)
				{
					var pp1 = Polygon.Inflate (polygons, this.marginWidth+this.shadowInflate, exact);
					var pp2 = Polygon.Move (pp1, this.shadowOffsetX, this.shadowOffsetY);
					var shadowPath = Polygon.GetPolygonPathCorner (drawingContext, pp2, corner, false);

					var shape = new Shape ();
					shape.Path = shadowPath;
					shape.SetPropertySurface (port, this.PropertyShadowSurface);

					shapes.Add (shape);
				}

				if (this.FrameType != Properties.FrameType.ShadowAlone)
				{
					var shape = new Shape ();
					shape.Path = path;
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
			info.AddValue ("ShadowInflate", this.shadowInflate, typeof (double));
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
			this.shadowInflate = (double) info.GetValue ("ShadowInflate", typeof (double));
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
		protected double				shadowInflate;
		protected double				shadowOffsetX;
		protected double				shadowOffsetY;
	}
}
