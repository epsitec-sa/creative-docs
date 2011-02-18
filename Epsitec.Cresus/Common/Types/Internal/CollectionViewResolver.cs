//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Internal
{
	/// <summary>
	/// The <c>CollectionViewResolver</c> class is used to map collections to their
	/// default <see cref="ICollectionView"/>. The mapping is done relative to a
	/// binding data context.
	/// </summary>
	public class CollectionViewResolver
	{
		private CollectionViewResolver()
		{
		}

		/// <summary>
		/// Gets the collection view. If there is no <see cref="ICollectionView"/>
		/// for the collection object, it will be created automatically.
		/// </summary>
		/// <param name="binding">The data context binding.</param>
		/// <param name="collection">The collection object.</param>
		/// <returns>
		/// An <c>ICollectionView</c> for the specified collection or
		/// <c>null</c> if the collection is not supported.
		/// </returns>
		public ICollectionView GetCollectionView(Binding binding, object collection)
		{
			return this.GetCollectionView (binding, collection, true);
		}

		/// <summary>
		/// Gets the collection view. If there is no <see cref="ICollectionView"/>
		/// for the collection object, optionally create it.
		/// </summary>
		/// <param name="binding">The data context binding.</param>
		/// <param name="collection">The collection object.</param>
		/// <param name="autoCreate">If set to <c>true</c>, automatically creates
		/// the <c>ICollectionView</c> when needed.</param>
		/// <returns>
		/// An <c>ICollectionView</c> for the specified collection or
		/// <c>null</c> if the collection is not supported.
		/// </returns>
		public ICollectionView GetCollectionView(Binding binding, object collection, bool autoCreate)
		{
			if (CollectionViewResolver.IsCollectionViewCompatible (collection))
			{
				Context context = this.GetContext (binding);
				ViewDef viewDef = context.GetViewDef (collection, autoCreate);

				if ((System.Threading.Interlocked.Increment (ref this.counter) % 1000) == 999)
				{
					this.FreeDeadContexts ();
				}

				return viewDef.View;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Frees the dead contexts.
		/// </summary>
		public void FreeDeadContexts()
		{
			//	TODO: make sure that this gets invoked when a binding object attached to a context dies...
			
			System.Threading.Interlocked.Exchange (ref this.counter, 0);
			
			lock (this.exclusion)
			{
				this.contexts.RemoveAll
					(
						delegate (Context item)
						{
							if (item.Binding == null)
							{
								item.Dispose ();
								return true;
							}
							else
							{
								return false;
							}
						}
					);
			}
		}

		/// <summary>
		/// Determines whether the specified collection object is compatible with <c>ICollectionView</c>.
		/// </summary>
		/// <param name="collection">The collection object.</param>
		/// <returns>
		/// 	<c>true</c> if the specified collection object is compatible with <c>ICollectionView</c>; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsCollectionViewCompatible(object collection)
		{
			if (collection is System.Collections.IList)
			{
				return true;
			}

			return false;
		}

		private static ICollectionView CreateCollectionView(object collection)
		{
			System.Collections.IList list = collection as System.Collections.IList;
			
			if (list != null)
			{
				return new CollectionView (list);
			}
			
			return null;
		}

		private Context GetContext(Binding binding)
		{
			lock (this.exclusion)
			{
				Context context = null;
				int     index   = this.contexts.FindIndex
					(
						delegate (Context item)
						{
							if (item.Binding == binding)
							{
								context = item;
								return true;
							}
							else
							{
								return false;
							}
						}
					);

				if (index < 0)
				{
					context = new Context (binding);
					this.contexts.Insert (0, context);
				}
				else if (index > 0)
				{
					this.contexts.RemoveAt (index);
					this.contexts.Insert (0, context);
				}

				return context;
			}
		}

		#region Private Context Class

		/// <summary>
		/// The <c>Context</c> class associates a set of collection/view pairs
		/// represented by <c>ViewDef</c> with a given binding (data context).
		/// </summary>
		private class Context
		{
			public Context(Binding binding)
			{
				this.Binding = binding;
			}

			public Binding						Binding
			{
				get
				{
					return this.binding == null ? null : this.binding.Target;
				}
				set
				{
					this.binding = value == null ? null : new Weak<Binding> (value);
				}
			}


			/// <summary>
			/// Gets the <c>ViewDef</c> for the specified collection object.
			/// </summary>
			/// <param name="collection">The collection object.</param>
			/// <param name="autoCreate">If set to <c>true</c>, the <c>ICollectionView</c>
			/// will be created automatically if none can be found for the
			/// collection object.</param>
			/// <returns>The <c>ViewDef</c> for the collection.</returns>
			public ViewDef GetViewDef(object collection, bool autoCreate)
			{
				lock (this.exclusion)
				{
					ViewDef viewDef = new ViewDef ();
					int     index   = this.viewDefs.FindIndex
						(
							delegate (ViewDef item)
							{
								if (item.Collection == collection)
								{
									viewDef = item;
									return true;
								}
								else
								{
									return false;
								}
							}
						);

					if ((index < 0) &&
						(autoCreate))
					{
						//	The collection has no view associated with it. Create one
						//	and record it for further references:

						viewDef = new ViewDef (collection, CollectionViewResolver.CreateCollectionView (collection));
						this.viewDefs.Insert (0, viewDef);
					}
					else if (index > 0)
					{
						//	The view/collection pair is not at the beginning of
						//	our internal array; move it there so that we find it
						//	faster next time.
						
						this.viewDefs.RemoveAt (index);
						this.viewDefs.Insert (0, viewDef);
					}

					return viewDef;
				}
			}


			/// <summary>
			/// Disposes this instance.
			/// </summary>
			public void Dispose()
			{
				foreach (ViewDef viewDef in this.viewDefs)
				{
					System.IDisposable disposable = viewDef.View as System.IDisposable;
					
					if (disposable != null)
					{
						disposable.Dispose ();
					}
				}
				
				this.viewDefs.Clear ();
			}
			
			private Weak<Binding>				binding;
			private object						exclusion = new object ();
			private List<ViewDef>				viewDefs = new List<ViewDef> ();
		}

		#endregion

		#region Private ViewDef Structure

		/// <summary>
		/// The <c>ViewDef</c> structure is used to represent a collection/view
		/// pair.
		/// </summary>
		private struct ViewDef
		{
			public ViewDef(object collection, ICollectionView view)
			{
				this.collection = collection;
				this.view = view;
			}

			public object						Collection
			{
				get
				{
					return this.collection;
				}
			}

			public ICollectionView				View
			{
				get
				{
					return this.view;
				}
			}

			private object						collection;
			private ICollectionView				view;
		}

		#endregion
		
		public static readonly CollectionViewResolver Default = new CollectionViewResolver ();

		private object							exclusion = new object ();
		private List<Context>					contexts  = new List<Context> ();
		private int								counter;
	}
}
