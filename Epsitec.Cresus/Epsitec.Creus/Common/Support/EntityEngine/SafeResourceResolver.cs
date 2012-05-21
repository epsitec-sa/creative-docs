using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;


namespace Epsitec.Common.Support.EntityEngine
{


	public sealed class SafeResourceResolver : IStructuredTypeResolver, ICaptionResolver
	{


		public SafeResourceResolver(IStructuredTypeResolver structuredTypeResolver, ICaptionResolver captionResolver)
		{
			structuredTypeResolver.ThrowIfNull ("structuredTypeResolver");
			captionResolver.ThrowIfNull ("captionResolver");

			this.exclusion = new object ();

			this.structuredTypes = new AutoCache<Druid, StructuredType> (id => this.ComputeStructuredType (structuredTypeResolver, id));
			this.captions = new AutoCache<Druid, Caption> (id => this.ComputeCaption (captionResolver, id));
		}


		#region IStructuredTypeResolver Members

		
		public StructuredType GetStructuredType(Druid typeId)
		{
			typeId.ThrowIf (id => !id.IsValid, "Invalid typeId");

			return this.structuredTypes[typeId];
		}


		#endregion


		#region ICaptionResolver Members

		
		public Caption GetCaption(Druid captionId)
		{
			captionId.ThrowIf (id => !id.IsValid, "Invalid captionId");

			return this.captions[captionId];
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


		private readonly object exclusion;


		private readonly AutoCache<Druid, Caption> captions;


		private readonly AutoCache<Druid, StructuredType> structuredTypes;


		static SafeResourceResolver()
		{
			IStructuredTypeResolver structuredTypeResolver = Resources.DefaultManager;
			ICaptionResolver captionResolver = Resources.DefaultManager;

			instance = new SafeResourceResolver (structuredTypeResolver, captionResolver);
		}


		public static SafeResourceResolver Instance
		{
			get
			{
				return SafeResourceResolver.instance;
			}
		}


		private static readonly SafeResourceResolver instance;


	}


}
