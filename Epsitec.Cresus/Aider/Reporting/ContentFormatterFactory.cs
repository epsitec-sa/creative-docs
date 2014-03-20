//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.PlugIns;
using Epsitec.Common.Types;

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Reporting
{
	/// <summary>
	/// The <c>ContentFormatterFactory</c> class implements the factory which
	/// creates the classes implementing <see cref="IContent"/>, based on the
	/// format name.
	/// </summary>
	public sealed class ContentFormatterFactory : PlugInFactory<IContent, ContentFormatterAttribute, string>
	{
		public static IContent Create(string format)
		{
			return ContentFormatterFactory.CreateInstance (format);
		}
	}
}

