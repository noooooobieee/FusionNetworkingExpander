namespace FNPlus
{
    internal partial class Mod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            NetStandardLoader.Load();
            RiptideNetworkingLoader.Load();

            NetworkLayer.RegisterLayersFromAssembly(Assembly.GetExecutingAssembly());

            LoggerInstance.Msg("FNPlus Initialized!");
        }

        public override void OnLateInitializeMelon()
        {
            PlayerInfo.Initialize();
        }
    }
}
