//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Binders
{
	/// <summary>
	/// La classe AbstractBinder sert de base à la majeure partie des "binders",
	/// qui font le lien entre une source de données et des consommateurs.
	/// </summary>
	public abstract class AbstractBinder : IBinder
	{
		protected AbstractBinder()
		{
		}
		
		
		public Adapters.IAdapter				Adapter
		{
			get
			{
				return this.adapter;
			}
			set
			{
				if (this.adapter != value)
				{
					this.adapter = value;
					this.OnAdapterChanged ();
				}
			}
		}
		
		public abstract string					Caption
		{
			get;
		}
		
		public abstract bool					IsValid
		{
			get;
		}
		
		
		public virtual System.Type GetDataType()
		{
			return null;
		}
		
		
		public virtual bool ReadData(out object data)
		{
			data = null;
			return false;
		}

		public virtual bool WriteData(object data)
		{
			return false;
		}

		public virtual bool EqualsData(object data)
		{
			object x;
			
			if (this.ReadData (out x))
			{
				if (x == data)
				{
					return true;
				}
				
				if (x == null)
				{
					return false;
				}
				
				return x.Equals (data);
			}
			
			return false;
		}
		
		
		protected virtual void OnAdapterChanged()
		{
			this.SyncToAdapter (SyncReason.AdapterChanged);
		}
		
		
		protected virtual void SyncToAdapter(SyncReason reason)
		{
			if (this.adapter != null)
			{
				this.adapter.SyncFromBinder (reason);
			}
		}
		
		
		
		private Adapters.IAdapter				adapter;
	}
}
