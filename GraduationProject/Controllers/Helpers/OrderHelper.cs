using GraduationProject.Models;

namespace GraduationProject.Controllers.Helpers
{
    public class OrderHelper
    {
        public class OStatusModel
        {
            public string Message { get; set; }
            public bool IsSucceeded { get; set; }
            public OrderStatus newStatus { get; set; }  
        }

        public static OStatusModel changeStatus(OrderStatus currentStatus, OrderStatus reqStatus) 
        {

            switch (reqStatus)
            {
                case OrderStatus.Shipped:
                    if (currentStatus != OrderStatus.Confirmed)
                    {
                        return new OStatusModel {
                            Message = $"Order is {currentStatus.ToString()} can`t mark it shipped",
                            IsSucceeded = false
                        };
                    }
                    else { 
                        return new OStatusModel() { Message = string.Empty, IsSucceeded = true, newStatus= OrderStatus.Shipped };
                    }
                case OrderStatus.Receipted:
                    if (currentStatus == OrderStatus.Receipted)
                    {
                        return new OStatusModel{
                            Message = "Order is already Receipted",
                            IsSucceeded = false };
                        
                    }
                    if (currentStatus != OrderStatus.Shipped)
                    {
                        return new OStatusModel {
                            Message = $"Order is {currentStatus.ToString()} can`t mark it Receipted",
                            IsSucceeded = false };
                     
                    }
                    else
                    {
                        return new OStatusModel() { Message = string.Empty, IsSucceeded = true, newStatus = OrderStatus.Receipted };
                    }
                case OrderStatus.Cancelled:
                    if ((currentStatus == OrderStatus.Receipted) || (currentStatus == OrderStatus.Returned))
                    {
                        return new OStatusModel() { 
                        
                            Message = $"Order is {currentStatus.ToString()} can`t mark it Cancelled",
                            IsSucceeded = false };
                        
                    }
                    else
                    {
                        return new OStatusModel() { Message = string.Empty, IsSucceeded = true, newStatus = OrderStatus.Cancelled };
                    }
                case OrderStatus.Returned:
                    if (currentStatus != OrderStatus.Receipted)
                    {
                        return new OStatusModel(){
                            Message = $"Order is {currentStatus.ToString()} can`t mark it Returned",
                            IsSucceeded = false };
                        
                    }
                    else
                    {
                        return new OStatusModel() { Message = string.Empty, IsSucceeded = true, newStatus = OrderStatus.Returned };
                    }

                default:
                    return new OStatusModel() { Message = "Error cann`t confirm ,order is by default confirmed ",IsSucceeded = false };
            }
        }

    }
}
