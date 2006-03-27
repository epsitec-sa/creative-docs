//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public struct InheritedPropertyCache : IInheritedPropertyCache
	{
		#region IInheritedPropertyCache Members

		public void ClearAllValues(DependencyObject node)
		{
			ListWrapper<DependencyProperty> list = new ListWrapper<DependencyProperty> ();
			
			if (this.cachedValueFlags != 0)
			{
				int mask = 1;

				for (int i = 0; i < InheritedPropertyCache.MaskBits; i++)
				{
					if ((mask & this.cachedValueFlags) != 0)
					{
						list.Add (DependencyProperty.GetInheritedPropertyFromCacheMask (mask));
					}
					
					mask = mask << 1;
				}

				this.cachedValueFlags = 0;
				this.currentValues = this.defaultValues;
			}
			
			if (this.more != null)
			{
				this.more.ClearAllValues (ref list);

				if (this.more.ContainsChanges () == false)
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
						this.more.ClearValues (ref list, properties);
					}
				}
				else
				{
					if ((mask & this.cachedValueFlags) != 0)
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
			
			if (list.Instance != null)
			{
				InheritedPropertyCache.ClearChildrenProperties (node, list.Instance);
			}
		}

		public void SetValues(DependencyObject node, IEnumerable<Serialization.Generic.PropertyValue<object>> propertyValues)
		{
			ListWrapper<Serialization.Generic.PropertyValue<object>> list = new ListWrapper<Serialization.Generic.PropertyValue<object>> ();

			foreach (Serialization.Generic.PropertyValue<object> propertyValue in propertyValues)
			{
				byte mask = (byte) propertyValue.Property.InheritedPropertyCacheMask;

				if (mask == 0)
				{
					if (this.more != null)
					{
						//	TODO: ...
					}
					
					//	TODO: ...
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

		public bool TryGetValue(DependencyObject node, DependencyProperty property, out object value)
		{
			byte mask = (byte) property.InheritedPropertyCacheMask;

			if (mask == 0)
			{
				if (this.more != null)
				{
					//	TODO: ...
				}

				value = UndefinedValue.Instance;
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
			
			value = UndefinedValue.Instance;
			return false;
		}

		public void NotifyChanges(DependencyObject node)
		{
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
					}
					
					mask = mask << 1;
				}

				this.oldValues = this.currentValues;
			}
			if (this.more != null)
			{
				//	TODO: ...
			}
			
			//	And now also walk through all the children nodes :
			
			if (DependencyObjectTree.GetHasChildren (node))
			{
				ICollection<DependencyObject> children = DependencyObjectTree.GetChildren (node);

				foreach (DependencyObject child in children)
				{
					child.GetInheritedPropertyCache ().NotifyChanges (child);
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
					child.GetInheritedPropertyCache ().ClearValues (child, properties);
				}
			}
		}
		private static void SetChildrenProperties(DependencyObject node, List<Serialization.Generic.PropertyValue<object>> properties)
		{
			//	Walk through the node's children and set the specified properties.

			System.Diagnostics.Debug.Assert (properties != null);
			System.Diagnostics.Debug.Assert (properties.Count > 0);

			if (DependencyObjectTree.GetHasChildren (node))
			{
				ICollection<DependencyObject> children = DependencyObjectTree.GetChildren (node);

				foreach (DependencyObject child in children)
				{
					child.GetInheritedPropertyCache ().SetValues (child, properties);
				}
			}
		}


		private abstract class More
		{
			public abstract void ClearAllValues(ref ListWrapper<DependencyProperty> list);
			public abstract void ClearValues(ref ListWrapper<DependencyProperty> list, IEnumerable<DependencyProperty> properties);
			public abstract bool ContainsChanges();
		}
		private abstract class MoreSmall : More
		{
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
		void SetValues(DependencyObject node, IEnumerable<Serialization.Generic.PropertyValue<object>> propertyValues);
		bool TryGetValue(DependencyObject node, DependencyProperty property, out object value);
		void NotifyChanges(DependencyObject node);
	}
}
