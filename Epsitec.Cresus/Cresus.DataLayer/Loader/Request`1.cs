//	Copyright © 2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX

using Epsitec.Common.Support.EntityEngine;

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


		public new TEntity RootEntity
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


		public new TEntity RequestedEntity
		{
			get
			{
				return (TEntity) base.RequestedEntity;
			}
			set
			{
				base.RequestedEntity = value;
			}
		}


		protected override Request CloneEmpty()
		{
			return new Request<TEntity> ();
		}


	}


}