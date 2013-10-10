//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	class Essai
	{
		public static void Test1()
		{
			var objets = new List<DataObject> ();
			int objectId = 0;

			{
				var o = new DataObject (objectId++);
				objets.Add (o);

				var e = new DataEvent (1, new Timestamp (new System.DateTime (2013, 1, 1), 0));
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty    (1, 0));
				e.Properties.Add (new DataStringProperty (2, "Immobilisations"));
			}

			{
				var o = new DataObject (objectId++);
				objets.Add (o);

				var e = new DataEvent (1, new Timestamp (new System.DateTime (2013, 1, 1), 0));
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty    (1, 1));
				e.Properties.Add (new DataStringProperty (2, "1"));
				e.Properties.Add (new DataStringProperty (3, "Bâtiments"));
			}

			{
				var o = new DataObject (objectId++);
				objets.Add (o);

				var e = new DataEvent (1, new Timestamp (new System.DateTime (2013, 1, 1), 0));
				o.AddEvent (e);
				e.Properties.Add (new DataIntProperty    (1, 2));
				e.Properties.Add (new DataStringProperty (2, "1.1"));
				e.Properties.Add (new DataStringProperty (3, "Immeubles"));
			}

			{
				var o = new DataObject (objectId++);
				objets.Add (o);

				{
					var e = new DataEvent (1, new Timestamp (new System.DateTime (2013, 1, 1), 0));
					o.AddEvent (e);
					e.Properties.Add (new DataIntProperty     (1, 3));
					e.Properties.Add (new DataStringProperty  (2, "1.1.1"));
					e.Properties.Add (new DataStringProperty  (3, "Centre administratif"));
					e.Properties.Add (new DataDecimalProperty (4, 2450000.0m));
					e.Properties.Add (new DataStringProperty  (6, "Paul"));
				}

				{
					var e = new DataEvent (1, new Timestamp (new System.DateTime (2013, 3, 1), 0));
					o.AddEvent (e);
					e.Properties.Add (new DataDecimalProperty (5, 4000000.0m));
					e.Properties.Add (new DataStringProperty  (6, "René"));
				}

				{
					var e = new DataEvent (1, new Timestamp (new System.DateTime (2013, 2, 1), 0));
					o.AddEvent (e);
					e.Properties.Add (new DataDecimalProperty (5, 3000000.0m));
				}

				var e1 = o.GetEvent (new Timestamp (new System.DateTime (2013, 1, 1), 0));
				var e2 = o.GetEvent (new Timestamp (new System.DateTime (2013, 2, 1), 0));
				var e3 = o.GetEvent (new Timestamp (new System.DateTime (2013, 3, 1), 0));

				System.Diagnostics.Debug.Assert (e1.Properties.Count == 5);
				System.Diagnostics.Debug.Assert (e2.Properties.Count == 1);
				System.Diagnostics.Debug.Assert (e3.Properties.Count == 2);

				var p11 = o.GetSingleProperty (new Timestamp (new System.DateTime (2013, 1, 1), 0), 1) as DataIntProperty;
				System.Diagnostics.Debug.Assert (p11 != null && p11.Value == 3);

				var p21 = o.GetSingleProperty (new Timestamp (new System.DateTime (2013, 2, 1), 0), 1) as DataIntProperty;
				System.Diagnostics.Debug.Assert (p21 == null);

				var p25 = o.GetSingleProperty (new Timestamp (new System.DateTime (2013, 2, 1), 0), 5) as DataDecimalProperty;
				System.Diagnostics.Debug.Assert (p25 != null && p25.Value == 3000000.0m);

				var ps1 = o.GetSyntheticProperty (new Timestamp (new System.DateTime (2013, 1, 15), 0), 4) as DataDecimalProperty;
				System.Diagnostics.Debug.Assert (ps1 != null && ps1.Value == 2450000.0m);

				var ps2 = o.GetSyntheticProperty (new Timestamp (new System.DateTime (2013, 2, 15), 0), 5) as DataDecimalProperty;
				System.Diagnostics.Debug.Assert (ps2 != null && ps2.Value == 3000000.0m);

				var ps3 = o.GetSyntheticProperty (new Timestamp (new System.DateTime (2013, 3, 15), 0), 5) as DataDecimalProperty;
				System.Diagnostics.Debug.Assert (ps3 != null && ps3.Value == 4000000.0m);
			}
		}


		public class DataObject
		{
			public DataObject(int id)
			{
				this.Id = id;

				this.events = new List<DataEvent> ();
			}

			public readonly int Id;

			public List<DataEvent> Events
			{
				get
				{
					return this.events;
				}
			}

			public void AddEvent(DataEvent e)
			{
				int index = 0;

				for (int i=0; i<this.events.Count; i++)
				{
					if (this.events[i].Timestamp < e.Timestamp)
					{
						index = i;
						break;
					}
				}

				this.events.Insert (index, e);
			}

			public DataEvent GetEvent(Timestamp timestamp)
			{
				return this.events.Where (x => x.Timestamp == timestamp).FirstOrDefault ();
			}

			//	Retourne la propriété définie à la date exacte.
			public AbstractDataProperty GetSingleProperty(Timestamp timestamp, int id)
			{
				var e = this.GetEvent (timestamp);

				if (e != null)
				{
					return e.GetProperty (id);
				}

				return null;
			}

			//	Retourne la propriété définie à la date exacte ou antérieurement.
			public AbstractDataProperty GetSyntheticProperty(Timestamp timestamp, int id)
			{
				var p = this.GetSingleProperty (timestamp, id);

				if (p != null)
				{
					return p;
				}

				// Implémentation totalement naïve qui cherche depuis la date donnée
				// en remontant dans le passé.
				foreach (var e in this.events)
				{
					if (timestamp >= e.Timestamp)
					{
						p = e.GetProperty (id);

						if (p != null)
						{
							return p;
						}
					}
				}

				return null;
			}

			private readonly List<DataEvent> events;
		}


		public class DataEvent
		{
			public DataEvent(int id, Timestamp timestamp)
			{
				this.Id   = id;
				this.Timestamp = timestamp;

				this.properties = new List<AbstractDataProperty> ();
			}

			public readonly int Id;
			public readonly Timestamp Timestamp;

			public List<AbstractDataProperty> Properties
			{
				get
				{
					return this.properties;
				}
			}

			public AbstractDataProperty GetProperty(int id)
			{
				return this.properties.Where (x => x.Id == id).FirstOrDefault ();
			}

			private readonly List<AbstractDataProperty> properties;
		}


		public class AbstractDataProperty
		{
			public AbstractDataProperty(int id)
			{
				this.Id = id;
			}

			public readonly int Id;
		}

		public class DataStringProperty : AbstractDataProperty
		{
			public DataStringProperty(int id, string value)
				: base (id)
			{
				this.Value = value;
			}

			public readonly string Value;
		}

		public class DataIntProperty : AbstractDataProperty
		{
			public DataIntProperty(int id, int value)
				: base (id)
			{
				this.Value = value;
			}

			public readonly int Value;
		}

		public class DataDecimalProperty : AbstractDataProperty
		{
			public DataDecimalProperty(int id, decimal value)
				: base (id)
			{
				this.Value = value;
			}

			public readonly decimal Value;
		}

		public class DataLinkProperty : AbstractDataProperty
		{
			public DataLinkProperty(int id, DataObject value)
				: base (id)
			{
				this.Value = value;
			}

			public readonly DataObject Value;
		}


		public struct Timestamp : System.IEquatable<Timestamp>, System.IComparable<Timestamp>
		{
			public Timestamp(System.DateTime date, int position)
			{
				this.date     = date;
				this.position = position;
			}

			public static Timestamp Now
			{
				get
				{
					return new Timestamp (System.DateTime.Now, 0);
				}
			}

			public static Timestamp FromDate(int year, int month, int day, int position = 0)
			{
				return new Timestamp (new System.DateTime (year, month, day), position);
			}

			#region IComparable<EqoTimestamp> Members
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

			#region IEquatable<EqoTimestamp> Members
			public bool Equals(Timestamp other)
			{
				return this.date == other.date
					&& this.position == other.position;
			}
			#endregion


			public override int GetHashCode()
			{
				return this.date.GetHashCode ();
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
}
