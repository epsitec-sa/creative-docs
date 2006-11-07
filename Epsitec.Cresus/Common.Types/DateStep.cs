//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DateSpan</c> structure represents a date interval.
	/// </summary>
	[System.Serializable]
	[System.ComponentModel.TypeConverter (typeof (DateStep.Converter))]
	
	public struct DateStep : System.IEquatable<DateStep>
	{
		public DateStep(int days)
		{
			this.days = days;
			this.months = 0;
			this.years = 0;
		}
		
		public DateStep(int days, int months, int years)
		{
			this.days = days;
			this.months = months;
			this.years = years;
		}


		public int Days
		{
			get
			{
				return this.days;
			}
		}

		public int Months
		{
			get
			{
				return this.months;
			}
		}

		public int Years
		{
			get
			{
				return this.years;
			}
		}

		public override string ToString()
		{
			if (this.years == 0)
			{
				if (this.months == 0)
				{
					return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}", this.days);
				}
				else
				{
					return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", this.days, this.months);
				}
			}
			else
			{
				return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}.{2}", this.days, this.months, this.years);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is DateStep)
			{
				return this.Equals ((DateStep) obj);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return this.days ^ this.months*31 ^ this.years*366;
		}

		#region IEquatable<DateStep> Members

		public bool Equals(DateStep other)
		{
			return (this.days == other.days)
				&& (this.months == other.months)
				&& (this.years == other.years);
		}

		#endregion

		public static bool operator==(DateStep a, DateStep b)
		{
			return a.Equals (b);
		}

		public static bool operator!=(DateStep a, DateStep b)
		{
			return !a.Equals (b);
		}

		public static DateStep Parse(string value)
		{
			string[] args = value.Split ('.');

			switch (args.Length)
			{
				case 1:
					return new DateStep (int.Parse (args[0], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
				case 2:
					return new DateStep (int.Parse (args[0], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture),
						/**/			 int.Parse (args[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), 0);
				case 3:
					return new DateStep (int.Parse (args[0], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture),
						/**/			 int.Parse (args[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture),
						/**/			 int.Parse (args[2], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
			}

			throw new System.FormatException (string.Format ("Invalid date step: '{0}'", value));
		}

		public class Converter : AbstractStringConverter
		{
			public override object ParseString(string value, System.Globalization.CultureInfo culture)
			{
				return DateStep.Parse (value);
			}

			public override string ToString(object value, System.Globalization.CultureInfo culture)
			{
				DateStep that = (DateStep) value;
				return that.ToString ();
			}
		}
		
		private int days;
		private int months;
		private int years;
	}
}
