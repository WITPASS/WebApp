using Blazor.Fluxor;
using Data;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Admin.Store
{
    public class LoginState
    {
        internal UserInfo UserInfo { get; set; }
        internal bool LoggedIn => UserInfo != null;
        internal JwToken JwToken (){
            var token = UserInfo.Token;
            token = token.Split('.')[1];

            int mod4 = token.Length % 4;

            if (mod4 > 0)
            {
                token += new string('=', 4 - mod4);
            }

            byte[] data = Convert.FromBase64String(token);
            string decoded = Encoding.UTF8.GetString(data);
            var jwt = JObject.Parse(decoded);
            return new JwToken(jwt);
        }

        public LoginState(UserInfo userInfo) => UserInfo = userInfo;
        public LoginState() {
        }
    }

    public class ActionLogin : IAction
    {
        public ActionLogin(UserInfo userInfo)
        {
            UserInfo = userInfo;
        }
        internal UserInfo UserInfo { get; set; } = null;
    }
    public class ActionLogout : IAction { }

    public class ReducerLogin : Reducer<LoginState, ActionLogin>
    {
        public override LoginState Reduce(LoginState state, ActionLogin action) => new LoginState(action.UserInfo);
    }
    public class ReducerLogout : Reducer<LoginState, ActionLogout>
    {
        public override LoginState Reduce(LoginState state, ActionLogout action) => new LoginState();
    }
}
