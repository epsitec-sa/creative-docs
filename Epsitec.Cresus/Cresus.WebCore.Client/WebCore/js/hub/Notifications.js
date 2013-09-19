Ext.require([
],
function() {
  Ext.define('Epsitec.cresus.webcore.hub.Notifications', {
    alternateClassName: ['Epsitec.Notifications'],

    hub: null,

    constructor: function(ToastrFunc, username) {
      var me = this;
      $.getScript('signalr/hubs', function() {
        $.connection.hub.logging = true;
        $.chat({
            // your user information
            user: {
                Id: 1,
                Name: username,
                ProfilePictureUrl: ''
            },
            // text displayed when the other user is typing
            typingText: ' tappe...',
            // the title for the user's list window
            titleText: 'Chat AIDER',
            // text displayed when there's no other users in the room
            emptyRoomText: "There's no one around here. You can still open a session in another browser and chat with yourself :)",
            // the adapter you are using
            adapter: new SignalRAdapter()
        });
        // Start the connection
        var toastrInstance = new ToastrFunc();
        $.connection.hub.start(function() {
          toastrInstance.init(username, me);
          me.initHub(username);
          $.connection.chatJSHub.server.registerMe(username);
        });
      });
    },

    initHub: function() {
      this.hub = $.connection.notificationHub;
      this.hub.server.setupUserConnection();      
    }
  });
});
