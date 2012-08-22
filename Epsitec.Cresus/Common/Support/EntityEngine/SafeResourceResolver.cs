//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;


namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>SafeResourceResolver</c> class provides a thread-safe access to captions and
	/// structured types
	/// </summary>
	public sealed class SafeResourceResolver : IStructuredTypeResolver, ICaptionResolver, System.IDisposable
	{
		public SafeResourceResolver(IStructuredTypeResolver structuredTypeResolver, ICaptionResolver captionResolver)
		{
			structuredTypeResolver.ThrowIfNull ("structuredTypeResolver");
			captionResolver.ThrowIfNull ("captionResolver");

			this.exclusion = new object ();

			this.structuredTypes = new AutoCache<Druid, StructuredType> (id => this.ComputeStructuredType (structuredTypeResolver, id));
			this.captions = new AutoCache<Druid, Caption> (id => this.ComputeCaption (captionResolver, id));
		}

		
		public static SafeResourceResolver		Instance
		{
			get
			{
				return SafeResourceResolver.instance;
			}
		}


		/// <summary>
		/// Clears the caches. This should be called whenever the user interface is
		/// switched from one language to another.
		/// </summary>
		public void ClearCaches()
		{
			this.captions.Clear ();
			this.structuredTypes.Clear ();
		}

		#region IStructuredTypeResolver Members

		
		public StructuredType GetStructuredType(Druid typeId)
		{
			typeId.ThrowIf (id => id.IsEmpty, "Invalid typeId");

			return this.structuredTypes[typeId];
		}


		#endregion

		#region ICaptionResolver Members

		
		public Caption GetCaption(Druid captionId)
		{
			captionId.ThrowIf (id => id.IsEmpty, "Invalid captionId");

			return this.captions[captionId];
		}


		#endregion

		#region IDisposable Members


		public void Dispose()
		{
			this.captions.Dispose ();
			this.structuredTypes.Dispose ();
		}


		#endregion
		

		private Caption ComputeCaption(ICaptionResolver resolver, Druid captionId)
		{
			lock (this.exclusion)
			{
				return resolver.GetCaption (captionId);
			}
		}

		private StructuredType ComputeStructuredType(IStructuredTypeResolver resolver, Druid typeId)
		{
			lock (this.exclusion)
			{
				return resolver.GetStructuredType (typeId);
			}
		}


		static SafeResourceResolver()
		{
			IStructuredTypeResolver structuredTypeResolver = Resources.DefaultManager;
			ICaptionResolver captionResolver = Resources.DefaultManager;

			instance = new SafeResourceResolver (structuredTypeResolver, captionResolver);
		}




		private static readonly SafeResourceResolver		instance;
		
		private readonly object								exclusion;
		private readonly AutoCache<Druid, Caption>			captions;
		private readonly AutoCache<Druid, StructuredType>	structuredTypes;
	}
}
