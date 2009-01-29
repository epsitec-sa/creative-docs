//	Copyright � 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Requests;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// La classe RequestFactory permet de construire les requ�tes correspondant
	/// aux modifications stock�es dans un DataSet.
	/// </summary>
	public class RequestFactory : System.IDisposable
	{
		public RequestFactory()
		{
			this.requests = new List<AbstractRequest> ();
		}
		
		
		public IEnumerable<AbstractRequest>	PendingRequests
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
//-			DynamicFieldCollection dynamic = DynamicFieldCollection.GetDynamicFiels (data_table);
//-			FieldMatchResult[]     matches = (dynamic == null) ? null : dynamic.AnalyseRows (data_table);
			
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
					case System.Data.DataRowState.Added:
						this.GenerateInsertRowRequest (row, r/*, dynamic, matches*/);
						break;
					
					case System.Data.DataRowState.Modified:
						this.GenerateUpdateRowRequest (row, r/*, dynamic, matches*/);
						break;
					
					case System.Data.DataRowState.Deleted:
						this.GenerateRemoveRowRequest (row);
						break;
				}
			}
		}
		
		
		public RequestCollection CreateGroup()
		{
			//	Cr�e un objet "groupe de requ�tes" contenant toutes les requ�tes individuelles
			//	g�n�r�es au moyen d'appels � GenerateRequests.

			RequestCollection group = new RequestCollection ();
			
			group.AddRange (this.requests);
			
			return group;
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
		}
		#endregion
		
		protected void GenerateInsertRowRequest(System.Data.DataRow row, int row_index /*, DynamicFieldCollection dynamic, FieldMatchResult[] matches */)
		{
			//	Cr�e une requ�te d'insertion pour la ligne sp�cifi�e.
			
			if (true /*(dynamic == null) ||
				(matches[row_index] == FieldMatchResult.Zero)*/)
			{
				//	La ligne ne contient aucun champ dynamique. On cr�e par cons�quent une
				//	requ�te de cr�ation de ligne statique :
				
				this.requests.Add (new InsertStaticDataRequest (row));
			}
			else
			{
				//	TODO: cr�er une ligne comprenant des donn�es dynamiques
				
				throw new System.NotImplementedException ("Dynamic fields not supported.");
			}
		}
		
		protected void GenerateUpdateRowRequest(System.Data.DataRow row, int row_index /*, DynamicFieldCollection dynamic, FieldMatchResult[] matches */)
		{
			//	Cr�e une requ�te de mise � jour pour la ligne sp�cifi�e.
			
			if (true /*(dynamic == null) ||
				(matches[row_index] == FieldMatchResult.Zero)*/)
			{
				//	La ligne ne contient aucun champ dynamique. On cr�e par cons�quent une
				//	requ�te de mise � jour de ligne statique :
				
				this.requests.Add (new UpdateStaticDataRequest (row, UpdateMode.Changed));
			}
			else
			{
				//	TODO: mettre � jour une ligne comprenant des donn�es dynamiques
				
				throw new System.NotImplementedException ("Dynamic fields not supported.");
			}
		}
		
		protected void GenerateRemoveRowRequest(System.Data.DataRow row)
		{
			//	Cr�e une requ�te de suppression pour la ligne sp�cifi�e.
			
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
		
		
		private List<AbstractRequest>	requests;
	}
}
