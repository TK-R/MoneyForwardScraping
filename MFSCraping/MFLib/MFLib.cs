using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace MoneyForward
{
    public class MFLib
    {
        /// <summary>
        /// MoneyForwardのサインインURL
        /// </summary>
        const string LoginUrl = "https://moneyforward.com/users/sign_in";

        /// <summary>
        /// メインページのURL
        /// </summary>
        const string MainURL = "https://moneyforward.com/";

        /// <summary>
        /// MoneyForwardへのログインを試みる
        /// </summary>
        /// <param name="mail">メールアドレス</param>
        /// <param name="password">パスワード</param>
        /// <returns>取得したクッキー</returns>
        public static async Task<CookieContainer> LoginAsync(string mail, string password)
        {

            using (var handler = new HttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(10.0);
                    var content = new FormUrlEncodedContent(new Dictionary<string, string> {
                                                                                { "next_url", string.Empty },
                                                                                { "'user[email]", mail },
                                                                                { "'user[password]", password }});

                    await client.PostAsync(LoginUrl, content);
                    return handler.CookieContainer;
                }
            }
        }

        
        /// <summary>
        /// Coockieを使用してHTMLを取得する 
        /// </summary>
        /// <param name="uri">取得先のURL</param>
        /// <param name="cc">取得済みのCoockie</param>
        /// <returns>htmlソース</returns>
        static async Task<string> GetWebPageAsync(Uri uri, CookieContainer cc)
        {
            var handler = new HttpClientHandler();
            handler.CookieContainer = cc;

            using (HttpClient client = new HttpClient(handler))
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

        ///
        public static async Task<int> GetAllAsset(string id, string password)
        {
            var coockie = await LoginAsync(id, password);
            var html = await GetWebPageAsync(new Uri(MainURL), coockie);

            // HTMLをパースしたい



            return 0;
        }

    }
}
