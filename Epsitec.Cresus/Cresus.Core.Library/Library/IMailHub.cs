//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using Epsitec.Common.Types;
using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Core.Library
{
	public interface IMailHub
	{
		void SendEmail(IEnumerable<string> to,IEnumerable<string> cc, string from,string subjet, string content);
	}
}