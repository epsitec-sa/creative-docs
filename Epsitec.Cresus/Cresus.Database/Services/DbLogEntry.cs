namespace Epsitec.Cresus.Database.Services
{
	
	
	public sealed class DbLogEntry
	{
		
		
		public DbLogEntry(DbId entryId, DbId connectionId, System.DateTime dateTime, long sequenceNumber)
		{
			this.EntryId = entryId;
			this.ConnectionId = connectionId;
			this.DateTime = dateTime;
			this.SequenceNumber = sequenceNumber;
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


		public long SequenceNumber
		{
			get;
			private set;
		}


	}


}