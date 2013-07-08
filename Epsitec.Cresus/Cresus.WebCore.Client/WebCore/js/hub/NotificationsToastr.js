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

  this.hub.client.StickyWarningNavToast = function(title, msg, header, field,
      error, datasetId, entityId) {

    var path, message, errorField;

    path = {};
    path.id = entityId;
    path.name = datasetId;

    message = {
      title: title,
      body: msg
    };

    errorField = {
      name: field,
      message: error,
      header: header
    };

    toastr.options = {
      'debug': false,
      'positionClass': 'toast-bottom-full-width',
      'onclick': function() {
        Epsitec.Cresus.Core.app.showEditableEntityWithError(
            path, message, errorField
        );
      },
      'fadeIn': 300,
      'fadeOut': 1000,
      'timeOut': 0,
      'extendedTimeOut': 5000
    };
    toastr.warning(message.body, message.title);
  };

  this.hub.client.Toast = function(title, msg, datasetId, entityId) {
    var path, message;

    path = {};
    path.id = entityId;
    path.name = datasetId;

    message = {
      title: title,
      body: msg
    };

    toastr.options = {
      'debug': false,
      'positionClass': 'toast-bottom-full-width',
      'fadeOut': 1000,
      'timeOut': 5000,
      'extendedTimeOut': 1000
    };
    toastr.info(message.body, message.title);
  };
}
