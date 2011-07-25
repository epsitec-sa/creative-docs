using Nancy;

namespace Epsitec.Cresus.Core.Server
{
	public class MainModule : NancyModule
	{
		public MainModule()
		{
			Get["/"] = parameters =>
			{
				return "Hello World";
			};
		}
	}
}
