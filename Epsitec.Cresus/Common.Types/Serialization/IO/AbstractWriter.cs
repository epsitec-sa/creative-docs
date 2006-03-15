//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization.IO
{
	public abstract class AbstractWriter
	{
		protected AbstractWriter()
		{
		}

		public abstract void WriteTypeDefinition(int id, string p);
		public abstract void WriteObjectDefinition(int id, int typeId);
		public abstract void WriteObjectData(int id, DependencyObject obj);
	}
}
