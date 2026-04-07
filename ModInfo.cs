using AliLib.Core;
using System;

namespace QuickHack;

public static class ModInfo
{
    public const string Name = "QuickHack";
    public const string Description = "Cyberpunk Quickhack.";
    public const string ModVersion = "1.0";

    public const string Author = "Allieum256";

    [ExportedString("manifest.json")]
    public const string QUICKHACK_MANIFEST =
$@"{{
    ""Name"": ""{Name}"",
    ""Description"": ""{Description}"",
    ""Author"": ""{Author}"",
    ""Version"": ""{ModVersion}"",
    ""GameVersion"": ""1.0.0.0"",
    ""Thumbnail"": """"
}}";
}
