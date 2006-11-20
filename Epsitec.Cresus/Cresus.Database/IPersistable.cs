//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>IPersistable</c> interface can be used to persist and load data from the
	/// database.
	/// </summary>
	public interface IPersistable
	{
		/// <summary>
		/// Persists the instance data to the database.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		void PersistToBase(DbTransaction transaction);
		
		/// <summary>
		/// Loads the instance data from the database.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		void LoadFromBase(DbTransaction transaction);
	}
}
