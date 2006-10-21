//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
				return (this.subgroups != null)
					&& (this.subgroups.Count > 0);
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
				if (this.itemCount == -1)
				{
					this.RefreshItemCount ();
				}

				return this.itemCount;
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
		public IEnumerable<object>				Items
		{
			get
			{
				return new Internal.CollectionViewGroupItems (this);
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
				if (this.HasSubgroups)
				{
					return new Collections.ReadOnlyList<CollectionViewGroup> (this.subgroups);
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
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged (this, e);
			}
		}

		protected void RefreshItemCount()
		{
			int count = 0;

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
		
		protected void InvalidateItemCount()
		{
			if (this.itemCount != -1)
			{
				this.itemCount = -1;
				
				if (this.parentGroup != null)
				{
					this.parentGroup.InvalidateItemCount ();
				}
			}
		}
		
		internal Collections.ObservableList<object> GetItems()
		{
			if (this.items == null)
			{
				this.items = new Collections.ObservableList<object> ();
				this.items.CollectionChanged += this.HandleItemsCollectionChanged;
			}
			
			return this.items;
		}

		internal Collections.ObservableList<CollectionViewGroup> GetSubgroups()
		{
			if (this.subgroups == null)
			{
				this.subgroups = new Collections.ObservableList<CollectionViewGroup> ();
				this.subgroups.CollectionChanged += this.HandleSubgroupsCollectionChanged;
			}

			return this.subgroups;
		}

		#region INotifyPropertyChanged Members

		public event Support.EventHandler<DependencyPropertyChangedEventArgs> PropertyChanged;

		#endregion


		private string							name;
		private int								itemCount;
		private CollectionViewGroup				parentGroup;
		
		private Collections.ObservableList<object> items;
		private Collections.ObservableList<CollectionViewGroup> subgroups;
	}
}
