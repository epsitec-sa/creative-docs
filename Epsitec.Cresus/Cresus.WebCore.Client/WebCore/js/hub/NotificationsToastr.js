function NotificationsToastr() {
  this.hub = $.connection.notificationHub;
  var notificationsClient = null;

  //Initialize
  this.init = function(con,username, client) {   
    this.hub = con.notificationHub;
    this.hub.state.connectionId = this.hub.connection.id;
    this.hub.state.userName = username;
    notificationsClient = client;
    this.hub.server.setupUserConnection();
  };

  //Entry points for calling hub
  this.hub.client.Toast = function(title, message) {
      toastr.options = {
        'debug': false,
        'positionClass': 'toast-bottom-full-width',
        'fadeOut': 1000,
        'timeOut': 5000,
        'extendedTimeOut': 1000
      };
      toastr.info(message, title);
  };

  this.hub.client.StickyWarningNavToast = function(title, message, headerMsg,
      field, errorMsg, datasetId, entityId) {

    var path, error;

    path = {
      databaseName: datasetId,
      entityId: entityId
    };

    error = {
      tileMessage: headerMsg,
      fieldMessage: errorMsg,
      fieldName: field
    };

    toastr.options = {
      debug: false,
      positionClass: 'toast-bottom-full-width',
      fadeIn: 300,
      fadeOut: 1000,
      timeOut: 0,
      extendedTimeOut: 5000
    };

    if(path.entityId!="-")
    {
      toastr.options.onclick = function() {
        Epsitec.Cresus.Core.app.showEditableEntityWithError(path, error);
      };
    }

    toastr.warning(message, title);
  };

  this.WarningToastTo = function(cId, title, message, datasetId, entityId) {
    this.hub.server.warningToast(cId, title, message, datasetId, entityId);
  };
  
}
