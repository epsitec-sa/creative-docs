//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Resolvers;

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


		internal CoreData						Data
		{
			get
			{
				return this.data;
			}
		}

		internal DataContext					DataContext
		{
			get
			{
				return this.dataContext;
			}
		}

		internal bool							HasMapper
		{
			get
			{
				return this.mapper != null;
			}
		}


		public Repository DefineMapper(System.Func<AbstractEntity, AbstractEntity> mapper)
		{
			if (this.mapper == null)
			{
				this.mapper = mapper;
				return this;
			}

			var copy = RepositoryResolver.Clone (this) as Repository;
			return copy.DefineMapper (mapper);
		}
		
		public abstract Druid GetEntityType();



		protected T Map<T>(T entity)
			where T : AbstractEntity
		{
			return this.mapper (entity) as T;
		}

		private readonly DataLifetimeExpectancy lifetimeExpectancy;
		protected readonly CoreData				data;
		protected readonly DataContext			dataContext;
		private System.Func<AbstractEntity, AbstractEntity> mapper;
	}
}