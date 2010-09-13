//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Repositories
{
	public abstract class Repository
	{
		protected Repository(CoreData data, DataContext context, DataLifetimeExpectancy lifetimeExpectancy)
		{
			this.lifetimeExpectancy = lifetimeExpectancy;
			
			this.data = data;
			this.dataContext = context ?? this.data.GetDataContext (this.lifetimeExpectancy);
		}


		public abstract Druid GetEntityType();


		private readonly DataLifetimeExpectancy lifetimeExpectancy;
		protected readonly CoreData data;
		protected readonly DataContext dataContext;
	}
}