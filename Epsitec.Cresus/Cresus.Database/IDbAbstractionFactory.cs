//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>IDbAbstractionFactory</c> interface is used to instanciate database
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
		/// Allocates a new database abstraction object and establishes a connection
		/// to the specified database.
		/// </summary>
		/// <param name="dbAccess">The database access definition.</param>
		/// <returns>The database abstraction object.</returns>
		IDbAbstraction CreateDatabaseAbstraction(DbAccess dbAccess);
	}
}
