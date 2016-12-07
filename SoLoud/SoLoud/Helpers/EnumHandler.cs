using System;

namespace SoLoud.Helpers
{
    public static class EnumHandler
    {
        /// <summary>
        /// Parses string to the enum T. If it can not be parsed it will throw ArgumentException
        /// </summary>
        /// <typeparam name="T">The enum type to which to parse the string</typeparam>
        /// <param name="value">The string to parse to enum</param>
        /// <returns></returns>
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Parses string to the enum T. If it can not be parsed it will return the default value
        /// </summary>
        /// <typeparam name="T">The enum type to which to parse the string</typeparam>
        /// <param name="value">The string to parse to enum</param>
        /// <param name="defaultValue">The default value to return if the string cannot be parsed to the specified enum</param>
        /// <returns></returns>
        public static T ParseEnum<T>(string value, T defaultValue)
        {
            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch (ArgumentException)
            {
                return defaultValue;
            }
        }
    }
}