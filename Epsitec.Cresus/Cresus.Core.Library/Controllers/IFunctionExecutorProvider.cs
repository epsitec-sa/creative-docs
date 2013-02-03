using Epsitec.Cresus.Bricks;

namespace Epsitec.Cresus.Core.Controllers
{
	public interface IFunctionExecutorProvider
	{
		FunctionExecutor GetExecutor();
	}
}
