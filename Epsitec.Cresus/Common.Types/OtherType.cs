//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.OtherType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>OtherType</c> class describes a generic system type which does
	/// not fit into any of the predefined type categories.
	/// </summary>
	public sealed class OtherType : AbstractType
	{
		public OtherType()
			: base ("Other")
		{
		}

		public OtherType(Caption caption)
			: base (caption)
		{
		}

		/// <summary>
		/// Gets the type code for the type.
		/// </summary>
		/// <value>The type code.</value>
		public override TypeCode TypeCode
		{
			get
			{
				return TypeCode.Other;
			}
		}



		#region ISystemType Members

		public override System.Type SystemType
		{
			get
			{
				string systemTypeName = AbstractType.GetSystemType (this.Caption);

				if (string.IsNullOrEmpty (systemTypeName))
				{
					return null;
				}

				System.Type systemType = AbstractType.GetSystemTypeFromSystemTypeName (systemTypeName);

				return systemType;
			}
		}

		#endregion

		public override bool IsValidValue(object value)
		{
			if (this.IsNullValue (value))
			{
				return this.IsNullable;
			}

			System.Type expectedType = this.SystemType;

			if (expectedType == null)
			{
				return false;
			}
			else
			{
				return expectedType.IsAssignableFrom (value.GetType ());
			}
		}

		public void DefineSystemType(System.Type type)
		{
			AbstractType.SetSystemType (this.Caption, type);
		}
	}
}
