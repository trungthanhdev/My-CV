using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ZEN.Domain.Definition;
using ZEN.Domain.Entities;
using ZEN.Infrastructure.Mysql;
using ZEN.Infrastructure.Mysql.Persistence;

namespace ZEN.Infrastructure.Extentions;

public static class Migration
{
    public static async Task ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        var context1 = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context1.Database.Migrate();

        using var roleContext = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var roles = await roleContext.Roles.ToListAsync();
        if (roles.Count != RoleMigrates.Values.Length)
        {
            var rolesToAdd = RoleMigrates.Values.Except(
                roles.Select(r => r.Name)
            ).ToList();

            foreach (var ro in rolesToAdd)
            {
                await roleContext.CreateAsync(new IdentityRole(ro!));
            }
        }
    }

}

public class NullEmailSender : IEmailSender<AspUser>
{
    public Task SendConfirmationLinkAsync(AspUser user, string email, string confirmationLink)
        => Task.CompletedTask;

    public Task SendPasswordResetLinkAsync(AspUser user, string email, string resetLink)
        => Task.CompletedTask;

    public Task SendPasswordResetCodeAsync(AspUser user, string email, string resetCode)
        => Task.CompletedTask;
}
