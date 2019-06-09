using Admin.Services;
using Blazor.Fluxor;
using Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admin.Stores
{
    public class AppStateFeature : Feature<AppState>
    {
        public override string GetName() => "App";
        protected override AppState GetInitialState() => new AppState();
    }

    public class AppState
    {
        internal bool LoggingIn { get; private set; }
        internal string Error { get; private set; }
        internal UserInfo UserInfo { get; private set; }
        internal bool LoggedIn => UserInfo != null;
        internal Branch Branch { get; set; }
        internal IList<string> Roles { get; private set; } = new List<string>();

        public AppState() : this(false, null, null) { }
        public AppState(bool loggingIn, string error, UserInfo userInfo)
        {
            Error = error;
            LoggingIn = loggingIn;

            if (userInfo != null)
            {
                var token = new JwToken(userInfo.Token);

                UserInfo = userInfo;
                Roles = token.Roles;

                if (token.BranchId != Guid.Empty)
                {
                    Branch = new Branch { Id = token.BranchId, Name = userInfo.BranchName };
                }

            }
        }
    }

    #region Login
    // login
    public class LoginAction : IAction
    {
        public LoginAction(LoginInfo loginInfo)
        {
            LoginInfo = loginInfo;
        }

        public LoginInfo LoginInfo { get; private set; }
    }

    public class LoginReducer : Reducer<AppState, LoginAction>
    {
        public override AppState Reduce(AppState state, LoginAction action) => new AppState();
    }

    public class LoginEffect : Effect<LoginAction>
    {
        private readonly ApiService api;
        private readonly LocalStorageService localStorage;
        private readonly IUriHelper uriHelper;

        public LoginEffect(ApiService api, LocalStorageService localStorage, IUriHelper uriHelper)
        {
            this.api = api;
            this.localStorage = localStorage;
            this.uriHelper = uriHelper;
        }

        protected async override Task HandleAsync(LoginAction action, IDispatcher dispatcher)
        {
            var userInfo = await api.PostAsyncUnauthorized<UserInfo, LoginInfo>("api/users/login", action.LoginInfo);

            if (userInfo != null)
            {
                await localStorage.SetItem("branch.name", userInfo.BranchName);
                await localStorage.SetItem("user.name", userInfo.UserName);
                await localStorage.SetItem("token", userInfo.Token);
                api.BranchId = new JwToken(userInfo.Token).BranchId;
                dispatcher.Dispatch(new LoginSuccessAction(userInfo));
                uriHelper.NavigateTo("/");
            }
            else
            {
                dispatcher.Dispatch(new LoginFailedAction("email or password is incorrect"));
            }
        }
    }

    // login success
    public class LoginSuccessAction : IAction
    {
        public LoginSuccessAction(UserInfo userInfo)
        {
            UserInfo = userInfo;
        }

        public UserInfo UserInfo { get; private set; }
    }

    public class LoginSuccessReducer : Reducer<AppState, LoginSuccessAction>
    {
        public override AppState Reduce(AppState state, LoginSuccessAction action) => new AppState(false, null, action.UserInfo);
    }

    // login failed
    public class LoginFailedAction : IAction
    {
        public LoginFailedAction(string error)
        {
            Error = error;
        }

        public string Error { get; private set; }
    }

    public class LoginFailedReducer : Reducer<AppState, LoginFailedAction>
    {
        public override AppState Reduce(AppState state, LoginFailedAction action) => new AppState(false, action.Error, null);
    }

    // check login
    public class CheckLoginAction : IAction { }

    public class CheckLoginReducer : Reducer<AppState, CheckLoginAction>
    {
        public override AppState Reduce(AppState state, CheckLoginAction action) => new AppState();
    }

    public class CheckLoginEffect : Effect<CheckLoginAction>
    {
        private readonly ApiService api;
        private readonly LocalStorageService localStorage;
        private readonly IUriHelper uriHelper;

        public CheckLoginEffect(ApiService api, LocalStorageService localStorage, IUriHelper uriHelper)
        {
            this.api = api;
            this.localStorage = localStorage;
            this.uriHelper = uriHelper;
        }

        protected async override Task HandleAsync(CheckLoginAction action, IDispatcher dispatcher)
        {
            var token = await localStorage.GetItem("token");

            if (token != null)
            {
                var jwt = new JwToken(token);
                api.BranchId = jwt.BranchId;
                var branch = await localStorage.GetItem("branch.name");
                var user = await localStorage.GetItem("user.name");
                var userInfo = new UserInfo { Token = token, UserName = user, BranchName = branch };

                dispatcher.Dispatch(new LoginSuccessAction(userInfo));
            }
            else
            {
                uriHelper.NavigateTo("/login");
            }
        }
    }

    // set branch
    public class SetBranchAction : IAction
    {
        public SetBranchAction(Branch branch)
        {
            Branch = branch;
        }

        public Branch Branch { get; private set; }
    }

    public class SetBranchReducer: Reducer<AppState, SetBranchAction>
    {
        ApiService api;
        public SetBranchReducer(ApiService api)
        {
            this.api = api;
        }
        public override AppState Reduce(AppState state, SetBranchAction action)
        {
            api.BranchId = action.Branch.Id;

            var st = new AppState(false, null, state.UserInfo)
            {
                Branch = action.Branch
            };

            return st;
        }
    }

    #endregion

    #region Logout

    public class LogoutAction : IAction { }

    public class LogoutReducer : Reducer<AppState, LogoutAction>
    {
        public override AppState Reduce(AppState state, LogoutAction action) => new AppState();
    }

    public class LogoutEffect : Effect<LogoutAction>
    {
        private readonly ApiService api;
        private readonly LocalStorageService localStorage;
        private readonly IUriHelper uriHelper;

        public LogoutEffect(ApiService api, LocalStorageService localStorage, IUriHelper uriHelper)
        {
            this.api = api;
            this.localStorage = localStorage;
            this.uriHelper = uriHelper;
        }

        protected async override Task HandleAsync(LogoutAction action, IDispatcher dispatcher)
        {
            await localStorage.RemoveItem("branch.name");
            await localStorage.RemoveItem("user.name");
            await localStorage.RemoveItem("token");
            api.BranchId = Guid.Empty;

            uriHelper.NavigateTo("/login");
        }
    }

    #endregion
}
