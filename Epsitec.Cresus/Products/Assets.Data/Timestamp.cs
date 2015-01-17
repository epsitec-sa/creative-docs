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


		public System.DateTime					Date
		{
			get
			{
				return this.date;
			}
		}

		public int								Position
		{
			get
			{
				return this.position;
			}
		}

		public Timestamp						JustBefore
		{
			//	Retourne un timestamp un chouia avant.
			get
			{
				var before = this.date.AddTicks (-1);
				return new Timestamp (before, int.MaxValue);
			}
		}

		public static Timestamp					Now
		{
			get
			{
				var now = System.DateTime.Now;

				//	Il faut absolument forcer l'heure à 00:00:00 !

				return Timestamp.FromDate (
					now.Year,
					now.Month,
					now.Day,
					0);
			}
		}

		public static Timestamp					MinValue
		{
			get
			{
				return Timestamp.FromDate (
					System.DateTime.MinValue.Year,
					System.DateTime.MinValue.Month,
					System.DateTime.MinValue.Day,
					int.MinValue);
			}
		}

		public static Timestamp					MaxValue
		{
			get
			{
				//	Il ne faut surtout pas utiliser System.DateTime.MaxValue de façon brute (avec
				//	l'heure), car il faut toujours avoir 00:00:00 et non 23:59:59 !

				return Timestamp.FromDate (
					System.DateTime.MaxValue.Year,
					System.DateTime.MaxValue.Month,
					System.DateTime.MaxValue.Day,
					int.MaxValue);
			}
		}

		public static Timestamp FromDate(int year, int month, int day, int position = 0)
		{
			return new Timestamp (new System.DateTime (year, month, day), position);
		}


		#region IComparable<Timestamp> Members
		public int CompareTo(Timestamp other)
		{
			//	Voir la remarqur importante de Equals juste en dessous !

			int result = this.date.Year.CompareTo (other.date.Year);

			if (result == 0)
			{
				result = this.date.Month.CompareTo (other.date.Month);

				if (result == 0)
				{
					result = this.date.Day.CompareTo (other.date.Day);

					if (result == 0)
					{
						result = this.position.CompareTo (other.position);
					}
				}
			}

			return result;
		}
		#endregion

		#region IEquatable<Timestamp> Members
		public bool Equals(Timestamp other)
		{
			//	La comparaison ne doit pas tenir compte de l'heure. En effet, la sérialisation ne la sérialise pas.
			//	On a donc parfois un timestamp désérialisé qui n'a pas la même heure, avec l'emploi de
			//	System.DateTime.MaxValue !

			return this.date.Year  == other.date.Year
				&& this.date.Month == other.date.Month
				&& this.date.Day   == other.date.Day
				&& this.position   == other.position;
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


		public void Serialize(System.Xml.XmlWriter writer, string name)
		{
			writer.WriteElementString (name, this.ToString ());
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
