using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoogleCalenderDemo.Models
{
    [Table("GoogleRefreshToken")]
    public class GoogleRefreshToken
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RefreshTokenId { get; set; }

        public string UserName { get; set; }
        
        public string RefreshToken { get; set; }
    }
}