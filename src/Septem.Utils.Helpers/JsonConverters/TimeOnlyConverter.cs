﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Septem.Utils.Helpers.JsonConverters;

public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    private readonly string _serializationFormat;

    public TimeOnlyConverter() : this(null)
    {
    }

    public TimeOnlyConverter(string serializationFormat)
    {
        _serializationFormat = serializationFormat ?? "HH:mm:ss.fff";
    }

    public override TimeOnly Read(ref Utf8JsonReader reader,
        Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()!;
        var result =  TimeOnly.ParseExact(value, _serializationFormat);
        return result;
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value,
        JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(_serializationFormat));
}