//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// Summary description for AbstractController.
	/// </summary>
	public abstract class AbstractController : IController
	{
		protected AbstractController()
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
					if (this.adapter != null)
					{
						this.Detach (this.adapter);
					}
					
					this.adapter = value;
					
					if (this.adapter != null)
					{
						this.Attach (this.adapter);
					}
					
					this.OnAdapterChanged ();
				}
			}
		}
		
		public string							Caption
		{
			get
			{
				return this.caption;
			}
			set
			{
				if (this.caption != value)
				{
					this.caption = value;
					this.OnCaptionChanged ();
				}
			}
		}
		
		
		public abstract void CreateUI(Widget panel);
		public abstract void SyncFromUI();
		public abstract void SyncFromAdapter(SyncReason reason);
		
		protected void Attach(Adapters.IAdapter adapter)
		{
			adapter.Changed += new EventHandler (this.HandleAdapterValueChanged);
		}
		
		protected void Detach(Adapters.IAdapter adapter)
		{
			adapter.Changed -= new EventHandler (this.HandleAdapterValueChanged);
		}
		
		
		protected virtual void OnAdapterChanged()
		{
			if (this.adapter != null)
			{
				this.OnAdapterDataChanged ();
			}
		}
		
		protected virtual void OnCaptionChanged()
		{
			if (this.caption_label != null)
			{
				this.caption_label.Text = this.caption;
			}
		}
		
		protected virtual void OnAdapterDataChanged()
		{
			this.SyncFromAdapter (SyncReason.AdapterChanged);
		}
		
		protected virtual void OnUIDataChanged()
		{
			if (this.counter > 0)
			{
				return;
			}
			
			try
			{
				this.counter++;
				this.SyncFromUI ();
			}
			finally
			{
				this.counter--;
			}
		}
		
		
		private void HandleAdapterValueChanged(object sender)
		{
			if (this.counter > 0)
			{
				return;
			}
			
			Adapters.IAdapter adapter = sender as Adapters.IAdapter;
			
			try
			{
				this.counter++;
				this.SyncFromAdapter (adapter.SyncReason);
			}
			finally
			{
				this.counter--;
			}
		}
		
		
		private Adapters.IAdapter				adapter;
		protected string						caption;
		protected StaticText					caption_label;
		protected int							counter;
	}
}
