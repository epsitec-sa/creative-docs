Ext.define('Epsitec.cresus.webcore.hub.Notifications', {
    alternateClassName: ['Epsitec.Notifications'],

    client: null,

    constructor: function (ToastrFunc) {

        var context = this;

        $.getScript('signalr/hubs', function () {
            $.connection.hub.logging = false;
            // Start the connection
            var toastrInstance = new ToastrFunc();
            
            $.connection.hub.start(function () { toastrInstance.init(); context.initClient(); });

        });
    },

    initClient: function () {
        this.client = $.connection.notificationHub;
    }
});
