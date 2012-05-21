using System.Collections.Generic;


namespace Epsitec.Cresus.Database.Logging
{


	public class ConsoleLog : AbstractLog
	{


		public ConsoleLog(ConsoleType output) : base ()
		{
			this.Output = output;
		}


		public ConsoleType Output
		{
			get;
			private set;
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
			string text = this.CreateText (query);

			this.LogText (text);			
		}


		private string CreateText(Query query)
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

			return sb.ToString ();
		}


		private void LogText(string text)
		{
			switch (this.Output)
			{
				case ConsoleType.Debug:
					System.Diagnostics.Debug.WriteLine (text);
					break;

				case ConsoleType.Console:
					System.Console.WriteLine (text);
					break;

				default:
					throw new System.NotImplementedException ();
			}
		}


		protected override int GetNextNumber()
		{
			return this.nextNumber++;
		}


		private int nextNumber;


	}


	public enum ConsoleType
	{
		Console,
		Debug,
	}


}
