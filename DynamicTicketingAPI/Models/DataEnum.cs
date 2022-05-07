using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicTicketingAPI.Models
{
    public enum PrmType
    {
        AnsiString,
        AnsiStringFixedLength,
        Binary,
        Boolean,
        Byte,
        Currency,
        Date,
        DateTime,
        DateTime2,
        DateTimeOffset,
        Decimal,
        Double,
        Guid,
        Int16,
        Int32,
        Int64,
        Object,
        SByte,
        Single,
        String,
        StringFixedLength,
        Time,
        UInt16,
        UInt32,
        UInt64,
        VarNumeric,
        Xml
    }

    public enum PrmDirection
    {
        Input,
        InputOutput,
        Output,
        ReturnValue
    }
    public enum PrmDataRowVersion
    {
        Current,
        Default,
        Original,
        Proposed
    }
}