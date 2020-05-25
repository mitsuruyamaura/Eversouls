using System;

public class ConvertEnumType
{
    public static IEnum GetEnumTypeFromString<IEnum>(string str) where IEnum : struct {
        return (IEnum)Enum.Parse(typeof(IEnum), str, true);
    }
}
