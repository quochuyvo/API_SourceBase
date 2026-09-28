using API_SourceBase.Models.Common;

namespace API_SourceBase.Models.Response
{
    public class MRes_Department : BaseModel.History
    {
        public int Id { get; set; }
        public int FarmId { get; set; }
        public int FactoryId { get; set; }
        public int? WorkerId { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int Sort { get; set; }

        public MRes_Account_Info_Custom? CreatedByObj { get; set; }
        public MRes_Account_Info_Custom? UpdatedByObj { get; set; }
    }
}
