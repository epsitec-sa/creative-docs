//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	public class DataViewContext
	{
		public DataViewContext()
		{
			this.enumerableFactories = new Dictionary<System.Type, EnumerableFactory> ();
		}


		public System.Collections.IEnumerable GetEnumerable(object value)
		{
			if (value == null)
			{
				return null;
			}

			System.Type type = value.GetType ();
			EnumerableFactory factory;

			if (!this.enumerableFactories.TryGetValue (type, out factory))
			{
				if (TypeRosetta.DoesTypeImplementInterface (type, typeof (System.Collections.IEnumerable)))
				{
					factory = new TransparentEnumerableFactory ();
				}
				else
				{
					//	Check if the value implements IEnumerable<T> and create the
					//	matching enumerable factory.
					
					System.Type enumerableItemType = TypeRosetta.GetEnumerableItemType (type);

					if (enumerableItemType != null)
					{
						System.Type genericType  = typeof (GenericEnumerableFactory<>);
						System.Type concreteType = genericType.MakeGenericType (new System.Type[] { enumerableItemType });

						Allocator<EnumerableFactory> factoryAllocator = DynamicCodeFactory.CreateAllocator<EnumerableFactory> (concreteType);
						factory = factoryAllocator.Invoke ();
					}
					else
					{
						factory = new NullEnumerableFactory ();
					}
				}

				this.enumerableFactories[type] = factory;
			}

			return factory.GetEnumerable (value);
		}


		#region EnumerableFactory Classes

		/// <summary>
		/// The <c>EnumerableFactory</c> class is used to retrieve the <see cref="System.Collections.IEnumerable"/>
		/// interface for any enumerable value, even if it does only implement the
		/// generic version of <c>IEnumerable</c>.
		/// </summary>
		abstract class EnumerableFactory
		{
			public abstract System.Collections.IEnumerable GetEnumerable(object value);
		}

		/// <summary>
		/// The <c>NullEnumerableFactory</c> class always returns <c>null</c>.
		/// </summary>
		class NullEnumerableFactory : EnumerableFactory
		{
			public override System.Collections.IEnumerable GetEnumerable(object value)
			{
				return null;
			}
		}

		/// <summary>
		/// The <c>TransparentEnumerableFactory</c> class casts the value to its
		/// <see cref="System.Collections.IEnumerable"/> implementation.
		/// </summary>
		class TransparentEnumerableFactory : EnumerableFactory
		{
			public override System.Collections.IEnumerable GetEnumerable(object value)
			{
				return value as System.Collections.IEnumerable;
			}
		}

		/// <summary>
		/// The <c>GenericEnumerableFactory&lt;T&gt;</c> class reimplements the
		/// <see cref="System.Collections.IEnumerable"/> interface for the value,
		/// using the generic version of <c>IEnumerable</c>.
		/// </summary>
		/// <typeparam name="T">The enumerable item type.</typeparam>
		class GenericEnumerableFactory<T> : EnumerableFactory
		{
			public override System.Collections.IEnumerable GetEnumerable(object value)
			{
				System.Collections.IEnumerable enumerable = value as System.Collections.IEnumerable;

				if (enumerable == null)
				{
					return GenericEnumerableFactory<T>.CreateEnumerable (value as IEnumerable<T>);
				}
				else
				{
					return enumerable;
				}
			}

			private static System.Collections.IEnumerable CreateEnumerable(IEnumerable<T> enumerable)
			{
				foreach (T item in enumerable)
				{
					yield return item;
				}
			}
		}

		#endregion


		private readonly Dictionary<System.Type, EnumerableFactory> enumerableFactories;
	}
}
