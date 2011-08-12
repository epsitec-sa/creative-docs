namespace Epsitec.Cresus.Core.Server.Modules
{
	public class MainModule : CoreModule
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
