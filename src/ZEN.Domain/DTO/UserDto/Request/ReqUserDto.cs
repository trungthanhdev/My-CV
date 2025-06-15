using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ZEN.Contract.AspAccountDto
{
    public class ReqUserDto
    {
        public string? fullname { get; set; }
        public string? university_name { get; set; }
        public string? address { get; set; }
        public string? phone_number { get; set; }
        public string? github { get; set; }
        public DateTime? dob { get; set; }
        public IFormFile? avatar { get; set; }
        public string? email { get; set; }
        public string? position_career { get; set; }
        public int? expOfYear { get; set; }
        public string? background { get; set; }
        public string? mindset { get; set; }
        public string? linkedin_url { get; set; }
        public string? facebook_url { get; set; }
        public double? GPA { get; set; }


    }
}