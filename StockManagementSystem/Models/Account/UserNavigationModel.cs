using System.Collections.Generic;
using StockManagementSystem.Web.Models;

namespace StockManagementSystem.Models.Account
{
    public class UserNavigationModel : BaseModel
    {
        public UserNavigationModel()
        {
            UserNavigationItem = new List<UserNavigationItemModel>();
        }

        public IList<UserNavigationItemModel> UserNavigationItem { get; set; }

        public UserNavigationEnum SelectedTab { get; set; }
    }

    public class UserNavigationItemModel : BaseModel
    {
        public string RouteName { get; set; }
        public string Title { get; set; }
        public UserNavigationEnum Tab { get; set; }
        public string ItemClass { get; set; }
    }

    public enum UserNavigationEnum
    {
        Info = 0,
        ChangePassword = 70,
        Avatar = 80,
    }
}