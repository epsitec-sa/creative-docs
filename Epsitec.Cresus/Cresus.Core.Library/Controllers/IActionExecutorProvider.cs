using Epsitec.Cresus.Bricks;

namespace Epsitec.Cresus.Core.Controllers
{
	public interface IActionExecutorProvider
	{
		ActionExecutor GetExecutor();
	}
}