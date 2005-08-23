using Epsitec.Common.Widgets;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Properties
{
	/// <summary>
	/// La classe Name représente une propriété d'un objet graphique.
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
				value = value.Trim();  // enlève les espaces superflus avant et après

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

		// Effectue une copie de la propriété.
		public override void CopyTo(Abstract property)
		{
			base.CopyTo(property);
			Name p = property as Name;
			p.stringValue = this.stringValue;
		}

		// Compare deux propriétés.
		public override bool Compare(Abstract property)
		{
			if ( !base.Compare(property) )  return false;

			Name p = property as Name;
			if ( p.stringValue != this.stringValue )  return false;

			return true;
		}

		// Crée le panneau permettant d'éditer la propriété.
		public override Panels.Abstract CreatePanel(Document document)
		{
			Panels.Abstract.StaticDocument = document;
			return new Panels.Name(document);
		}


		#region Serialization
		// Sérialise la propriété.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("StringValue", this.stringValue);
		}

		// Constructeur qui désérialise la propriété.
		protected Name(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.stringValue = info.GetString("StringValue");
		}
		#endregion

	
		protected string			stringValue;
	}
}
