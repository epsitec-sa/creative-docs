using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	// ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	// sous peine de plant�e lors de la d�s�rialisation.
	public enum GuideType
	{
		None             = 0,
		HorizontalBottom = 10,
		HorizontalCenter = 11,
		HorizontalTop    = 12,
		VerticalLeft     = 20,
		VerticalCenter   = 21,
		VerticalRight    = 22,
	}

	/// <summary>
	/// La classe Guide contient un rep�re magn�tique.
	/// </summary>
	[System.Serializable()]
	public class Guide : ISerializable
	{
		public Guide(Document document)
		{
			this.document = document;
		}

		public GuideType Type
		{
			get
			{
				return this.type;
			}

			set
			{
				if ( this.type != value )
				{
					this.type = value;
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
					this.document.IsDirtySerialize = true;
				}
			}
		}

		public double Position
		{
			get
			{
				return this.position;
			}

			set
			{
				if ( this.position != value )
				{
					this.position = value;
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
					this.document.IsDirtySerialize = true;
				}
			}
		}

		public bool Hilite
		{
			get
			{
				return this.hilite;
			}

			set
			{
				if ( this.hilite != value )
				{
					this.hilite = value;
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
				}
			}
		}

		public bool IsHorizontal
		{
			get
			{
				return ( this.type == GuideType.HorizontalBottom ||
						 this.type == GuideType.HorizontalCenter ||
						 this.type == GuideType.HorizontalTop    );
			}
		}

		public double AbsolutePosition
		{
			get
			{
				Size size = this.document.Size;
				switch ( this.type )
				{
					case GuideType.VerticalLeft:
						return this.position;

					case GuideType.VerticalCenter:
						return size.Width/2 + this.position;

					case GuideType.VerticalRight:
						return size.Width - this.position;

					case GuideType.HorizontalBottom:
						return this.position;

					case GuideType.HorizontalCenter:
						return size.Height/2 + this.position;

					case GuideType.HorizontalTop:
						return size.Height - this.position;
				}
				return 0.0;
			}
		}

		public void CopyTo(Guide dst)
		{
			dst.type     = this.type;
			dst.position = this.position;
			dst.hilite   = this.hilite;
		}


		public static string TypeToString(GuideType type)
		{
			switch ( type )
			{
				case GuideType.HorizontalBottom:  return "Horizontal, depuis le bas";
				case GuideType.HorizontalCenter:  return "Horizontal, centr�";
				case GuideType.HorizontalTop:     return "Horizontal, depuis le haut";
				case GuideType.VerticalLeft:      return "Vertical, depuis la gauche";
				case GuideType.VerticalCenter:    return "Vertical, centr�";
				case GuideType.VerticalRight:     return "Vertical, depuis la droite";
			}
			return "?";
		}

		public static GuideType StringToType(string text)
		{
			foreach ( int value in System.Enum.GetValues(typeof(GuideType)) )
			{
				GuideType type = (GuideType)value;
				if ( text == Guide.TypeToString(type) )  return type;
			}
			return GuideType.None;
		}


		#region Serialization
		// S�rialise le rep�re.
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Type", this.type);
			info.AddValue("Pos", this.position);
		}

		// Constructeur qui d�s�rialise le rep�re.
		protected Guide(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;
			this.type = (GuideType) info.GetValue("Type", typeof(GuideType));
			this.position = info.GetDouble("Pos");
		}
		#endregion


		protected Document			document;
		protected GuideType			type;
		protected double			position;
		protected bool				hilite;
	}
}
