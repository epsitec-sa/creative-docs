//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Binders
{
	/// <summary>
	/// La classe PropertyBinder permet d'accéder à la propriété d'un objet
	/// comme source de données.
	/// </summary>
	public class PropertyBinder : AbstractBinder
	{
		public PropertyBinder()
		{
		}
		
		public PropertyBinder(string property_name) : this ()
		{
			this.PropertyName = property_name;
		}
		
		
		public object							Source
		{
			get
			{
				return this.source_object;
			}
			set
			{
				if (this.source_object != value)
				{
					this.source_object = value;
					this.UpdateSourceType ();
					this.OnSourceChanged ();
				}
			}
		}
		
		public string							PropertyName
		{
			get
			{
				return this.prop_name;
			}
			set
			{
				if (this.prop_name != value)
				{
					this.prop_name = value;
					this.UpdatePropertyInfo ();
					this.OnPropertyChanged ();
				}
			}
		}
		
		public override string					Caption
		{
			get
			{
				return this.prop_name;
			}
		}
		
		public override bool					IsValid
		{
			get
			{
				return this.prop_info != null;
			}
		}
		
		
		public override System.Type GetDataType()
		{
			return this.prop_info == null ? null : this.prop_info.PropertyType;
		}
		
		public override bool ReadData(out object data)
		{
			if (this.IsValid && this.prop_info.CanRead)
			{
				data = this.prop_info.GetValue (this.source_object, null);
				return true;
			}
			
			data = null;
			
			return false;
		}
		
		public override bool WriteData(object data)
		{
			if (this.IsValid && this.prop_info.CanWrite)
			{
				this.prop_info.SetValue (this.source_object, data, null);
				return true;
			}
			
			return false;
		}
		
		
		protected void UpdateSourceType()
		{
			System.Type type = this.source_object == null ? null : this.source_object.GetType ();
			
			if (this.source_type != type)
			{
				this.source_type = type;
				this.OnSourceTypeChanged ();
				this.UpdatePropertyInfo ();
			}
		}
		
		protected void UpdatePropertyInfo()
		{
			if ((this.source_type != null) &&
				(this.prop_name != null))
			{
				this.prop_info = this.source_type.GetProperty (this.prop_name);
			}
			else
			{
				this.prop_info = null;
			}
		}
		
		
		protected virtual void OnSourceChanged()
		{
			this.SyncToAdapter ();
		}
		
		protected virtual void OnSourceTypeChanged()
		{
		}
		
		protected virtual void OnPropertyChanged()
		{
			this.SyncToAdapter ();
		}
		
		
		private object							source_object;
		private System.Type						source_type;
		private string							prop_name;
		private System.Reflection.PropertyInfo	prop_info;
	}
}
