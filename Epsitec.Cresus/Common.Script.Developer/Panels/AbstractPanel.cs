//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Script.Developer.Panels
{
	/// <summary>
	/// Summary description for AbstractPanel.
	/// </summary>
	public abstract class AbstractPanel : Common.UI.AbstractPanel
	{
		public AbstractPanel()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		
		
		public bool								IsModified
		{
			get
			{
				return this.is_modified;
			}
			set
			{
				if (this.is_modified != value)
				{
					this.is_modified = value;
					this.OnIsModifiedChanged ();
				}
			}
		}
		
		
		protected virtual void OnIsModifiedChanged()
		{
			if (this.IsModifiedChanged != null)
			{
				this.IsModifiedChanged (this);
			}
		}
		
		
		public event Support.EventHandler		IsModifiedChanged;
		
		protected bool							is_modified;
	}
}
