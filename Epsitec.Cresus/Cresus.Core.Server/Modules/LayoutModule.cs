//using System.Linq;
//using Epsitec.Cresus.Core.Entities;
//using Nancy;


//namespace Epsitec.Cresus.Core.Server.Modules
//{
//    public class LayoutModule : CoreModule
//    {
//        public LayoutModule(): base("/layout")
//        {
//            Get["/person/{id}"] = parameters =>
//            {
//                var coreSession = CoreModule.GetCoreSession ();
//                var context = coreSession.GetBusinessContext ();

//                var customer = (from x in context.GetAllEntities<CustomerEntity> ()
//                                where x.GetEntitySerialId () == parameters.id
//                                select x).FirstOrDefault ();

//                if (customer == null)
//                {
//                    return new NotFoundResponse ();
//                }

//                var s = PanelBuilder.BuildController (customer, Controllers.ViewControllerMode.Summary);

//                return Response.AsJson (s);
//            };

//            Get["/affair/{id}"] = parameters =>
//            {
//                var coreSession = CoreModule.GetCoreSession ();
//                var context = coreSession.GetBusinessContext ();

//                var affair = (from x in context.GetAllEntities<AffairEntity> ()
//                              where x.GetEntitySerialId () == parameters.id
//                              select x).FirstOrDefault ();

//                if (affair == null)
//                {
//                    return new NotFoundResponse ();
//                }

//                var s = PanelBuilder.BuildController (affair, Controllers.ViewControllerMode.Summary);

//                return Response.AsJson (s);
//            };
//        }
//    }
//}
