﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrencyValueGeneratorTest
{
    public class MyValueGeneratorSelector : ValueGeneratorSelector
    {        
        public MyValueGeneratorSelector(ValueGeneratorSelectorDependencies dependencies)
           : base(dependencies)
        {
        }

        //public override IValueGeneratorCache Cache => base.Cache;

        //protected override ValueGeneratorSelectorDependencies Dependencies => base.Dependencies;

        public override ValueGenerator Create(IProperty property, IEntityType entityType)
        {
            Type? propertyType = default;
            if (property.ClrType.IsGenericType && property.ClrType.HasElementType)
            {
                propertyType = property.ClrType.GetElementType();
            }
            else if (!property.ClrType.IsGenericType)
            {
                propertyType = property.ClrType;
            }

            //A value is never generated by the database
            //if (property.ValueGenerated != ValueGenerated.Never)
            //{
                if (propertyType == typeof(int))
                {
                    return new TemporaryIntValueGenerator();
                }

                if (propertyType == typeof(long))
                {
                    //return new TemporaryLongValueGenerator();
                    //Replace This
                    return new MyLongValueGenerator();
                }

                if (propertyType == typeof(short))
                {
                    return new TemporaryShortValueGenerator();
                }

                if (propertyType == typeof(byte))
                {
                    return new TemporaryByteValueGenerator();
                }

                if (propertyType == typeof(char))
                {
                    return new TemporaryCharValueGenerator();
                }

                if (propertyType == typeof(ulong))
                {
                    return new TemporaryULongValueGenerator();
                }

                if (propertyType == typeof(uint))
                {
                    return new TemporaryUIntValueGenerator();
                }

                if (propertyType == typeof(ushort))
                {
                    return new TemporaryUShortValueGenerator();
                }

                if (propertyType == typeof(sbyte))
                {
                    return new TemporarySByteValueGenerator();
                }

                if (propertyType == typeof(decimal))
                {
                    return new TemporaryDecimalValueGenerator();
                }

                if (propertyType == typeof(float))
                {
                    return new TemporaryFloatValueGenerator();
                }

                if (propertyType == typeof(double))
                {
                    return new TemporaryDoubleValueGenerator();
                }


                //if (propertyType == typeof(DateTime))
                //{
                //    return new TemporaryDateTimeValueGenerator();
                //}

                //if (propertyType == typeof(DateTimeOffset))
                //{
                //    return new TemporaryDateTimeOffsetValueGenerator();
                //}


                if (propertyType == typeof(Guid))
                {
                    return new TemporaryGuidValueGenerator();
                    //return new MyGuidValueGenerator(false);
                }

            if (propertyType == typeof(string))
            {
                return new StringValueGenerator();
            }

            if (propertyType == typeof(byte[]))
            {
                return new BinaryValueGenerator();
            }


            //} 
            throw new ArgumentException(property.Name, property.DeclaringEntityType.DisplayName(),new Exception($"not support,property.ValueGenerated is { Enum.GetName(property.ValueGenerated)}"));
        }
        public override ValueGenerator Select(IProperty property, IEntityType entityType)
        {
#if !DEBUG
            var vg = Cache.GetOrAdd(property, entityType, (p, t) => Create(p, t) ?? base.Select(property, entityType));
#else
            var vg = Create(property, entityType) ?? base.Select(property, entityType);
#endif
            return vg;
        }
    }
}