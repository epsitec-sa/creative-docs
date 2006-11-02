//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.BinaryType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>BinaryType</c> class describes data stored as arrays of <c>byte</c>.
	/// </summary>
	public class BinaryType : AbstractType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BinaryType"/> class.
		/// </summary>
		public BinaryType()
			: base ("Binary")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BinaryType"/> class.
		/// </summary>
		/// <param name="caption">The type caption.</param>
		public BinaryType(Caption caption)
			: base (caption)
		{
		}


		#region ISystemType Members

		/// <summary>
		/// Gets the system type described by this object.
		/// </summary>
		/// <value>The system type described by this object.</value>
		public override System.Type				SystemType
		{
			get
			{
				return typeof (byte[]);
			}
		}
		
		#endregion
		
		#region IDataConstraint Members

		/// <summary>
		/// Determines whether the specified value is valid according to the
		/// constraint.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is valid; otherwise, <c>false</c>.
		/// </returns>
		public override bool IsValidValue(object value)
		{
			if (this.IsNullValue (value))
			{
				return this.IsNullable;
			}

			if (value is byte[])
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		public static BinaryType Default
		{
			get
			{
				TypeRosetta.InitializeKnownTypes ();

				if (BinaryType.defaultValue == null)
				{
					//	TODO: fix DRUID

					BinaryType.defaultValue = (BinaryType) TypeRosetta.CreateTypeObject (Support.Druid.Parse ("[xxxx]"));
				}

				return BinaryType.defaultValue;
			}
		}

		private static BinaryType defaultValue;
	}
}
