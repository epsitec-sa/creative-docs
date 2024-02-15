function ElasticSearch() {
  this.hub = $.connection.elasticSearchHub;

  //Initialize
  this.init = function(con) {
    this.hub = con.elasticSearchHub;    
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
          htmlMenu += "<li id='"+id+"'><a href='#' onclick='" +
                      "var target = document.getElementById(\""+id+"\");" +
                      "var spinner = new Spinner().spin(target);" + 
                      "var path = { " +
                      "databaseName: \"" + doc.DatasetId + "\"," + 
                      "entityType: \"Contacts\"," + 
                      "entityId: \"" + doc.EntityId + "\"};" + 
                      "Epsitec.Cresus.Core.app.showEntity(path,Epsitec.Callback.create(function(){spinner.stop()},this));'>" + 
                      doc.Name + 
                      "<p>"+ doc.Text +"</p></a></li>";
          id++;
        });

        htmlMenu += "<li><a href='#' onclick='$.sidr(\"close\",\"sidr\");'>Fermer le volet</a></li></ul>";

        return htmlMenu;
      }
    });

    $.sidr('open','sidr');
  };

  this.query = function(q) {
    this.hub.server.query(q);
  };
}