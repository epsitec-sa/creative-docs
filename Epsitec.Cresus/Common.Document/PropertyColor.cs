using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe PropertyColor repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class PropertyColor : AbstractProperty
	{
		public PropertyColor(Document document) : base(document)
		{
			this.color = Color.FromBrightness(0.0);
		}

		// Couleur de la propri�t�.
		public Color Color
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

		// Effectue une copie de la propri�t�.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyColor p = property as PropertyColor;
			p.color = this.color;
		}

		// Compare deux propri�t�s.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyColor p = property as PropertyColor;
			if ( p.color != this.color )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override AbstractPanel CreatePanel(Document document)
		{
			return new PanelColor(document);
		}


		// D�finition de la couleur pour l'impression.
		public bool PaintColor(Printing.PrintPort port, DrawingContext drawingContext)
		{
			if ( !this.color.IsOpaque )  return false;

			port.Color = this.color;
			return true;
		}


		#region Serialization
		// S�rialise la propri�t�.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Color", this.color);
		}

		// Constructeur qui d�s�rialise la propri�t�.
		protected PropertyColor(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.color = (Color) info.GetValue("Color", typeof(Color));
		}
		#endregion

	
		protected Color			color = Color.FromBrightness(0);
	}
}
