using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Memory est un objet cach� qui collectionne toutes les propri�t�s.
	/// </summary>
	public class Memory : Objects.Abstract
	{
		public Memory(Document document, Objects.Abstract model) : base(document, model)
		{
			System.Diagnostics.Debug.Assert(model == null);
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.None )  return false;
			if ( type == Properties.Type.Shadow )  return false;
			return true;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Memory(document, model);
		}
	}
}
