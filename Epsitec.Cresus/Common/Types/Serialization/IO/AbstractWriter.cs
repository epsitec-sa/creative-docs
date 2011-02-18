//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization.IO
{
	public abstract class AbstractWriter
	{
		protected AbstractWriter()
		{
		}

		public virtual void BeginStorageBundle(int id, int externalCount, int typeCount, int objectCount)
		{
		}
		public virtual void EndStorageBundle()
		{
		}

		public abstract void WriteAttributeStrings();
		public abstract void WriteExternalReference(string name);
		public abstract void WriteTypeDefinition(int id, string name);
		public abstract void WriteObjectDefinition(int id, int typeId);

		public virtual void BeginObject(int id, DependencyObject obj)
		{
		}
		public virtual void EndObject(int id, DependencyObject obj)
		{
		}

		public abstract void WriteObjectFieldReference(DependencyObject obj, string name, int id);
		public abstract void WriteObjectFieldReferenceList(DependencyObject obj, string name, IList<int> ids);
		public abstract void WriteObjectFieldValue(DependencyObject obj, string name, string value);
	}
}
