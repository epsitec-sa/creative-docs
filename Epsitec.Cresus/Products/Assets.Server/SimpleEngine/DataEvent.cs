//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	public class DataEvent : IGuid
	{
		public DataEvent(Timestamp timestamp, EventType type)
		{
			this.guid      = Guid.NewGuid ();
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


		public int PropertiesCount
		{
			get
			{
				return this.properties.Count;
			}
		}

		public void SetProperties(DataEvent model)
		{
			this.properties.Clear ();

			foreach (var property in model.properties)
			{
				var copy = AbstractDataProperty.Copy (property);
				System.Diagnostics.Debug.Assert (copy != null);
				this.properties.Add (copy);
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
			switch (field)
			{
				case ObjectField.EventDate:
					return new DataDateProperty (field, this.Timestamp.Date);

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
