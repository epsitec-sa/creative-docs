//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public struct InheritedPropertyCache : IInheritedPropertyCache
	{
		internal void SetValue(DependencyObject node, DependencyProperty property, object value)
		{
			PropertyValuePair[] propertyValues = new PropertyValuePair[1];
			propertyValues[0] = new PropertyValuePair (property, value);
			this.SetValues (node, propertyValues);
		}
		internal void ClearValue(DependencyObject node, DependencyProperty property)
		{
			DependencyProperty[] properties = new DependencyProperty[] { property };
			this.ClearValues (node, properties);
		}
		
		#region IInheritedPropertyCache Members

		public void ClearAllValues(DependencyObject node)
		{
			ListWrapper<DependencyProperty> list = new ListWrapper<DependencyProperty> ();
			
			if (this.cachedValueFlags != 0)
			{
				int mask = 1;

				for (int i = 0; i < InheritedPropertyCache.MaskBits; i++)
				{
					if (this.cachedValueFlags == 0)
					{
						break;
					}
					if ((mask & this.cachedValueFlags) != 0)
					{
						DependencyProperty property = DependencyProperty.GetInheritedPropertyFromCacheMask (mask);
						
						if (node.ContainsLocalValue (property) == false)
						{
							list.Add (property);
							
							this.cachedValueFlags &= (byte) ~mask;
							this.currentValues &= (byte) ~mask;
							this.currentValues |= (byte) (mask & this.defaultValues);
						}
					}
					
					mask = mask << 1;
				}
			}
			
			if (this.more != null)
			{
				this.more.ClearAllValues (node, ref list);

				if ((this.more.ContainsChanges () == false) &&
					(this.more.ContainsData () == false))
				{
					//	There is no longer any useful information in the additional
					//	cache, so simply release it:
					
					this.more = null;
				}
			}

			if (list.Instance != null)
			{
				InheritedPropertyCache.ClearChildrenProperties (node, list.Instance);
			}
		}
		public void ClearValues(DependencyObject node, IEnumerable<DependencyProperty> properties)
		{
			ListWrapper<DependencyProperty> list = new ListWrapper<DependencyProperty> ();
			
			foreach (DependencyProperty property in properties)
			{
				byte mask = (byte) property.InheritedPropertyCacheMask;

				if (mask == 0)
				{
					if (this.more != null)
					{
						if (node.ContainsLocalValue (property) == false)
						{
							this.more.ClearValue (ref list, property);

							if ((this.more.ContainsChanges () == false) &&
								(this.more.ContainsData () == false))
							{
								//	There is no longer any useful information in the additional
								//	cache, so simply release it:

								this.more = null;
							}
						}
					}
				}
				else
				{
					if ((mask & this.cachedValueFlags) != 0)
					{
						if (node.ContainsLocalValue (property) == false)
						{
							byte bits;
							bits  = this.currentValues;
							bits ^= this.defaultValues;
							bits &= mask;

							this.cachedValueFlags ^= mask;
							this.currentValues    ^= bits;

							list.Add (property);
						}
					}
				}
			}
			
			if (list.Instance != null)
			{
				InheritedPropertyCache.ClearChildrenProperties (node, list.Instance);
			}
		}

		public IEnumerable<PropertyValuePair> GetValues(DependencyObject node)
		{
			if (this.cachedValueFlags != 0)
			{
				int mask = 1;

				for (int i = 0; i < InheritedPropertyCache.MaskBits; i++)
				{
					if ((mask & this.cachedValueFlags) != 0)
					{
						DependencyProperty property = DependencyProperty.GetInheritedPropertyFromCacheMask (mask);
						bool value = ((this.currentValues & mask) != 0) ? true : false;
						yield return new PropertyValuePair (property, value);
					}

					mask = mask << 1;
				}
			}

			if (this.more != null)
			{
				foreach (PropertyValuePair propertyValue in this.more.GetValues (node))
				{
					yield return propertyValue;
				}
			}
		}
		
		public void InheritValuesFromParent(DependencyObject node, DependencyObject parent)
		{
			if (parent == null)
			{
				this.ClearAllValues (node);
			}
			else
			{
				this.SetValues (node, InheritedPropertyCache.OnlyUndefinedEntries (node, parent.InheritedPropertyCache.GetValues (parent)));
			}
		}

		private static IEnumerable<PropertyValuePair> OnlyUndefinedEntries(DependencyObject node, IEnumerable<PropertyValuePair> entries)
		{
			foreach (PropertyValuePair entry in entries)
			{
				if (node.ContainsLocalValue (entry.Property) == false)
				{
					yield return entry;
				}
			}
		}

		public void SetValues(DependencyObject node, IEnumerable<PropertyValuePair> propertyValues)
		{
			ListWrapper<PropertyValuePair> list = new ListWrapper<PropertyValuePair> ();

			foreach (PropertyValuePair propertyValue in propertyValues)
			{
				byte mask = (byte) propertyValue.Property.InheritedPropertyCacheMask;

				if (mask == 0)
				{
					if (this.more == null)
					{
						this.more = new MoreSmall ();
					}

					this.more = this.more.SetValue (node, propertyValue.Property, propertyValue.Value);
					list.Add (propertyValue);
				}
				else
				{
					byte bits;

					if ((mask & this.cachedValueFlags) == 0)
					{
						bits  = ((bool) propertyValue.Property.DefaultMetadata.DefaultValue) ? mask : (byte) 0;
						bits ^= this.defaultValues;
						bits &= mask;
						
						this.defaultValues ^= bits;

						bits  = this.defaultValues;
						bits ^= this.oldValues;
						bits &= mask;

						this.oldValues ^= bits;
					}
					
					bits  = ((bool) propertyValue.Value) ? mask : (byte) 0;
					bits ^= this.currentValues;
					bits &= mask;
					
					if (((mask & this.cachedValueFlags) == 0) ||
						(bits != 0))
					{
						//	This property is not yet cached, or its value is not
						//	up-to-date.

						this.cachedValueFlags |= mask;
						this.currentValues    ^= bits;

						list.Add (propertyValue);
					}
				}
			}

			if (list.Instance != null)
			{
				InheritedPropertyCache.SetChildrenProperties (node, list.Instance);
			}
		}

		public bool IsDefined(DependencyObject node, DependencyProperty property)
		{
			byte mask = (byte) property.InheritedPropertyCacheMask;

			if (mask == 0)
			{
				if (this.more != null)
				{
					return this.more.IsDefined (property);
				}

				return false;
			}
			else
			{
				if ((mask & this.cachedValueFlags) != 0)
				{
					return true;
				}
			}

			return false;
		}
		public bool TryGetValue(DependencyObject node, DependencyProperty property, out object value)
		{
			byte mask = (byte) property.InheritedPropertyCacheMask;

			if (mask == 0)
			{
				if (this.more != null)
				{
					return this.more.TryGetValue (property, out value);
				}

				value = UndefinedValue.Value;
				return false;
			}
			else
			{
				if ((mask & this.cachedValueFlags) != 0)
				{
					if ((mask & this.currentValues) != 0)
					{
						value = true;
					}
					else
					{
						value = false;
					}
					
					return true;
				}
			}
			
			value = UndefinedValue.Value;
			return false;
		}

		public void NotifyChanges(DependencyObject node)
		{
			int changeCount = 0;
			byte changes;
			
			changes  = this.oldValues;
			changes ^= this.currentValues;
			
			if (changes != 0)
			{
				int mask = 1;
				
				for (int i = 0; i < InheritedPropertyCache.MaskBits; i++)
				{
					if ((changes & mask) != 0)
					{
						DependencyProperty property = DependencyProperty.GetInheritedPropertyFromCacheMask (mask);
						
						object oldValue = ((this.oldValues & mask) != 0) ? true : false;
						object newValue = ((this.currentValues & mask) != 0) ? true : false;
						
						node.InvalidateProperty (property, oldValue, newValue);
						changeCount++;
					}
					
					mask = mask << 1;
				}

				this.oldValues = this.currentValues;
			}
			if (this.more != null)
			{
				changeCount += this.more.NotifyChanges (node);
			}
			
			//	And now, if we had local changes, we also walk through all the
			//	children nodes :

			if (changeCount > 0)
			{
				if (DependencyObjectTree.GetHasChildren (node))
				{
					ICollection<DependencyObject> children = DependencyObjectTree.GetChildren (node);

					foreach (DependencyObject child in children)
					{
						child.InheritedPropertyCache.NotifyChanges (child);
					}
				}
			}
		}

		#endregion

		#region Generic ListWrapper Structure
		struct ListWrapper<T>
		{
			public List<T> Instance
			{
				get
				{
					return this.list;
				}
			}
			
			public void Add(T property)
			{
				if (this.list == null)
				{
					this.list = new List<T> ();
				}
				this.list.Add (property);
			}

			private List<T> list;
		}
		#endregion

		private static void ClearChildrenProperties(DependencyObject node, List<DependencyProperty> properties)
		{
			//	Walk through the node's children and clear the specified properties.

			System.Diagnostics.Debug.Assert (properties != null);
			System.Diagnostics.Debug.Assert (properties.Count > 0);

			if (DependencyObjectTree.GetHasChildren (node))
			{
				ICollection<DependencyObject> children = DependencyObjectTree.GetChildren (node);
				
				foreach (DependencyObject child in children)
				{
					child.InheritedPropertyCache.ClearValues (child, properties);
				}
			}
		}
		private static void SetChildrenProperties(DependencyObject node, List<PropertyValuePair> properties)
		{
			//	Walk through the node's children and set the specified properties.

			System.Diagnostics.Debug.Assert (properties != null);
			System.Diagnostics.Debug.Assert (properties.Count > 0);

			if (DependencyObjectTree.GetHasChildren (node))
			{
				ICollection<DependencyObject> children = DependencyObjectTree.GetChildren (node);

				foreach (DependencyObject child in children)
				{
					child.InheritedPropertyCache.SetValues (child, InheritedPropertyCache.OnlyUndefinedEntries (child, properties));
				}
			}
		}


		private abstract class More
		{
			public abstract void ClearAllValues(DependencyObject node, ref ListWrapper<DependencyProperty> list);
			public abstract void ClearValue(ref ListWrapper<DependencyProperty> list, DependencyProperty property);
			public abstract bool ContainsChanges();
			public abstract bool ContainsData();
			public abstract More SetValue(DependencyObject node, DependencyProperty property, object value);
			public abstract bool IsDefined(DependencyProperty property);
			public abstract bool TryGetValue(DependencyProperty property, out object value);
			public abstract int NotifyChanges(DependencyObject node);
			public abstract IEnumerable<Epsitec.Common.Types.PropertyValuePair> GetValues(DependencyObject node);
		}
		private class MoreSmall : More
		{
			public override void ClearAllValues(DependencyObject node, ref ListWrapper<DependencyProperty> list)
			{
				if (this.isValueDefined)
				{
					if (node.ContainsLocalValue (this.property) == false)
					{
						list.Add (this.property);

						this.currentValue    = this.defaultValue;
						this.hasValueChanged = true;
						this.isValueDefined  = false;
					}
				}
			}
			public override void ClearValue(ref ListWrapper<DependencyProperty> list, DependencyProperty property)
			{
				if (this.isValueDefined)
				{
					if (this.property == property)
					{
						list.Add (this.property);

						this.currentValue    = this.defaultValue;
						this.hasValueChanged = true;
						this.isValueDefined  = false;
					}
				}
			}

			public override bool ContainsChanges()
			{
				if (this.hasValueChanged)
				{
					if (this.oldValue != this.currentValue)
					{
						if ((this.oldValue == null) ||
							(UndefinedValue.IsUndefinedValue (this.oldValue)) ||
							(this.oldValue.Equals (this.currentValue) == false))
						{
							return true;
						}
					}
				}
				
				return false;
			}
			public override bool ContainsData()
			{
				return this.isValueDefined;
			}
			public override More SetValue(DependencyObject node, DependencyProperty property, object value)
			{
				if (this.isValueDefined)
				{
					if (this.property == property)
					{
						this.currentValue = value;
						this.hasValueChanged = true;

						return this;
					}
					else
					{
						//	TODO: ...
						
						throw new System.NotImplementedException ();
					}
				}
				else
				{
					if (this.property != property)
					{
						this.property     = property;
						this.defaultValue = property.DefaultMetadata.CreateDefaultValue ();
						this.oldValue     = this.defaultValue;
					}
					
					this.currentValue    = value;
					
					this.hasValueChanged = true;
					this.isValueDefined  = true;
					
					return this;
				}
			}

			public override int NotifyChanges(DependencyObject node)
			{
				int changeCount = 0;
				
				if (this.hasValueChanged)
				{
					if (this.oldValue != this.currentValue)
					{
						if ((this.oldValue == null) ||
							(UndefinedValue.IsUndefinedValue (this.oldValue)) ||
							(this.oldValue.Equals (this.currentValue) == false))
						{
							node.InvalidateProperty (this.property, this.oldValue, this.currentValue);
							
							this.oldValue = this.currentValue;
							changeCount++;
						}
					}
					
					this.hasValueChanged = false;
				}
				
				return changeCount;
			}
			public override bool IsDefined(DependencyProperty property)
			{
				if (this.isValueDefined)
				{
					if (this.property == property)
					{
						return true;
					}
				}
				
				return false;
			}
			public override bool TryGetValue(DependencyProperty property, out object value)
			{
				if (this.property == property)
				{
					value = this.currentValue;
					return true;
				}
				
				value = UndefinedValue.Value;
				return false;
			}
			public override IEnumerable<Epsitec.Common.Types.PropertyValuePair> GetValues(DependencyObject node)
			{
				if (this.isValueDefined)
				{
					yield return new PropertyValuePair (this.property, this.currentValue);
				}
			}

			private DependencyProperty property;
			private object oldValue;
			private object defaultValue;
			private object currentValue;
			private bool isValueDefined;
			private bool hasValueChanged;
		}
		private abstract class MoreExtended : More
		{
		}
		
		internal const int MaskBits = 8;
		
		private More more;
		private byte oldValues;
		private byte defaultValues;
		private byte currentValues;
		private byte cachedValueFlags;
	}

	public interface IInheritedPropertyCache
	{
		void ClearAllValues(DependencyObject node);
		void ClearValues(DependencyObject node, IEnumerable<DependencyProperty> properties);
		void SetValues(DependencyObject node, IEnumerable<PropertyValuePair> propertyValues);
		IEnumerable<PropertyValuePair> GetValues(DependencyObject node);
		void InheritValuesFromParent(DependencyObject node, DependencyObject parent);
		bool IsDefined(DependencyObject node, DependencyProperty property);
		bool TryGetValue(DependencyObject node, DependencyProperty property, out object value);
		void NotifyChanges(DependencyObject node);
	}
}
