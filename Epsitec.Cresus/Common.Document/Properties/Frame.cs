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
		White          = 20,		// bord blanc
		Shadow         = 21,		// ombre
		WhiteAndSnadow = 22,		// bord blanc et ombre
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
			this.marginWidth = 0.0;
			this.shadowSize = 0.0;
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
				value = System.Math.Max(value, 0.0);
				value = System.Math.Min(value, 10.0);

				if (this.frameWidth != value)
				{
					this.NotifyBefore();
					this.frameWidth = value;
					this.NotifyAfter();
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
				value = System.Math.Min(value, 20.0);

				if (this.marginWidth != value)
				{
					this.NotifyBefore();
					this.marginWidth = value;
					this.NotifyAfter();
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
				value = System.Math.Min(value, 10.0);

				if (this.shadowSize != value)
				{
					this.NotifyBefore();
					this.shadowSize = value;
					this.NotifyAfter();
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
				case  2:  type = FrameType.White;           break;
				case  3:  type = FrameType.Shadow;          break;
				case  4:  type = FrameType.WhiteAndSnadow;  break;
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
				case FrameType.White:           name = Res.Strings.Property.Frame.White;           break;
				case FrameType.Shadow:          name = Res.Strings.Property.Frame.Shadow;          break;
				case FrameType.WhiteAndSnadow:  name = Res.Strings.Property.Frame.WhiteAndShadow;  break;
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
				case FrameType.White:           return "FrameWhite";
				case FrameType.Shadow:          return "FrameShadow";
				case FrameType.WhiteAndSnadow:  return "FrameWhiteAndShadow";
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
			p.marginWidth = this.marginWidth;
			p.shadowSize = this.shadowSize;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			Frame p = property as Frame;
			if (p.frameType != this.frameType)  return false;
			if ( p.frameWidth != this.frameWidth)  return false;
			if ( p.marginWidth != this.marginWidth )  return false;
			if ( p.shadowSize != this.shadowSize )  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			Panels.Abstract.StaticDocument = document;
			return new Panels.Frame(document);
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise la propriété.
			base.GetObjectData(info, context);

			info.AddValue ("FrameType", this.frameType, typeof (FrameType));
			info.AddValue ("FrameWidth", this.frameWidth, typeof (double));
			info.AddValue ("MarginWidth", this.marginWidth, typeof (double));
			info.AddValue ("ShadowSize", this.shadowSize, typeof (double));
		}

		protected Frame(SerializationInfo info, StreamingContext context)
			: base (info, context)
		{
			//	Constructeur qui désérialise la propriété.
			this.frameType = (FrameType) info.GetValue ("FrameType", typeof (FrameType));
			this.frameWidth = (double) info.GetValue ("FrameWidth", typeof (double));
			this.marginWidth = (double) info.GetValue ("MarginWidth", typeof (double));
			this.shadowSize = (double) info.GetValue ("ShadowSize", typeof (double));
		}
		#endregion


		protected FrameType				frameType;
		protected double				frameWidth;
		protected double				marginWidth;
		protected double				shadowSize;
	}
}
