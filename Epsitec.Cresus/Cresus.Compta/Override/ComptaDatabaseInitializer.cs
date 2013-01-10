using Epsitec.Cresus.Compta.Entities;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Compta.Override
{
	public class ComptaDatabaseInitializer : DatabaseInitializer
	{
		public ComptaDatabaseInitializer(BusinessContext businessContext)
			: base (businessContext)
		{
		}

		public override void Run()
		{
			base.Run ();

			this.CreateTestUsers ();
			this.CreateCompta ();
		}

		private void CreateCompta()
		{
			this.BusinessContext.CreateAndRegisterEntity<ComptaEntity> ();
		}
	}
}

