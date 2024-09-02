namespace FNPlus.Utilities
{
    public class LayerDataSaving
    {
        public static void SaveLayerData(NetworkLayer layer, Dictionary<string, object> metaData)
        {
            LayerData layerData = new LayerData(layer, metaData);

            DataSaver.WriteJson($"FNPlus/{layer.Title}/LayerData.json", layerData);
        }

        public static LayerData LoadLayerData(NetworkLayer layer)
        {
            return DataSaver.ReadJson<LayerData>($"FNPlus/{layer.Title}/LayerData.json");
        }

        public class LayerData(NetworkLayer layer, Dictionary<string, object> MetaData)
        {
            public NetworkLayer Layer { get; } = layer;
            public Dictionary<string, object> MetaData { get; } = MetaData;
        }
    }
}
