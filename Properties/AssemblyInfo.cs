using System.Reflection;
using RiptideNetworkLayer;
using MelonLoader;

[assembly: AssemblyTitle(RiptideNetworkLayer.Main.Description)]
[assembly: AssemblyDescription(RiptideNetworkLayer.Main.Description)]
[assembly: AssemblyCompany(RiptideNetworkLayer.Main.Company)]
[assembly: AssemblyProduct(RiptideNetworkLayer.Main.Name)]
[assembly: AssemblyCopyright("Developed by " + RiptideNetworkLayer.Main.Author)]
[assembly: AssemblyTrademark(RiptideNetworkLayer.Main.Company)]
[assembly: AssemblyVersion(RiptideNetworkLayer.Main.Version)]
[assembly: AssemblyFileVersion(RiptideNetworkLayer.Main.Version)]
[assembly: MelonInfo(typeof(RiptideNetworkLayer.Main), RiptideNetworkLayer.Main.Name, RiptideNetworkLayer.Main.Version, RiptideNetworkLayer.Main.Author, RiptideNetworkLayer.Main.DownloadLink)]
[assembly: MelonColor(System.ConsoleColor.White)]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONELAB")]
[assembly: MelonOptionalDependencies("RiptideNetworking", "netstandard")]
[assembly: MelonAdditionalDependencies("LabFusion")]