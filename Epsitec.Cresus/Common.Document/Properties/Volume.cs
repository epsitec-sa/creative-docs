using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	// ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	// sous peine de plant�e lors de la d�s�rialisation.
	public enum VolumeType
	{
		None          = 0,
		BoxClose      = 10,		// bo�te ferm�e
		BoxOpen       = 11,		// bo�te ouverte
		Pyramid       = 20,		// pyramide
		Cylinder      = 30,		// cylindre
	}

	/// <summary>
	/// La classe Volume repr�sente une propri�t� d'un objet volume 3d.
	/// </summary>
	[System.Serializable()]
	public class Volume : Abstract
	{
		public Volume(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialise()
		{
			this.volumeType = VolumeType.BoxClose;
			this.rapport = 0.40;
			this.angleLeft = 30.0;
			this.angleRight = 20.0;
		}

		public VolumeType VolumeType
		{
			get
			{
				return this.volumeType;
			}

			set
			{
				if ( this.volumeType != value )
				{
					this.NotifyBefore();
					this.volumeType = value;
					this.AdjustType();
					this.NotifyAfter();
				}
			}
		}

		public double Rapport
		{
			get
			{
				return this.rapport;
			}

			set
			{
				value = System.Math.Max(value, 0.0);
				value = System.Math.Min(value, 1.0);

				if ( this.rapport != value )
				{
					this.NotifyBefore();
					this.rapport = value;
					this.NotifyAfter();
				}
			}
		}

		public double AngleLeft
		{
			get
			{
				return this.angleLeft;
			}

			set
			{
				value = System.Math.Max(value, 0.0);
				value = System.Math.Min(value, 90.0);

				if ( this.angleLeft != value )
				{
					this.NotifyBefore();
					this.angleLeft = value;
					this.NotifyAfter();
				}
			}
		}

		public double AngleRight
		{
			get
			{
				return this.angleRight;
			}

			set
			{
				value = System.Math.Max(value, 0.0);
				value = System.Math.Min(value, 90.0);

				if ( this.angleRight != value )
				{
					this.NotifyBefore();
					this.angleRight = value;
					this.NotifyAfter();
				}
			}
		}


		// D�termine le nom de la propri�t� dans la liste (Lister).
		public string GetListName()
		{
			return Volume.GetName(this.volumeType);
		}

		// Cherche le type correspondant � un index donn�.
		// Ceci d�termine l'ordre dans le TextFieldCombo du panneau.
		public static VolumeType ConvType(int index)
		{
			VolumeType type = VolumeType.None;
			switch ( index )
			{
				case  0:  type = VolumeType.BoxClose;  break;
				case  1:  type = VolumeType.BoxOpen;   break;
				case  2:  type = VolumeType.Pyramid;   break;
				case  3:  type = VolumeType.Cylinder;  break;
			}
			return type;
		}

		// Cherche le rang d'un type donn�.
		public static int ConvType(VolumeType type)
		{
			for ( int i=0 ; i<100 ; i++ )
			{
				VolumeType t = Volume.ConvType(i);
				if ( t == VolumeType.None )  break;
				if ( t == type )  return i;
			}
			return -1;
		}

		// Retourne le nom d'un type donn�.
		public static string GetName(VolumeType type)
		{
			string name = "";
			switch ( type )
			{
				case VolumeType.BoxClose:  name = "Bo�te ferm�e";   break;
				case VolumeType.BoxOpen:   name = "Bo�te ouverte";  break;
				case VolumeType.Pyramid:   name = "Pyramide";       break;
				case VolumeType.Cylinder:  name = "Cylindre";       break;
			}
			return name;
		}

		
		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }
		}


		// Ajuste les param�tres selon le type.
		protected void AdjustType()
		{
			switch ( this.volumeType )
			{
				case VolumeType.BoxClose:
				case VolumeType.BoxOpen:
					break;

				case VolumeType.Cylinder:
					break;
			}
		}

		// Nombre de poign�es.
		public override int TotalHandle(Objects.Abstract obj)
		{
			return 4;
		}

		// Indique si une poign�e est visible.
		public override bool IsHandleVisible(Objects.Abstract obj, int rank)
		{
			switch ( this.volumeType )
			{
				case VolumeType.BoxClose:
				case VolumeType.BoxOpen:
				case VolumeType.Pyramid:
					return (rank < 3);

				case VolumeType.Cylinder:
					return (rank < 1);
			}

			return false;
		}
		
		// Retourne la position d'une poign�e.
		public override Point GetHandlePosition(Objects.Abstract obj, int rank)
		{
			Point pos = new Point(0,0);

			switch ( this.volumeType )
			{
				case VolumeType.BoxClose:
				case VolumeType.BoxOpen:
				case VolumeType.Pyramid:
					if ( rank == 0 )  // rapport de base ?
					{
						pos = Point.Scale(obj.Handle(0).Position, obj.Handle(3).Position, this.rapport);
					}
					if ( rank == 1 )  // angle a ?
					{
						Point p14 = Point.Scale(obj.Handle(0).Position, obj.Handle(3).Position, this.rapport);
						double d114 = Point.Distance(obj.Handle(0).Position, p14);
						double a = this.angleLeft*System.Math.PI/180.0;
						pos = Point.Move(obj.Handle(0).Position, obj.Handle(2).Position, d114*System.Math.Tan(a));
					}
					if ( rank == 2 )  // angle b ?
					{
						Point p14 = Point.Scale(obj.Handle(0).Position, obj.Handle(3).Position, this.rapport);
						double d414 = Point.Distance(obj.Handle(3).Position, p14);
						double b = this.angleRight*System.Math.PI/180.0;
						pos = Point.Move(obj.Handle(3).Position, obj.Handle(1).Position, d414*System.Math.Tan(b));
					}
					break;

				case VolumeType.Cylinder:
					if ( rank == 0 )
					{
						double d21 = Point.Distance(obj.Handle(2).Position, obj.Handle(1).Position);
						Point p21 = Point.Scale(obj.Handle(2).Position, obj.Handle(1).Position, 0.5);
						Point p30 = Point.Scale(obj.Handle(3).Position, obj.Handle(0).Position, 0.5);
						pos = Point.Move(p21,p30, d21*this.rapport);
					}
					break;
			}

			return pos;
		}

		// Modifie la position d'une poign�e.
		public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			switch ( this.volumeType )
			{
				case VolumeType.BoxClose:
				case VolumeType.BoxOpen:
				case VolumeType.Pyramid:
					if ( rank == 0 )  // rapport de base ?
					{
						double d1 = Point.Distance(obj.Handle(0).Position, obj.Handle(3).Position);
						double d2 = Point.Distance(obj.Handle(0).Position, pos);
						if ( d1 == 0.0 )  this.Rapport = 0.0;
						else              this.Rapport = d2/d1;
					}
					if ( rank == 1 )  // angle a ?
					{
						Point p14 = Point.Scale(obj.Handle(0).Position, obj.Handle(3).Position, this.rapport);
						double d1 = Point.Distance(p14, obj.Handle(0).Position);
						double d2 = Point.Distance(pos, obj.Handle(0).Position);
						double a = System.Math.Atan(d2/d1);
						this.AngleLeft = a*180.0/System.Math.PI;
					}
					if ( rank == 2 )  // angle b ?
					{
						Point p14 = Point.Scale(obj.Handle(0).Position, obj.Handle(3).Position, this.rapport);
						double d1 = Point.Distance(p14, obj.Handle(3).Position);
						double d2 = Point.Distance(pos, obj.Handle(3).Position);
						double b = System.Math.Atan(d2/d1);
						this.AngleRight = b*180.0/System.Math.PI;
					}
					break;

				case VolumeType.Cylinder:
					if ( rank == 0 )
					{
						Point p21 = Point.Scale(obj.Handle(2).Position, obj.Handle(1).Position, 0.5);
						Point p30 = Point.Scale(obj.Handle(3).Position, obj.Handle(0).Position, 0.5);
						double d1 = Point.Distance(obj.Handle(2).Position, obj.Handle(1).Position);
						double d2 = Point.Distance(p21, pos);
						if ( d1 == 0.0 )  this.Rapport = 0.0;
						else              this.Rapport = d2/d1;
					}
					break;
			}

			base.SetHandlePosition(obj, rank, pos);
		}
		
		
		// Effectue une copie de la propri�t�.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Volume p = property as Volume;
			p.volumeType  = this.volumeType;
			p.rapport = this.rapport;
			p.angleLeft = this.angleLeft;
			p.angleRight = this.angleRight;
		}

		// Compare deux propri�t�s.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Volume p = property as Volume;
			if ( p.volumeType != this.volumeType )  return false;
			if ( p.rapport != this.rapport )  return false;
			if ( p.angleLeft != this.angleLeft )  return false;
			if ( p.angleRight != this.angleRight )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override Panels.Abstract CreatePanel(Document document)
		{
			return new Panels.Volume(document);
		}


		#region Serialization
		// S�rialise la propri�t�.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("VolumeType", this.volumeType, typeof(VolumeType));
			info.AddValue("Rapport", this.rapport, typeof(double));
			info.AddValue("AngleLeft", this.angleLeft, typeof(double));
			info.AddValue("AngleRight", this.angleRight, typeof(double));
		}

		// Constructeur qui d�s�rialise la propri�t�.
		protected Volume(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volumeType = (VolumeType) info.GetValue("VolumeType", typeof(VolumeType));
			this.rapport = (double) info.GetValue("Rapport", typeof(double));
			this.angleLeft = (double) info.GetValue("AngleLeft", typeof(double));
			this.angleRight = (double) info.GetValue("AngleRight", typeof(double));
		}
		#endregion

	
		protected VolumeType			volumeType;
		protected double				rapport;
		protected double				angleLeft;
		protected double				angleRight;
	}
}
