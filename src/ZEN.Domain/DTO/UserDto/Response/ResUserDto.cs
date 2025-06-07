using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZEN.Domain.DTO.UserDto.Response
{
    public class ResUserDto
    {
        public string? fullname { get; set; }
        public string? university_name { get; set; }
        public string? address { get; set; }
        public string? phone_number { get; set; }
        public string? github { get; set; }
        public DateTime? dob { get; set; }
        public string? avatar { get; set; }
    }
}