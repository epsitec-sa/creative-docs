namespace Epsitec.Cresus.Database.Implementation
{
	using Epsitec.Cresus.Database;
	
	/// <summary>
	/// Implémentation de IDbAbstractionFactory pour Firebird.
	/// </summary>
	public class FirebirdAbstractionFactory : IDbAbstractionFactory
	{
		public FirebirdAbstractionFactory()
		{
			DbFactory.RegisterDbAbstraction (this);
		}
		
		#region IDbAbstractionFactory Members
		
		public IDbAbstraction NewDbAbstraction(DbAccess db_access)
		{
			System.Diagnostics.Debug.Assert (db_access.provider == this.ProviderName);
			
			FirebirdAbstraction fb = new FirebirdAbstraction (db_access, this);
			
			return fb;
		}
		
		public string								ProviderName
		{
			get	{ return "Firebird"; }
		}
		
		public ITypeConverter						TypeConverter
		{
			get { return null; }
		}
		
		#endregion
	}
}
