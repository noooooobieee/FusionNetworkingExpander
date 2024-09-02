using BoneLib.BoneMenu.UI;

namespace FNPlus.Utilities
{
    public class ServerListing
    {
        public string ServerName { get; private set; } = "";
        public string ServerID { get; private set; } = "";

        private static List<ServerListing> _serverListings = new();
        private static string _serverName = "Unnamed";
        private static string _serverID = "";
        public static void CreateServerListingMenu(NetworkLayer layer, Page page, string serverIdName, Action<string> onJoin)
        {
            var createCategory = page.CreatePage("Create Listing", Color.cyan);
            var nameElement = createCategory.CreateString("Server Name", Color.white, _serverName, (value) => _serverName = value);
            var codeElement = createCategory.CreateString(serverIdName, Color.white, _serverID, (value) => _serverID = value);

            page.CreateFunction("Refresh Listings", Color.yellow, () => RefreshServerListingMenu(layer, page, serverIdName, onJoin));

            createCategory.CreateFunction("Add Listing", Color.green, () =>
            {
                _serverListings.Add(new ServerListing(_serverName, _serverID));
                SaveServerListings(layer, _serverListings);

                _serverName = "Unnamed";
                _serverID = "";

                RefreshServerListingMenu(layer, page, serverIdName, onJoin);
            });

            _serverListings = LoadServerListings(layer);
            foreach (var listing in _serverListings)
            {
                CreateServerListing(page, layer, listing, serverIdName, onJoin);
            }
        }

        private static void RefreshServerListingMenu(NetworkLayer layer, Page page, string serverIdName, Action<string> onJoin)
        {
            page.RemoveAll();

            CreateServerListingMenu(layer, page, serverIdName, onJoin);

            Menu.OpenPage(page);
        }

        private static void CreateServerListing(Page page, NetworkLayer layer, ServerListing listing, string serverIdName, Action<string> onJoin)
        {
            var serverListingCategory = page.CreatePage(listing.ServerName, Color.white);
            serverListingCategory.CreateFunction("Join Server", Color.green, () => onJoin(listing.ServerID));

            var editListingCategory = serverListingCategory.CreatePage("Edit Listing", Color.gray);
            editListingCategory.CreateString("Server Name", Color.white, listing.ServerName, (value) => 
            {
                listing.ServerName = value; 
                serverListingCategory.Name = value;
                SaveServerListings(layer, _serverListings);
            });
            editListingCategory.CreateString(serverIdName, Color.white, listing.ServerID, (value) => 
            {
                listing.ServerID = value; 
                SaveServerListings(layer, _serverListings);
            });

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
            List<ServerListing> listings;
            listings = DataSaver.ReadJson<List<ServerListing>>($"FNPlus/{layer.Title}/ServerListings.json");

            if (listings == null)
                listings = new List<ServerListing>();   

            return listings;
        }

        public ServerListing(string serverName, string serverID)
        {
            ServerName = serverName;
            ServerID = serverID;
        }
    }
}
