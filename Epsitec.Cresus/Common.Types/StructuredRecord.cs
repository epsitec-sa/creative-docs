//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class StructuredRecord : IStructuredTree
	{
		public StructuredRecord(StructuredRecordType type)
		{
			this.type = type;
		}

		public StructuredRecordType StructuredRecordType
		{
			get
			{
				return this.type;
			}
		}

		public bool IsDynamicType
		{
			get
			{
				return this.type == null;
			}
		}
		
		#region IStructuredTree Members

		public object GetFieldTypeObject(string name)
		{
			if (this.IsDynamicType)
			{
				return this.GetDynamicFieldTypeObject (name);
			}
			else
			{
				return this.type.GetFieldTypeObject (name);
			}
		}
		
		public string[] GetFieldNames()
		{
			if (this.IsDynamicType)
			{
				return this.GetDynamicFieldNames ();
			}
			else
			{
				return this.type.GetFieldNames ();
			}
		}

		public string[] GetFieldPaths(string path)
		{
			if (this.IsDynamicType)
			{
				return this.GetDynamicFieldPaths (path);
			}
			else
			{
				return this.type.GetFieldPaths (path);
			}
		}

		#endregion

		protected virtual object GetDynamicFieldTypeObject(string name)
		{
			return null;
		}
		
		protected virtual string[] GetDynamicFieldNames()
		{
			return new string[0];
		}

		protected virtual string[] GetDynamicFieldPaths(string path)
		{
			return null;
		}
		
		private StructuredRecordType type;
	}
}
