using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IcecreamApp.Pages;
using IcecreamApp.Services;
using IcecreamApp.Shared.Dtos;
using Refit;

namespace IcecreamApp.ViewModels
{
    public partial class OrdersViewModel : BaseViewModel
    {
        private readonly AuthService _authService;
        private readonly IOrderApi _orderApi;

        public OrdersViewModel(AuthService authService, IOrderApi orderApi)
        {
            _authService = authService;
            _orderApi = orderApi;
        }
        [ObservableProperty]
        private OrderDto[] _orders = [];

        public async Task InitializeAsync() =>
        await LoadOrderAsync();

        [RelayCommand]
        public async Task LoadOrderAsync()
        {
            IsBusy = true;
            try
            {
                Orders = await _orderApi.GetMyOrdersAsync();
                if (Orders.Length == 0)
                {
                    await ShowToastAsync("No orders found");
                }
            }
            catch (ApiException ex)
            {

                await HandleApiExceptionAsync(ex, () => _authService.Signout());
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        private async Task GoToOrderDetailsPageAsync(long orderId)
        {
            var parameter = new Dictionary<string, object>
            {
                [nameof(OrderDetailsViewModel.OrderId)]=orderId
            };
            await GoToAsync(nameof(OrderDetailsPage),animate:true,parameter);
        }
    }
}