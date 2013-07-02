Ext.require([
],
function() {
  Ext.define('Epsitec.cresus.webcore.hub.Notifications', {
    alternateClassName: ['Epsitec.Notifications'],

    hub: null,

    constructor: function(ToastrFunc, formData) {
      var me = this;

      $.getScript('signalr/hubs', function() {
        $.connection.hub.logging = true;
        // Start the connection
        var toastrInstance = new ToastrFunc();
        $.connection.hub.start(function() {
          toastrInstance.init(formData.getFieldValues().username, me);
          me.initHub();
        });
      });
    },

    initHub: function() {
      this.hub = $.connection.notificationHub;
      this.hub.server.setupUserConnection();
    },

    displayErrorInTile: function(tile, headerMsg, fieldName, fieldMsg) {
      if (headerMsg) {
        tile.showError(headerMsg);
      }

      if (fieldName && fieldMsg) {
        var invalidField = tile.getForm().findField(fieldName);
        if (invalidField) {
          invalidField.markInvalid(fieldMsg);
          invalidField.focus();
        }
      }
    }
  });
});
