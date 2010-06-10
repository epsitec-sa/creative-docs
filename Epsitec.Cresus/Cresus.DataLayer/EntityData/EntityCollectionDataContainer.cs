using Epsitec.Common.Types;
using Epsitec.Cresus.Database;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.EntityData
{


	internal class EntityCollectionDataContainer
	{

		public List<DbKey> this[StructuredTypeField field]
		{
			get
			{
				if (!this.collectionKeys.ContainsKey (field))
				{
					this.collectionKeys[field] = new List<DbKey> ();
				}

				return this.collectionKeys[field];
			}
		}


		public EntityCollectionDataContainer()
		{
			this.collectionKeys = new Dictionary<StructuredTypeField, List<DbKey>> ();
		}


		private Dictionary<StructuredTypeField, List<DbKey>> collectionKeys;


	}


}
