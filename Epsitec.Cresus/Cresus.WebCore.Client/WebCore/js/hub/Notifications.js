Ext.require([
],
function () {
    Ext.define('Epsitec.cresus.webcore.hub.Notifications', {
        alternateClassName: ['Epsitec.Notifications'],

        hub: null,
        form: null,

        constructor: function (ToastrFunc, formData) {

            this.form = formData._fields.items;
            var context = this;

            $.getScript('signalr/hubs', function () {
                $.connection.hub.logging = true;
                // Start the connection
                var toastrInstance = new ToastrFunc();
                $.connection.hub.start(function () { toastrInstance.init(context.form[0].lastValue, context); context.initHub(); });

            });
        },

        initHub: function () {
            this.hub = $.connection.notificationHub;
            this.hub.server.setupUserConnection();
        },

        displayErrorInTile: function (tile,headerErrorMessage,fieldName,fieldErrorMessage) {
            
            if (headerErrorMessage)
            {
                tile.items.items[0].showError(headerErrorMessage);
            }
            
            if (fieldName && fieldErrorMessage)
            {
                tile.items.items[0].getForm().findField(fieldName).markInvalid(fieldErrorMessage);
            }
        }

    });
});
