//	Copyright � 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.DataProperties;

namespace Epsitec.Cresus.Assets.Data
{
	public class DataObject : IGuid
	{
		public DataObject(UndoManager undoManager, bool simulation = false)
		{
			this.simulation  = simulation;
			this.undoManager = undoManager;
			this.guid        = Guid.NewGuid ();

			this.events = new GuidDictionary<DataEvent> (undoManager);
			this.sortedEvents = new List<DataEvent> ();
			this.lastGenerationNumber = -1;
		}

		public DataObject(UndoManager undoManager, Guid guid)
		{
			this.undoManager = undoManager;
			this.guid        = guid;

			this.events = new GuidDictionary<DataEvent> (undoManager);
			this.sortedEvents = new List<DataEvent> ();
			this.lastGenerationNumber = -1;
		}

		public DataObject(UndoManager undoManager, System.Xml.XmlReader reader)
		{
			this.undoManager = undoManager;

			this.events = new GuidDictionary<DataEvent> (undoManager);
			this.sortedEvents = new List<DataEvent> ();

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

			this.lastGenerationNumber = -1;
		}


		public bool								Simulation
		{
			get
			{
				return this.simulation;
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


		public Timestamp GetNewTimestamp(System.DateTime date)
		{
			//	Retourne le Timestamp � utiliser pour cr�er un nouvel �v�nement � une
			//	date donn�e, en partant du principe que l'�v�nement viendra toujours
			//	apr�s les �v�nements existants.
			this.UpdateSortedList ();

			var e = this.sortedEvents.Where (x => x.Timestamp.Date == date).LastOrDefault ();

			if (e == null)
			{
				return new Timestamp (date, 0);
			}
			else
			{
				return new Timestamp (date, e.Timestamp.Position + 1);
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

		public List<DataEvent> Events
		{
			//	Retourne tous les �v�nements tri�s chronologiquement.
			//	La liste ne doit �tre utilis�e qu'en lecture.
			get
			{
				this.UpdateSortedList ();
				return this.sortedEvents;
			}
		}

		public int FindEventIndex(Timestamp timestamp)
		{
			this.UpdateSortedList ();

			return this.sortedEvents.FindIndex (x => x.Timestamp == timestamp);
		}

		public void ChangeEventTimestamp(DataEvent e, Timestamp timestamp)
		{
			//	Change le timestamp d'un �v�nement. Ici, rien n'emp�che les b�tises, telles
			//	que d�placer un �v�nement d'entr�e apr�s celui de sortie.
			//	La modification du timestamp n�cessite de cr�er une copie de l'�v�nement, dont
			//	on ne changera que le timestamp.
			var newEvent = new DataEvent (this.undoManager, e.Guid, timestamp, e.Type);
			newEvent.SetProperties (e);

			this.RemoveEvent (e);
			this.AddEvent (newEvent);
		}

		public void AddEvent(DataEvent e)
		{
			this.events.Add (e);

			int i = this.sortedEvents.Where (x => x.Timestamp < e.Timestamp).Count ();
			this.sortedEvents.Insert (i, e);

			this.SortedListUpdated ();
		}

		public void RemoveEvent(DataEvent e)
		{
			this.events.Remove (e);
			this.sortedEvents.Remove (e);

			this.SortedListUpdated ();
		}

		public DataEvent GetInputEvent()
		{
			//	Retourne le premier �v�nement d'entr�e d'un objet.
			this.UpdateSortedList ();

			if (this.sortedEvents.Any ())
			{
				return this.sortedEvents.First ();
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
			this.UpdateSortedList ();
			return this.sortedEvents.Where (x => x.Timestamp == timestamp).FirstOrDefault ();
		}

		public DataEvent GetPrevEvent(Timestamp timestamp)
		{
			int i = this.FindEventIndex (timestamp);
			if (i > 0)
			{
				return this.sortedEvents[i-1];
			}
			else
			{
				return null;
			}
		}

		public DataEvent GetNextEvent(Timestamp timestamp)
		{
			int i = this.FindEventIndex (timestamp);
			if (i >= 0 && i < this.sortedEvents.Count-1)
			{
				return this.sortedEvents[i+1];
			}
			else
			{
				return null;
			}
		}


		public AbstractDataProperty GetSingleProperty(Timestamp timestamp, ObjectField field)
		{
			//	Retourne la propri�t� d�finie � la date exacte.
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
			//	Retourne la propri�t� d�finie lors de l'�v�nement d'entr�e.
			this.UpdateSortedList ();

			var e = this.sortedEvents.FirstOrDefault ();  // �v�nement d'entr�e
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
			//	Retourne la propri�t� d�finie � la date exacte ou ant�rieurement.
			var p = this.GetSingleProperty (timestamp, field);

			if (p != null)
			{
				p.State = PropertyState.Single;
				return p;
			}

			// On cherche depuis la date donn�e en remontant dans le pass�.
			if (!DataObject.IsOneShotField (field))
			{
				this.UpdateSortedList ();

				var e = this.sortedEvents
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


		private void UpdateSortedList()
		{
			//	Met � jour la liste des �v�nements tri�e chronologiquement, mais
			//	seulement si c'est n�cessaire.
			if (this.lastGenerationNumber != this.events.GenerationNumber)
			{
				this.sortedEvents.Clear ();
				this.sortedEvents.AddRange (this.events.OrderBy (x => x.Timestamp));

				this.SortedListUpdated ();
			}
		}

		private void SortedListUpdated()
		{
			this.lastGenerationNumber = this.events.GenerationNumber;
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

			this.UpdateSortedList ();

			foreach (var e in this.sortedEvents)
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


		private readonly bool					simulation;
		private readonly UndoManager			undoManager;
		private readonly Guid					guid;
		private readonly GuidDictionary<DataEvent> events;
		private readonly List<DataEvent>		sortedEvents;

		public int								lastGenerationNumber;
	}
}
