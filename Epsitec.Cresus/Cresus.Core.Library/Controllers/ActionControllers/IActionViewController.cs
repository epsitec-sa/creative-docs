//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System;

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	public interface IActionViewController : IDisposable
	{
		FormattedText GetTitle();
		
		bool IsEnabled
		{
			get;
		}
	}
}
