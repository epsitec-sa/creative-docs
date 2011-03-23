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
				PrettyPrinterResolver.Setup ();
			}

			IPrettyPrinter prettyPrinter;

			if (PrettyPrinterResolver.prettyPrinters.TryGetValue (type, out prettyPrinter))
			{
				return prettyPrinter;
			}
			else
			{
				return null;
			}
		}

		private static void Setup()
		{
			PrettyPrinterResolver.prettyPrinters = new Dictionary<System.Type, IPrettyPrinter> ();

			foreach (var item in InterfaceImplementationResolver<IPrettyPrinter>.CreateInstances ())
			{
				foreach (var convertibleType in item.GetConvertibleTypes ())
				{
					PrettyPrinterResolver.prettyPrinters[convertibleType] = item;
				}
			}
		}

		[System.ThreadStatic]
		private static Dictionary<System.Type, IPrettyPrinter> prettyPrinters;
	}
}
