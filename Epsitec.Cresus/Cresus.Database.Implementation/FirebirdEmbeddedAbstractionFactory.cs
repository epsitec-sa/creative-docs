//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Implementation
{
	using Epsitec.Cresus.Database;
	
	/// <summary>
	/// Implémentation de IDbAbstractionFactory pour Firebird (version embarquée).
	/// </summary>
	public class FirebirdEmbeddedAbstractionFactory : IDbAbstractionFactory
	{
		public FirebirdEmbeddedAbstractionFactory()
		{
			DbFactory.RegisterDbAbstraction (this);
		}
		
		#region IDbAbstractionFactory Members
		public IDbAbstraction NewDbAbstraction(DbAccess db_access)
		{
			System.Diagnostics.Debug.Assert (db_access.Provider == this.ProviderName);
			
			FirebirdAbstraction fb = new FirebirdAbstraction (db_access, this, EngineType.Embedded);
			
			return fb;
		}
		
		public string								ProviderName
		{
			get	{ return "FirebirdEmbedded"; }
		}
		
		public ITypeConverter						TypeConverter
		{
			get { return this.type_converter; }
		}
		#endregion
		
		protected FirebirdTypeConverter	type_converter = new FirebirdTypeConverter ();
	}
}
