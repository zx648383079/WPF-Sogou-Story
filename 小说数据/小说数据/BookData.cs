using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 小说数据
{
    /// <summary>
    /// 小说目录的数据结构
    /// </summary>
    public class BookData
    {
        private string name;
        /// <summary>
        /// 书名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string writer;
        /// <summary>
        /// 小说作者
        /// </summary>
        public string Writer
        {
            get { return writer; }
            set { writer = value; }
        }


        private string kind;
        /// <summary>
        /// 小说分类
        /// </summary>
        public string Kind
        {
            get { return kind; }
            set { kind = value; }
        }


        private string count;
        /// <summary>
        /// 总章节
        /// </summary>
        public string Count
        {
            get { return count; }
            set { count = value; }
        }

        private string web;
        /// <summary>
        /// 来源个数
        /// </summary>
        public string Web
        {
            get { return web; }
            set { web = value; }
        }

        private string dateTime;

        public string DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }


        private string url;
        /// <summary>
        /// 网址
        /// </summary>
        public string Url
        {
            get { return url; }
            set { url = value; }
        }



    }
}
