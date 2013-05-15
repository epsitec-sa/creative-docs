﻿///AIDER NotificationHub


var signalRClient = {};

Ext.onReady(function () {

    function Toastr() {

        this.hub = $.connection.notificationHub;
        this.app = Epsitec.Cresus.Core.getApplication();

        //Initialize
        this.init = function () {
            signalRClient.connectionId = this.hub.connection.id;

            //add field to login panel
            this.app.loginPanel.addConnectionIdField(signalRClient.connectionId);
        }

        //Test Hub
        this.WarningToastTo = function (connectionId, title, message, datasetId, entityId) {
            this.hub.server.warningToast(connectionId, title, message, datasetId, entityId);

        }

        //Handlers for our Hub callbacks
        this.hub.client.StickyWarningNavToast = function (title, msg, datasetId, entityId) {
            var path = {};
            path.id = entityId;
            path.name = datasetId;

            toastr.options = {
                "debug": false,
                "positionClass": "toast-bottom-full-width",
                "onclick": function () {
                    this.app.showEditableEntity(path);
                },
                "fadeIn": 300,
                "fadeOut": 1000,
                "timeOut": 0
            };
            toastr.warning(msg, title);
        }

        this.hub.client.Toast = function (title, msg, datasetId, entityId) {
            var path = {};
            path.id = entityId;
            path.name = datasetId;

            toastr.options = {
                "debug": false,
                "positionClass": "toast-bottom-full-width",
                "fadeOut": 1000,
                "timeOut": 5000,
                "extendedTimeOut": 1000
            };
            toastr.info(msg, title);
        }

    };

    if (epsitecConfig.featureNotifications) {
        $.getScript('signalr/hubs', function () {

            $.connection.hub.logging = false;

            signalRClient.instance = new Toastr();
            // Start the connection
            $.connection.hub.start(function () { signalRClient.instance.init(); });
        });
    }


});