using System.ComponentModel.DataAnnotations;
using API_SourceBase.Models.Common;

namespace API_SourceBase.Models.Request
{
    public class MReq_Department : BaseModel.History
    {
        public int Id { get; set; }

        public int FarmId { get; set; }

        public int FactoryId { get; set; }

        public int? WorkerId { get; set; }

        [Required(ErrorMessage = "Mã phòng ban không được để trống")]
        [StringLength(30, ErrorMessage = "Mã phòng ban tối đa 30 ký tự")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Tên phòng ban không được để trống")]
        [StringLength(100, ErrorMessage = "Tên phòng ban tối đa 100 ký tự")]
        public string Name { get; set; } = null!;

        public int Sort { get; set; }
    }
}
