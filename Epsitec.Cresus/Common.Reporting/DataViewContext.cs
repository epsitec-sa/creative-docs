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
			this.collectionSettings = new Dictionary<string, Settings.CollectionSetting> ();
			this.vectorSettings = new Dictionary<string, Settings.VectorSetting> ();

			this.DefaultCollectionSetting = new Settings.CollectionSetting ();
		}


		public Settings.CollectionSetting DefaultCollectionSetting
		{
			get;
			set;
		}

		public Settings.CollectionSetting GetCollectionSetting(string fullPath)
		{
			Settings.CollectionSetting setting = this.FindCollectionSetting (fullPath);
			return setting ?? this.DefaultCollectionSetting;
		}

		public Settings.VectorSetting GetVectorSetting(string fullPath)
		{
			Settings.VectorSetting setting;

			if (this.vectorSettings.TryGetValue (fullPath, out setting))
			{
				return setting;
			}
			else
			{
				//	TODO: ...

				return null;
			}
		}


		public void DefineCollectionSetting(string fullPath, Settings.CollectionSetting setting)
		{
			this.collectionSettings[fullPath] = setting;
		}

		public void DefineVectorSetting(string fullPath, Settings.VectorSetting setting)
		{
			this.vectorSettings[fullPath] = setting;
		}


		protected Settings.CollectionSetting FindCollectionSetting(string fullPath)
		{
			Settings.CollectionSetting setting;

			if (this.collectionSettings.TryGetValue (fullPath, out setting))
			{
				return setting;
			}
			else
			{
				return null;
			}
		}


		/// <summary>
		/// Converts the specified value to an entity enumeration, if this is
		/// possible. An adapter will be automatically generated when needed.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The entity enumeration or <c>null</c>.</returns>
		public IEnumerable<AbstractEntity> ToEntityEnumerable(object value)
		{
			if (value == null)
			{
				return null;
			}

			System.Type type = value.GetType ();
			EnumerableFactory factory;

			if (this.enumerableFactories.TryGetValue (type, out factory))
			{
				//	OK, we already have an enumerable factory for this type. Just
				//	re-use it.
			}
			else
			{
				//	Check if the value implements IEnumerable<T> and create the
				//	matching enumerable factory if it does. <T> must be derived
				//	from AbstractEntity !
				
				System.Type enumerableItemType = TypeRosetta.GetEnumerableItemType<AbstractEntity> (type);

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

				this.enumerableFactories[type] = factory;
			}

			return factory.GetEnumerable (value);
		}


		#region EnumerableFactory Class

		/// <summary>
		/// The <c>EnumerableFactory</c> class is used to retrieve the <see cref="System.Collections.IEnumerable"/>
		/// interface for any enumerable value, even if it does only implement the
		/// generic version of <c>IEnumerable</c>.
		/// </summary>
		abstract class EnumerableFactory
		{
			public abstract IEnumerable<AbstractEntity> GetEnumerable(object value);
		}

		#endregion

		#region NullEnumerableFactory Class

		/// <summary>
		/// The <c>NullEnumerableFactory</c> class always returns <c>null</c>.
		/// </summary>
		class NullEnumerableFactory : EnumerableFactory
		{
			public override IEnumerable<AbstractEntity> GetEnumerable(object value)
			{
				return null;
			}
		}

		#endregion

		#region GenericEnumerableFactory Class

		/// <summary>
		/// The <c>GenericEnumerableFactory&lt;T&gt;</c> class reimplements the
		/// <see cref="System.Collections.IEnumerable"/> interface for the value,
		/// using the generic version of <c>IEnumerable</c>.
		/// </summary>
		/// <typeparam name="T">The enumerable item type.</typeparam>
		class GenericEnumerableFactory<T> : EnumerableFactory where T : AbstractEntity
		{
			public override IEnumerable<AbstractEntity> GetEnumerable(object value)
			{
				IEnumerable<AbstractEntity> enumerable = value as IEnumerable<AbstractEntity>;

				if (enumerable == null)
				{
					return GenericEnumerableFactory<T>.CreateEnumerable (value as IEnumerable<T>);
				}
				else
				{
					return enumerable;
				}
			}

			private static IEnumerable<AbstractEntity> CreateEnumerable(IEnumerable<T> enumerable)
			{
				foreach (T item in enumerable)
				{
					yield return item;
				}
			}
		}

		#endregion


		private readonly Dictionary<System.Type, EnumerableFactory> enumerableFactories;
		private readonly Dictionary<string, Settings.CollectionSetting> collectionSettings;
		private readonly Dictionary<string, Settings.VectorSetting> vectorSettings;
	}
}
