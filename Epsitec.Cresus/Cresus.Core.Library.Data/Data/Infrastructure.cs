//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Infrastructure;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	public sealed class Infrastructure : CoreDataComponent, System.IDisposable
	{
		public Infrastructure(CoreData data)
			: base (data)
		{
			this.dbInfrastructure = new DbInfrastructure ();
			this.dataInfrastructure = new DataLayer.Infrastructure.DataInfrastructure (this.dbInfrastructure);
		}


		public DataInfrastructure DataInfrastructure
		{
			get
			{
				return this.dataInfrastructure;
			}
		}

		public override void ExecuteSetupPhase()
		{
			base.ExecuteSetupPhase ();
			
			//	TODO: ...
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.dataInfrastructure.Dispose ();

			if (this.dbInfrastructure.IsConnectionOpen)
			{
				this.dbInfrastructure.Dispose ();
			}
		}

		#endregion
		
		
		private readonly DbInfrastructure dbInfrastructure;
		private readonly DataLayer.Infrastructure.DataInfrastructure dataInfrastructure;
	}
}
