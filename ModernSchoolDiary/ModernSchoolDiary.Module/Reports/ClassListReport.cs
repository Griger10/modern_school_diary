using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.XtraReports.UI;
using System;
using System.Drawing;

namespace ModernSchoolDiary.Module.Reports
{
    public partial class ClassListReport : XtraReport
    {
        public ClassListReport()
        {
            InitializeComponent();

            if (ModuleHelper.IsDesignMode)
                return;

            this.Bands.Clear();

            DataSource = new CollectionDataSource
            {
                ObjectTypeName = "ModernSchoolDiary.Module.Domain.Models.Student"
            };
            DataMember = string.Empty;

            var classParam = new DevExpress.XtraReports.Parameters.Parameter
            {
                Name = "ClassParam",
                Description = "Класс:",
                Type = typeof(Guid),
                Visible = true
            };
            classParam.ValueSourceSettings = new DevExpress.XtraReports.Parameters.DynamicListLookUpSettings
            {
                DataSource = new CollectionDataSource
                {
                    ObjectTypeName = "ModernSchoolDiary.Module.Domain.Models.SchoolClass"
                },
                ValueMember = "Id",
                DisplayMember = "Name",
                FilterString = "IsCurrentUserInRole('Администраторы') " +
                               "Or ClassTeacher.LinkedUser.ID = CurrentUserId()"
            };
            Parameters.Add(classParam);
            RequestParameters = true;

            FilterString = "[SchoolClass.Id] = ?ClassParam";

            var topMargin = new TopMarginBand { HeightF = 50 };
            var bottomMargin = new BottomMarginBand { HeightF = 50 };

            var reportHeader = new ReportHeaderBand { HeightF = 70 };

            var title = MakeLabel(0, 5, 640, 30, "Список класса");
            title.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
            title.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            var lblClass = MakeExprLabel(0, 38, 640, 22, "'Класс: ' + [SchoolClass.Name]");
            lblClass.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblClass.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;

            reportHeader.Controls.AddRange(new XRControl[] { title, lblClass });

            var pageHeader = new PageHeaderBand { HeightF = 28 };
            var hNum = MakeCell(0, 0, 35, 28, "№");
            var hName = MakeCell(35, 0, 285, 28, "ФИО");
            var hEmail = MakeCell(320, 0, 160, 28, "Email");
            var hSign = MakeCell(480, 0, 160, 28, "Подпись");
            foreach (var h in new[] { hNum, hName, hEmail, hSign })
            {
                h.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
                h.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            }
            pageHeader.Controls.AddRange(new XRControl[] { hNum, hName, hEmail, hSign });

            var detail = new DetailBand { HeightF = 26 };

            var cellNum = MakeCell(0, 0, 35, 26, null);
            cellNum.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            cellNum.ExpressionBindings.Add(new ExpressionBinding(
                "BeforePrint", "Text", "[DataSource.CurrentRowIndex] + 1"));

            var cellName = MakeCell(35, 0, 285, 26, null);
            cellName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            cellName.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 0, 0, 0);
            cellName.ExpressionBindings.Add(new ExpressionBinding(
                "BeforePrint", "Text",
                "Trim(Concat(ToStr([LastName]), ' ', ToStr([FirstName]), ' ', ToStr([FatherName])))"));

            var cellEmail = MakeCell(320, 0, 160, 26, null);
            cellEmail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            cellEmail.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 0, 0, 0);
            cellEmail.ExpressionBindings.Add(new ExpressionBinding(
                "BeforePrint", "Text", "[Email]"));

            var cellSign = MakeCell(480, 0, 160, 26, null);

            detail.Controls.AddRange(new XRControl[] { cellNum, cellName, cellEmail, cellSign });
            detail.SortFields.Add(new GroupField("LastName", XRColumnSortOrder.Ascending));

            var reportFooter = new ReportFooterBand { HeightF = 30 };

            var lblCount = MakeLabel(0, 6, 400, 22, null);
            lblCount.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            lblCount.ExpressionBindings.Add(new ExpressionBinding(
                "BeforePrint", "Text", "'Всего учеников: ' + [DataSource.RowCount]"));
            lblCount.ExpressionBindings.Add(new ExpressionBinding(
                "BeforePrint", "Visible", "[DataSource.RowCount] > 0"));

            var lblNoData = MakeLabel(0, 6, 640, 22, "В выбранном классе нет учеников");
            lblNoData.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblNoData.ForeColor = Color.Gray;
            lblNoData.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            lblNoData.ExpressionBindings.Add(new ExpressionBinding(
                "BeforePrint", "Visible", "[DataSource.RowCount] == 0"));

            reportFooter.Controls.AddRange(new XRControl[] { lblCount, lblNoData });

            string hasData = "[DataSource.RowCount] > 0";
            reportHeader.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Visible", hasData));
            pageHeader.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Visible", hasData));

            Bands.AddRange(new Band[]
            {
                topMargin,
                bottomMargin,
                reportHeader,
                pageHeader,
                detail,
                reportFooter
            });
        }

        private static XRLabel MakeLabel(float x, float y, float w, float h, string text)
        {
            return new XRLabel
            {
                Text = text,
                LocationFloat = new DevExpress.Utils.PointFloat(x, y),
                SizeF = new SizeF(w, h),
                Font = new Font("Segoe UI", 10f)
            };
        }

        private static XRLabel MakeExprLabel(float x, float y, float w, float h, string expression)
        {
            var label = new XRLabel
            {
                LocationFloat = new DevExpress.Utils.PointFloat(x, y),
                SizeF = new SizeF(w, h),
                Font = new Font("Segoe UI", 10f),
                CanGrow = true
            };
            label.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", expression));
            return label;
        }
        private static XRLabel MakeCell(float x, float y, float w, float h, string text)
        {
            var label = new XRLabel
            {
                LocationFloat = new DevExpress.Utils.PointFloat(x, y),
                SizeF = new SizeF(w, h),
                Font = new Font("Segoe UI", 10f),
                CanGrow = true,
                Borders = DevExpress.XtraPrinting.BorderSide.All,
                BorderWidth = 1,
                BorderColor = Color.FromArgb(180, 180, 180)
            };
            if (text != null)
                label.Text = text;
            return label;
        }
    }
}