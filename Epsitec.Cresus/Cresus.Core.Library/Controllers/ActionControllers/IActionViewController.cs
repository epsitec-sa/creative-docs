using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using System;

namespace Epsitec.Cresus.Core.Controllers.ActionControllers
{
	public interface IActionViewController
	{
		FormattedText GetTitle();

		ActionExecutor GetExecutor();
	}
}
