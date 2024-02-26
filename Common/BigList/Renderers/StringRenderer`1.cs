//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Common.BigList.Renderers
{
    public class StringRenderer<TData> : StringRenderer
    {
        public StringRenderer(System.Func<TData, string> converter)
        {
            this.converter = converter;
        }

        public StringRenderer(System.Func<TData, FormattedText> converter)
        {
            this.converter = x => converter(x).ToSimpleText();
        }

        protected override string GetStringValue(ItemData data)
        {
            return this.converter(data.GetData<TData>());
        }

        private readonly System.Func<TData, string> converter;
    }
}
