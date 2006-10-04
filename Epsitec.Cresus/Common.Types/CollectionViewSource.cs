//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>CollectionViewSource</c> class represents a view of a data
	/// collection.
	/// </summary>
	public class CollectionViewSource : DependencyObject
	{
		public CollectionViewSource()
		{
			this.sortDescriptions = new Collections.HostedList<SortDescription> (this.HandleSortInsertion, this.HandleSortRemoval);
			this.groupDescriptions = new Collections.HostedList<AbstractGroupDescription> (this.HandleGroupInsertion, this.HandleGroupRemoval);
		}

		public object Source
		{
			get
			{
				return this.source;
			}
			set
			{
				if (this.source != value)
				{
					object oldValue = this.source;
					object newValue = value;

					this.source = newValue;

					this.InvalidateProperty (CollectionViewSource.SourceProperty, oldValue, newValue);
				}
			}
		}

		public System.Type CollectionViewType
		{
			get
			{
				return this.collectionViewType;
			}
			set
			{
				this.collectionViewType = value;
			}
		}

		public ICollectionView View
		{
			get
			{
				if (this.view == null)
				{
					//	TODO: create view instance
				}
				
				return this.view;
			}
		}

		public Collections.ObservableList<SortDescription> SortDescriptions
		{
			get
			{
				return this.sortDescriptions;
			}
		}

		public Collections.ObservableList<AbstractGroupDescription> GroupDescriptions
		{
			get
			{
				return this.groupDescriptions;
			}
		}

		private void HandleSortInsertion(SortDescription item)
		{
		}

		private void HandleSortRemoval(SortDescription item)
		{
		}

		private void HandleGroupInsertion(AbstractGroupDescription item)
		{
		}

		private void HandleGroupRemoval(AbstractGroupDescription item)
		{
		}

		private static object GetSourceValue(DependencyObject obj)
		{
			CollectionViewSource that = (CollectionViewSource) obj;
			return that.Source;
		}

		private static void SetSourceValue(DependencyObject obj, object value)
		{
			CollectionViewSource that = (CollectionViewSource) obj;
			that.Source = value;
		}

		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register ("Source", typeof (object), typeof (CollectionViewSource), new DependencyPropertyMetadata (CollectionViewSource.GetSourceValue, CollectionViewSource.SetSourceValue));

		private Collections.HostedList<SortDescription> sortDescriptions;
		private Collections.HostedList<AbstractGroupDescription> groupDescriptions;
		private object source;
		private System.Type collectionViewType;
		private ICollectionView view;
	}
}
