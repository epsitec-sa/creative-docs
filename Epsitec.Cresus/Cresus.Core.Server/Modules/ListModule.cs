//using System.Collections.Generic;
//using System.Linq;
//using Epsitec.Common.Support.Extensions;
//using Epsitec.Cresus.Core.Entities;
//using Nancy;

//namespace Epsitec.Cresus.Core.Server.Modules
//{
//    public class ListModule : CoreModule
//    {

//        protected ListModule() : base ("/list")
//        {
//            Get["/persons"] = parameters =>
//            {
//                var coreSession = GetCoreSession ();
//                var context = coreSession.GetBusinessContext ();

//                var customers = from x in context.GetAllEntities<CustomerEntity> ()
//                                select x;

//                var obj = new List<object> ();

//                customers.ForEach (c => obj.Add (new
//                {
//                    firstName = c.IdA,
//                    lastName = c.IdB,
//                    uniqueId = c.GetEntitySerialId ()
//                }));

//                var res = Response.AsJson (obj);

//                return res;

//            };
//        }
//    }
//}
