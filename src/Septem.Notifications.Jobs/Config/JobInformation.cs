﻿using System;

namespace Septem.Notifications.Jobs.Config;

public class JobInformation
{
    public Type JobType { get; set; }

    public double Interval { get; set; }

    public JobInformation()
    {
        
    }

    public JobInformation(Type jobType, double interval)
    {
        JobType = jobType;
        Interval = interval;
    }
}