//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public sealed class CoreDataLocker : System.IDisposable
	{
		public CoreDataLocker(DataInfrastructure dataInfrastructure)
		{
			this.dataInfrastructure = dataInfrastructure;
		}

		
		public bool IsReady
		{
			get
			{
				return this.isReady;
			}
		}

		
		public void Validate()
		{
			if (!this.isReady)
			{
				this.isReady = true;
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
		
		private readonly DataInfrastructure dataInfrastructure;

		private bool isReady;
	}
}
