//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Reporting
{
	/// <summary>
	/// The <c>IContentTextProducer</c> interface produces a formatted text based
	/// on a template and its own data.
	/// </summary>
	public interface IContentTextProducer
	{
		FormattedText GetFormattedText(string template);
	}
}

