using ExamTwo.Const;
using ExamTwo.DTOs;
using ExamTwo.Models;
using ExamTwo.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExamTwo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoffeeMachineController : ControllerBase
    {
        private readonly ICoffeeMachineService _coffeeMachineService;

        public CoffeeMachineController(ICoffeeMachineService coffeeMachineService)
        {
            _coffeeMachineService = coffeeMachineService;
        }

        [HttpGet("coffees")]
        public ActionResult<List<CoffeeTypeDto>> GetAvailableCoffees()
        {
            try
            {
                var coffees = _coffeeMachineService.GetAvailableCoffees();
                return Ok(coffees);
            }
            catch (Exception ex)
            {
                return StatusCode(Constants.HTTP_STATUS_INTERNAL_SERVER_ERROR, 
                    string.Format(Constants.ErrorMessages.ERROR_GET_COFFEES, ex.Message));
            }
        }

        [HttpPost("calculate-total")]
        public ActionResult<int> CalculateTotal([FromBody] Dictionary<string, int> order)
        {
            try
            {
                if (order == null || order.Count == Constants.ZERO)
                    return BadRequest(Constants.ErrorMessages.ORDER_EMPTY);

                var total = _coffeeMachineService.CalculateTotalCost(order);
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(Constants.HTTP_STATUS_INTERNAL_SERVER_ERROR, 
                    string.Format(Constants.ErrorMessages.ERROR_CALCULATE_TOTAL, ex.Message));
            }
        }

        [HttpPost("purchase")]
        public ActionResult<PurchaseResultDto> ProcessPurchase([FromBody] OrderRequestDto request)
        {
            try
            {
                if (request == null)
                    return BadRequest(Constants.ErrorMessages.REQUEST_NULL);

                if (request.Order == null || request.Order.Count == Constants.ZERO)
                    return BadRequest(Constants.ErrorMessages.ORDER_EMPTY);

                if (request.Payment == null)
                    return BadRequest(Constants.ErrorMessages.PAYMENT_NULL);

                var payment = new Payment
                {
                    TotalAmount = request.Payment.TotalAmount,
                    Coins = request.Payment.Coins ?? new List<int>(),
                    Bills = request.Payment.Bills ?? new List<int>()
                };

                var result = _coffeeMachineService.ProcessPurchase(request.Order, payment);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(Constants.HTTP_STATUS_INTERNAL_SERVER_ERROR, new PurchaseResultDto
                {
                    Success = false,
                    Message = string.Format(Constants.ErrorMessages.ERROR_PROCESS_PURCHASE, ex.Message)
                });
            }
        }
    }
}
