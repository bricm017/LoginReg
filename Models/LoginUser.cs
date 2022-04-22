using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginReg.Models
{
    public class LoginUser
    {
        [Key]

        [Required(ErrorMessage ="Email is required!")]
        [EmailAddress]
        public string LogEmail{get; set;}
        [Required(ErrorMessage ="Password is required!")]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string LogPassword{get; set;}
    }
}