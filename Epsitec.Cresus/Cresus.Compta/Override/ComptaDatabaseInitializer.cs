using Epsitec.Cresus.Compta.Entities;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;

namespace Epsitec.Cresus.Compta.Override
{
	public class ComptaDatabaseInitializer : DatabaseInitializer
	{
		public override void Run(BusinessContext businessContext)
		{
			base.Run (businessContext);

			this.CreateTestUsers (businessContext);
			this.CreateCompta (businessContext);
		}

		private void CreateCompta(BusinessContext businessContext)
		{
			businessContext.CreateAndRegisterEntity<ComptaEntity> ();

			businessContext.SaveChanges (LockingPolicy.ReleaseLock);
		}
	}
}

