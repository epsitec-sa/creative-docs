//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	public class DataBroker
	{
		public DataBroker(DbInfrastructure infrastructure, DbRichCommand richCommand)
		{
			this.infrastructure = infrastructure;
			this.richCommand = richCommand;
			this.tableBrokers = new Dictionary<string, DataTableBroker> ();
		}


		public DataTableBroker GetTableBroker(string tableName)
		{
			if (!this.richCommand.DataSet.Tables.Contains (tableName))
			{
				return null;
			}

			DataTableBroker broker;

			lock (this.exclusion)
			{
				if (!this.tableBrokers.TryGetValue (tableName, out broker))
				{
					DbTable tableDefinition = this.infrastructure.ResolveDbTable (tableName);
					Caption tableCaption    = tableDefinition.Caption;

					StructuredType structuredType = TypeRosetta.GetTypeObject (tableCaption) as StructuredType;
					System.Data.DataTable dataTable = this.richCommand.DataSet.Tables[tableName];
					
					broker = new DataTableBroker (structuredType, tableDefinition, dataTable);
					
					this.tableBrokers[tableName] = broker;
				}
			}
			
			return broker;
		}


		private object exclusion = new object ();
		private DbInfrastructure infrastructure;
		private DbRichCommand richCommand;
		private Dictionary<string, DataTableBroker> tableBrokers;
	}
}
