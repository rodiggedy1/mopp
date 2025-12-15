using DTO.Attributes;

namespace DTO.Enums;

public enum Status
{
    [LocalizationKey("enum.status.deleted")]
    Deleted = -1,
    [LocalizationKey("enum.status.inactive")]
    Active = 1,
    [LocalizationKey("enum.status.inactive")]
    Inactive = 2
}
