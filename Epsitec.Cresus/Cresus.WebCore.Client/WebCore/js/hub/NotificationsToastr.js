function NotificationsToastr() {
  this.hub = $.connection.notificationHub;
  var notificationsClient = null;

  //Initialize
  this.init = function(username, client) {
    this.hub.state.connectionId = this.hub.connection.id;
    this.hub.state.userName = username;
    notificationsClient = client;
  };

  //Entry points for calling hub

  this.WarningToastTo = function(cId, title, message, datasetId, entityId) {
    this.hub.server.warningToast(cId, title, message, datasetId, entityId);
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
      'debug': false,
      'positionClass': 'toast-bottom-full-width',
      'onclick': function() {
        Epsitec.Cresus.Core.app.showEditableEntityWithError(path, error);
      },
      'fadeIn': 300,
      'fadeOut': 1000,
      'timeOut': 0,
      'extendedTimeOut': 5000
    };
    toastr.warning(message, title);
  };

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
}
