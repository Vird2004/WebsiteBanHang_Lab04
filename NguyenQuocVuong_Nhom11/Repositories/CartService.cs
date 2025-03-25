namespace NguyenQuocVuong_Nhom11.Repositories
{
    public class CartService : ICartService
    {
        private readonly List<int> _cartItems = new List<int>(); // Danh sách giả lập

        public bool RemoveItem(int productId)
        {
            if (_cartItems.Contains(productId))
            {
                _cartItems.Remove(productId);
                return true;
            }
            return false;
        }
    }

}
