using Newtonsoft.Json;

namespace NameThatBeaver
{
    public class NameThatBeaverSettings
    {
        public string NamesListLocation { get; set; } = string.Empty;

        public bool ReuseNames { get; set; } = false;

        public int RedownloadListAfter { get; set; } = 0;

        public bool RefreshNamePoolOnFileChange { get; set; } = false;

        [JsonIgnore]
        public bool NamesListIsRemote { get; set; }

        [JsonIgnore]
        public string BeaversNamesFileLocation { get; set; } = string.Empty;
    }
}