//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Library.Formatters;
using Epsitec.Cresus.Core.Resolvers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	/// <summary>
	/// The <c>FormatTokenFormatterResolver</c> class is used to find all classes
	/// which implement <see cref="IFormatTokenFormatter"/> and get the collection
	/// of their associated format tokens (see <see cref="FormatToken"/>).
	/// </summary>
	public sealed class FormatTokenFormatterResolver
	{
		/// <summary>
		/// Gets the collection of all the dynamic format tokens.
		/// </summary>
		/// <returns>The collection of all the dynamic format tokens.</returns>
		public static IEnumerable<FormatToken> GetFormatTokens()
		{
			return FormatTokenFormatterResolver.Resolve ().Select (x => x.GetFormatToken ());
		}


		/// <summary>
		/// Gets the collection of all classes which implement <see cref="IFormatTokenFormatter"/>.
		/// </summary>
		/// <returns>The collection of all classes which implement <see cref="IFormatTokenFormatter"/>.</returns>
		private static IEnumerable<IFormatTokenFormatter> Resolve()
		{
			if (FormatTokenFormatterResolver.formatters == null)
			{
				FormatTokenFormatterResolver.formatters = InterfaceImplementationResolver<IFormatTokenFormatter>.CreateInstances ().ToList ();
			}

			return FormatTokenFormatterResolver.formatters;
		}

		[System.ThreadStatic]
		private static List<IFormatTokenFormatter> formatters;
	}
}