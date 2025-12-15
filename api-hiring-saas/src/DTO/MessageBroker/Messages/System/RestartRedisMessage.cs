namespace DTO.MessageBroker.Messages.System;

public sealed record RestartRedisMessage(
    string ErrorCodeMessage) : MessageBase;