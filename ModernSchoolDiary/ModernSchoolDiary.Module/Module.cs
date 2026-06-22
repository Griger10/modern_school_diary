using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.StateMachine;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF.StateMachine;
using ModernSchoolDiary.Module.Domain.Models;
using ModernSchoolDiary.Module.Reports;
using System.ComponentModel;

namespace ModernSchoolDiary.Module
{
    
    public sealed class ModernSchoolDiaryModule : ModuleBase
    {
        public ModernSchoolDiaryModule()
        {
            AdditionalExportedTypes.Add(typeof(ModernSchoolDiary.Module.BusinessObjects.ApplicationUser));
            AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.PermissionPolicy.PermissionPolicyRole));
            AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.ModelDifference));
            AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.ModelDifferenceAspect));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Security.SecurityModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.AuditTrail.EFCore.AuditTrailModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Chart.ChartModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Dashboards.DashboardsModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Notifications.NotificationsModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Office.OfficeModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.PivotGrid.PivotGridModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ReportsV2.ReportsModuleV2));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.StateMachine.StateMachineModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule));
            DevExpress.ExpressApp.Security.SecurityModule.UsedExportedTypes = DevExpress.Persistent.Base.UsedExportedTypes.Custom;
            AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.FileData));
            AdditionalExportedTypes.Add(typeof(DevExpress.Persistent.BaseImpl.EF.FileAttachment));
            AdditionalExportedTypes.Add(typeof(StateMachine));
            AdditionalExportedTypes.Add(typeof(StateMachineTransition));
            AdditionalExportedTypes.Add(typeof(StateMachineAppearance));
            AdditionalExportedTypes.Add(typeof(StateMachineState));
        }
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
        {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            PredefinedReportsUpdater reportsUpdater = new PredefinedReportsUpdater(Application, objectSpace, versionFromDB);
            reportsUpdater.AddPredefinedReport<ModernSchoolDiary.Module.Reports.GradesReport>(
                "Ведомость успеваемости ученика",
                typeof(ModernSchoolDiary.Module.Domain.Models.Grade),
                isInplaceReport: false);
            reportsUpdater.AddPredefinedReport<ClassListReport>("Список класса", typeof(Student), isInplaceReport: false);
            return new ModuleUpdater[] { updater, reportsUpdater };
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
        }
        public override void Setup(ApplicationModulesManager moduleManager)
        {
            base.Setup(moduleManager);
            StateMachineModule stateMachineModule = moduleManager.Modules.FindModule<StateMachineModule>();
            stateMachineModule.StateMachineStorageType = typeof(StateMachine);
        }
    }
}
