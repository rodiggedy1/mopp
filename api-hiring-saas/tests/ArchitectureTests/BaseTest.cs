using Api.Controllers;
using Application.Common.Interfaces;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using Domain.Entities.Base;
using DTO.MessageBroker;
using EmailService.Services;
using Infrastructure.Persistence;
using NotificationService.Models;
using Assembly = System.Reflection.Assembly;

namespace ArchitectureTests;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(BaseEntity).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(IUnitOfWork).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(UnitOfWork).Assembly;
    protected static readonly Assembly ApiAssembly = typeof(ApiControllerBase).Assembly;
    protected static readonly Assembly DTOAssembly = typeof(MessageBase).Assembly;
    protected static readonly Assembly WorkerAssembly = typeof(Program).Assembly;
    protected static readonly Assembly EmailServiceAssembly = typeof(EmailSender).Assembly;
    protected static readonly Assembly NotificationServiceAssembly = typeof(SignalrUser).Assembly;

    protected static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            DomainAssembly,
            ApplicationAssembly,
            InfrastructureAssembly,
            ApiAssembly,
            DTOAssembly,
            WorkerAssembly,
            EmailServiceAssembly,
            NotificationServiceAssembly)
        .Build();

    protected static readonly IObjectProvider<IType> DomainLayer =
        ArchRuleDefinition.Types().That().ResideInAssembly(DomainAssembly).As("Domain layer");

    protected static readonly IObjectProvider<IType> ApplicationLayer =
        ArchRuleDefinition.Types().That().ResideInAssembly(ApplicationAssembly).As("Application layer");

    protected static readonly IObjectProvider<IType> InfrastructureLayer =
        ArchRuleDefinition.Types().That().ResideInAssembly(InfrastructureAssembly).As("Infrastructure layer");

    protected static readonly IObjectProvider<IType> PresentationLayer =
        ArchRuleDefinition.Types().That().ResideInAssembly(ApiAssembly).As("Presentation layer");

    protected static readonly IObjectProvider<IType> DTOLayer =
        ArchRuleDefinition.Types().That().ResideInAssembly(DTOAssembly).As("DTO layer");

    protected static readonly IObjectProvider<IType> ServiceLayer =
        ArchRuleDefinition
        .Types()
        .That()
        .ResideInAssembly(EmailServiceAssembly)
        .Or()
        .ResideInAssembly(NotificationServiceAssembly)
        .Or()
        .ResideInAssembly(WorkerAssembly)
        .As("Service layer");
}
