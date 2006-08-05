//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using System.Collections.Generic;

namespace Epsitec.Common.UI.Data
{
	/// <summary>
	/// La classe Record décrit un ensemble de champs utilisés pour échanger des
	/// données entre une application et son interface via mapper/binder/...
	/// </summary>
	public class ObsoleteRecord : Types.AbstractDataCollection, Types.IDataFolder, Types.IDataGraph, Types.IChange
	{
		public ObsoleteRecord()
		{
			this.graph = new Types.DataGraph (this);
		}
		
		public ObsoleteRecord(string name) : this (name, null)
		{
		}
		
		public ObsoleteRecord(string name, string prefix) : this ()
		{
			this.DefineName (name);
			this.DefineResourcePrefix (prefix);
		}
		
		
		public new ObsoleteField						this[string name]
		{
			get
			{
				return base[name] as ObsoleteField;
			}
		}

		public new ObsoleteField						this[int index]
		{
			get
			{
				return base[index] as ObsoleteField;
			}
		}
		
		public IValidator						Validator
		{
			get
			{
				if (this.validator == null)
				{
					this.validator = new RecordValidator (this);
				}
				
				return this.validator;
			}
		}
		
		
		public void DefineName(string name)
		{
			if (name == "")
			{
				name = null;
			}
			
			this.name = name;
		}
		
		public void DefineResourcePrefix(string prefix)
		{
			if (prefix == "")
			{
				prefix = null;
			}
			
			this.resource_prefix = prefix;
		}
		
		
		
		public override void Add(Types.IDataItem item)
		{
			ObsoleteField field = item as ObsoleteField;
			
			if (field == null)
			{
				throw new System.ArgumentException ("Expected a valid field.", "item");
			}
			
			base.Add (field);
			
			field.AttachToRecord (this);
			
			this.OnChanged ();
		}

		public override bool Remove(Types.IDataItem item)
		{
			if (base.Remove (item))
			{
				ObsoleteField field = item as ObsoleteField;
				field.DetachFromRecord (this);
				this.OnChanged ();
				return true;
			}
			else
			{
				return false;
			}
		}

		public override void AddRange(System.Collections.ICollection fields)
		{
			if (fields.Count > 0)
			{
				foreach (object field in fields)
				{
					if (!(field is ObsoleteField))
					{
						throw new System.ArgumentException ("Collection contains invalid field.", "fields");
					}
				}
				
				base.AddRange (fields);
				
				foreach (ObsoleteField field in fields)
				{
					field.AttachToRecord (this);
				}
				
				this.OnChanged ();
			}
		}
		
		public void AddGraph(Types.IDataGraph graph)
		{
			if ((graph == null) ||
				(graph.Root == null))
			{
				return;
			}
			
			int count = graph.Root.Count;
			
			if (count == 0)
			{
				return;
			}
			
			Types.IDataItem[] items  = new Types.IDataItem[count];
			List<ObsoleteField>       fields = new List<ObsoleteField> ();
			
			graph.Root.CopyTo (items, 0);
			
			for (int i = 0; i < count; i++)
			{
				if (items[i].Classes == Types.DataItemClasses.Value)
				{
					fields.Add (ObsoleteField.CreateFromValue (items[i] as Types.IDataValue));
				}
			}
			
			this.AddRange (fields);
		}
		
		public override void Clear()
		{
			if (this.Count > 0)
			{
				foreach (ObsoleteField field in this)
				{
					field.DetachFromRecord (this);
				}
				
				base.Clear ();
				this.OnChanged ();
			}
		}
		
		
		public ObsoleteField AddField(string name)
		{
			return this.InitFieldAndAdd (new ObsoleteField (name));
		}
		
		public ObsoleteField AddField(string name, object value)
		{
			return this.InitFieldAndAdd (new ObsoleteField (name, value));
		}
		
		public ObsoleteField AddField(string name, object value, Types.INamedType type)
		{
			return this.InitFieldAndAdd (new ObsoleteField (name, value, type));
		}
		
		public ObsoleteField AddField(string name, object value, Types.INamedType type, Types.IDataConstraint constraint)
		{
			return this.InitFieldAndAdd (new ObsoleteField (name, value, type, constraint));
		}
		
		
		public Types.IDataValue[] GetDataValues()
		{
			if (this.fields == null)
			{
				this.UpdateCachedItemArray ();
			}
			
			Types.IDataValue[] values = new Types.IDataValue[this.fields.Length];
			this.fields.CopyTo (values, 0);
			
			return values;
		}
		
		
		public ObsoleteRecord Clone()
		{
			System.ICloneable cloneable = this;

			return cloneable.Clone () as ObsoleteRecord;
		}
		
		
		#region IDataItem Members
		public Types.DataItemClasses			Classes
		{
			get
			{
				return Types.DataItemClasses.Folder;
			}
		}
		#endregion
		
		#region INameCaption Members

		public Support.Druid CaptionId
		{
			get
			{
				return Support.Druid.Empty;
			}
		}
		#endregion

		#region IName Members

