﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.Data.SQLite.Linq;

namespace Scraping.Model
{
    /// <summary>
    /// データベース管理用クラス
    /// </summary>
    class DataBaseManager
    {
        private static readonly string _fileName = @"favorite.splite3";
        private readonly string _connectionString;

        public DataBaseManager()
        {
            var builder = new SQLiteConnectionStringBuilder { DataSource =  _fileName};
            _connectionString = builder.ToString();
        }

        /// <summary>
        /// データベース書き込み用
        /// </summary>
        /// <param name="datas">登録するお気に入りリスト</param>
        public void DataBaseWrite(IReadOnlyList<Data> datas)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    // Tableがなければ作成
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS favorite (ID int PRIMARY KEY, Title string)";
                    cmd.ExecuteNonQuery();

                    // データの追加(書き込み)
                    cmd.CommandText = @"SELECT * FROM favorite";
                    var list = new List<int>();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read() == true)
                        {
                            int.TryParse(reader["ID"].ToString(), out int id);
                            list.Add(id);
                        }
                    }
                    foreach (var data in datas)
                    {
                        if (!(list.Contains(data.Id)))
                        {
                            cmd.CommandText = @"INSERT INTO favorite (ID, Title) VALUES (@ID, @Title)";
                            cmd.Parameters.Add(new SQLiteParameter("@ID", data.Id));
                            cmd.Parameters.Add(new SQLiteParameter("@Title", data.Title));
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}