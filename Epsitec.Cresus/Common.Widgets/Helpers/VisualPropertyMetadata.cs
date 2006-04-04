//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		
		public VisualPropertyMetadata(Types.GetValueOverrideCallback get_value_override_callback) : base (get_value_override_callback)
		{
		}
		public VisualPropertyMetadata(Types.GetValueOverrideCallback get_value_override_callback, VisualPropertyMetadataOptions flags) : base (get_value_override_callback)
		{
			this.InitialiseFromFlags (flags);
		}
		public VisualPropertyMetadata(Types.GetValueOverrideCallback get_value_override_callback, Types.PropertyInvalidatedCallback property_invalidated_callback, VisualPropertyMetadataOptions flags) : base (get_value_override_callback, property_invalidated_callback)
		{
			this.InitialiseFromFlags (flags);
		}

		public VisualPropertyMetadata(VisualPropertyMetadataOptions flags)
		{
			this.InitialiseFromFlags (flags);
		}

		public VisualPropertyMetadata(object default_value, VisualPropertyMetadataOptions flags) : base (default_value)
		{
			this.InitialiseFromFlags (flags);
		}
		public VisualPropertyMetadata(object default_value, Types.GetValueOverrideCallback get_value_override_callback, VisualPropertyMetadataOptions flags) : base (default_value, get_value_override_callback)
		{
			this.InitialiseFromFlags (flags);
		}
		public VisualPropertyMetadata(object default_value, Types.PropertyInvalidatedCallback property_invalidated_callback, VisualPropertyMetadataOptions flags) : base (default_value, property_invalidated_callback)
		{
			this.InitialiseFromFlags (flags);
		}
		public VisualPropertyMetadata(object default_value, Types.GetValueOverrideCallback get_value_override_callback, Types.PropertyInvalidatedCallback property_invalidated_callback, VisualPropertyMetadataOptions flags) : base (default_value, get_value_override_callback, property_invalidated_callback)
		{
			this.InitialiseFromFlags (flags);
		}
		public VisualPropertyMetadata(object default_value, Types.SetValueOverrideCallback set_value_override_callback, VisualPropertyMetadataOptions flags) : base (default_value, set_value_override_callback)
		{
			this.InitialiseFromFlags (flags);
		}
		public VisualPropertyMetadata(object default_value, Types.GetValueOverrideCallback get_value_override_callback, Types.SetValueOverrideCallback set_value_override_callback, VisualPropertyMetadataOptions flags) : base (default_value, get_value_override_callback, set_value_override_callback)
		{
			this.InitialiseFromFlags (flags);
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
		
		public override bool					InheritsValue
		{
			get
			{
				return this.inheritsValue;
			}
		}
		public override bool					PropertyNotifiesChanges
		{
			get
			{
				return this.notifiesChanges;
			}
		}
		
		protected virtual void InitialiseFromFlags(VisualPropertyMetadataOptions flags)
		{
			this.affectsArrange        = (flags & VisualPropertyMetadataOptions.AffectsArrange) != 0;
			this.affectsMeasure	       = (flags & VisualPropertyMetadataOptions.AffectsMeasure) != 0;
			this.affectsDisplay        = (flags & VisualPropertyMetadataOptions.AffectsDisplay) != 0;
			this.affectsChildrenLayout = (flags & VisualPropertyMetadataOptions.AffectsChildrenLayout) != 0;
			
			this.inheritsValue         = (flags & VisualPropertyMetadataOptions.InheritsValue) != 0;
			this.notifiesChanges       = (flags & VisualPropertyMetadataOptions.ChangesSilently) == 0;
		}
		
		protected override void OnPropertyInvalidated(Types.DependencyObject sender, object old_value, object new_value)
		{
			Visual visual = sender as Visual;
			
			System.Diagnostics.Debug.Assert (visual != null);

			if (this.affectsChildrenLayout)
			{
				visual.NotifyLayoutChanged ();
			}

			if ((this.affectsArrange) ||
				(this.affectsMeasure))
			{
				visual.NotifyParentLayoutChanged ();
			}

			if (this.affectsDisplay)
			{
				visual.NotifyDisplayChanged ();
			}

			//	Layout support :
			
			if (this.affectsMeasure)
			{
				Layouts.LayoutContext.AddToMeasureQueue (visual);
			}
			
			base.OnPropertyInvalidated (sender, old_value, new_value);
		}


		private bool							affectsChildrenLayout;
		private bool							affectsArrange;
		private bool							affectsMeasure;
		private bool							affectsDisplay;
		private bool							inheritsValue;
		private bool							notifiesChanges;
	}
}
