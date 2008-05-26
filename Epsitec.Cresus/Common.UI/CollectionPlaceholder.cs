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

		protected override void GetAssociatedController(out string newControllerName, out string newControllerParameters)
		{
			newControllerName = "Collection";
			newControllerParameters = Controllers.ControllerParameters.MergeParameters (string.Concat ("EntityId=", this.EntityId.ToString ()), this.ControllerParameters);
		}
		
		private static void NotifyCollectionChanged(DependencyObject o, object oldValue, object newValue)
		{
			CollectionPlaceholder that = (CollectionPlaceholder) o;
			that.HandleValueChanged (oldValue, newValue);
		}
		
		public static readonly DependencyProperty CollectionProperty = DependencyProperty.Register ("Collection", typeof (ICollectionView), typeof (CollectionPlaceholder), new DependencyPropertyMetadata (null, CollectionPlaceholder.NotifyCollectionChanged).MakeNotSerializable ());
	}
}
