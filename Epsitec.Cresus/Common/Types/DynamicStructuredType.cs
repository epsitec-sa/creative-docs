//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.DynamicStructuredType))]

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

		/// <summary>
		/// Gets the type code for the type.
		/// </summary>
		/// <value>The type code.</value>
		public override TypeCode TypeCode
		{
			get
			{
				return TypeCode.Dynamic;
			}
		}

		#region IStructuredType Members

		public StructuredTypeField GetField(string fieldId)
		{
			object value = this.data.GetValue (fieldId);

			if (UnknownValue.IsUnknownValue (value))
			{
				return null;
			}
			
			object typeObject = TypeRosetta.GetTypeObjectFromValue (value);
			INamedType namedType = TypeRosetta.GetNamedTypeFromTypeObject (typeObject);

			return new StructuredTypeField (fieldId, namedType);
		}

		public IEnumerable<string> GetFieldIds()
		{
			return this.data.GetValueIds ();
		}

		public StructuredTypeClass GetClass()
		{
			return StructuredTypeClass.None;
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
			if (this.IsNullValue (value))
			{
				return this.IsNullable;
			}

			StructuredData data = value as StructuredData;

			return (data != null) && (data.StructuredType == this);
		}

		private IStructuredData data;
	}
}
