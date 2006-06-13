//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.UI.Data
{
	/// <summary>
	/// La classe Field décrit un champ d'un Record, utilisé pour échanger des
	/// données entre une application et son interface via mapper/binder/...
	/// </summary>
	public class ObsoleteField : Types.IDataValue, Types.IChange
	{
		public ObsoleteField()
		{
		}
		
		public ObsoleteField(string name)
		{
			this.name        = name;
			this.caption     = null;
			this.description = null;
		}
		
		public ObsoleteField(string name, object value) : this (name)
		{
			this.DefineValue (value);
		}
		
		public ObsoleteField(string name, object value, Types.INamedType type) : this (name)
		{
			if (type == null)
			{
				this.DefineValue (value);
			}
			else
			{
				this.DefineValue (value, type);
			}
			
			if (type is Types.IDataConstraint)
			{
				this.DefineConstraint (type as Types.IDataConstraint);
			}
		}
		
		public ObsoleteField(string name, object value, Types.INamedType type, Types.IDataConstraint constraint) : this (name, value, type)
		{
			this.DefineConstraint (constraint);
		}
		
		
		public object							Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (Common.Types.Comparer.Equal (this.value, value) == false)
				{
					if (this.type == null)
					{
						throw new System.InvalidOperationException ("Value cannot be changed; no type is defined.");
					}
					
					this.value = value;
					this.OnChanged ();
				}
			}
		}
		
		public ObsoleteRecord							Record
		{
			get
			{
				return this.record;
			}
		}
		
		
		public void DefineValue(object value)
		{
			if (value is string)
			{
				this.value = value;
				this.type  = new Types.StringType ();
				
				return;
			}
			if (value is decimal)
			{
				decimal large = 1000000000000M;
				decimal small = 1M / large;
				decimal max   = large - small;
				
				this.value = value;
				this.type  = new Types.DecimalType (-max, max, small);
				
				return;
			}
			if (value is bool)
			{
				this.value = value;
				this.type  = new Types.BooleanType ();
				
				return;
			}
			if (value is int)
			{
				this.value = value;
				this.type  = new Types.IntegerType ();
				
				return;
			}
			if (value is System.Enum)
			{
				this.value = value;
				this.type  = new Types.EnumType (value.GetType ());
				
				return;
			}
			
			throw new System.ArgumentException (string.Format ("The specified value's type is not supported ({0})", value.GetType ()));
		}
		
		public void DefineValue(object value, Types.INamedType type)
		{
			this.value = value;
			this.type  = type;
		}
		
		public void DefineConstraint(Types.IDataConstraint constraint)
		{
			this.constraint = constraint;
		}
		
		public void DefineCaption(string caption)
		{
			this.caption = caption;
		}
		
		public void DefineDescription(string description)
		{
			this.description = description;
		}
		
		
		#region IDataValue Members
		public Types.INamedType					DataType
		{
			get
			{
				return this.type;
			}
		}
		
		public Types.IDataConstraint			DataConstraint
		{
			get
			{
				return this.constraint;
			}
		}
		
		public bool								IsValueValid
		{
			get
			{
				object value = this.Value;
				return Types.InvalidValue.IsValueInvalid (value) ? false : true;
			}
		}
		
		public object ReadValue()
		{
			object value = this.Value;
			
			if ((value != null) &&
				(this.type != null) &&
				(value.GetType () != this.type.SystemType))
			{
				if (value is string)
				{
					if (Common.Types.InvariantConverter.Convert (value, this.type.SystemType, out value))
					{
						return value;
					}
				}
				
				throw new System.InvalidOperationException ("Value is not of required type.");
			}
			
			return value;
		}

		public void WriteValue(object value)
		{
			if (this.type != null)
			{
				if (Common.Types.InvariantConverter.Convert (value, this.type.SystemType, out value))
				{
					this.Value = value;
				}
				else
				{
					throw new System.InvalidOperationException ("Value cannot be mapped to required type.");
				}
			}
			else
			{
				this.Value = value;
			}
		}
		
//		public void NotifyInvalidData()
//		{
//			//	Donnée pas valide...
//			
//			this.is_value_valid = false;
//			this.OnChanged ();
//		}
		#endregion

		#region IDataItem Members
		public Types.DataItemClasses			Classes
		{
			get
			{
				return Types.DataItemClasses.Value;
			}
		}
		#endregion

		#region INameCaption Members
		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		
		public string							Caption
		{
			get
			{
				return this.caption;
			}
		}
		
		public string							Description
		{
			get
			{
				return this.description;
			}
		}
		#endregion
		
		#region IChange Members
		public event Support.EventHandler		Changed;
		#endregion
		
		#region ICloneable Members
		object System.ICloneable.Clone()
		{
			return this.Clone ();
		}
		#endregion
		
		public ObsoleteField Clone()
		{
			return this.CloneCopyToNewObject (this.CloneNewObject ()) as ObsoleteField;
		}
		
		
		internal void AttachToRecord(ObsoleteRecord record)
		{
			System.Diagnostics.Debug.Assert (this.record == null);
			System.Diagnostics.Debug.Assert (record != null);
			
			this.record = record;
		}
		
		internal void DetachFromRecord(ObsoleteRecord record)
		{
			System.Diagnostics.Debug.Assert (this.record == record);
			
			this.record = null;
		}
		
		
		protected virtual object CloneNewObject()
		{
			return new ObsoleteField ();
		}
		
		protected virtual object CloneCopyToNewObject(object o)
		{
			//	La copie du champ ne se fait pas en profondeur, car on part de l'idée que
			//	seule la valeur présente un réel intérêt; celle-ci étant passée par valeur
			//	(justement !) elle n'a pas besoin de traitement particulier.
			
			ObsoleteField that = o as ObsoleteField;
			
			that.name           = this.name;
			that.caption        = this.caption;
			that.description    = this.description;
			that.type           = this.type;
			that.constraint     = this.constraint;
			that.value          = this.value;
//-			that.is_value_valid = this.is_value_valid;
			
			System.Diagnostics.Debug.Assert (Types.InvariantConverter.IsSimple (that.value));
			
			return that;
		}
		
		
		public static ObsoleteField CreateFromValue(Types.IDataValue value)
		{
			ObsoleteField field = new ObsoleteField (value.Name, null, value.DataType, value.DataConstraint);
			
			field.DefineCaption (value.Caption);
			field.DefineDescription (value.Description);
			
			return field;
		}
		
		
		protected virtual void OnChanged()
		{
			if (this.Changed != null)
			{
				this.Changed (this);
			}
			
			if (this.record != null)
			{
				this.record.NotifyFieldChanged (this);
			}
		}
		
		
		private string							name;
		private string							caption;
		private string							description;
		
		private Types.INamedType				type;
		private Types.IDataConstraint			constraint;
		
		private ObsoleteRecord							record;
		private object							value;
//-		private bool							is_value_valid;
	}
}
