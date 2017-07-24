﻿/**
 * Copyright(C) 2017  Apexol Technologies
 * 
 * This file (DotEntityDb.cs) is part of dotEntity(https://github.com/RoastedBytes/dotentity).
 * 
 * dotEntity is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 
 * dotEntity is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU Affero General Public License for more details.
 
 * You should have received a copy of the GNU Affero General Public License
 * along with dotEntity.If not, see<http://www.gnu.org/licenses/>.

 * You can release yourself from the requirements of the AGPL license by purchasing
 * a commercial license (dotEntity or dotEntity Pro). Buying such a license is mandatory as soon as you
 * develop commercial activities involving the dotEntity software without
 * disclosing the source code of your own applications. The activites include:
 * shipping dotEntity with a closed source product, offering paid services to customers
 * as an Application Service Provider.
 * To know more about our commercial license email us at support@roastedbytes.com or
 * visit http://dotentity.net/licensing
 */
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using DotEntity.Providers;

#if DEBUG
[assembly: InternalsVisibleTo("DotEntity.Tests")]
#endif
namespace DotEntity
{
    public partial class DotEntityDb
    {
        public static string ConnectionString { get; set; }

        private static ConcurrentDictionary<Type, string> EntityTableNames { get; set; }

        static DotEntityDb()
        {
            EntityTableNames = new ConcurrentDictionary<Type, string>();
            QueryProcessor = new QueryProcessor();
        }

        public static void Initialize(string connectionString, IDatabaseProvider provider)
        {
            ConnectionString = connectionString;
            Provider = provider;
            Provider.QueryGenerator = Provider.QueryGenerator ?? new DefaultQueryGenerator();
            Provider.TypeMapProvider = Provider.TypeMapProvider ?? new DefaultTypeMapProvider();
        }

        public static IDatabaseProvider Provider { get; internal set; }

        internal static QueryProcessor QueryProcessor { get; set; }

        public static void MapTableNameForType<T>(string tableName)
        {
            EntityTableNames.AddOrUpdate(typeof(T), tableName, (type, s) => tableName);
        }

        public static void Relate<TSource, TTarget>(string sourceColumnName, string destinationColumnName)
        {
            RelationMapper.Relate<TSource, TTarget>(sourceColumnName, destinationColumnName);
        }

        public static string GetTableNameForType<T>()
        {
            var tType = typeof(T);
            return GetTableNameForType(tType);
        }

        public static string GetTableNameForType(Type type)
        {
            return EntityTableNames.ContainsKey(type) ? EntityTableNames[type] : type.Name;
        }
    }
}