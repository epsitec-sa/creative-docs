//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// The <c>DataBroker</c> class is used to make data stored in the database
	/// available to the user interface binding code.
	/// </summary>
	public class DataBroker
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataBroker"/> class.
		/// </summary>
		/// <param name="infrastructure">The database infrastructure.</param>
		public DataBroker(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
			this.tableBrokers = new Dictionary<string, DataTableBroker> ();
			this.richCommand = new DbRichCommand (this.infrastructure);
		}


		/// <summary>
		/// Gets the rich command associated with this data broker.
		/// </summary>
		/// <value>The rich command.</value>
		public DbRichCommand RichCommand
		{
			get
			{
				return this.richCommand;
			}
		}

		/// <summary>
		/// Loads the specified table, filled with the live data taken from the
		/// database.
		/// </summary>
		/// <param name="type">The type of the table.</param>
		public void LoadTable(StructuredType type)
		{
			this.LoadTable (type, this.infrastructure.CreateSelectCondition ());
		}

		/// <summary>
		/// Loads the specified table, filled using the select condition.
		/// </summary>
		/// <param name="type">The type of the table.</param>
		/// <param name="condition">The select condition.</param>
		public void LoadTable(StructuredType type, DbSelectCondition condition)
		{
			using (DbTransaction transaction = this.infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTable tableDef = Adapter.FindTableDefinition (transaction, type);
				string tableName = tableDef.Name;
				
				lock (this.exclusion)
				{
					if (this.richCommand.DataSet.Tables.Contains (tableName))
					{
						//	TODO: handle reloading

						throw new System.NotImplementedException ();
					}
					else
					{
						this.richCommand.ImportTable (transaction, tableDef, condition);
					}
				}

				transaction.Commit ();
			}
		}

		/// <summary>
		/// Gets the table broker for the named table.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <returns>The table broker or <c>null</c> if the specified table is not
		/// loaded.</returns>
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
