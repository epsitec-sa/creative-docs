namespace Epsitec.Cresus.Database.Services
{
	
	
	public sealed class DbLogEntry
	{
		
		
		public DbLogEntry(DbId entryId, DbId connectionId, System.DateTime dateTime)
		{
			this.EntryId = entryId;
			this.ConnectionId = connectionId;
			this.DateTime = dateTime;
		}


		public DbId EntryId
		{
			get;
			private set;
		}


		public DbId ConnectionId
		{
			get;
			private set;
		}


		public System.DateTime DateTime
		{
			get;
			private set;
		}


	}


}