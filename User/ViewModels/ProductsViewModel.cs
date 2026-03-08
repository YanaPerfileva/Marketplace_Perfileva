using Marketplace.DAL.Interfaces;
using Marketplace.Data.Dto;
using Marketplace.Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace User.ViewModels
{
    public class ProductsViewModel : BindableBase
    {
        private readonly IProductRepository _productRepo;

        public ObservableCollection<ProductDto> Products { get; } = new();

        public ObservableCollection<Category> Categories { get; } = new();

        private Category? _selectedCategory;
        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set { if (SetProperty(ref _selectedCategory, value)) ResetAndLoad(); }
        }

        private string _sortBy = "CreatedAt";
        public string SortBy
        {
            get => _sortBy;
            set { if (SetProperty(ref _sortBy, value)) ResetAndLoad(); }
        }

        private string _searchText = string.Empty;
        private decimal? _minPrice;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private bool _isLoading;
        private const int _pageSize = 9;

        public ProductsViewModel(IProductRepository productRepo)
        {
            _productRepo = productRepo;
            _ = LoadInitialDataAsync();
        }

        #region Свойства (Properties)
        public string SearchText
        {
            get => _searchText;
            set { if (SetProperty(ref _searchText, value)) ResetAndLoad(); }
        }

        public decimal? MinPrice
        {
            get => _minPrice;
            set { if (SetProperty(ref _minPrice, value)) ResetAndLoad(); }
        }

        public int CurrentPage { get => _currentPage; set => SetProperty(ref _currentPage, value); }
        public int TotalPages { get => _totalPages; set => SetProperty(ref _totalPages, value); }
        public bool IsLoading { get => _isLoading; set => SetProperty(ref _isLoading, value); }
        #endregion

        #region Команды (Commands)
        public ICommand NextPageCommand => new DelegateCommand(async () =>
        {
            if (CurrentPage < TotalPages) { CurrentPage++; await LoadProductsAsync(); }
        }, () => CurrentPage < TotalPages && !IsLoading);

        public ICommand PrevPageCommand => new DelegateCommand(async () =>
        {
            if (CurrentPage > 1) { CurrentPage--; await LoadProductsAsync(); }
        }, () => CurrentPage > 1 && !IsLoading);
        #endregion

        #region Методы (Methods)
        private void ResetAndLoad()
        {
            CurrentPage = 1;
            _ = LoadProductsAsync();
        }

        private async Task LoadInitialDataAsync()
        {
            IsLoading = true;
            try
            {
                // 1. Грузим категории
                var cats = await _productRepo.GetAllCategoriesAsync();

                Categories.Clear();
                // Добавляем пустую категорию, чтобы можно было сбросить фильтр
                Categories.Add(new Category { Id = 0, Name = "Все категории" });

                foreach (var cat in cats)
                    Categories.Add(cat);

                // Устанавливаем "Все категории" по умолчанию
                SelectedCategory = Categories[0];

                // 2. Только после категорий грузим товары
                await LoadProductsAsync();
            }
            finally { IsLoading = false; }
        }

        public async Task LoadProductsAsync()
        {
            IsLoading = true;
            try
            {
                string dbSortField = "CreatedAt";
                bool isAscending = false;

                switch (SortBy)
                {
                    case "PriceAsc":
                        dbSortField = "BasePrice"; isAscending = true; break;
                    case "PriceDesc":
                        dbSortField = "BasePrice"; isAscending = false; break;
                    case "NameAsc":
                        dbSortField = "Name"; isAscending = true; break;
                    case "CreatedAtDesc":
                        dbSortField = "CreatedAt"; isAscending = false; break;
                }

                var result = await _productRepo.GetPaginatedProductsAsync(
            search: SearchText,
            categoryId: SelectedCategory?.Id == 0 ? null : SelectedCategory?.Id,
            minPrice: MinPrice,
            page: CurrentPage,
            pageSize: _pageSize,
            sortBy: dbSortField,  
            ascending: isAscending 
                );

                Products.Clear();
                if (result?.Items != null)
                {
                    foreach (var item in result.Items) Products.Add(item);
                    TotalPages = (int)Math.Ceiling((double)result.TotalCount / _pageSize);
                }

                if (TotalPages == 0) TotalPages = 1;
            }
            finally
            {
                IsLoading = false;
                (NextPageCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                (PrevPageCommand as DelegateCommand)?.RaiseCanExecuteChanged();
            }
        }
        #endregion
    }
}


