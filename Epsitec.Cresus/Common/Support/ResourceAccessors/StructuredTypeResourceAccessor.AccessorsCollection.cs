//	Copyright © 2007-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Support.ResourceAccessors.StructuredTypeResourceAccessor))]

namespace Epsitec.Common.Support.ResourceAccessors
{
	public partial class StructuredTypeResourceAccessor : CaptionResourceAccessor
	{
		/// <summary>
		/// The <c>AccessorsCollection</c> maintains a collection of <see cref="StructuredTypeReourceAccessor"/>
		/// instances.
		/// </summary>
		private sealed class AccessorsCollection
		{
			public AccessorsCollection()
			{
				this.list = new List<Weak<StructuredTypeResourceAccessor>> ();
			}

			public void Add(StructuredTypeResourceAccessor item)
			{
				this.list.Add (new Weak<StructuredTypeResourceAccessor> (item));
			}

			public void Remove(StructuredTypeResourceAccessor item)
			{
				this.list.RemoveAll (
					delegate (Weak<StructuredTypeResourceAccessor> probe)
					{
						if (probe.IsAlive)
						{
							return probe.Target == item;
						}
						else
						{
							return true;
						}
					});
			}

			public IEnumerable<StructuredTypeResourceAccessor> Collection
			{
				get
				{
					foreach (Weak<StructuredTypeResourceAccessor> item in this.list)
					{
						StructuredTypeResourceAccessor accessor = item.Target;

						if (accessor != null)
						{
							yield return accessor;
						}
					}
				}
			}

			private List<Weak<StructuredTypeResourceAccessor>> list;
		}
	}
}
