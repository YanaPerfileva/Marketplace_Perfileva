using System;

namespace Marketplace.Data.Dto
{
    public class UserLogDto
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public string Details { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string UserEmail { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;

        public string EntityName { get; set; } = string.Empty;
        public string EntityDescription { get; set; } = string.Empty;
    }
}
