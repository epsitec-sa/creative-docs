namespace Epsitec.Cresus.Database.Implementation
{
	using Epsitec.Cresus.Database;
	
	/// <summary>
	/// Implémentation de IDbAbstractionFactory pour FireBird.
	/// </summary>
	public class FireBirdAbstractionFactory : IDbAbstractionFactory
	{
		public FireBirdAbstractionFactory()
		{
			DbFactory.RegisterDbAbstraction (this);
		}
		
		#region IDbAbstractionFactory Members
		
		public IDbAbstraction NewDbAbstraction(DbAccess db_access)
		{
			return null;
		}
		
		public string								ProviderName
		{
			get	{ return "FireBird"; }
		}
		
		public ITypeConverter						TypeConverter
		{
			get { return null; }
		}
		
		public ISqlBuilder							SqlBuilder
		{
			get { return null; }
		}

		#endregion
	}
}
