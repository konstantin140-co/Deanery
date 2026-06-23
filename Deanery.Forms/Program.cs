using Deanery.Data.Context;
using Deanery.Forms.Forms;
using Microsoft.EntityFrameworkCore;

namespace Deanery.Forms;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        using var db = new AppDbContext();
        db.Database.Migrate();
        Application.Run(new MainForm());
    }
}
