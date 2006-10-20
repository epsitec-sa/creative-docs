//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>CollectionViewGrop</c> class represents a group created by a
	/// <see cref="CollectionView"/> object, based on its <see cref="AbstractGroupDescription"/>
	/// collection.
	/// </summary>
	public class CollectionViewGroup : INotifyPropertyChanged
	{
		internal CollectionViewGroup(string name)
		{
			this.name = name;
			this.items = new Collections.ObservableList<object> ();
			
			this.items.CollectionChanged += this.HandleItemsCollectionChanged;
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
		/// Gets the number of items in this group.
		/// </summary>
		/// <value>The number of items in this group.</value>
		public int								ItemCount
		{
			get
			{
				return this.items.Count;
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
		/// Gets the items contained in this group. An item will be an instance
		/// of <see cref="CollectionViewGroup"/> if it describes a subgroup.
		/// </summary>
		/// <value>A read-only collection of the items contained in this group.</value>
		public Collections.ReadOnlyObservableList<object> Items
		{
			get
			{
				if (this.readOnlyItems == null)
				{
					this.readOnlyItems = new Collections.ReadOnlyObservableList<object> (this.items);
				}
				
				return this.readOnlyItems;
			}
		}

		public Collections.ReadOnlyObservableList<CollectionViewGroup> Subgroups
		{
			get
			{
				if ((this.readOnlySubgroups == null) &&
					(this.subgroups != null))
				{
					this.readOnlySubgroups = new Collections.ReadOnlyObservableList<CollectionViewGroup> (this.subgroups);
				}

				return this.readOnlySubgroups;
			}
		}

		private void HandleItemsCollectionChanged(object sender, CollectionChangedEventArgs e)
		{
			this.OnPropertyChanged (new DependencyPropertyChangedEventArgs ("Items"));
		}
		
		internal Collections.ObservableList<object> GetItems()
		{
			return this.items;
		}

		internal Collections.ObservableList<CollectionViewGroup> GetSubgroups()
		{
			if (this.subgroups == null)
			{
				this.subgroups = new Collections.ObservableList<CollectionViewGroup> ();
			}
			
			return this.subgroups;
		}

		
		protected virtual void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged (this, e);
			}
		}

		#region INotifyPropertyChanged Members

		public event Support.EventHandler<DependencyPropertyChangedEventArgs> PropertyChanged;

		#endregion


		private string							name;
		private Collections.ObservableList<object> items;
		private Collections.ObservableList<CollectionViewGroup> subgroups;
		private Collections.ReadOnlyObservableList<object> readOnlyItems;
		private Collections.ReadOnlyObservableList<CollectionViewGroup> readOnlySubgroups;
	}
}
