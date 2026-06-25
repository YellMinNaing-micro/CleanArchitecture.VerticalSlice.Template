using System;

namespace CleanArch.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}
