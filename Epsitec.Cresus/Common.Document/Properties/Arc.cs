using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	public enum ArcType
	{
		Full,
		Open,
		Close,
		Pie,
	}

	/// <summary>
	/// La classe Arc représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Arc : Abstract
	{
		public Arc(Document document, Type type) : base(document, type)
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

		// Indique si un changement de cette propriété modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }
		}


		// Nombre de poignées.
		public override int TotalHandle(Objects.Abstract obj)
		{
			return 2;
		}

		// Indique si une poignée est visible.
		public override bool IsHandleVisible(Objects.Abstract obj, int rank)
		{
			return (this.arcType != ArcType.Full);
		}
		
		// Retourne la position d'une poignée.
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

		// Modifie la position d'une poignée.
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
		
		
		// Effectue une copie de la propriété.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Arc p = property as Arc;
			p.arcType       = this.arcType;
			p.startingAngle = this.startingAngle;
			p.endingAngle   = this.endingAngle;
		}

		// Compare deux propriétés.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Arc p = property as Arc;
			if ( p.arcType       != this.arcType       )  return false;
			if ( p.startingAngle != this.startingAngle )  return false;
			if ( p.endingAngle   != this.endingAngle   )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override Panels.Abstract CreatePanel(Document document)
		{
			return new Panels.Arc(document);
		}


		#region Serialization
		// Sérialise la propriété.
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

		// Constructeur qui désérialise la propriété.
		protected Arc(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.arcType = (ArcType) info.GetValue("ArcType", typeof(ArcType));
			if ( this.arcType != ArcType.Full )
			{
				this.startingAngle = info.GetDouble("StartingAngle");
				this.endingAngle   = info.GetDouble("EndingAngle");
			}
			else
			{
				this.startingAngle =  90.0;
				this.endingAngle   = 360.0;
			}
		}
		#endregion

	
		protected ArcType				arcType;
		protected double				startingAngle;
		protected double				endingAngle;
	}
}
