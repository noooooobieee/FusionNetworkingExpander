namespace FNExtender
{
    internal partial class Mod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            NetworkLayer.RegisterLayersFromAssembly(Assembly.GetExecutingAssembly());
        }

        public override void OnLateInitializeMelon()
        {
            Utilities.PlayerInfo.Initialize();
        }
    }
}
