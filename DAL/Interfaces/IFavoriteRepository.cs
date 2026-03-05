using Marketplace.Data.Entities;
using Marketplace.DAL.Models;
using System.Threading.Tasks;
using Marketplace.Data.Dto;

namespace Marketplace.DAL.Interfaces
{
    public interface IFavoriteRepository : IGenericRepository<Favorite>
    {
        Task<Favorite?> GetFavoriteWithProductAsync(int favoriteId);
        Task<Favorite?> GetFavoriteWithUserAsync(int favoriteId);
        Task<PaginatedResult<FavoriteDto>> GetPaginatedUserFavoritesAsync(
            int userId,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            bool ascending = false);
    }
}
