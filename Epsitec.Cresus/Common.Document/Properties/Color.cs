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
			this.color = Drawing.Color.FromBrightness(0.0);
		}

		// Couleur de la propriété.
		public Drawing.Color ColorValue
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
			return new Panels.Color(document);
		}


		// Définition de la couleur pour l'impression.
		public bool PaintColor(Printing.PrintPort port, DrawingContext drawingContext)
		{
			if ( !this.color.IsOpaque )  return false;

			port.Color = this.color;
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
			this.color = (Drawing.Color) info.GetValue("Color", typeof(Drawing.Color));
		}
		#endregion

	
		protected Drawing.Color			color;
	}
}
