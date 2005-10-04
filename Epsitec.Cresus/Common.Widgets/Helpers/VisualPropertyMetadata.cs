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
		
		public VisualPropertyMetadata(object default_value, Types.GetValueOverrideCallback get_value_override_callback) : base (default_value, get_value_override_callback)
		{
		}
		
		public VisualPropertyMetadata(VisualPropertyFlags flags)
		{
			this.affects_layout        = (flags & VisualPropertyFlags.AffectsLayout) != 0;
			this.affects_parent_layout = (flags & VisualPropertyFlags.AffectsParentLayout) != 0;
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
			
			base.OnPropertyInvalidated (sender, old_value, new_value);
		}
		
		
		private bool							affects_layout;
		private bool							affects_parent_layout;
	}
}
