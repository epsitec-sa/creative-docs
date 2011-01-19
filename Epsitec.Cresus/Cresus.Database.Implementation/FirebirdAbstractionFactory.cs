//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;

using FirebirdSql.Data.FirebirdClient;

namespace Epsitec.Cresus.Database.Implementation
{
	/// <summary>
	/// The <c>FirebirdAbstractionFactory</c> class implements the <c>IDbAbstractionFactory</c>
	/// interface for the Firebird engine.
	/// </summary>
	internal sealed class FirebirdAbstractionFactory : IDbAbstractionFactory
	{
		public FirebirdAbstractionFactory()
		{
			DbFactory.RegisterDatabaseAbstraction (this);
		}
		
		#region IDbAbstractionFactory Members

		public IDbAbstraction CreateDatabaseAbstraction(DbAccess dbAccess)
		{
			System.Diagnostics.Debug.Assert (dbAccess.Provider == this.ProviderName);

			FirebirdAbstraction fb = new FirebirdAbstraction (dbAccess, this, EngineType.Server);
			
			return fb;
		}

		public string							ProviderName
		{
			get
			{
				return "Firebird";
			}
		}

		public ITypeConverter					TypeConverter
		{
			get
			{
				return this.typeConverter;
			}
		}

		public string[] QueryDatabaseFilePaths(DbAccess dbAccess)
		{
			return FirebirdAbstractionFactory.GetDatabaseFilePaths (dbAccess);
		}

		#endregion

		internal static string[] GetDatabaseFilePaths(DbAccess dbAccess)
		{
			return new string[]
			{
				FirebirdAbstraction.MakeDbFilePath (dbAccess, EngineType.Server)
			};
		}
		
		
		private FirebirdTypeConverter			typeConverter = new FirebirdTypeConverter ();
	}


}
