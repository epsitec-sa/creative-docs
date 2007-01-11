using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	//	ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	//	sous peine de plantée lors de la désérialisation.
	public enum VolumeType
	{
		None          = 0,
		BoxClose      = 10,		// boîte fermée
		BoxOpen       = 11,		// boîte ouverte
		Pyramid       = 20,		// pyramide
		Cylinder      = 30,		// cylindre
	}

	/// <summary>
	/// La classe Volume représente une propriété d'un objet volume 3d.
	/// </summary>
	[System.Serializable()]
	public class Volume : Abstract
	{
		public Volume(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialize()
		{
			base.Initialize ();
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


		public override string SampleText
		{
			//	Donne le petit texte pour les échantillons.
			get
			{
				return Volume.GetName(this.volumeType);
			}
		}

		public override void PutStyleBrief(System.Text.StringBuilder builder)
		{
			//	Construit le texte résumé d'un style pour une propriété.
			this.PutStyleBriefPrefix(builder);
			builder.Append(this.SampleText);
			this.PutStyleBriefPostfix(builder);
		}

		public static VolumeType ConvType(int index)
		{
			//	Cherche le type correspondant à un index donné.
			//	Ceci détermine l'ordre dans le TextFieldCombo du panneau.
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

		public static int ConvType(VolumeType type)
		{
			//	Cherche le rang d'un type donné.
			for ( int i=0 ; i<100 ; i++ )
			{
				VolumeType t = Volume.ConvType(i);
				if ( t == VolumeType.None )  break;
				if ( t == type )  return i;
			}
			return -1;
		}

		public static string GetName(VolumeType type)
		{
			//	Retourne le nom d'un type donné.
			string name = "";
			switch ( type )
			{
				case VolumeType.BoxClose:  name = Res.Strings.Property.Volume.BoxClose;  break;
				case VolumeType.BoxOpen:   name = Res.Strings.Property.Volume.BoxOpen;   break;
				case VolumeType.Pyramid:   name = Res.Strings.Property.Volume.Pyramid;   break;
				case VolumeType.Cylinder:  name = Res.Strings.Property.Volume.Cylinder;  break;
			}
			return name;
		}

		public static string GetIconText(VolumeType type)
		{
			//	Retourne l'icône pour un type donné.
			switch ( type )
			{
				case VolumeType.BoxClose:  return "VolumeBoxClose";
				case VolumeType.BoxOpen:   return "VolumeBoxOpen";
				case VolumeType.Pyramid:   return "VolumePyramid";
				case VolumeType.Cylinder:  return "VolumeCylinder";
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
			switch ( this.volumeType )
			{
				case VolumeType.BoxClose:
				case VolumeType.BoxOpen:
					break;

				case VolumeType.Cylinder:
					break;
			}
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
		
		public override Point GetHandlePosition(Objects.Abstract obj, int rank)
		{
			//	Retourne la position d'une poignée.
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

		public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			//	Modifie la position d'une poignée.
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
		
		
		public override void CopyTo(Abstract property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			Volume p = property as Volume;
			p.volumeType  = this.volumeType;
			p.rapport = this.rapport;
			p.angleLeft = this.angleLeft;
			p.angleRight = this.angleRight;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			Volume p = property as Volume;
			if ( p.volumeType != this.volumeType )  return false;
			if ( p.rapport != this.rapport )  return false;
			if ( p.angleLeft != this.angleLeft )  return false;
			if ( p.angleRight != this.angleRight )  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			Panels.Abstract.StaticDocument = document;
			return new Panels.Volume(document);
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise la propriété.
			base.GetObjectData(info, context);

			info.AddValue("VolumeType", this.volumeType, typeof(VolumeType));
			info.AddValue("Rapport", this.rapport, typeof(double));
			info.AddValue("AngleLeft", this.angleLeft, typeof(double));
			info.AddValue("AngleRight", this.angleRight, typeof(double));
		}

		protected Volume(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise la propriété.
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
