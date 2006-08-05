//	Copyright � 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DynamicStructuredType</c> class describes a data structure which is
	/// dynamic (it has no defined <c>IStructuredType</c>).
	/// </summary>
	public class DynamicStructuredType : AbstractType, IStructuredType
	{
		public DynamicStructuredType(IStructuredData data) : base ("DynamicStructure")
		{
			this.data = data;
		}

		#region IStructuredType Members

		public object GetFieldTypeObject(string name)
		{
			object value = this.data.GetValue (name);
			return TypeRosetta.GetTypeObjectFromValue (value);
		}

		public string[] GetFieldNames()
		{
			return this.data.GetValueNames ();
		}

		#endregion

		#region ISystemType Members

		public override System.Type SystemType
		{
			get
			{
				return null;
			}
		}

		#endregion

		public override bool IsValidValue(object value)
		{
			return false;
		}

		private IStructuredData data;
	}
}
