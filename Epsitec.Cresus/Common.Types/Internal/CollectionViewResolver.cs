//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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

		public ICollectionView GetCollectionView(Binding binding, object collection)
		{
			if (CollectionViewResolver.IsCollectionViewCompatible (collection))
			{
				Context context = this.GetContext (binding);
				ViewDef viewDef = context.GetViewDef (collection);

				if ((System.Threading.Interlocked.Increment (ref this.counter) % 1000) == 999)
				{
					this.TrimContexts ();
				}

				return viewDef.View;
			}
			else
			{
				return null;
			}
		}

		public void TrimContexts()
		{
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

			public ViewDef GetViewDef(object collection)
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

					if (index < 0)
					{
						viewDef = new ViewDef (collection, CollectionViewResolver.CreateCollectionView (collection));
						this.viewDefs.Insert (0, viewDef);
					}
					else if (index > 0)
					{
						this.viewDefs.RemoveAt (index);
						this.viewDefs.Insert (0, viewDef);
					}

					return viewDef;
				}
			}

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
