//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		
		public SyncReason						SyncReason
		{
			get
			{
				return this.sync_reason;
			}
		}
		
		public bool								Validity
		{
			get
			{
				return this.validity;
			}
			set
			{
				if (this.validity != value)
				{
					this.validity = value;
					this.OnValidityChanged ();
				}
			}
		}
		
		public void SyncFromBinder(SyncReason reason)
		{
			this.sync_reason = reason;
			
			this.ReadFromBinder ();
			
			this.sync_reason = SyncReason.None;
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
			this.OnChanged ();
		}
		
		protected virtual void OnValidityChanged()
		{
			if (this.validity == false)
			{
				//	TODO: fix
//				this.binder.NotifyInvalidData ();
			}
			else
			{
				this.WriteToBinder ();
			}
			
			this.OnChanged ();
		}
		
		protected virtual void OnChanged()
		{
			SyncReason old_reason = this.sync_reason;
			SyncReason new_reason = this.sync_reason == SyncReason.None ? SyncReason.ValueChanged : old_reason;
			
			this.sync_reason = new_reason;
			
			if (this.Changed != null)
			{
				this.Changed (this);
			}
			
			this.sync_reason = old_reason;
		}
		
		
		protected virtual bool ReadFromBinder()
		{
			if ((this.binder != null) &&
				(this.binder.IsValid) &&
				(this.job_in_progress == false))
			{
				object data;
				
				try
				{
					this.job_in_progress = true;
					
					if (this.binder.ReadData (out data))
					{
						return this.ConvertFromObject (data);
					}
				}
				finally
				{
					this.job_in_progress = false;
				}
			}
			
			return false;
		}
		
		protected virtual bool WriteToBinder()
		{
			if ((this.binder != null) &&
				(this.binder.IsValid) &&
				(this.job_in_progress == false))
			{
				object data = this.ConvertToObject ();
				
				try
				{
					this.job_in_progress = true;
					
					if (this.binder.WriteData (data))
					{
						return true;
					}
				}
				finally
				{
					this.job_in_progress = false;
				}
			}
			
			return false;
		}
		
		
		protected abstract object ConvertToObject();
		protected abstract bool   ConvertFromObject(object data);
		
		
		public event EventHandler				Changed;
		
		protected Binders.IBinder				binder;
		protected SyncReason					sync_reason;
		protected bool							job_in_progress;
		protected bool							validity = true;
	}
}
