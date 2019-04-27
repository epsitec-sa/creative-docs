//	Copyright © 2010-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	/// <summary>
	/// The <c>AbstractEntityExtensions</c> class provides extension methods for the <see cref="AbstractEntity"/>
	/// class.
	/// </summary>
	public static class AbstractEntityExtensions
	{
		public static bool IsNull(this AbstractEntity entity)
		{
			return entity.UnwrapNullEntity () == null;
		}

		public static bool IsNotNull(this AbstractEntity entity)
		{
			return entity.UnwrapNullEntity () != null;
		}

		public static DataContext GetDataContext(this AbstractEntity entity)
		{
			return DataContextPool.GetDataContext (entity);
		}


		public static IList<T> GetVirtualCollection<T>(this AbstractEntity entity, ref IList<T> cache, System.Action<T> exampleSetter)
			where T : AbstractEntity, new ()
		{
			if (cache == null)
			{
				cache = entity.ExecuteWithDataContext<IList<T>> (d => entity.FindVirtualCollection<T> (d, exampleSetter), () => new List<T> ());
			}

			return cache;
		}

		public static ISet<T> GetVirtualCollection<T>(this AbstractEntity entity, ref ISet<T> cache, System.Action<T> exampleSetter)
			where T : AbstractEntity, new ()
		{
			if (cache == null)
			{
				cache = entity.ExecuteWithDataContext<IList<T>> (d => entity.FindVirtualCollection<T> (d, exampleSetter), () => new List<T> ()).ToSet ();
			}

			return cache;
		}

		public static IList<T> FindVirtualCollection<T>(this AbstractEntity entity, DataContext dataContext, System.Action<T> exampleSetter)
			where T : AbstractEntity, new ()
		{
			T example = new T ();
			exampleSetter (example);
			return dataContext.GetByExample (example);
		}
	

		public static T ExecuteWithDataContext<T>(this AbstractEntity entity,
												  System.Func<DataContext, T> functionWithDataContext,
												  System.Func<T> defaultFunction)
		{
			var dataContext = entity.GetDataContext ();

			if ((dataContext != null) &&
				(dataContext.IsPersistent (entity)))
			{
				return functionWithDataContext (dataContext);
			}
			else
			{
				return defaultFunction ();
			}
		}

		public static void ExecuteWithDataContext(this AbstractEntity entity,
												  System.Action<DataContext> functionWithDataContext)
		{
			var dataContext = entity.GetDataContext ();

			if ((dataContext != null) &&
				(dataContext.IsPersistent (entity)))
			{
				functionWithDataContext (dataContext);
			}
		}

		/// <summary>
		/// Compares two entities and returns <c>true</c> if they refer to the same database key
		/// or if they are the same memory instance.
		/// </summary>
		/// <param name="entityA">The first entity.</param>
		/// <param name="entityB">The second entity.</param>
		/// <returns><c>true</c> if both entities refer to the same database key; otherwise, <c>false</c>.</returns>
		public static bool DbKeyEquals(this AbstractEntity entityA, AbstractEntity entityB)
		{
			if (entityA.RefEquals (entityB))
			{
				return true;
			}
			
			if ((entityA == null) ||
				(entityB == null))
			{
				return false;
			}

			DataContext contextA = entityA.GetDataContext ();
			DataContext contextB = entityB.GetDataContext ();

			if ((contextA == null) ||
				(contextB == null))
			{
				return false;
			}

			var keyA = contextA.GetNormalizedEntityKey (entityA);
			var keyB = contextB.GetNormalizedEntityKey (entityB);

			if ((keyA.HasValue) &&
				(keyB.HasValue))
			{
				return keyA.Value == keyB.Value;
			}

			return false;
		}

		public static bool RefEquals(this AbstractEntity that, AbstractEntity other)
		{
			return that.UnwrapNullEntity () == other.UnwrapNullEntity ();
		}
		
		public static bool RefDiffers(this AbstractEntity that, AbstractEntity other)
		{
			return that.UnwrapNullEntity () != other.UnwrapNullEntity ();
		}

		public static TOut GetValueOrDefault<TIn, TOut>(this TIn entity, System.Func<TIn, TOut> function)
			where TIn : AbstractEntity
		{
			return entity.IsNull ()
				? default (TOut)
				: function (entity);
		}
	}
}
