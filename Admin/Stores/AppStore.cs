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
        internal Branch AppBranch { get; set; }
        internal string ErrorMessage { get; set; }
        internal AppUser AppUser { get; set; }
        internal bool IsLoading { get; set; } = false;
        internal bool IsLoggedIn => AppUser != null;
        internal IList<string> AppUserRoles { get; set; } = new List<string>();
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
        //public override AppState Reduce(AppState state, LoginAction action) => new AppState();
        public override AppState Reduce(AppState state, LoginAction action)
        {
            var _state = new AppState
            {
                AppBranch = state.AppBranch,
                AppUser = state.AppUser,
                AppUserRoles = state.AppUserRoles,
                ErrorMessage = state.ErrorMessage,
                IsLoading = true
            };

            return _state;
        }
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

                var token = new JwToken(userInfo.Token);
                var user = new AppUser { Id = token.UserId, Name = userInfo.UserName };

                api.BranchId = token.BranchId;

                if(api.BranchId == Guid.Empty)
                {
                    var branchId = await localStorage.GetItem("branch.id");

                    if (!string.IsNullOrEmpty(branchId))
                    {
                        api.BranchId = new Guid(branchId);
                    }
                }

                Branch branch = null;

                if (api.BranchId != Guid.Empty) {
                    branch = new Branch { Id = api.BranchId };
                    await localStorage.SetItem("branch.id", api.BranchId.ToString());
                }


                dispatcher.Dispatch(new LoginSuccessAction(user, token.Roles, branch));

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
        public LoginSuccessAction(AppUser appUser, IList<string> appUserRoles, Branch appBranch)
        {
            AppUser = appUser;
            AppUserRoles = appUserRoles;
            AppBranch = appBranch;
        }

        public AppUser AppUser { get; private set; }
        public IList<string> AppUserRoles { get; private set; }
        public Branch AppBranch { get; private set; }
    }

    public class LoginSuccessReducer : Reducer<AppState, LoginSuccessAction>
    {
        public override AppState Reduce(AppState state, LoginSuccessAction action)
        {
            var _state = new AppState
            {
                AppBranch = action.AppBranch,
                AppUser = action.AppUser,
                AppUserRoles = action.AppUserRoles,
                ErrorMessage = null,
                IsLoading = false
            };

            return _state;
        }
    }

    // login failed
    public class LoginFailedAction : IAction
    {
        public LoginFailedAction(string error)
        {
            ErrorMessage = error;
        }

        public string ErrorMessage { get; private set; }
    }

    public class LoginFailedReducer : Reducer<AppState, LoginFailedAction>
    {
        public override AppState Reduce(AppState state, LoginFailedAction action)
        {
            var _state = new AppState
            {
                AppBranch = state.AppBranch,
                AppUser = state.AppUser,
                AppUserRoles = state.AppUserRoles,
                ErrorMessage = action.ErrorMessage,
                IsLoading = false
            };

            return _state;
        }
    }

    // check login
    public class CheckLoginAction : IAction { }

    public class CheckLoginReducer : Reducer<AppState, CheckLoginAction>
    {
        public override AppState Reduce(AppState state, CheckLoginAction action)
        {
            var _state = new AppState
            {
                AppBranch = state.AppBranch,
                AppUser = state.AppUser,
                AppUserRoles = state.AppUserRoles,
                ErrorMessage = state.ErrorMessage,
                IsLoading = true
            };

            return _state;
        }
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

                if(api.BranchId == Guid.Empty)
                {
                   var branchId = await localStorage.GetItem("branch.id");
                    if (!string.IsNullOrEmpty(branchId))
                    {
                        api.BranchId = new Guid(branchId);
                    }

                }

                var branchName = await localStorage.GetItem("branch.name");

                Branch branch = null;

                if(api.BranchId != Guid.Empty)
                {
                    branch = new Branch { Id = api.BranchId, Name = branchName };
                }

                var userName = await localStorage.GetItem("user.name");

                var user = new AppUser { Id = jwt.UserId, Name = userName };
            
                dispatcher.Dispatch(new LoginSuccessAction(user, jwt.Roles, branch));
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

            var _state = new AppState
            {
                AppBranch = action.Branch,
                AppUser = state.AppUser,
                AppUserRoles = state.AppUserRoles,
                ErrorMessage = state.ErrorMessage,
                IsLoading = false
            };

            return _state;
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
            await localStorage.RemoveItem("branch.id");
            await localStorage.RemoveItem("branch.name");
            await localStorage.RemoveItem("user.name");
            await localStorage.RemoveItem("token");
            api.BranchId = Guid.Empty;

            uriHelper.NavigateTo("/login");
        }
    }

    #endregion
}
