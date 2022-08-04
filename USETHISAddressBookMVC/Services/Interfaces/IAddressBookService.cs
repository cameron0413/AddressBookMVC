using USETHISAddressBookMVC.Models;



namespace USETHISAddressBookMVC.Services.Interfaces
{
    public interface IAddressBookService
    {
        Task<IEnumerable<Category>> GetUserCategoriesAsync(string appUserId);

        Task<IEnumerable<Contact>> GetUserContactsAsync(string appUserId);

        Task AddContactToCategoryAsync(int categoryId, int contactId);
        Task<bool> IsContactInCategory(int categoryId, int contactId);
        Task<IEnumerable<Category>> GetContactCategoriesAsync(string appUserId);
        Task<IEnumerable<int>> GetContactCategoryIdsAsync(int contactId);
        Task RemoveContactFromCategoryAsync(int categoryId, int contactId);
        IEnumerable<Contact> SearchForContacts(string searchString, string appUserId);

     

    }
}
