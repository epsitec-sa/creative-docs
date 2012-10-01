//	Copyright © 2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;

using System;
using System.Collections.Generic;

using System.Linq.Expressions;


namespace Epsitec.Cresus.DataLayer.Loader
{

	/// <summary>
	/// The <c>Request&lt;TEntity&gt;</c> class is a specialized version of the
	/// <see cref="Request"/> class.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	public sealed class Request<TEntity> : Request
		where TEntity : AbstractEntity
	{


		public Request()
		{
		}

		public new TEntity						RootEntity
		{
			get
			{
				return (TEntity) base.RootEntity;
			}

			set
			{
				base.RootEntity = value;
			}
		}


		protected override Request CloneEmpty()
		{
			return new Request<TEntity> ();
		}


	}


}