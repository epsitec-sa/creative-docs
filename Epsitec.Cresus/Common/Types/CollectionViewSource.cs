//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
#if false
	using SortDescriptionList=Collections.ObservableList<SortDescription>;
	using GroupDescriptionList=Collections.ObservableList<GroupDescription>;

	/// <summary>
	/// The <c>CollectionViewSource</c> class represents a view of a data
	/// collection.
	/// </summary>
	public class CollectionViewSource : DependencyObject
	{
		public CollectionViewSource()
		{
			this.sortDescriptions = new Collections.HostedList<SortDescription> (this.HandleSortInsertion, this.HandleSortRemoval);
			this.groupDescriptions = new Collections.HostedList<GroupDescription> (this.HandleGroupInsertion, this.HandleGroupRemoval);
		}

		public object							Source
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

		public System.Type						CollectionViewType
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

		public ICollectionView					View
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

		public SortDescriptionList				SortDescriptions
		{
			get
			{
				return this.sortDescriptions;
			}
		}

		public GroupDescriptionList				GroupDescriptions
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

		private void HandleGroupInsertion(GroupDescription item)
		{
		}

		private void HandleGroupRemoval(GroupDescription item)
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

		private SortDescriptionList				sortDescriptions;
		private GroupDescriptionList			groupDescriptions;
		private object							source;
		private System.Type						collectionViewType;
		private ICollectionView					view;
	}
#endif
}
