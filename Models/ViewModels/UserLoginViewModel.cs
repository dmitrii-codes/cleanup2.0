using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class UserLoginViewModel : BaseEntity
    {
        
        [Required]
        [MinLength(2, ErrorMessage="Username must be at least 2 letters")]
        public string UserNameLogin { get; set; }
        
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[$@$!%*#?&])[A-Za-z\d$@$!%*#?&]{8,}$")]
        public string PasswordLogin { get; set; }
    }
}