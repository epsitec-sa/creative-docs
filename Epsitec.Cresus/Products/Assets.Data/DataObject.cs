//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.DataProperties;

namespace Epsitec.Cresus.Assets.Data
{
	public class DataObject : IGuid
	{
		public DataObject(UndoManager undoManager)
		{
			this.undoManager = undoManager;
			this.guid        = Guid.NewGuid ();

			this.events = new GuidDictionary<DataEvent> (undoManager);
		}

		public DataObject(UndoManager undoManager, Guid guid)
		{
			this.undoManager = undoManager;
			this.guid        = guid;

			this.events = new GuidDictionary<DataEvent> (undoManager);
		}

		public DataObject(UndoManager undoManager, System.Xml.XmlReader reader)
		{
			this.undoManager = undoManager;

			this.events = new GuidDictionary<DataEvent> (undoManager);

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "Guid":
							this.guid = new Guid (reader);
							break;

						case "Events":
							this.DeserializeEvents (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
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


		public bool EventsAny
		{
			get
			{
				return this.events.Any ();
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
			//	Enumère chronologiquement les événements.
			get
			{
				return this.events.OrderBy (x => x.Timestamp);
			}
		}

		public IEnumerable<DataEvent> UnsortedEvents
		{
			//	Enumère non chronologiquement les événements.
			get
			{
				return this.events;
			}
		}

		public void ChangeEventTimestamp(DataEvent e, Timestamp timestamp)
		{
			//	Change le timestamp d'un événement. Ici, rien n'empêche les bêtises, telles
			//	que déplacer un événement d'entrée après celui de sortie.
			//	La modification du timestamp nécessite de créer une copie de l'événement, dont
			//	on ne changera que le timestamp.
			var newEvent = new DataEvent (this.undoManager, e.Guid, timestamp, e.Type);
			newEvent.SetProperties (e);

			this.RemoveEvent (e);
			this.AddEvent (newEvent);
		}

		public void AddEvent(DataEvent e)
		{
			this.events.Add (e);
		}

		public void RemoveEvent(DataEvent e)
		{
			this.events.Remove (e);
		}

		public DataEvent GetInputEvent()
		{
			//	Retourne le premier événement d'entrée d'un objet.
			if (this.events.Any ())
			{
				return this.Events.First ();
			}
			else
			{
				return null;
			}
		}

		public DataEvent GetEvent(Guid guid)
		{
			return this.events[guid];
		}

		public DataEvent GetEvent(Timestamp timestamp)
		{
			return this.events.Where (x => x.Timestamp == timestamp).FirstOrDefault ();
		}

		public DataEvent GetPrevEvent(Timestamp timestamp)
		{
			var a = this.Events.ToList ();
			int i = a.FindIndex (x => x.Timestamp == timestamp);

			if (i > 0)
			{
				return a[i-1];
			}
			else
			{
				return null;
			}
		}

		public DataEvent GetNextEvent(Timestamp timestamp)
		{
			var a = this.Events.ToList ();
			int i = a.FindIndex (x => x.Timestamp == timestamp);

			if (i >= 0 && i < a.Count-1)
			{
				return a[i+1];
			}
			else
			{
				return null;
			}
		}

		public void CheckEvents()
		{
			Timestamp? lastTimestamp = null;
			int index = 0;

			foreach (var e in this.events)
			{
				if (lastTimestamp.HasValue)
				{
					//?System.Diagnostics.Debug.Assert (lastTimestamp.Value < e.Timestamp);

					if (lastTimestamp.Value >= e.Timestamp)
					{
						throw new System.InvalidOperationException (string.Format ("Event list corrupted, index={0} prev={1} current={2}", index, lastTimestamp.Value, e.Timestamp));
					}
				}

				lastTimestamp = e.Timestamp;
				index++;
			}
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
			if (!DataObject.IsOneShotField (field))
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

		public static bool IsOneShotField(ObjectField objectField)
		{
			switch (objectField)
			{
				case ObjectField.OneShotNumber:
				case ObjectField.OneShotUser:
				case ObjectField.OneShotDateEvent:
				case ObjectField.OneShotDateOperation:
				case ObjectField.OneShotComment:
				case ObjectField.OneShotDocuments:
					return true;

				default:
					return false;
			}
		}


		#region Serialize
		public void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement ("Object");

			this.guid.Serialize (writer, "Guid");
			this.SerializeEvents (writer);

			writer.WriteEndElement ();
		}

		private void SerializeEvents(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement ("Events");

			foreach (var e in this.events)
			{
				e.Serialize (writer);
			}

			writer.WriteEndElement ();
		}

		private void DeserializeEvents(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == "Event")
					{
						var e = new DataEvent (this.undoManager, reader);
						this.events.Add (e);
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}
		#endregion


		private readonly UndoManager			undoManager;
		private readonly Guid					guid;
		private readonly GuidDictionary<DataEvent>	events;
	}
}
