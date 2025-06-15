using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CTCore.DynamicQuery.Common.Exceptions;
using CTCore.DynamicQuery.Core.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZEN.Contract.AspAccountDto;
using ZEN.Domain.DTO.UserDto.Response;
using ZEN.Domain.DTO.WorkExperienceDto.Request;
using ZEN.Domain.Entities.Identities;
using ZEN.Domain.Interfaces;

namespace ZEN.Domain.Entities;

public class AspUser : IdentityUser, IAggregationRoot
{
    [StringLength(256)]
    public string fullname { get; set; } = default!;
    public string? university_name { get; set; }
    public string? address { get; set; }
    public string? phone_number { get; set; }
    public string? github { get; set; }
    public DateTime? dob { get; set; }
    public string? avatar { get; set; }
    public string? position_career { get; set; }
    public int? expOfYear { get; set; }
    public string? background { get; set; }
    public string? mindset { get; set; }
    public string? linkedin_url { get; set; }
    public string? facebook_url { get; set; }
    public double? GPA { get; set; }

    public virtual List<RefreshToken> RefreshTokens { get; set; } = [];
    public virtual List<UserSkill> UserSkills { get; set; } = [];
    public virtual List<UserProject> UserProjects { get; set; } = [];
    public virtual List<WorkExperience> WorkExperiences { get; set; } = [];

    private IReadOnlyCollection<WorkExperience> we => WorkExperiences.AsReadOnly();
    private IReadOnlyCollection<UserSkill> us => UserSkills.AsReadOnly();


    public AspUser() { }
    public AspUser(
       string fullname,
        string? university_name,
        string? address,
        string? phone_number,
        string? github,
        DateTime? dob,
        string? avatar,
        string? email,
        string? position_career,
        int? expOfYear,
        string? background,
        string? mindset,
        string? linkedin_url,
        string? facebook_url,
        double? GPA)
    {
        this.fullname = fullname;
        this.university_name = university_name;
        this.address = address;
        this.phone_number = phone_number;
        this.github = github;
        this.dob = dob;
        this.avatar = avatar;
        this.Email = email;
        this.position_career = position_career;
        this.expOfYear = expOfYear;
        this.background = background;
        this.mindset = mindset;
        this.linkedin_url = linkedin_url;
        this.facebook_url = facebook_url;
        this.GPA = GPA;
        Validate();
    }
    public static AspUser Create(
        string fullname,
        string? university_name = null,
        string? address = null,
        string? phone_number = null,
        string? github = null,
        DateTime? dob = null,
        string? avatar = null,
        string? email = null,
        string? position_career = null,
        int? expOfYear = null,
        string? background = null,
        string? mindset = null,
        string? linkedin_url = null,
        string? facebook_url = null,
        double? GPA = null)
    {
        return new AspUser(fullname, university_name, address, phone_number, github, dob, avatar, email, position_career, expOfYear, background, mindset, linkedin_url, facebook_url, GPA);
    }

    public void Update(ReqUserDto userDto, string ava_url)
    {
        this.fullname = userDto.fullname ?? this.fullname;
        this.university_name = userDto.university_name ?? this.university_name;
        this.address = userDto.address ?? this.address;
        this.phone_number = userDto.phone_number ?? this.phone_number;
        this.github = userDto.github ?? this.github;
        this.dob = userDto.dob ?? this.dob;
        this.Email = userDto.email ?? this.Email;
        this.avatar = ava_url ?? this.avatar;
        this.position_career = userDto.position_career ?? this.position_career;
        this.expOfYear = userDto.expOfYear ?? this.expOfYear;
        this.background = userDto.background ?? this.background;
        this.mindset = userDto.mindset ?? this.mindset;
        this.linkedin_url = userDto.linkedin_url ?? this.linkedin_url;
        this.facebook_url = userDto.facebook_url ?? this.facebook_url;
        this.GPA = userDto.GPA ?? GPA;
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(fullname))
            throw new BadHttpRequestException("Full name is required!");
    }

    public void AddWorkExperience(ReqWorkExperienceDto dto, string user_id)
    {
        var newWE = WorkExperience.Create(user_id, dto.company_name, dto.position, dto.duration, dto.description, dto.project_id);
        WorkExperiences.Add(newWE);
    }

    public void UpdateWorkExperience(ReqWorkExperienceDto dto, string we_id)
    {
        var currentWE = WorkExperiences.FirstOrDefault(x => x.Id == we_id);
        if (currentWE is null) throw new NotFoundException("Work experience not found!");
        currentWE.Update(dto);
    }

    public void AddUserSkill(string skill_id)
    {
        var newUS = UserSkill.Create(this.Id, skill_id);
        UserSkills.Add(newUS);
    }
    public void DeleteUserSkill(List<string> skill_id)
    {
        var skillsToRemove = UserSkills
            .Where(us => us.skill_id != null && skill_id.Contains(us.skill_id))
            .ToList();

        skillsToRemove.ForEach(us => UserSkills.Remove(us));
    }
}

public class AspUserConfiguration : IEntityTypeConfiguration<AspUser>
{
    public void Configure(EntityTypeBuilder<AspUser> builder)
    {
        builder.HasMany(u => u.UserSkills)
                       .WithOne(us => us.AspUser)
                       .HasForeignKey(us => us.user_id)
                       .OnDelete(DeleteBehavior.Cascade);
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