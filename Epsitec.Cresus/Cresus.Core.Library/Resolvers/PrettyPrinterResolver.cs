//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	public sealed class PrettyPrinterResolver
	{
		public static IPrettyPrinter Resolve(System.Type type)
		{
			if (PrettyPrinterResolver.prettyPrinters == null)
			{
				PrettyPrinterResolver.prettyPrinters = InterfaceImplementationResolver<IPrettyPrinter>.CreateInstances ().ToList ();
			}

			return PrettyPrinterResolver.prettyPrinters.Where (x => x.CanConvertToFormattedText (type)).FirstOrDefault ();
		}

		[System.ThreadStatic]
		private static List<IPrettyPrinter>		prettyPrinters;
	}
}
