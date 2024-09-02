namespace FNPlus.Utilities
{
    public class ServerListing
    {
        public string ServerName { get; private set; } = "";
        public string ServerCode { get; private set; } = "";

        private static List<ServerListing> _serverListings = new();
        private static string _serverName = "Unnamed";
        private static string _serverCode = "";
        public static void CreateServerListingMenu(NetworkLayer layer, Page page, string serverIdName, Action<string> onJoin)
        {
            var createCategory = page.CreatePage("Create Listing", Color.gray);
            var nameElement = createCategory.CreateString("Server Name", Color.white, _serverName, (value) => _serverName = value);
            var codeElement = createCategory.CreateString(serverIdName, Color.white, _serverCode, (value) => _serverCode = value);

            page.CreateFunction("Refresh Listings", Color.yellow, () => RefreshServerListingMenu(layer, page, serverIdName, onJoin));

            createCategory.CreateFunction("Create Listing", Color.white, () =>
            {
                _serverListings.Add(new ServerListing(_serverName, _serverCode));
                SaveServerListings(layer, _serverListings);

                _serverName = "Unnamed";
                _serverCode = "";

                RefreshServerListingMenu(layer, page, serverIdName, onJoin);
            });
        }

        private static void RefreshServerListingMenu(NetworkLayer layer, Page page, string serverIdName, Action<string> onJoin)
        {
            page.RemoveAll();

            _serverListings = LoadServerListings(layer);

            CreateServerListingMenu(layer, page, serverIdName, onJoin);

            foreach (var listing in _serverListings)
            {
                CreateServerListing(page, layer, listing, serverIdName, onJoin);
            }

            Menu.OpenPage(page);
        }

        private static void CreateServerListing(Page page, NetworkLayer layer, ServerListing listing, string serverIdName, Action<string> onJoin)
        {
            var serverListingCategory = page.CreatePage(listing.ServerName, Color.white);
            serverListingCategory.CreateFunction("Join Server", Color.white, () => onJoin(listing.ServerCode));
            serverListingCategory.CreateFunction("Remove Listing", Color.red, () =>
            {
                _serverListings.Remove(listing);
                SaveServerListings(layer, _serverListings);

                RefreshServerListingMenu(layer, page, serverIdName, onJoin);
            });
        }

        public static void SaveServerListings(NetworkLayer layer, List<ServerListing> serverListings)
        {
            DataSaver.WriteJson($"FNPlus/{layer.Title}/ServerListings.json", serverListings);
        }

        public static List<ServerListing> LoadServerListings(NetworkLayer layer)
        {
            return DataSaver.ReadJson<List<ServerListing>>($"FNPlus/{layer.Title}/ServerListings.json");
        }

        public ServerListing(string serverName, string serverCode)
        {
            ServerName = serverName;
            ServerCode = serverCode;
        }
    }
}
