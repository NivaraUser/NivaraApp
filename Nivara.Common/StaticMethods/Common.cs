using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Nivara.Common.StaticMethod
{
    public static class Common
    {
        public static List<KeyValuePair<int, string>> CreateDropDown<TEnum>()
        {
            List<KeyValuePair<int, string>> selectionType = new List<KeyValuePair<int, string>>();
            foreach (var item in Enum.GetValues(typeof(TEnum)))
            {
                selectionType.Add(new KeyValuePair<int, string>(
                (int)item,
                item.GetType()
                .GetMember(item.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                .GetName()
                ));
            }
            return selectionType;
        }

        public static List<string> CreateDropDownForName<TEnum>()
        {
            List<string> selectionType = new List<string>();
            foreach (var item in Enum.GetValues(typeof(TEnum)))
            {
                selectionType.Add(new string(
                item.GetType()
                .GetMember(item.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                .GetName()
                ));
            }
            return selectionType;
        }

       
    }
    
}
