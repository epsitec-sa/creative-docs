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
		public DataBroker(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
			this.tableBrokers = new Dictionary<string, DataTableBroker> ();
			this.adapter = new Adapter (this.infrastructure);
			this.richCommand = new DbRichCommand (this.infrastructure);
		}


		public DbRichCommand RichCommand
		{
			get
			{
				return this.richCommand;
			}
		}

		public void LoadTable(StructuredType type)
		{
			this.LoadTable (type, this.infrastructure.CreateSelectCondition ());
		}

		public void LoadTable(StructuredType type, DbSelectCondition condition)
		{
			using (DbTransaction transaction = this.infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTable tableDef = Adapter.FindTableDefinition (transaction, type);
				this.richCommand.ImportTable (transaction, tableDef, condition);
				transaction.Commit ();
			}
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
		private Adapter adapter;
		private DbRichCommand richCommand;
		private Dictionary<string, DataTableBroker> tableBrokers;
	}
}
