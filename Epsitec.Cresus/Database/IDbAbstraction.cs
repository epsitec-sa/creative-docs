namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IDbAbstraction est utilis�e pour acc�der aux m�canismes ADO.NET
	/// d�pendants d'un provider sp�cifique.
	/// </summary>
	public interface IDbAbstraction
	{
		IDbAbstractionFactory		Factory		{ get; }
		System.Data.IDbConnection	Connection	{ get; }
		
		System.Data.IDbCommand NewDbCommand();
		System.Data.IDataAdapter NewDataAdapter(System.Data.IDbCommand command);
	}
}
