using System;

namespace SocialNetwork.Core.Scope
{
    public interface IFeedableNote
    {
        string OwnerId { get; }

        string Title { get; }

        string Description { get; }

        DateTime Time { get; }
    }
}
