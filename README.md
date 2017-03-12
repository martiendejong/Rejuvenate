# Rejuvenate

To setup this library in your ASP.NET MVC project, do the following:

Include SignalR.

Create a DbContext that inherits from RejuvenatingDbContext and declare your entities as normal. 
Then implement the following methods:
Replace the MyType and MyTypeDbSet with your entity class.


        public IRejuvenatingQueryable<MyType> RejuvenatingGames
        {
            get
            {
                return new RejuvenatingQueryable<MyType>(MyTypeDbSet, this);
            }
        }

        // declare the polling executors for the change aware entities
        override protected List<IEntityRejuvenator> EntityRejuvenators
        {
            get
            {
                return new List<IEntityRejuvenator>
                {
                    GetRejuvenator<MyType>(),
                };
            }
        }


Create a Hub that inherits from RejuvenatingHub.

Create a script in your View and adjust as needed:

<script type="text/javascript">

        var myHub;

        function hostGame()
        {
            myHub.server.hostGame();
        }

        $(function () {
            // Declare a proxy to reference the hub.
            myHub = $.connection.exampleHub;

            myHub.client.itemsAdded = function (type, rejuvenatorId, items)
            {
                items.forEach(function (item, index)
                {
                    var html = "<p id='item" + item.Id + "'>" + item.Host + " (" + item.Players.length + ")</p>";
                    $("#items").append(html);
                });
            }

            myHub.client.itemsRemoved = function (type, rejuvenatorId, items) {
                items.forEach(function (item, index)
                {
                    var selector = "#item" + item.Id;
                    $(selector).remove();
                });
            }

            myHub.client.itemsUpdated = function (type, rejuvenatorId, items) {
                items.forEach(function (item, index)
                {
                    var selector = "#item" + item.Id;
                    $(selector).text(item.Name);
                });
            }

            // Start the connection.
            $.connection.hub.start().done(function () {
                // todo make this safe and use a token so it cant be meddled with
                myHub.server.registerRejuvenatingClient([@ViewBag.rejuvenatorId]);
            });
        });

    </script>


In your controller call the RejuvenatingQueryable as follows:

            var rejuvenatingQuery = DbContext.RejuvenatingGames;
            var rejuvenator = rejuvenatingQuery.RejuvenateQuery<ExampleHub>();
            ViewBag.RejuvenatorId = rejuvenator.Id;
            var result = rejuvenatingQuery.AsQueryable().ToList();
            return View(result);