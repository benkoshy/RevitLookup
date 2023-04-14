using System.Text;
using System.Text.RegularExpressions;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using Octokit;
using Serilog;

partial class Build
{
    [GeneratedRegex(@"(\d+\.)+\d+", RegexOptions.Compiled)]
    private static partial Regex VersionRegexGenerator();
    readonly Regex VersionRegex = VersionRegexGenerator();
    readonly AbsolutePath ChangeLogPath = RootDirectory / "Changelog.md";
    [Parameter] string GitHubToken { get; set; }
    [GitVersion(NoFetch = true)] readonly GitVersion GitVersion;

    Target PublishGitHubRelease => _ => _
        .TriggeredBy(CreateInstaller)
        .Requires(() => GitHubToken)
        .Requires(() => GitRepository)
        .Requires(() => GitVersion)
        .OnlyWhenStatic(() => GitRepository.IsOnMasterBranch() && IsServerBuild)
        .Executes(async () =>
        {
            GitHubTasks.GitHubClient = new GitHubClient(new ProductHeaderValue(Solution.Name))
            {
                Credentials = new Credentials(GitHubToken)
            };

            var gitHubName = GitRepository.GetGitHubName();
            var gitHubOwner = GitRepository.GetGitHubOwner();
            var artifacts = Directory.GetFiles(ArtifactsDirectory, "*");
            var version = VersionRegex.Match(artifacts[^1]).Value;

            await CheckTagsAsync(gitHubOwner, gitHubName, version);
            Log.Information("Tag: {Version}", version);

            var newRelease = new NewRelease(version)
            {
                Name = version,
                Body = CreateChangelog(version),
                Draft = true,
                // Prerelease = true,
                TargetCommitish = GitVersion.Sha
            };

            var draft = await CreatedDraftAsync(gitHubOwner, gitHubName, newRelease);
            await UploadArtifactsAsync(draft, artifacts);
            await ReleaseDraftAsync(gitHubOwner, gitHubName, draft);
        });

    string CreateChangelog(string version)
    {
        if (!File.Exists(ChangeLogPath))
        {
            Log.Warning("Unable to locate the changelog file: {Log}", ChangeLogPath);
            return string.Empty;
        }

        Log.Information("Changelog: {Path}", ChangeLogPath);

        var logBuilder = new StringBuilder();
        var changelogLineRegex = new Regex($@"^.*({version})\S*\s");
        const string nextRecordSymbol = "- ";

        foreach (var line in File.ReadLines(ChangeLogPath))
        {
            if (logBuilder.Length > 0)
            {
                if (line.StartsWith(nextRecordSymbol)) break;
                logBuilder.AppendLine(line);
                continue;
            }

            if (!changelogLineRegex.Match(line).Success) continue;
            var truncatedLine = changelogLineRegex.Replace(line, string.Empty);
            logBuilder.AppendLine(truncatedLine);
        }

        if (logBuilder.Length == 0) Log.Warning("No version entry exists in the changelog: {Version}", version);
        return logBuilder.ToString();
    }

    static async Task CheckTagsAsync(string gitHubOwner, string gitHubName, string version)
    {
        var gitHubTags = await GitHubTasks.GitHubClient.Repository.GetAllTags(gitHubOwner, gitHubName);
        if (gitHubTags.Select(tag => tag.Name).Contains(version))
            throw new ArgumentException($"A Release with the specified tag already exists in the repository: {version}");
    }

    static async Task<Release> CreatedDraftAsync(string gitHubOwner, string gitHubName, NewRelease newRelease) =>
        await GitHubTasks.GitHubClient.Repository.Release.Create(gitHubOwner, gitHubName, newRelease);

    static async Task ReleaseDraftAsync(string gitHubOwner, string gitHubName, Release draft) =>
        await GitHubTasks.GitHubClient.Repository.Release.Edit(gitHubOwner, gitHubName, draft.Id, new ReleaseUpdate {Draft = false});

    static async Task UploadArtifactsAsync(Release createdRelease, string[] artifacts)
    {
        foreach (var file in artifacts)
        {
            var releaseAssetUpload = new ReleaseAssetUpload
            {
                ContentType = "application/x-binary",
                FileName = Path.GetFileName(file),
                RawData = File.OpenRead(file)
            };

            await GitHubTasks.GitHubClient.Repository.Release.UploadAsset(createdRelease, releaseAssetUpload);
            Log.Information("Artifact: {Path}", file);
        }
    }
}