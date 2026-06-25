using System;

namespace PlaylistApi.Core.Entities
{
    /// <summary>
    /// Implemented by entities that track when they were created and last updated.
    /// Timestamps are set centrally in <c>AppDbContext.SaveChangesAsync</c>.
    /// </summary>
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}
