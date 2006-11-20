//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.DynamicData;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe RequestFactory permet de construire les requêtes correspondant
	/// aux modifications stockées dans un DataSet.
	/// </summary>
	public class RequestFactory : System.IDisposable
	{
		public RequestFactory()
		{
			this.requests = new System.Collections.ArrayList ();
		}
		
		
		public System.Collections.ArrayList		PendingRequests
		{
			get
			{
				return this.requests;
			}
		}
		
		
		public void Clear()
		{
			this.requests.Clear ();
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
				
				Database.DbKey key = new Database.DbKey (row);
				
				if ((key.IsTemporary) ||
					(key.Id.ClientId == 0))
				{
					throw new Database.Exceptions.InvalidIdException (string.Format ("Invalid key {0}.", key));
				}
				
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
		
		
		public Requests.Group CreateGroup()
		{
			//	Crée un objet "groupe de requêtes" contenant toutes les requêtes individuelles
			//	générées au moyen d'appels à GenerateRequests.
			
			Requests.Group group = new Requests.Group ();
			
			group.AddRange (this.requests);
			
			return group;
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
		}
		#endregion
		
		protected void GenerateInsertRowRequest(System.Data.DataRow row, int row_index, DynamicFieldCollection dynamic, FieldMatchResult[] matches)
		{
			//	Crée une requête d'insertion pour la ligne spécifiée.
			
			if ((dynamic == null) ||
				(matches[row_index] == FieldMatchResult.Zero))
			{
				//	La ligne ne contient aucun champ dynamique. On crée par conséquent une
				//	requête de création de ligne statique :
				
				this.requests.Add (new Requests.InsertStaticData (row));
			}
			else
			{
				//	TODO: créer une ligne comprenant des données dynamiques
				
				throw new System.NotImplementedException ("Dynamic fields not supported.");
			}
		}
		
		protected void GenerateUpdateRowRequest(System.Data.DataRow row, int row_index, DynamicFieldCollection dynamic, FieldMatchResult[] matches)
		{
			//	Crée une requête de mise à jour pour la ligne spécifiée.
			
			if ((dynamic == null) ||
				(matches[row_index] == FieldMatchResult.Zero))
			{
				//	La ligne ne contient aucun champ dynamique. On crée par conséquent une
				//	requête de mise à jour de ligne statique :
				
				this.requests.Add (new Requests.UpdateStaticData (row, Requests.UpdateMode.Changed));
			}
			else
			{
				//	TODO: mettre à jour une ligne comprenant des données dynamiques
				
				throw new System.NotImplementedException ("Dynamic fields not supported.");
			}
		}
		
		protected void GenerateRemoveRowRequest(System.Data.DataRow row)
		{
			//	Crée une requête de suppression pour la ligne spécifiée.
			
			throw new System.NotImplementedException ("Row removal not supported.");
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.requests.Clear ();
				this.requests = null;
			}
		}
		
		
		private System.Collections.ArrayList	requests;
	}
}
