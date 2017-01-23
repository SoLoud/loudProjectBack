using SoLoud.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace SoLoud.Helpers
{
    public static class MyExtensions
    {
        public static bool HasProperty(this object objectToCheck, string propertyName)
        {
            var type = objectToCheck.GetType();
            return type.GetProperty(propertyName) != null;
        }

        public static void makeEqualTo(this object ToChange, object ToCopyFrom)
        {
            foreach (PropertyInfo property in ToCopyFrom.GetType().GetProperties())
            {
                if (!property.CanRead || !property.CanWrite) continue;

                var propToChange = ToChange.GetType().GetProperty(property.Name);
                if (propToChange != null)
                    propToChange.SetValue(ToChange, property.GetValue(ToCopyFrom));
            }
        }

        public static void makeEqualTo<T, TCopy>(this List<T> ToChange, List<TCopy> ToCopyFrom)
        {
            ToChange.RemoveAll(x => true);

            foreach (var item in ToCopyFrom)
            {
                var newTemp = Activator.CreateInstance<T>();

                newTemp.makeEqualTo(item);
                ToChange.Add(newTemp);
            }
        }
    }
}