using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe ObjectMemory est un objet cach� qui collectionne toutes les propri�t�s.
	/// </summary>
	public class ObjectMemory : AbstractObject
	{
		public ObjectMemory(Document document, AbstractObject model) : base(document, model)
		{
			System.Diagnostics.Debug.Assert(model == null);
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
		}

		protected override bool ExistingProperty(PropertyType type)
		{
			if ( type == PropertyType.None )  return false;
			if ( type == PropertyType.Shadow )  return false;
			return true;
		}

		protected override AbstractObject CreateNewObject(Document document, AbstractObject model)
		{
			return new ObjectMemory(document, model);
		}
	}
}
