//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			DbFactory.RegisterDbAbstraction (this);
		}
		
		#region IDbAbstractionFactory Members

		public IDbAbstraction NewDbAbstraction(DbAccess dbAccess)
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
		
		#endregion
		
		private FirebirdTypeConverter			typeConverter = new FirebirdTypeConverter ();
	}
}
