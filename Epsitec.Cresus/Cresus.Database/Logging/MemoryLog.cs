using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

using System.Collections.ObjectModel;


namespace Epsitec.Cresus.Database.Logging
{


	// TODO Comment this class.
	// Marc


	internal sealed class MemoryLog : AbstractLog
	{


		public MemoryLog(int size) : base ()
		{
			size.ThrowIf (s => s < 1, "size cannot be smaller than 1");

			this.nextNumber = 0;
			this.log = new CircularBuffer<Query> (size);
		}


		public override void Clear()
		{
			this.log.Clear ();
			this.nextNumber = 1;
		}


		public override int GetNbEntries()
		{
			return this.log.Count;
		}


		public override Query GetEntry(int index)
		{
			return this.log[index];
		}


		public override ReadOnlyCollection<Query> GetEntries(int index, int count)
		{
			index.ThrowIf (i => i < 0 || index >= this.log.Count, "index is out of bounds");
			count.ThrowIf (c => c < 0 || index + c > this.log.Count, "count is out of bounds");

			List<Query> data = new List<Query> ();

			for (int i = index; i < index + count; i++)
			{
				data.Add (this.log[i]);
			}

			return data.AsReadOnly ();
		}


		protected override void AddEntry(Query query)
		{
			if (this.log.Count >= this.log.Size)
			{
				this.log.Remove ();
			}

			this.log.Add (query);
		}


		protected override int GetNextNumber()
		{
			return this.nextNumber++;
		}


		private int nextNumber;


		private readonly CircularBuffer<Query> log;


	}


}
