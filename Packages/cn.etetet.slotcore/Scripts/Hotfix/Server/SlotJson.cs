using System.Text.Json;

namespace ET.Server
{
    public static class SlotJson
    {
        public static JsonSerializerOptions Options()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.IncludeFields = true;
            return options;
        }
    }
}
