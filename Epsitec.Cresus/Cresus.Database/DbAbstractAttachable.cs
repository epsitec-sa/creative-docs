using Epsitec.Common.Support.Extensions;


namespace Epsitec.Cresus.Database
{


	/// <summary>
	/// The <c>DbAbstractAttachable</c> class provides the basic functions for classes that must deal
	/// with a single <see cref="DbTable"/> in the database.
	/// </summary>
	public class DbAbstractAttachable : IAttachable
	{

		/// <summary>
		/// Builds a new instance of <c>DbAbstractAttachable</c>
		/// </summary>
		public DbAbstractAttachable()
		{
			this.IsAttached = false;
		}


		/// <summary>
		/// The state of this instance.
		/// </summary>
		protected bool IsAttached
		{
			get;
			private set;
		}


		/// <summary>
		/// The <see cref="DbInfrastructure"/> object to use to communicate with the database.
		/// </summary>
		protected DbInfrastructure DbInfrastructure
		{
			get;
			private set;
		}


		/// <summary>
		/// The <see cref="DbTable"/> used to store the counters data.
		/// </summary>
		protected DbTable DbTable
		{
			get;
			private set;
		}
		

		#region IAttachable Members

		
		/// <summary>
		/// Attaches this instance to the specified database table.
		/// </summary>
		/// <param name="dbInfrastructure">The infrastructure.</param>
		/// <param name="dbTable">The database table.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbInfrastructure" /> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbTable" /> is <c>null</c>.</exception>
		public void Attach(DbInfrastructure dbInfrastructure, DbTable dbTable)
		{			
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			dbTable.ThrowIfNull ("dbTable");

			this.DbInfrastructure = dbInfrastructure;
			this.DbTable = dbTable;

			this.IsAttached = true;
		}


		/// <summary>
		/// Detaches this instance from the database.
		/// </summary>
		public void Detach()
		{
			this.IsAttached = false;

			this.DbInfrastructure = null;
			this.DbTable = null;
		}


		#endregion
		

		/// <summary>
		/// Checks that this instance is attached to a <see cref="DbInfrastructure"/>.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		protected void CheckIsAttached()
		{
			if (!this.IsAttached)
			{
				throw new System.InvalidOperationException ("Cannot use this instance because it is detached.");
			}
		}


	}


}
