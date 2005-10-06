//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// VisualPropertyMetadata.
	/// </summary>
	public class VisualPropertyMetadata : Types.PropertyMetadata
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
		
		public VisualPropertyMetadata(VisualPropertyFlags flags)
		{
			this.InitialiseFromFlags (flags);
		}
		
		public VisualPropertyMetadata(Types.GetValueOverrideCallback get_value_override_callback, VisualPropertyFlags flags) : base (get_value_override_callback)
		{
			this.InitialiseFromFlags (flags);
		}
		
		public VisualPropertyMetadata(object default_value, VisualPropertyFlags flags) : base (default_value)
		{
			this.InitialiseFromFlags (flags);
		}
		
		public VisualPropertyMetadata(object default_value, Types.GetValueOverrideCallback get_value_override_callback, VisualPropertyFlags flags) : base (default_value, get_value_override_callback)
		{
			this.InitialiseFromFlags (flags);
		}
		
		public VisualPropertyMetadata(object default_value, Types.SetValueOverrideCallback set_value_override_callback, VisualPropertyFlags flags) : base (default_value, set_value_override_callback)
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
		
		public bool								InheritsValue
		{
			get
			{
				return this.inherits_value;
			}
		}
		
		
		protected virtual void InitialiseFromFlags(VisualPropertyFlags flags)
		{
			this.affects_layout        = (flags & VisualPropertyFlags.AffectsLayout) != 0;
			this.affects_parent_layout = (flags & VisualPropertyFlags.AffectsParentLayout) != 0;
			this.affects_display       = (flags & VisualPropertyFlags.AffectsDisplay) != 0;
			this.inherits_value        = (flags & VisualPropertyFlags.InheritsValue) != 0;
		}
		
		protected override void OnPropertyInvalidated(Types.Object sender, object old_value, object new_value)
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
