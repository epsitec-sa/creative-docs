using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Memory est un objet cach� qui collectionne toutes les propri�t�s.
	/// </summary>
	[System.Serializable()]
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

	
		#region Serialization
		// S�rialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui d�s�rialise l'objet.
		protected Memory(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion
	}
}
