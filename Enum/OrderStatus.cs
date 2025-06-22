namespace Chill_Closet.Enums
{
    public enum OrderStatus
    {
        Pending,        // Chờ xác nhận
        Confirmed,      // Đã xác nhận
        Shipping,       // Đang giao
        Completed,      // Đã giao thành công
        Cancelled,      // Đã hủy
        ReturnRequested  //Hoàn trả tiền
    }
}