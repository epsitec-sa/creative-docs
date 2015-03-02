//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Data.Serialization;

namespace Epsitec.Cresus.Assets.Data
{
	/**
	 **
	 **	ATTENTION: cette classe n'est pas thread safe
	 **
	 **/
	public class DataEvent : IGuid
	{
		public DataEvent(UndoManager undoManager, Timestamp timestamp, EventType type)
		{
			this.undoManager = undoManager;
			this.guid        = Guid.NewGuid ();
			this.Timestamp   = timestamp;
			this.Type        = type;

			this.properties = new UndoableList<AbstractDataProperty> (this.undoManager);
		}

		public DataEvent(UndoManager undoManager, Guid guid, Timestamp timestamp, EventType type)
		{
			this.undoManager = undoManager;
			this.guid        = guid;
			this.Timestamp   = timestamp;
			this.Type        = type;

			this.properties = new UndoableList<AbstractDataProperty> (this.undoManager);
		}

		public DataEvent(UndoManager undoManager, DataEvent model)
		{
			//	Copie un événement avec toutes ses propriétés.
			this.undoManager = undoManager;
			this.guid        = model.guid;
			this.Timestamp   = model.Timestamp;
			this.Type        = model.Type;

			this.properties = new UndoableList<AbstractDataProperty> (this.undoManager);
			this.SetProperties (model);
		}

		public DataEvent(UndoManager undoManager, System.Xml.XmlReader reader)
		{
			this.undoManager = undoManager;

			this.properties = new UndoableList<AbstractDataProperty> (this.undoManager);

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.Guid:
							this.guid = new Guid (reader);
							break;

						case X.Type:
							var s = reader.ReadElementContentAsString ();
							this.Type = (EventType) IOHelpers.ParseType (s, typeof (EventType));
							break;

						case X.Timestamp:
							this.Timestamp = new Timestamp (reader);
							break;

						case X.Properties:
							this.DeserializeProperties (reader);
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


		public readonly Timestamp				Timestamp;
		public readonly EventType				Type;


		public IEnumerable<AbstractDataProperty> Properties
		{
			get
			{
				return this.properties;
			}
		}

		public int PropertiesCount
		{
			get
			{
				return this.properties.Count;
			}
		}

		public void SetUndefinedProperties(DataEvent model)
		{
			//	Ajoute toutes les propriétés de model à l'événement courant, pour autant
			//	que ce dernier ne les aient pas encore.
			foreach (var property in model.properties)
			{
				if (property.Field != ObjectField.OneShotDateEvent)
				{
					var p = this.GetProperty (property.Field);
					if (p == null)
					{
						var copy = AbstractDataProperty.Copy (property);
						System.Diagnostics.Debug.Assert (copy != null);
						this.properties.Add (copy);
					}
				}
			}
		}

		public void SetProperties(DataEvent model)
		{
			//	Ajoute toutes les propriétés de model à l'événement courant.
			this.properties.Clear ();

			foreach (var property in model.properties)
			{
				if (property.Field != ObjectField.OneShotDateEvent)
				{
					var copy = AbstractDataProperty.Copy (property);
					System.Diagnostics.Debug.Assert (copy != null);
					this.properties.Add (copy);
				}
			}
		}

		public void AddProperty(AbstractDataProperty property)
		{
			int i = this.properties.FindIndex (x => x.Field == property.Field);
			if (i == -1)
			{
				this.properties.Add (property);
			}
			else
			{
				this.properties[i] = property;
			}

		}

		public void RemoveProperty(ObjectField field)
		{
			var p = this.GetProperty(field);
			if (p != null)
			{
				int i = this.properties.IndexOf (p);
				if (i != -1)
				{
					this.properties.RemoveAt (i);
				}
			}
		}

		public AbstractDataProperty GetProperty(ObjectField field)
		{
			//	Les propriétés ObjectField.Event* ne sont pas de vraies propriétés.
			//	On ne peut pas les modifier. Elles sont là uniquement pour le moteur
			//	de tri.
			switch (field)
			{
				case ObjectField.OneShotDateEvent:
					{
						var p = this.properties.Where (x => x.Field == field).FirstOrDefault ();
						if (p == null)
						{
							//	Si la propriété n'existe pas, elle est créé à la volée.
							return new DataDateProperty (field, this.Timestamp.Date);
						}
						else
						{
							//	Si la propriété existe, on la retourne comme n'importe quelle autre propriété.
							return p;
						}
					}

				case ObjectField.EventDate:
					return new DataDateProperty (field, this.Timestamp.Date);

				//	Si on trie selon la colonne du glyph, on trie en fait selon le
				//	nom du type de l'événement. Cela parait plausible.
				case ObjectField.EventGlyph:
				case ObjectField.EventType:
					return new DataStringProperty (field, DataDescriptions.GetEventDescription (this.Type));
			}

			return this.properties.Where (x => x.Field == field).FirstOrDefault ();
		}


		#region Serialize
		public void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement (X.Event);

			this.guid.Serialize (writer, X.Guid);
			writer.WriteElementString (X.Type, this.Type.ToStringIO ());
			this.Timestamp.Serialize (writer, X.Timestamp);
			this.SerializeProperties (writer);

			writer.WriteEndElement ();
		}

		public void SerializeProperties(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement (X.Properties);

			foreach (var property in this.properties)
			{
				property.Serialize (writer);
			}

			writer.WriteEndElement ();
		}

		private void DeserializeProperties(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.Property_String:
							this.properties.Add (new DataStringProperty (reader));
							break;

						case X.Property_Int:
							this.properties.Add (new DataIntProperty (reader));
							break;

						case X.Property_GuidRatio:
							this.properties.Add (new DataGuidRatioProperty (reader));
							break;

						case X.Property_Guid:
							this.properties.Add (new DataGuidProperty (reader));
							break;

						case X.Property_Decimal:
							this.properties.Add (new DataDecimalProperty (reader));
							break;

						case X.Property_Date:
							this.properties.Add (new DataDateProperty (reader));
							break;

						case X.Property_ComputedAmount:
							this.properties.Add (new DataComputedAmountProperty (reader));
							break;

						case X.Property_AmortizedAmount:
							this.properties.Add (new DataAmortizedAmountProperty (reader));
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}
		#endregion


		private readonly UndoManager				undoManager;
		private readonly Guid						guid;
		private readonly UndoableList<AbstractDataProperty>	properties;
	}
}
