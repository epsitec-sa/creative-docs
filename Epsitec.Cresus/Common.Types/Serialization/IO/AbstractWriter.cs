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

		public virtual void BeginStorageBundle()
		{
		}
		public virtual void EndStorageBundle()
		{
		}
		
		public abstract void WriteTypeDefinition(int id, string p);
		public abstract void WriteObjectDefinition(int id, int typeId);

		public virtual void BeginObject(int id, DependencyObject obj)
		{
		}
		public virtual void EndObject(int id, DependencyObject obj)
		{
		}

		public abstract void WriteObjectFieldReference(DependencyObject obj, string name, int id);
		public abstract void WriteObjectFieldData(DependencyObject obj, string name, string value);
	}
}
