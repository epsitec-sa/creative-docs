//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.NaiveEngine
{
	public class DataObject : IGuid
	{
		public DataObject()
		{
			this.guid = Guid.NewGuid ();

			this.events = new List<DataEvent> ();
		}

		public Guid								Guid
		{
			get
			{
				return this.guid;
			}
		}

		public List<DataEvent>					Events
		{
			get
			{
				return this.events;
			}
		}

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

		public void AddEvent(DataEvent e)
		{
			int i = this.events.Where (x => x.Timestamp < e.Timestamp).Count ();
			this.events.Insert (i, e);
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
				var p = e.GetProperty (id);
				if (p != null)
				{
					p.State = PropertyState.Single;
					return p;
				}
			}

			return null;
		}

		//	Retourne la propriété définie à la date exacte ou antérieurement.
		public AbstractDataProperty GetSyntheticProperty(Timestamp timestamp, int id)
		{
			var p = this.GetSingleProperty (timestamp, id);

			if (p != null)
			{
				p.State = PropertyState.Single;
				return p;
			}

			// Implémentation totalement naïve qui cherche depuis la date donnée
			// en remontant dans le passé.
			var e = this.events
				.Where (x => x.Timestamp <= timestamp && x.GetProperty (id) != null)
				.LastOrDefault ();

			if (e != null)
			{
				p = e.GetProperty (id);
				if (p != null)
				{
					p.State = PropertyState.Synthetic;
					return p;
				}
			}

			return null;
		}


		private readonly Guid					guid;
		private readonly List<DataEvent>		events;
	}
}
