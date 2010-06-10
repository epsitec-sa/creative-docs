using Epsitec.Cresus.Database;
using Epsitec.Common.Types;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.EntityData
{


	internal class EntityReferenceDataContainer
	{

		
		public DbKey this[StructuredTypeField field]
		{
			get
			{
				return this.referenceKeys.ContainsKey (field) ? this.referenceKeys[field] : new DbKey ();
			}
			set
			{
				this.referenceKeys[field] = value;
			}
		}


		public EntityReferenceDataContainer()
		{
			this.referenceKeys = new Dictionary<StructuredTypeField, DbKey> ();
		}


		public bool Contains(StructuredTypeField field)
		{
			return this.referenceKeys.ContainsKey (field);
		}


		private Dictionary<StructuredTypeField, DbKey> referenceKeys;


	}


}
