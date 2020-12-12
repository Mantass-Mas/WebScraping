using System;
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
    public class DataBaseManager
    {
        private static readonly string _fileName = @"favorite.splite3";
        private readonly string _connectionString;

        public DataBaseManager()
        {
            Console.WriteLine("コンストラクタ起動");
            var builder = new SQLiteConnectionStringBuilder { DataSource =  _fileName};
            _connectionString = builder.ToString();
            using(var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using(var cmd = conn.CreateCommand())
                {
                    // Tableがなければ作成
                    Console.WriteLine("テーブル作成開始");
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS favorite (ID int PRIMARY KEY, Title string)";
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("テーブル作成終了");
                }
            }
            Console.WriteLine("コンストラクタ終了");
        }

        /// <summary>
        /// データベース書き込み用
        /// </summary>
        /// <param name="datas">登録するお気に入りリスト</param>
        public void DataBaseWrite(IReadOnlyList<Data> datas)
        {
            Console.WriteLine("データベース書き込み");
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                Console.WriteLine("データベース接続");
                using (var cmd = conn.CreateCommand())
                {
                    // データの追加(書き込み)
                    Console.WriteLine("データ書き込み開始");
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
                    Console.WriteLine("データ書き込み終了");
                }
            }
        }

        /// <summary>
        /// データベース読み込み用
        /// </summary>
        /// <returns>お気に入りデータ</returns>
        public List<Data> DataBaseRead()
        {
            Console.WriteLine("データベース読み込み");
            var list = new List<Data>();
            using(var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                Console.WriteLine("データベース接続");
                using (var cmd = conn.CreateCommand())
                {
                    Console.WriteLine("データ読み込み開始");
                    cmd.CommandText = @"SELECT * FROM favorite";
                    using(var reader = cmd.ExecuteReader())
                    {
                        while(reader.Read() == true)
                        {
                            int.TryParse(reader["ID"].ToString(), out int id);
                            var title = reader["Title"].ToString();
                            var data = new Data()
                            {
                                Id = id,
                                Title = title,
                            };
                            list.Add(data);
                        }
                    }
                    Console.WriteLine("データ読み込み終了");
                }
            }
            return list;
        }

        /// <summary>
        /// 単体削除
        /// </summary>
        /// <param name="id">削除するID</param>
        public void RemoveAt(int id)
        {
            Console.WriteLine("データ削除");
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                Console.WriteLine("データベース接続");
                using (var cmd = conn.CreateCommand())
                {
                    Console.WriteLine("削除処理開始");
                    cmd.CommandText = @"DELETE FROM favorite WHERE ID = @ID";
                    cmd.Parameters.Add(new SQLiteParameter("@ID", id));
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("削除処理終了");
                }
            }
        }

        /// <summary>
        /// 全削除
        /// </summary>
        public void RemoveAll()
        {
            Console.WriteLine("データ全削除");
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                Console.WriteLine("データベース接続");
                using (var cmd = conn.CreateCommand())
                {
                    Console.WriteLine("削除処理開始");
                    cmd.CommandText = @"DELETE FROM favorite";
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("削除処理終了");
                }
            }
        }
    }
}
