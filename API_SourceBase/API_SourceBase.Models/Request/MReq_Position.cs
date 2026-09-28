using System.ComponentModel.DataAnnotations;
using API_SourceBase.Models.Common;

namespace API_SourceBase.Models.Request
{
    public class MReq_Position : BaseModel.History
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Phòng ban không được để trống")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Tên chức vụ không được để trống")]
        [StringLength(100, ErrorMessage = "Tên chức vụ tối đa 100 ký tự")]
        public string Name { get; set; } = null!;

        public byte Type { get; set; }

        public short? Role { get; set; }

        [Required(ErrorMessage = "Mã chức vụ không được để trống")]
        [StringLength(10, ErrorMessage = "Mã chức vụ tối đa 10 ký tự")]
        public string Code { get; set; } = null!;

        public int Sort { get; set; }
    }
}
