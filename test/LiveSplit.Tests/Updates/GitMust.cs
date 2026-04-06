using LiveSplit.Updates;

using Xunit;

namespace LiveSplit.Tests.Updates;

public class GitMust
{
    [Fact]
    public void ParseStandardGitDescribeOutput()
    {
        (string lastTag, int commitsSinceLastTag, bool isDirty) = Git.ParseDescribe("1.8.37-2-gcceb2719");

        Assert.Equal("1.8.37", lastTag);
        Assert.Equal(2, commitsSinceLastTag);
        Assert.False(isDirty);
    }

    [Fact]
    public void ParseGitDescribeOutputWhenTagContainsHyphens()
    {
        (string lastTag, int commitsSinceLastTag, bool isDirty) = Git.ParseDescribe("1.8.37-multilingual.3-2-gcceb2719-dirty");

        Assert.Equal("1.8.37-multilingual.3", lastTag);
        Assert.Equal(2, commitsSinceLastTag);
        Assert.True(isDirty);
    }
}
