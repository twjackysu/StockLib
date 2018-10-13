using System;
using System.Reflection;

namespace StockLib.EnumExtension
{
    public static class EnumExtension
    {
        public static string ToKey(this Enum @enum, string stockNo)
        {
            MemberInfo[] memberInfo = @enum.GetType().GetMember(@enum.ToString());

            if (memberInfo != null && memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(StockKeyAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    return ((StockKeyAttribute)attributes[0])?.Key.Replace("{StockNo}", stockNo);
                }
            }
            return null;
        }
    }
}
