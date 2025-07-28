using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
   public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public int RoleId { get; set; }
    public string MobileNo { get; set; }
    public string Email { get; set; }
    public string Region { get; set; }
    public int LoginTypeId { get; set; }
    public bool IsActive { get; set; }
}
}