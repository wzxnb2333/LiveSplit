using System;
using System.Linq;

namespace LiveSplit.Updates;

public static class Git
{
    private static readonly string Revision =
        string.IsNullOrWhiteSpace(GitInfo.revision)
        ? null
        : GitInfo.revision.Replace("\r", "").Replace("\n", "")
    ;
    private static readonly string Describe =
        string.IsNullOrWhiteSpace(GitInfo.version)
        ? null
        : GitInfo.version.Replace("\r", "").Replace("\n", "")
    ;
    private static readonly (string LastTag, int CommitsSinceLastTag, bool IsDirty) DescribeInfo = ParseDescribe(Describe);
    private static readonly bool IsDirty = DescribeInfo.IsDirty;
    public static readonly string LastTag = DescribeInfo.LastTag;
    public static readonly int CommitsSinceLastTag = DescribeInfo.CommitsSinceLastTag;
    public static readonly string Version =
        Describe == null ? null : new[] { LastTag }
        .Concat(CommitsSinceLastTag > 0 ? new[] { CommitsSinceLastTag.ToString() } : [])
#if DEBUG
        .Concat(new[] { "debug" })
#endif
        .Concat(IsDirty ? new[] { "dirty" } : [])
        .Aggregate((a, b) => a + "-" + b)
    ;
    public static readonly string Branch =
        string.IsNullOrWhiteSpace(GitInfo.branch)
        ? null
        : GitInfo.branch.Replace("\r", "").Replace("\n", "")
    ;
    public static readonly Uri RevisionUri =
        LastTag == null || Revision == null
        ? null
        : new Uri("https://github.com/LiveSplit/LiveSplit/tree/" + (CommitsSinceLastTag > 0 ? Revision : LastTag))
    ;

    internal static (string LastTag, int CommitsSinceLastTag, bool IsDirty) ParseDescribe(string describe)
    {
        if (string.IsNullOrWhiteSpace(describe))
        {
            return (null, 0, false);
        }

        bool isDirty = false;
        if (describe.EndsWith("-dirty", StringComparison.Ordinal))
        {
            describe = describe[..^"-dirty".Length];
            isDirty = true;
        }

        int revisionSeparator = describe.LastIndexOf('-');
        if (revisionSeparator < 0)
        {
            return (describe, 0, isDirty);
        }

        string revisionText = describe[(revisionSeparator + 1)..];
        if (!revisionText.StartsWith("g", StringComparison.Ordinal) || revisionText.Length <= 1)
        {
            return (describe, 0, isDirty);
        }

        int commitsSeparator = describe.LastIndexOf('-', revisionSeparator - 1);
        if (commitsSeparator < 0)
        {
            return (describe, 0, isDirty);
        }

        string commitsText = describe[(commitsSeparator + 1)..revisionSeparator];
        if (!int.TryParse(commitsText, out int commitsSinceLastTag))
        {
            return (describe, 0, isDirty);
        }

        string lastTag = describe[..commitsSeparator];
        return (lastTag, commitsSinceLastTag, isDirty);
    }
}
