﻿using System;
using Septem.Utils.Helpers.DateTime;

namespace Septem.Utils.Domain;

public abstract class DomainEventBase
{
    protected DomainEventBase(IDateTimeProvider dateTimeProvider)
    {
        DateOccurred = dateTimeProvider.UtcNow;
    }

    public DateTimeOffset DateOccurred { get; protected set; }
}