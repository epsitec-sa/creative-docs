//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>CollectionViewGrop</c> class represents a group created by a
	/// <see cref="CollectionView"/> object, based on a collection of
	/// <see cref="GroupDescription"/> objects.
	/// </summary>
	public class CollectionViewGroup : INotifyPropertyChanged
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CollectionViewGroup"/> class.
		/// </summary>
		/// <param name="name">The name of the group.</param>
		/// <param name="parentGroup">The parent group.</param>
		internal CollectionViewGroup(string name, CollectionViewGroup parentGroup)
		{
			this.name = name;
			this.parentGroup = parentGroup;
		}

		/// <summary>
		/// Gets a value indicating whether this group has any subgroups.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this group has subgroups; otherwise, <c>false</c>.
		/// </value>
		public bool								HasSubgroups
		{
			get
			{
				lock (this.exclusion)
				{
					return (this.subgroups != null)
						&& (this.subgroups.Count > 0);
				}
			}
		}

		/// <summary>
		/// Gets the number of items in this group (this will also count
		/// the items found in the subgroups, if any).
		/// </summary>
		/// <value>The number of items in this group.</value>
		public int								ItemCount
		{
			get
			{
				int count = this.itemCount;

				if (count == -1)
				{
					count = this.RefreshItemCount ();
				}

				return count;
			}
		}

		/// <summary>
		/// Gets the name of this group.
		/// </summary>
		/// <value>The name of this group.</value>
		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		/// <summary>
		/// Gets the items contained in this group (and possibly its subgroups).
		/// </summary>
		/// <value>The items containes in this group.</value>
		public IList<object>					Items
		{
			get
			{
				return new Internal.CollectionViewGroupItems (this);
			}
		}

		/// <summary>
		/// Gets the parent group (internal use only).
		/// </summary>
		/// <value>The parent group.</value>
		internal CollectionViewGroup			ParentGroup
		{
			get
			{
				return this.parentGroup;
			}
		}

		/// <summary>
		/// Gets the subgroups contained in this group.
		/// </summary>
		/// <value>The subgroups or an empty list if there are no subgroups.</value>
		public Collections.ReadOnlyList<CollectionViewGroup> Subgroups
		{
			get
			{
				IList<CollectionViewGroup> subgroups;

				lock (this.exclusion)
				{
					subgroups = this.subgroups;
				}

				if ((subgroups != null) &&
					(subgroups.Count > 0))
				{
					return new Collections.ReadOnlyList<CollectionViewGroup> (subgroups);
				}
				else
				{
					return Collections.ReadOnlyList<CollectionViewGroup>.Empty;
				}
			}
		}

		private void HandleItemsCollectionChanged(object sender, CollectionChangedEventArgs e)
		{
			this.InvalidateItemCount ();
			this.OnPropertyChanged (new DependencyPropertyChangedEventArgs ("Items"));
			
			if (this.parentGroup != null)
			{
				this.parentGroup.OnPropertyChanged (new DependencyPropertyChangedEventArgs ("Items"));
			}
		}

		private void HandleSubgroupsCollectionChanged(object sender, CollectionChangedEventArgs e)
		{
			this.InvalidateItemCount ();
			
			this.OnPropertyChanged (new DependencyPropertyChangedEventArgs ("Subgroups"));
			this.OnPropertyChanged (new DependencyPropertyChangedEventArgs ("Items"));

			if (this.parentGroup != null)
			{
				this.parentGroup.OnPropertyChanged (new DependencyPropertyChangedEventArgs ("Items"));
			}
		}

		protected virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			Support.EventHandler<DependencyPropertyChangedEventArgs> handler;

			lock (this.exclusion)
			{
				handler = this.propertyChangedEvent;
			}

			if (handler != null)
			{
				handler (this, e);
			}
		}

		protected int RefreshItemCount()
		{
			int count = 0;

			lock (this.exclusion)
			{
				if (this.HasSubgroups)
				{
					foreach (CollectionViewGroup group in this.subgroups)
					{
						count += group.ItemCount;
					}
				}
				else if (this.items != null)
				{
					count = this.items.Count;
				}

				this.itemCount = count;
			}
			
			return count;
		}
		
		protected void InvalidateItemCount()
		{
			if (this.itemCount != -1)
			{
				lock (this.exclusion)
				{
					this.itemCount = -1;
				}
				
				if (this.parentGroup != null)
				{
					this.parentGroup.InvalidateItemCount ();
				}
			}
		}

		/// <summary>
		/// Gets the local item collection; create one if none exists yet.
		/// </summary>
		/// <returns>The local item collection.</returns>
		internal Collections.ObservableList<object> GetItems()
		{
			if (this.items == null)
			{
				lock (this.exclusion)
				{
					if (this.items == null)
					{
						this.items = new Collections.ObservableList<object> ();
						this.items.CollectionChanged += this.HandleItemsCollectionChanged;
					}
				}
			}
			
			return this.items;
		}

		/// <summary>
		/// Gets the subgroup collection; create one if none exists yet.
		/// </summary>
		/// <returns>The subgroup collection.</returns>
		internal Collections.ObservableList<CollectionViewGroup> GetSubgroups()
		{
			if (this.subgroups == null)
			{
				lock (this.exclusion)
				{
					if (this.subgroups == null)
					{
						this.subgroups = new Collections.ObservableList<CollectionViewGroup> ();
						this.subgroups.CollectionChanged += this.HandleSubgroupsCollectionChanged;
					}
				}
			}

			return this.subgroups;
		}

		/// <summary>
		/// Clears the subgroup collection.
		/// </summary>
		internal void ClearSubgroups()
		{
			lock (this.exclusion)
			{
				if (this.subgroups != null)
				{
					this.subgroups.Clear ();
				}
			}
		}

		/// <summary>
		/// Clears the local item collection.
		/// </summary>
		internal void ClearItems()
		{
			lock (this.exclusion)
			{
				if (this.items != null)
				{
					this.items.Clear ();
				}
			}
		}
		
		#region INotifyPropertyChanged Members

		public event Support.EventHandler<DependencyPropertyChangedEventArgs> PropertyChanged
		{
			add
			{
				lock (this.exclusion)
				{
					this.propertyChangedEvent += value;
				}
			}
			remove
			{
				lock (this.exclusion)
				{
					this.propertyChangedEvent -= value;
				}
			}
		}

		#endregion


		private readonly object					exclusion = new object();
		private readonly string					name;
		private readonly CollectionViewGroup	parentGroup;
		private int								itemCount;
		
		private Collections.ObservableList<object> items;
		private Collections.ObservableList<CollectionViewGroup> subgroups;
		private Support.EventHandler<DependencyPropertyChangedEventArgs> propertyChangedEvent;
	}
}
