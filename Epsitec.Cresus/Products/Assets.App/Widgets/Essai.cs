using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	class Essai
	{
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
					if (this.events[i].CompareWith (e) < 1)
					{
						index = i;
						break;
					}
				}

				this.events.Insert (index, e);
			}

			public DataEvent GetEvent(DateTime date, int rank)
			{
				return this.events.Where (x => x.Date == date && x.Rank == rank).FirstOrDefault ();
			}

			//	Retourne la propriété définie à la date exacte.
			public AbstractDataProperty GetSingleProperty(DateTime date, int rank, int id)
			{
				var e = this.GetEvent (date, rank);

				if (e != null)
				{
					return e.GetProperty (id);
				}

				return null;
			}

			//	Retourne la propriété définie à la date exacte ou antérieurement.
			public AbstractDataProperty GetSyntheticProperty(DateTime date, int rank, int id)
			{
				var p = this.GetSingleProperty (date, rank, id);

				if (p != null)
				{
					return p;
				}

				// Implémentation totalement naïve qui cherche depuis la date donnée
				// en remontant dans le passé.
				var sorted = this.events.OrderBy (x => x.Rank).OrderBy (x => x.Date).Reverse ();

				foreach (var e in sorted)
				{
					if (date >= e.Date)  // TODO: tenir compte du rank
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
			public DataEvent(int id, DateTime date, int rank)
			{
				this.Id   = id;
				this.Date = date;
				this.Rank = rank;

				this.properties = new List<AbstractDataProperty> ();
			}

			public readonly int Id;
			public readonly DateTime Date;
			public readonly int Rank;

			public int CompareWith(DataEvent that)
			{
				if (this.Date < that.Date)
				{
					return -1;
				}
				else if (this.Date > that.Date)
				{
					return 1;
				}
				else
				{
					if (this.Rank < that.Rank)
					{
						return -1;
					}
					else if (this.Rank > that.Rank)
					{
						return 1;
					}
					else
					{
						return 0;
					}
				}
			}

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
	}
}
