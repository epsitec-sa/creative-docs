using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe ObjectPage est la classe de l'objet graphique "page".
	/// </summary>
	[System.Serializable()]
	public class ObjectPage : AbstractObject
	{
		public ObjectPage(Document document, AbstractObject model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
			this.objects = new UndoableList(this.document, UndoableListType.ObjectsInsideDocument);
		}

		protected override bool ExistingProperty(PropertyType type)
		{
			return false;
		}

		protected override AbstractObject CreateNewObject(Document document, AbstractObject model)
		{
			return new ObjectPage(document, model);
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
		protected ObjectPage(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion

		
		protected int			currentLayer;
	}
}
