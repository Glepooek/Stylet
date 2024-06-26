﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace Stylet.Samples.RedditBrowser.RedditApi;

public interface IRedditClient
{
    Task<PostCollection> GetPostsAsync(string subreddit, SortMode sortMode);
    Task<CommentCollection> GetPostComments(string subreddit, string postId36);
}

public class RedditClient : IRedditClient
{
    private readonly IRedditApi api;

    public RedditClient()
    {
        this.api = RestClient.For<IRedditApi>("http://reddit.com");
    }

    public async Task<PostCollection> GetPostsAsync(string subreddit, SortMode sortMode)
    {
        var postCollection = new PostCollection(this.api, subreddit, sortMode.Mode);
        await postCollection.LoadAsync();
        return postCollection;
    }

    public async Task<CommentCollection> GetPostComments(string subreddit, string postId36)
    {
        var commentCollection = new CommentCollection(this.api, subreddit, postId36);
        await commentCollection.LoadAsync();
        return commentCollection;
    }
}

public struct SortMode
{
    public static SortMode New { get; } = new("new", "New");
    public static SortMode Hot { get; } = new("hot", "Hot");
    public static SortMode Top { get; } = new("top", "Top");

    public static IEnumerable<SortMode> AllModes => new[] { New, Hot, Top };

    public string Name { get; private set; }
    public string Mode { get; private set; }
    private SortMode(string mode, string name) : this()
    {
        this.Mode = mode;
        this.Name = name;
    }
}
