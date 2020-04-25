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

namespace Z64
{
    public enum Z64VersionEnum
    {
        Unknow,
        MmJapan10,
        MmJapan11,
        MmEurope10,
        MmEurope11,
        MmEurope11Debug,
        MmUSA10,
        MmUSADebug,
        MmUSADemo,

        OotJPUS10,
        OotJPUS11,
        OotEurope10,
        OotJPUS12,
        OotEurope11,
        OotJapanGC,
        OotJapanMq,
        OotUSAGC,
        OotUSAMq,
        OotEuropeMqDbg,
        OotEuropeGC,
        OotEuropeMq,
        OotJapanGcZeldaCollection,
    }

    public enum Z64FileType
    {
        Unknow,
        Code,
        Object,
        Room,
        Scene,
    }



    public static class Z64Version
    {
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

        private class FileEntry
        {
            [JsonConverter(typeof(AddrToStringConverter))]
            public uint? vrom { get; set; }
            public string name { get; set; }
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public Z64FileType type { get; set; }
        }
        private class Z64VersionJson
        {
            public CodeInfo memory { get; set; }
            public List<FileEntry> files { get; set; }
        }
        public class CodeInfo
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


        public static readonly Dictionary<string, Z64VersionEnum> BuildIds = new Dictionary<string, Z64VersionEnum>()
        {
            //source : https://wiki.cloudmodding.com/mm/Build_Dates

            //mm
            { "zelda@srd44 00-03-31 02:22:11", Z64VersionEnum.MmJapan10 },
            { "zelda@srd44 00-04-04 09:34:16", Z64VersionEnum.MmJapan11 },
            { "zelda@srd44 00-07-06 16:46:35", Z64VersionEnum.MmUSADebug },
            { "zelda@srd44 00-07-12 16:14:06", Z64VersionEnum.MmUSADemo },
            { "zelda@srd44 00-07-31 17:04:16", Z64VersionEnum.MmUSA10 },
            { "zelda@srd44 00-09-25 11:16:53", Z64VersionEnum.MmEurope10 },
            { "zelda@srd44 00-09-29 09:29:05", Z64VersionEnum.MmEurope11Debug },
            { "zelda@srd44 00-09-29 09:29:41", Z64VersionEnum.MmEurope11 },
            //oot
            { "zelda@srd44 98-10-21 04:56:31", Z64VersionEnum.OotJPUS10 },
            { "zelda@srd44 98-10-26 10:58:45", Z64VersionEnum.OotJPUS11 },
            { "zelda@srd44 98-11-10 14:34:22", Z64VersionEnum.OotEurope10 },
            { "zelda@srd44 98-11-12 18:17:03", Z64VersionEnum.OotJPUS12 },
            { "zelda@srd44 98-11-18 17:36:49", Z64VersionEnum.OotEurope11 },
            { "zelda@srd022j   02-10-29 23:49:53", Z64VersionEnum.OotJapanGC },
            { "zelda@srd022j   02-10-30 00:15:15", Z64VersionEnum.OotJapanMq },
            { "zelda@srd022j   02-12-19 13:28:09", Z64VersionEnum.OotUSAGC },
            { "zelda@srd022j   02-12-19 14:05:42", Z64VersionEnum.OotUSAMq },
            { "zelda@srd022j   03-02-21 00:16:31", Z64VersionEnum.OotEuropeMqDbg },
            { "zelda@srd022j   03-02-21 20:12:23", Z64VersionEnum.OotEuropeGC },
            { "zelda@srd022j   03-02-21 20:37:19", Z64VersionEnum.OotEuropeMq },
            { "zelda@srd022j   03-10-08 21:53:00", Z64VersionEnum.OotJapanGcZeldaCollection },
        };
        
        public static Dictionary<Z64VersionEnum, Dictionary<int, Tuple<string, Z64FileType>>> FileTable { get; private set; }
        public static Dictionary<Z64VersionEnum, CodeInfo> CodeInfos { get; private set; }

        public static void LoadRessources()
        {
            FileTable = new Dictionary<Z64VersionEnum, Dictionary<int, Tuple<string, Z64FileType>>>();
            CodeInfos = new Dictionary<Z64VersionEnum, CodeInfo>();

            var versions = Enum.GetValues(typeof(Z64VersionEnum)).Cast<Z64VersionEnum>().ToList();
            foreach (var v in versions)
            {
                if (v == Z64VersionEnum.Unknow) continue;

                string path = $"versions/{v}.json";

                if (File.Exists(path))
                {
                    var dict = new Dictionary<int, Tuple<string, Z64FileType>>();
                    string json = File.ReadAllText(path);

                    Z64VersionJson ver = JsonSerializer.Deserialize<Z64VersionJson>(json, new JsonSerializerOptions() { IgnoreNullValues=true,});

                    // memory
                    CodeInfos.Add(v, ver.memory);

                    // files
                    foreach (var file in ver.files)
                    {
                        if (!file.vrom.HasValue)
                            throw new Exception("Invalid vrom");
                        dict.Add((int)file.vrom, new Tuple<string, Z64FileType>(file.name, file.type));
                    }

                    FileTable.Add(v, dict);
                }
            }
        }

        public static bool ContainsConfig(Z64VersionEnum v)
        {
            return File.Exists($"versions/{v}.json");
        }
    }
}
