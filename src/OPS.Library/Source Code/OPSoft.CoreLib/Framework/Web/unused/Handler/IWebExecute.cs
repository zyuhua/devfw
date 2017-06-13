﻿/*
 * name     :  IWebExecuteableAttribute
 * author   : Newmin
 * date     : 2010/11/05
 */

namespace Ops.Framework.Web.unused.Handler
{
    /// <summary>
    /// 能被WebHandler(.ashx文件)执行的特性接口
    /// </summary>
    public interface IWebExecute
    {
        /// <summary>
        /// 在执行前触发此方法
        /// </summary>
        void PreExecuting();
    }
}