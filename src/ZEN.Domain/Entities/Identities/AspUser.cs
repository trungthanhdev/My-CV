using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZEN.Contract.AspAccountDto;
using ZEN.Domain.DTO.UserDto.Response;
using ZEN.Domain.Entities.Identities;

namespace ZEN.Domain.Entities;

public class AspUser : IdentityUser
{
    [StringLength(256)]
    public string fullname { get; set; } = default!;
    public string? university_name { get; set; }
    public string? address { get; set; }
    public string? phone_number { get; set; }
    public string? github { get; set; }
    public DateTime? dob { get; set; }
    public string? avatar { get; set; }

    public virtual List<RefreshToken> RefreshTokens { get; set; } = [];
    public virtual List<UserSkill> UserSkills { get; set; } = [];
    public virtual List<UserProject> UserProjects { get; set; } = [];
    public virtual List<WorkExperience> WorkExperiences { get; set; } = [];

    public AspUser() { }
    public AspUser(
        string fullname,
        string? university_name,
        string? address,
        string? phone_number,
        string? github,
        DateTime? dob,
        string? avatar,
        string? email)
    {
        this.fullname = fullname;
        this.university_name = university_name;
        this.address = address;
        this.phone_number = phone_number;
        this.github = github;
        this.dob = dob;
        this.avatar = avatar;
        this.Email = email;
        Validate();
    }
    public static AspUser Create(
        string fullname,
        string? university_name,
        string? address,
        string? phone_number,
        string? github,
        DateTime? dob,
        string? email,
        string? avatar)
    {
        return new AspUser(fullname, university_name, address, phone_number, github, dob, avatar, email);
    }

    public void Update(ReqUserDto userDto)
    {
        this.fullname = userDto.fullname ?? this.fullname;
        this.university_name = userDto.university_name ?? this.university_name;
        this.address = userDto.address ?? this.address;
        this.phone_number = userDto.phone_number ?? this.phone_number;
        this.github = userDto.github ?? this.github;
        this.dob = userDto.dob ?? this.dob;
        this.Email = userDto.email ?? this.Email;
        this.avatar = userDto.avatar ?? this.avatar;
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(fullname))
            throw new BadHttpRequestException("Full name is required!");
    }
}

public class AspUserConfiguration : IEntityTypeConfiguration<AspUser>
{
    public void Configure(EntityTypeBuilder<AspUser> builder)
    {

        var trungthanh = new AspUser
        {
            Id = "d726c4b1-5a4e-4b89-84af-92c36d3e28aa",
            UserName = "trungthanh",
            NormalizedUserName = "TRUNGTHANH",
            Email = "buithanh10112000@gmail.com",
            NormalizedEmail = "BUITHANH10112000@GMAIL.COM",
            EmailConfirmed = true,
            fullname = "Bùi Nguyễn Trung Thành",
            dob = new DateTime(2003, 11, 10, 0, 0, 0, DateTimeKind.Utc),
            phone_number = "0878508886",
            address = "Thành phố Hồ Chí Minh, Vietnam",
            university_name = "Trường Đại học Công nghệ TP.HCM - HUTECH",
            github = "https://github.com/trungthanhdev",
            SecurityStamp = "751978da-7429-490f-9307-7343d9c25243",
            ConcurrencyStamp = "83309615-b3ff-43f4-9570-cd276fbccb70",

        };
        trungthanh.PasswordHash = "AQAAAAEAACcQAAAAED/5CrSrnoyXt+feHi0NO7bPjy2E+gl5Tpxu9gLyyX0t7Wh19gMIJZB4DaPOj9B1JA==";
        var trunghuy = new AspUser
        {
            Id = "d3c1945f-a4e0-470b-aabd-88e81fb2a1b6",
            UserName = "trunghuy",
            NormalizedUserName = "TRUNGHUY",
            Email = "trunghuy832@gmail.com",
            NormalizedEmail = "TRUNGHUY832@GMAIL.COM",
            EmailConfirmed = true,
            fullname = "Nguyễn Trung Huy",
            SecurityStamp = "9edcbd37-fa5c-4530-bf5f-d6c67a77883c",
            ConcurrencyStamp = "4f5e70b3-5b7c-489c-b4e5-fd24fb711d0d",
            dob = new DateTime(2001, 3, 8, 0, 0, 0, DateTimeKind.Utc),
            phone_number = "0917764302",
            address = "Thành phố Hồ Chí Minh, Vietnam",
            university_name = "Trường Đại học Công nghệ TP.HCM - HUTECH",
            github = "https://github.com/trunghuydev",
        };
        trunghuy.PasswordHash = "AQAAAAEAACcQAAAAEG/qHqS7A33fynGiElPAVykGCTZ792oFGFoJKjvYYOLw4sgEh8NPnlvY9ez/k7ODBQ==";
        builder.HasData(trungthanh, trunghuy);
    }
}