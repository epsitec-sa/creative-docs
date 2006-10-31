//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;

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
			DbFactory.RegisterDbAbstraction (this);
		}
		
		#region IDbAbstractionFactory Members

		public IDbAbstraction NewDbAbstraction(DbAccess dbAccess)
		{
			System.Diagnostics.Debug.Assert (dbAccess.Provider == this.ProviderName);

			return new FirebirdAbstraction (dbAccess, this, EngineType.Server);
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
		
		#endregion
		
		private FirebirdTypeConverter			typeConverter = new FirebirdTypeConverter ();
	}
}
