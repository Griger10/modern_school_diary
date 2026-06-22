using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Security;
using ModernSchoolDiary.Module.BusinessObjects;

namespace ModernSchoolDiary.Module.Controllers
{
    public class HideJournalForAdminController : WindowController
    {
        public HideJournalForAdminController()
        {
            TargetWindowType = WindowType.Main;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            var security = SecuritySystem.Instance as SecurityStrategy;
            bool isAdmin = security?.User is ApplicationUser user
                           && user.Roles.Any(r => r.IsAdministrative);

            if (!isAdmin)
                return;

            var navController = Frame.GetController<ShowNavigationItemController>();
            if (navController?.ShowNavigationItemAction != null)
            {
                RemoveItem(navController.ShowNavigationItemAction.Items, "Журнал");
            }
        }

        private void RemoveItem(System.Collections.Generic.IList<ChoiceActionItem> items, string caption)
        {
            var target = items.FirstOrDefault(i => i.Caption == caption);
            if (target != null)
                items.Remove(target);
        }
    }
}