//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Reporting
{
	public interface IContent
	{
		string Format { get; }
		byte[] GetContentBlob();
		IContent Setup(byte[] blob);
		FormattedText GetContentText(string template);
	}
}
