using BoneLib.BoneMenu.Elements;
using LabFusion.BoneMenu;
using LabFusion.Riptide.Utilities;
using LabFusion.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabFusion.Preferences;
using UnityEngine;
using UnityEngine.Playables;
using System.IO;
using BoneLib.BoneMenu;
using RiptideNetworkLayer.Utilities;
using RiptideNetworkLayer.Layer;

namespace RiptideNetworkLayer.BoneMenu
{
    public class ServerListingCategory
    {
        private MenuCategory _category;

        private string _nameToEnter = "";
        private string _codeToEnter = "";
        private ushort _portToEnter = 7777;

        private Keyboard _nameKeyboard;
        private Keyboard _codeKeyboard;
        private Keyboard _portKeyboard;

        private List<MenuCategory> _listingCategories = new List<MenuCategory>();
        
        /// <summary>
        /// Creates a category for Server Listings to be created and managed.
        /// </summary>
        /// <param name="category"></param>
        public static void CreateServerListingCategory(MenuCategory category)
        {
            ServerListingCategory serverListingCategory = new();
            serverListingCategory._category = category.CreateCategory("Server Listings", Color.white);

            var createCategory = serverListingCategory._category.CreateCategory("Create Listing", Color.green);
            
            serverListingCategory._nameKeyboard = Keyboard.CreateKeyboard(createCategory, $"Edit Name:\n{serverListingCategory._nameToEnter}", (name) => serverListingCategory.OnEnterName(name));
            serverListingCategory._codeKeyboard = Keyboard.CreateKeyboard(createCategory, $"Edit Code:\n{serverListingCategory._codeToEnter}", (code) => serverListingCategory.OnEnterCode(code));
            serverListingCategory._portKeyboard = Keyboard.CreateKeyboard(createCategory, $"Edit Port:\n{serverListingCategory._portToEnter}", (port) => serverListingCategory.OnEnterPort(port));

            createCategory.CreateFunctionElement("Done", Color.green, () => serverListingCategory.OnClickDone(true));

            serverListingCategory.CreateServerList(serverListingCategory._category);
        }

        private void OnEnterName(string name)
        {
            _nameToEnter = name;
            _nameKeyboard.Category.SetName($"Edit Name:\n" +
                                           $"{name}");
        }

        private void OnEnterCode(string code)
        {
            _codeToEnter = code;
            _codeKeyboard.Category.SetName($"Edit Code:\n" +
                                           $"{code}");
        }

        private void OnEnterPort(string port)
        {
            if (!ushort.TryParse(port, out ushort result) || result <= 1024 || result >= 65535)
            {
                FusionNotifier.Send(new FusionNotification()
                {
                    isMenuItem = false,
                    isPopup = true,
                    message = "Entered a Port which is incorrect!" +
                              "\nMake SURE to only input numbers and that the port range is between 1024 and 65535",
                    type = NotificationType.ERROR,
                });
                
                return;
            }

            _portToEnter = result;
            _portKeyboard.Category.SetName($"Edit Port:\n" +
                                           $"{result}");
        }

        private void OnClickDone(bool createNewListing)
        {
            ServerListData data = new();
            data.Name = _nameToEnter;
            data.ServerCode = _codeToEnter;
            data.Port = _portToEnter;
            ServerListSaving.SaveServerList(data);

            RefreshListings();

            BoneLib.BoneMenu.MenuManager.SelectCategory(_category);

            _nameToEnter = "";
            _codeToEnter = "";
            _portToEnter = 7777;

            _nameKeyboard.Category.SetName($"Edit Name:\n{_nameToEnter}");
            _codeKeyboard.Category.SetName($"Edit Code:\n{_codeToEnter}");
            _portKeyboard.Category.SetName($"Edit Port:\n{_portToEnter}");
        }

        /// <summary>
        /// Removes all listings and re-creates them.
        /// </summary>
        private void RefreshListings()
        {
            foreach (var category in _listingCategories.ToArray())
            {
                _category.Elements.Remove(category);
                _listingCategories.Remove(category);
            }

            CreateServerList(_category);
        }

        private void OnClickDelete(ServerListData data, MenuCategory category)
        {
            System.IO.File.Delete(data.ServerListDataPath);
            _category.Elements.Remove(category);
            
            BoneLib.BoneMenu.MenuManager.SelectCategory(_category);
        }

        /// <summary>
        /// Writes over a Server Listing with new data.
        /// </summary>
        /// <param name="data"></param>
        private void EditServerListing(ServerListData data)
        {
            var jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data);

            File.WriteAllText(data.ServerListDataPath, jsonData);

            RefreshListings();
        }

        /// <summary>
        /// Creates a list of all the Server Listings in a specified category.
        /// </summary>
        /// <param name="category"></param>
        private void CreateServerList(MenuCategory category)
        {
            foreach (var listing in ServerListSaving.LoadServerList())
            {
                var listingCategory = category.CreateCategory(listing.Name, Color.white);

                var infoPanel = listingCategory.CreateSubPanel("Show Server Info", Color.yellow);
                infoPanel.CreateFunctionElement($"Server Code:\n{listing.ServerCode}", Color.white, null);
                infoPanel.CreateFunctionElement($"Server Port:\n{listing.Port}", Color.white, null);

                var editCategory = listingCategory.CreateCategory("Edit Server Listing", Color.yellow);
                Keyboard.CreateKeyboard(editCategory, "Edit Name", (name) =>
                {
                    listing.Name = name;
                    EditServerListing(listing);
                    MenuManager.SelectCategory(_category);
                });
                Keyboard.CreateKeyboard(editCategory, "Edit Code", (code) =>
                {
                    listing.ServerCode = code;
                    EditServerListing(listing);
                    MenuManager.SelectCategory(_category);
                });
                editCategory.CreateFunctionElement("Delete Listing", Color.red, () => OnClickDelete(listing, listingCategory));

                listingCategory.CreateFunctionElement("Connect to Server", Color.green, () => ClientManagement.P2PJoinServer(listing.ServerCode, listing.Port));

                _listingCategories.Add(listingCategory);
            }
        }
    }
}