using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Buffers;
using System.Buffers.Text;
using N64;

namespace Z64
{
    public enum Z64FileType
    {
        Unknow,
        Code,
        Object,
        Room,
        Scene,
    }

    public enum Z64GameType
    {
        Unknow,
        Oot,
        Mm,
    }


    public class Z64Version
    {
        #region JSON Data Type

        private class AddrToStringConverter : JsonConverter<uint?>
        {
            
            public override uint? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    if (uint.TryParse(reader.GetString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint number))
                        return number;
                }
                if (reader.TokenType == JsonTokenType.Null)
                    return null;

                return reader.GetUInt32();
            }
            public override void Write(Utf8JsonWriter writer, uint? v, JsonSerializerOptions options)
            {
                writer.WriteStringValue(v.HasValue? v.Value.ToString("X8") : "null");
            }
        }

        public class VersionIdentifier
        {
            [JsonPropertyName("build_team")]
            public string BuildTeam { get; set; }

            [JsonPropertyName("build_date")]
            public string BuildDate { get; set; }

            [JsonPropertyName("rom_name")]
            public string RomName { get; set; }

            [JsonPropertyName("rom_code")]
            public string RomCode { get; set; }

            [JsonPropertyName("rom_version")]
            public byte? RomVersion { get; set; }

        }
        public class MemoryInfo
        {
            [JsonPropertyName("code")]
            [JsonConverter(typeof(AddrToStringConverter))]
            public uint? CodeVram { get; set; }
            [JsonPropertyName("actor_table")]
            [JsonConverter(typeof(AddrToStringConverter))]
            public uint? ActorTable { get; set; }
            [JsonPropertyName("gamestate_table")]
            [JsonConverter(typeof(AddrToStringConverter))]
            public uint? GameStateTable { get; set; }
            [JsonPropertyName("effect_table")]
            [JsonConverter(typeof(AddrToStringConverter))]
            public uint? EffectTable { get; set; }
            [JsonPropertyName("kaleido_mgr_table")]
            [JsonConverter(typeof(AddrToStringConverter))]
            public uint? KaleidoMgrTable { get; set; }
            [JsonPropertyName("map_mark_data_table")]
            [JsonConverter(typeof(AddrToStringConverter))]
            public uint? MapMarkDataOvl { get; set; } // specific to oot
            [JsonPropertyName("fbdemo_table")]
            [JsonConverter(typeof(AddrToStringConverter))]
            public uint? FBDemoTable { get; set; } // specific to mm
        }
        public class FileEntry
        {
            [JsonConverter(typeof(AddrToStringConverter))]
            [JsonPropertyName("vrom")]
            public uint? Vrom { get; set; }
            [JsonPropertyName("name")]
            public string Fileame { get; set; }
            [JsonConverter(typeof(JsonStringEnumConverter))]
            [JsonPropertyName("type")]
            public Z64FileType FileType { get; set; }
        }

        #endregion JSON Data Type
       


        [JsonPropertyName("version_name")]
        public string VersionName { get; set; }
        [JsonPropertyName("version_game")]
        public Z64GameType Game { get; set; }

        [JsonPropertyName("identifier")]
        public VersionIdentifier Identifier { get; set; }
        [JsonPropertyName("memory")]
        public MemoryInfo Memory { get; set; }
        [JsonPropertyName("files")]
        public List<FileEntry> Files { get; set; }


        public int? GetVrom(string filename) => (int)Files.Find(f => f.Fileame == filename).Vrom;
        public string GetFileName(int vrom) => Files.Find(f => f.Vrom == vrom).Fileame;
        public Z64FileType GetFileType(int vrom) => Files.Find(f => f.Vrom == vrom).FileType;

        private int FindBuildTeam(N64Rom rom)
        {
            string team = Identifier.BuildTeam;
            int start = 0x1000;
            int end = 0x20000 - team.Length - Identifier.BuildDate.Length;

            for (int i = start; i < end; i += 4)
            {
                int count = 0;
                while (count < team.Length && i + count < end && rom.RawRom[i + count] == team[count])
                    count++;

                if (count >= team.Length)
                    return i;
            }
            return -1;
        }
        public bool Match(N64Rom rom, out int fileTableOff)
        {
            fileTableOff = 0;

            // search team
            int teamOff = FindBuildTeam(rom);
            if (teamOff < 0)
                return false;

            int dateOff = (teamOff + Identifier.BuildTeam.Length) + 3 & ~3;

            // check date
            string date = Identifier.BuildDate;
            int count = 0;
            while (dateOff + count < rom.RawRom.Length && count < date.Length && date[count] == rom.RawRom[dateOff + count++])
                count++;

            if (count < date.Length)
                return false;

            fileTableOff = teamOff + 0x30;
            return true;
        }

        public static List<Z64Version> Versions { get; private set; }
        
        public static void LoadResources()
        {
            var files = Directory.GetFiles("versions", "*.json");

            Versions = new List<Z64Version>();
            var options = new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                PropertyNameCaseInsensitive = true,
            };
            options.Converters.Add(new JsonStringEnumConverter());

            foreach (var file in files)
            {
                string json = File.ReadAllText(file);
                var ver = JsonSerializer.Deserialize<Z64Version>(json, options);
                if (ver.Identifier == null || ver.Identifier.BuildDate == null || ver.Identifier.BuildTeam == null)
                    throw new InvalidDataException($"Error loading \"{file}\": identifier.build_date or identifier.build_team missing");
                Versions.Add(ver);
            }


        }

        public static Z64Version IdentifyRom(N64Rom rom, out int fileTableOff)
        {
            foreach (var ver in Versions)
            {
                if (ver.Match(rom, out fileTableOff))
                {
                    return ver;
                }
            }

            fileTableOff = 0;
            return null;
        }
    }
}
