using DTO.Attributes;

namespace DTO.Enums.User;

public enum UserNoteType
{
    [LocalizationKey("enum.noteType.featureRequest")]
    FeatureRequest = 1,
    [LocalizationKey("enum.noteType.cancellationReason")]
    CancellationReason = 2
}
