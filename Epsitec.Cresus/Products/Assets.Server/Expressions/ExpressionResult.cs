//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.Expression
{
	public struct ExpressionResult
	{
		public ExpressionResult(decimal? value, string trace, string error)
		{
			this.Value = value;
			this.Trace = trace;
			this.Error = error;
		}

		public bool HasError
		{
			get
			{
				return !string.IsNullOrEmpty (this.Error);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return !this.Value.HasValue
					&& this.Trace == null
					&& this.Error == null;
			}
		}

		public static ExpressionResult Empty = new ExpressionResult (null, null, null);

		public readonly decimal?			Value;
		public readonly string				Trace;
		public readonly string				Error;
	}
}
