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

		protected override void Initialise()
		{
			this.color = Drawing.RichColor.FromBrightness(0.0);
		}

		// Couleur de la propriété.
		public Drawing.RichColor ColorValue
		{
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

		// Indique si une impression complexe est nécessaire.
		public override bool IsComplexPrinting
		{
			get
			{
				if ( this.color.A > 0.0 && this.color.A < 1.0 )  return true;
				return false;
			}
		}

		// Effectue une copie de la propriété.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Color p = property as Color;
			p.color = this.color;
		}

		// Compare deux propriétés.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Color p = property as Color;
			if ( p.color != this.color )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override Panels.Abstract CreatePanel(Document document)
		{
			Panels.Abstract.StaticDocument = document;
			return new Panels.Color(document);
		}


		// Modifie l'espace des couleurs.
		public override bool ChangeColorSpace(ColorSpace cs)
		{
			this.NotifyBefore();
			this.color.ColorSpace = cs;
			this.NotifyAfter();
			this.document.Notifier.NotifyPropertyChanged(this);
			return true;
		}

		// Modifie les couleurs.
		public override bool ChangeColor(double adjust, bool stroke)
		{
			this.NotifyBefore();
			this.color.ChangeBrightness(adjust);
			this.NotifyAfter();
			this.document.Notifier.NotifyPropertyChanged(this);
			return true;
		}

		
		#region Serialization
		// Sérialise la propriété.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Color", this.color);
		}

		// Constructeur qui désérialise la propriété.
		protected Color(SerializationInfo info, StreamingContext context) : base(info, context)
		{
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
