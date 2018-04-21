﻿using RepoDb.Attributes;
using System;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A factory class used for certains reflection activity.
    /// </summary>
    public static class ReflectionFactory
    {
        /// <summary>
        /// Creates a System.Reflection.ConstructorInfo object of the defined type.
        /// </summary>
        /// <param name="type">The System.Type object where to create a constructor info.</param>
        /// <returns>A System.Reflection.ConstructorInfo object of the defined type.</returns>
        public static ConstructorInfo GetConstructor(Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }

        /// <summary>
        /// Creates a System.Reflection.ConstructorInfo object of the defined type.
        /// </summary>
        /// <param name="type">The System.Type object  where to create a constructor info.</param>
        /// <param name="contructorTypes">The arguments of the constructor.</param>
        /// <returns>A System.Reflection.ConstructorInfo object of the defined type.</returns>
        public static ConstructorInfo GetConstructor(Type type, params Type[] contructorTypes)
        {
            return type.GetConstructor(contructorTypes);
        }

        /// <summary>
        /// Creates a System.Reflection.MethodInfo object based on type.
        /// </summary>
        /// <param name="type">A type of System.Reflection.MethodInfo object.</param>
        /// <returns>A System.Reflection.MethodInfo object.</returns>
        public static MethodInfo CreateMethod(MethodInfoTypes type)
        {
            var createMethodInfoAttribute = typeof(MethodInfoTypes)
                .GetMembers()
                .First(member => member.Name.ToLower() == type.ToString().ToLower())
                .GetCustomAttribute<CreateMethodInfoAttribute>();
            return TypeCache.Get(createMethodInfoAttribute.Type)
                .GetMethod(createMethodInfoAttribute.MethodName, createMethodInfoAttribute.ReflectedTypes);
        }

        /// <summary>
        /// Creates a Type based on type.
        /// </summary>
        /// <param name="type">The type of Type to be created.</param>
        /// <returns>A type object.</returns>
        public static Type CreateType(TypeTypes type)
        {
            switch (type)
            {
                case TypeTypes.DbDataReader:
                    return typeof(DbDataReader); // TODO: Why is it failing on a literal dynamic approach like below?
                default:
                    var textAttribute = typeof(TypeTypes)
                        .GetMembers()
                        .First(member => member.Name.ToLower() == type.ToString().ToLower())
                        .GetCustomAttribute<TextAttribute>();
                    return Type.GetType(textAttribute.Text);
            }
        }

        /// <summary>
        /// Creates an array of Types based on the type of passed.
        /// </summary>
        /// <param name="types">The type of Type array to be created.</param>
        /// <returns>An array of Types.</returns>
        public static Type[] CreateTypes(params TypeTypes[] types)
        {
            var length = Convert.ToInt32(types?.Length);
            var convertedTypes = new Type[length];
            for (var index = 0; index < length; index++)
            {
                convertedTypes[index] = TypeCache.Get(types[index]);
            }
            return convertedTypes;
        }
    }
}
