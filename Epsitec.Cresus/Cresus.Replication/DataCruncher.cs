//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Replication
{
	/// <summary>
	/// Summary description for DataCruncher.
	/// </summary>
	public sealed class DataCruncher
	{
		public DataCruncher(DbInfrastructure infrastructure, DbTransaction transaction)
		{
			this.infrastructure = infrastructure;
			this.transaction    = transaction;
		}
		
		public System.Data.DataTable ExtractData(DbTable table, DbId sync_id)
		{
			//	Extrait les données de la table spécifiée en ne considérant que ce qui a changé
			//	depuis la synchronisation définie par 'sync_id'.
			
			long sync_id_min = sync_id.Value;
			long sync_id_max = DbId.LocalRange * (sync_id.ClientId + 1) - 1;
			
			System.Diagnostics.Debug.Assert (DbId.AnalyzeClass (sync_id_min) == DbIdClass.Standard);
			System.Diagnostics.Debug.Assert (DbId.AnalyzeClass (sync_id_max) == DbIdClass.Standard);
			
			DbSelectCondition condition = new DbSelectCondition (this.infrastructure.TypeConverter);
			
			condition.AddCondition (table.Columns[Tags.ColumnRefLog], DbCompare.GreaterThan, sync_id_min);
			condition.AddCondition (table.Columns[Tags.ColumnRefLog], DbCompare.LessThan, sync_id_max);
			
			using (DbRichCommand command = DbRichCommand.CreateFromTable (this.infrastructure, this.transaction, table, condition))
			{
				return command.DataSet.Tables[0];
			}
		}
		
		
		
		private DbInfrastructure				infrastructure;
		private DbTransaction					transaction;
	}
}
