//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Binders
{
	/// <summary>
	/// La classe DataValueBinder permet de faire le lien avec une source
	/// compatible avec l'interface IDataValue.
	/// </summary>
	public class DataValueBinder : AbstractBinder, System.IDisposable
	{
		public DataValueBinder()
		{
		}
		
		public DataValueBinder(Types.IDataValue source) : this ()
		{
			this.Source = source;
		}
		
		
		public Types.IDataValue					Source
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
			return this.source == null ? null : this.source.DataType.SystemType;
		}
		
		public override bool ReadData(out object data)
		{
			if (this.IsValid)
			{
				data = this.source.ReadValue ();
				return true;
			}
			
			data = null;
			
			return false;
		}
		
		public override bool WriteData(object data)
		{
			if (this.IsValid)
			{
				this.source.WriteValue (data);
				return true;
			}
			
			return false;
		}
		
//		public override bool NotifyInvalidData()
//		{
//			if (this.IsValid)
//			{
//				this.source.NotifyInvalidData ();
//				return true;
//			}
//			
//			return false;
//		}

		
		
		protected void Attach()
		{
			Types.IChange change_source = this.source as Types.IChange;
			
			if (change_source != null)
			{
				change_source.Changed += new Support.EventHandler (this.HandleSourceValueChanged);
			}
		}
		
		protected void Detach()
		{
			Types.IChange change_source = this.source as Types.IChange;
			
			if (change_source != null)
			{
				change_source.Changed -= new Support.EventHandler (this.HandleSourceValueChanged);
			}
		}
		
		
		protected virtual void OnSourceChanged()
		{
			this.SyncToAdapter (SyncReason.SourceChanged);
		}
		
		protected virtual void OnSourceValueChanged()
		{
			object value = this.source.ReadValue ();
			
			if (Types.InvalidValue.IsValueInvalid (value) == false)
			{
				this.SyncToAdapter (SyncReason.ValueChanged);
			}
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
		
		
		private void HandleSourceValueChanged(object sender)
		{
			this.OnSourceValueChanged ();
		}
		
		
		private Types.IDataValue				source;
	}
}
