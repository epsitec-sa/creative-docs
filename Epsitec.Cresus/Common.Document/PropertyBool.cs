using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe PropertyBool repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class PropertyBool : AbstractProperty
	{
		public PropertyBool(Document document) : base(document)
		{
		}

		// Valeur de la propri�t�.
		public bool Bool
		{
			get
			{
				return this.boolValue;
			}
			
			set
			{
				if ( this.boolValue != value )
				{
					this.NotifyBefore();
					this.boolValue = value;
					this.NotifyAfter();
				}
			}
		}

		// Indique si un changement de cette propri�t� modifie la bbox de l'objet.
		public override bool AlterBoundingBox
		{
			get { return ( this.type == PropertyType.PolyClose ); }
		}

		// Effectue une copie de la propri�t�.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyBool p = property as PropertyBool;
			p.boolValue = this.boolValue;
		}

		// Compare deux propri�t�s.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyBool p = property as PropertyBool;
			if ( p.boolValue != this.boolValue )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override AbstractPanel CreatePanel(Document document)
		{
			return new PanelBool(document);
		}


		#region Serialization
		// S�rialise la propri�t�.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("BoolValue", this.boolValue);
		}

		// Constructeur qui d�s�rialise la propri�t�.
		protected PropertyBool(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.boolValue = info.GetBoolean("BoolValue");
		}
		#endregion

	
		protected bool			boolValue = false;
	}
}
