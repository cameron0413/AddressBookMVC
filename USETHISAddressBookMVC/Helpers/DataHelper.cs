using USETHISAddressBookMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace USETHISAddressBookMVC.Helpers
{
    public static class DataHelper
    {
        public static async Task ManageDataAsync(IServiceProvider svcProvider)
        {
            var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
            await dbContextSvc.Database.MigrateAsync();
        }
    }
}
