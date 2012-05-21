//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (CollectionPlaceholder))]

namespace Epsitec.Common.UI
{
	public class CollectionPlaceholder : Placeholder
	{
		public CollectionPlaceholder()
		{
		}

		public StructuredType EntityType
		{
			get;
			set;
		}

		public Druid EntityId
		{
			get
			{
				return this.EntityType == null ? Druid.Empty : this.EntityType.CaptionId;
			}
		}

		public EntityFieldPath EntityFieldPath
		{
			get;
			set;
		}

		public System.Collections.IList Source
		{
			get
			{
				return this.Value as System.Collections.IList;
			}
		}

		public override IController ControllerInstance
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}
#if false
		public override object Value
		{
			get
			{
				return this.GetValue (CollectionPlaceholder.CollectionProperty);
			}
			set
			{
				if (value == UndefinedValue.Value)
				{
					this.ClearValue (CollectionPlaceholder.CollectionProperty);
				}
				else
				{
					this.SetValue (CollectionPlaceholder.CollectionProperty, value);
				}
			}
		}
#endif

		public override DependencyProperty GetValueProperty()
		{
			return CollectionPlaceholder.CollectionProperty;
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
//				this.table.Items = this.collectionView;
			}
			else if (source != null)
			{
				this.isAutomaticCollectionView = false;
				this.collectionView            = new CollectionView (source);
//				this.table.Items = this.collectionView;
			}
		}

		protected override void GetAssociatedController(out string newControllerName, out string newControllerParameters)
		{
			newControllerName = "Collection";
			newControllerParameters = Controllers.ControllerParameters.MergeParameters (string.Concat ("EntityId=", this.EntityId.ToString ()), this.ControllerParameters);
		}
		
		private static void NotifyCollectionChanged(DependencyObject o, object oldValue, object newValue)
		{
			CollectionPlaceholder that = (CollectionPlaceholder) o;
			that.Value = newValue;
		}
		
		public static readonly DependencyProperty CollectionProperty = DependencyProperty.Register ("Collection", typeof (ICollectionView), typeof (CollectionPlaceholder), new DependencyPropertyMetadata (null, CollectionPlaceholder.NotifyCollectionChanged).MakeNotSerializable ());
		
		private ICollectionView collectionView;
		private bool isAutomaticCollectionView;
	}
}
