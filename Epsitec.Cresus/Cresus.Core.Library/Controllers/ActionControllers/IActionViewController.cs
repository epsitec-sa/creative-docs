using Epsitec.Common.Types;

using System;

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	public interface IActionViewController : IDisposable
	{
		FormattedText GetTitle();
	}
}
