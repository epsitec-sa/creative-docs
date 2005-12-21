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

		public string Name
		{
			//	Nom logique.
			get
			{
				return this.name;
			}
		}

		public string Text
		{
			//	Texte explicatif.
			get
			{
				return this.text;
			}
		}

		public string ConditionName
		{
			//	Nom de la condition.
			get
			{
				return this.conditionName;
			}
		}

		public bool ConditionState
		{
			//	Etat de la condition.
			get
			{
				return this.conditionState;
			}
		}

		
		#region Serialization
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise le réglage.
			info.AddValue("Name", this.name);
		}

		protected Abstract(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui désérialise le réglage.
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
