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
		
		
		public bool								AffectsLayout
		{
			get
			{
				return this.affects_layout;
			}
			set
			{
				this.affects_layout = value;
			}
		}
		public bool								AffectsParentLayout
		{
			get
			{
				return this.affects_parent_layout;
			}
			set
			{
				this.affects_parent_layout = value;
			}
		}
		public bool								AffectsDisplay
		{
			get
			{
				return this.affects_display;
			}
			set
			{
				this.affects_display = value;
			}
		}
		
		public override bool					InheritsValue
		{
			get
			{
				return this.inherits_value;
			}
		}
		
		protected virtual void InitialiseFromFlags(VisualPropertyMetadataOptions flags)
		{
			this.affects_layout        = (flags & VisualPropertyMetadataOptions.AffectsLayout) != 0;
			this.affects_parent_layout = (flags & VisualPropertyMetadataOptions.AffectsParentLayout) != 0;
			this.affects_display       = (flags & VisualPropertyMetadataOptions.AffectsDisplay) != 0;
			this.inherits_value        = (flags & VisualPropertyMetadataOptions.InheritsValue) != 0;
		}
		
		protected override void OnPropertyInvalidated(Types.DependencyObject sender, object old_value, object new_value)
		{
			Visual visual = sender as Visual;
			
			System.Diagnostics.Debug.Assert (visual != null);
			
			if (this.affects_layout)
			{
				visual.NotifyLayoutChanged ();
			}
			
			if (this.affects_parent_layout)
			{
				visual.NotifyParentLayoutChanged ();
			}
			
			if (this.affects_display)
			{
				visual.NotifyDisplayChanged ();
			}
			
			base.OnPropertyInvalidated (sender, old_value, new_value);
		}
		
		
		private bool							affects_layout;
		private bool							affects_parent_layout;
		private bool							affects_display;
		private bool							inherits_value;
	}
}
