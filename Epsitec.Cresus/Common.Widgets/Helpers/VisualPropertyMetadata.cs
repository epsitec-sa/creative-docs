//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		public VisualPropertyMetadata(object default_value) : base (default_value)
		{
		}
		public VisualPropertyMetadata(object default_value, Types.PropertyInvalidatedCallback property_invalidated_callback) : base (default_value, property_invalidated_callback)
		{
		}
		public VisualPropertyMetadata(object default_value, Types.PropertyInvalidatedCallback property_invalidated_callback, Types.CoerceValueCallback coerce_value_callback) : base (default_value, property_invalidated_callback)
		{
			this.CoerceValue = coerce_value_callback;
		}
		
		public VisualPropertyMetadata(Types.GetValueOverrideCallback get_value_override_callback) : base (get_value_override_callback)
		{
		}
		public VisualPropertyMetadata(Types.GetValueOverrideCallback get_value_override_callback, VisualPropertyMetadataOptions flags) : base (get_value_override_callback)
		{
			this.InitializeFromFlags (flags);
		}
		public VisualPropertyMetadata(Types.GetValueOverrideCallback get_value_override_callback, Types.PropertyInvalidatedCallback property_invalidated_callback, VisualPropertyMetadataOptions flags) : base (get_value_override_callback, property_invalidated_callback)
		{
			this.InitializeFromFlags (flags);
		}

		public VisualPropertyMetadata(VisualPropertyMetadataOptions flags)
		{
			this.InitializeFromFlags (flags);
		}

		public VisualPropertyMetadata(object default_value, VisualPropertyMetadataOptions flags) : base (default_value)
		{
			this.InitializeFromFlags (flags);
		}
		public VisualPropertyMetadata(object default_value, Types.GetValueOverrideCallback get_value_override_callback, VisualPropertyMetadataOptions flags) : base (default_value, get_value_override_callback)
		{
			this.InitializeFromFlags (flags);
		}
		public VisualPropertyMetadata(object default_value, Types.PropertyInvalidatedCallback property_invalidated_callback, VisualPropertyMetadataOptions flags) : base (default_value, property_invalidated_callback)
		{
			this.InitializeFromFlags (flags);
		}
		public VisualPropertyMetadata(object default_value, Types.PropertyInvalidatedCallback property_invalidated_callback, Types.CoerceValueCallback coerce_value_callback, VisualPropertyMetadataOptions flags) : base (default_value, property_invalidated_callback)
		{
			this.InitializeFromFlags (flags);
			this.CoerceValue = coerce_value_callback;
		}
		public VisualPropertyMetadata(object default_value, Types.GetValueOverrideCallback get_value_override_callback, Types.PropertyInvalidatedCallback property_invalidated_callback, VisualPropertyMetadataOptions flags) : base (default_value, get_value_override_callback, property_invalidated_callback)
		{
			this.InitializeFromFlags (flags);
		}
		public VisualPropertyMetadata(object default_value, Types.SetValueOverrideCallback set_value_override_callback, VisualPropertyMetadataOptions flags) : base (default_value, set_value_override_callback)
		{
			this.InitializeFromFlags (flags);
		}
		public VisualPropertyMetadata(object default_value, Types.GetValueOverrideCallback get_value_override_callback, Types.SetValueOverrideCallback set_value_override_callback, VisualPropertyMetadataOptions flags) : base (default_value, get_value_override_callback, set_value_override_callback)
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
		
		protected override void OnPropertyInvalidated(Types.DependencyObject sender, object old_value, object new_value)
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
			
			base.OnPropertyInvalidated (sender, old_value, new_value);
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
