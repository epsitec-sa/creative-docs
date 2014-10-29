//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Data
{
	public struct Timestamp : System.IEquatable<Timestamp>, System.IComparable<Timestamp>
	{
		public Timestamp(System.DateTime date, int position)
		{
			this.date     = date;
			this.position = position;
		}

		public Timestamp(System.Xml.XmlReader reader)
		{
			this.date     = System.DateTime.MinValue;
			this.position = 0;

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Text)
				{
					var t = Timestamp.Parse (reader.Value);
					this.date     = t.date;
					this.position = t.position;
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}


		public System.DateTime Date
		{
			get
			{
				return this.date;
			}
		}

		public int Position
		{
			get
			{
				return this.position;
			}
		}

		public Timestamp JustBefore
		{
			//	Retourne un timestamp un chouia avant.
			get
			{
				var before = this.date.AddTicks (-1);
				return new Timestamp (before, int.MaxValue);
			}
		}

		public static Timestamp Now
		{
			get
			{
				var now = System.DateTime.Now;

				//	Il faut absolument forcer l'heure à 00:00:00 !
				return new Timestamp (new System.DateTime (now.Year, now.Month, now.Day), 0);
			}
		}

		public static Timestamp MinValue
		{
			get
			{
				return new Timestamp (System.DateTime.MinValue, int.MinValue);
			}
		}

		public static Timestamp MaxValue
		{
			get
			{
				return new Timestamp (System.DateTime.MaxValue, int.MaxValue);
			}
		}

		public static Timestamp FromDate(int year, int month, int day, int position = 0)
		{
			return new Timestamp (new System.DateTime (year, month, day), position);
		}


		#region IComparable<Timestamp> Members
		public int CompareTo(Timestamp other)
		{
			int result = this.date.CompareTo (other.date);

			if (result == 0)
			{
				result = this.position.CompareTo (other.position);
			}

			return result;
		}
		#endregion

		#region IEquatable<Timestamp> Members
		public bool Equals(Timestamp other)
		{
			return this.date     == other.date
				&& this.position == other.position;
		}
		#endregion


		public override int GetHashCode()
		{
			return this.date.GetHashCode () ^ this.position;
		}

		public override bool Equals(object obj)
		{
			if (obj is Timestamp)
			{
				return this.Equals ((Timestamp) obj);
			}
			else
			{
				return false;
			}
		}

		public static bool operator==(Timestamp t1, Timestamp t2)
		{
			return t1.CompareTo (t2) == 0;
		}
		public static bool operator!=(Timestamp t1, Timestamp t2)
		{
			return t1.CompareTo (t2) != 0;
		}
		public static bool operator<(Timestamp t1, Timestamp t2)
		{
			return t1.CompareTo (t2) < 0;
		}
		public static bool operator>(Timestamp t1, Timestamp t2)
		{
			return t1.CompareTo (t2) > 0;
		}
		public static bool operator<=(Timestamp t1, Timestamp t2)
		{
			return t1.CompareTo (t2) <= 0;
		}
		public static bool operator>=(Timestamp t1, Timestamp t2)
		{
			return t1.CompareTo (t2) >= 0;
		}

		public override string ToString()
		{
			return string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0}-{1:00}-{2:00}-{3}", this.date.Year, this.date.Month, this.date.Day, this.position);
		}


		public void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteElementString ("Timestamp", this.ToString ());
		}


		public static Timestamp Parse(string text)
		{
			var args = text.Split ('-');

			if (args.Length != 4)
			{
				throw new System.FormatException ("Invalid date format");
			}

			var year  = int.Parse (args[0], System.Globalization.CultureInfo.InvariantCulture);
			var month = int.Parse (args[1], System.Globalization.CultureInfo.InvariantCulture);
			var day   = int.Parse (args[2], System.Globalization.CultureInfo.InvariantCulture);
			var pos   = int.Parse (args[3], System.Globalization.CultureInfo.InvariantCulture);

			return new Timestamp (new System.DateTime (year, month, day), pos);
		}


		private readonly System.DateTime		date;
		private readonly int					position;
	}
}
