//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.DynamicData;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe RequestFactory permet de construire les requêtes correspondant
	/// aux modifications stockées dans un DataSet.
	/// </summary>
	public class RequestFactory
	{
		public RequestFactory()
		{
			this.requests = new System.Collections.ArrayList ();
		}
		
		public void GenerateRequests(System.Data.DataSet data_set)
		{
			foreach (System.Data.DataTable data_table in data_set.Tables)
			{
				this.GenerateRequests (data_table);
			}
		}
		
		public void GenerateRequests(System.Data.DataTable data_table)
		{
			DynamicFieldCollection dynamic = DynamicFieldCollection.GetDynamicFiels (data_table);
			FieldMatchResult[]     matches = (dynamic == null) ? null : dynamic.AnalyseRows (data_table);
			
			for (int r = 0; r < data_table.Rows.Count; r++)
			{
				System.Data.DataRow      row   = data_table.Rows[r];
				System.Data.DataRowState state = row.RowState;
				
				if (state == System.Data.DataRowState.Unchanged)
				{
					continue;
				}
				
				switch (state)
				{
					case System.Data.DataRowState.Added:	this.GenerateInsertRowRequest (row, r, dynamic, matches); break;
					case System.Data.DataRowState.Modified:	this.GenerateUpdateRowRequest (row, r, dynamic, matches); break;
					case System.Data.DataRowState.Deleted:	this.GenerateRemoveRowRequest (row);					  break;
				}
			}
		}
		
		public void GenerateInsertRowRequest(System.Data.DataRow row, int row_index, DynamicFieldCollection dynamic, FieldMatchResult[] matches)
		{
			//	Crée une requête d'insertion pour la ligne spécifiée.
			
			if ((dynamic == null) ||
				(matches[row_index] == FieldMatchResult.Zero))
			{
				//	La ligne ne contient aucun champ dynamique. On crée par conséquent 
				this.requests.Add (new Requests.InsertStaticData (row));
			}
			else
			{
			}
		}
		
		public void GenerateUpdateRowRequest(System.Data.DataRow row, int row_index, DynamicFieldCollection dynamic, FieldMatchResult[] matches)
		{
		}
		
		public void GenerateRemoveRowRequest(System.Data.DataRow row)
		{
		}
		
		private System.Collections.ArrayList	requests;
	}
}
