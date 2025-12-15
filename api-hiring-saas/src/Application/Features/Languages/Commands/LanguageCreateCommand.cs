using Application.Common.Interfaces;
using Application.Common.Interfaces.Repository.Base;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Domain.Entities.Languages;
using FluentValidation;

namespace Application.Features.Languages.Commands;

public sealed record LanguageCreateCommand(
    string Name,
    string Code,
    string CultureCode,
    bool IsDefault) : ILanguageUpsertData, ICommand;

public sealed class LanguageCreateCommandHandler : ICommandHandler<LanguageCreateCommand>
{
    private readonly IRepository<Language> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public LanguageCreateCommandHandler(
    IRepository<Language> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(LanguageCreateCommand command, CancellationToken cancellationToken)
    {
        await _repository.AddAsync(Language.Create(command), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

public sealed class LanguageCreateCommandValidator : AbstractValidator<LanguageCreateCommand>
{
    public LanguageCreateCommandValidator()
    {
        RuleFor(cmd => cmd.Name)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(cmd => cmd.Code)
        .NotEmpty()
        .Length(2);

        RuleFor(cmd => cmd.CultureCode)
            .NotEmpty()
            .Length(5);
    }

}
