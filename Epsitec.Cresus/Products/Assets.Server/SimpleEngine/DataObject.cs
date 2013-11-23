//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class DataObject : IGuid
	{
		public DataObject()
		{
			this.guid = Guid.NewGuid ();

			this.events = new GuidList<DataEvent> ();
		}

		#region IGuid Members
		public Guid								Guid
		{
			get
			{
				return this.guid;
			}
		}
		#endregion


		public int GetNewPosition(System.DateTime date)
		{
			var e = this.events.Where (x => x.Timestamp.Date == date).LastOrDefault ();

			if (e == null)
			{
				return 0;
			}
			else
			{
				return e.Timestamp.Position+1;
			}
		}


		public int EventsCount
		{
			get
			{
				return this.events.Count;
			}
		}

		public IEnumerable<DataEvent> Events
		{
			get
			{
				return this.events;
			}
		}

		public void AddEvent(DataEvent e)
		{
			int i = this.events.Where (x => x.Timestamp < e.Timestamp).Count ();
			this.events.Insert (i, e);
		}

		public void RemoveEvent(DataEvent e)
		{
			this.events.Remove (e);
		}

		public DataEvent GetEvent(int index)
		{
			return this.events[index];
		}

		public DataEvent GetEvent(Guid guid)
		{
			return this.events[guid];
		}

		public DataEvent GetEvent(Timestamp timestamp)
		{
			return this.events.Where (x => x.Timestamp == timestamp).FirstOrDefault ();
		}


		public AbstractDataProperty GetSingleProperty(Timestamp timestamp, ObjectField field)
		{
			//	Retourne la propriété définie à la date exacte.
			var e = this.GetEvent (timestamp);

			if (e != null)
			{
				var p = e.GetProperty (field);
				if (p != null)
				{
					p.State = PropertyState.Single;
					return p;
				}
			}

			return null;
		}

		public AbstractDataProperty GetInputProperty(ObjectField field)
		{
			//	Retourne la propriété définie lors de l'événement d'entrée.
			var e = this.events.FirstOrDefault ();  // événement d'entrée
			if (e != null)
			{
				var p = e.GetProperty (field);
				if (p != null)
				{
					p.State = PropertyState.InputValue;
					return p;
				}
			}

			return null;
		}

		public AbstractDataProperty GetSyntheticProperty(Timestamp timestamp, ObjectField field)
		{
			//	Retourne la propriété définie à la date exacte ou antérieurement.
			var p = this.GetSingleProperty (timestamp, field);

			if (p != null)
			{
				p.State = PropertyState.Single;
				return p;
			}

			// On cherche depuis la date donnée en remontant dans le passé.
			if (!DataAccessor.IsOneShotField (field))
			{
				var e = this.events
					.Where (x => x.Timestamp <= timestamp && x.GetProperty (field) != null)
					.LastOrDefault ();

				if (e != null)
				{
					p = e.GetProperty (field);
					if (p != null)
					{
						p.State = PropertyState.Synthetic;
						return p;
					}
				}
			}

			return null;
		}


		private readonly Guid					guid;
		private readonly GuidList<DataEvent>	events;
	}
}
