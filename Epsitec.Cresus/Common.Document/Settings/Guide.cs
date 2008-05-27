using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	//	ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	//	sous peine de plantée lors de la désérialisation.
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
	/// La classe Guide contient un repère magnétique.
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
					this.OpletQueueInsert();
					this.type = value;
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
					this.document.SetDirtySerialize(CacheBitmapChanging.None);
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
					this.OpletQueueInsert();
					this.position = value;
					this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
					this.document.SetDirtySerialize(CacheBitmapChanging.None);
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
				Size size = this.document.PageSize;
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

			set
			{
				Size size = this.document.PageSize;
				switch ( this.type )
				{
					case GuideType.VerticalLeft:
						this.Position = value;
						break;

					case GuideType.VerticalCenter:
						this.Position = value - size.Width/2;
						break;

					case GuideType.VerticalRight:
						this.Position = size.Width - value;
						break;

					case GuideType.HorizontalBottom:
						this.Position = value;
						break;

					case GuideType.HorizontalCenter:
						this.Position = value - size.Height/2;
						break;

					case GuideType.HorizontalTop:
						this.Position = size.Height - value;
						break;
				}
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
				case GuideType.HorizontalBottom:  return Res.Strings.Container.Guides.HorizontalBottom;
				case GuideType.HorizontalCenter:  return Res.Strings.Container.Guides.HorizontalCenter;
				case GuideType.HorizontalTop:     return Res.Strings.Container.Guides.HorizontalTop;
				case GuideType.VerticalLeft:      return Res.Strings.Container.Guides.VerticalLeft;
				case GuideType.VerticalCenter:    return Res.Strings.Container.Guides.VerticalCenter;
				case GuideType.VerticalRight:     return Res.Strings.Container.Guides.VerticalRight;
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


		#region OpletGuide
		protected void OpletQueueInsert()
		{
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletGuide oplet = new OpletGuide(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		protected class OpletGuide : AbstractOplet
		{
			public OpletGuide(Guide host)
			{
				this.host = host;

				this.initial = new Guide(this.host.document);
				this.host.CopyTo(this.initial);
			}

			protected void Swap()
			{
				Guide temp = new Guide(this.host.document);
				this.host.CopyTo(temp);
				this.initial.CopyTo(this.host);
				temp.CopyTo(this.initial);

				this.host.document.Notifier.NotifyGuidesChanged();
				this.host.document.Notifier.NotifyArea(this.host.document.Modifier.ActiveViewer);
			}

			public override IOplet Undo()
			{
				this.Swap();
				return this;
			}

			public override IOplet Redo()
			{
				this.Swap();
				return this;
			}

			protected Guide					host;
			protected Guide					initial;
		}
		#endregion

		
		#region Serialization
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise le repère.
			info.AddValue("Type", this.type);
			info.AddValue("Pos", this.position);
		}

		protected Guide(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui désérialise le repère.
			this.document = Document.ReadDocument;
			this.type = (GuideType) info.GetValue("Type", typeof(GuideType));
			this.position = info.GetDouble("Pos");
		}
		#endregion


		public static readonly double	Undefined = -1000000;

		protected Document				document;
		protected GuideType				type;
		protected double				position;
		protected bool					hilite;
	}
}
