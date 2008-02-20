//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization.IO
{
	public abstract class AbstractReader
	{
		protected AbstractReader()
		{
		}

		public abstract void BeginStorageBundle(out int rootId, out int externalCount, out int typeCount, out int objectCount);
		public abstract void EndStorageBundle();

		public abstract string ReadExternalReference();
		public abstract string ReadTypeDefinition(int id);
		public abstract int ReadObjectDefinition(int id);

		public abstract void BeginObject(int id, DependencyObject obj);
		public abstract bool ReadObjectFieldValue(DependencyObject obj, out string field, out string value);
		public abstract void EndObject(int id, DependencyObject obj);
	}
}
