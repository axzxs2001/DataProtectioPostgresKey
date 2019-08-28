using Microsoft.AspNetCore.DataProtection.Repositories;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace DataProtectioPostgresPersist
{
    /// <summary>
    /// Data Protection Postgres Extension
    /// </summary>
    public class DataProtectionKeyRepository : IXmlRepository
    {
        /// <summary>
        /// postgres connection string
        /// </summary>
        private readonly string _connectionString;
        public DataProtectionKeyRepository(string connectionString)
        {
            _connectionString = connectionString;

        }
        /// <summary>
        /// get all elements
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return new ReadOnlyCollection<XElement>(GetXElements());
        }
        /// <summary>
        /// store element
        /// </summary>
        /// <param name="element">element</param>
        /// <param name="friendlyName">FriendlyName</param>
        public void StoreElement(XElement element, string friendlyName)
        {
            var entity = GetElement(friendlyName);
            if (null != entity)
            {
                entity.XmlData = element.ToString();
                Update(entity);
            }
            else
            {
                Add(new DataProtectionKey
                {
                    FriendlyName = friendlyName,
                    XmlData = element.ToString()
                });
            }
        }

        #region postgres operation
        /// <summary>
        /// create DataProtectionKeys
        /// </summary>
        void CreateTable()
        {
            using (var con = new Npgsql.NpgsqlConnection(_connectionString))
            {
                var sql = @"CREATE TABLE if not exists public.""DataProtectionKeys""
 (
     ""FriendlyName"" character varying(256) COLLATE pg_catalog.""default"" NOT NULL,
     ""XmlData"" text COLLATE pg_catalog.""default"",
     CONSTRAINT ""DataProtectionKeys_pkey"" PRIMARY KEY(""FriendlyName"")
 )";
                using (var cmd = new Npgsql.NpgsqlCommand(sql, con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
        }
        /// <summary>
        /// add dataProtectionKey
        /// </summary>
        /// <param name="dataProtectionKey">Data Protection Key</param>
        /// <returns></returns>
        bool Add(DataProtectionKey dataProtectionKey)
        {
            using (var con = new Npgsql.NpgsqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO public.""DataProtectionKeys""(""FriendlyName"", ""XmlData"")  VALUES(@FriendlyName, @XmlData);";
                using (var cmd = new Npgsql.NpgsqlCommand(sql, con))
                {
                    cmd.Parameters.Add(new Npgsql.NpgsqlParameter("@FriendlyName", dataProtectionKey.FriendlyName));
                    cmd.Parameters.Add(new Npgsql.NpgsqlParameter("@XmlData", dataProtectionKey.XmlData));
                    con.Open();
                    var result = cmd.ExecuteNonQuery() > 0;
                    con.Close();
                    return result;
                }
            }
        }
        /// <summary>
        /// get DataProtectionKey by FriendlyName
        /// </summary>
        /// <param name="friendlyName">Friendly Name</param>
        /// <returns></returns>
        DataProtectionKey GetElement(string friendlyName)
        {
            using (var con = new Npgsql.NpgsqlConnection(_connectionString))
            {
                var sql = @"select ""FriendlyName"",""XmlData"" from public.""DataProtectionKeys"" where  ""FriendlyName""=@FriendlyName;";
                using (var cmd = new Npgsql.NpgsqlCommand(sql, con))
                {
                    DataProtectionKey dataProtectionKey = null;
                    cmd.Parameters.Add(new Npgsql.NpgsqlParameter("@FriendlyName", friendlyName));
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dataProtectionKey = new DataProtectionKey();
                            dataProtectionKey.FriendlyName = reader.GetString(0);
                            dataProtectionKey.XmlData = reader.GetString(1);
                        }
                        reader.Close();
                    }
                    con.Close();
                    return dataProtectionKey;
                }
            }
        }
        /// <summary>
        /// get XmlData list
        /// </summary>
        /// <returns></returns>
        IList<XElement> GetXElements()
        {
            CreateTable();
            using (var con = new Npgsql.NpgsqlConnection(_connectionString))
            {
                var sql = @"select ""XmlData"" from public.""DataProtectionKeys"" ";
                using (var cmd = new Npgsql.NpgsqlCommand(sql, con))
                {
                    var elements = new List<XElement>();
                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            elements.Add(XElement.Parse(reader.GetString(0)));
                        }
                        reader.Close();
                    }
                    con.Close();
                    return elements;
                }
            }
        }

        bool Update(DataProtectionKey dataProtectionKey)
        {
            using (var con = new Npgsql.NpgsqlConnection(_connectionString))
            {
                var sql = @"update public.""DataProtectionKeys"" set ""XmlData""=@XmlData where  ""FriendlyName""=@FriendlyName;";
                using (var cmd = new Npgsql.NpgsqlCommand(sql, con))
                {
                    cmd.Parameters.Add(new Npgsql.NpgsqlParameter("@FriendlyName", dataProtectionKey.FriendlyName));
                    cmd.Parameters.Add(new Npgsql.NpgsqlParameter("@XmlData", dataProtectionKey.XmlData));
                    con.Open();
                    var result = cmd.ExecuteNonQuery() > 0;
                    con.Close();
                    return result;
                }
            }
        }
        #endregion
    }
}
