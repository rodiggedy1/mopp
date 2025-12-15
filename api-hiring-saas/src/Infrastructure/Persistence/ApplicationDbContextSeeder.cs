using Application.Common.Interfaces;
using Application.Features.Jobs.JobForms.Commands;
using Application.Features.Jobs.JobsDetails.Commands;
using Application.Features.Languages.Commands;
using Domain.Entities.Medias;
using Domain.Entities.User;
using DTO.Enums.Media;
using DTO.Enums.User;
using DTO.Job.JobForm.JobFormSection;
using MassTransit.Mediator;
using MassTransit.Testing;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Persistence;

public static class ApplicationDbContextSeeder
{
    public static async Task<ApplicationUser> SeedDefaultRolesAndUsersAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager)
    {
        var administratorRole = new IdentityRole<int>("Administrator");
        var customerRole = new IdentityRole<int>("Customer");

        // Create the default roles if not exsist
        if (roleManager.Roles.All(r => r.Name != administratorRole.Name)) await roleManager.CreateAsync(administratorRole);
        if (roleManager.Roles.All(r => r.Name != customerRole.Name)) await roleManager.CreateAsync(customerRole);

        var administrator = new ApplicationUser
        {
            FirstName = "John",
            LastName = "Doe",
            UserName = "administrator@localhost",
            Email = "administrator@localhost",
            EmailConfirmed = true,
            Media = new Media(MediaEntityType.User),
            Status = UserStatus.Active
        };

        administrator.SetActualPassword("Administrator1!");

        // Check if default administrator already created
        var existedAdministrator = userManager.Users.FirstOrDefault(u => u.UserName == administrator.UserName);
        if (existedAdministrator != null) return existedAdministrator;

        // Create default administraotor
        await userManager.CreateAsync(administrator, "_Virtual007!");
        await userManager.AddClaimAsync(administrator, new Claim("scope", "default"));
        await userManager.AddClaimAsync(administrator, new Claim("isAdmin", true.ToString()));

        // Add the user to the role (Administrator)
        await userManager.AddToRolesAsync(
            administrator,
            new[] { administratorRole.Name! });

        return administrator;
    }

    public static async Task SeedDefaultLanguages(
        ISender mediatr,
        IApplicationDbContext dbContext)
    {
        if (!await dbContext.Language.AnyAsync())
        {
            await mediatr.Send(new LanguageCreateCommand("English", "en", "en-US", true));
        }
    }

    public static async Task SeedDefaultJobOffer(
        ISender mediatr,
        IApplicationDbContext dbContext)
    {
        if(!await dbContext.JobDetails.AnyAsync())
        {
            var jobForm = await mediatr.Send(new JobFormCreateCommand(
                "Default Home cleaning Form",
                "Ready Made Form for Residential Cleaners",
                string.Empty,
                GenerateDefaultJobOfferSections(),
                false));

            await mediatr.Send(new JobDetailsCreateCommand(
                "Cleaning Professional Application", 
                "Join our team of cleaning professionals",
                "Mumbai",
                "Full Time",
                10000,
                jobForm.UniqueHash,
                jobForm.Id));
        }
    }

    private static List<JobFormSectionCreateRequest> GenerateDefaultJobOfferSections()
    {
        var response = new List<JobFormSectionCreateRequest>();

        // Basic Info
        response.Add(new JobFormSectionCreateRequest()
        {
            Name = "Basic Info",
            Code = "Default",
            Position = 0,
            Icon = new JobFormSectionIconCreateRequest()
            {
                ChangingThisBreaksApplicationSecurity = @"<svg xmlns=""http://www.w3.org/2000/svg"" fill=""none"" viewBox=""0 0 24 24"" stroke-width=""1.5"" stroke=""currentColor"" style=""width: 24px; height: 24px""><path stroke-linecap=""round"" stroke-linejoin=""round"" d=""M17.982 18.725A7.488 7.488 0 0 0 12 15.75a7.488 7.488 0 0 0-5.982 2.975m11.963 0a9 9 0 1 0-11.963 0m11.963 0A8.966 8.966 0 0 1 12 21a8.966 8.966 0 0 1-5.982-2.275M15 9.75a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z"" /></svg>"
            }
        });

        // Requirements
        response.Add(new JobFormSectionCreateRequest()
        {
            Name = "Requirements",
            Description = "Requirements",
            Code = "Custom",
            Position = 1,
            Icon = new JobFormSectionIconCreateRequest()
            {
                ChangingThisBreaksApplicationSecurity = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""24"" height=""24"" viewBox=""0 0 24 24"" fill=""none"" stroke=""currentColor"" stroke-width=""1.5"" stroke-linecap=""round"" stroke-linejoin=""round"" class=""w-6 h-6""><path d=""m9 11 3 3L22 4"" /><path d=""M21 12v7a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11"" /></svg>"
            },
            JobFormSectionProperties = new List<JobFormSectionPropertyCreateRequest>
            {
                new JobFormSectionPropertyCreateRequest
                {
                    Type = "radio",
                    Label = "Do you have professional cleaning experience?",
                    Required = false,
                    Position = 0,
                    PlaceHolder = "",
                    Options = new List<string> { "Yes", "No" },
                    Validation = new JobFormSectionPropertyValidationCreateRequest { Pattern = "" }
                },
                new JobFormSectionPropertyCreateRequest
                {
                    Type = "radio",
                    Label = "Do you have a bank account for direct deposit?",
                    Required = false,
                    Position = 1,
                    PlaceHolder = "",
                    Options = new List<string> { "Yes", "No" },
                    Validation = new JobFormSectionPropertyValidationCreateRequest { Pattern = "" }
                },
                new JobFormSectionPropertyCreateRequest
                {
                    Type = "radio",
                    Label = "Are you legally authorized to work in the united states?",
                    Required = false,
                    Position = 2,
                    PlaceHolder = "",
                    Options = new List<string> { "Yes", "No" },
                    Validation = new JobFormSectionPropertyValidationCreateRequest { Pattern = "" }
                },
                new JobFormSectionPropertyCreateRequest
                {
                    Type = "radio",
                    Label = "Do you consent to a background check?",
                    Required = false,
                    Position = 3,
                    PlaceHolder = "",
                    Options = new List<string> { "Yes", "No" },
                    Validation = new JobFormSectionPropertyValidationCreateRequest { Pattern = "" }
                },
                new JobFormSectionPropertyCreateRequest
                {
                    Type = "about-textarea",
                    Label = "Tell us about your professional cleaning experience",
                    Required = false,
                    Position = 4,
                    PlaceHolder = "",
                    Options = new List<string>(),
                    Validation = new JobFormSectionPropertyValidationCreateRequest { Pattern = "" }
                }
            }
        });

        // Specialties
        response.Add(new JobFormSectionCreateRequest()
        {
            Name = "Specialties",
            Description = "Select your top 3 areas of cleaning experience",
            Code = "Custom",
            Position = 2,
            Icon = new JobFormSectionIconCreateRequest()
            {
                ChangingThisBreaksApplicationSecurity = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""24"" height=""24"" viewBox=""0 0 24 24"" fill=""none"" stroke=""currentColor"" stroke-width=""1.5"" stroke-linecap=""round"" stroke-linejoin=""round"" class=""w-6 h-6""><rect width=""7"" height=""9"" x=""3"" y=""3"" rx=""1"" /><rect width=""7"" height=""5"" x=""14"" y=""3"" rx=""1"" /><rect width=""7"" height=""9"" x=""14"" y=""12"" rx=""1"" /><rect width=""7"" height=""5"" x=""3"" y=""16"" rx=""1"" /></svg>"
            },
            JobFormSectionProperties = new List<JobFormSectionPropertyCreateRequest>
            {
                new JobFormSectionPropertyCreateRequest
                {
                    Type = "tags",
                    Label = "Skills",
                    Required = false,
                    Position = 0,
                    PlaceHolder = "",
                    Options = new List<string>
                    {
                        "Pro Residential Cleaning",
                        "Commercial Cleaning",
                        "Hotel Cleaning",
                        "Move in/Move Out",
                        "Office Cleaning",
                        "Post Construction",
                        "Window Cleaning",
                        "Airbnb Cleaning",
                        "Eco-Friendly Cleaning",
                        "Medical Facility Cleaning"
                    },
                    Validation = new JobFormSectionPropertyValidationCreateRequest { Pattern = "" }
                }
            }
        });

        // Your Bio
        response.Add(new JobFormSectionCreateRequest()
        {
            Name = "Your Bio",
            Description = "Upload your image",
            Code = "Custom",
            Position = 3,
            Icon = new JobFormSectionIconCreateRequest()
            {
                ChangingThisBreaksApplicationSecurity = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""24"" height=""24"" viewBox=""0 0 24 24"" fill=""none"" stroke=""currentColor"" stroke-width=""1.5"" stroke-linecap=""round"" stroke-linejoin=""round"" class=""w-6 h-6""><path d=""M6.827 6.175A2.31 2.31 0 0 1 5.186 7.23c-.38.054-.757.112-1.134.175C2.999 7.58 2.25 8.507 2.25 9.574V18a2.25 2.25 0 0 0 2.25 2.25h15A2.25 2.25 0 0 0 21.75 18V9.574c0-1.067-.75-1.994-1.802-2.169a47.865 47.865 0 0 0-1.134-.175 2.31 2.31 0 0 1-1.64-1.055l-.822-1.316a2.192 2.192 0 0 0-1.736-1.039 48.774 48.774 0 0 0-5.232 0 2.192 2.192 0 0 0-1.736 1.039l-.821 1.316Z"" /><path d=""M16.5 12.75a4.5 4.5 0 1 1-9 0 4.5 4.5 0 0 1 9 0ZM18.75 10.5h.008v.008h-.008V10.5Z"" /></svg>"
            },
            JobFormSectionProperties = new List<JobFormSectionPropertyCreateRequest>
            {
                new JobFormSectionPropertyCreateRequest
                {
                    Type = "file",
                    Label = "File Upload",
                    Required = false,
                    Position = 0,
                    PlaceHolder = "",
                    Options = new List<string>(),
                    Validation = new JobFormSectionPropertyValidationCreateRequest { Pattern = "" }
                }
            }
        });

        // Video
        response.Add(new JobFormSectionCreateRequest()
        {
            Name = "Video",
            Code = "Custom",
            Position = 4,
            Icon = new JobFormSectionIconCreateRequest()
            {
                ChangingThisBreaksApplicationSecurity = @"<svg xmlns=""http://www.w3.org/2000/svg"" width=""24"" height=""24"" viewBox=""0 0 24 24"" fill=""none"" stroke=""currentColor"" stroke-width=""1.5"" stroke-linecap=""round"" stroke-linejoin=""round"" class=""w-6 h-6""><path d=""m15.75 10.5 4.72-4.72a.75.75 0 0 1 1.28.53v11.38a.75.75 0 0 1-1.28.53l-4.72-4.72M4.5 18.75h9a2.25 2.25 0 0 0 2.25-2.25v-9a2.25 2.25 0 0 0-2.25-2.25h-9A2.25 2.25 0 0 0 2.25 7.5v9a2.25 2.25 0 0 0 2.25 2.25Z"" /></svg>"
            },
            JobFormSectionProperties = new List<JobFormSectionPropertyCreateRequest>
            {
                new JobFormSectionPropertyCreateRequest
                {
                    Type = "video",
                    Label = "Video",
                    Required = false,
                    Position = 0,
                    PlaceHolder = "",
                    Options = new List<string>(),
                    Validation = new JobFormSectionPropertyValidationCreateRequest { Pattern = "" }
                }
            }
        });

        return response;
    }
}

