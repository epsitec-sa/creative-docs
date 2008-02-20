//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.DateType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DateType</c> class defines a <c>Date</c> based type.
	/// </summary>
	public sealed class DateType : AbstractDateTimeType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DateType"/> class.
		/// </summary>
		public DateType()
			: this ("Date")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DateType"/> class.
		/// </summary>
		/// <param name="name">The type name.</param>
		public DateType(string name)
			: base (name)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DateType"/> class.
		/// </summary>
		/// <param name="caption">The type caption.</param>
		public DateType(Caption caption)
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
				return TypeCode.Date;
			}
		}

		/// <summary>
		/// Gets the default <c>DateType</c>.
		/// </summary>
		/// <value>The default <c>DateType</c>.</value>
		public static DateType Default
		{
			get
			{
				TypeRosetta.InitializeKnownTypes ();

				if (DateType.defaultValue == null)
				{
					DateType.defaultValue = (DateType) TypeRosetta.CreateTypeObject (Support.Druid.Parse ("[100H]"));
				}

				return DateType.defaultValue;
			}
		}

		/// <summary>
		/// Gets the system type described by this object.
		/// </summary>
		/// <value>The system type described by this object.</value>
		public override System.Type SystemType
		{
			get
			{
				return typeof (Date);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the value is null.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified value represents the <c>null</c> value; otherwise, <c>false</c>.
		/// </returns>
		public override bool IsNullValue(object value)
		{
			if (base.IsNullValue (value))
			{
				return true;
			}

			if ((value.GetType () == typeof (Date)) &&
				((Date) value == Date.Null))
			{
				return true;
			}
			
			return false;
		}

		/// <summary>
		/// Determines whether the specified value is in a valid range.
		/// </summary>
		/// <param name="value">The value (never null and always of a valid type).</param>
		/// <returns>
		/// 	<c>true</c> if the specified value is in a valid range; otherwise, <c>false</c>.
		/// </returns>
		protected override bool IsInRange(object value)
		{
			Date date = (Date) value;

			Date min = this.MinimumDate;
			Date max = this.MaximumDate;

			if (((!min.IsNull) && (date < min)) ||
				((!max.IsNull) && (date > max)))
			{
				return false;
			}
			else
			{
				return true;
			}
		}


		private static DateType defaultValue;
	}
}
