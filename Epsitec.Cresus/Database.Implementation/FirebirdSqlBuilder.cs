using FirebirdSql.Data.Firebird;

namespace Epsitec.Cresus.Database.Implementation
{
	using Epsitec.Cresus.Database;
	
	/// <summary>
	/// Implémentation de ISqlBuilder pour Firebird.
	/// </summary>
	public class FirebirdSqlBuilder : ISqlBuilder
	{
		public FirebirdSqlBuilder(FirebirdAbstraction fb)
		{
			this.fb = fb;
		}
		
		protected void UpdateCommand()
		{
			if (this.command_cache == null)
			{
				this.command_cache = new FbCommand ();
				//	TODO: ajouter le code pour remplir la commande
			}
		}
		
		#region ISqlBuilder Members

		public bool								AutoClear
		{
			get { return this.auto_clear; }
			set { this.auto_clear = value; }
		}

		public System.Data.IDbCommand			Command
		{
			get
			{
				if (this.auto_clear)
				{
					this.Clear ();
				}
				
				this.UpdateCommand ();
				
				return this.command_cache;
			}
		}

		
		public void Clear()
		{
			this.command_cache = null;
		}

		
		public bool ValidateName(string value)
		{
			return DbSqlStandard.ValidateName (value);
		}

		public bool ValidateString(string value)
		{
			return DbSqlStandard.ValidateString (value);
		}

		public bool ValidateNumber(string value)
		{
			return DbSqlStandard.ValidateNumber (value);
		}

		
		public void InsertTable(SqlTable table)
		{
			// TODO:  Add FirebirdSqlBuilder.InsertTable implementation
		}

		public void RemoveTable(string table_name)
		{
			// TODO:  Add FirebirdSqlBuilder.RemoveTable implementation
		}

		
		public void InsertTableColumns(string table_name, SqlColumn[] columns)
		{
			// TODO:  Add FirebirdSqlBuilder.InsertTableColumns implementation
		}

		public void UpdateTableColumns(string table_name, SqlColumn[] columns)
		{
			// TODO:  Add FirebirdSqlBuilder.UpdateTableColumns implementation
		}

		public void RemoveTableColumns(string table_name, SqlColumn[] columns)
		{
			// TODO:  Add FirebirdSqlBuilder.RemoveTableColumns implementation
		}

		
		public void SelectData(SqlSelect query)
		{
			// TODO:  Add FirebirdSqlBuilder.SelectData implementation
		}

		public void InsertData(string table_name, SqlFieldCollection fields)
		{
			// TODO:  Add FirebirdSqlBuilder.InsertData implementation
		}

		public void UpdateData(string table_name, SqlFieldCollection fields, SqlFieldCollection conditions)
		{
			// TODO:  Add FirebirdSqlBuilder.UpdateData implementation
		}

		public void RemoveData(string table_name, SqlFieldCollection conditions)
		{
			// TODO:  Add FirebirdSqlBuilder.RemoveData implementation
		}

		
		public void ExecuteProcedure(string procedure_name, SqlFieldCollection fields)
		{
			// TODO:  Add FirebirdSqlBuilder.ExecuteProcedure implementation
		}

		
		public void GetSqlParameters(System.Data.IDbCommand command, SqlFieldCollection fields)
		{
			// TODO:  Add FirebirdSqlBuilder.GetSqlParameters implementation
		}

		public void SetSqlParameters(System.Data.IDbCommand command, SqlFieldCollection fields)
		{
			// TODO:  Add FirebirdSqlBuilder.SetSqlParameters implementation
		}

		#endregion
		
		private FirebirdAbstraction		fb;
		private bool					auto_clear;
		private FbCommand				command_cache;
	}
}
