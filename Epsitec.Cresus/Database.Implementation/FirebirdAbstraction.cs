namespace Epsitec.Cresus.Database.Implementation
{
	using Epsitec.Cresus.Database;
	
	/// <summary>
	/// Impl�mentation de IDbAbstraction pour Firebird.
	/// </summary>
	public class FirebirdAbstraction : IDbAbstraction, System.IDisposable
	{
		public FirebirdAbstraction(DbAccess db_access, IDbAbstractionFactory db_factory)
		{
			this.db_factory = db_factory;
			this.db_connection = null;
			
			//	TODO: initialisation
			//
			//	1. La connexion est ouverte pour v�rifier que la base existe. Il faut voir
			//	   si la connexion doit �tre conserv�e dans l'�tat ouvert.
			//
			//	2. Si la base n'existe pas mais que db_access.create est actif, on cr�e
			//	   la base de donn�es, puis on ouvre la connexion.
			//
			//	- La propri�t� 'Connection' doit-elle s'assurer que la connexion est ouverte ?
			//
			//	- Comment sont g�r�es les transactions ? Je crois que IDbCommand.Transaction
			//	  et IDbConnection.BeginTransaction offrent ce qu'il faut, donc ce n'est pas
			//	  la peine de s'en occuper ici.
			
			throw new DbFactoryException ();
		}

		~FirebirdAbstraction()
		{
			this.Dispose (false);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				//	TODO: lib�re les ressources "managed" => appeler 'Dispose'
			}
			
			//	TODO: lib�re les ressources non "managed"
		}
		
		
		#region IDbAbstraction Members
		public IDbAbstractionFactory				Factory
		{
			get { return this.db_factory; }
		}
		
		public System.Data.IDbConnection			Connection
		{
			get { return this.db_connection; }
		}
		
		public ISqlBuilder							SqlBuilder
		{
			get { return null; }
		}

		
		public System.Data.IDbCommand NewDbCommand()
		{
			return this.db_connection.CreateCommand ();
		}
		
		public System.Data.IDataAdapter NewDataAdapter(System.Data.IDbCommand command)
		{
			//	TODO: impl�menter new DataAdapter(command)
			return null;
		}
		
		public string[] QueryTableNames()
		{
			//	TODO: rechercher les noms des tables connues (en filtrant �ventuellement les
			//	noms qui ne correspondent pas � des tables cr��es par Cr�sus, par exemple si
			//	la base a des tables internes pour l'administration).
			
			return new string[0];
		}
		
		public SqlTable QueryTableSchema(string table_name)
		{
			//	TODO: trouver le sch�ma de la table (si elle existe). Si la table ne peut pas
			//	�tre trouv�e, on retourne simplement null.
			
			return null;
		}
		#endregion
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		private IDbAbstractionFactory				db_factory;
		private System.Data.IDbConnection			db_connection;
	}
}
