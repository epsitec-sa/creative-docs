using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Abstract représente un réglage.
	/// </summary>
	[System.Serializable()]
	public abstract class Abstract : ISerializable
	{
		public Abstract(Document document)
		{
			this.document = document;
		}

		// Nom logique.
		public string Name
		{
			get
			{
				return this.name;
			}

			set
			{
				this.name = value;
			}
		}

		// Texte explicatif.
		public string Text
		{
			get
			{
				return this.text;
			}

			set
			{
				this.text = value;
			}
		}

		
		// Met à jour la valeur du réglage.
		public virtual void UpdateValue()
		{
		}


		#region Serialization
		// Sérialise le réglage.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Name", this.name);
		}

		// Constructeur qui désérialise le réglage.
		protected Abstract(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;
			this.name = info.GetString("Name");
		}
		#endregion


		protected Document						document;
		protected string						name;
		protected string						text;
	}
}
