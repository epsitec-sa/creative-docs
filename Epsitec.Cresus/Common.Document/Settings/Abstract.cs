using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Settings
{
	/// <summary>
	/// La classe Abstract repr�sente un r�glage.
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

		
		// Met � jour la valeur du r�glage.
		public virtual void UpdateValue()
		{
		}


		#region Serialization
		// S�rialise le r�glage.
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Name", this.name);
		}

		// Constructeur qui d�s�rialise le r�glage.
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
