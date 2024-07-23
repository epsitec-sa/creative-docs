/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


namespace Epsitec.Common.Types
{
    [SerializationConverter(typeof(ResourceBinding.SerializationConverter))]
    public class ResourceBinding : Binding
    {
        public ResourceBinding()
        {
            this.Mode = BindingMode.OneTime;
        }

        public ResourceBinding(string resourceId)
            : this()
        {
            this.ResourceId = resourceId;
        }

        public string ResourceId
        {
            get { return this.resourceId; }
            set { this.resourceId = value; }
        }

        #region SerializationConverter Class

        public new class SerializationConverter : ISerializationConverter
        {
            #region ISerializationConverter Members

            public string ConvertToString(object value, IContextResolver context)
            {
                ResourceBinding binding = value as ResourceBinding;
                return Serialization.MarkupExtension.ResourceBindingToString(context, binding);
            }

            public object ConvertFromString(string value, IContextResolver context)
            {
                return Serialization.MarkupExtension.ResourceBindingFromString(context, value);
            }

            #endregion
        }

        #endregion

        public delegate void Rebinder(object resourceManager, ResourceBinding binding);

        public static Rebinder RebindCallback
        {
            get { return ResourceBinding.rebindCallback; }
            set
            {
                if (ResourceBinding.rebindCallback != null)
                {
                    throw new System.InvalidOperationException(
                        "RebindCallback cannot be defined twice"
                    );
                }

                ResourceBinding.rebindCallback = value;
            }
        }

        private static Rebinder rebindCallback;
        private string resourceId;
    }
}
