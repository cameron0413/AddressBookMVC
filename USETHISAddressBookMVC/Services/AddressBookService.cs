using USETHISAddressBookMVC.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using USETHISAddressBookMVC.Models;
using USETHISAddressBookMVC.Data;

//implementation class/Service class

namespace USETHISAddressBookMVC.Services
{
    public class AddressBookService : IAddressBookService
    {

        private readonly ApplicationDbContext _context;
        public AddressBookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddContactToCategoryAsync(int categoryId, int contactId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Category>> GetContactCategoriesAsync(string appUserId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Category>> GetUserCategoriesAsync(string appUserId)
        {

            List<Category> categories = new List<Category>();

            try
            {
                categories = await _context.Category!.
                                                     Where(c => c.AppUserId == appUserId)
                                                     .OrderBy(c => c.Name)
                                                     .ToListAsync();

            }
            catch
            {
                throw;
            }
            return categories;
        }

        public async Task<IEnumerable<Contact>> GetUserContactsAsync(string appUserId)
        {
            List<Contact> contacts = new List<Contact>();

            try
            {
                contacts = await _context.Contact!
                                         .Where(c => c.AppUserId == appUserId)
                                         .OrderBy(c => c.LastName)
                                         .ThenBy(c => c.FirstName)
                                         .ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
            return contacts;
        }

        public Task<bool> IsContactInCategory(int categoryId, int contactId)
        {
            
        }

        public async Task RemoveContactFromCategoryAsync(int categoryId, int contactId)
        {
            Contact? contact = await _context.Contact.FindAsync(contactId);
            Category? category = await _context.Category.FindAsync(categoryId);

            if (category != null && contact != null)
            {
                category.Contacts.Remove(contact);
                //something goes here check the code later
            }
        }

        public IEnumerable<Contact> SearchForContacts(string searchString, string appUserId)
        {
            
        }

        async Task<IEnumerable<int>> IAddressBookService.GetContactCategoryIdsAsync(int contactId)
        {
            try
            {
                Contact contact = await _context.Contact
                                                .Include(c => c.Categories)
                                                .FirstOrDefaultAsync(c => c.Id == contactId);
                List<int> categoryIds = contact.Categories.Select(c => c.Id).ToList();
                return categoryIds;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
