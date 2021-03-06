﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Qwack.Paths.Features
{
    public interface ITimeStepsFeature
    {
        int TimeStepCount { get; }
        double[] TimeSteps { get; }
        double[] Times { get; }
        void AddDate(DateTime date);
        void AddDates(IEnumerable<DateTime> dates);
        int GetDateIndex(DateTime date);
    }
}