using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe Name repr�sente une propri�t� d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Name : Abstract
	{
		public Name(Document document, Type type) : base(document, type)
		{
		}

		protected override void Initialise()
		{
			this.stringValue = "";
		}

		public string String
		{
			get
			{
				return this.stringValue;
			}
			
			set
			{
				value = value.Trim();  // enl�ve les espaces superflus avant et apr�s

				if ( this.stringValue != value )
				{
					this.NotifyBefore();
					this.stringValue = value;
					if ( value != "" )
					{
						this.document.Modifier.NamesExist = true;
					}
					this.NotifyAfter();
				}
			}
		}

		// Effectue une copie de la propri�t�.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Name p = property as Name;
			p.stringValue = this.stringValue;
		}

		// Compare deux propri�t�s.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Name p = property as Name;
			if ( p.stringValue != this.stringValue )  return false;

			return true;
		}

		// Cr�e le panneau permettant d'�diter la propri�t�.
		public override Panels.Abstract CreatePanel(Document document)
		{
			Panels.Abstract.StaticDocument = document;
			return new Panels.Name(document);
		}


		#region Serialization
		// S�rialise la propri�t�.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("StringValue", this.stringValue);
		}

		// Constructeur qui d�s�rialise la propri�t�.
		protected Name(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.stringValue = info.GetString("StringValue");
		}
		#endregion

	
		protected string			stringValue;
	}
}
