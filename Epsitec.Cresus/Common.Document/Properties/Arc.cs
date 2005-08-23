using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	// ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	// sous peine de plant�e lors de la d�s�rialisation.
	public enum ArcType
	{
		Full  = 0,
		Open  = 1,
		Close = 2,
		Pie   = 3,
	}

	/// <summary>
	/// La classe Arc repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Arc : Abstract
	{
		public Arc(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialise()
		{
			this.arcType       = ArcType.Full;
			this.startingAngle =  90.0;
			this.endingAngle   = 360.0;
		}

		public ArcType ArcType
		{
			get
			{
				return this.arcType;
			}

			set
			{
				if ( this.arcType != value )
				{
					this.NotifyBefore();
					this.arcType = value;
					this.NotifyAfter();
				}
			}
		}

		public double StartingAngle
		{
			get
			{
				return this.startingAngle;
			}
			
			set
			{
				if ( this.startingAngle != value )
				{
					this.NotifyBefore();
					this.startingAngle = value;
					this.NotifyAfter();
				}
			}
		}

		public double EndingAngle
		{
			get
			{
				return this.endingAngle;
			}
			
			set
			{
				if ( this.endingAngle != value )
				{
					this.NotifyBefore();
					this.endingAngle = value;
					this.NotifyAfter();
				}
			}
		}

#if false
		// Donne le petit texte pour les �chantillons.
		public override string SampleText
		{
			get
			{
				string a1 = this.document.Modifier.AngleToString(this.startingAngle);
				string a2 = this.document.Modifier.AngleToString(this.endingAngle);
				return string.Format("{0} - {1}", a1, a2);
			}
		}
#endif

		// Retourne le nom d'un type donn�.
		public static string GetName(ArcType type)
		{
			string name = "";
			switch ( type )
			{
				case ArcType.Full:   name = Res.Strings.Property.Arc.Full;   break;
				case ArcType.Open:   name = Res.Strings.Property.Arc.Open;   break;
				case ArcType.Close:  name = Res.Strings.Property.Arc.Close;  break;
				case ArcType.Pie:    name = Res.Strings.Property.Arc.Pie;    break;
			}
			return name;
		}

		// Retourne l'ic�ne pour un type donn�.
		public static string GetIconText(ArcType type)
		{
			switch ( type )
			{
				case ArcType.Full:   return "ArcFull";
				case ArcType.Open:   return "ArcOpen";
				case ArcType.Close:  return "ArcClose";
				case ArcType.Pie:    return "ArcPie";
			}
			return "";
		}

		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }
		}


		// D�but du d�placement d'une poign�e.
		public override void MoveHandleStarting(Objects.Abstract obj, int rank, Point pos, DrawingContext drawingContext)
		{
			Point center = obj.Handle(0).Position;
			drawingContext.ConstrainAddCenter(center);
		}
		
		// Nombre de poign�es.
		public override int TotalHandle(Objects.Abstract obj)
		{
			return 2;
		}

		// Indique si une poign�e est visible.
		public override bool IsHandleVisible(Objects.Abstract obj, int rank)
		{
			if ( !this.document.Modifier.IsPropertiesExtended(this.type) )
			{
				return false;
			}

			return (this.arcType != ArcType.Full);
		}
		
		// Retourne la position d'une poign�e.
		public override Point GetHandlePosition(Objects.Abstract obj, int rank)
		{
			Point pos = new Point();

			if ( obj is Objects.Circle )
			{
				Objects.Circle circle = obj as Objects.Circle;

				if ( rank == 0 )  // starting angle ?
				{
					pos = circle.ComputeArcHandle(this.startingAngle);
				}

				if ( rank == 1 )  // ending angle ?
				{
					pos = circle.ComputeArcHandle(this.endingAngle);
				}
			}

			if ( obj is Objects.Ellipse )
			{
				Objects.Ellipse ellipse = obj as Objects.Ellipse;

				if ( rank == 0 )  // starting angle ?
				{
					pos = ellipse.ComputeArcHandle(this.startingAngle);
				}

				if ( rank == 1 )  // ending angle ?
				{
					pos = ellipse.ComputeArcHandle(this.endingAngle);
				}
			}

			return pos;
		}

		// Modifie la position d'une poign�e.
		public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			if ( obj is Objects.Circle )
			{
				Objects.Circle circle = obj as Objects.Circle;

				if ( rank == 0 )  // starting angle ?
				{
					this.StartingAngle = circle.ComputeArcHandle(pos);
				}

				if ( rank == 1 )  // ending angle ?
				{
					this.EndingAngle = circle.ComputeArcHandle(pos);
				}
			}

			if ( obj is Objects.Ellipse )
			{
				Objects.Ellipse ellipse = obj as Objects.Ellipse;

				if ( rank == 0 )  // starting angle ?
				{
					this.StartingAngle = ellipse.ComputeArcHandle(pos);
				}

				if ( rank == 1 )  // ending angle ?
				{
					this.EndingAngle = ellipse.ComputeArcHandle(pos);
				}
			}

			base.SetHandlePosition(obj, rank, pos);
		}


		// Cr�e le chemin d'une ellipse inscrite dans un rectangle.
		public Path PathEllipse(Rectangle rect)
		{
			Stretcher stretcher = new Stretcher();
			stretcher.InitialRectangle = rect;
			stretcher.FinalBottomLeft = rect.BottomLeft;
			stretcher.FinalBottomRight = rect.BottomRight;
			stretcher.FinalTopLeft = rect.TopLeft;
			stretcher.FinalTopRight = rect.TopRight;

			Point center = rect.Center;

			Path path = new Path();

			double a1, a2;
			if ( this.ArcType == Properties.ArcType.Full )
			{
				a1 =   0.0;
				a2 = 360.0;
			}
			else
			{
				a1 = this.StartingAngle;
				a2 = this.EndingAngle;
			}
			if ( a1 != a2 )
			{
				Geometry.ArcBezierDeg(path, stretcher, center, rect.Width/2.0, rect.Height/2.0, a1, a2, true, false);
			}

			if ( this.ArcType == Properties.ArcType.Close )
			{
				path.Close();
			}
			if ( this.ArcType == Properties.ArcType.Pie )
			{
				path.LineTo(stretcher.Transform(center));
				path.Close();
			}

			return path;
		}
		
		
		// Effectue une copie de la propri�t�.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Arc p = property as Arc;
			p.arcType       = this.arcType;
			p.startingAngle = this.startingAngle;
			p.endingAngle   = this.endingAngle;
		}

		// Compare deux propri�t�s.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Arc p = property as Arc;
			if ( p.arcType       != this.arcType       )  return false;
			if ( p.startingAngle != this.startingAngle )  return false;
			if ( p.endingAngle   != this.endingAngle   )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override Panels.Abstract CreatePanel(Document document)
		{
			Panels.Abstract.StaticDocument = document;
			return new Panels.Arc(document);
		}


		#region Serialization
		// S�rialise la propri�t�.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("ArcType", this.arcType);
			if ( this.arcType != ArcType.Full )
			{
				info.AddValue("StartingAngle", this.startingAngle);
				info.AddValue("EndingAngle", this.endingAngle);
			}
		}

		// Constructeur qui d�s�rialise la propri�t�.
		protected Arc(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.arcType = (ArcType) info.GetValue("ArcType", typeof(ArcType));
			if ( this.arcType != ArcType.Full )
			{
				this.startingAngle = info.GetDouble("StartingAngle");
				this.endingAngle   = info.GetDouble("EndingAngle");
			}
		}
		#endregion

	
		protected ArcType				arcType;
		protected double				startingAngle;
		protected double				endingAngle;
	}
}
