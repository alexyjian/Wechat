﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Wodsoft.Wechat.Open
{
    /// <summary>
    /// 微信开放平台服务扩展。
    /// </summary>
    public static class OpenServiceExtensions
    {
        /// <summary>
        /// 二维码登录跳转。
        /// </summary>
        /// <param name="service">微信开放平台服务。</param>
        /// <param name="controller">Mvc控制器。</param>
        /// <param name="returnUrl">回调地址。</param>
        /// <returns>返回跳转结果。</returns>
        public static ActionResult QrSignIn(this OpenService service, Controller controller, string returnUrl)
        {
            var state = new Random().Next(100000000, 1000000000);
            controller.Session["wechatState"] = state;
            string url = service.GetQrSignInUrl(returnUrl, state.ToString());
            return new RedirectResult(url);
        }
    }
}
