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


		internal Query(string sourceCode, IEnumerable<Parameter> parameters, Result result, DateTime startTime, TimeSpan duration)
		{
			sourceCode.ThrowIfNull ("sourceCode");
			parameters.ThrowIfNull ("parameters");
			
			this.SourceCode = sourceCode;
			this.Parameters = parameters.ToList ().AsReadOnly ();
			this.Result = result;
			this.StartTime = startTime;
			this.Duration = duration;
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
