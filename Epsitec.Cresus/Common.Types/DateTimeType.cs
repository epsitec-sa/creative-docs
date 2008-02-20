//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.DateTimeType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DateTimeType</c> class defines a <c>System.DateTime</c> based type.
	/// </summary>
	public sealed class DateTimeType : AbstractDateTimeType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DateTimeType"/> class.
		/// </summary>
		public DateTimeType()
			: this ("DateTime")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DateTimeType"/> class.
		/// </summary>
		/// <param name="name">The type name.</param>
		public DateTimeType(string name)
			: base (name)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DateTimeType"/> class.
		/// </summary>
		/// <param name="caption">The type caption.</param>
		public DateTimeType(Caption caption)
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
				return TypeCode.DateTime;
			}
		}


		/// <summary>
		/// Gets the default <c>DateTimeType</c>.
		/// </summary>
		/// <value>The default <c>DateTimeType</c>.</value>
		public static DateTimeType Default
		{
			get
			{
				TypeRosetta.InitializeKnownTypes ();

				if (DateTimeType.defaultValue == null)
				{
					DateTimeType.defaultValue = (DateTimeType) TypeRosetta.CreateTypeObject (Support.Druid.Parse ("[100J]"));
				}

				return DateTimeType.defaultValue;
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
				return typeof (System.DateTime);
			}
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
			System.DateTime dateTime = (System.DateTime) value;

			Date date = new Date (dateTime);
			Time time = new Time (dateTime);

			Date minDate = this.MinimumDate;
			Date maxDate = this.MaximumDate;

			if (((!minDate.IsNull) && (date < minDate)) ||
				((!maxDate.IsNull) && (date > maxDate)))
			{
				return false;
			}
			
			Time minTime = this.MinimumTime;
			Time maxTime = this.MaximumTime;

			if (((!minTime.IsNull) && (time < minTime)) ||
				((!maxTime.IsNull) && (time > maxTime)))
			{
				return false;
			}

			return true;
		}


		private static DateTimeType defaultValue;
	}
}
