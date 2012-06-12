//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>TextFormatterConverterResolver</c> class is used to find a pretty printer
	/// (<see cref=" ITextFormatterConverter"/>) for a given data type.
	/// </summary>
	public sealed class TextFormatterConverterResolver
	{
		/// <summary>
		/// Resolves the pretty printer for the specified data type.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		/// <returns>A pretty printer or <c>null</c>.</returns>
		public static ITextFormatterConverter Resolve(System.Type dataType)
		{
			if (TextFormatterConverterResolver.converters == null)
			{
				TextFormatterConverterResolver.Setup ();
			}

			ITextFormatterConverter prettyPrinter;

			if (TextFormatterConverterResolver.converters.TryGetValue (dataType, out prettyPrinter))
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
