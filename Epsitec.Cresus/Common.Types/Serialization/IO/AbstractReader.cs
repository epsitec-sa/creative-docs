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

		public abstract void BeginStorageBundle(out int rootId);
		public abstract void EndStorageBundle();
	}
}
