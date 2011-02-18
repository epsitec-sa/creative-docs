//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[SerializationConverter (typeof (ResourceBinding.SerializationConverter))]
	public class ResourceBinding : Binding
	{
		public ResourceBinding()
		{
			this.Mode = BindingMode.OneTime;
		}
		
		public ResourceBinding(string resourceId) : this ()
		{
			this.ResourceId = resourceId;
		}
		
		public string ResourceId
		{
			get
			{
				return this.resourceId;
			}
			set
			{
				this.resourceId = value;
			}
		}
		
		#region SerializationConverter Class

		public new class SerializationConverter : ISerializationConverter
		{
			#region ISerializationConverter Members

			public string ConvertToString(object value, IContextResolver context)
			{
				ResourceBinding binding = value as ResourceBinding;
				return Serialization.MarkupExtension.ResourceBindingToString (context, binding);
			}

			public object ConvertFromString(string value, IContextResolver context)
			{
				return Serialization.MarkupExtension.ResourceBindingFromString (context, value);
			}

			#endregion
		}

		#endregion

		public delegate void Rebinder(object resourceManager, ResourceBinding binding);
		
		public static Rebinder RebindCallback
		{
			get
			{
				return ResourceBinding.rebindCallback;
			}
			set
			{
				if (ResourceBinding.rebindCallback != null)
				{
					throw new System.InvalidOperationException ("RebindCallback cannot be defined twice");
				}

				ResourceBinding.rebindCallback = value;
			}
		}

		private static Rebinder rebindCallback;
		private string resourceId;
	}
}
