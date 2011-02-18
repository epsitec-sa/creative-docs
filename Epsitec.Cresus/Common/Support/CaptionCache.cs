//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Globalization;
using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>CaptionCache</c> class implements a cache for captions. The cache
	/// associates the caption ID with the resource manager used to allocate the
	/// caption.
	/// </summary>
	public sealed class CaptionCache
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CaptionCache"/> class.
		/// This is a singleton.
		/// </summary>
		private CaptionCache()
		{
			CacheManager.RegisterTrimCache (this, this.TrimCache);
		}


		/// <summary>
		/// Gets the singleton instance of the caption cache.
		/// </summary>
		/// <value>The caption cache instance.</value>
		public static CaptionCache			Instance
		{
			get
			{
				if (CaptionCache.instance == null)
				{
					lock (CaptionCache.exclusion)
					{
						CaptionCache.instance = new CaptionCache ();
					}
				}

				return CaptionCache.instance;
			}
		}

		
		/// <summary>
		/// Gets the caption.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="id">The caption id.</param>
		/// <returns>The caption or <c>null</c>.</returns>
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

		/// <summary>
		/// Gets the caption.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="id">The caption id.</param>
		/// <returns>The caption or <c>null</c>.</returns>
		public Caption GetCaption(ResourceManager manager, Druid id)
		{
			if (id.Type == DruidType.Full)
			{
				return this.GetCaption (manager, id, id.ToLong ());
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the caption.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="id">The caption id.</param>
		/// <returns>The caption or <c>null</c>.</returns>
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

		/// <summary>
		/// Gets the caption.
		/// </summary>
		/// <param name="resolver">The resolver.</param>
		/// <param name="id">The caption id.</param>
		/// <returns>The caption or <c>null</c>.</returns>
		public Caption GetCaption(ICaptionResolver resolver, Druid id)
		{
			if (id.Type == DruidType.Full)
			{
				ResourceManager manager = resolver as ResourceManager;

				if (manager == null)
				{
					//	This does not go through a standard resource manager. Do not
					//	cache the result :
					
					return resolver.GetCaption (id);
				}
				else
				{
					return this.GetCaption (manager, id, id.ToLong ());
				}
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the caption for the specified property.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="property">The property.</param>
		/// <returns>The caption or <c>null</c>.</returns>
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

		/// <summary>
		/// Gets the caption for the specified property of the specified object.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="obj">The dependency object.</param>
		/// <param name="property">The property.</param>
		/// <returns>The caption or <c>null</c>.</returns>
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

		/// <summary>
		/// Gets the property caption for the specified type.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="type">The type.</param>
		/// <param name="property">The property.</param>
		/// <returns>The caption or <c>null</c>.</returns>
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

		/// <summary>
		/// Gets the caption for the specified type object.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="type">The type object.</param>
		/// <returns>The caption or <c>null</c>.</returns>
		public Caption GetTypeCaption(ResourceManager manager, INamedType type)
		{
			return this.GetObjectCaption (manager, type);
		}

		/// <summary>
		/// Gets the caption for the type of the data represented by the specified
		/// property.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="property">The property.</param>
		/// <returns>The caption or <c>null</c>.</returns>
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

		/// <summary>
		/// Gets the caption for the type of the data represented by the specified
		/// property of the specified object.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="obj">The dependency object.</param>
		/// <param name="property">The property.</param>
		/// <returns>The caption or <c>null</c>.</returns>
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

		/// <summary>
		/// Gets the caption for the type of the data represented by the specified
		/// property of the specified type.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="sysType">The type.</param>
		/// <param name="property">The property.</param>
		/// <returns>The caption or <c>null</c>.</returns>
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

		/// <summary>
		/// Gets the caption for the specified named object.
		/// </summary>
		/// <param name="manager">The manager.</param>
		/// <param name="obj">The named object.</param>
		/// <returns>The caption or <c>null</c>.</returns>
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


		/// <summary>
		/// Trims the cache by removing any dead entries.
		/// </summary>
		public void TrimCache()
		{
			lock (CaptionCache.exclusion)
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

		public int DebugGetLiveCaptionsCount()
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



		/// <summary>
		/// Gets the cached caption for the specified resource manager. If it
		/// cannot be found in the cache, it will be retrieved directly from the
		/// resource manager.
		/// </summary>
		/// <param name="manager">The resource manager.</param>
		/// <param name="druid">The DRUID.</param>
		/// <param name="id">The 64-bit id for the DRUID.</param>
		/// <returns>The caption or <c>null</c> if the caption does not exist.</returns>
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

			lock (CaptionCache.exclusion)
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

		/// <summary>
		/// The <c>BigKey</c> structure represents a key vector based on the
		/// resource manager serial id and a DRUID id.
		/// </summary>
		private struct BigKey : System.IEquatable<BigKey>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="BigKey"/> struct.
			/// </summary>
			/// <param name="manager">The resource manager.</param>
			/// <param name="id">The 64-bit id of the DRUID.</param>
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

			private readonly long serial;
			private readonly long id;
		}

		#endregion


		private readonly static object		exclusion	= new object ();
		private static CaptionCache			instance;

		Dictionary<BigKey, Weak<Caption>>	cache		= new Dictionary<BigKey, Weak<Caption>> ();
	}
}
