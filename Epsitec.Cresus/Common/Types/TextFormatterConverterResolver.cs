//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types
{
	public sealed class TextFormatterConverterResolver
	{
		public static ITextFormatterConverter Resolve(System.Type type)
		{
			if (TextFormatterConverterResolver.converters == null)
			{
				TextFormatterConverterResolver.Setup ();
			}

			ITextFormatterConverter prettyPrinter;

			if (TextFormatterConverterResolver.converters.TryGetValue (type, out prettyPrinter))
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
			TextFormatterConverterResolver.converters = new Dictionary<System.Type, ITextFormatterConverter> ();

			foreach (var item in InterfaceImplementationResolver<ITextFormatterConverter>.CreateInstances ())
			{
				foreach (var convertibleType in item.GetConvertibleTypes ())
				{
					TextFormatterConverterResolver.converters[convertibleType] = item;
				}
			}
		}

		[System.ThreadStatic]
		private static Dictionary<System.Type, ITextFormatterConverter> converters;
	}
}
