//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Binders
{
	/// <summary>
	/// La classe DataValueBinder permet d'acc�der � la propri�t� d'un objet
	/// comme source de donn�es.
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
					this.source = value;
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
		
		
		protected virtual void OnSourceChanged()
		{
			this.SyncToAdapter ();
		}
		
		protected virtual void OnPropertyChanged()
		{
			this.SyncToAdapter ();
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
				this.source = null;
			}
		}
		
		
		
		private Types.IDataValue				source;
	}
}
