//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Adapters
{
	/// <summary>
	/// Summary description for AbstractAdapter.
	/// </summary>
	public abstract class AbstractAdapter : IAdapter
	{
		protected AbstractAdapter()
		{
		}
		
		
		public Binders.IBinder					Binder
		{
			get
			{
				return this.binder;
			}
			set
			{
				if (this.binder != value)
				{
					this.binder = value;
					this.OnBinderChanged ();
				}
			}
		}
		
		
		public void SyncFromBinder()
		{
			this.ReadFromBinder ();
		}
		
		protected virtual void OnBinderChanged()
		{
			if (this.binder != null)
			{
				this.ReadFromBinder ();
			}
		}
		
		protected virtual void OnValueChanged()
		{
			this.WriteToBinder ();
			
			if (this.ValueChanged != null)
			{
				this.ValueChanged (this);
			}
		}
		
		
		protected virtual bool ReadFromBinder()
		{
			if ((this.binder != null) &&
				(this.binder.IsValid))
			{
				object data;
				
				if (this.binder.ReadData (out data))
				{
					return this.ConvertFromObject (data);
				}
			}
			
			return false;
		}
		
		protected virtual bool WriteToBinder()
		{
			if ((this.binder != null) &&
				(this.binder.IsValid))
			{
				object data = this.ConvertToObject ();
				
				if (this.binder.WriteData (data))
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		protected abstract object ConvertToObject();
		protected abstract bool   ConvertFromObject(object data);
		
		
		public event EventHandler				ValueChanged;
		
		protected Binders.IBinder				binder;
	}
}
