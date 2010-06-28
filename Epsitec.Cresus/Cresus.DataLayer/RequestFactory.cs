//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Requests;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// The <c>RequestFactory</c> class produces the requests based on modifications
	/// found in a data set.
	/// </summary>
	public sealed class RequestFactory : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RequestFactory"/> class.
		/// </summary>
		public RequestFactory()
		{
			this.requests = new List<AbstractRequest> ();
		}


		/// <summary>
		/// Gets the pending requests.
		/// </summary>
		/// <value>The pending requests.</value>
		public IEnumerable<AbstractRequest>		PendingRequests
		{
			get
			{
				return this.requests;
			}
		}


		/// <summary>
		/// Clears all pending requests produced by the factory.
		/// </summary>
		public void Clear()
		{
			this.requests.Clear ();
		}


		/// <summary>
		/// Generates the requests for the changes found in the data set. This
		/// simply walks through all the tables.
		/// </summary>
		/// <param name="dataSet">The data set.</param>
		public void GenerateRequests(System.Data.DataSet dataSet)
		{
			foreach (System.Data.DataTable dataTable in dataSet.Tables)
			{
				this.GenerateRequests (dataTable);
			}
		}

		/// <summary>
		/// Generates the requests for the changes found in the data table.
		/// </summary>
		/// <param name="dataTable">The data table.</param>
		public void GenerateRequests(System.Data.DataTable dataTable)
		{
			//	TODO: handle dynamic row contents
			
			foreach (System.Data.DataRow row in dataTable.Rows)
			{
				Database.DbKey key = new Database.DbKey (row);
				
				if ((key.IsTemporary) ||
					(key.Id.ClientId == 0))
				{
					throw new Database.Exceptions.InvalidIdException (string.Format ("Invalid key {0}.", key));
				}

				switch (row.RowState)
				{
					case System.Data.DataRowState.Detached:
					case System.Data.DataRowState.Unchanged:
						break;

					case System.Data.DataRowState.Added:
						this.GenerateInsertRowRequest (row);
						break;
					
					case System.Data.DataRowState.Modified:
						this.GenerateUpdateRowRequest (row);
						break;
					
					case System.Data.DataRowState.Deleted:
						this.GenerateRemoveRowRequest (row);
						break;
				}
			}
		}


		/// <summary>
		/// Creates the request collection containing all inidividual requests.
		/// </summary>
		/// <returns></returns>
		public RequestCollection CreateRequestCollection()
		{
			RequestCollection collection = new RequestCollection ();
			
			collection.AddRange (this.requests);
			
			return collection;
		}
		
		
		#region IDisposable Members
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		#endregion
		
		
		void GenerateInsertRowRequest(System.Data.DataRow row)
		{
			this.requests.Add (new InsertStaticDataRequest (row));
		}
		
		void GenerateUpdateRowRequest(System.Data.DataRow row)
		{
			this.requests.Add (new UpdateStaticDataRequest (row, UpdateMode.Changed));
		}
		
		void GenerateRemoveRowRequest(System.Data.DataRow row)
		{
			throw new System.NotImplementedException ("Row removal not supported.");
		}
		
		
		void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.requests.Clear ();
				this.requests = null;
			}
		}
		
		
		private List<AbstractRequest>			requests;
	}
}
