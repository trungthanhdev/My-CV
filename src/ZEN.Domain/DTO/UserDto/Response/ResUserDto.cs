using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZEN.Domain.DTO.UserDto.Response
{
    public class ResUserDto
    {
        public string? user_id { get; set; }
        public string? fullname { get; set; }
        public string? university_name { get; set; }
        public string? address { get; set; }
        public string? phone_number { get; set; }
        public string? github { get; set; }
        public string? dob { get; set; }
        public string? avatar { get; set; }
        public double? GPA { get; set; }
        public string? email { get; set; }
        public string? workExpOfYear { get; set; }
        public string? linkedin_url { get; set; }
        public string? mindset { get; set; }
        public string? position_career { get; set; }
        public string? background { get; set; }
        public string? facebook_url { get; set; }
    }
}