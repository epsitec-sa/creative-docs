//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DynamicStructuredType</c> class describes a data structure which is
	/// dynamic (it has no defined IStructuredType).
	/// </summary>
	public class DynamicStructuredType : INamedType, IStructuredType
	{
		public DynamicStructuredType(IStructuredData data)
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

		#region INamedType Members

		public System.Type SystemType
		{
			get
			{
				return null;
			}
		}

		#endregion

		#region INameCaption Members

		public long CaptionId
		{
			get
			{
				return -1;
			}
		}

		#endregion

		#region IName Members

		public string Name
		{
			get
			{
				return "Dynamic";
			}
		}

		#endregion

		private IStructuredData data;
	}
}
