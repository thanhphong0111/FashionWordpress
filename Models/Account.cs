using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Group1_CourseOnline.Models
{
    public partial class Account
    {
        public int AccountId { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid format email.")]
        public string? Email { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
        public int? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public int? Role { get; set; }
        public bool? IsSocialAccount { get; set; }
        public DateTime? CreateDate { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}
