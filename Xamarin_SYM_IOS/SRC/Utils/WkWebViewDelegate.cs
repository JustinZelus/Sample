using System;
using CoreGraphics;
using UIKit;
using Foundation;
using WebKit;
using CoreFoundation;

namespace Xamarin_SYM_IOS.SRC.Utils
{
    public class WkWebViewDelegate : WKNavigationDelegate
    {
        public WkWebViewDelegate()
        {
        }
        //    public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        //     {
        ////base.DecidePolicy(webView, navigationAction, decisionHandler);
        //Console.WriteLine("llliikknknknknk");
        //}

        public override void DidReceiveServerRedirectForProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {
            //base.DidReceivedServerRedirectForProvisionalNavigation(webView, navigation);
            Console.WriteLine("......DidReceiveServerRedirectForProvisionalNavigation");
        }
        public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {
            //base.DidStartProvisionalNavigation(webView, navigation);
            Console.WriteLine("......DidStartProvisionalNavigation");
        }

        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            //base.DidFinishNavigation(webView, navigation);
            Console.WriteLine("......DidFinishNavigation");
        }

        public override void DidCommitNavigation(WKWebView webView, WKNavigation navigation)
        {
            //base.DidCommitNavigation(webView, navigation);
            Console.WriteLine("......DidCommitNavigation");
        }
    }
}
