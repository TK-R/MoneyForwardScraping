using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using HtmlAgilityPack;

namespace MoneyForward
{
    public class MFLib
    { 
        #region Param
        /// <summary>
        /// MoneyForwardのクッキー情報
        /// </summary>
        CookieContainer MFCoockie { set; get; }

        /// <summary>
        /// MoneyForwardのサインインURL
        /// </summary>
        const string LoginUrl = "https://moneyforward.com/users/sign_in/";

        /// <summary>
        /// メインページのURL
        /// </summary>
        const string MainURL = "https://moneyforward.com/";
        #endregion

        /// <summary>
        /// MoneyForwardへのログインを試みる
        /// </summary>
        /// <param name="mail">メールアドレス</param>
        /// <param name="password">パスワード</param>
        public async Task<bool> LoginAsync(string mail, string password)
        {
            // サインイン画面からauthenticity_tokenを取得　
            var html = await GetWebPageAsync(new Uri(MainURL), null);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var nodes = htmlDoc.DocumentNode.SelectNodes(@"//input[@name=""authenticity_token""]");
            var authToken = nodes.First().Attributes["value"].Value;

            using (var handler = new HttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {

                    // ユーザーエージェント文字列をセット（オプション）
                    client.DefaultRequestHeaders.Add(
                        "User-Agent",
                        "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko");

                    // 受け入れ言語をセット（オプション）
                    client.DefaultRequestHeaders.Add("Accept-Language", "ja-JP");

                    client.Timeout = TimeSpan.FromSeconds(10.0);
                    var content = new FormUrlEncodedContent(new Dictionary<string, string> {
                        { "utf8", "✓"},
                        { "authenticity_token", authToken },
                        { "sign_in_session_service[email]", mail },
                        { "sign_in_session_service[password]", password },
                        { "commit",  "ログイン"}
                    });
                    await client.PostAsync(LoginUrl, content);
                    MFCoockie = handler.CookieContainer;
                    return true;
                }
            }
        }

        /// <summary>
        /// Coockieを使用してHTMLを取得する 
        /// </summary>
        /// <param name="uri">取得先のURL</param>
        /// <param name="cc">取得済みのCoockie</param>
        /// <returns>htmlソース</returns>
        async Task<string> GetWebPageAsync(Uri uri, CookieContainer cookie)
        {
            // Cookieがnullでなければ使用する
            using (var handler = (cookie != null ?
                   new HttpClientHandler() { CookieContainer = cookie } :
                   new HttpClientHandler()))
            using (var client = new HttpClient(handler))
            {
                client.Timeout = TimeSpan.FromSeconds(10.0);

                try
                {
                    return await client.GetStringAsync(uri);
                }
                catch (HttpRequestException e)
                {
                    // 404エラーや、名前解決失敗など
                    Console.WriteLine("\n例外発生!");
                    // InnerExceptionも含めて、再帰的に例外メッセージを表示する
                    Exception ex = e;
                    while (ex != null)
                    {
                        Console.WriteLine("例外メッセージ: {0} ", ex.Message);
                        ex = ex.InnerException;
                    }
                }
                catch (TaskCanceledException e)
                {
                    Console.WriteLine("\nタイムアウト!");
                    Console.WriteLine("例外メッセージ: {0} ", e.Message);
                }
                return null;
            }
        }


        /// <summary>
        /// 総資産の値を返す
        /// </summary>
        /// <returns>***,***円</returns>
        public async Task<string> GetAllAsset()
        {
            if (MFCoockie == null)
                return null;

            var htmlString = await GetWebPageAsync(new Uri(MainURL), MFCoockie);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlString);

            return htmlDoc.DocumentNode.SelectNodes(@"//div[@class=""heading-radius-box""]").First().InnerText;

        }


        /// <summary>
        /// 総資産の値と前日比を返す
        /// </summary>
        /// <returns>***円, ***%</returns>
        public async Task<string> GetAllAssetWithDiff()
        {
            if(MFCoockie == null)
                return null;

            var htmlString = await GetWebPageAsync(new Uri(MainURL), MFCoockie);

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlString);
            
            return htmlDoc.DocumentNode.SelectNodes(@"//p[@class=""number heading-radius-box""]").First().InnerText;
        }
    }
}
