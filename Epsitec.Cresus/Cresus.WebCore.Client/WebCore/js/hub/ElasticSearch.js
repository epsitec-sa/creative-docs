function ElasticSearch() {
  this.hub = $.connection.elasticSearchHub;

  //Initialize
  this.init = function(con) {
    this.hub = con.elasticSearchHub;
    //this.hub.state.connectionId = this.hub.connection.id;
    
  };

  //Entry points for calling hub
  this.hub.client.processResult = function(result) {
    $('#elasticsearch').sidr({
      name: 'sidr',
      side: 'left',
      body: '#columnmanager',
      source: function(name) {
        var htmlMenu = "<h1>Résultat de la recherche</h1><ul>";
        var id = 0;


        Ext.Array.each(result.Documents, function (doc)
        {       

         // Generate or pull any HTML you want for the back.
        
          htmlMenu += "<li id='"+id+"'><a href='#' onclick='" +
                      "var target = document.getElementById(\""+id+"\");" +
                      "var spinner = new Spinner().spin(target);" + 
                      "var path = { " +
                      "databaseName: \"" + doc.DatasetId + "\"," + 
                      "entityId: \"" + doc.EntityId + "\"};" + 
                      "Epsitec.Cresus.Core.app.showEntity(path,Epsitec.Callback.create(function(){spinner.stop()},this));'>" + 
                      doc.Name + 
                      "<p>"+ doc.Text +"</p></a></li>";
          id++;
        });

        htmlMenu += "<li><a href='#' onclick='$.sidr(\"close\",\"sidr\");'>fermer</a></li></ul>";

        return htmlMenu;
      }
    });

    $.sidr('open','sidr');

    /*Ext.Array.each(result.Documents, function (doc)
    {

      var path = {
          databaseName: doc.DatasetId,
          entityId: doc.EntityId
        };

      toastr.options = null;
      toastr.options = {
          debug: false,
          positionClass: 'toast-top-full-width',
          fadeOut: 1000,
          timeOut: 5000,
          extendedTimeOut: 1000
      };

      toastr.options.onclick = function() {
        Epsitec.Cresus.Core.app.showEntity(path, null);
      };

      toastr.info(doc.Text, doc.Name);
    });*/
      
  };

  this.query = function(q) {
    this.hub.server.query(q);
  };
}