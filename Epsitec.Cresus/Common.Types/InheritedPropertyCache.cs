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
			throw new System.Exception ("The method or operation is not implemented.");
		}

		public bool TryGetValue(DependencyObject node, DependencyProperty property, out object value)
		{
			throw new System.Exception ("The method or operation is not implemented.");
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

		private abstract class More
		{
			public abstract void ClearAllValues(ref ListWrapper<DependencyProperty> list);
			public abstract void ClearValues(ref ListWrapper<DependencyProperty> list, IEnumerable<DependencyProperty> properties);
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
	}
}
