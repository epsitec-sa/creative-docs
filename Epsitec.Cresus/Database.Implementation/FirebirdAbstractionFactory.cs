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
			System.Diagnostics.Debug.Assert (db_access.Provider == this.ProviderName);
			
			FirebirdAbstraction fb = new FirebirdAbstraction (db_access, this);
			
			return fb;
		}
		
		public string								ProviderName
		{
			get	{ return "Firebird"; }
		}
		
		public ITypeConverter						TypeConverter
		{
			get { return this.type_converter; }
		}
		
		#endregion
		
		protected FirebirdTypeConverter				type_converter = new FirebirdTypeConverter ();
	}
}
