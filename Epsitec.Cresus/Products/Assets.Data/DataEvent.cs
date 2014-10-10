//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.DataProperties;

namespace Epsitec.Cresus.Assets.Data
{
	/**
	 **
	 **	ATTENTION: cette classe n'est pas thread safe
	 **
	 **/
	public class DataEvent : IGuid
	{
		public DataEvent(Timestamp timestamp, EventType type)
		{
			this.guid      = Guid.NewGuid ();
			this.Timestamp = timestamp;
			this.Type      = type;

			this.properties = new List<AbstractDataProperty> ();
		}

		public DataEvent(Guid guid, Timestamp timestamp, EventType type)
		{
			this.guid      = guid;
			this.Timestamp = timestamp;
			this.Type      = type;

			this.properties = new List<AbstractDataProperty> ();
		}

		public DataEvent(DataEvent model)
		{
			//	Copie un événement avec toutes ses propriétés.
			this.guid      = model.guid;
			this.Timestamp = model.Timestamp;
			this.Type      = model.Type;

			this.properties = new List<AbstractDataProperty> ();
			this.SetProperties (model);
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

				case ObjectField.EventDate:
					return new DataDateProperty (field, this.Timestamp.Date);

				//	Si on trie selon la colonne du glyphe, on trie en fait selon le
				//	nom du type de l'événement. Cela parait plausible.
				case ObjectField.EventGlyph:
				case ObjectField.EventType:
					return new DataStringProperty (field, DataDescriptions.GetEventDescription (this.Type));
			}

			return this.properties.Where (x => x.Field == field).FirstOrDefault ();
		}


		private readonly Guid						guid;
		private readonly List<AbstractDataProperty>	properties;
	}
}
