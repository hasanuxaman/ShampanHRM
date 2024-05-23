using System.Web.Mvc;

namespace SymWebUI.Areas.Attendance
{
    public class AttendanceAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Attendance";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Attendance_default",
                "Attendance/{controller}/{action}/{id}",
                 new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "SymWebUI.Areas.Attendance.Controllers" }
            );
        }
    }
}
