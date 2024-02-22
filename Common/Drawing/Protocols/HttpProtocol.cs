//	Copyright © 2007-2011, OPaC bright ideas, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Net.Http;

namespace Epsitec.Common.Drawing.Protocols
{
    internal static class HttpProtocol
    {
        public static byte[] ReadBytes(string name)
        {
            using var client = new HttpClient();
            return client.GetByteArrayAsync(string.Concat("http:", name)).Result;
        }
    }
}
