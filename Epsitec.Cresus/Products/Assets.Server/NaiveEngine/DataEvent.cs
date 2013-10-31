//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Epsitec.Cresus.Assets.Server.NaiveEngine
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

		public void AddProperty(AbstractDataProperty property)
		{
			int i = this.properties.FindIndex (x => x.FieldId == property.FieldId);
			if (i == -1)
			{
				this.properties.Add (property);
			}
			else
			{
				this.properties[i] = property;
			}

		}

		public void RemoveProperty(int fieldId)
		{
			var p = this.GetProperty(fieldId);
			if (p != null)
			{
				int i = this.properties.IndexOf (p);
				if (i != -1)
				{
					this.properties.RemoveAt (i);
				}
			}
		}

		public AbstractDataProperty GetProperty(int fieldId)
		{
			return this.properties.Where (x => x.FieldId == fieldId).FirstOrDefault ();
		}


		private readonly Guid						guid;
		private readonly List<AbstractDataProperty>	properties;
	}
}
