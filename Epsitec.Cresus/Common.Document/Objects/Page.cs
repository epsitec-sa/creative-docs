using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Page est la classe de l'objet graphique "page".
	/// </summary>
	[System.Serializable()]
	public class Page : Objects.Abstract
	{
		public Page(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
			this.objects = new UndoableList(this.document, UndoableListType.ObjectsInsideDocument);
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Page(document, model);
		}

		public int CurrentLayer
		{
			get
			{
				return this.currentLayer;
			}
			
			set
			{
				this.currentLayer = value;
			}
		}


		#region Serialization
		// Sérialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui désérialise l'objet.
		protected Page(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion

		
		protected int			currentLayer;
	}
}
