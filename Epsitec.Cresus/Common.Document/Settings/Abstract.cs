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
		public Abstract(Document document, string name)
		{
			this.document = document;
			this.name = name;
			this.conditionName = "";
			this.conditionState = false;
		}

		// Nom logique.
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		// Texte explicatif.
		public string Text
		{
			get
			{
				return this.text;
			}
		}

		// Nom de la condition.
		public string ConditionName
		{
			get
			{
				return this.conditionName;
			}
		}

		// Etat de la condition.
		public bool ConditionState
		{
			get
			{
				return this.conditionState;
			}
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
		protected string						conditionName;
		protected bool							conditionState;
	}
}
