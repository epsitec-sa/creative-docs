//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

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
		public abstract void SyncFromAdapter();
		
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
			this.SyncFromAdapter ();
		}
		
		protected virtual void OnUIDataChanged()
		{
			this.SyncFromUI ();
		}
		
		
		private void HandleAdapterValueChanged(object sender)
		{
			this.SyncFromAdapter ();
		}
		
		
		private Adapters.IAdapter				adapter;
		protected string						caption;
		protected StaticText					caption_label;
	}
}
