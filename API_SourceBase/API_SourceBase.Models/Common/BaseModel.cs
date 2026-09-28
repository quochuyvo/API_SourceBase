using System;

namespace API_SourceBase.Models.Common
{
    public class BaseModel
    {
        public class History
        {
            public short Status { get; set; } = 1;
            public DateTime? CreatedAt { get; set; }
            public int? CreatedBy { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public int? UpdatedBy { get; set; }
        }
    }
}
