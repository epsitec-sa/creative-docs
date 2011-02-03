using Epsitec.Common.Support.Extensions;

using System;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;


namespace Epsitec.Cresus.Database.Logging
{


	// TODO Comment this class.
	// Marc


	public sealed class Query
	{


		internal Query(int number, string sourceCode, IEnumerable<Parameter> parameters, Result result, DateTime startTime, TimeSpan duration)
		{
			number.ThrowIf (n => n < 0, "number is smaller than zero");
			sourceCode.ThrowIfNull ("sourceCode");
			parameters.ThrowIfNull ("parameters");

			this.Number = number;
			this.SourceCode = sourceCode;
			this.Parameters = parameters.ToList ().AsReadOnly ();
			this.Result = result;
			this.StartTime = startTime;
			this.Duration = duration;
		}


		public int Number
		{
			get;
			private set;
		}


		public string SourceCode
		{
			get;
			private set;
		}


		public ReadOnlyCollection<Parameter> Parameters
		{
			get;
			private set;
		}


		public DateTime StartTime
		{
			get;
			private set;
		}


		public TimeSpan Duration
		{
			get;
			private set;
		}


		public Result Result
		{
			get;
			private set;
		}


	}


}
