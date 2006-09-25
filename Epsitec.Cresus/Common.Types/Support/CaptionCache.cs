//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Globalization;
using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>CaptionCache</c> class implements a cache for captions. The cache
	/// associates the caption ID with the resource manager used to allocate the
	/// caption.
	/// </summary>
	public sealed class CaptionCache
	{
		private CaptionCache()
		{
		}

		public Caption GetCaption(ResourceManager manager, long id)
		{
			if (id >= 0)
			{
				Druid druid = Druid.FromLong (id);
				return this.GetCaption (manager, druid, id);
			}
			else
			{
				return null;
			}
		}

		public Caption GetCaption(ResourceManager manager, Druid druid)
		{
			if (druid.Type == DruidType.Full)
			{
				long id = druid.ToLong ();
				return this.GetCaption (manager, druid, id);
			}
			else
			{
				return null;
			}
		}

		public Caption GetCaption(ResourceManager manager, string id)
		{
			Druid druid;

			if (Druid.TryParse (id, out druid))
			{
				return this.GetCaption (manager, druid);
			}
			else
			{
				return null;
			}
		}

		public Caption GetPropertyCaption(ResourceManager manager, DependencyProperty property)
		{
			if (property != null)
			{
				return this.GetCaption (manager, property.DefaultMetadata.CaptionId);
			}
			else
			{
				return null;
			}
		}

		public Caption GetPropertyCaption(ResourceManager manager, DependencyObject obj, DependencyProperty property)
		{
			if (property != null)
			{
				return this.GetCaption (manager, property.GetMetadata (obj).CaptionId);
			}
			else
			{
				return null;
			}
		}

		public Caption GetPropertyCaption(ResourceManager manager, System.Type type, DependencyProperty property)
		{
			if (property != null)
			{
				return this.GetCaption (manager, property.GetMetadata (type).CaptionId);
			}
			else
			{
				return null;
			}
		}

		public Caption GetTypeCaption(ResourceManager manager, INamedType type)
		{
			return this.GetObjectCaption (manager, type);
		}

		public Caption GetTypeCaption(ResourceManager manager, DependencyProperty property)
		{
			if (property != null)
			{
				INamedType type = property.DefaultMetadata.NamedType;
				return this.GetTypeCaption (manager, type ?? TypeRosetta.GetNamedTypeFromTypeObject (property.PropertyType));
			}
			else
			{
				return null;
			}
		}

		public Caption GetTypeCaption(ResourceManager manager, DependencyObject obj, DependencyProperty property)
		{
			if (property != null)
			{
				INamedType type = property.GetMetadata (obj).NamedType;
				return this.GetTypeCaption (manager, type ?? TypeRosetta.GetNamedTypeFromTypeObject (property.PropertyType));
			}
			else
			{
				return null;
			}
		}

		public Caption GetTypeCaption(ResourceManager manager, System.Type sysType, DependencyProperty property)
		{
			if (property != null)
			{
				INamedType type = property.GetMetadata (sysType).NamedType;
				return this.GetTypeCaption (manager, type ?? TypeRosetta.GetNamedTypeFromTypeObject (property.PropertyType));
			}
			else
			{
				return null;
			}
		}

		public Caption GetObjectCaption(ResourceManager manager, ICaption obj)
		{
			if (obj != null)
			{
				return this.GetCaption (manager, obj.CaptionId);
			}
			else
			{
				return null;
			}
		}

		public void TrimCache()
		{
			lock (this.exclusion)
			{
				Dictionary<BigKey, Weak<Caption>> copy = new Dictionary<BigKey, Weak<Caption>> ();

				foreach (KeyValuePair<BigKey, Weak<Caption>> pair in this.cache)
				{
					if (pair.Value.IsAlive)
					{
						copy.Add (pair.Key, pair.Value);
					}
				}

				this.cache = copy;
			}
		}

		public int DebugCountLiveCaptions()
		{
			int count = 0;
			
			foreach (KeyValuePair<BigKey, Weak<Caption>> pair in this.cache)
			{
				if (pair.Value.IsAlive)
				{
					count++;
				}
			}
			
			return count;
		}

		private Caption GetCaption(ResourceManager manager, Druid druid, long id)
		{
			Caption caption;
			BigKey key = new BigKey (manager, id);
			Weak<Caption> weak;

			if (this.cache.TryGetValue (key, out weak))
			{
				caption = weak.Target;

				if (caption != null)
				{
					return caption;
				}
			}

			lock (this.exclusion)
			{
				caption = manager.GetCaption (druid);

				if (caption != null)
				{
					this.cache[key] = new Weak<Caption> (caption);
				}
				else
				{
					this.cache.Remove (key);
				}
			}
			
			return caption;
		}

		#region BigKey Structure

		private struct BigKey : System.IEquatable<BigKey>
		{
			public BigKey(ResourceManager manager, long id)
			{
				this.serial = manager.SerialId;
				this.id     = id;
			}
			
			public override int GetHashCode()
			{
				return ((int) (this.id >> 32)) ^ ((int) (this.id)) ^ ((int) (this.serial));
			}

			public override bool Equals(object obj)
			{
				return this.Equals ((BigKey) obj);
			}

			#region IEquatable<BigKey> Members

			public bool Equals(BigKey other)
			{
				return (this.id == other.id) && (this.serial == other.serial);
			}

			#endregion

			private long serial;
			private long id;
		}

		#endregion


		public static readonly CaptionCache Instance = new CaptionCache ();

		private Dictionary<BigKey, Weak<Caption>> cache = new Dictionary<BigKey, Weak<Caption>> ();
		private object exclusion = new object ();
	}
}
