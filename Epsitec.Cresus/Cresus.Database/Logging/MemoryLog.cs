using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types.Collections;


namespace Epsitec.Cresus.Database.Logging
{


	// TODO Comment this class.
	// Marc


	internal sealed class MemoryLog : AbstractLog
	{


		public MemoryLog(int size) : base ()
		{
			size.ThrowIf (s => s < 1, "size cannot be smaller than 1");

			this.log = new CircularBuffer<Query> (size);
		}


		public override void Clear()
		{
			this.log.Clear ();
		}


		public override int GetNbEntries()
		{
			return this.log.Count;
		}


		public override Query GetEntry(int index)
		{
			return this.log[index];
		}


		internal override void AddEntry(Query query)
		{
			if (this.log.Count >= this.log.Size)
			{
				this.log.Remove ();
			}

			this.log.Add (query);
		}


		private readonly CircularBuffer<Query> log;


	}


}
