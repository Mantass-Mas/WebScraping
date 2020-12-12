using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraping.Model
{
    /// <summary>
    /// データベース用クラス
    /// </summary>
    public class Data
    {
        /// <summary>
        /// プライマリーキー
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// タイトル名
        /// </summary>
        public string Title { get; set; }
    }
}