		public string							Name
		{
			get
			{
				return this.name;
			}
		}
		#endregion
		
		#region IDataGraph Members
		public Types.IDataCollection Select(string query)
		{
			return this.graph.Select (query);
		}
		
		public Types.IDataFolder						Root
		{
			get
			{
				return this;
			}
		}
		
		public Types.IDataItem Navigate(string path)
		{
			return this.graph.Navigate (path);
		}
		#endregion
		
		protected override object CloneNewObject()
		{
			return new ObsoleteRecord ();
		}
		
		protected override object CloneCopyToNewObject(object o)
		{
			ObsoleteRecord that = o as ObsoleteRecord;
			
			base.CloneCopyToNewObject (o);
			
			that.DefineName (this.name);
			that.DefineResourcePrefix (this.resource_prefix);
			
			return that;
		}
		
		protected virtual ObsoleteField InitFieldAndAdd(ObsoleteField field)
		{
			string field_name = field.Name;
			
#if false //#fix
			if ((field_name != null) &&
				(field_name.Length > 0) &&
				(this.resource_prefix != null) &&
				(this.name != null))
			{
				//	Les valeurs de 'caption' et 'description' sont recherchées dans les ressources définies
				//	par un chemin de type "base:records#RecordName.FieldName.capt" et "...desc".
				
				string caption     = string.Concat (this.resource_prefix, "#", this.name, ".", field_name, ".", Support.Tags.Caption);
				string description = string.Concat (this.resource_prefix, "#", this.name, ".", field_name, ".", Support.Tags.Description);
				
				field.DefineCaption (caption);
				field.DefineDescription (description);
				
				//	Traitement spécial des champs qui représentent des énumérations; pour ceux-ci, il faut
				//	essayer d'affecter des valeurs 'caption' et 'description' pour chaque élément :
				
				Types.INamedType type = field.DataType;
				
				if (type is Types.EnumType)
				{
					Types.EnumType enum_type = type as Types.EnumType;
					string         enum_id   = string.Concat (this.resource_prefix, "#Type.", enum_type.SystemType.Name);
					
					//	On va utiliser les strings du type "base:records#Type.EnumType.Xyz.capt" pour
					//	définir par exemple 'caption' pour la valeur 'Xyz' du type 'EnumType'.
					
					enum_type.DefineTextsFromResources (enum_id);
				}
			}
#endif
			
			this.Add (field);
			
			return field;
		}
		
		
		protected virtual void OnChanged()
		{
			if (this.validator != null)
			{
				this.validator.MakeDirty (false);
			}
			
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}
		
		protected virtual void OnFieldChanged(ObsoleteField field)
		{
			if (this.validator != null)
			{
				this.validator.MakeDirty (false);
			}
			
			if (this.FieldChanged != null)
			{
				this.FieldChanged (field);
			}
		}
		
		
		protected override Types.IDataItem[] GetCachedItemArray()
		{
			return this.fields;
		}
		
		protected override void ClearCachedItemArray()
		{
			base.ClearCachedItemArray ();
			
			this.fields = null;
		}
		
		protected override void UpdateCachedItemArray()
		{
			base.UpdateCachedItemArray ();
			
			this.fields = new ObsoleteField[this.Count];
			this.CopyTo (this.fields, 0);
		}

		
		internal void NotifyFieldChanged(ObsoleteField field)
		{
			this.OnFieldChanged (field);
		}
		
		
		#region RecordValidator Class
		protected class RecordValidator : IValidator
		{
			public RecordValidator(ObsoleteRecord record)
			{
				this.record = record;
				this.state  = ValidationState.Dirty;
			}
			
			
			public ValidationState				State
			{
				get
				{
					return this.state;
				}
			}
			
			public bool							IsValid
			{
				get
				{
					if (this.state == ValidationState.Dirty)
					{
						this.Validate ();
					}
				
					return (this.state == ValidationState.Ok);
				}
			}
			
			public string						ErrorMessage
			{
				get
				{
					return null;
				}
			}
			
			
			public void Validate()
			{
				this.state = ValidationState.Unknown;
				
				foreach (ObsoleteField field in this.record.CachedItemArray)
				{
					if (field.IsValueValid == false)
					{
						this.state = ValidationState.Error;
						return;
					}
				}
				
				this.state = ValidationState.Ok;
			}
			
			public void MakeDirty(bool deep)
			{
				this.state = ValidationState.Dirty;
				this.OnBecameDirty ();
			}
			
			
			protected void OnBecameDirty()
			{
				if (this.BecameDirty != null)
				{
					this.BecameDirty (this);
				}
			}
			
			
			public event Support.EventHandler	BecameDirty;
			
			private ObsoleteRecord						record;
			private ValidationState				state;
		}
		#endregion
		
		public event Support.EventHandler		Changed;
		public event Support.EventHandler		FieldChanged;
		
		private ObsoleteField[]							fields;
		private Types.DataGraph					graph;
		private string							name;
		private string							resource_prefix;
		private RecordValidator					validator;
	}
}
