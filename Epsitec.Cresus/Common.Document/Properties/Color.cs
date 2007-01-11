using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe Color représente une propriété d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Color : Abstract
	{
		public Color(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialize()
		{
			base.Initialize ();
			this.color = Drawing.RichColor.FromBrightness(0.0);
		}

		public Drawing.RichColor ColorValue
		{
			//	Couleur de la propriété.
			get
			{
				return this.color;
			}
			
			set
			{
				if ( this.color != value )
				{
					this.NotifyBefore();
					this.color = value;
					this.NotifyAfter();
				}
			}
		}

		public override bool IsComplexPrinting
		{
			//	Indique si une impression complexe est nécessaire.
			get
			{
				if ( this.color.A > 0.0 && this.color.A < 1.0 )  return true;
				return false;
			}
		}

		public override void CopyTo(Abstract property)
		{
			//	Effectue une copie de la propriété.
			base.CopyTo(property);
			Color p = property as Color;
			p.color = this.color;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propriétés.
			if ( !base.Compare(property) )  return false;

			Color p = property as Color;
			if ( p.color != this.color )  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Crée le panneau permettant d'éditer la propriété.
			Panels.Abstract.StaticDocument = document;
			return new Panels.Color(document);
		}


		public override bool ChangeColorSpace(ColorSpace cs)
		{
			//	Modifie l'espace des couleurs.
			this.NotifyBefore();
			this.color.ColorSpace = cs;
			this.NotifyAfter();
			this.document.Notifier.NotifyPropertyChanged(this);
			return true;
		}

		public override bool ChangeColor(double adjust, bool stroke)
		{
			//	Modifie les couleurs.
			this.NotifyBefore();
			this.color.ChangeBrightness(adjust);
			this.NotifyAfter();
			this.document.Notifier.NotifyPropertyChanged(this);
			return true;
		}

		
		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise la propriété.
			base.GetObjectData(info, context);

			info.AddValue("Color", this.color);
		}

		protected Color(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise la propriété.
			if ( this.document.IsRevisionGreaterOrEqual(1,0,22) )
			{
				this.color = (Drawing.RichColor) info.GetValue("Color", typeof(Drawing.RichColor));
			}
			else
			{
				Drawing.Color c = (Drawing.Color) info.GetValue("Color", typeof(Drawing.Color));
				this.color = new RichColor(c);
			}
		}
		#endregion

	
		protected Drawing.RichColor		color;
	}
}
