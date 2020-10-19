using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;

namespace BabylonTools.Wix.Setup.CustomAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult DeferredHello(Session session)
        {
            var data = session.CustomActionData;
            session.Log($"Hello { data["Name"] }");
            return ActionResult.Success;
        }
    }
}
