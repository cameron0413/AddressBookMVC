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

        public async Task AddContactToCategoryAsync(int categoryId, int contactId)
        {
            if (!await IsContactInCategory(categoryId, contactId))
            {
                Contact? contact = await _context.Contact.FindAsync(contactId);
                Category? category = await _context.Category.FindAsync(categoryId);
                if (category != null && contact != null)
                {
                    category.Contacts.Add(contact);
                    await _context.SaveChangesAsync();
                }
            }

        }

        //Homework 8/11/22 - Get a comments form going

        public async Task<IEnumerable<Category>> GetContactCategoriesAsync(int contactId)
        {
            Contact? contact = await _context.Contact.Include(c => c.Categories).FirstOrDefaultAsync(c => c.Id == contactId);

            return contact.Categories;
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

        public async Task<bool> IsContactInCategory(int categoryId, int contactId)
        {
            Contact? contact = await _context.Contact.FindAsync(contactId);
            return await _context.Category.Include(c => c.Contacts).Where(c => c.Id == categoryId && c.Contacts.Contains(contact)).AnyAsync();
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

        public async Task<IEnumerable<Contact>> SearchForContacts(string searchString, string appUserId)
        {
            searchString = searchString.ToLower();
            List<Contact> contacts = new List<Contact>();
            if (string.IsNullOrEmpty(searchString))
            {
                contacts = await _context.Contact.Where(c => c.AppUserId == appUserId).ToListAsync();
            }
            else
            {
                contacts = _context.Contact.Where(c => c.AppUserId == appUserId && c.FullName.ToLower().Contains(searchString))
                                           .OrderBy(c => c.LastName)
                                           .ThenBy(c => c.FirstName)
                                           .ToList();


                

            }
            return contacts;
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
