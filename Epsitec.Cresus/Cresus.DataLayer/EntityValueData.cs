using Epsitec.Common.Types;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer
{


	public class EntityValueData
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


		public EntityValueData()
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
