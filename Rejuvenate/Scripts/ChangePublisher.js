function startChangePublisher(hub, publisherIds, hubStart, itemsAdded, itemsRemoved, itemsUpdated) {
    hub.client.itemsAdded = itemsAdded;
    hub.client.itemsRemoved = itemsRemoved;
    hub.client.itemsUpdated = itemsUpdated;

    // Start the connection.
    $.connection.hub.start().done(function () {
        // todo make this safe and use a token so it cant be meddled with
        hub.server.subscribe(publisherIds);
        hubStart();
    });
}