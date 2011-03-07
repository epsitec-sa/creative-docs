using System.Collections.Generic;


namespace Epsitec.Cresus.Database.Logging
{


	public class ConsoleLog : AbstractLog
	{


		public ConsoleLog() : base ()
		{
		}


		public override void Clear()
		{
		}


		public override int GetNbEntries()
		{
			return 0;
		}


		public override Query GetEntry(int index)
		{
			throw new System.IndexOutOfRangeException ();
		}


		public override System.Collections.ObjectModel.ReadOnlyCollection<Query> GetEntries(int index, int count)
		{
			return new List<Query> ().AsReadOnly ();
		}


		protected override void AddEntry(Query query)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();

			sb.Append ("\n=========================================================================");
			sb.Append ("\nQuery number: " + query.Number);
			sb.Append ("\nQuery start time: " + query.StartTime);
			sb.Append ("\nQuery duration: " + query.Duration);
			sb.Append ("\nQuery source code: " + query.SourceCode);

			if (query.StackTrace != null)
			{
				sb.Append (query.StackTrace.ToString ());
			}

			sb.Append ("\n=========================================================================\n");

			string log = sb.ToString ();

			//System.Diagnostics.Debug.WriteLine (log);
			//System.Console.WriteLine (log);
		}


		protected override int GetNextNumber()
		{
			return this.nextNumber++;
		}


		private int nextNumber;


	}


}
