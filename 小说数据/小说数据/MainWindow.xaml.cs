using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 小说数据
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string pageKind;   //下一页的标记
        int page;          //记录页码
        string baseurl;   //记录基网址，用于获取完整网址
        string[] regexs=new string[6];  //收集所有的正则表达式

        private void SoBtn_Click(object sender, RoutedEventArgs e)
        {
            BookList.Items.Clear();      //清空列表
            page = 1;                    //实现加载更多

            switch (NameTb.Text)
            {
                case "最热榜":
                    GetBangList("top?channelName=novel_top_zuirebang");
                    break;
                case "飞升榜":
                    GetBangList("top?channelName=novel_top_feishengbang");
                    break;
                case "起点榜":
                    GetBangList("cptop?channelName=cp_top_qidian");
                    break;
                case "红袖榜":
                    GetBangList("cptop?channelName=cp_top_hongxiu");
                    break;
                case "纵横榜":
                    GetBangList("cptop?channelName=cp_top_zongheng");
                    break;
                case "都市类":
                    GetBangList("category_detail?channelName=novel_category_dushi");
                    break;
                case "玄幻类":
                    GetBangList("category_detail?channelName=novel_category_xuanhuan");
                    break;
                case "仙侠类":
                    GetBangList("category_detail?channelName=novel_category_xianxia");
                    break;
                case "武侠类":
                    GetBangList("category_detail?channelName=novel_category_wuxia");
                    break;
                case "军事类":
                    GetBangList("category_detail?channelName=novel_category_junshi");
                    break;
                case "历史类":
                    GetBangList("category_detail?channelName=novel_category_lishi");
                    break;
                case "游戏类":
                    GetBangList("category_detail?channelName=novel_category_youxi");
                    break;
                case "科幻类":
                    GetBangList("category_detail?channelName=novel_category_kehuan");
                    break;

                default:
                    #region 直接搜索书名
                    if (NameTb.Text.Trim()!="")
                    {
                        NameTb.Items.Add(NameTb.Text);         //加入历史纪录

                        baseurl = "http://k.sogou.com/search?keyword="+
                                    Regex.Replace(NameTb.Text,@"\s+","+");  //实现搜索
                        regexs[0] = @"\<ul.+list.+\>(?<content>[\s\S]+?)\</ul\>";
                        regexs[1] = @"href.+?\>(?<name>.+?)\</a\>";
                        regexs[2] = @"green""\>(?<date>.+?)\<";
                        regexs[3] = @"info""\>\s*(?<count>[^\<\s]+)\s*\<";
                        regexs[4] = @"所有\<b\>(?<web>\d+)\</b\>个";
                        regexs[5] = @"href="".+(?<url>md=[^&]+).+""\>";

                        pageKind = "p";

                        SetRegex(baseurl);
                    }
                    else
                    {
                        MessageBox.Show("请输入小说名！", "提示");
                    }
                    #endregion
                    break;
            }
            
        }

        private void BookList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (BookList.SelectedIndex>=0 && BookList.SelectedIndex<BookList.Items.Count)
            {

                BookData book = BookList.SelectedItem as BookData;     //选中项还原
                if (book.Url !="" && book.Url !=null)
                {
                    //调用系统默认浏览器打开网页。
                    System.Diagnostics.Process.Start(book.Url);
                }
                else
                {
                    //如果没有网址表明是从表单获取的数据
                    NameTb.Text = book.Name;
                }
                
            }
            
        }

        private void PageBtn_Click(object sender, RoutedEventArgs e)
        {
            page++;
            SetRegex(string.Format("{0}&{1}={2}", baseurl, pageKind, page));
        }
        /// <summary>
        /// 获取榜单
        /// </summary>
        void GetBangList(string name)
        {
            baseurl = "http://k.sogou.com/"+name;

            regexs[0] = @"con[\s\S]+?\<ul\>(?<content>[\s\S]+?)\</ul\>";
            regexs[1] = @"\d\.(?<name>[\s\S]+)\</a\>";

            pageKind = "pageNo";

            SetRegex(baseurl);
        }

        /// <summary>
        /// 把正则法则赋值
        /// </summary>
        /// <param name="url"></param>
        void SetRegex(string url)
        {
            //避免出现为负值报错的情况
            for (int i = 0; i < regexs.Length; i++)
            {
                if (regexs[i] ==null)
                {
                    regexs[i] = "";
                }
            }
            GetList(url, regexs[0], regexs[1], regexs[2], regexs[3], regexs[4], regexs[5]);
        }

        /// <summary>
        /// 获取源代码
        /// </summary>
        /// <param name="url">网址</param>
        /// <returns>源代码</returns>
        string GetHtml(string url)
        {
            HttpClient http = new HttpClient();
            byte[] bytes = http.GetByteArrayAsync(url).Result;            //获取源码
            string html = Encoding.UTF8.GetString(bytes);

            string pattern = @"charset=[""]{0,1}(?<encod>[^""]*?)""";

            string encod = "UTF8";
            //编码检测，更改解码方式
            if (Regex.IsMatch(html, pattern))
            {
                encod = Regex.Match(html, pattern).Groups["encod"].Value;
                html = Encoding.GetEncoding(encod).GetString(bytes);
            }

            //查看是否有更多数据
            if (Regex.IsMatch(html, @"page[\s\S]+?下一页\</a\>"))
            {
                PageBtn.IsEnabled = true;
            }
            else
            {
                PageBtn.IsEnabled = false;
            }

            return html;
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="html">网页源码</param>
        /// <param name="contentRegex">缩小范围</param>
        /// <param name="nameRegex">书名正则</param>
        /// <param name="dateRegex">更新时间正则</param>
        /// <param name="countRegex">章节数正则</param>
        /// <param name="webRegex">来源个数正则</param>
        /// <param name="urlRegex">网址的正则</param>
        void GetList(string url,string contentRegex,string nameRegex,string dateRegex="",string countRegex="",string webRegex="",string urlRegex="")
        {


            string html = GetHtml(url);

            //提取列表总数据
            string content = Regex.Match(html, contentRegex).Groups["content"].Value;
            //去除被注释的部分
            content = Regex.Replace(content, @"\<!--[\s\S]+?--\>", "");
            //提取li标签
            MatchCollection ms = Regex.Matches(content, @"\<li\>[\s\S]+?\</li\>");// @"\<li[\s\S]+?\<a.href=""(?<url>.+?)""\>(?<name>.+?)\</a\>[\s\S]+\>\s*(?<count>[^\<\s]+)\s*\<[\s\S]+green""\>(?<date>.+?)\<[\S\s]+所有\<b\>(?<web>\d+)\</b\>个[\s\S]+?\</li\>");

            foreach (Match item in ms)
            {
                //Match m = Regex.Match(item.Value, @"\<a.href="".+(?<url>md=[^&]+).+""\>(?<name>.+?)\</a\>[\s\S]+\>\s*(?<count>[^\<\s]+)\s*\<[\s\S]+green""\>(?<date>.+?)\<[\S\s]+所有\<b\>(?<web>\d+)\</b\>个");
                BookData book = new BookData();
                //提取书名
                string tem1 = Regex.Replace(Regex.Replace(Regex.Match(item.Value, nameRegex).Groups["name"].Value,@"\s*",""), @"\<.+?\>", "");

                book.Kind = Regex.Match(tem1, @"\[(?<kind>.+?)\]").Groups["kind"].Value;
                //去掉分类信息
                string tem2 = Regex.Replace(tem1, @"\[.+?\]","");
                //分别提取书名和作者
                Match match = Regex.Match(tem2, @"(?<name>.+?)-(?<writer>[^-]+)");
                book.Name = match.Groups["name"].Value;
                book.Writer = match.Groups["writer"].Value;

                //提取最近更新时间
                book.DateTime = Regex.Match(item.Value, dateRegex).Groups["date"].Value;
                //提取书的章数
                book.Count = Regex.Match(item.Value, countRegex).Groups["count"].Value;
                //提取来源的个数，但有些只有一个来源，需进行判断
                //string web = "0";
                //if (Regex.IsMatch(item.Value, webRegex))
                //{
                //    web = Regex.Match(item.Value, webRegex).Groups["web"].Value;
                //}
                book.Web = Regex.Match(item.Value, webRegex).Groups["web"].Value;
                //提取网址中的 md=
                string md=Regex.Match(item.Value, urlRegex).Groups["url"].Value;
                if (md!="")
                {
                    book.Url = "http://k.sogou.com/list?" + md;
                }
                
                BookList.Items.Add(book);
            }
        }
        
    }
}
