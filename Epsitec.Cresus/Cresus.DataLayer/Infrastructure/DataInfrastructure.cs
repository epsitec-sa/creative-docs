using Epsitec.Cresus.Database;


using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	/// <summary>
	/// The <c>DataInfrastructure</c> class provides an high level access to the data stored in the
	/// database.
	/// </summary>
	public sealed class DataInfrastructure
	{


		/// <summary>
		/// Creates a new instance of <c>DataInfrastructure</c>.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> used to communicate to the Database.</param>
		public DataInfrastructure(DbInfrastructure dbInfrastructure)
		{
			this.DbInfrastructure = dbInfrastructure;
		}


		/// <summary>
		/// The <see cref="DbInfrastructure"/> object used to communicate with the database.
		/// </summary>
		public DbInfrastructure DbInfrastructure
		{
			get;
			private set;
		}


		/// <summary>
		/// Creates a new generator for unique ids in the database.
		/// </summary>
		/// <remarks>
		/// The slots of the generator are defined as a sequence of minimum and maximum values for
		/// each slot. Those bounds are included in each slot. 
		/// </remarks>
		/// <param name="name">The name of the generator.</param>
		/// <param name="slots">The definition of the slots of the generator.</param>
		public void CreateUidGenerator(string name, IEnumerable<System.Tuple<long, long>> slots)
		{
			UidGenerator.CreateUidGenerator (this.DbInfrastructure, name, slots);
		}


		/// <summary>
		/// Deletes a generator for unique ids from the database.
		/// </summary>
		/// <param name="name">The name of the generator.</param>
		public void DeleteUidGenerator(string name)
		{
			UidGenerator.DeleteUidGenerator (this.DbInfrastructure, name);
		}


		/// <summary>
		/// Tells whether a generator for unique ids exists in the database.
		/// </summary>
		/// <param name="name">The name of the generator.</param>
		/// <returns><c>true</c> if a generator with <paramref name="name"/> exists in the database, <c>false</c> if there aren't.</returns>
		public bool UidGeneratorExists(string name)
		{
			return UidGenerator.UidGeneratorExists (this.DbInfrastructure, name);
		}


		/// <summary>
		/// Gets the <see cref="UidGenerator"/> object used to manipulate a generator of unique ids
		/// in the database.
		/// </summary>
		/// <param name="name">The name of the generator.</param>
		/// <returns>The <see cref="UidGenerator"/> object.</returns>
		public UidGenerator GetUidGenerator(string name)
		{
			return UidGenerator.GetUidGenerator (this.DbInfrastructure, name);
		}
	
	
	}


}
