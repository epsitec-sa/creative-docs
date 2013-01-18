//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Database.Implementation
{

	/// <summary>
	/// The <c>FirebirdEmbeddedAbstractionFactory</c> class implements the <c>IDbAbstractionFactory</c>
	/// interface for the embedded Firebird engine.
	/// </summary>
	internal sealed class FirebirdEmbeddedAbstractionFactory : IDbAbstractionFactory
	{
		public FirebirdEmbeddedAbstractionFactory()
		{
			DbFactory.RegisterDatabaseAbstraction (this);
		}
		
		#region IDbAbstractionFactory Members

		public IDbAbstraction CreateDatabaseAbstraction(DbAccess dbAccess)
		{
			System.Diagnostics.Debug.Assert (dbAccess.Provider == this.ProviderName);

			return new FirebirdAbstraction (dbAccess, this, EngineType.Embedded);
		}

		public string							ProviderName
		{
			get
			{
				return "FirebirdEmbedded";
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
			return new string[]
			{
				FirebirdAbstraction.MakeDbFilePath (dbAccess)
			};
		}

		#endregion
		
		private FirebirdTypeConverter			typeConverter = new FirebirdTypeConverter ();
	}
}
