//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe DataTable stocke une collection de DataSets.
	/// </summary>
	public class DataTable : DataRecord
	{
		public DataTable()
		{
			this.type = new DataType ();
		}
		
#if false
		public override bool					IsTable
		{
			get { return true; }
		}
		
		
		public override DataRecord FindRecord(string path, DataVersion version)
		{
			if ((path == null) || (path.Length == 0))
			{
				return this;
			}
			
			//	Si aucune donnée n'est stockée dans cette table, alors on ne peut
			//	de toute manière rien y trouver...
			
			if ((this.lines == null) || (this.columns == null))
			{
				return null;
			}
			
			string remaining;
			string local_path = DataRecord.SplitPath (path, out remaining);
			
			int p1 = local_path.IndexOf ('[');
			int p2 = local_path.IndexOf (']');

			if ((p1 != 0) || (p2+1 != local_path.Length))
			{
				throw new DataException (string.Format ("Syntax error in accessor ({0}) for table '{1}'", local_path, this.name));
			}
			
			int line_index = System.Int32.Parse (local_path.Substring (1, p2-1));
			
			if ((line_index < 0) || (line_index >= this.line_count))
			{
				throw new System.ArgumentOutOfRangeException ("index", line_index, string.Format ("Invalid index for table '{0}'", this.name));
			}
			
			//	L'index est valide... Reste à voir si l'appelant à fourni un nom de colonne
			//	ou non. Dans le premier cas, on retourne un DataField, dans le second cas,
			//	on retourne un DataSet synthétique.
			
			if (remaining != "")
			{
				//	Un nom de colonne a été spécifié.
				
				local_path = DataRecord.SplitPath (remaining, out remaining);
				
				if (this.columns.Contains (local_path))
				{
					ColumnDef  column = this.columns[local_path] as ColumnDef;
					DataRecord record = this.lines[line_index*this.column_count + column.Index] as DataRecord;
					
					return record.RecursiveFindRecord (remaining, version);
				}
			}
			else
			{
				//	L'appelant veut la ligne complète : on crée le DataSet qui lui
				//	correspond.
				
				DataSet   line  = new DataSet (local_path);
				DataState state = this.line_states[line_index] as DataState;
				
				DataRecord[] records = new DataRecord[this.column_count];
				
				this.lines.CopyTo (line_index*this.column_count, records, 0, this.column_count);
				
				line.SetParent (this);
				line.DefineContents (this.column_names, records, state);
			}
			
			return null;
		}
		
		public override void ValidateChanges()
		{
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			for (int i = 0; i < this.line_count; i++)
			{
				DataRecord record = entry.Value as DataRecord;
				
				switch (record.DataState)
				{
					case DataState.Removed:
					case DataState.Invalid:
						
						//	Les records qui ont été supprimés sont retirés définitivement de
						//	la table, ceux qui n'ont jamais existé aussi.
						
						list.Add (name);
						break;
					
					default:
						record.ValidateChanges ();
						break;
				}
			}
			
			if (list.Count > 0)
			{
				foreach (string name in list)
				{
					this.data.Remove (name);
				}
			}
		}
		
		protected class LineDef
		{
			public LineDef(int table_index, int lines_index)
			{
				this.table_index = table_index;
				this.lines_index = lines_index;
				this.state       = DataState.Unchanged;
			}
			
			
			public int							TableIndex
			{
				get { return this.table_index; }
			}
			
			public int							LinesIndex
			{
				get { return this.lines_index; }
			}
			
			public DataState					DataState
			{
				get { return this.state; }
				set { this.state = value; }
			}
			
			
			protected int						table_index;
			protected int						lines_index;
			protected DataState					state;
		}
		
		
		protected class ColumnDef
		{
			public ColumnDef(DataType data_type, int index)
			{
				this.data_type  = data_type;
				this.index      = index;
			}
			
			
			public DataType						DataType
			{
				get { return this.data_type; }
			}
			
			public int							Index
			{
				get { return this.index; }
			}
			
			
			protected DataType					data_type;
			protected int						index;
		}
		
		
		
		protected string						name;
		protected System.Collections.ArrayList	lines = null;
		protected System.Collections.ArrayList	mapping = null;
		protected System.Collections.Hashtable	columns = null;
		protected string[]						column_names = null;
		
		protected int							line_count;
		protected int							column_count;
#endif
	}
}
