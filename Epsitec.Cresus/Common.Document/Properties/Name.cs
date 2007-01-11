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

		protected override void Initialize()
		{
			base.Initialize ();
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

		public override void CopyTo(Abstract property)
		{
			//	Effectue une copie de la propri�t�.
			base.CopyTo(property);
			Name p = property as Name;
			p.stringValue = this.stringValue;
		}

		public override bool Compare(Abstract property)
		{
			//	Compare deux propri�t�s.
			if ( !base.Compare(property) )  return false;

			Name p = property as Name;
			if ( p.stringValue != this.stringValue )  return false;

			return true;
		}

		public override Panels.Abstract CreatePanel(Document document)
		{
			//	Cr�e le panneau permettant d'�diter la propri�t�.
			Panels.Abstract.StaticDocument = document;
			return new Panels.Name(document);
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	S�rialise la propri�t�.
			base.GetObjectData(info, context);

			info.AddValue("StringValue", this.stringValue);
		}

		protected Name(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui d�s�rialise la propri�t�.
			this.stringValue = info.GetString("StringValue");
		}
		#endregion

	
		protected string			stringValue;
	}
}
