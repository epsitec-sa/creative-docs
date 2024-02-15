//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Factories
{
    public struct ViewId
    {
        public ViewId(int? id, string arg = default)
        {
            this.Id = id;
            this.Arg = arg;
        }

        public int? Id { get; }
        public string Arg { get; }

        public bool HasValue => this.Id.HasValue;

        public override string ToString()
        {
            if (this.Id == null)
            {
                return "null";
            }

            if (string.IsNullOrEmpty (this.Arg))
            {
                return InvariantConverter.ToString (this.Id.Value);
            }
            else
            {
                return InvariantConverter.ToString (this.Id.Value) + " " + this.Arg;
            }
        }

        public static readonly ViewId Empty = new ViewId (null, null);
    }
}
