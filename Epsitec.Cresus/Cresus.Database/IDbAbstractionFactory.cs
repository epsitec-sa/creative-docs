//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>IDbAbstractionFactory</c> interface is used to instantiate database
	/// abstraction objects for a given provider.
	/// </summary>
	public interface IDbAbstractionFactory
	{
		/// <summary>
		/// Gets the name of the provider.
		/// </summary>
		/// <value>The name of the provider.</value>
		string ProviderName
		{
			get;
		}

		/// <summary>
		/// Gets the type converter for this provider.
		/// </summary>
		/// <value>The type converter for this provider.</value>
		ITypeConverter TypeConverter
		{
			get;
		}

		/// <summary>
		/// Queries the database file paths and returns an array of paths for the
		/// files which make up the database.
		/// </summary>
		/// <param name="dbAccess">The database access definition.</param>
		/// <returns>The array of file paths.</returns>
		string[] QueryDatabaseFilePaths(DbAccess dbAccess);

		/// <summary>
		/// Allocates a new database abstraction object and establishes a connection
		/// to the specified database.
		/// </summary>
		/// <param name="dbAccess">The database access definition.</param>
		/// <returns>The database abstraction object.</returns>
		IDbAbstraction CreateDatabaseAbstraction(DbAccess dbAccess);
	}
}
