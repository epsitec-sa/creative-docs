using Epsitec.Common.Types;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.EntityData
{


	internal class EntityValueDataContainer
	{

		public object this[StructuredTypeField field]
		{
			get
			{
				return this.values.ContainsKey (field) ? this.values[field] : null;
			}
			set
			{
				this.values[field] = value;
			}
		}


		public EntityValueDataContainer()
		{
			this.values = new Dictionary<StructuredTypeField, object> ();
		}

		public bool Contains(StructuredTypeField field)
		{
			return this.values.ContainsKey (field);
		}

		private Dictionary<StructuredTypeField, object> values;

	}


}
