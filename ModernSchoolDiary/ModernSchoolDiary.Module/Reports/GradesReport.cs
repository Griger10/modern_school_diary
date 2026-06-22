using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.Base.ReportsV2;
using DevExpress.XtraReports.UI;
using System.Drawing;

namespace ModernSchoolDiary.Module.Reports
{
    public partial class GradesReport : XtraReport
    {
        private const string F_StudentFullName = "[Student.FullName]";
        private const string F_StudentClass = "[Student.SchoolClass.Name]";
        private const string F_Period = "[Period.Name]";
        private const string F_SubjectTitle = "[Subject.Title]";
        private const string F_GradeValue = "[Value]";
        private const string F_GradeDate = "[Date]";
        private const string F_GradeComment = "[Comment]";

        public GradesReport()
        {
            InitializeComponent();

            if (ModuleHelper.IsDesignMode)
                return;

            var dataSource = new CollectionDataSource
            {
                ObjectTypeName = "ModernSchoolDiary.Module.Domain.Models.Grade"
            };
            DataSource = dataSource;
            DataMember = string.Empty;
            var periodParam = new DevExpress.XtraReports.Parameters.Parameter
            {
                Name = "PeriodParam",
                Description = "Учебный период:",
                Type = typeof(Guid),
                Visible = true
            };
            var periodLookup = new DevExpress.XtraReports.Parameters.DynamicListLookUpSettings
            {
                DataAdapter = null,
                DataSource = new CollectionDataSource
                {
                    ObjectTypeName = "ModernSchoolDiary.Module.Domain.Models.AcademicTerm"
                },
                ValueMember = "Id",
                DisplayMember = "Name"
            };
            periodParam.ValueSourceSettings = periodLookup;

            Parameters.Add(periodParam);
            RequestParameters = true;
            FilterString = "[Period.Id] = ?PeriodParam";

            Margins = new System.Drawing.Printing.Margins(50, 50, 50, 50);

            var reportHeader = new ReportHeaderBand { HeightF = 45 };
            var title = MakeLabel(0, 5, 670, 30, "Ведомость успеваемости");
            title.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
            title.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            reportHeader.Controls.Add(title);

            var ghStudent = new GroupHeaderBand { HeightF = 64, Level = 1 };
            ghStudent.GroupFields.Add(new GroupField("Student.FullName", XRColumnSortOrder.Ascending));

            var lblStudent = MakeExprLabel(0, 4, 670, 24, "'Ученик: ' + " + F_StudentFullName);
            lblStudent.Font = new Font("Segoe UI", 12f, FontStyle.Bold);

            var lblClass = MakeExprLabel(0, 32, 330, 22, "'Класс: ' + " + F_StudentClass);
            lblClass.Font = new Font("Segoe UI", 10f);

            var lblPeriod = MakeExprLabel(340, 32, 330, 22, "'Период: ' + " + F_Period);
            lblPeriod.Font = new Font("Segoe UI", 10f);

            ghStudent.Controls.AddRange(new XRControl[] { lblStudent, lblClass, lblPeriod });

            var ghSubject = new GroupHeaderBand { HeightF = 28, Level = 0 };
            ghSubject.GroupFields.Add(new GroupField("Subject.Title", XRColumnSortOrder.Ascending));

            var lblSubject = MakeExprLabel(0, 4, 670, 22, F_SubjectTitle);
            lblSubject.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblSubject.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            ghSubject.Controls.Add(lblSubject);

            var detail = new DetailBand { HeightF = 22 };

            var lblDate = MakeExprLabel(30, 0, 150, 22, F_GradeDate);
            lblDate.TextFormatString = "{0:dd.MM.yyyy}";

            var lblValue = MakeExprLabel(190, 0, 60, 22, F_GradeValue);
            lblValue.Font = new Font("Segoe UI", 10f, FontStyle.Bold);

            var lblComment = MakeExprLabel(260, 0, 410, 22, F_GradeComment);

            detail.Controls.AddRange(new XRControl[] { lblDate, lblValue, lblComment });

            detail.SortFields.Add(new GroupField("Date", XRColumnSortOrder.Ascending));

            var gfSubject = new GroupFooterBand { HeightF = 26, Level = 0 };
            var lblSubjAvgCap = MakeLabel(30, 2, 220, 22, "Средний балл по предмету:");

            var lblSubjAvg = MakeSummaryLabel(250, 2, 90, 22, "sumAvg(ToDouble(" + F_GradeValue + "))", "{0:n2}");
            lblSubjAvg.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            gfSubject.Controls.AddRange(new XRControl[] { lblSubjAvgCap, lblSubjAvg });

            var gfStudent = new GroupFooterBand { HeightF = 54, Level = 1 };
            var lblTotalCap = MakeLabel(0, 6, 240, 22, "Общий средний балл:");
            lblTotalCap.Font = new Font("Segoe UI", 11f, FontStyle.Bold);

            var lblTotalAvg = MakeSummaryLabel(250, 6, 120, 22, "sumAvg(ToDouble(" + F_GradeValue + "))", "{0:n2}");
            lblTotalAvg.Font = new Font("Segoe UI", 11f, FontStyle.Bold);

            string verbalExpression = "Iif(sumAvg(ToDouble(" + F_GradeValue + ")) < 2.67, 'Неудовлетворительно', " +
                                      "sumAvg(ToDouble(" + F_GradeValue + ")) < 3.67, 'Удовлетворительно', " +
                                      "sumAvg(ToDouble(" + F_GradeValue + ")) < 4.67, 'Хорошо', 'Отлично')";

            var lblVerbal = MakeSummaryLabel(0, 30, 670, 22, verbalExpression);
            lblVerbal.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblVerbal.Borders = DevExpress.XtraPrinting.BorderSide.Top;

            var reportFooter = new ReportFooterBand { HeightF = 40 };
            var lblNoData = MakeLabel(0, 8, 670, 24, "За выбранный период оценки отсутствуют");
            lblNoData.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            lblNoData.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            lblNoData.ForeColor = Color.Gray;
            lblNoData.ExpressionBindings.Add(new ExpressionBinding(
                "BeforePrint", "Visible", "[DataSource.RowCount] == 0"));

            reportFooter.Controls.Add(lblNoData);

            gfStudent.Controls.AddRange(new XRControl[] { lblTotalCap, lblTotalAvg, lblVerbal });

            string hasData = "[DataSource.RowCount] > 0";

            reportHeader.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Visible", hasData));
            ghStudent.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Visible", hasData));
            ghSubject.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Visible", hasData));
            detail.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Visible", hasData));
            gfSubject.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Visible", hasData));
            gfStudent.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Visible", hasData));

            Bands.AddRange(new Band[]
            {
                reportHeader,
                ghStudent,
                ghSubject,
                detail,
                gfSubject,
                gfStudent,
                reportFooter,
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

        private static XRLabel MakeSummaryLabel(float x, float y, float w, float h, string expression, string formatString = null)
        {
            var label = new XRLabel
            {
                LocationFloat = new DevExpress.Utils.PointFloat(x, y),
                SizeF = new SizeF(w, h),
                Font = new Font("Segoe UI", 10f),
                CanGrow = true
            };

            label.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", expression));
            label.Summary = new XRSummary { Running = SummaryRunning.Group };

            if (!string.IsNullOrEmpty(formatString))
                label.TextFormatString = formatString;

            return label;
        }
    }
}