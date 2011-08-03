
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

			Get["/login"] = parameters =>
			{
				// Init session
				GetCoreSession ();

				return "logged in";


				//var res = Response.AsJson (obj);
				//res.Headers["Access-Control-Allow-Origin"] = "*";
			}; 
		}
	}
}
