//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// VisualPropertyMetadata.
	/// </summary>
	public class VisualPropertyMetadata : Types.DependencyPropertyMetadata
	{
		public VisualPropertyMetadata()
		{
		}
		
		public VisualPropertyMetadata(object defaultValue) : base (defaultValue)
		{
		}
		public VisualPropertyMetadata(object defaultValue, Types.PropertyInvalidatedCallback propertyInvalidatedCallback) : base (defaultValue, propertyInvalidatedCallback)
		{
		}
		public VisualPropertyMetadata(object defaultValue, Types.PropertyInvalidatedCallback propertyInvalidatedCallback, Types.CoerceValueCallback coerceValueCallback) : base (defaultValue, propertyInvalidatedCallback)
		{
			this.CoerceValue = coerceValueCallback;
		}
		
		public VisualPropertyMetadata(Types.GetValueOverrideCallback getValueOverrideCallback) : base (getValueOverrideCallback)
		{
		}
		public VisualPropertyMetadata(Types.GetValueOverrideCallback getValueOverrideCallback, VisualPropertyMetadataOptions flags) : base (getValueOverrideCallback)
		{
			this.InitializeFromFlags (flags);
		}
		public VisualPropertyMetadata(Types.GetValueOverrideCallback getValueOverrideCallback, Types.PropertyInvalidatedCallback propertyInvalidatedCallback, VisualPropertyMetadataOptions flags) : base (getValueOverrideCallback, propertyInvalidatedCallback)
		{
			this.InitializeFromFlags (flags);
		}

		public VisualPropertyMetadata(VisualPropertyMetadataOptions flags)
		{
			this.InitializeFromFlags (flags);
		}

		public VisualPropertyMetadata(object defaultValue, VisualPropertyMetadataOptions flags) : base (defaultValue)
		{
			this.InitializeFromFlags (flags);
		}
		public VisualPropertyMetadata(object defaultValue, Types.GetValueOverrideCallback getValueOverrideCallback, VisualPropertyMetadataOptions flags) : base (defaultValue, getValueOverrideCallback)
		{
			this.InitializeFromFlags (flags);
		}
		public VisualPropertyMetadata(object defaultValue, Types.PropertyInvalidatedCallback propertyInvalidatedCallback, VisualPropertyMetadataOptions flags) : base (defaultValue, propertyInvalidatedCallback)
		{
			this.InitializeFromFlags (flags);
		}
		public VisualPropertyMetadata(object defaultValue, Types.PropertyInvalidatedCallback propertyInvalidatedCallback, Types.CoerceValueCallback coerceValueCallback, VisualPropertyMetadataOptions flags) : base (defaultValue, propertyInvalidatedCallback)
		{
			this.InitializeFromFlags (flags);
			this.CoerceValue = coerceValueCallback;
		}
		public VisualPropertyMetadata(object defaultValue, Types.GetValueOverrideCallback getValueOverrideCallback, Types.PropertyInvalidatedCallback propertyInvalidatedCallback, VisualPropertyMetadataOptions flags) : base (defaultValue, getValueOverrideCallback, propertyInvalidatedCallback)
		{
			this.InitializeFromFlags (flags);
		}
		public VisualPropertyMetadata(object defaultValue, Types.SetValueOverrideCallback setValueOverrideCallback, VisualPropertyMetadataOptions flags) : base (defaultValue, setValueOverrideCallback)
		{
			this.InitializeFromFlags (flags);
		}
		public VisualPropertyMetadata(object defaultValue, Types.GetValueOverrideCallback getValueOverrideCallback, Types.SetValueOverrideCallback setValueOverrideCallback, VisualPropertyMetadataOptions flags) : base (defaultValue, getValueOverrideCallback, setValueOverrideCallback)
		{
			this.InitializeFromFlags (flags);
		}
		
		
		public bool								AffectsMeasure
		{
			get
			{
				return this.affectsMeasure;
			}
			set
			{
				this.affectsMeasure = value;
			}
		}
		public bool								AffectsArrange
		{
			get
			{
				return this.affectsArrange;
			}
			set
			{
				this.affectsArrange = value;
			}
		}
		public bool								AffectsDisplay
		{
			get
			{
				return this.affectsDisplay;
			}
			set
			{
				this.affectsDisplay = value;
			}
		}
		public bool								AffectsChildrenLayout
		{
			get
			{
				return this.affectsChildrenLayout;
			}
			set
			{
				this.affectsChildrenLayout = value;
			}
		}
		public bool								AffectsTextLayout
		{
			get
			{
				return this.affectsTextLayout;
			}
			set
			{
				this.affectsTextLayout = value;
			}
		}

		
		public override bool					InheritsValue
		{
			get
			{
				return this.inheritsValue;
			}
		}
		public override bool					HasSerializationFilter
		{
			get
			{
				return true;
			}
		}
		public override bool					PropertyNotifiesChanges
		{
			get
			{
				return this.notifiesChanges;
			}
		}

		public override bool FilterSerializableItem(object item)
		{
			//	Return true for items which may be serialized.
			
			Widget widget = item as Widget;
			
			if ((widget != null) &&
				(widget.IsEmbedded))
			{
				return false;
			}
			else
			{
				return base.FilterSerializableItem (item);
			}
		}
		
		protected virtual void InitializeFromFlags(VisualPropertyMetadataOptions flags)
		{
			this.affectsArrange        = (flags & VisualPropertyMetadataOptions.AffectsArrange) != 0;
			this.affectsMeasure	       = (flags & VisualPropertyMetadataOptions.AffectsMeasure) != 0;
			this.affectsDisplay        = (flags & VisualPropertyMetadataOptions.AffectsDisplay) != 0;
			this.affectsChildrenLayout = (flags & VisualPropertyMetadataOptions.AffectsChildrenLayout) != 0;
			this.affectsTextLayout     = (flags & VisualPropertyMetadataOptions.AffectsTextLayout) != 0;
			
			this.inheritsValue         = (flags & VisualPropertyMetadataOptions.InheritsValue) != 0;
			this.notifiesChanges       = (flags & VisualPropertyMetadataOptions.ChangesSilently) == 0;
		}
		
		protected override void OnPropertyInvalidated(Types.DependencyObject sender, object oldValue, object newValue)
		{
			Visual visual = sender as Visual;
			
			System.Diagnostics.Debug.Assert (visual != null);

			if (this.affectsDisplay)
			{
				visual.Invalidate ();
			}

			//	Layout support :
			
			if (this.affectsMeasure)
			{
				Layouts.LayoutContext.AddToMeasureQueue (visual);
			}
			if (this.affectsArrange)
			{
				Visual parent = visual.Parent;

				if (parent != null)
				{
					Layouts.LayoutContext.AddToMeasureQueue (parent);
					Layouts.LayoutContext.AddToArrangeQueue (parent);
				}
			}
			if (this.affectsChildrenLayout)
			{
				Layouts.LayoutContext.AddToArrangeQueue (visual);
			}
			if (this.affectsTextLayout)
			{
				visual.InvalidateTextLayout ();
			}
			
			base.OnPropertyInvalidated (sender, oldValue, newValue);
		}

		protected override Types.DependencyPropertyMetadata CloneNewObject()
		{
			return new VisualPropertyMetadata ();
		}

		protected override Types.DependencyPropertyMetadata CloneCopyToNewObject(Types.DependencyPropertyMetadata copy)
		{
			base.CloneCopyToNewObject (copy);

			VisualPropertyMetadata that = (VisualPropertyMetadata) copy;

			that.affectsArrange        = this.affectsArrange;
			that.affectsChildrenLayout = this.affectsChildrenLayout;
			that.affectsDisplay        = this.affectsDisplay;
			that.affectsMeasure        = this.affectsMeasure;
			that.affectsTextLayout     = this.affectsTextLayout;
			that.inheritsValue         = this.inheritsValue;
			that.notifiesChanges       = this.notifiesChanges;

			return that;
		}

		private bool							affectsChildrenLayout;
		private bool							affectsArrange;
		private bool							affectsMeasure;
		private bool							affectsDisplay;
		private bool							affectsTextLayout;
		private bool							inheritsValue;
		private bool							notifiesChanges;
	}
}
