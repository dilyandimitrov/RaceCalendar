﻿namespace RaceCalendar.Domain.Commands;

public interface IDeleteUserSettingsCommand
{
    Task Execute(string userId);
}
