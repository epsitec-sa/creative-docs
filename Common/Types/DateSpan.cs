/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>DateSpan</c> structure represents a date interval.
    /// </summary>
    [System.Serializable]
    [System.ComponentModel.TypeConverter(typeof(DateSpan.Converter))]
    public struct DateSpan : System.IEquatable<DateSpan>
    {
        public DateSpan(int days)
        {
            this.days = days;
            this.months = 0;
            this.years = 0;
        }

        public DateSpan(int days, int months, int years)
        {
            this.days = days;
            this.months = months;
            this.years = years;
        }

        public int Days
        {
            get { return this.days; }
        }

        public int Months
        {
            get { return this.months; }
        }

        public int Years
        {
            get { return this.years; }
        }

        public static readonly DateSpan Zero = new DateSpan(0);

        public override string ToString()
        {
            if (this.years == 0)
            {
                if (this.months == 0)
                {
                    return string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "{0}",
                        this.days
                    );
                }
                else
                {
                    return string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "{0}.{1}",
                        this.days,
                        this.months
                    );
                }
            }
            else
            {
                return string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "{0}.{1}.{2}",
                    this.days,
                    this.months,
                    this.years
                );
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is DateSpan)
            {
                return this.Equals((DateSpan)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.days ^ this.months * 31 ^ this.years * 366;
        }

        #region IEquatable<DateStep> Members

        public bool Equals(DateSpan other)
        {
            return (this.days == other.days)
                && (this.months == other.months)
                && (this.years == other.years);
        }

        #endregion

        public static bool operator ==(DateSpan a, DateSpan b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(DateSpan a, DateSpan b)
        {
            return !a.Equals(b);
        }

        public static DateSpan Parse(string value)
        {
            string[] args = value.Split('.');

            switch (args.Length)
            {
                case 1:
                    return new DateSpan(
                        int.Parse(
                            args[0],
                            System.Globalization.NumberStyles.Integer,
                            System.Globalization.CultureInfo.InvariantCulture
                        )
                    );
                case 2:
                    return new DateSpan(
                        int.Parse(
                            args[0],
                            System.Globalization.NumberStyles.Integer,
                            System.Globalization.CultureInfo.InvariantCulture
                        ),
                        /* */int.Parse(
                            args[1],
                            System.Globalization.NumberStyles.Integer,
                            System.Globalization.CultureInfo.InvariantCulture
                        ),
                        0
                    );
                case 3:
                    return new DateSpan(
                        int.Parse(
                            args[0],
                            System.Globalization.NumberStyles.Integer,
                            System.Globalization.CultureInfo.InvariantCulture
                        ),
                        /* */int.Parse(
                            args[1],
                            System.Globalization.NumberStyles.Integer,
                            System.Globalization.CultureInfo.InvariantCulture
                        ),
                        /* */int.Parse(
                            args[2],
                            System.Globalization.NumberStyles.Integer,
                            System.Globalization.CultureInfo.InvariantCulture
                        )
                    );
            }

            throw new System.FormatException(string.Format("Invalid date step: '{0}'", value));
        }

        public class Converter : AbstractStringConverter
        {
            public override object ParseString(
                string value,
                System.Globalization.CultureInfo culture
            )
            {
                return DateSpan.Parse(value);
            }

            public override string ToString(object value, System.Globalization.CultureInfo culture)
            {
                DateSpan that = (DateSpan)value;
                return that.ToString();
            }
        }

        private int days;
        private int months;
        private int years;
    }
}
