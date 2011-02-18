//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.UI.Collections;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (TablePlaceholder))]

namespace Epsitec.Common.UI
{
	public class TablePlaceholder : AbstractPlaceholder, Types.Serialization.IDeserialization
	{
		public TablePlaceholder()
		{
			this.table = new ItemTable (this);
			this.table.Dock = Widgets.DockStyle.Fill;
		}

		public TablePlaceholder(Widgets.Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		public ItemTableColumnCollection		Columns
		{
			get
			{
				return this.table.Columns;
			}
		}

		public ICollectionView					CollectionView
		{
			get
			{
				return this.collectionView;
			}
		}

		public System.Collections.IList			Source
		{
			get
			{
				return this.Value as System.Collections.IList;
			}
		}

		public Support.Druid					SourceTypeId
		{
			get
			{
				return this.sourceTypeId;
			}
			set
			{
				if (this.sourceTypeId != value)
				{
					object oldValue = this.SourceTypeId;
					object newValue = value;
					
					this.sourceTypeId = value;

					this.UpdateSourceType ();
					this.InvalidateProperty (TablePlaceholder.SourceTypeProperty, oldValue, newValue);
				}
			}
		}

		public override IController				ControllerInstance
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}

		public override DependencyProperty GetValueProperty()
		{
			return TablePlaceholder.ItemsProperty;
		}

		protected override void UpdateValue(object oldValue, object newValue)
		{
			base.UpdateValue (oldValue, newValue);

			this.DisposeCollectionView ();
			this.CreateCollectionView ();
		}

		protected override void UpdateValueType(object oldValueType, object newValueType)
		{
			base.UpdateValueType (oldValueType, newValueType);
		}

		private void DisposeCollectionView()
		{
			if (this.collectionView != null)
			{
				System.IDisposable disposable = this.collectionView as System.IDisposable;
				this.collectionView = null;
				
				if (this.isAutomaticCollectionView)
				{
					//	Nothing to do...
				}
				else if (disposable != null)
				{
					disposable.Dispose ();
				}
			}
		}

		private void CreateCollectionView()
		{
			System.Diagnostics.Debug.Assert (this.collectionView == null);

			System.Collections.IList source = this.Source;
			ICollectionView          view   = this.Value as ICollectionView;

			if (source != null)
			{
				view = Binding.FindCollectionView (source, DataObject.GetDataContext (this));
			}

			if (view != null)
			{
				this.isAutomaticCollectionView = true;
				this.collectionView            = view;
				this.table.Items = this.collectionView;
			}
			else if (source != null)
			{
				this.isAutomaticCollectionView = false;
				this.collectionView            = new CollectionView (source);
				this.table.Items = this.collectionView;
			}
		}

		private void UpdateSourceType()
		{
			if ((this.suspendUpdates == 0) &&
				(this.sourceTypeId.IsValid))
			{
				Support.ResourceManager manager = Widgets.Helpers.VisualTree.GetResourceManager (this);
				Caption caption = manager.GetCaption (this.sourceTypeId);
				AbstractType type = TypeRosetta.GetTypeObject (caption);
				this.table.SourceType = type as StructuredType;
			}
		}

		#region IDeserialization Members

		bool Epsitec.Common.Types.Serialization.IDeserialization.NotifyDeserializationStarted(Epsitec.Common.Types.Serialization.Context context)
		{
			this.suspendUpdates++;
			return true;
		}

		void Epsitec.Common.Types.Serialization.IDeserialization.NotifyDeserializationCompleted(Epsitec.Common.Types.Serialization.Context context)
		{
			this.suspendUpdates--;
			this.UpdateSourceType ();
		}

		#endregion

		private static object GetColumnsValue(DependencyObject o)
		{
			TablePlaceholder placeholder = (TablePlaceholder) o;
			return placeholder.Columns;
		}

		private static object GetSourceTypeValue(DependencyObject o)
		{
			TablePlaceholder placeholder = (TablePlaceholder) o;
			return placeholder.SourceTypeId;
		}

		private static void SetSourceTypeValue(DependencyObject o, object value)
		{
			TablePlaceholder placeholder = (TablePlaceholder) o;
			placeholder.SourceTypeId = (Support.Druid) value;
		}

		private static void NotifyItemsChanged(DependencyObject o, object oldValue, object newValue)
		{
			TablePlaceholder placeholder = (TablePlaceholder) o;
			placeholder.Value = newValue;
		}

		public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register ("Items", typeof (ICollectionView), typeof (TablePlaceholder), new DependencyPropertyMetadata (TablePlaceholder.NotifyItemsChanged).MakeNotSerializable ());
		public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterReadOnly ("Columns", typeof (ItemTableColumnCollection), typeof (TablePlaceholder), new DependencyPropertyMetadata (TablePlaceholder.GetColumnsValue).MakeReadOnlySerializable ());
		public static readonly DependencyProperty SourceTypeProperty = DependencyProperty.Register ("SourceType", typeof (Support.Druid), typeof (TablePlaceholder), new DependencyPropertyMetadata (TablePlaceholder.GetSourceTypeValue, TablePlaceholder.SetSourceTypeValue));

		private ItemTable table;
		private ICollectionView collectionView;
		private bool isAutomaticCollectionView;
		private Support.Druid sourceTypeId;
		private int suspendUpdates;
	}
}
