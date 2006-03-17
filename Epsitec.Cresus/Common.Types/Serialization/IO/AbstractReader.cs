//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
	}
}
