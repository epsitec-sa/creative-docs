using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe PropertyName repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class PropertyName : AbstractProperty
	{
		public PropertyName(Document document) : base(document)
		{
		}

		public string String
		{
			get
			{
				return this.stringValue;
			}
			
			set
			{
				if ( this.stringValue != value )
				{
					this.NotifyBefore();
					this.stringValue = value;
					this.NotifyAfter();
				}
			}
		}

		// D�termine le nom de la propri�t� dans la liste (Lister).
		public string GetListName()
		{
			return this.stringValue;
		}

		// Effectue une copie de la propri�t�.
		public override void CopyTo(AbstractProperty property)
		{
			base.CopyTo(property);
			PropertyName p = property as PropertyName;
			p.stringValue = this.stringValue;
		}

		// Compare deux propri�t�s.
		public override bool Compare(AbstractProperty property)
		{
			if ( !base.Compare(property) )  return false;

			PropertyName p = property as PropertyName;
			if ( p.stringValue != this.stringValue )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override AbstractPanel CreatePanel(Document document)
		{
			return new PanelName(document);
		}


		#region Serialization
		// S�rialise la propri�t�.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("StringValue", this.stringValue);
		}

		// Constructeur qui d�s�rialise la propri�t�.
		protected PropertyName(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.stringValue = info.GetString("StringValue");
		}
		#endregion

	
		protected string			stringValue = "";
	}
}
