﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RepoDb.Exceptions;

namespace RepoDb
{
    /// <summary>
    /// A static class used to map the .NET CLR Types into database types.
    /// </summary>
    public static class TypeMapper
    {
        private static readonly IList<TypeMapItem> _typeMapItems = new List<TypeMapItem>();

        static TypeMapper()
        {
            new List<TypeMapItem>();
        }

        /// <summary>
        /// Gets the list of type-mapping objects.
        /// </summary>
        public static IEnumerable<TypeMapItem> TypeMaps => _typeMapItems;

        /// <summary>
        /// Adds a mapping between .NET CLR Type and database type.
        /// </summary>
        /// <param name="type">The .NET CLR Type to be mapped.</param>
        /// <param name="dbType">The database type where to map the .NET CLR Type.</param>
        public static void AddMap(Type type, DbType dbType)
        {
            AddMap(type, dbType, false);
        }

        /// <summary>
        /// Adds a mapping between .NET CLR Type and database type.
        /// </summary>
        /// <param name="type">The .NET CLR Type to be mapped.</param>
        /// <param name="dbType">The database type where to map the .NET CLR Type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void AddMap(Type type, DbType dbType, bool force = false)
        {
            AddMap(new TypeMapItem(type, dbType), force);
        }

        /// <summary>
        /// Adds a mapping between .NET CLR Type and database type.
        /// </summary>
        /// <param name="item">The instance of type-mapping object that holds the mapping of .NET CLR Type and database type.</param>
        public static void AddMap(TypeMapItem item)
        {
            AddMap(item, false);
        }

        /// <summary>
        /// Adds a mapping between .NET CLR Type and database type.
        /// </summary>
        /// <param name="item">The instance of type-mapping object that holds the mapping of .NET CLR Type and database type.</param>
        /// <param name="force">A value that indicates whether to force the mapping. If one is already exists, then it will be overwritten.</param>
        public static void AddMap(TypeMapItem item, bool force = false)
        {
            var target = Get(item.Type);
            if (target == null)
            {
                _typeMapItems.Add(item);
            }
            else
            {
                if (force == false)
                {
                    throw new DuplicateTypeMapException($"A mapping for type '{target.Type.FullName}' is already defined. It is currently mapped to '{target.DbType.GetType().FullName}' database type.");
                }
                else
                {
                    target.SetDbType(item.DbType);
                }
            }
        }

        /// <summary>
        /// Gets the instance of type-mapping object that holds the mapping of .NET CLR Type and database type.
        /// </summary>
        /// <param name="type">The .NET CLR Type used for mapping.</param>
        /// <returns>The instance of type-mapping object that holds the mapping of .NET CLR Type and database type.</returns>
        public static TypeMapItem Get(Type type)
        {
            return _typeMapItems.FirstOrDefault(t => t.Type == type);
        }

        /// <summary>
        /// Gets the instance of type-mapping object that holds the mapping of .NET CLR Type and database type.
        /// </summary>
        /// <typeparam name="T">The dynamic .NET CLR Type used for mapping.</typeparam>
        /// <returns>The instance of type-mapping object that holds the mapping of .NET CLR Type and database type.</returns>
        public static TypeMapItem Get<T>()
        {
            return Get(typeof(T));
        }

        /// <summary>
        /// Removes a mapping of targetted .NET CLR Type from the collection.
        /// </summary>
        /// <param name="type">The .NET CLR Type mapping to be removed.</param>
        public static void RemoveMap(Type type)
        {
            var item = Get(type);
            if (item == null)
            {
                throw new InvalidOperationException($"The type mapping for type '{type.FullName}' is not found.");
            }
            _typeMapItems.Remove(item);
        }
    }
}
