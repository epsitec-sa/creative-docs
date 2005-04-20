using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	// ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	// sous peine de plant�e lors de la d�s�rialisation.
	public enum SurfaceType
	{
		None          = 0,
		ParallelT     = 10,		// parall�logramme haut
		ParallelB     = 11,		// parall�logramme bas
		ParallelL     = 12,		// parall�logramme gauche
		ParallelR     = 13,		// parall�logramme droite
		TrapezeT      = 20,		// trap�ze haut
		TrapezeB      = 21,		// trap�ze bas
		TrapezeL      = 22,		// trap�ze gauche
		TrapezeR      = 23,		// trap�ze droite
		QuadriL       = 30,		// losange
		QuadriP       = 31,		// parall�logramme
		QuadriC       = 32,		// cerf-volant
		QuadriX       = 33,		// quadrilat�re quelconque
		Grid          = 40,		// grille
		Pattern       = 41,		// damier
		Ring          = 50,		// anneau
		SpiralCW      = 51,		// spirale horaire
		SpiralCCW     = 52,		// spirale anti-horaire
	}

	/// <summary>
	/// La classe Surface repr�sente une propri�t� d'un objet surface 2d.
	/// </summary>
	[System.Serializable()]
	public class Surface : Abstract
	{
		public Surface(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialise()
		{
			this.surfaceType = SurfaceType.ParallelR;

			this.factors = new double[4];
			this.factors[0] = 0.0;
			this.factors[1] = 0.5;
			this.factors[2] = 0.0;
			this.factors[3] = 0.5;

			this.scalars = new int[2];
			this.scalars[0] = 10;
			this.scalars[1] = 10;
		}

		public SurfaceType SurfaceType
		{
			get
			{
				return this.surfaceType;
			}

			set
			{
				if ( this.surfaceType != value )
				{
					this.NotifyBefore();
					this.surfaceType = value;
					this.AdjustType();
					this.NotifyAfter();
				}
			}
		}

		public double GetFactor(int rank)
		{
			return this.factors[rank];
		}

		public void SetFactor(int rank, double value)
		{
			value = System.Math.Max(value, 0.0);
			value = System.Math.Min(value, 1.0);

			if ( this.factors[rank] != value )
			{
				this.NotifyBefore();
				this.SetFactorInternal(rank, value);
				this.NotifyAfter();
			}
		}

		protected void SetFactorInternal(int rank, double value)
		{
			this.factors[rank] = value;

			if ( this.surfaceType == SurfaceType.ParallelT ||
				 this.surfaceType == SurfaceType.ParallelB ||
				 this.surfaceType == SurfaceType.ParallelL ||
				 this.surfaceType == SurfaceType.ParallelR ||
				 this.surfaceType == SurfaceType.QuadriP   )
			{
				this.factors[(rank+2)%4] = value;
			}

			if ( this.surfaceType == SurfaceType.QuadriC )
			{
				this.factors[(rank+2)%4] = 1.0-value;
			}
		}


		public int GetScalar(int rank)
		{
			return this.scalars[rank];
		}

		public void SetScalar(int rank, int value)
		{
			if ( this.scalars[rank] != value )
			{
				this.NotifyBefore();
				this.scalars[rank] = value;
				this.NotifyAfter();
			}
		}


		// D�termine le nom de la propri�t� dans la liste (Lister).
		public string GetListName()
		{
			return Surface.GetName(this.surfaceType);
		}

		// Cherche le type correspondant � un index donn�.
		// Ceci d�termine l'ordre dans le TextFieldCombo du panneau.
		public static SurfaceType ConvType(int index)
		{
			SurfaceType type = SurfaceType.None;
			switch ( index )
			{
				case  0:  type = SurfaceType.ParallelT;  break;
				case  1:  type = SurfaceType.ParallelB;  break;
				case  2:  type = SurfaceType.ParallelL;  break;
				case  3:  type = SurfaceType.ParallelR;  break;
				case  4:  type = SurfaceType.TrapezeT;   break;
				case  5:  type = SurfaceType.TrapezeB;   break;
				case  6:  type = SurfaceType.TrapezeL;   break;
				case  7:  type = SurfaceType.TrapezeR;   break;
				case  8:  type = SurfaceType.QuadriL;    break;
				case  9:  type = SurfaceType.QuadriP;    break;
				case 10:  type = SurfaceType.QuadriC;    break;
				case 11:  type = SurfaceType.QuadriX;    break;
				case 12:  type = SurfaceType.Grid;       break;
				case 13:  type = SurfaceType.Pattern;    break;
				case 14:  type = SurfaceType.Ring;       break;
				case 15:  type = SurfaceType.SpiralCW;   break;
				case 16:  type = SurfaceType.SpiralCCW;  break;
			}
			return type;
		}

		// Cherche le rang d'un type donn�.
		public static int ConvType(SurfaceType type)
		{
			for ( int i=0 ; i<100 ; i++ )
			{
				SurfaceType t = Surface.ConvType(i);
				if ( t == SurfaceType.None )  break;
				if ( t == type )  return i;
			}
			return -1;
		}

		// Retourne le nom d'un type donn�.
		public static string GetName(SurfaceType type)
		{
			string name = "";
			switch ( type )
			{
				case SurfaceType.ParallelT:  name = Res.Strings.Property.Surface.ParallelT;  break;
				case SurfaceType.ParallelB:  name = Res.Strings.Property.Surface.ParallelB;  break;
				case SurfaceType.ParallelL:  name = Res.Strings.Property.Surface.ParallelL;  break;
				case SurfaceType.ParallelR:  name = Res.Strings.Property.Surface.ParallelR;  break;
				case SurfaceType.TrapezeT:   name = Res.Strings.Property.Surface.TrapezeT;   break;
				case SurfaceType.TrapezeB:   name = Res.Strings.Property.Surface.TrapezeB;   break;
				case SurfaceType.TrapezeL:   name = Res.Strings.Property.Surface.TrapezeL;   break;
				case SurfaceType.TrapezeR:   name = Res.Strings.Property.Surface.TrapezeR;   break;
				case SurfaceType.QuadriL:    name = Res.Strings.Property.Surface.QuadriL;    break;
				case SurfaceType.QuadriP:    name = Res.Strings.Property.Surface.QuadriP;    break;
				case SurfaceType.QuadriC:    name = Res.Strings.Property.Surface.QuadriC;    break;
				case SurfaceType.QuadriX:    name = Res.Strings.Property.Surface.QuadriX;    break;
				case SurfaceType.Grid:       name = Res.Strings.Property.Surface.Grid;       break;
				case SurfaceType.Pattern:    name = Res.Strings.Property.Surface.Pattern;    break;
				case SurfaceType.Ring:       name = Res.Strings.Property.Surface.Ring;       break;
				case SurfaceType.SpiralCW:   name = Res.Strings.Property.Surface.SpiralCW;   break;
				case SurfaceType.SpiralCCW:  name = Res.Strings.Property.Surface.SpiralCCW;  break;
			}
			return name;
		}

		// Indique si un facteur est visible.
		public static bool IsVisibleFactor(SurfaceType type, int rank)
		{
			switch ( type )
			{
				case SurfaceType.ParallelT:
				case SurfaceType.ParallelB:
				case SurfaceType.ParallelL:
				case SurfaceType.ParallelR:
				case SurfaceType.TrapezeT:
				case SurfaceType.TrapezeB:
				case SurfaceType.TrapezeL:
				case SurfaceType.TrapezeR:
				case SurfaceType.QuadriL:
				case SurfaceType.QuadriP:
				case SurfaceType.QuadriC:
				case SurfaceType.QuadriX:  return true;
				case SurfaceType.Ring:  return (rank == 0);
				case SurfaceType.Grid:
				case SurfaceType.Pattern:  return (rank == 2 || rank == 3);
				case SurfaceType.SpiralCW:
				case SurfaceType.SpiralCCW:  return (rank == 2);
			}
			return false;
		}

		// Indique si un scalaire est visible.
		public static bool IsVisibleScalar(SurfaceType type, int rank)
		{
			switch ( type )
			{
				case SurfaceType.Grid:
				case SurfaceType.Pattern:  return true;
				case SurfaceType.SpiralCW:
				case SurfaceType.SpiralCCW:   return (rank == 0);
			}
			return false;
		}

		// Indique si un facteur est �ditable.
		public static bool IsEnableFactor(SurfaceType type, int rank)
		{
			switch ( type )
			{
				case SurfaceType.ParallelT:  return (rank == 0);
				case SurfaceType.ParallelB:  return (rank == 2);
				case SurfaceType.ParallelL:  return (rank == 1);
				case SurfaceType.ParallelR:  return (rank == 3);
				case SurfaceType.TrapezeT:   return (rank == 1);
				case SurfaceType.TrapezeB:   return (rank == 3);
				case SurfaceType.TrapezeL:   return (rank == 0);
				case SurfaceType.TrapezeR:   return (rank == 2);
				case SurfaceType.QuadriL:    return false;
				case SurfaceType.QuadriP:    return (rank == 0 || rank == 1);
				case SurfaceType.QuadriC:    return (rank == 0 || rank == 1);
				case SurfaceType.QuadriX:    return true;
				case SurfaceType.Ring:       return (rank == 0);
				case SurfaceType.Grid:
				case SurfaceType.Pattern:    return (rank == 2 || rank == 3);
				case SurfaceType.SpiralCW:
				case SurfaceType.SpiralCCW:  return (rank == 2);
			}
			return false;
		}

		// Indique si un scalaire est �ditable.
		public static bool IsEnableScalar(SurfaceType type, int rank)
		{
			switch ( type )
			{
				case SurfaceType.Grid:       return true;
				case SurfaceType.Pattern:    return true;
				case SurfaceType.SpiralCW:
				case SurfaceType.SpiralCCW:  return (rank == 0);
			}
			return false;
		}

		
		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return true; }
		}


		// Remet les param�tres standards.
		public void Reset()
		{
			this.NotifyBefore();
			this.AdjustType();
			this.NotifyAfter();
		}

		// Ajuste les param�tres selon le type.
		public void AdjustType()
		{
			switch ( this.surfaceType )
			{
				case SurfaceType.ParallelT:
					this.factors[0] = 0.5;
					this.factors[1] = 0.0;
					this.factors[2] = 0.5;
					this.factors[3] = 0.0;
					break;

				case SurfaceType.ParallelB:
					this.factors[0] = 0.5;
					this.factors[1] = 1.0;
					this.factors[2] = 0.5;
					this.factors[3] = 1.0;
					break;

				case SurfaceType.ParallelL:
					this.factors[0] = 1.0;
					this.factors[1] = 0.5;
					this.factors[2] = 1.0;
					this.factors[3] = 0.5;
					break;

				case SurfaceType.ParallelR:
					this.factors[0] = 0.0;
					this.factors[1] = 0.5;
					this.factors[2] = 0.0;
					this.factors[3] = 0.5;
					break;

				case SurfaceType.TrapezeT:
				case SurfaceType.TrapezeB:
				case SurfaceType.TrapezeL:
				case SurfaceType.TrapezeR:
					this.factors[0] = 0.25;
					this.factors[1] = 0.25;
					this.factors[2] = 0.25;
					this.factors[3] = 0.25;
					break;

				case SurfaceType.QuadriL:
					this.factors[0] = 0.5;
					this.factors[1] = 0.5;
					this.factors[2] = 0.5;
					this.factors[3] = 0.5;
					break;

				case SurfaceType.QuadriP:
					this.factors[0] = 0.25;
					this.factors[1] = 0.25;
					this.factors[2] = 0.25;
					this.factors[3] = 0.25;
					break;

				case SurfaceType.QuadriC:
					this.factors[0] = 0.75;
					this.factors[1] = 0.50;
					this.factors[2] = 0.25;
					this.factors[3] = 0.50;
					break;

				case SurfaceType.QuadriX:
					this.factors[0] = 0.2;
					this.factors[1] = 0.3;
					this.factors[2] = 0.4;
					this.factors[3] = 0.5;
					break;

				case SurfaceType.Ring:
					this.factors[0] = 0.25;
					this.factors[1] = 0.0;
					this.factors[2] = 0.0;
					this.factors[3] = 0.0;
					break;

				case SurfaceType.Grid:
				case SurfaceType.Pattern:
					this.scalars[0] = 10;
					this.scalars[1] = 10;
					this.factors[2] = 0.5;
					this.factors[3] = 0.5;
					break;

				case SurfaceType.SpiralCW:
				case SurfaceType.SpiralCCW:
					this.scalars[0] = 5;
					this.factors[2] = 0.5;
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
			if ( !this.document.Modifier.IsPropertiesExtended(this.type) )
			{
				return false;
			}

			switch ( this.surfaceType )
			{
				case SurfaceType.ParallelT:
				case SurfaceType.ParallelB:
					return (rank == 0 || rank == 2);

				case SurfaceType.ParallelL:
				case SurfaceType.ParallelR:
					return (rank == 1 || rank == 3);

				case SurfaceType.TrapezeT:
					return (rank == 1);
				case SurfaceType.TrapezeB:
					return (rank == 3);
				case SurfaceType.TrapezeL:
					return (rank == 0);
				case SurfaceType.TrapezeR:
					return (rank == 2);

				case SurfaceType.QuadriL:
					return false;

				case SurfaceType.QuadriP:
				case SurfaceType.QuadriC:
				case SurfaceType.QuadriX:
					return true;

				case SurfaceType.Ring:
					return (rank < 1);
			}

			return false;
		}
		
		// Retourne la position d'une poign�e.
		public override Point GetHandlePosition(Objects.Abstract obj, int rank)
		{
			Point pos = new Point(0,0);

			switch ( this.surfaceType )
			{
				case SurfaceType.ParallelT:
				case SurfaceType.ParallelB:
				case SurfaceType.ParallelL:
				case SurfaceType.ParallelR:
				case SurfaceType.TrapezeT:
				case SurfaceType.TrapezeB:
				case SurfaceType.TrapezeL:
				case SurfaceType.TrapezeR:
				case SurfaceType.QuadriL:
				case SurfaceType.QuadriP:
				case SurfaceType.QuadriC:
				case SurfaceType.QuadriX:
					if ( rank == 0 )  pos = Point.Scale(obj.Handle(0).Position, obj.Handle(2).Position, this.factors[rank]);
					if ( rank == 1 )  pos = Point.Scale(obj.Handle(2).Position, obj.Handle(1).Position, this.factors[rank]);
					if ( rank == 2 )  pos = Point.Scale(obj.Handle(1).Position, obj.Handle(3).Position, this.factors[rank]);
					if ( rank == 3 )  pos = Point.Scale(obj.Handle(3).Position, obj.Handle(0).Position, this.factors[rank]);
					break;

				case SurfaceType.Ring:
					if ( rank == 0 )
					{
						Point p21 = Point.Scale(obj.Handle(2).Position, obj.Handle(1).Position, 0.5);
						Point p30 = Point.Scale(obj.Handle(3).Position, obj.Handle(0).Position, 0.5);
						pos = Point.Scale(p21,p30, this.factors[0]);
					}
					break;
			}

			return pos;
		}

		// Modifie la position d'une poign�e.
		public override void SetHandlePosition(Objects.Abstract obj, int rank, Point pos)
		{
			switch ( this.surfaceType )
			{
				case SurfaceType.ParallelT:
				case SurfaceType.ParallelB:
				case SurfaceType.ParallelL:
				case SurfaceType.ParallelR:
				case SurfaceType.TrapezeT:
				case SurfaceType.TrapezeB:
				case SurfaceType.TrapezeL:
				case SurfaceType.TrapezeR:
				case SurfaceType.QuadriL:
				case SurfaceType.QuadriP:
				case SurfaceType.QuadriC:
				case SurfaceType.QuadriX:
					if ( rank >= 0 && rank < 4 )
					{
						int r1=0, r2=0;
						if ( rank == 0 )  { r1 = 0;  r2 = 2; }
						if ( rank == 1 )  { r1 = 2;  r2 = 1; }
						if ( rank == 2 )  { r1 = 1;  r2 = 3; }
						if ( rank == 3 )  { r1 = 3;  r2 = 0; }
						double d1 = Point.Distance(obj.Handle(r1).Position, obj.Handle(r2).Position);
						double d2 = Point.Distance(obj.Handle(r1).Position, pos);
						if ( d1 == 0.0 )  this.SetFactor(rank, 0.0);
						else              this.SetFactor(rank, d2/d1);
					}
					break;

				case SurfaceType.Ring:
					if ( rank == 0 )
					{
						Point p21 = Point.Scale(obj.Handle(2).Position, obj.Handle(1).Position, 0.5);
						Point p30 = Point.Scale(obj.Handle(3).Position, obj.Handle(0).Position, 0.5);
						double d1 = Point.Distance(p21, p30);
						double d2 = Point.Distance(p21, pos);
						if ( d1 == 0.0 )  this.SetFactor(0, 0.0);
						else              this.SetFactor(0, d2/d1);
					}
					break;
			}

			base.SetHandlePosition(obj, rank, pos);
		}
		
		
		// Effectue une copie de la propri�t�.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Surface p = property as Surface;
			p.surfaceType  = this.surfaceType;
			for ( int i=0 ; i<4 ; i++ )
			{
				p.factors[i] = this.factors[i];
			}
			for ( int i=0 ; i<2 ; i++ )
			{
				p.scalars[i] = this.scalars[i];
			}
		}

		// Compare deux propri�t�s.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Surface p = property as Surface;
			if ( p.surfaceType != this.surfaceType )  return false;
			for ( int i=0 ; i<4 ; i++ )
			{
				if ( p.factors[i] != this.factors[i] )  return false;
			}
			for ( int i=0 ; i<2 ; i++ )
			{
				if ( p.scalars[i] != this.scalars[i] )  return false;
			}

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override Panels.Abstract CreatePanel(Document document)
		{
			return new Panels.Surface(document);
		}


		#region Serialization
		// S�rialise la propri�t�.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("SurfaceType", this.surfaceType, typeof(SurfaceType));
			info.AddValue("Factors", this.factors, typeof(double[]));
			info.AddValue("Scalars", this.scalars, typeof(int[]));
		}

		// Constructeur qui d�s�rialise la propri�t�.
		protected Surface(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.surfaceType = (SurfaceType) info.GetValue("SurfaceType", typeof(SurfaceType));
			this.factors = (double[]) info.GetValue("Factors", typeof(double[]));
			this.scalars = (int[]) info.GetValue("Scalars", typeof(int[]));
		}
		#endregion

	
		protected SurfaceType			surfaceType;
		protected double[]				factors;
		protected int[]					scalars;
	}
}
