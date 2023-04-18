using Nuke.Common.IO;

partial class Build
{
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";

    readonly string[] Configurations =
    {
        "Release*",
        "Installer"
    };

    readonly Dictionary<string, string> VersionMap = new()
    {
        {"Release R21", "2021.2.2"},
        {"Release R22", "2022.2.2"},
        {"Release R23", "2023.2.2"},
        {"Release R24", "2024.0.2"}
    };
}