Ext.define('Epsitec.cresus.webcore.hub.Notifications', {
    alternateClassName: ['Epsitec.Notifications'],

    client: null,

    constructor: function (toastrFunc) {

        var context = this;

        $.getScript('signalr/hubs', function () {
            $.connection.hub.logging = false;
            // Start the connection
            var toastrInstance = new toastrFunc();
            
            $.connection.hub.start(function () { toastrInstance.init(); context.initClient() });

        });
    },

    initClient: function () {
        this.client = $.connection.notificationHub;
    }
});
