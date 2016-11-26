﻿// ==========================================================================
//  RangeValidator.cs
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex Group
//  All rights reserved.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Squidex.Infrastructure.Tasks;

namespace Squidex.Core.Schemas.Validators
{
    public sealed class RangeValidator<T> : IValidator where T : struct, IComparable<T>
    {
        private readonly T? min;
        private readonly T? max;

        public RangeValidator(T? min, T? max)
        {
            if (min.HasValue && max.HasValue && min.Value.CompareTo(max.Value) >= 0)
            {
                throw new ArgumentException("Min value must be greater than max value", nameof(min));
            }

            this.min = min;
            this.max = max;
        }

        public Task ValidateAsync(object value, ICollection<string> errors)
        {
            if (value == null)
            {
                return TaskHelper.Done;
            }

            var typedValue = (T)value;

            if (min.HasValue && typedValue.CompareTo(min.Value) < 0)
            {
                errors.Add($"<FIELD> must be greater than '{min}'");
            }

            if (max.HasValue && typedValue.CompareTo(max.Value) > 0)
            {
                errors.Add($"<FIELD> must be less than '{max}'");
            }

            return TaskHelper.Done;
        }
    }
}