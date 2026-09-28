using API_SourceBase.Models.Common;

namespace API_SourceBase.Models.Response
{
    public class MRes_Position : BaseModel.History
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string Name { get; set; } = null!;
        public byte Type { get; set; }
        public short? Role { get; set; }
        public string Code { get; set; } = null!;
        public int Sort { get; set; }

        public MRes_Account_Info_Custom? CreatedByObj { get; set; }
        public MRes_Account_Info_Custom? UpdatedByObj { get; set; }
    }
}
