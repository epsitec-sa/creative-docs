//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Binders
{
	/// <summary>
	/// La classe DataFolderBinder permet de faire le lien avec une source
	/// compatible avec l'interface IDataFolder.
	/// </summary>
	public class DataFolderBinder : AbstractBinder, System.IDisposable
	{
		public DataFolderBinder()
		{
		}
		
		public DataFolderBinder(Types.IDataFolder source) : this ()
		{
			this.Source = source;
		}
		
		
		public Types.IDataFolder				Source
		{
			get
			{
				return this.source;
			}
			set
			{
				if (this.source != value)
				{
					this.Detach ();
					this.source = value;
					this.Attach ();
					this.OnSourceChanged ();
				}
			}
		}
		
		public override string					Caption
		{
			get
			{
				return this.source.Caption;
			}
		}
		
		public override bool					IsValid
		{
			get
			{
				return this.source != null;
			}
		}
		
		
		public override System.Type GetDataType()
		{
			return this.source.GetType ();
		}
		
		public override bool ReadData(out object data)
		{
			if (this.IsValid)
			{
				Types.IDataItem[] items = new Types.IDataItem[this.source.Count];
				this.source.CopyTo (items, 0);
				data = items;
				return true;
			}
			
			data = null;
			
			return false;
		}
		
		public override bool WriteData(object data)
		{
			return false;
		}
		
		
		protected void Attach()
		{
			Support.Data.IChangedSource changed_source = this.source as Support.Data.IChangedSource;
			
			if (changed_source != null)
			{
				changed_source.Changed += new Support.EventHandler (this.HandleSourceContentsChanged);
			}
		}
		
		protected void Detach()
		{
			Support.Data.IChangedSource changed_source = this.source as Support.Data.IChangedSource;
			
			if (changed_source != null)
			{
				changed_source.Changed -= new Support.EventHandler (this.HandleSourceContentsChanged);
			}
		}
		
		
		protected virtual void OnSourceChanged()
		{
			this.SyncToAdapter (SyncReason.SourceChanged);
		}
		
		protected virtual void OnSourceContentsChanged()
		{
			this.SyncToAdapter (SyncReason.ValueChanged);
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Detach ();
				this.source = null;
			}
		}
		
		
		private void HandleSourceContentsChanged(object sender)
		{
			this.OnSourceContentsChanged ();
		}
		
		
		private Types.IDataFolder				source;
	}
}
