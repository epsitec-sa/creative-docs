using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;


namespace Epsitec.Aider.Override
{


	public class AiderDatabaseInitializer : DatabaseInitializer
	{


		public AiderDatabaseInitializer(BusinessContext businessContext)
			: base (businessContext)
		{
		}
		

		public override void Run()
		{
			base.Run ();
		}


	}


}
