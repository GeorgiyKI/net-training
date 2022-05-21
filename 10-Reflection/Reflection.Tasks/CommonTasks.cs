using System;
using System.Collections.Generic;
using System.Reflection;

namespace Reflection.Tasks
{
    public static class CommonTasks
    {
        /// <summary>
        /// Returns the lists of public and obsolete classes for specified assembly.
        /// Please take attention: classes (not interfaces, not structs)
        /// </summary>
        /// <param name="assemblyName">name of assembly</param>
        /// <returns>List of public but obsolete classes</returns>
        public static IEnumerable<string> GetPublicObsoleteClasses(string assemblyName)
        {
            if (assemblyName is null)
                throw new ArgumentNullException("assemblyName is null");

            Assembly assembly = Assembly.Load(assemblyName);
            var result = new List<string>();
            foreach (var item in assembly.GetTypes())
            {
                bool isHaveObsoleteAtribute = item.GetCustomAttribute(typeof(ObsoleteAttribute)) != null;
                if (item.IsClass && item.IsPublic && isHaveObsoleteAtribute)
                    result.Add(item.Name);
            }

            return result;
        }

        /// <summary>
        /// Returns the value for required property path
        /// </summary>
        /// <example>
        ///  1) 
        ///  string value = instance.GetPropertyValue("Property1")
        ///  The result should be equal to invoking statically
        ///  string value = instance.Property1;
        ///  2) 
        ///  string name = instance.GetPropertyValue("Property1.Property2.FirstName")
        ///  The result should be equal to invoking statically
        ///  string name = instance.Property1.Property2.FirstName;
        /// </example>
        /// <typeparam name="T">property type</typeparam>
        /// <param name="obj">source object to get property from</param>
        /// <param name="propertyPath">dot-separated property path</param>
        /// <returns>property value of obj for required propertyPath</returns>
        public static T GetPropertyValue<T>(this object obj, string propertyPath)
        {
            // TODO : Implement GetPropertyValue method
            foreach (string part in propertyPath.Split('.'))
            {
                if (obj == null)
                    throw new ArgumentNullException("obj is null");

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null)
                    throw new ArgumentNullException("Property info is null");

                obj = info.GetValue(obj);
            }

            return (T)obj;
        }

        /// <summary>
        /// Assign the value to the required property path
        /// </summary>
        /// <example>
        ///  1)
        ///  instance.SetPropertyValue("Property1", value);
        ///  The result should be equal to invoking statically
        ///  instance.Property1 = value;
        ///  2)
        ///  instance.SetPropertyValue("Property1.Property2.FirstName", value);
        ///  The result should be equal to invoking statically
        ///  instance.Property1.Property2.FirstName = value;
        /// </example>
        /// <param name="obj">source object to set property to</param>
        /// <param name="propertyPath">dot-separated property path</param>
        /// <param name="value">assigned value</param>
        public static void SetPropertyValue(this object obj, string propertyPath, object value)
        {
            // TODO : Implement SetPropertyValue method
            PropertyInfo info = null;
            foreach (string part in propertyPath.Split('.'))
            {
                if (obj == null)
                    throw new ArgumentNullException("obj is null");

                Type type = obj.GetType();
                info = type.GetProperty(part);

                if (info == null)
                    throw new ArgumentNullException("Property info is null");
                else if (!info.CanWrite)
                    info = info.DeclaringType.GetProperty(part);

                Type valueGeted = info.GetValue(obj).GetType();
                if (!(valueGeted.IsPrimitive || valueGeted == typeof(Decimal) || valueGeted == typeof(String)))
                    obj = info.GetValue(obj);
            }

            info.SetValue(obj, value);
        }
    }
}
