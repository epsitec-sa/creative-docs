//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	public sealed class ReferenceNumberFormatterResolver
	{
		public static IEnumerable<IReferenceNumberFormatter> Resolve()
		{
			if (ReferenceNumberFormatterResolver.formatters == null)
			{
				ReferenceNumberFormatterResolver.formatters = InterfaceImplementationResolver<IReferenceNumberFormatter>.CreateInstances ().ToList ();
			}

			return ReferenceNumberFormatterResolver.formatters;
		}

		public static IEnumerable<FormatToken> GetTokens()
		{
			return ReferenceNumberFormatterResolver.Resolve ().Select (x => x.FormatToken);
		}

		[System.ThreadStatic]
		private static List<IReferenceNumberFormatter> formatters;
	}
}