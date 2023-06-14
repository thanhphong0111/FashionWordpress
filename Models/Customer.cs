using System;
using System.Collections.Generic;

namespace Group1_CourseOnline.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Accounts = new HashSet<Account>();
            Comments = new HashSet<Comment>();
        }

        public int CustomerId { get; set; }
        public string LastName { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public bool? Status { get; set; }

        public virtual Order? Order { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
