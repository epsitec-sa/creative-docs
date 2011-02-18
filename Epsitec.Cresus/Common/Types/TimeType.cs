//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.TimeType))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>TimeType</c> class defines a <c>Time</c> based type.
	/// </summary>
	public sealed class TimeType : AbstractDateTimeType
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TimeType"/> class.
		/// </summary>
		public TimeType()
			: this ("Time")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeType"/> class.
		/// </summary>
		/// <param name="name">The type name.</param>
		public TimeType(string name)
			: base (name)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeType"/> class.
		/// </summary>
		/// <param name="caption">The type caption.</param>
		public TimeType(Caption caption)
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
				return TypeCode.Time;
			}
		}



		/// <summary>
		/// Gets the default <c>TimeType</c>.
		/// </summary>
		/// <value>The default <c>TimeType</c>.</value>
		public static TimeType Default
		{
			get
			{
				TypeRosetta.InitializeKnownTypes ();

				if (TimeType.defaultValue == null)
				{
					TimeType.defaultValue = (TimeType) TypeRosetta.CreateTypeObject (Support.Druid.Parse ("[100I]"));
				}

				return TimeType.defaultValue;
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
				return typeof (Time);
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

			if ((value.GetType () == typeof (Time)) &&
				(((Time) value).IsNull))
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
			Time time = (Time) value;

			Time min = this.MinimumTime;
			Time max = this.MaximumTime;

			if (((!min.IsNull) && (time < min)) ||
				((!max.IsNull) && (time > max)))
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		
		
		private static TimeType defaultValue;
	}
}
